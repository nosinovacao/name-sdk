using Xunit;

namespace NAME.Tests.Digest
{
    public class Digest
    {
        private const string genericMessage = "The quick brown fox jumps over the lazy dog";
        private const string genericAvalancheMessage = "The quick brown fox jumps over the lazy dog.";

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TestEmptyMD5()
        {
            const string expectedDigest = "d41d8cd98f00b204e9800998ecf8427e";
            TestValue(string.Empty, expectedDigest);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TestSpecificMD5()
        {
            const string expectedDigest = "9e107d9d372bb6826bd81d3542a419d6";
            TestValue(genericMessage, expectedDigest);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TestAvalancheEffectMD5()
        {
            const string expectedDigest = "e4d909c290d0fb1ca068ffaddf22cbd0";
            TestValue(genericAvalancheMessage, expectedDigest);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TestEmptySHA1()
        {
            const string algorithm = "SHA1";
            const string expectedDigest = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
            TestValue(algorithm, string.Empty, expectedDigest);
        }


        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TestSpecificSHA1()
        {
            const string algorithm = "SHA1";
            const string expectedDigest = "2fd4e1c67a2d28fced849ee1bb76e7391b93eb12";
            TestValue(algorithm, genericMessage, expectedDigest);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TestAvalancheEffectSHA1()
        {
            const string algorithm = "SHA1";
            const string expectedDigest = "408d94384216f890ff7a0c3528e8bed1e0b01621";
            TestValue(algorithm, genericAvalancheMessage, expectedDigest);
        }

        private static void TestValue(string algorithm, string message, string expected)
        {
            var helperResult = DigestHelpers.DigestHelper.GetDigestForMessage(algorithm, message);
            Assert.Equal(expected, helperResult);
        }

        private static void TestValue(string message, string expected)
        {
            var helperResult = DigestHelpers.DigestHelper.GetDigestForMessage(message);
            Assert.Equal(expected, helperResult);
        }
    }
}
