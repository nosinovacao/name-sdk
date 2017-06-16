using NAME.Core;
using NAME.Core.Exceptions;
using NAME.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.RabbitMq
{
    public class RabbitMqVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_ConnectionStringNotFound()
        {
            IVersionResolver resolver = new RabbitMqVersionResolver(new DummyNotFoundConnectionStringProvider(), 10000, 10000);

            await Assert.ThrowsAsync<ConnectionStringNotFoundException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }
    }
}
