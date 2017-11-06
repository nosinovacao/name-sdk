
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Core.Utils;
using NAME.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests.OperatingSystem
{
    public class WindowsVersionTranslatorTests
    {

        [Theory]
        [Trait("TestCategory", "Unit")]
        [InlineData(WindowsVersions.WindowsXP, "5.1")]
        [InlineData(WindowsVersions.WindowsXPProfessionalx64, "5.2")]
        [InlineData(WindowsVersions.WindowsServer2003, "5.2")]
        [InlineData(WindowsVersions.WindowsServer2003R2, "5.2")]
        [InlineData(WindowsVersions.WindowsVista, "6.0")]
        [InlineData(WindowsVersions.WindowsServer2008, "6.0")]
        [InlineData(WindowsVersions.WindowsServer2008R2, "6.1")]
        [InlineData(WindowsVersions.Windows7, "6.1")]
        [InlineData(WindowsVersions.WindowsServer2012, "6.2")]
        [InlineData(WindowsVersions.Windows8, "6.2")]
        [InlineData(WindowsVersions.WindowsServer2012R2, "6.3")]
        [InlineData(WindowsVersions.Windows81, "6.3")]
        [InlineData(WindowsVersions.WindowsServer2016, "10.0")]
        [InlineData(WindowsVersions.Windows10, "10.0")]
        public void Translate_WindowsVersions(WindowsVersions version, string expectedVersion)
        {
            IVersionTranslator translator = new WindowsVersionTranslator();

            DependencyVersion v = translator.Translate(version.ToString());

            Assert.True(v >= DependencyVersionParser.Parse(expectedVersion, false));
        }

        [Theory]
        [Trait("TestCategory", "Unit")]
        [InlineData(WindowsVersions.WindowsXP, "5.1")]
        [InlineData(WindowsVersions.WindowsServer2003, "5.2")]
        [InlineData(WindowsVersions.WindowsServer2008, "6.0")]
        [InlineData(WindowsVersions.WindowsServer2008R2, "6.1")]
        [InlineData(WindowsVersions.WindowsServer2012, "6.2")]
        [InlineData(WindowsVersions.WindowsServer2012R2, "6.3")]
        [InlineData(WindowsVersions.WindowsServer2016, "10.0")]
        public void Translate_Versions(WindowsVersions exptectedVersion, string versionStr)
        {
            IVersionTranslator translator = new WindowsVersionTranslator();

            string v = translator.Translate(new OperatingSystemDependencyVersion("windows", DependencyVersionParser.Parse(versionStr, false)));

            Assert.True(v == exptectedVersion.ToString());
        }
        
    }
}
