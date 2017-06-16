using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core.Exceptions
{
    /// <summary>
    /// Happens when dependency is not reachable by NAME.
    /// </summary>
    /// <seealso cref="NAME.Core.Exceptions.NAMEException" />
    public class DependencyNotReachableException : NAMEException
    {
        /// <summary>
        /// Gets the name of the dependency.
        /// </summary>
        /// <value>
        /// The name of the dependency.
        /// </value>
        public string DependencyName { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyNotReachableException" /> class.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency.</param>
        /// <param name="message">The message that represents the error.</param>
        /// <param name="inner">Inner exception.</param>
        public DependencyNotReachableException(string dependencyName, string message, Exception inner)
            : base(message, inner)
        {
            this.DependencyName = dependencyName;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyNotReachableException"/> class.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency.</param>
        public DependencyNotReachableException(string dependencyName)
            : this(dependencyName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyNotReachableException"/> class.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency.</param>
        /// <param name="inner">Inner exception.</param>
        public DependencyNotReachableException(string dependencyName, Exception inner)
            : this(dependencyName, $"Dependency {dependencyName} is not reachable!", inner)
        {

        }
    }
}
