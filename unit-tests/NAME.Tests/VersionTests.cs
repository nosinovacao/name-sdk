using NAME.Core;
using NAME.Core.Exceptions;
using Xunit;

namespace NAME.Tests
{
    public class VersionTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_ValidVersion()
        {
            string version = "3.1.1";

            var parsedVersion = Core.DependencyVersion.Parse(version);

            Assert.Equal(3, (int)parsedVersion.Major);
            Assert.Equal(1, (int)parsedVersion.Minor);
            Assert.Equal(1, (int)parsedVersion.Patch);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_ValidVersion_MissingValues()
        {
            string version = "3";

            var parsedVersion = Core.DependencyVersion.Parse(version);

            Assert.Equal(3, (int)parsedVersion.Major);
            Assert.Equal(0, (int)parsedVersion.Minor);
            Assert.Equal(0, (int)parsedVersion.Patch);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_Operators()
        {
            var version = new DependencyVersion(3);

            Assert.True(version > new DependencyVersion(2));
            Assert.True(version < new DependencyVersion(4));
            Assert.True(version == new DependencyVersion(3));
            Assert.True(version <= new DependencyVersion(4));
            Assert.True(version <= new DependencyVersion(3));
            Assert.True(version >= new DependencyVersion(2));
            Assert.True(version >= new DependencyVersion(3));

            //Assert.Equal(3, (int)parsedVersion.Major);
            //Assert.Equal(0, (int)parsedVersion.Minor);
            //Assert.Equal(0, (int)parsedVersion.Patch);
        }

        [Theory]
        [Trait("TestCategory", "Unit")]
        [InlineData("3.")]
        [InlineData("3.1.")]
        [InlineData("3.1.1.")]
        [InlineData("3-2-1")]
        [InlineData("3.2-1")]
        [InlineData("sd")]
        [InlineData("1.213.sa")]
        [InlineData("sd.asd12.as")]
        public void Parse_InvalidVersions(string version)
        {
            Assert.Throws<VersionParsingException>(() =>
            {
                DependencyVersion.Parse(version);
            });
        }
    }
}
