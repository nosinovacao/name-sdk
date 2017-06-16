using NAME.Dependencies;
using System.Collections.Generic;

namespace NAME
{
    /// <summary>
    /// A NAME Configuration
    /// </summary>
    public class ParsedDependencies
    {
        /// <summary>
        /// Gets the infrastructure dependencies.
        /// </summary>
        /// <value>
        /// The infrastructure dependencies.
        /// </value>
        public IEnumerable<Dependency> InfrastructureDependencies { get; private set; }
        
        /// <summary>
        /// Gets the service dependencies.
        /// </summary>
        /// <value>
        /// The service dependencies.
        /// </value>
        public IEnumerable<Dependency> ServiceDependencies { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedDependencies"/> class.
        /// </summary>
        /// <param name="infrastructureDependencies">The infrastructure dependencies.</param>
        /// <param name="serviceDependencies">The service dependencies.</param>
        public ParsedDependencies(IList<Dependency> infrastructureDependencies, IList<Dependency> serviceDependencies)
        {
            this.InfrastructureDependencies = infrastructureDependencies ?? new List<Dependency>();
            this.ServiceDependencies = serviceDependencies ?? new List<Dependency>();
        }
    }
}
