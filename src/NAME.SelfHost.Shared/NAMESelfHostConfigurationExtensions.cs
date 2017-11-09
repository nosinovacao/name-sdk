using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.SelfHost.Shared
{
    internal static class NAMESelfHostConfigurationExtensions
    {
        public static string GetRelativeManifestPath(this NAMESelfHostConfiguration configuration)
        {
            string normalizedPrefix = string.Empty;

            if (!string.IsNullOrWhiteSpace(configuration.ManifestUriPrefix) && configuration.ManifestUriPrefix != "/")
                normalizedPrefix = '/' + configuration.ManifestUriPrefix.TrimStart('/').TrimEnd('/');

            return normalizedPrefix + Constants.MANIFEST_ENDPOINT;
        }
    }
}
