using NAME.Core.Exceptions;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Provides an abstraction for Version resolvers that connect to external services.
    /// </summary>
    public abstract class ConnectedVersionResolver : IVersionResolver
    {
        /// <summary>
        /// Gets or sets the connect timeout.
        /// </summary>
        /// <value>
        /// The connect timeout.
        /// </value>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the read and write timeout.
        /// </summary>
        /// <value>
        /// The read and write timeout.
        /// </value>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedVersionResolver"/> class.
        /// </summary>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="readWriteTimeout">The read and write timeout.</param>
        public ConnectedVersionResolver(int connectTimeout, int readWriteTimeout)
        {
            this.ConnectTimeout = connectTimeout;
            this.ReadWriteTimeout = readWriteTimeout;
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation. The result contains an enumerable with the versions.
        /// </returns>
        public abstract Task<IEnumerable<DependencyVersion>> GetVersions();

        /// <summary>
        /// Opens a TCP client.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="dependencyName">Name of the dependency.</param>
        /// <returns>Returns a TcpClient with the connection open.</returns>
        /// <exception cref="DependencyNotReachableException">Happens when the dependency can not be reached.</exception>
        protected async Task<TcpClient> OpenTcpClient(string host, int port, string dependencyName)
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient();

                client.NoDelay = true;
                client.ReceiveTimeout = this.ConnectTimeout;
                client.ReceiveTimeout = this.ReadWriteTimeout;
                client.SendTimeout = this.ReadWriteTimeout;
                await client.ConnectAsync(host, port).ConfigureAwait(false);

                return client;
            }
            catch (SocketException ex)
            {
#if NET45
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
