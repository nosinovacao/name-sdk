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
        /// Initializes a new instance of the <see cref="NAMEAspNetCoreConfiguration"/> class.
        /// </summary>
        public NAMEAspNetCoreConfiguration()
        {
            this.DependenciesFilePath = "dependencies.json";
        }
    }
}
