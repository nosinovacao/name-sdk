using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.Net;
using NAME.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using NAME.AspNetCore;
using System.Reflection;

namespace NAME.AspNetCore.Tests
{
    public class ApplicationBuilderExtensionsTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task CorrectManifest()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\" + Guid.NewGuid().ToString() + ".json";
            try
            {
                var dependenciesValue = @"{
                    ""infrastructure_dependencies"": [
                    ],
                    ""service_dependencies"": [
                    ]
                }";
                File.WriteAllText(fileName, dependenciesValue);

                using (var testServer = new TestServer(new WebHostBuilder()
                    .Configure(appBuilder =>
                    {
                        appBuilder.UseNAME(config =>
                        {
                            config.APIName = "Test";
                            config.APIVersion = "1.0.0";
                            config.DependenciesFilePath = fileName;
                        });
                    })))
                {
                    var response = await testServer.CreateClient().GetAsync("/manifest");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var manifestJson = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    Assert.Equal("Test", manifestJson["name"]);
                    Assert.Equal("1.0.0", manifestJson["version"]);
                    Assert.Equal(Constants.NAME_ASSEMBLY_VERSION, manifestJson["nameVersion"]);
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task CorrectManifest_NameDisabled()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\" + Guid.NewGuid().ToString() + ".json";
            try
            {
                var dependenciesValue = @"{
                    ""Overrides"": {
                        ""RunningMode"": ""NAMEDisabled""
                    },
                    ""infrastructure_dependencies"": [
                    ],
                    ""service_dependencies"": [
                    ]
                }";
                File.WriteAllText(fileName, dependenciesValue);

                using (var testServer = new TestServer(new WebHostBuilder()
                    .Configure(appBuilder =>
                    {
                        appBuilder.UseNAME(config =>
                        {
                            config.APIName = "Test";
                            config.APIVersion = "1.0.0";
                            config.DependenciesFilePath = fileName;
                        });
                    })))
                {
                    var response = await testServer.CreateClient().GetAsync("/manifest");
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task ManifestUI_ReturnsRelativeURL()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\" + Guid.NewGuid().ToString() + ".json";
            try
            {
                var dependenciesValue = @"{
                    ""infrastructure_dependencies"": [
                    ],
                    ""service_dependencies"": [
                    ]
                }";
                File.WriteAllText(fileName, dependenciesValue);

                using (var testServer = new TestServer(new WebHostBuilder()
                    .Configure(appBuilder =>
                    {
                        appBuilder.UseNAME(config =>
                        {
                            config.APIName = "Test";
                            config.APIVersion = "1.0.0";
                            config.DependenciesFilePath = fileName;
                        });
                    })))
                {
                    var response = await testServer.CreateClient().GetAsync("/manifest/ui");

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    Assert.Contains("loadfromURL(\"/manifest\");", await response.Content.ReadAsStringAsync());
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
