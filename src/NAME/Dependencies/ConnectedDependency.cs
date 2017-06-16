using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Json;

namespace NAME.Dependencies
{
    /// <summary>
    /// Dependency with a version resolver and a connectionString
    /// </summary>
    /// <seealso cref="NAME.Dependencies.VersionedDependency" />
    internal class ConnectedDependency : VersionedDependency
    {
        public ConnectedDependency(IVersionResolver versionResolver)
            : base(versionResolver)
        {
        }

        public bool ShowConnectionStringInJson { get; set; }

        public IConnectionStringProvider ConnectionStringProvider { get; set; }

        internal override async Task<JsonNode> ToJson()
        {
            JsonNode node = await base.ToJson().ConfigureAwait(false);
            if (this.ShowConnectionStringInJson)
            {
                string connectionString = string.Empty;
                if (this.ConnectionStringProvider?.TryGetConnectionString(out connectionString) == true)
                    node.Add("value", connectionString);
            }
            return node;
        }
    }
}
