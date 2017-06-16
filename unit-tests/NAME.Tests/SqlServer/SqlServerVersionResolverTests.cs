using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.MongoDb;
using NAME.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.SqlServer
{
    public class SqlServerVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_WrongPort()
        {
            string connectionString = "metadata = res://*/ProfilePreferences.csdl|res://*/ProfilePreferences.ssdl|res://*/ProfilePreferences.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=10.149.194.64,65535;initial catalog=NosProfilePreferencesManagerV2_TST;user id=userProfileSettingsDB;password=userProfileSettings;MultipleActiveResultSets=True;App=EntityFramework\"";
            IVersionResolver resolver = new SqlServerVersionResolver(new StaticConnectionStringProvider(connectionString), 10000, 10000);

            await Assert.ThrowsAsync<DependencyNotReachableException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_ConnectionStringNotFound()
        {   IVersionResolver resolver = new SqlServerVersionResolver(new DummyNotFoundConnectionStringProvider(), 10000, 10000);

            await Assert.ThrowsAsync<ConnectionStringNotFoundException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }
    }
}
