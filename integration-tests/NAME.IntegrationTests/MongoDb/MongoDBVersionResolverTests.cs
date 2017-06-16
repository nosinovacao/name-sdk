using NAME.Core;
using NAME.MongoDb;
using NAME.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests.MongoDb
{
    public class MongoDbVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_LatestVersion()
        {
            MongoDbVersionResolver resolver = new MongoDbVersionResolver(new StaticConnectionStringProvider($"mongodb://{ Constants.LatestMongoHostname }:27017"), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.True(versions.First() >= DependencyVersion.Parse("3.0.0"));
        }
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_SpecificVersion_SingleServer()
        {
            MongoDbVersionResolver resolver = new MongoDbVersionResolver(new StaticConnectionStringProvider($"mongodb://{ Constants.SpecificMongoHostname }:27017"), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificMongoVersion));
        }
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_SpecificVersion_UriWithQueryString()
        {
            MongoDbVersionResolver resolver = new MongoDbVersionResolver(new StaticConnectionStringProvider($"mongodb://{ Constants.SpecificMongoHostname }:27017/?w=1"), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificMongoVersion));
        }
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_SpecificVersion_DoubleServers()
        {
            MongoDbVersionResolver resolver = new MongoDbVersionResolver(new StaticConnectionStringProvider($"mongodb://{ Constants.SpecificMongoHostname }:27017,{ Constants.SpecificMongoHostname }/?w=1"), 10000, 10000);
           
            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(2, versions.Count());
            Assert.Equal(versions.First(), DependencyVersion.Parse(Constants.SpecificMongoVersion));
            Assert.Equal(versions.Skip(1).First(), DependencyVersion.Parse(Constants.SpecificMongoVersion));
        }
    }
}
