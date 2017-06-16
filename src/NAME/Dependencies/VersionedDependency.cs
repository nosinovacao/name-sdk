using NAME.Core;
using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Json;

namespace NAME.Dependencies
{
    /// <summary>
    /// Represents a versioned dependency.
    /// </summary>
    /// <seealso cref="NAME.Dependencies.Dependency" />
    internal abstract class VersionedDependency : Dependency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedDependency"/> class.
        /// </summary>
        /// <param name="versionResolver">The version resolver.</param>
        public VersionedDependency(IVersionResolver versionResolver)
        {
            this.VersionResolver = versionResolver;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public SupportedDependencies Type { get; set; }

        /// <summary>
        /// Gets or sets the minimum version.
        /// </summary>
        /// <value>
        /// The minimum version.
        /// </value>
        public DependencyVersion MinimumVersion { get; set; }

        /// <summary>
        /// Gets or sets the maximum version. If this value is null, it means there is no upper bound.
        /// </summary>
        /// <value>
        /// The maximum version.
        /// </value>
        public DependencyVersion MaximumVersion { get; set; }

        /// <summary>
        /// Gets the version resolver.
        /// </summary>
        /// <value>
        /// The version resolver.
        /// </value>
        protected IVersionResolver VersionResolver { get; }

        /// <summary>
        /// Determines the dependency status.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation. The result contains the status of the dependency.
        /// </returns>
        public override async Task<DependencyCheckStatus> GetStatus()
        {
            try
            {
                IEnumerable<DependencyVersion> actualVersions = await this.VersionResolver.GetVersions().ConfigureAwait(false);
                if (!actualVersions.Any())
                    return new DependencyCheckStatus(false, message: "Could not fetch the versions.");

                foreach (var version in actualVersions)
                {
                    if (version < this.MinimumVersion || (this.MaximumVersion != null && version > this.MaximumVersion))
                    {
                        return new DependencyCheckStatus(false, version: version, message: "Unsupported version.");
                    }
                }

                return new DependencyCheckStatus(true, actualVersions.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return new DependencyCheckStatus(false, message: ex.Message, innerException: ex);
            }
        }

        internal override async Task<JsonNode> ToJson()
        {
            JsonClass jsonDependency = new JsonClass();
            jsonDependency.Add("name", this.ToString());
            DependencyCheckStatus status = await this.GetStatus().ConfigureAwait(false);

            if (status.Version != null)
                jsonDependency.Add("version", status.Version.ToString());

            if (!status.CheckPassed)
                jsonDependency.Add("error", status.Message ?? "Unhandled error");

            jsonDependency.Add("min_version", this.MinimumVersion.ToString());
            jsonDependency.Add("max_version", this.MaximumVersion?.ToString() ?? "*");

            if (status.Version?.ManifestNode != null)
            {
                JsonNode infrastructureDependencies = status.Version.ManifestNode["infrastructure_dependencies"];
                JsonNode serviceDependencies = status.Version.ManifestNode["service_dependencies"];

                if (infrastructureDependencies != null)
                    jsonDependency.Add("infrastructure_dependencies", infrastructureDependencies);
                if (serviceDependencies != null)
                    jsonDependency.Add("service_dependencies", serviceDependencies);
            }

            return jsonDependency;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? this.Type.ToString() : this.Name;
        }

    }
}
