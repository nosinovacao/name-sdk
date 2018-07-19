using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NAME.Core.Exceptions;
using NAME.Core;
using System.Threading.Tasks;
using NAME.Json;
using static NAME.Utils.LogUtils;

namespace NAME.Hosting.Shared
{
    /// <summary>
    /// Dependency-related util methods
    /// </summary>
    public static class DependenciesUtils
    {

        /// <summary>
        /// Reads the and log dependencies.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logToConsole">if set to <c>true</c> [log to console].</param>
        /// <param name="pathMapper">The path mapper.</param>
        /// <param name="settings">The context information.</param>
        /// <param name="connectionStringProviderOverrider">The function that may be used to override the connection string provider each dependency wil use</param>
        /// <returns>
        /// The <see cref="ParsedDependencies" /> object populated from the dependencies file
        /// </returns>
        /// <exception cref="NAMEException">Error parsing the dependencies file.</exception>
        /// <exception cref="DependenciesCheckException">Wrapper for all possible exceptions in the NAME process</exception>
        public static ParsedDependencies ReadAndLogDependencies(NAMEBaseConfiguration configuration, bool logToConsole, IFilePathMapper pathMapper, out NAMESettings settings, Func<IJsonNode, IConnectionStringProvider> connectionStringProviderOverrider = null)
        {
            var dependencies = new ParsedDependencies(null, null);

            settings = DependenciesReader.ReadNAMESettingsOverrides(configuration.DependenciesFilePath, pathMapper);
            settings.ConnectionStringProviderOverride = connectionStringProviderOverrider;

            try
            {
                dependencies = DependenciesReader.ReadDependencies(configuration.DependenciesFilePath, pathMapper, settings, new NAMEContext());
            }
            catch (NAMEException ex)
            {

                LogWarning($"Could not parse the dependencies file: {ex.Message}.", logToConsole);
                if (configuration.ThrowOnDependenciesFail)
                {
                    if (ex is NAMEException)
                        throw;
                    else
                        throw new NAMEException("Error parsing the dependencies file.", ex, NAMEStatusLevel.Error);
                }
                return dependencies;
            }

            if (settings.RunningMode == SupportedNAMEBehaviours.NAMEDisabled)
            {
                LogInfo("NAME was disabled in the dependencies file.", logToConsole);
                return dependencies;
            }

            LogInfo("Starting the dependencies state logs.", logToConsole);

            Func<IEnumerable<DependencyCheckStatus>> logStatusesAction = () =>
            {
                var allStatuses = LogDependenciesStatuses(dependencies.InfrastructureDependencies, logToConsole);
                allStatuses.AddRange(LogDependenciesStatuses(dependencies.ServiceDependencies, logToConsole));
                return allStatuses;
            };

            if (configuration.ThrowOnDependenciesFail)
            {
                var allStatuses = logStatusesAction();
                if (allStatuses.Any(s => s.CheckStatus != NAMEStatusLevel.Ok))
                    throw new DependenciesCheckException(allStatuses);
            }
            else
            {
                Task.Factory
                    .StartNew(logStatusesAction)
                    .ContinueWith(
                        task =>
                        {
                            LogWarning("Exception logging dependencies status:", logToConsole);
                            var flattened = task.Exception.Flatten();
                            flattened.Handle(ex =>
                            {
                                LogWarning(ex.Message, logToConsole);
                                return true;
                            });
                        },
                        TaskContinuationOptions.OnlyOnFaulted);
            }

            return dependencies;
        }
    }
}
