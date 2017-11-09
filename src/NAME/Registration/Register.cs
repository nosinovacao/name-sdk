using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using NAME.Core;
using NAME.Core.DTOs;
using NAME.Core.Exceptions;
using NAME.DigestHelpers;
using NAME.DTOs;
using NAME.Core.Utils;
using static NAME.Utils.LogUtils;

namespace NAME.Registration
{
    /// <summary>
    /// Manages the registry workflow between the NAME instances and one or many registrys
    /// </summary>
    public class Register
    {
        private Task RegisterTask { get; set; }
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private string ApiName { get; set; }
        private string ApiVersion { get; set; }
        private NAMESettings settings { get; set; }

        private string CurrentDigest { get; set; } = string.Empty;
        private string Hostname { get; set; }
        private uint? Port { get; set; }
        private string NameVersion { get; set; }
        private string NameEndpoint { get; set; }
        private uint[] SupportedProtocols { get; set; }

        private string SessionId { get; set; }
        private uint Protocol { get; set; }
        private string RegistryEndpoint { get; set; } = null;

        private string dependenciesFileLocation;

        private IFilePathMapper pathMapper;

        /// <summary>
        /// Registers the instance in one or more registars.
        /// </summary>
        /// <param name="pathMapper">The path mapper.</param>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="apiVersion">The API version.</param>
        /// <param name="dependenciesFileLocation">The dependencies file location.</param>
        /// <param name="settings">The context.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="port">The port.</param>
        /// <param name="nameVersion">The name version.</param>
        /// <param name="nameEndpoint">The name endpoint.</param>
        /// <param name="supportedProtocols">The supported protocols.</param>
        /// <exception cref="System.ArgumentException">Too many dots. - nameVersion</exception>
        /// <exception cref="System.ArgumentNullException">nameConfig
        /// or
        /// hostname
        /// or
        /// vpath
        /// or
        /// nameVersion
        /// or
        /// nameEndpoint
        /// or
        /// supportedProtocols</exception>
        public void RegisterInstance(
            IFilePathMapper pathMapper,
            string apiName,
            string apiVersion,
            string dependenciesFileLocation,
            NAMESettings settings,
            string hostname,
            uint? port,
            string nameVersion,
            string nameEndpoint,
            params uint[] supportedProtocols)
        {
            this.pathMapper = Guard.NotNull(pathMapper, nameof(pathMapper));
            // todo guard
            this.settings = Guard.NotNull(settings, nameof(settings));
            this.Hostname = Guard.NotNull(hostname, nameof(hostname));
            this.NameVersion = Guard.NotNull(nameVersion, nameof(nameVersion));
            this.NameEndpoint = Guard.NotNull(nameEndpoint, nameof(nameEndpoint));
            this.SupportedProtocols = Guard.NotNull(supportedProtocols, nameof(supportedProtocols));

            this.dependenciesFileLocation = dependenciesFileLocation;
            this.ApiName = apiName;
            this.ApiVersion = apiVersion;
            this.Hostname = hostname;
            this.Port = port;

            int dotsCount = nameVersion.Length - nameVersion.Replace(".", string.Empty).Length;
            if (dotsCount > 3)
                throw new ArgumentException("Too many dots.", nameof(nameVersion));

            this.NameVersion = nameVersion.Substring(0, nameVersion.LastIndexOf('.'));

            this.SupportedProtocols = supportedProtocols;

            if (this.settings.RegistryEndpoints.Length == 0)
            {
                LogInfo("No registry endpoints to register this api", true);
                return;
            }

            LogInfo("RegisterInstance started", true);

            this.RegisterTask = Task.Factory.StartNew(this.RegisterLoop, this.cancellationTokenSource.Token);
        }

        /// <summary>
        /// Cancels the register task.
        /// </summary>
        public void Cancel()
        {
            this.cancellationTokenSource.Cancel();
        }

        private async Task RegisterLoop()
        {
            await this.Bootstrap().ConfigureAwait(false);

            if (this.settings.RunningMode >= SupportedNAMEBehaviours.HeartbeatDisabled)
            {
                LogInfo($"The {nameof(this.settings.RunningMode)} was set to {this.settings.RunningMode.ToString()}. Leaving the register loop.", true);
                return;
            }

            try
            {
                await this.Announce().ConfigureAwait(false);

                var nextAnnounce = DateTime.Now.Add(this.settings.RegistryReAnnounceFrequency);
                var nextPing = DateTime.Now.Add(this.settings.RegistryPingFrequency);

                while (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    DateTime nextWait = nextAnnounce < nextPing ? nextAnnounce : nextPing;

                    var delay = nextWait - DateTime.Now;

                    if (delay < TimeSpan.Zero)
                        delay = TimeSpan.Zero;

                    await Task.Delay(delay, this.cancellationTokenSource.Token).ConfigureAwait(false);

                    if (nextWait == nextPing)
                    {
                        await this.Ping().ConfigureAwait(false);
                        nextPing = DateTime.Now.Add(this.settings.RegistryPingFrequency);
                    }
                    else
                    {
                        await this.Announce().ConfigureAwait(false);
                        nextAnnounce = DateTime.Now.Add(this.settings.RegistryReAnnounceFrequency);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                LogInfo($"The {nameof(this.RegisterLoop)} was cancelled. Exiting.", true);
            }
            catch (NAMEException)
            {
                // start again
                await Task.Delay(this.settings.RegistryPingFrequency).ConfigureAwait(false);
                this.RegisterTask = Task.Factory.StartNew(this.RegisterLoop, this.cancellationTokenSource.Token);
            }
        }

        private async Task Ping()
        {
            HttpWebRequest request = GetWebRequest(this.RegistryEndpoint + $"/registrar/{this.SessionId}");
            request.Method = "HEAD";

            HttpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var msg = "Unable to send new manifest to the registry. Status code = " + response.StatusCode;
                LogWarning(msg, true);
                throw new NAMEException(msg, NAMEStatusLevel.Error);
            }
        }

        private async Task Announce()
        {
            HttpWebRequest request = GetWebRequest(this.RegistryEndpoint + $"/registrar/{this.SessionId}/manifest");

            string manifest = await this.GetManifest(new NAMEContext()).ConfigureAwait(false);
            string digest = DigestHelper.GetDigestForMessage(manifest);

            if (this.CurrentDigest != digest && this.settings.RunningMode < SupportedNAMEBehaviours.AnnounceDisabled)
            {
                var announceObj = new SendManifestDTO
                {
                    Manifest = manifest
                };

                var serializer = new DataContractJsonSerializer(typeof(SendManifestDTO));

                using (Stream stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    serializer.WriteObject(stream, announceObj);
                }

                HttpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "Unable to send new manifest to the registry. Status code = " + response.StatusCode;
                    LogWarning(msg, true);
                    throw new NAMEException(msg, NAMEStatusLevel.Warn);
                }
                else
                {
                    this.CurrentDigest = digest;
                }
            }
            else
            {
                await this.Ping().ConfigureAwait(false);
            }
        }

        private async Task Bootstrap()
        {
            // safeguard
            if (this.settings.RunningMode >= SupportedNAMEBehaviours.BootstrapDisabled)
                return;

            var announceInfo = new BootstrapDTO
            {
                AppName = this.ApiName,
                AppVersion = this.ApiVersion,
                Hostname = this.Hostname,
                SupportedProtocols = this.SupportedProtocols,
                NAMEVersion = this.NameVersion,
                NAMEEndpoint = this.NameEndpoint,
                NAMEPort = this.Port
            };

            do
            {

                foreach (var registryEndpoint in this.settings.RegistryEndpoints)
                {
                    BootstrapResultDto bootstrapData = null;
                    Func<CancellationToken, Task<BootstrapResultDto>> bootstrapTaskFunction = async (token) =>
                    {
                        try
                        {
                            HttpWebRequest request = GetWebRequest(registryEndpoint + "/registrar");
                            var serializer = new DataContractJsonSerializer(typeof(BootstrapDTO));
                            var deserializer = new DataContractJsonSerializer(typeof(BootstrapResultDto));
                            using (Stream stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                            {
                                serializer.WriteObject(stream, announceInfo);
                            }
                            token.ThrowIfCancellationRequested();

                            using (HttpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse)
                            {
                                token.ThrowIfCancellationRequested();

                                if (response == null)
                                {
                                    LogWarning("Cannot register with " + registryEndpoint, true);
                                    return null;
                                }

                                if (response.StatusCode == HttpStatusCode.Conflict)
                                {
                                    LogWarning($"Cannot register with {registryEndpoint}. The server did not accept any protocol.", true);
                                    return null;
                                }
                                var result = deserializer.ReadObject(response.GetResponseStream()) as BootstrapResultDto;
                                if (result == null)
                                    LogWarning("Could not deserialize the Boostrap Result.", true);

                                return result;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogWarning($"Could not register with {registryEndpoint}: {ex.Message}", true);
                            return null;
                        }
                    };

                    var cancellationTokenSource = new CancellationTokenSource();

                    Task<BootstrapResultDto> bootstrapTask = bootstrapTaskFunction(cancellationTokenSource.Token);
                    if (await Task.WhenAny(bootstrapTask, Task.Delay(this.settings.RegistryBootstrapTimeout)) == bootstrapTask)
                    {
                        // The bootstrap task finished before the timeout.
                        cancellationTokenSource.Cancel();
                        bootstrapData = await bootstrapTask;
                    }
                    else
                    {
                        // The bootstrap task took longer then the timeout
                        cancellationTokenSource.Cancel();
                        LogInfo($"The bootstrap timedout for registry {registryEndpoint}.", true);
                        continue;
                    }


                    if (bootstrapData == null)
                    {
                        continue;
                    }

                    this.Protocol = bootstrapData.Protocol;
                    this.SessionId = bootstrapData.SessionId;
                    this.RegistryEndpoint = registryEndpoint;

                    if (this.SupportedProtocols.Contains(this.Protocol) == false)
                    {
                        LogWarning("Given protocol is not supported", true);
                        continue;
                    }

                    if (bootstrapData.Overrides == null)
                        break;

                    // process overrides
                    var overrides = bootstrapData.Overrides;

                    if (overrides.RunningMode > this.settings.RunningMode)
                        this.settings.RunningMode = overrides.RunningMode;

                    if (overrides.RegistryEndpoints.Length > 0)
                        this.settings.RegistryEndpoints = overrides.RegistryEndpoints;

                    if (overrides.ConnectedDependencyShowConnectionString)
                        this.settings.ConnectedDependencyShowConnectionString = true;

                    if (overrides.DependencyConnectTimeout > 0)
                        this.settings.DependencyConnectTimeout = overrides.DependencyConnectTimeout;

                    if (overrides.DependencyReadWriteTimeout > 0)
                        this.settings.DependencyReadWriteTimeout = overrides.DependencyReadWriteTimeout;

                    if (overrides.RegistryPingFrequency != null && overrides.RegistryPingFrequency != TimeSpan.Zero)
                        this.settings.RegistryPingFrequency = overrides.RegistryPingFrequency;

                    if (overrides.RegistryReAnnounceFrequency != null &&
                        overrides.RegistryReAnnounceFrequency != TimeSpan.Zero)
                        this.settings.RegistryReAnnounceFrequency = overrides.RegistryReAnnounceFrequency;

                    if (overrides.ServiceDependencyMaxHops > 0)
                        this.settings.ServiceDependencyMaxHops = overrides.ServiceDependencyMaxHops;

                    //all done
                    break;
                }

                if (this.RegistryEndpoint == null)
                {
                    var msg = $"Could not register with any registry. Waiting { this.settings.RegistryBootstrapRetryFrequency } to try again.";
                    LogWarning(msg, true);
                    await Task.Delay(this.settings.RegistryBootstrapRetryFrequency).ConfigureAwait(false);
                }
                else
                {
                    LogInfo($"Bootstrapped in the registry with endpoint {this.RegistryEndpoint}.", true);
                }
            }
            while (this.RegistryEndpoint == null);
        }

        private static HttpWebRequest GetWebRequest(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Accept = "application/json";
            request.ContentType = request.Accept;
            request.Method = "POST";
            return request;
        }

        private async Task<string> GetManifest(NAMEContext context)
        {
            ParsedDependencies innerDependencies = DependenciesReader.ReadDependencies(this.dependenciesFileLocation, this.pathMapper, this.settings, context);
            return await ManifestGenerator.GenerateJson(this.ApiName, this.ApiVersion, innerDependencies).ConfigureAwait(false);
        }
    }
}