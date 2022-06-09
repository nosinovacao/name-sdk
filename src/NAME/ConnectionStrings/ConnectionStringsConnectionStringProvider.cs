#if NET462
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Provides a mechanism to fetch a connection string from the <see cref="ConfigurationManager"/> ConnectionStrings section.
    /// </summary>
    /// <seealso cref="NAME.Core.IConnectionStringProvider" />
    public class ConnectionStringsConnectionStringProvider : IConnectionStringProvider
    {
        private string key;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringsConnectionStringProvider"/> class.
        /// </summary>
        /// <param name="key">The key of the connection string.</param>
        public ConnectionStringsConnectionStringProvider(string key)
        {
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
            connectionString = ConfigurationManager.ConnectionStrings[this.key]?.ConnectionString;
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
            return $"Locator: {SupportedConnectionStringLocators.ConnectionStrings}, Key: {this.key}";
        }
    }
}
#endif
