using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Core.Utils;
using Xunit;

namespace NAME.Tests
{
    public class DependencyVersionParserTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_ValidVersion()
        {
            string version = "3.1.1";

            var parsedVersion = DependencyVersionParser.Parse(version, true);

            Assert.Equal(3, (int)parsedVersion.Major);
            Assert.Equal(1, (int)parsedVersion.Minor);
            Assert.Equal(1, (int)parsedVersion.Patch);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_ValidVersion_MissingValues()
        {
            string version = "3";

            var parsedVersion = DependencyVersionParser.Parse(version, true);

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
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_ToString()
        {
            var version = new DependencyVersion(1, 2, 3);

            Assert.Equal("1.2.3", version.ToString());
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_MajorWildcard()
        {
            string version = "*";

            var parsedVersion = DependencyVersionParser.Parse(version, true);

            var castedVersion = Assert.IsAssignableFrom<WildcardDependencyVersion>(parsedVersion);
            Assert.True(castedVersion.IsMajorWildcard, "Major should be marked as wildcard");
            Assert.True(castedVersion.IsMinorWildcard, "Minor should be marked as wildcard");
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_MinorWildcard()
        {
            string version = "3.*";

            var parsedVersion = DependencyVersionParser.Parse(version, true);

            var castedVersion = Assert.IsAssignableFrom<WildcardDependencyVersion>(parsedVersion);
            Assert.Equal(3, (int)castedVersion.Major);
            Assert.True(castedVersion.IsMinorWildcard, "Minor should be marked as wildcard");
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Parse_PatchWildcard()
        {
            string version = "3.1.*";

            var parsedVersion = DependencyVersionParser.Parse(version, true);

            var castedVersion = Assert.IsAssignableFrom<WildcardDependencyVersion>(parsedVersion);
            Assert.Equal(3, (int)castedVersion.Major);
            Assert.Equal(1, (int)castedVersion.Minor);
            Assert.False(castedVersion.IsMinorWildcard, "Minor should not be marked as wildcard");
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_MajorWildcard_Operators()
        {
            var version = new WildcardDependencyVersion();

            Assert.True(version > new DependencyVersion(2));
            Assert.True(version > new WildcardDependencyVersion(3, 123));
            Assert.True(version > new DependencyVersion(3, 1));
            Assert.True(version > new DependencyVersion(uint.MaxValue, uint.MaxValue, 123));
            Assert.False(version < new DependencyVersion(4));
            Assert.False(version == new DependencyVersion(3));
            Assert.True(version != new DependencyVersion(3));
            Assert.True(version == new WildcardDependencyVersion());
            Assert.False(version == new WildcardDependencyVersion(3, 1));
            Assert.False(version == new WildcardDependencyVersion(3));
            Assert.False(version < new DependencyVersion(4));
            Assert.False(version < new WildcardDependencyVersion(4));
            Assert.True(version > new DependencyVersion(2));
            Assert.False(version <= new DependencyVersion(3));
            Assert.True(version >= new DependencyVersion(3));
            Assert.True(version >= new WildcardDependencyVersion(3));
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_MinorWildcard_Operators()
        {
            var version = new WildcardDependencyVersion(3);

            Assert.True(version > new DependencyVersion(2));
            Assert.True(version > new WildcardDependencyVersion(3, 123));
            Assert.True(version > new DependencyVersion(3, 1));
            Assert.True(version > new DependencyVersion(3, uint.MaxValue, 123));
            Assert.True(version < new DependencyVersion(4));
            Assert.False(version == new DependencyVersion(3));
            Assert.True(version != new DependencyVersion(3));
            Assert.True(version == new WildcardDependencyVersion(3));
            Assert.False(version == new WildcardDependencyVersion(3, 1));
            Assert.True(version < new DependencyVersion(4));
            Assert.True(version < new WildcardDependencyVersion(4));
            Assert.True(version > new DependencyVersion(2));
            Assert.False(version <= new DependencyVersion(3));
            Assert.True(version >= new DependencyVersion(3));
            Assert.True(version >= new WildcardDependencyVersion(3));
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_PatchWildcard_Operators()
        {
            var version = new WildcardDependencyVersion(3, 1);

            Assert.True(version > new DependencyVersion(2));
            Assert.True(version > new DependencyVersion(3, 1, 123));
            Assert.True(version > new DependencyVersion(3, 1));
            Assert.True(version > new DependencyVersion(3, 1, uint.MaxValue));
            Assert.True(version < new DependencyVersion(4));
            Assert.False(version == new DependencyVersion(3));
            Assert.True(version != new DependencyVersion(3));
            Assert.True(version == new WildcardDependencyVersion(3, 1));
            Assert.False(version == new WildcardDependencyVersion(3));
            Assert.True(version < new DependencyVersion(4));
            Assert.True(version < new WildcardDependencyVersion(4));
            Assert.True(version < new WildcardDependencyVersion(3));
            Assert.True(version > new DependencyVersion(2));
            Assert.False(version <= new DependencyVersion(3));
            Assert.True(version >= new DependencyVersion(3));
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_MinorWildcard_ToString()
        {
            var version = new WildcardDependencyVersion(1);

            Assert.Equal("1.*", version.ToString());
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Version_MajorWildcard_ToString()
        {
            var version = new WildcardDependencyVersion(1,2);

            Assert.Equal("1.2.*", version.ToString());
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
        [InlineData("3.*.")]
        [InlineData("3*")]
        [InlineData("3..*")]
        [InlineData("3.*.*")]
        [InlineData("*.*")]
        [InlineData("*.*.*")]
        [InlineData("*.123.1")]
        [InlineData("*.123.*")]
        public void Parse_InvalidVersions(string version)
        {
            Assert.Throws<VersionParsingException>(() =>
            {
                var parsedVersion = DependencyVersionParser.Parse(version, true);
            });
        }
    }
}
