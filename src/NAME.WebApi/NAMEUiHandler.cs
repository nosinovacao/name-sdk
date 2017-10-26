using NAME.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NAME.Core.Utils;
using NAME.Utils;
using NAME.Hosting.Shared;
using System.Net;

namespace NAME.WebApi
{
    internal class NAMEUiHandler : HttpMessageHandler
    {
        private volatile string cachedHtml = null;
        private readonly NAMESettings settings;

        public NAMEUiHandler(NAMESettings settings)
        {
            this.settings = settings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!this.settings.IsManifestEndpointEnabled())
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            string manifestLink = request.GetUrlHelper().Route("NAME_Manifest", new { });

            if (this.cachedHtml == null)
                this.cachedHtml = await ResourcesFetcher.GetNAMEUiAsync(manifestLink);

            var content = new StringContent(this.cachedHtml);

            content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return new HttpResponseMessage { Content = content };
        }
    }
}
