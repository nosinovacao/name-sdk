using NAME.Core;
using NAME.SelfHost.Shared;
using SocketHttpListener.Net;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAME.Utils;
using System.Reflection;
using NAME.Hosting.Shared;
using static NAME.Utils.LogUtils;

namespace NAME.SelfHost.HttpListener
{
    internal class HttpListenerServer : ISelfHostServer
    {
        private SocketHttpListener.Net.HttpListener httpListener;
        private NAMEHttpListenerConfiguration nameConfiguration;
        private NAMESettings settings;
        private byte[] cachedHtml;
        private IFilePathMapper pathMapper;

        public int? Port { get; private set; }

        public HttpListenerServer(NAMEHttpListenerConfiguration nameConfiguration, IFilePathMapper pathMapper)
        {
            this.pathMapper = pathMapper;
            this.nameConfiguration = nameConfiguration;
        }

        public bool Start(int port, NAMESettings settings)
        {
            try
            {
                this.settings = settings;

                this.httpListener = new SocketHttpListener.Net.HttpListener();
                this.httpListener.Prefixes.Add($"http://{this.nameConfiguration.AddressToListenOn}:{port}/{this.nameConfiguration.ManifestUriPrefix.TrimStart('/').TrimEnd('/')}/");
                this.httpListener.OnContext = this.OnContext;
                this.httpListener.Start();

                this.Port = port;

                return true;
            }
            catch (System.Net.HttpListenerException)
            {
                this.httpListener?.Stop();
                return false;
            }

        }

        private void OnContext(HttpListenerContext context)
        {
            try
            {
                context.Response.Headers.Add(Constants.MANIFEST_ENDPOINT_HEADER_NAME, this.nameConfiguration.GetRelativeManifestPath());

                if (this.settings.IsManifestEndpointEnabled())
                {
                    if (context.Request.Url.AbsolutePath.Equals(this.nameConfiguration.ManifestUriPrefix.TrimEnd('/') + Constants.MANIFEST_ENDPOINT))
                    {
                        this.GetManifest(context, this.nameConfiguration).GetAwaiter().GetResult();
                    }
                    else if (context.Request.Url.AbsolutePath.Equals(this.nameConfiguration.ManifestUriPrefix.TrimEnd('/') + Constants.MANIFEST_UI_ENDPOINT))
                    {
                        this.GetManifestUI(context);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                context.Response.Close();
            }
            catch (Exception ex)
            {
                LogWarning("Exception when handling the request." + Environment.NewLine + ex.Message, this.nameConfiguration.LogHealthCheckToConsole);
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private async Task GetManifest(HttpListenerContext context, NAMEHttpListenerConfiguration configuration)
        {
            context.Response.ContentType = "application/json";

            int currentHopCount = 0;
            var hopCountHeaders = context.Request.Headers.GetValues(Constants.HOP_COUNT_HEADER_NAME);
            if (hopCountHeaders?.Length > 0)
            {
                if (!int.TryParse(hopCountHeaders.First(), out currentHopCount))
                {
                    currentHopCount = 0;
                    LogWarning($"The received hop count header it not a valid int value ({hopCountHeaders.First()}), defaulting to 0.", this.nameConfiguration.LogHealthCheckToConsole);
                }
            }

            currentHopCount++;

            var nameContext = new NAMEContext()
            {
                ServiceDependencyCurrentNumberOfHops = currentHopCount
            };

            var dependencies = DependenciesReader.ReadDependencies(configuration.DependenciesFilePath, this.pathMapper, this.settings, nameContext);

            if (currentHopCount == this.settings.ServiceDependencyMaxHops)
            {
                context.Response.StatusCode = Constants.SERVICE_HOPS_ERROR_STATUS_CODE;
                return;
            }

            string manifest = await ManifestGenerator.GenerateJson(configuration.APIName, configuration.APIVersion, dependencies).ConfigureAwait(false);

            context.Response.ContentType = "application/json";

            var manifestBytes = Encoding.UTF8.GetBytes(manifest);

            context.Response.OutputStream.Write(manifestBytes, 0, manifestBytes.Length);
        }

        private void GetManifestUI(HttpListenerContext context)
        {
            context.Response.ContentType = "text/html";
            string manifestLink = this.nameConfiguration.GetRelativeManifestPath();

            if (this.cachedHtml == null)
                this.cachedHtml = Encoding.UTF8.GetBytes(ResourcesFetcher.GetNAMEUi(manifestLink));

            context.Response.OutputStream.Write(this.cachedHtml, 0, this.cachedHtml.Length);
        }


        public void Dispose()
        {
            this.httpListener?.Stop();
        }

    }
}
