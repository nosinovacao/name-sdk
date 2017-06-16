using NAME.Core;
using NAME.Core.Exceptions;
using NAME.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.MongoDb
{
    public class MongoDbVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetVersions_ConnectionStringNotFound()
        {
            IVersionResolver resolver = new MongoDbVersionResolver(new DummyNotFoundConnectionStringProvider(),10,10);

            await Assert.ThrowsAsync<ConnectionStringNotFoundException>(async () =>
            {
                var value = await resolver.GetVersions().ConfigureAwait(false);
            });
        }
    }
}
