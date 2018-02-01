using System;
using Xunit;
using NAME.Core;
using NAME.ConnectionStrings;
 

namespace NAME.Tests.ConnectionStrings
{
    public class EnvironmentVariableConnectionStringProviderTests
    {

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString()
        {
            string key = "ConnectionStrings_Accounts-MongoDb";
            string connString = "mongodb://some-mongo-instance:27017/some-db";
            Environment.SetEnvironmentVariable(key, connString);
            IConnectionStringProvider provider = new EnvironmentVariableConnectionStringProvider(key);
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.True(result, "Environment variable not found");
            Assert.Equal(outConnection, connString);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString_NotFound()
        {
            string key = "ConnectionStrings_Accounts-MongoDb_nonexistant";
             IConnectionStringProvider provider = new EnvironmentVariableConnectionStringProvider(key);
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The connection string should not be found.");
            Assert.Null(outConnection);
        }
    }
}
