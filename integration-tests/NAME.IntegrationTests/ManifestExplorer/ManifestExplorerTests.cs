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
                        ""name"":""DummyConsoleKestrel"",
                        ""min_version"": ""1.0.0"",
                        ""max_version"": ""2.0.0"",
                        ""connection_string"": ""http://dummy-console-kestrel:40500""
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

                Assert.True(1 == parsedDependencies.ServiceDependencies.Count(), "The number of service dependencies did not match.");
                Assert.Empty(parsedDependencies.InfrastructureDependencies);

                var serviceDependency = parsedDependencies.ServiceDependencies.First();

                DependencyCheckStatus status = await serviceDependency.GetStatus().ConfigureAwait(false);

                // we still need to test for backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
                Assert.True(status.CheckPassed);
#pragma warning restore CS0618 // Type or member is obsolete
                Assert.Equal(NAMEStatusLevel.Ok, status.CheckStatus);
                Assert.NotNull(status.Version.ManifestNode);
                Assert.Equal("1.0.0", status.Version.ToString());
                Assert.Equal("NAME.DummyService", status.Version.ManifestNode["name"]);
                Assert.Empty(status.Version.ManifestNode["infrastructure_dependencies"].Children);
                Assert.Empty(status.Version.ManifestNode["service_dependencies"].Children);
            }
        }
    }
}