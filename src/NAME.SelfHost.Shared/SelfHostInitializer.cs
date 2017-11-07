using NAME.Core;
using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using static NAME.Hosting.Shared.DependenciesUtils;
using static NAME.Utils.LogUtils;

namespace NAME.SelfHost.Shared
{
    internal static class SelfHostInitializer
    {
        public static ParsedDependencies Initialize(ISelfHostServer server, IFilePathMapper pathMapper, NAMESelfHostConfiguration configuration)
        {

            ParsedDependencies dependencies = ReadAndLogDependencies(
                configuration,
                configuration.LogHealthCheckToConsole,
                pathMapper,
                out NAMESettings settings);

            if (settings.RunningMode == SupportedNAMEBehaviours.NAMEDisabled)
            {
                LogInfo("Not starting NAME since it is disabled in the dependencies file.", configuration.LogHealthCheckToConsole);
                return dependencies;
            }

            int portNumber = settings.SelfHostPortRangeFirst;
            try
            {
                bool result = false;
                LogInfo($"Starting the server", configuration.LogHealthCheckToConsole);
                while (portNumber <= settings.SelfHostPortRangeLast)
                {
                    result = server.Start(portNumber, settings);
                    if (result)
                        break;
                    portNumber++;
                }
                if (result == false)
                    throw new NAMEException("Tried all ports, without success.", NAMEStatusLevel.Error);

                Console.WriteLine($"Succesfully started the server. Listening on {portNumber}.");
            }
            catch (Exception ex)
            {
                LogWarning($"Could not bind to the SelfHost address: {ex.Message}.", configuration.LogHealthCheckToConsole);
                if (configuration.ThrowOnDependenciesFail)
                {
                    if (ex is NAMEException)
                        throw;
                    else
                        throw new NAMEException("Could not bind to the SelfHost address.", ex, NAMEStatusLevel.Error);
                }
                return dependencies;
            }

            if (settings.RunningMode < SupportedNAMEBehaviours.BootstrapDisabled)
            {
                var register = new Registration.Register();

                register.RegisterInstance(
                    pathMapper,
                    configuration.APIName,
                    configuration.APIVersion,
                    configuration.DependenciesFilePath,
                    settings,
                    Environment.MachineName,
                    (uint)portNumber,
                    typeof(SelfHostInitializer).GetTypeInfo().Assembly.GetName().Version.ToString(),
                    (configuration.ManifestUriPrefix?.TrimEnd('/') ?? string.Empty) + Constants.MANIFEST_ENDPOINT,
                    Constants.REGISTRY_SUPPORTED_PROTOCOL_VERSIONS);
            }

            if (settings.RunningMode == SupportedNAMEBehaviours.NAMEDisabled)
            {
                LogInfo("Stopping the NAME self host server since it is disabled in the Registry.", configuration.LogHealthCheckToConsole);
                server.Dispose();
                return dependencies;
            }

            return dependencies;
        }
    }
}
