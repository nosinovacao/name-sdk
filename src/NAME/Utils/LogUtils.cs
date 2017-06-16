using NAME.Core;
using NAME.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Utils
{
    /// <summary>
    /// Log-related util functions
    /// </summary>
    public static class LogUtils
    {
        /// <summary>
        /// Logs with information importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logToConsole">if set to <c>true</c> logs to console.</param>
        public static void LogInfo(string message, bool logToConsole)
        {
            System.Diagnostics.Trace.TraceInformation(message);
            if (logToConsole)
                Console.WriteLine("[INFO] " + message);
        }

        /// <summary>
        /// Logs with warning importance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logToConsole">if set to <c>true</c> logs to console.</param>
        public static void LogWarning(string message, bool logToConsole)
        {
            System.Diagnostics.Trace.TraceWarning(message);
            if (logToConsole)
                Console.WriteLine("[WARN] " + message);
        }

        /// <summary>
        /// Logs the dependencies statuses.
        /// </summary>
        /// <param name="dependencies">The dependencies.</param>
        /// <param name="logToConsole">if set to <c>true</c> logs to console.</param>
        /// <returns>A list with the status for all the dependencies.</returns>
        public static List<DependencyCheckStatus> LogDependenciesStatuses(IEnumerable<Dependency> dependencies, bool logToConsole)
        {
            List<DependencyCheckStatus> allStatuses = new List<DependencyCheckStatus>();
            foreach (var dependency in dependencies)
            {
                var status = dependency.GetStatus().ConfigureAwait(false).GetAwaiter().GetResult();
                string versionStr = status.Version != null ? $" ({status.Version})" : string.Empty;

                if (status.CheckPassed)
                    LogInfo($"{dependency}{versionStr} check passed.", logToConsole);
                else
                    LogWarning($"{dependency}{versionStr} check failed with the message: {status.Message}.", logToConsole);

                allStatuses.Add(status);
            }
            return allStatuses;
        }
    }
}
