using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NAME.Core.Exceptions;
using NAME.Core;
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
        /// <returns>
        /// The <see cref="ParsedDependencies" /> object populated from the dependencies file
        /// </returns>
        /// <exception cref="NAMEException">Error parsing the dependencies file.</exception>
        /// <exception cref="DependenciesCheckException">Wrapper for all possible exceptions in the NAME process</exception>
        public static ParsedDependencies ReadAndLogDependencies(NAMEBaseConfiguration configuration, bool logToConsole, IFilePathMapper pathMapper, out NAMESettings settings)
        {
            var dependencies = new ParsedDependencies(null, null);

            settings = DependenciesReader.ReadNAMESettingsOverrides(configuration.DependenciesFilePath, pathMapper);

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

            var allStatuses = LogDependenciesStatuses(dependencies.InfrastructureDependencies, logToConsole);
            allStatuses.AddRange(LogDependenciesStatuses(dependencies.ServiceDependencies, logToConsole));

            if (configuration.ThrowOnDependenciesFail && allStatuses.Any(s => s.CheckStatus != NAMEStatusLevel.Ok))
            {
                throw new DependenciesCheckException(allStatuses);
            }

            return dependencies;
        }
    }
}
