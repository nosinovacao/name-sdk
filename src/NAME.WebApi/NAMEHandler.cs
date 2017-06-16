using System;
using NAME.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using NAME.Hosting.Shared;
using static NAME.Utils.LogUtils;

namespace NAME.WebApi
{
    internal class NAMEHandler : HttpMessageHandler
    {

        private readonly string apiName;
        private readonly string apiVersion;
        private readonly string dependenciesFile;
        private readonly NAMESettings settings;
        private readonly IFilePathMapper pathMapper;

        public NAMEHandler(string apiName, string apiVersion, string dependenciesFile, IFilePathMapper pathMapper, NAMESettings settings)
        {
            this.apiName = apiName;
            this.apiVersion = apiVersion;
            this.dependenciesFile = dependenciesFile;
            this.pathMapper = pathMapper;
            this.settings = settings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!this.settings.IsManifestEndpointEnabled())
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            int currentHopCount;
            if (request.Headers.TryGetValues(Constants.HOP_COUNT_HEADER_NAME, out IEnumerable<string> headerValues))
            {
                if (!int.TryParse(headerValues.First(), out currentHopCount))
                {
                    currentHopCount = 0;
                    LogWarning($"The received hop count header it not a valid int value ({headerValues.First()}), defaulting to 0.", false);
                }
            }
            else
            {
                currentHopCount = 0;
            }
            currentHopCount++;

            if (currentHopCount == this.settings.ServiceDependencyMaxHops)
                return new HttpResponseMessage { StatusCode = (HttpStatusCode)Constants.SERVICE_HOPS_ERROR_STATUS_CODE };

            var context = new NAMEContext()
            {
                ServiceDependencyCurrentNumberOfHops = currentHopCount
            };

            ParsedDependencies innerDependencies = DependenciesReader.ReadDependencies(this.dependenciesFile, this.pathMapper, this.settings, context);
            var manifest = await ManifestGenerator.GenerateJson(this.apiName, this.apiVersion, innerDependencies).ConfigureAwait(false);

            var content = new StringContent(manifest);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return new HttpResponseMessage { Content = content };
        }
    }
}

