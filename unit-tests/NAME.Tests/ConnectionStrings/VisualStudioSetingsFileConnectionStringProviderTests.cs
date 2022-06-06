#if NET462
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NAME.Core;
using NAME.ConnectionStrings;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.Configuration.Internal;
using System.Xml;

namespace NAME.Tests.ConnectionStrings
{
    public class VisualStudioSetingsFileConnectionStringProviderTests
    {
        internal const string SETTING_SECTION = "applicationSettings/OhMySectionItBurns";
        internal const string SETTING_KEY = "TheAllMightyKey";
        internal const string SETTING_VALUE = "mongodb://some-mongo-instance:27017/some-db";

        public VisualStudioSetingsFileConnectionStringProviderTests()
        {
            var fi = typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.Static | BindingFlags.NonPublic);
            fi.SetValue(null, new MoqConfigSystem());
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString()
        {
            IConnectionStringProvider provider = new VisualStudioSetingsFileConnectionStringProvider(SETTING_SECTION, SETTING_KEY);
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.True(result, "Unable to get the connection string.");
            Assert.Equal(outConnection, SETTING_VALUE);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void OnceUponALovelyDayWhenAttemptingToInvokeTryGetConnectionStringIWasExpectingTheKeyToNotBeFound()
        {
            IConnectionStringProvider provider = new VisualStudioSetingsFileConnectionStringProvider(SETTING_SECTION, SETTING_KEY + "Thou shall not find it!");
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The connection string should not be found.");
            Assert.Null(outConnection);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void TryGetConnectionString_SectionNotFound()
        {
            IConnectionStringProvider provider = new VisualStudioSetingsFileConnectionStringProvider(SETTING_SECTION + "Thou shall not find it!", SETTING_KEY);
            bool result = provider.TryGetConnectionString(out string outConnection);

            Assert.False(result, "The section should not be found.");
            Assert.Null(outConnection);
        }

    }

    public class MoqConfigSystem : IInternalConfigSystem
    {
        private static IInternalConfigSystem oldConfigSystem;
        public MoqConfigSystem()
        {
            //Ensure the setup of the ConfigurationManager before.
            var count = ConfigurationManager.AppSettings.Get("asdasd");
            if (oldConfigSystem == null)
                oldConfigSystem = (IInternalConfigSystem)typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }
        public bool SupportsUserConfig
        {
            get
            {
                return true;
            }
        }

        public object GetSection(string sectionName)
        {
            if (sectionName != VisualStudioSetingsFileConnectionStringProviderTests.SETTING_SECTION)
                return oldConfigSystem?.GetSection(sectionName);

            var settingElement = new SettingElement(VisualStudioSetingsFileConnectionStringProviderTests.SETTING_KEY, SettingsSerializeAs.String);
            var settingValue = new SettingValueElement();
            settingValue.ValueXml = new XmlDocument().CreateElement("value");
            settingValue.ValueXml.InnerText = VisualStudioSetingsFileConnectionStringProviderTests.SETTING_VALUE;
            settingElement.Value = settingValue;
            var section = new ClientSettingsSection();
            section.Settings.Add(settingElement);

            return section;
        }

        public void RefreshConfig(string sectionName)
        {
        }
    }
}
#endif