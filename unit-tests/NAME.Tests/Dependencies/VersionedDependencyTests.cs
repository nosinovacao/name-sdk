using NAME.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAME.Core;
using Xunit;
using NAME.Core.Utils;

namespace NAME.Tests.Dependencies
{
    public class VersionedDependencyTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetStatus_SupportedVersion()
        {
            var resolver = new StaticVersionResolverMock(new[] { new DependencyVersion(1, 0, 0) });
            var dep = new ImplementedVersionedDependency(resolver)
            {
                MinimumVersion = new DependencyVersion(1),
                MaximumVersion = new DependencyVersion(2)
            };
            var status = await dep.GetStatus();

            Assert.Equal(new DependencyVersion(1), status.Version);
            // Support backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.True(status.CheckPassed);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Equal(NAMEStatusLevel.Ok, status.CheckStatus);
            Assert.Null(status.InnerException);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetStatus_UnsupportedVersion()
        {
            var resolver = new StaticVersionResolverMock(new[] { new DependencyVersion(3, 0, 0) });
            var dep = new ImplementedVersionedDependency(resolver)
            {
                MinimumVersion = new DependencyVersion(1),
                MaximumVersion = new DependencyVersion(2)
            };
            var status = await dep.GetStatus();

            Assert.Equal(new DependencyVersion(3), status.Version);
            // Support backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.False(status.CheckPassed);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Equal(NAMEStatusLevel.Error, status.CheckStatus);
            Assert.Null(status.InnerException);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GetStatus_VersionsNotReturned()
        {
            var resolver = new StaticVersionResolverMock(new DependencyVersion[0]);
            var dep = new ImplementedVersionedDependency(resolver)
            {
                MinimumVersion = new DependencyVersion(1),
                MaximumVersion = new DependencyVersion(2)
            };
            var status = await dep.GetStatus();

            Assert.Null(status.Version);
            // Support backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.False(status.CheckPassed);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Equal(NAMEStatusLevel.Warn, status.CheckStatus);
            Assert.Null(status.InnerException);
        }

        internal class ImplementedVersionedDependency : VersionedDependency
        {
            public ImplementedVersionedDependency(IVersionResolver versionResolver)
                : base(versionResolver)
            {
            }
        }
    }
}
