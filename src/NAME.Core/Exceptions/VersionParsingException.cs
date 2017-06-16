using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Happens when parsing of an invalid version.
    /// </summary>
    /// <seealso cref="NAME.Core.Exceptions.NAMEException" />
    public class VersionParsingException : NAMEException
    {
        /// <summary>
        /// Gets the version attempted to parse.
        /// </summary>
        /// <value>
        /// The version attempted to parse.
        /// </value>
        public string SourceVersion { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionParsingException"/> class.
        /// </summary>
        /// <param name="sourceVersion">The source version.</param>
        /// <param name="message">The message.</param>
        public VersionParsingException(string sourceVersion, string message)
            : base(message)
        {
            this.SourceVersion = sourceVersion;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionParsingException"/> class.
        /// </summary>
        /// <param name="sourceVersion">The source version.</param>
        public VersionParsingException(string sourceVersion)
            : this(sourceVersion, $"The provided version {sourceVersion} is invalid.")
        {
        }
    }
}
