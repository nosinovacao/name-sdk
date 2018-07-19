using NAME.Json;
using System;

namespace NAME.Core
{
    /// <summary>
    /// Represents the settings to be used by NAME.
    /// </summary>
    public sealed class NAMESettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAMESettings"/> class.
        /// </summary>
        public NAMESettings()
        {

        }

        /// <summary>
        /// Gets the registry server endpoints.
        /// </summary>
        /// <value>
        /// The registry server endpoints.
        /// </value>
        /// <remarks> the defauld values reflect the expectancy that the HOSTS file can resolve these names </remarks>
        public string[] RegistryEndpoints { get; internal set; } = new[] 
        {
            "http://registry1.name.local.internal:80/api/v1",
            "http://registry2.name.local.internal:80/api/v1",
            "http://registry1-name:80/api/v1",
            "http://registry2-name:80/api/v1"
        };

        /// <summary>
        /// Gets the first port in the range for SelfHost servers.
        /// </summary>
        /// <value>
        /// The self host port range first value.
        /// </value>
        public int SelfHostPortRangeFirst { get; internal set; } = 40500;

        /// <summary>
        /// Gets the last port in the range for SelfHost servers.
        /// </summary>
        /// <value>
        /// The self host port range last value.
        /// </value>
        public int SelfHostPortRangeLast { get; internal set; } = 40600;

        /// <summary>
        /// Gets the service dependency maximum number of hops.
        /// </summary>
        /// <value>
        /// The service dependency maximum number of hops.
        /// </value>
        public int ServiceDependencyMaxHops { get; internal set; } = 5;

        /// <summary>
        /// Gets a value indicating whether the connection strings should be added to the manifest.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection strings should be added to the manifest; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectedDependencyShowConnectionString { get; internal set; } = true;

        /// <summary>
        /// Gets the dependency connect timeout in miliseconds.
        /// </summary>
        /// <value>
        /// The dependency connect timeout in miliseconds.
        /// </value>
        public int DependencyConnectTimeout { get; internal set; } = 7000;

        /// <summary>
        /// Gets the dependency read and write timeout in miliseconds.
        /// </summary>
        /// <value>
        /// The dependency read and write timeout in miliseconds.
        /// </value>
        public int DependencyReadWriteTimeout { get; internal set; } = 10000;

        /// <summary>
        /// Gets the registry running mode.
        /// </summary>
        /// <value>
        /// The registry running mode.
        /// </value>
        public SupportedNAMEBehaviours RunningMode { get; internal set; } = SupportedNAMEBehaviours.Standard;

        /// <summary>
        /// Gets the registry ping frequency.
        /// </summary>
        /// <value>
        /// The registry ping frequency.
        /// </value>
        public System.TimeSpan RegistryPingFrequency { get; internal set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets the registry re-announce frequency.
        /// </summary>
        /// <value>
        /// The registry re-announce frequency.
        /// </value>
        public System.TimeSpan RegistryReAnnounceFrequency { get; internal set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Gets the registry bootstrap retry frequency.
        /// </summary>
        /// <value>
        /// The registry bootstrap rety frequency.
        /// </value>
        public TimeSpan RegistryBootstrapRetryFrequency { get; internal set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Gets the registry bootstrap connect timeout.
        /// </summary>
        /// <value>
        /// The registry bootstrap connect timeout.
        /// </value>
        public TimeSpan RegistryBootstrapTimeout { get; internal set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets or sets the function that may be used to override the connection string provider each dependency wil use.
        /// </summary>
        /// <value>
        /// The function that may be used to override the connection string provider each dependency wil use.
        /// </value>
        public Func<IJsonNode, IConnectionStringProvider> ConnectionStringProviderOverride { get; set; }
    }
}
