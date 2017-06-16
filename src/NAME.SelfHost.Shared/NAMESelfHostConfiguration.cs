using NAME.Hosting.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.SelfHost.Shared
{
    // Disable this error, the types will be coming from the same assembly anyway.
    // TODO: When this project is ported to the new SDK tooling (.csproj) migrate the shared code projects to code library in order to fix this.
#pragma warning disable CS0436 // Type conflicts with imported type

    /// <summary>
    /// Represents the configuration used for NAME SelfHosting.
    /// </summary>
    public class NAMESelfHostConfiguration : NAMEBaseConfiguration
#pragma warning restore CS0436 // Type conflicts with imported type
    {
        /// <summary>
        /// Gets or sets the adress where the server will listen for connections.
        /// By default it is "*", meaning it will bind to all IPs.
        /// </summary>
        /// <value>
        /// The address where the server will listen for connections.
        /// </value>
        public string AddressToListenOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the initial health check should to logged to the console.
        /// </summary>
        /// <value>
        /// <c>true</c> if the initial health check should to logged to the console; otherwise, <c>false</c>.
        /// </value>
        public bool LogHealthCheckToConsole { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMESelfHostConfiguration"/> class.
        /// </summary>
        public NAMESelfHostConfiguration()
        {
            this.AddressToListenOn = "*";
            this.DependenciesFilePath = "./dependencies.json";
            this.LogHealthCheckToConsole = false;
        }
    }
}
