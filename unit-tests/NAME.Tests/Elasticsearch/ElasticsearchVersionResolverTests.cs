using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Elasticsearch;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.Elasticsearch
{
    public class ElasticsearchVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_ConnectionStringNotFound()
        {
            IVersionResolver resolver = new ElasticsearchVersionResolver(new DummyNotFoundConnectionStringProvider(), 10, 10);

            await Assert.ThrowsAsync<ConnectionStringNotFoundException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_DependencyNotReachable()
        {
            IVersionResolver resolver = new ElasticsearchVersionResolver(new StaticConnectionStringProvider(""), 10, 10);

            await Assert.ThrowsAsync<DependencyNotReachableException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }
    }
}