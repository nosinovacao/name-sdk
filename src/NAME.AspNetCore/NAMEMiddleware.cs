using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Template;
using NAME.Core;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;
using NAME.Utils;
using NAME.Hosting.Shared;

namespace NAME.AspNetCore
{
    /// <summary>
    /// Provides an Asp.NET core middleware for NAME.
    /// </summary>
    public class NAMEMiddleware
    {
        private string cachedHtml = null;
        private readonly RequestDelegate next;
        private readonly NAMEBaseConfiguration nameConfiguration;
        private readonly TemplateMatcher nameRequestMatcher;
        private readonly TemplateMatcher nameUiRequestMatcher;
        private readonly NAMESettings settings;
        private readonly IFilePathMapper pathMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="nameConfiguration">The name configuration.</param>
        /// <param name="settings">The name settings.</param>
        /// <param name="pathMapper">The path mapper.</param>
        public NAMEMiddleware(
                    RequestDelegate next,
                    NAMEBaseConfiguration nameConfiguration,
                    NAMESettings settings,
                    IFilePathMapper pathMapper)
        {
            this.next = next;
            this.nameConfiguration = nameConfiguration;
            this.settings = settings;
            this.pathMapper = pathMapper;


            if (string.IsNullOrEmpty(nameConfiguration.ManifestUriPrefix))
            {
                this.nameRequestMatcher = new TemplateMatcher(TemplateParser.Parse(Constants.MANIFEST_ENDPOINT.TrimStart('/')), new RouteValueDictionary());
                this.nameUiRequestMatcher = new TemplateMatcher(TemplateParser.Parse(Constants.MANIFEST_UI_ENDPOINT.TrimStart('/')), new RouteValueDictionary());
            }
            else
            {
                this.nameRequestMatcher = new TemplateMatcher(TemplateParser.Parse(nameConfiguration.ManifestUriPrefix.TrimEnd('/') + Constants.MANIFEST_ENDPOINT), new RouteValueDictionary());
                this.nameUiRequestMatcher = new TemplateMatcher(TemplateParser.Parse(nameConfiguration.ManifestUriPrefix.TrimEnd('/') + Constants.MANIFEST_UI_ENDPOINT), new RouteValueDictionary());
            }
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>Returns a task.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            string relativeManifestUrl = this.BuildRelativeNAMEPath(httpContext.Request);
            // Set headers
            httpContext.Response.Headers[Constants.MANIFEST_ENDPOINT_HEADER_NAME] = relativeManifestUrl;

            if (this.settings.IsManifestEndpointEnabled() && httpContext.Request.Method == "GET")
            {
                if (this.nameRequestMatcher.TryMatch(httpContext.Request.Path, new RouteValueDictionary()))
                {
                    Trace.TraceInformation("Received a Manifest request.");
                    await this.GetManifest(httpContext);
                }
                else if (this.nameUiRequestMatcher.TryMatch(httpContext.Request.Path, new RouteValueDictionary()))
                {
                    Trace.TraceInformation("Received a Manifest UI request.");
                    await this.GetManifestUI(httpContext);
                }
                else
                {
                    await this.next(httpContext);
                }
            }
            else
            {
                await this.next(httpContext);
            }

            // Take into consideration middlewares that clean the headers.
            if (!httpContext.Response.HasStarted && !httpContext.Response.Headers.ContainsKey(Constants.MANIFEST_ENDPOINT_HEADER_NAME))
                httpContext.Response.Headers[Constants.MANIFEST_ENDPOINT_HEADER_NAME] = relativeManifestUrl;
        }


        private async Task GetManifest(HttpContext context)
        {
            context.Response.ContentType = "application/json";


            int currentHopCount;
            if (context.Request.Headers.TryGetValue(Constants.HOP_COUNT_HEADER_NAME, out StringValues headerValues))
            {
                if (!int.TryParse(headerValues.First(), out currentHopCount))
                {
                    currentHopCount = 0;
                    Trace.TraceWarning($"The received hop count header it not a valid int value ({headerValues.First()}), defaulting to 0.");
                }
            }
            else
            {
                currentHopCount = 0;
            }
            currentHopCount++;

            var nameContext = new NAMEContext()
            {
                ServiceDependencyCurrentNumberOfHops = currentHopCount
            };

            var dependencies = DependenciesReader.ReadDependencies(this.nameConfiguration.DependenciesFilePath, this.pathMapper, this.settings, nameContext);

            if (currentHopCount == this.settings.ServiceDependencyMaxHops)
            {
                context.Response.StatusCode = Constants.SERVICE_HOPS_ERROR_STATUS_CODE;
                return;
            }

            string manifest = await ManifestGenerator.GenerateJson(this.nameConfiguration.APIName, this.nameConfiguration.APIVersion, dependencies);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(manifest);
        }

        private async Task GetManifestUI(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            UriBuilder builder = new UriBuilder(context.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? 80, this.BuildRelativeNAMEPath(context.Request));

            if (this.cachedHtml == null)
                this.cachedHtml = await ResourcesFetcher.GetNAMEUiAsync(builder.Uri.AbsoluteUri);

            await context.Response.WriteAsync(this.cachedHtml);
        }

        private PathString BuildRelativeNAMEPath(HttpRequest request)
        {
            var prefix = this.nameConfiguration.ManifestUriPrefix;

            if (!prefix.StartsWith("/"))
                prefix = prefix.Insert(0, "/");

            return request.PathBase.Add(prefix).Add(Constants.MANIFEST_ENDPOINT);

        }
    }
}
