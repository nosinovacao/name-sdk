using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Elasticsearch;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests.Elasticsearch
{
    public class ElasticsearchVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_CurrentUsedVersion()
        {
            ElasticsearchVersionResolver resolver = new ElasticsearchVersionResolver(new StaticConnectionStringProvider("http://es.use.default.docker.zondev.lab:9200/"), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.True(versions.First() >= DependencyVersion.Parse("5.5.1"));
        }
    }
}