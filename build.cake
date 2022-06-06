#addin nuget:https://www.nuget.org/api/v2/?package=Cake.Docker&version=0.8.5
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.DoInDirectory&version=2.0.0
// #addin nuget:https://www.nuget.org/api/v2/?package=Cake.FileHelpers

//////////////////////////////////////////////////////////////////////
// CONFIGURATION
//////////////////////////////////////////////////////////////////////

const string TESTER_SERVICE_INTEGRATION_TESTS = "integration-tester";

var PROJECTS_TO_PACK = new List<string>
{
    "NAME",
    "NAME.WebApi",
    "NAME.AspNetCore",
    "NAME.SelfHost.Kestrel",
    "NAME.SelfHost.HttpListener"
    // "NAME.Registry"
};

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Full-Build");
var configuration = Argument("configuration", "Release");
var nugetPreReleaseTag = Argument("nugetPreReleaseTag", "dev");
var dockerComposeProject = Argument("composeProject", "name-integration-tests");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var outputDir = Directory("Output/");
var artifactsDir = outputDir + Directory("Artifacts/");
var nugetPackagesDir = artifactsDir + Directory("NuGets/");
var preReleaseNugetPackagesDir = nugetPackagesDir + Directory("PreRelease/");
var releaseNugetPackagesDir = nugetPackagesDir + Directory("Release/");
var integrationTestResultsOutputDir = outputDir + Directory("IntegrationTestsResults/");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
        CleanDirectory(integrationTestResultsOutputDir);
    });

Task("Restore-NuGet-Packages")
    .Does(() =>
    {
        DotNetCoreRestore("NAME.sln");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var dotNetBuildConfig = new DotNetCoreBuildSettings() {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
        };

        dotNetBuildConfig.MSBuildSettings.TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error;

        DotNetCoreBuild("NAME.sln", dotNetBuildConfig);
    });

Task("Nuget-Pack")
    .Does(()=>{
        EnsureDirectoryExists(preReleaseNugetPackagesDir);
        EnsureDirectoryExists(releaseNugetPackagesDir);

        CleanDirectory(preReleaseNugetPackagesDir);
        CleanDirectory(releaseNugetPackagesDir);

        var preReleaseSettings = new DotNetCorePackSettings{
            Configuration = configuration,
            OutputDirectory = preReleaseNugetPackagesDir,
            VersionSuffix = nugetPreReleaseTag
        };
        var releaseSettings = new DotNetCorePackSettings{
            Configuration = configuration,
            OutputDirectory = releaseNugetPackagesDir
        };


        // https://github.com/NuGet/Home/issues/4337
        // While this issue is not fixed we need to specify the version suffix in the restore task.

        Action<DotNetCorePackSettings> packProjects = (settings) => {
            var dotnetCoreRestoreSettings = new DotNetCoreRestoreSettings();
            if (settings.VersionSuffix != null) {
                dotnetCoreRestoreSettings.EnvironmentVariables = new Dictionary<string, string>()
                {
                    { "VersionSuffix", settings.VersionSuffix }
                };
            }

            foreach(var project in PROJECTS_TO_PACK){
                var projectFolder = "./src/" + project;
                DotNetCoreRestore(projectFolder, dotnetCoreRestoreSettings);
                DotNetCorePack(projectFolder, settings);
            }
        };

        packProjects(preReleaseSettings);
        packProjects(releaseSettings);
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var files = GetFiles("./unit-tests/**/*.csproj");

        int highestExitCode = 0;

        foreach (var file in files){
            // While https://github.com/cake-build/cake/pull/1578 is not merged,
            // we need to start our own process to run dotnet xunit.
            // Related: https://github.com/cake-build/cake/issues/1577

            var processSettings = new ProcessSettings {
                Arguments = "xunit -trait \"TestCategory=\"Unit\"\"",
                WorkingDirectory = file.GetDirectory()
            };

            if (IsRunningOnUnix()) {
                var frameworks = XmlPeek(file, "/Project/PropertyGroup/TargetFrameworks/text()");
                if (frameworks == null)
                    frameworks = XmlPeek(file, "/Project/PropertyGroup/TargetFramework/text()");

                if (frameworks == null || frameworks.Contains("netcoreapp") == false) {
                    continue;
                }
                processSettings.Arguments.Append("-framework netcoreapp2.2");
            }

            var exitCode = StartProcess("dotnet", processSettings);

            if(exitCode > highestExitCode)
                highestExitCode = exitCode;
        }

        // Means there was an error
        if(highestExitCode > 0 )
            throw new Exception("Some tests failed.");
    });

Task("Run-Integration-Tests")
    .IsDependentOn("Nuget-Pack")
    .Does(() => {
        EnsureDirectoryExists(releaseNugetPackagesDir);
        EnsureDirectoryExists(integrationTestResultsOutputDir);
        System.IO.File.Create(integrationTestResultsOutputDir + File("integrationTests.trx"));

        DoInDirectory("./integration-tests/", () => {
            try
            {
                DockerComposePull();
                DockerComposeBuild(new DockerComposeBuildSettings() { ProjectName = dockerComposeProject });
                DockerComposeRun(new DockerComposeRunSettings() { ProjectName = dockerComposeProject, Rm = true}, TESTER_SERVICE_INTEGRATION_TESTS);
            }
            finally
            {
                DockerComposeKill(new DockerComposeKillSettings() { ProjectName = dockerComposeProject });
                DockerComposeRm(new DockerComposeRmSettings() { ProjectName = dockerComposeProject, Force = true , Volumes = true});
            }
        });
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////


Task("Build-AND-Test")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
    .IsDependentOn("Build-AND-Test")
    .IsDependentOn("Nuget-Pack");

Task("TravisCI")
    .IsDependentOn("Build-AND-Test")
    .Does(() => {
        if(IsRunningOnUnix()) {
            Information("Travis CI running on Linux, executing the integration tests.");
            RunTarget("Run-Integration-Tests");
        }
    });

Task("Default")
    .IsDependentOn("Build-AND-Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);