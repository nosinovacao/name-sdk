using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Core
{
    /// <summary>
    /// Represents the current NAME context.
    /// </summary>
    public class NAMEContext
    {
        /// <summary>
        /// Gets or sets the service dependency current number of hops.
        /// </summary>
        /// <value>
        /// The service dependency current number of hops.
        /// </value>
        public int ServiceDependencyCurrentNumberOfHops { get; set; } = 0;

    }
}
