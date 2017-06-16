using NAME.Core;
using NAME.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.Dependencies
{
    public class ConnectedDependencyTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task WithoutShowingConnectionString()
        {
            var versionResolver = new StaticVersionResolverMock(new[] { new DependencyVersion(1, 0, 0) });
            ConnectedDependency dependency = new ConnectedDependency(versionResolver)
            {
                MinimumVersion = new DependencyVersion(0, 0, 1),
                MaximumVersion = new DependencyVersion(9, 9, 9),
                Name = "Test",
                ShowConnectionStringInJson = false,
                Type = SupportedDependencies.Service
            };
            var result = await dependency.ToJson();

            Assert.Null(result["value"]);
        }
    }
}
