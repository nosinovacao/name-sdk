namespace NAME.Core.DTOs
{
    /// <summary>
    /// Represents a data transfer object for the service.
    /// </summary>
    public class BootstrapDTO
    {

        /// <summary>
        /// Gets or sets the supported protocols.
        /// </summary>
        /// <value>
        /// The supported protocols.
        /// </value>
        public uint[] SupportedProtocols { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the name endpoint.
        /// </summary>
        /// <value>
        /// The name endpoint.
        /// </value>
        public string NAMEEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the NAME port.
        /// </summary>
        /// <value>
        /// The NAME port.
        /// </value>
        public uint? NAMEPort { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the NAME version.
        /// </summary>
        /// <value>
        /// The name version.
        /// </value>
        public string NAMEVersion { get; set; }
    }
}
