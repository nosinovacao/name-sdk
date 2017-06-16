using NAME.Core;
using NAME.Core.Exceptions;
using NAME.MongoDb;
using NAME.SqlServer;
using NAME.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests.SqlServer
{
    public class SqlServerVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_SingleServer()
        {
            string connectionString = $"Data Source={ Constants.LatestSqlServerHostname };Initial Catalog=DBTests;Integrated Security=True;";
            IVersionResolver resolver = new SqlServerVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.True(versions.First() >= DependencyVersion.Parse("12.0.0"), "The version should be greater than 12.0.0");
        }
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_SpecificPort()
        {
            string connectionString = $"Data Source={ Constants.LatestSqlServerHostname },1433;Initial Catalog=DBTests;Integrated Security=True;";
            IVersionResolver resolver = new SqlServerVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.True(versions.First() >= DependencyVersion.Parse("12.0.0"), "The version should be greater than 12.0.0");
        }
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_EntityFrameworkConnectionString()
        {
            string connectionString = $"metadata = res://*/ProfilePreferences.csdl|res://*/ProfilePreferences.ssdl|res://*/ProfilePreferences.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source={ Constants.LatestSqlServerHostname },1433;initial catalog=SomeDb;user id=user;password=pass;MultipleActiveResultSets=True;App=EntityFramework\"";
            IVersionResolver resolver = new SqlServerVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
        }
    }
}
