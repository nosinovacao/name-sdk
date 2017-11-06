using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Service;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests.Service
{
    public class SelfHostKestrelServiceVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions()
        {
            string connectionString = $"http://{ Constants.SpecificKestrelSelfHostHostname }:40500";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificKestrelSelfHostVersion));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_WrongEndpoint()
        {
            string connectionString = $"http://{ Constants.SpecificKestrelSelfHostHostname }:40500/api/v1";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificKestrelSelfHostVersion));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_WrongEndpoint_ReturnedOk()
        {
            string connectionString = $"http://{ Constants.SpecificKestrelSelfHostHostname }:40500/not/the/real";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificKestrelSelfHostVersion));
        }
        // /endpoint/before/name/middleware

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_EndpointWithoutNAMEInstalled()
        {
            string connectionString = $"http://{ Constants.SpecificKestrelSelfHostHostname }:40500/endpoint/before/name/middleware";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            await Assert.ThrowsAsync<DependencyWithoutNAMEException>(async () =>
            {
                await resolver.GetVersions().ConfigureAwait(false);
            });
        }
    }
}
