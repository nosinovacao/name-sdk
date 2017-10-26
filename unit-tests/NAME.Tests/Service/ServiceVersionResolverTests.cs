using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.Service
{
    public class ServiceVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_NoEndpointListening()
        {
            string connectionString = "http://gibberish/nPVR";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            await Assert.ThrowsAsync(typeof(DependencyNotReachableException), async () =>
            {
                await resolver.GetVersions().ConfigureAwait(false);
            });
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_NotFoundListening()
        {
            string connectionString = "http://localhost:65534/NonExistentService";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            await Assert.ThrowsAsync(typeof(DependencyNotReachableException), async () =>
            {
                await resolver.GetVersions().ConfigureAwait(false);
            });
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_ConnectionStringNotFound()
        {
            IVersionResolver resolver = new ServiceVersionResolver(new DummyNotFoundConnectionStringProvider(), 0, 5, 10000, 10000);

            await Assert.ThrowsAsync<ConnectionStringNotFoundException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }
    }
}
