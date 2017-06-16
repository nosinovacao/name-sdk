using NAME.Core;
using NAME.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests.OperatingSystem
{
    public class OperatingSystemVersionResolverTests
    {
        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GetVersions_NoException()
        {
            IVersionResolver resolver = new OperatingSystemVersionResolver();

            var versions = await resolver.GetVersions().ConfigureAwait(false);

            Assert.Equal(1, versions.Count());
            Assert.True(versions.First() < new DependencyVersion(9999, 9999, 9999));
            Assert.True(versions.First() > new DependencyVersion(0, 0, 0));
            Assert.IsType<OperatingSystemDependencyVersion>(versions.First());
            Assert.False(string.IsNullOrEmpty(((OperatingSystemDependencyVersion)versions.First()).OperatingSystem), "The Operating System should not be null.");
            Assert.Equal(((OperatingSystemDependencyVersion)versions.First()).OperatingSystem, Constants.ExpectedOperatingSystem);
            //Assert.True(versions.First().OperatingSystem == "ubuntu" || versions.First().OperatingSystem == "windows");
        }
    }
}
