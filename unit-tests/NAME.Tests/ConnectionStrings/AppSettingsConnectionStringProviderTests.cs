#if NET452
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NAME.Core;
using NAME.ConnectionStrings;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;

namespace NAME.Tests.ConnectionStrings
{
    public class AppSettingsConnectionStringProviderTests
    {

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString()
        {
            string key = "TheAllMightyKey";
            string connString = "mongodb://some-mongo-instance:27017/some-db";
            ConfigurationManager.AppSettings.SetWritable()[key] = connString;
            IConnectionStringProvider provider = new AppSettingsConnectionStringProvider(key);
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.True(result, "Unable to get the connection string.");
            Assert.Equal(outConnection, connString);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString_NotFound()
        {
            string key = "TheSecondAllMightyKey";
            string connString = "mongodb://some-mongo-instance:27017/some-db";
            ConfigurationManager.AppSettings.SetWritable()[key + "Thou shall not find it!"] = connString;
            IConnectionStringProvider provider = new AppSettingsConnectionStringProvider(key);
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The connection string should not be found.");
            Assert.Null(outConnection);
        }
    }
}
#endif