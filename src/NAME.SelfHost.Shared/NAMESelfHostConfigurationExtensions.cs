using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.SelfHost.Shared
{
    internal static class NAMESelfHostConfigurationExtensions
    {
        public static string GetManifestPath(this NAMESelfHostConfiguration configuration, string requestHost, int? requestPort)
        {
            string baseUrl = $"http://{requestHost}";
            if (requestPort != null)
                baseUrl += $":{requestPort}";
            string normalizedPrefix = string.Empty;

            if (configuration.ManifestUriPrefix != "/")
                normalizedPrefix = '/' + configuration.ManifestUriPrefix.TrimStart('/').TrimEnd('/');

            return baseUrl + normalizedPrefix + Constants.MANIFEST_ENDPOINT;
        }
    }
}
