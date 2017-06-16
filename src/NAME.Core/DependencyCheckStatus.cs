using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Represents the status of a dependency check.
    /// </summary>
    public class DependencyCheckStatus
    {
        /// <summary>
        /// Gets a value indicating whether the dependency check is ok.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this dependency check is ok; otherwise, <c>false</c>.
        /// </value>
        public bool CheckPassed { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public DependencyVersion Version { get; }

        /// <summary>
        /// Gets the inner exception.
        /// </summary>
        /// <value>
        /// The inner exception.
        /// </value>
        public Exception InnerException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCheckStatus" /> class.
        /// </summary>
        /// <param name="checkPassed">if set to <c>true</c> the check passed.</param>
        /// <param name="version">The version.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DependencyCheckStatus(bool checkPassed, DependencyVersion version = null, string message = "", Exception innerException = null)
        {
            this.CheckPassed = checkPassed;
            this.Version = version;
            this.Message = message;
            this.InnerException = innerException;
        }
    }
}
