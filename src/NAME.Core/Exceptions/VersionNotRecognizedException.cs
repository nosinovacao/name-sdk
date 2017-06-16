using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that happens when the provided version is not recognized.
    /// </summary>
    /// <seealso cref="NAME.Core.Exceptions.NAMEException" />
    public class VersionNotRecognizedException : NAMEException
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string DependencyVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionNotRecognizedException"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public VersionNotRecognizedException(string version)
            : this(version, $"The version {version} was not recognized.")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionNotRecognizedException"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="message">The message.</param>
        public VersionNotRecognizedException(string version, string message)
            : this(version, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionNotRecognizedException"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public VersionNotRecognizedException(string version, string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
