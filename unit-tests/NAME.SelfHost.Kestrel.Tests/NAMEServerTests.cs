using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NAME.SelfHost.Kestrel;
using Xunit;
using System.IO;
using System.Net.Http;
using System.Net;
using NAME.SelfHost.Shared;

namespace NAME.SelfHost.Kestrel.Tests
{
    public class NAMEServerTests
    {

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task ManifestUI_ReturnsRelativeURL()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\" + Guid.NewGuid().ToString() + ".json";

            SelfHostResult server = null;
            HttpClient client = new HttpClient();

            try
            {
                File.WriteAllText(fileName, "{ }");
                server = NAMEServer.EnableName((config) =>
                {
                    config.LogHealthCheckToConsole = false;
                    config.APIName = "teste";
                    config.APIVersion = "1.0.0";
                    config.DependenciesFilePath = fileName;
                });

                var response = await client.GetAsync($"http://localhost:{server.ServerPort}/manifest/ui");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.Contains("loadfromURL(\"/manifest\");", await response.Content.ReadAsStringAsync());
            }
            finally
            {
                server?.Dispose();
                client.Dispose();
                File.Delete(fileName);
            }
        }
    }
}
