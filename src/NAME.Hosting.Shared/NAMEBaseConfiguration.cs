using System;
using System.IO;

namespace NAME.Hosting.Shared
{
    /// <summary>
    /// Represents the base configuration used for NAME.
    /// </summary>
    public class NAMEBaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEBaseConfiguration"/> class.
        /// </summary>
        public NAMEBaseConfiguration()
        {
            this.ManifestUriPrefix = string.Empty;
            this.DependenciesFilePath = string.Empty;
            this.APIName = string.Empty;
            this.APIVersion = string.Empty;
            this.ThrowOnDependenciesFail = false;
        }

        /// <summary>
        /// Gets or sets the dependencies file path.
        /// </summary>
        /// <value>
        /// The dependencies file path.
        /// </value>
        public string DependenciesFilePath { get; set; }

        /// <summary>
        /// Gets or sets the dependencies stream.
        /// </summary>
        /// <value>
        /// The dependencies stream.
        /// </value>
        public Stream DependenciesStream { get; set; }

        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        /// <value>
        /// The name of the API.
        /// </value>
        public string APIName { get; set; }

        private string apiVersion;

        /// <summary>
        /// Gets or sets the API version.
        /// </summary>
        /// <value>
        /// The API version.
        /// </value>
        public string APIVersion
        {
            get
            {
                return this.apiVersion;
            }
            set
            {
                int dotsCount = value.Length - value.Replace(".", string.Empty).Length;
                if (dotsCount > 2)
                    this.apiVersion = value.Substring(0, value.LastIndexOf('.'));
                else
                    this.apiVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an exception should be thrown when enabling NAME if any of the dependencies don't match the specified versions.
        /// If false, the dependencies state can be checked in the System.Diagnostics traces.
        /// </summary>
        /// <value>
        /// <c>true</c> if an exception should be thrown; otherwise, <c>false</c>.
        /// </value>
        public bool ThrowOnDependenciesFail { get; set; }

        /// <summary>
        /// Gets or sets the manifest prefix, to be used before /manifest.
        /// </summary>
        /// <value>
        /// The manifest prefix, to be used before /manifest.
        /// </value>
        public string ManifestUriPrefix { get; set; }
    }
}
