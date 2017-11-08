using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that happens when the dependencies checks fail.
    /// </summary>
    /// <seealso cref="NAME.Core.Exceptions.NAMEException" />
    public class DependenciesCheckException : AggregateException
    {
        /// <summary>
        /// Gets the dependencies statutes.
        /// </summary>
        /// <value>
        /// The dependencies statutes.
        /// </value>
        public IEnumerable<DependencyCheckStatus> DependenciesStatutes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependenciesCheckException" /> class.
        /// </summary>
        /// <param name="dependenciesStatuses">The dependencies statuses.</param>
        public DependenciesCheckException(IEnumerable<DependencyCheckStatus> dependenciesStatuses)
            : base(string.Join(Environment.NewLine, dependenciesStatuses.Where(s => s.CheckStatus != NAMEStatusLevel.Ok).Select(s => s.Message)), dependenciesStatuses.Where(d => d.CheckStatus != NAMEStatusLevel.Ok && d.InnerException != null).Select(d => d.InnerException))
        {
            this.DependenciesStatutes = dependenciesStatuses;
        }
    }
}
