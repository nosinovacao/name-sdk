using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NAME.WebApi
{
    /// <summary>
    /// Provides a mechanism to add the NAME endpoint as a custom header to all Web Api responses.
    /// </summary>
    internal class NAMEEndpointHttpModule : IHttpModule
    {
#pragma warning disable SA1401
        internal static string NameEndpoint = null;
#pragma warning restore SA1401

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            // This only works if running in IIS7+ Integrated Pipeline mode
            if (!HttpRuntime.UsingIntegratedPipeline)
                return;

            context.PreSendRequestHeaders += this.Context_PreSendRequestHeaders;
        }

        private void Context_PreSendRequestHeaders(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                app.Context.Response.AppendHeader(Constants.MANIFEST_ENDPOINT_HEADER_NAME, NameEndpoint);
            }
        }

    }
}
