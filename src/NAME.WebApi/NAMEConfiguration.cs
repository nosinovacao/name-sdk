using NAME.Hosting.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NAME.WebApi
{
    /// <summary>
    /// Represents the configuration for NAME.
    /// </summary>
    public class NAMEConfiguration : NAMEBaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEConfiguration"/> class.
        /// </summary>
        public NAMEConfiguration()
        {
            this.DependenciesFilePath = "~/dependencies.json";
        }
    }
}
