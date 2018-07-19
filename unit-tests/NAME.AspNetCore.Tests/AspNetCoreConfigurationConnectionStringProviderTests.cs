using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NAME.AspNetCore.Tests
{
    public class AspNetCoreConfigurationConnectionStringProviderTests
    {
        [Fact]
        public void TryGetConnectionString_ReturnsTrue_WhenFound()
        {
            var key = "ConnectionStrings:Service";
            var value = "http://example.com";

            var configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
            configurationMock
                .SetupGet(c => c[key])
                .Returns(value);

            var provider = new AspNetCoreConfigurationConnectionStringProvider(configurationMock.Object, key);
            var found = provider.TryGetConnectionString(out string connectionString);

            Assert.True(found);
            Assert.Equal(value, connectionString);
        }

        [Fact]
        public void TryGetConnectionString_ReturnsFalse_WhenNull()
        {
            var key = "ConnectionStrings:Service";
            string value = null;

            var configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
            configurationMock
                .SetupGet(c => c[key])
                .Returns(value);

            var provider = new AspNetCoreConfigurationConnectionStringProvider(configurationMock.Object, key);
            var found = provider.TryGetConnectionString(out string connectionString);

            Assert.False(found);
            Assert.Null(connectionString);
        }

        [Fact]
        public void TryGetConnectionString_ReturnsFalse_WhenException()
        {
            var key = "ConnectionStrings:Service";

            var configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
            configurationMock
                .SetupGet(c => c[key])
                .Throws<KeyNotFoundException>();

            var provider = new AspNetCoreConfigurationConnectionStringProvider(configurationMock.Object, key);
            var found = provider.TryGetConnectionString(out string connectionString);

            Assert.False(found);
            Assert.Null(connectionString);
        }
    }
}
