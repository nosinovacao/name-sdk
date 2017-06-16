using NAME.Core;
using NAME.Service;
using NAME.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests.Service
{
    public class ServiceVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions()
        {
            string connectionString = $"http://{ Constants.SpecificServiceHostname }:5000";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificServiceVersion));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_WrongEndpoint()
        {
            string connectionString = $"http://{ Constants.SpecificServiceHostname }:5000/api/v1";
            IVersionResolver resolver = new ServiceVersionResolver(new StaticConnectionStringProvider(connectionString), 0, 5, 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificServiceVersion));
        }
    }
}
