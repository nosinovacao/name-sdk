using System.Threading.Tasks;
using NAME.Core;
using NAME.Json;

namespace NAME.Dependencies
{
    /// <summary>
    /// Represents an external service dependency
    /// </summary>
    /// <seealso cref="NAME.Dependencies.ConnectedDependency" />
    internal class ServiceDependency : ConnectedDependency
    {
        public ServiceDependency(IVersionResolver versionResolver)
            : base(versionResolver)
        {
        }

        internal JsonNode Manifest { get; private set; }

        internal override async Task<JsonNode> ToJson()
        {
            JsonNode node = await base.ToJson();
            if (this.Manifest != null)
            {
                JsonNode infrastructureDependencies = this.Manifest["infrastructure_dependencies"];
                JsonNode serviceDependencies = this.Manifest["service_dependencies"];

                if (infrastructureDependencies != null)
                    node.Add("infrastructure_dependencies", infrastructureDependencies);
                if (serviceDependencies != null)
                    node.Add("service_dependencies", serviceDependencies);
            }
            return node;
        }
    }
}