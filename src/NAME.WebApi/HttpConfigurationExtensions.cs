using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Dependencies;
using System.Diagnostics;
using static NAME.Hosting.Shared.DependenciesUtils;
using static NAME.Utils.LogUtils;

namespace NAME.WebApi
{
    /// <summary>
    /// Provides extension methods for the <see cref="HttpConfiguration"/> class.
    /// </summary>
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Enables the NAME middleware.
        /// </summary>
        /// <param name="httpConfig">The HTTP configuration.</param>
        /// <param name="configure">The configuration action.</param>
        /// <returns>Returns the dependencies parsed from the dependencies file.</returns>
        public static ParsedDependencies EnableNAME(this HttpConfiguration httpConfig, Action<NAMEConfiguration> configure)
        {
            var config = new NAMEConfiguration();
            configure?.Invoke(config);

            var pathMapper = new WebApiFilePathMapper();

            var dependencies = ReadAndLogDependencies(config, false, pathMapper, out NAMESettings settings);

            if (settings.RunningMode == SupportedNAMEBehaviours.NAMEDisabled)
            {
                LogInfo("Not starting NAME since it is disabled in the dependencies file.", false);
                return dependencies;
            }

            var routeTemplate = string.IsNullOrEmpty(config.ManifestUriPrefix) ? Constants.MANIFEST_ENDPOINT.TrimStart('/') : config.ManifestUriPrefix.TrimEnd('/') + Constants.MANIFEST_ENDPOINT;

            var nameEndpoint = $"/{routeTemplate}";

            if (!string.IsNullOrEmpty(httpConfig.VirtualPathRoot))
            {
                nameEndpoint = httpConfig.VirtualPathRoot.TrimEnd('/') + nameEndpoint;
            }

            // Register in the registry

            if (settings.RunningMode < SupportedNAMEBehaviours.BootstrapDisabled)
            {
                var register = new Registration.Register();
                
                register.RegisterInstance(
                    pathMapper,
                    config.APIName,
                    config.APIVersion,
                    config.DependenciesFilePath,
                    settings,
                    Environment.MachineName,
                    null,
                    typeof(NAMEHandler).Assembly.GetName().Version.ToString(),
                    nameEndpoint,
                    Constants.REGISTRY_SUPPORTED_PROTOCOL_VERSIONS);
            }

            httpConfig.Routes.MapHttpRoute(
                "NAME_Manifest",
                routeTemplate,
                null,
                null,
                new NAMEHandler(config.APIName, config.APIVersion, config.DependenciesFilePath, pathMapper, settings));

            httpConfig.Routes.MapHttpRoute(
                "NAME_Ui",
                routeTemplate + "/ui",
                null,
                null,
                new NAMEUiHandler(settings));

            // This is hacky and should be revised.
            // been revised, still the same
            NAMEEndpointHttpModule.NameEndpoint = nameEndpoint;
            HttpApplication.RegisterModule(typeof(NAMEEndpointHttpModule));

            return dependencies;
        }
    }
}
