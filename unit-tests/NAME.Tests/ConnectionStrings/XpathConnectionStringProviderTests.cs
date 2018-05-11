using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NAME.Core;
using NAME.ConnectionStrings;
using System.Reflection;
using System.Xml;
using System.IO;

namespace NAME.Tests.ConnectionStrings
{
    public class XpathConnectionStringProviderTests : IClassFixture<CreateXPathFileFixture>
    {
        public static string CONFIGURATION_FILE = Guid.NewGuid() + ".xml";
        public const string CONFIGURATION_CONTENTS =
          @"<?xml version=""1.0"" encoding=""utf-8""?>
            <configuration>
                <configSections>
                    <sectionGroup name=""applicationSettings"" type=""System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
                        <section name=""nPVR.Services.AAAAClient.Properties.Settings"" type=""System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false"" />
                    </sectionGroup>
                </configSections>
                <applicationSettings>
                    <nPVR.Services.AAAAClient.Properties.Settings>
                        <setting name=""AAAAEndpoint"" serializeAs=""String"">
                            <value>http://localhost/service/api/v1/</value>
                        </setting>
                    </nPVR.Services.AAAAClient.Properties.Settings>
                    <nPVR.Services.OtherClient.Properties.Settings>
                        <setting name=""Timeout"" serializeAs=""Int32"">
                            <value>10000</value>
                        </setting>
                    </nPVR.Services.OtherClient.Properties.Settings>
                </applicationSettings>
            </configuration>";

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString()
        {
            IConnectionStringProvider provider = new XpathConnectionStringProvider(CONFIGURATION_FILE, "/configuration/applicationSettings/nPVR.Services.AAAAClient.Properties.Settings/setting[@name=\"AAAAEndpoint\"]/value");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.True(result, "The connection string was not found!");
            Assert.Equal("http://localhost/service/api/v1/", outConnection);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void OnceUponALovelyDayWhenAttemptingToInvokeTryGetConnectionStringIWasExpectingTheKeyToNotBeFound()
        {
            IConnectionStringProvider provider = new XpathConnectionStringProvider(CONFIGURATION_FILE, "Thou shall not find the key!");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The connection string should not be found.");
            Assert.Null(outConnection);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString_FileNotFound()
        {
            IConnectionStringProvider provider = new XpathConnectionStringProvider(CONFIGURATION_FILE + "Thou shall not find the file!", "/configuration/applicationSettings/nPVR.Services.AAAAClient.Properties.Settings/setting[@name=\"AAAAEndpoint\"]/value");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The file should not be found.");
            Assert.Null(outConnection);
        }
    }

    public class CreateXPathFileFixture : IDisposable
    {
        public CreateXPathFileFixture()
        {
            if (File.Exists(XpathConnectionStringProviderTests.CONFIGURATION_FILE))
                File.Delete(XpathConnectionStringProviderTests.CONFIGURATION_FILE);
            File.WriteAllText(XpathConnectionStringProviderTests.CONFIGURATION_FILE, XpathConnectionStringProviderTests.CONFIGURATION_CONTENTS);
        }
        public void Dispose()
        {
            File.Delete(XpathConnectionStringProviderTests.CONFIGURATION_FILE);
        }

    }
}