using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests
{
    public class DependenciesCheckerTests
    {

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task CheckDependencies()
        {
            string contents =
            @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    {
                        ""type"": ""MongoDb"",
                        ""min_version"": ""2.6"",
                        ""max_version"": """ + Constants.SpecificMongoVersion + @""",
                        ""connection_string"": ""mongodb://" + Constants.SpecificMongoHostname + @":27017/nPVR_Dev_TST""
                    }
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, contents);
            try
            {
                var parsedDependencies = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());
                await DependenciesExtensions.CheckDependencies(parsedDependencies);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task CheckDependencies_ConnectionStringFromOverride()
        {
            string contents =
            @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    {
                        ""type"": ""MongoDb"",
                        ""min_version"": ""2.6"",
                        ""max_version"": """ + Constants.SpecificMongoVersion + @""",
                        ""connection_string"": {
                            ""locator"": ""hard-coded""
                        }
                    }
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, contents);
            try
            {
                var settings = new NAMESettings()
                {
                    ConnectionStringProviderOverride = (node) =>
                    {
                        if (node["locator"].Value == "hard-coded")
                            return new StaticConnectionStringProvider("mongodb://" + Constants.SpecificMongoHostname + ":27017/nPVR_Dev_TST");
                        return null;
                    }
                };
                var parsedDependencies = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), settings, new Core.NAMEContext());
                await DependenciesExtensions.CheckDependencies(parsedDependencies);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task CheckDependencies_Fail()
        {
            string contents =
            @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    {
                        ""type"": ""MongoDb"",
                        ""min_version"": ""1.0"",
                        ""max_version"": ""2.0"",
                        ""connection_string"": ""mongodb://" + Constants.LatestMongoHostname + @":27017/nPVR_Dev_TST""
                    }
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, contents);
            try
            {
                var exception = await Assert.ThrowsAsync<DependenciesCheckException>(async () =>
                {
                    var parsedDependencies = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());
                    await DependenciesExtensions.CheckDependencies(parsedDependencies);
                });

                Assert.Equal(1, exception.DependenciesStatutes.Count());
                // we still need to test for backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
                Assert.False(exception.DependenciesStatutes.First().CheckPassed);
#pragma warning restore CS0618 // Type or member is obsolete
                Assert.Equal(NAMEStatusLevel.Error, exception.DependenciesStatutes.First().CheckStatus);
                Assert.Equal(string.Join(Environment.NewLine, exception.DependenciesStatutes.Select(s => s.Message)), exception.Message);
                Assert.NotNull(exception.DependenciesStatutes.First().Message);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetDependenciesStatutes_Fail()
        {
            string contents =
            @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    {
                        ""type"": ""MongoDb"",
                        ""min_version"": ""1.0"",
                        ""max_version"": ""2.0"",
                        ""connection_string"": ""mongodb://" + Constants.LatestMongoHostname + @":27017/nPVR_Dev_TST""
                    }
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, contents);
            try
            {
                var parsedDependencies = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());
                var statuses = await DependenciesExtensions.GetDependenciesStatutes(parsedDependencies);

                Assert.Equal(1, statuses.Count());
                // we still need to test for backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
                Assert.False(statuses.First().CheckPassed);
#pragma warning restore CS0618 // Type or member is obsolete
                Assert.Equal(NAMEStatusLevel.Error, statuses.First().CheckStatus);
                Assert.NotNull(statuses.First().Message);
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
