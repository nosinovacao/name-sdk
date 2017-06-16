using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Dependencies;
using NAME.Service;
using Xunit;

namespace NAME.IntegrationTests.ManifestExplorer
{
    public class ManifestExplorerTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task TestExplorer()
        {
            const string manifest = @"{
                    ""infrastructure_dependencies"": [  ],
                    ""service_dependencies"": [
                    {
                        ""name"":""DummyService2"",
                        ""min_version"": ""1.0.0"",
                        ""max_version"": ""2.0.0"",
                        ""connection_string"": ""http://dummy-service:5000""
                    }
                ]
            }";

            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteAsync(manifest).ConfigureAwait(false);
                await streamWriter.FlushAsync().ConfigureAwait(false);
                memoryStream.Seek(0, SeekOrigin.Begin);

                ParsedDependencies parsedDependencies = DependenciesReader.ReadDependencies(memoryStream, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());

                Assert.True(parsedDependencies.ServiceDependencies.Any());
                Assert.False(parsedDependencies.InfrastructureDependencies.Any());

                foreach (Dependency dependency in parsedDependencies.ServiceDependencies)
                {
                    DependencyCheckStatus status = await dependency.GetStatus().ConfigureAwait(false);
                    Assert.True(status.CheckPassed);
                    Assert.NotNull(status.Version.ManifestNode);
                    Assert.Equal("NAME.DummyService", status.Version.ManifestNode["name"]);
                    ////Assert.NotEmpty(status.Version.ManifestNode["infrastructure_dependencies"].Children);
                    Assert.NotEmpty(status.Version.ManifestNode["service_dependencies"].Children);
                }
            }
        }
    }
}