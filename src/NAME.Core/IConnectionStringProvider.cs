using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Contract for connection string providers to implement.
    /// </summary>
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionString">A <see cref="string"/> containing the connection string.</param>
        /// <returns>
        /// Returns true if the connection string was fetched successfully.
        /// </returns>
        bool TryGetConnectionString(out string connectionString);
    }
}
