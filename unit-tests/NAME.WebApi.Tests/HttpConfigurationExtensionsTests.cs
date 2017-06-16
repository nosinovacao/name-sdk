using System;
using System.Threading.Tasks;
using System.Web.Http;
using Xunit;
using System.IO;
using System.Net;
using NAME.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;
using System.Reflection;

namespace NAME.WebApi.Tests
{
    public class HttpConfigurationExtensionsTests
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
                using (var testServer = Microsoft.Owin.Testing.TestServer.Create(appBuilder =>
                 {
                     HttpConfiguration httpConfig = new HttpConfiguration();
                     httpConfig.EnableNAME(config =>
                     {
                         config.APIName = "Test";
                         config.APIVersion = "1.0.0";
                         config.DependenciesFilePath = fileName;
                     });
                     appBuilder.UseWebApi(httpConfig);
                 }))
                {
                    var response = await testServer.HttpClient.GetAsync("/manifest");
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
                using (var testServer = Microsoft.Owin.Testing.TestServer.Create(appBuilder =>
                {
                    HttpConfiguration httpConfig = new HttpConfiguration();
                    httpConfig.EnableNAME(config =>
                    {
                        config.APIName = "Test";
                        config.APIVersion = "1.0.0";
                        config.DependenciesFilePath = fileName;
                    });
                    appBuilder.UseWebApi(httpConfig);
                }))
                {
                    var response = await testServer.HttpClient.GetAsync("/manifest");
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
