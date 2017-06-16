using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.SelfHost.Shared
{
    /// <summary>
    /// Provides mechanisms for handling the SelfHost servers.
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal interface ISelfHostServer : IDisposable
    {
        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Returns true if the server is successfully started. Else returns false.
        /// </returns>
        bool Start(int port, NAMESettings settings);
    }
}
