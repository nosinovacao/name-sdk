using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NAME.Core.Utils
{
    /// <summary>
    /// Provides helper methods for Sockets.
    /// </summary>
    internal static class SocketHelpers
    {
        /// <summary>
        /// Initializes a <see cref="TcpClient" /> and connects to the specified host/port combination.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="dependencyName">Name of the dependency.</param>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="readWriteTimeout">The read write timeout.</param>
        /// <returns>
        /// Returns the initialized and connected <see cref="TcpClient" />.
        /// </returns>
        /// <exception cref="DependencyNotReachableException">Happens when the connection to the host is not successfull.</exception>
        public static async Task<TcpClient> OpenTcpClient(string host, int port, string dependencyName, int connectTimeout, int readWriteTimeout)
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient();

                client.NoDelay = true;
                client.ReceiveTimeout = connectTimeout;
                client.ReceiveTimeout = readWriteTimeout;
                client.SendTimeout = readWriteTimeout;
                await client.ConnectAsync(host, port).ConfigureAwait(false);

                return client;
            }
            catch (SocketException ex)
            {
#if NET462
                if (client.Connected)
                    client?.GetStream()?.Close();
                client?.Close();
#else
                client?.Dispose();
#endif
                throw new DependencyNotReachableException(dependencyName, ex);
            }
        }
    }
}
