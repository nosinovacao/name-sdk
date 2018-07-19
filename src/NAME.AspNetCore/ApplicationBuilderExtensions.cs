using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.IO;
using NAME.Core;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using NAME.Json;
using NAME.Core.Exceptions;
using static NAME.Hosting.Shared.DependenciesUtils;
using static NAME.Utils.LogUtils;

namespace NAME.AspNetCore
{
    /// <summary>
    /// Provides extensions to use NAME on an <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Sets up NAME.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configBuilder">The configuration builder.</param>
        /// <returns>Returns the <see cref="ParsedDependencies"/>.</returns>
        public static ParsedDependencies UseNAME(
            this IApplicationBuilder app,
            Action<NAMEAspNetCoreConfiguration> configBuilder)
        {
            var config = new NAMEAspNetCoreConfiguration();
            configBuilder?.Invoke(config);

            return UseNAME(app, config);
        }

        /// <summary>
        /// Sets up NAME.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>Returns the <see cref="ParsedDependencies"/>.</returns>
        public static ParsedDependencies UseNAME(
            this IApplicationBuilder app,
            NAMEAspNetCoreConfiguration config)
        {
            IConnectionStringProvider connectionStringProviderOverride(IJsonNode node)
            {
                if (node["locator"]?.Value == null)
                    return null;

                if (!node["locator"].Value.Equals("IConfiguration", StringComparison.OrdinalIgnoreCase))
                    return null;

                if (config.Configuration == null)
                    throw new NAMEException("To use the 'IConfiguration' locator you must add the IConfiguration in the configuration.", NAMEStatusLevel.Warn);

                var key = node["key"]?.Value;
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("key", "The key must be specified with the 'IConfiguration' locator.");

                return new ConfigurationProviderConnectionStringProvider(config.Configuration, key);
            }


            var env = app.ApplicationServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
            var filePathMapper = new AspNetCoreFilePathMapper(env);
            var dependencies = ReadAndLogDependencies(config, false, filePathMapper, out NAMESettings settings, connectionStringProviderOverride);

            if (settings.RunningMode == SupportedNAMEBehaviours.NAMEDisabled)
            {
                LogInfo("Not starting NAME since it is disabled in the dependencies file.", false);
                return dependencies;
            }

            if (settings.RunningMode < SupportedNAMEBehaviours.BootstrapDisabled)
            {
                var register = new Registration.Register();

                //TODO: NAME-32

                register.RegisterInstance(
                    filePathMapper,
                    config.APIName,
                    config.APIVersion,
                    config.DependenciesFilePath,
                    settings,
                    Environment.MachineName,
                    null,
                    typeof(ApplicationBuilderExtensions).GetTypeInfo().Assembly.GetName().Version.ToString(),
                    (config.ManifestUriPrefix?.TrimEnd('/') ?? string.Empty) + Constants.MANIFEST_ENDPOINT,
                    Constants.REGISTRY_SUPPORTED_PROTOCOL_VERSIONS);
            }

            if (settings.RunningMode == SupportedNAMEBehaviours.NAMEDisabled)
            {
                LogInfo("Not starting NAME since it is disabled in the Registry.", false);
                return dependencies;
            }

            app.UseMiddleware<NAMEMiddleware>(config, settings, filePathMapper);

            return dependencies;
        }
    }
}
