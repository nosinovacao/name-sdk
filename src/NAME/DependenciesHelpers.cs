using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME
{
    /// <summary>
    /// Provides a mechanism to check all the dependencies.
    /// </summary>
    public static class DependenciesExtensions
    {
        /// <summary>
        /// Checks the dependencies.
        /// </summary>
        /// <param name="dependencies">The parsed dependencies.</param>
        /// <returns>Returns a task that represents the asynchronous operation.</returns>
        /// <exception cref="NAME.Core.Exceptions.DependenciesCheckException">Thrown when any of the dependencies checks do not pass.</exception>
        public static async Task CheckDependencies(this ParsedDependencies dependencies)
        {
            var dependenciesStatuses = await GetDependenciesStatutes(dependencies).ConfigureAwait(false);

            if (dependenciesStatuses.Any(d => d.CheckStatus != NAMEStatusLevel.Ok))
                throw new DependenciesCheckException(dependenciesStatuses);
        }

        /// <summary>
        /// Gets the dependencies statutes.
        /// </summary>
        /// <param name="dependencies">The parsed dependencies.</param>
        /// <returns>Returns a task that represents the asynchrnous operationg. The result contains an enumerable with all the dependencies checks statuses.</returns>
        public static async Task<IEnumerable<DependencyCheckStatus>> GetDependenciesStatutes(this ParsedDependencies dependencies)
        {
            List<DependencyCheckStatus> dependenciesStatuses = new List<DependencyCheckStatus>();

            dependenciesStatuses.AddRange(await GetDependenciesStatutes(dependencies.ServiceDependencies).ConfigureAwait(false));
            dependenciesStatuses.AddRange(await GetDependenciesStatutes(dependencies.InfrastructureDependencies).ConfigureAwait(false));

            return dependenciesStatuses;
        }

        private static async Task<IEnumerable<DependencyCheckStatus>> GetDependenciesStatutes(IEnumerable<Dependency> dependencies)
        {
            List<DependencyCheckStatus> dependenciesStatuses = new List<DependencyCheckStatus>();

            foreach (var dependency in dependencies)
            {
                DependencyCheckStatus checkStatus;
                try
                {
                    checkStatus = await dependency.GetStatus().ConfigureAwait(false);
                }
                catch (NAMEException ex)
                {
                    checkStatus = new DependencyCheckStatus(ex.StatusLevel, message: ex.Message, innerException: ex);
                }
                catch (Exception ex)
                {
                    checkStatus = new DependencyCheckStatus(NAMEStatusLevel.Error, message: $"An unexpected exception happened checking the state of the dependency.", innerException: ex);
                }
                dependenciesStatuses.Add(checkStatus);
            }
            return dependenciesStatuses;
        }
    }
}
