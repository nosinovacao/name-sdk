#if NET45
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Provides a mechanism to fetch connection strings from the <see cref="ConfigurationManager"/> AppSettings.
    /// </summary>
    /// <seealso cref="NAME.Core.IConnectionStringProvider" />
    public class AppSettingsConnectionStringProvider : IConnectionStringProvider
    {

        private string key;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsConnectionStringProvider"/> class.
        /// </summary>
        /// <param name="key">The key of the connection string.</param>
        public AppSettingsConnectionStringProvider(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionString">A <see cref="string" /> containing the connection string..</param>
        /// <returns>Returns true if the connection string was fetched successfully.</returns>
        public bool TryGetConnectionString(out string connectionString)
        {
            connectionString = ConfigurationManager.AppSettings[this.key];
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
            return $"Locator: {SupportedConnectionStringLocators.AppSettings}, Key: {this.key}";
        }
    }
}
#endif