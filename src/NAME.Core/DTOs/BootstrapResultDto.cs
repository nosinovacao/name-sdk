using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.DTOs
{
    /// <summary>
    /// Represents the result of a bootstrap service operation
    /// </summary>
    public class BootstrapResultDto
    {
        /// <summary>
        /// Gets or sets the protocol to be used for subsequent operations.
        /// </summary>
        /// <value>
        /// The protocol to be used for subsequent operations.
        /// </value>
        public uint Protocol { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the overrides.
        /// </summary>
        /// <value>
        /// The overrides.
        /// </value>
        public NAMESettings Overrides { get; set; }
    }
}
