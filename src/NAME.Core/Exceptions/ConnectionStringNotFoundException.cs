using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Represents an exception for when the connection string could not be found.
    /// </summary>
    /// <seealso cref="NAME.Core.Exceptions.NAMEException" />
    public class ConnectionStringNotFoundException : NAMEException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringNotFoundException"/> class.
        /// </summary>
        /// <param name="provider">The provider used to try to get the connection string.</param>
        public ConnectionStringNotFoundException(string provider) 
            : base($"Unable to find the connection string using: {provider}.", NAMEStatusLevel.Warn)
        {
        }
    }
}
