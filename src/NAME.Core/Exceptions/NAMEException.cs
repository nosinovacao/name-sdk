using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Represents a generic NAME exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class NAMEException : Exception
    {
        /// <summary>
        /// Gets or sets the status level of the operation that originated this exception.
        /// </summary>
        /// <value>
        /// The status level of the operation that originated this exception.
        /// </value>
        public NAMEStatusLevel StatusLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="statusLevel">The status level of the operation that originated this exception.</param>
        public NAMEException(string message, NAMEStatusLevel statusLevel)
            : base(message)
        {
            this.StatusLevel = statusLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="statusLevel">The status level of the operation that originated this exception.</param>
        public NAMEException(string message, Exception inner, NAMEStatusLevel statusLevel)
            : base(message, inner)
        {
            this.StatusLevel = statusLevel;
        }
    }
}
