using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Happens when a dependency does not have NAME installed.
    /// </summary>
    /// <seealso cref="NAME.Core.Exceptions.NAMEException" />
    public class DependencyWithoutNAMEException : NAMEException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyWithoutNAMEException" /> class.
        /// </summary>
        /// <param name="message">The message that represents the error.</param>
        /// <param name="inner">Inner exception.</param>
        public DependencyWithoutNAMEException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyWithoutNAMEException"/> class.
        /// </summary>
        public DependencyWithoutNAMEException()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyWithoutNAMEException"/> class.
        /// </summary>
        /// <param name="inner">Inner exception.</param>
        public DependencyWithoutNAMEException(Exception inner)
            : this($"Dependency does not have NAME installed!", inner)
        {

        }
    }
}
