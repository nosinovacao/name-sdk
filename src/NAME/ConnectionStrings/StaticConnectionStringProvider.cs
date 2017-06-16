using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Represents an object that returns always the same connection string.
    /// </summary>
    /// <seealso cref="NAME.Core.IConnectionStringProvider" />
    public class StaticConnectionStringProvider : IConnectionStringProvider
    {
        private string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticConnectionStringProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public StaticConnectionStringProvider(string connectionString)
        {
            this.connectionString = connectionString;
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
            connectionString = this.connectionString;
            return true;
        }
    }
}
