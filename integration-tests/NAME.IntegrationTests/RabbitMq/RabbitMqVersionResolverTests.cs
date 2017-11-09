using NAME.Core;
using NAME.MongoDb;
using NAME.RabbitMq;
using NAME.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NAME.Core.Utils;

namespace NAME.IntegrationTests.RabbitMq
{
    public class RabbitMqVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_LatestVersion()
        {
            string connectionString = $"amqp://gibberish:notneeded@{ Constants.LatestRabbitHostname }:5672/";
            IVersionResolver resolver = new RabbitMqVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.True(versions.First() >= DependencyVersionParser.Parse("3.6.5", false));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_SpecificVersion()
        {
            string connectionString = $"amqp://gibberish:notneeded@{ Constants.SpecificRabbitHostname }:5672/";
            IVersionResolver resolver = new RabbitMqVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.Equal(versions.First(), DependencyVersionParser.Parse(Constants.SpecificRabbitVersion, false));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_MultipleConnectionStrings()
        {
            string connectionString = $"amqp://gibberish:notneeded@{ Constants.SpecificRabbitHostname }:5672/, amqp://gibberish:notneeded@{ Constants.LatestRabbitHostname }:5672/";
            IVersionResolver resolver = new RabbitMqVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(2, versions.Count());
            Assert.Equal(versions.First(), DependencyVersionParser.Parse(Constants.SpecificRabbitVersion, false));
        }
    }
}
