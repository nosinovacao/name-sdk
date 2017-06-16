using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Provides a mechanism to resolve the version of a service.
    /// </summary>
    public interface IVersionResolver //<TVersion> where TVersion : DependencyVersion
    {
        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <returns>Returns a task that represents the asynchronous operation. The result contains an enumerable with the versions.</returns>
        Task<IEnumerable<DependencyVersion>> GetVersions();
    }
}
