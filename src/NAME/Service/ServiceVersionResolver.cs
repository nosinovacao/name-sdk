using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Core.Utils;
using NAME.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NAME.Service
{
    /// <summary>
    /// Provides a mechanism to get the version of a service running NAME.
    /// </summary>
    /// <seealso cref="NAME.Core.IVersionResolver" />
    public class ServiceVersionResolver : ConnectedVersionResolver
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string InfrastructureDependenciesKey = "infrastructure_dependencies";
        private const string ServiceDependenciesKey = "service_dependencies";

        /// <summary>
        /// Gets the hop number.
        /// </summary>
        /// <value>
        /// The hop number.
        /// </value>
        public int HopNumber { get; }

        /// <summary>
        /// Gets the maximum hop count.
        /// </summary>
        /// <value>
        /// The maximum hop count.
        /// </value>
        public int MaxHopCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceVersionResolver" /> class.
        /// </summary>
        /// <param name="connectionStringProvider">The connection string provider.</param>
        /// <param name="currentHopNumber">The current hop number (i.e. How many manifest calls were made since the initial request).</param>
        /// <param name="maxHopCount">The maximum hop count.</param>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="readWriteTimeout">The read write timeout.</param>
        public ServiceVersionResolver(IConnectionStringProvider connectionStringProvider, int currentHopNumber, int maxHopCount, int connectTimeout, int readWriteTimeout)
            : base(connectTimeout, readWriteTimeout)
        {
            this._connectionStringProvider = connectionStringProvider;
            this.HopNumber = currentHopNumber;
            this.MaxHopCount = maxHopCount;
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <returns>
        /// Returns an enumerable with the versions.
        /// </returns>
        /// <exception cref="ConnectionStringNotFoundException">The connection string was not found</exception>
        /// <exception cref="NAMEException">The service returned an invalid JSON from the manifest endpoint</exception>
        /// <exception cref="VersionParsingException">The version from manifest cannot be parsed</exception>
        public override async Task<IEnumerable<DependencyVersion>> GetVersions()
        {
            if (!this._connectionStringProvider.TryGetConnectionString(out string serviceConnectionString))
                throw new ConnectionStringNotFoundException(this._connectionStringProvider.ToString());

            var serviceUri = new Uri(serviceConnectionString + Constants.MANIFEST_ENDPOINT);

            string jsonContents = await this.GetManifest(serviceUri, true, this.HopNumber)
                .ConfigureAwait(false);

            DependencyVersion dependencyVersion;
            JsonNode node;
            try
            {
                node = Json.Json.Parse(jsonContents);
                DependencyVersionParser.TryParse(node?["version"], false, out dependencyVersion);
            }
            catch (Exception ex)
            {
                throw new NAMEException($"{SupportedDependencies.Service}: The service returned an invalid JSON from the manifest endpoint.", ex);
            }

            if (dependencyVersion == null)
                throw new VersionParsingException(node?["version"], $"The version from manifest {node} cannot be parsed");

            dependencyVersion.ManifestNode = node;

            await this.GetDependantManifests(dependencyVersion).ConfigureAwait(false);

            return new[] { dependencyVersion };
        }

        /// <summary>
        /// Gets the dependant manifests.
        /// </summary>
        /// <param name="rootDependency">The root dependency.</param>
        /// <returns>
        /// .
        /// </returns>
        public async Task GetDependantManifests(DependencyVersion rootDependency)
        {
            if (rootDependency.ManifestNode == null)
            {
                return;
            }

            var nextHop = this.HopNumber + 1;

            if (nextHop == this.MaxHopCount)
            {
                return;
            }

            var dependencies = rootDependency.ManifestNode[ServiceDependenciesKey];
            foreach (var dependency in dependencies.Children)
            {
                var dependencyUrl = dependency["value"].Value;

                // get the next manifest
                if (string.IsNullOrEmpty(dependencyUrl))
                {
                    continue;
                }

                var uri = new Uri(dependencyUrl.TrimEnd('/') + Constants.MANIFEST_ENDPOINT);
                try
                {
                    var manifest = await this.GetManifest(uri, true, nextHop);

                    JsonNode manifestJsonNode = Json.Json.Parse(manifest);
                    JsonNode infrastructureDependencies = manifestJsonNode[InfrastructureDependenciesKey];
                    JsonNode serviceDependencies = manifestJsonNode[ServiceDependenciesKey];

                    dependency.Add(InfrastructureDependenciesKey, infrastructureDependencies);
                    dependency.Add(ServiceDependenciesKey, serviceDependencies);
                }
                catch (Exception e)
                {
                    dependency["manifest"] = e.Message;
                }
            }
        }

        private async Task<string> GetManifest(Uri endpointUri, bool retry, int hop)
        {
            HttpWebRequest request = this.GetHttpWebRequest(endpointUri.AbsoluteUri, SupportedDependencies.Service.ToString());
            request.Headers[Constants.HOP_COUNT_HEADER_NAME] = hop.ToString();

            // This timeout defines the time it should take to connect to the instance.

            try
            {
                var getResponseTask = request.GetResponseAsync();
                // Give it the ReadWriteTimeout time minus the number of hops in seconds, to ensure that the initial point gets the configured time to finish.
                // This avoids problems when the dependency accepts the connection but does not give a response. (e.g. when IIS is still starting the application)
                var timeout = (this.ConnectTimeout + this.ReadWriteTimeout) - ((this.HopNumber - 1) * 1000);
                // Let it at least have a shot at delivering
                if (timeout < 1000)
                    timeout = 1000;
                var completedTask = await Task.WhenAny(getResponseTask, Task.Delay(timeout)).ConfigureAwait(false);
                if (completedTask == getResponseTask)
                {
                    // await the task again to propagate exceptions/cancellations
                    using (HttpWebResponse response = (HttpWebResponse)await getResponseTask.ConfigureAwait(false))
                    {
                        if ((int)response.StatusCode < 200 || (int)response.StatusCode > 299)
                        {
                            throw new NAMEException($"{SupportedDependencies.Service}: The service returned an unsuccessfull status code: {response.StatusCode}.");
                        }

                        var headerManifestEndpoint = response.Headers[Constants.MANIFEST_ENDPOINT_HEADER_NAME];
                        if (headerManifestEndpoint == null)
                            throw new DependencyWithoutNAMEException();

                        Uri uriFromHeader = new Uri(endpointUri, headerManifestEndpoint);

                        if (uriFromHeader != endpointUri && retry)
                        {
                            return await this.GetManifest(uriFromHeader, false, hop);
                        }
                        else
                        {
                            using (Stream stream = response.GetResponseStream())
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                return await reader.ReadToEndAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    request.Abort();
                    throw new NAMEException($"{SupportedDependencies.Service}: Timed out, the server accepted the connection but did not send a response.");
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    if (retry)
                    {
                        var manifestEndpoint = response.Headers[Constants.MANIFEST_ENDPOINT_HEADER_NAME];
                        if (manifestEndpoint == null)
                            throw new DependencyWithoutNAMEException();

                        return await this.GetManifest(new Uri(endpointUri, manifestEndpoint), false, hop);
                    }

                    if ((int)response.StatusCode == Constants.SERVICE_HOPS_ERROR_STATUS_CODE)
                        throw new NAMEException($"The maximum number of hops between manifest endpoints was reached.");
                    if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.RequestTimeout)
                        throw new DependencyNotReachableException(SupportedDependencies.Service.ToString(), ex);
                }
                throw new DependencyNotReachableException(SupportedDependencies.Service.ToString(), ex);
            }
        }
    }
}