using Microsoft.Extensions.Configuration;
using NAME.Hosting.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.AspNetCore
{
    /// <summary>
    /// Represents the configuration used for NAME in a ASP.NET core environment.
    /// </summary>
    /// <seealso cref="NAME.Hosting.Shared.NAMEBaseConfiguration" />
    public class NAMEAspNetCoreConfiguration : NAMEBaseConfiguration
    {
        /// <summary>
        /// Gets or sets the Asp.Net Core configuration instance to be used by NAME.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEAspNetCoreConfiguration"/> class.
        /// </summary>
        public NAMEAspNetCoreConfiguration()
        {
            this.DependenciesFilePath = "dependencies.json";
        }
    }
}
