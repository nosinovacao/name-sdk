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
        /// Initializes a new instance of the <see cref="NAMEException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NAMEException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public NAMEException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
