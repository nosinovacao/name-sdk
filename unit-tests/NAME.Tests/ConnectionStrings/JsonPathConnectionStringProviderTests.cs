using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NAME.Core;
using NAME.ConnectionStrings;
using System.Reflection;
using System.Xml;
using System.IO;

namespace NAME.Tests.ConnectionStrings
{
    public class JsonPathConnectionStringProviderTests : IDisposable
    {
        private string CONFIGURATION_FILE = Guid.NewGuid() + ".json";
        private const string CONFIGURATION_CONTENTS =
            @"{
                ""ConnectionStrings"": {
                    ""MongoConnection"": ""mongodb://some-mongo:27017/some-db""
                }
            }";

        public JsonPathConnectionStringProviderTests()
        {
            if (File.Exists(CONFIGURATION_FILE))
                File.Delete(CONFIGURATION_FILE);
            File.WriteAllText(CONFIGURATION_FILE, CONFIGURATION_CONTENTS);
        }
        public void Dispose()
        {
            File.Delete(CONFIGURATION_FILE);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString()
        {
            IConnectionStringProvider provider = new JsonPathConnectionStringProvider(CONFIGURATION_FILE, "$.ConnectionStrings.MongoConnection");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.True(result, "The connection string was not found!");
            Assert.Equal("mongodb://some-mongo:27017/some-db", outConnection);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void OnceUponALovelyDayWhenAttemptingToInvokeTryGetConnectionStringIWasExpectingTheKeyToNotBeFound()
        {
            IConnectionStringProvider provider = new JsonPathConnectionStringProvider(CONFIGURATION_FILE, "$.ConnectionStrings.MongoConnection" + "Thou shall not find the key!");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The connection string should not be found.");
            Assert.Null(outConnection);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString_FileNotFound()
        {
            IConnectionStringProvider provider = new JsonPathConnectionStringProvider(CONFIGURATION_FILE + "Thou shall not find the file!", "$.ConnectionStrings.MongoConnection");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The file should not be found.");
            Assert.Null(outConnection);
        }

    }
}