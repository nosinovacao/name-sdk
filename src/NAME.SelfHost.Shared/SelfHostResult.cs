using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.SelfHost.Shared
{
    /// <summary>
    /// Represents an IDisposable proxy object for the result of the SelfHost servers start.
    /// Contains the parsed dependencies.
    /// </summary>
    /// <remarks>
    /// Disposing this object ensures that the underlying SelfHost server is stopped.
    /// </remarks>
    public class SelfHostResult : IDisposable
    {
        /// <summary>
        /// Gets the parsed dependencies.
        /// </summary>
        /// <value>
        /// The parsed dependencies.
        /// </value>
        public ParsedDependencies ParsedDependencies { get; }

        private IDisposable selfHostServer;

        internal SelfHostResult(ParsedDependencies parsedDependencies, IDisposable selfHostServer)
        {
            this.ParsedDependencies = parsedDependencies;
            this.selfHostServer = selfHostServer;
        }

        internal SelfHostResult(ParsedDependencies parsedDependencies)
        {
            this.ParsedDependencies = parsedDependencies;
            this.selfHostServer = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.selfHostServer?.Dispose();
        }
    }
}
