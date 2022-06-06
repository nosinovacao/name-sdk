#if NET462
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Provides a mechanism to fetch a connection string from a Visual Studio Settings file.
    /// </summary>
    /// <seealso cref="NAME.Core.IConnectionStringProvider" />
    public class VisualStudioSetingsFileConnectionStringProvider : IConnectionStringProvider
    {
        private const string BASE_SECTION = "applicationSettings/";
        
        private string sectionName;
        private string key;


        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioSetingsFileConnectionStringProvider" /> class.
        /// </summary>
        /// <param name="sectionName">Name of the configuration section.</param>
        /// <param name="key">The key of the connection string.</param>
        public VisualStudioSetingsFileConnectionStringProvider(string sectionName, string key)
        {
            this.sectionName = sectionName;
            this.key = key;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionString">A <see cref="string" /> containing the connection string.</param>
        /// <returns>
        /// Returns true if the connection string was fetched successfully.
        /// </returns>
        public bool TryGetConnectionString(out string connectionString)
        {
            connectionString = null;
            var tempSection = this.sectionName;
            if (!tempSection.StartsWith(BASE_SECTION))
                tempSection = BASE_SECTION + tempSection;

            ClientSettingsSection settingsSection = ConfigurationManager.GetSection(tempSection) as ClientSettingsSection;
            if (settingsSection == null) 
            {
                Trace.TraceInformation($"Could not fetch the connection string from the VSSettingsFile with the key {this.key}.");
                return false;
            }

            connectionString = settingsSection.Settings.Get(this.key)?.Value?.ValueXml?.InnerText;

            return connectionString != null;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Locator: {SupportedConnectionStringLocators.VSSettingsFile}, Section: {this.sectionName}, Key: {this.key}";
        }
    }
}
#endif