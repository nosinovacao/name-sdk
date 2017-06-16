using NAME.Core;
using NAME.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace NAME.Tests.SqlServer
{
    public class SqlServerVersionTranslatorTests
    {
        [Theory]
        [Trait("TestCategory", "Unit")]
        [MemberData(nameof(TestData))]
        public void Translate_SqlServerVersions(SqlServerVersions version, string expectedVersion)
        {
            IVersionTranslator translator = new SqlServerVersionTranslator();

            DependencyVersion v = translator.Translate(version.ToString());

            Assert.True(v >= DependencyVersion.Parse(expectedVersion));
        }

        [Theory]
        [Trait("TestCategory", "Unit")]
        [MemberData(nameof(TestData))]
        public void Translate_Versions(SqlServerVersions exptectedVersion, string versionStr)
        {
            IVersionTranslator translator = new SqlServerVersionTranslator();

            string v = translator.Translate(DependencyVersion.Parse(versionStr));

            Assert.True(v == exptectedVersion.ToString());
        }

        public static IEnumerable<object[]> TestData
        {
            get
            {
                return new[]
                {
                    new object[] { SqlServerVersions.SqlServer2016, "13.0.2186" },
                    new object[] { SqlServerVersions.SqlServer2014,"12.0.2000" },
                    new object[] { SqlServerVersions.SqlServer2012, "11.0.2100" },
                    new object[] { SqlServerVersions.SqlServer2008R2, "10.50.1600" },
                    new object[] { SqlServerVersions.SqlServer2008, "10.0.1600" },
                    new object[] { SqlServerVersions.SqlServer2005, "9.0.1399" },
                    new object[] { SqlServerVersions.SqlServer2000, "8.0.384" },
                    new object[] { SqlServerVersions.SqlServer7, "7.0.623" }
                };
            }
        }
    }
}
