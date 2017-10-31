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
        public async Task GetVersions_SpecificVersion()
        {
            ElasticsearchVersionResolver resolver = new ElasticsearchVersionResolver(new StaticConnectionStringProvider($"http://{ Constants.SpecificElasticsearchHostname }:9200"), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificElasticsearchVersion));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_LatestVersion()
        {
            ElasticsearchVersionResolver resolver = new ElasticsearchVersionResolver(new StaticConnectionStringProvider($"http://{ Constants.LatestElasticsearchHostname }:9200"), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            // Latest GA RELEASE: https://www.elastic.co/downloads/elasticsearch#ga-release
            Assert.Equal(versions.First(), DependencyVersion.Parse("5.6.3"));
        }
    }
}