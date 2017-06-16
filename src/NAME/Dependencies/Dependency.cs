using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Json;

namespace NAME.Dependencies
{
    /// <summary>
    /// A Dependency
    /// </summary>
    public abstract class Dependency
    {
        /// <summary>
        /// Determines the dependency status.
        /// </summary>
        /// <returns>Returns a task that represents the asynchronous operation. The result contains the status of the dependency.</returns>
        public abstract Task<DependencyCheckStatus> GetStatus();

        /// <summary>
        /// Gets the manifest representation of this instance in JSON format.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation. The result contains a <see cref="JsonNode" /> that represents this instance manifest representation in JSON format.
        /// </returns>
        internal abstract Task<JsonNode> ToJson();
    }
}