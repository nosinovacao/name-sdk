using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

//Based on the project in https://github.com/samus/mongodb-csharp under the Apache-2.0 license

namespace NAME.MongoDb
{
    /// <summary>
    /// Provides a mechanism to read a Mongo connection string.
    /// </summary>
    internal class MongoConnectionStringBuilder
    {
        /// <summary>
        /// The Default database.
        /// </summary>
        public const string DefaultDatabase = "admin";
        
        private static readonly Regex ServerRegex = new Regex(@"^\s*([^:]+)(?::(\d+))?\s*$");
        private static readonly Regex UriRegex = new Regex(@"^mongodb://(?:[^:]*:[^@]*@)?([^/]*)(?:/([^?]*))?(?:\?.*)?$");

        private readonly List<MongoServerEndPoint> servers = new List<MongoServerEndPoint>();

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref = "MongoConnectionStringBuilder" />
        ///   class. Uses the default server connection when
        ///   no server is added.
        /// </summary>
        public MongoConnectionStringBuilder()
        {
            this.Database = DefaultDatabase;
        }

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref = "MongoConnectionStringBuilder" />
        ///   class. Uses the default server connection when
        ///   no server is added.
        /// </summary>
        /// <param name = "connectionString">The connection string.</param>
        public MongoConnectionStringBuilder(string connectionString)
            : this()
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (connectionString.StartsWith("mongodb://"))
                    this.ParseUri(connectionString);
                else
                    throw new NAMEException("The connection string must start with mongodb://");
            }
        }

        /// <summary>
        ///   Gets the servers.
        /// </summary>
        /// <value>The servers.</value>
        public MongoServerEndPoint[] Servers
        {
            get
            {
                return this.servers.Count == 0 ? new[] { MongoServerEndPoint.Default } : this.servers.ToArray();
            }
        }
        
        /// <summary>
        ///   Gets or sets the database.
        /// </summary>
        /// <remarks>
        ///   Is only used when passing directly constructing MongoDatabase instance.
        /// </remarks>
        /// <value>The database.</value>
        public string Database { get; set; }

        /// <summary>
        ///   Parses the URI.
        /// </summary>
        /// <param name = "connectionString">The connection string.</param>
        private void ParseUri(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var uriMatch = UriRegex.Match(connectionString);

            if (!uriMatch.Success)
                throw new FormatException(string.Format("Invalid connection string: {0}", connectionString));
            
            var servers = uriMatch.Groups[1].Value;
            if (!string.IsNullOrEmpty(servers))
                this.ParseServers(servers);

            var database = uriMatch.Groups[2].Value;
            if (!string.IsNullOrEmpty(database))
                this.Database = database;
        }

        /// <summary>
        ///   Parses the servers.
        /// </summary>
        /// <param name = "value">The value.</param>
        private void ParseServers(string value)
        {
            var servers = value.Split(',');

            foreach (var serverMatch in servers.Select(server => ServerRegex.Match(server)))
            {
                if (!serverMatch.Success)
                    throw new FormatException(string.Format("Invalid server in connection string: {0}", serverMatch.Value));

                var serverHost = serverMatch.Groups[1].Value;

                int port;
                if (int.TryParse(serverMatch.Groups[2].Value, out port))
                    this.AddServer(serverHost, port);
                else
                    this.AddServer(serverHost);
            }
        }

        /// <summary>
        ///   Adds the server.
        /// </summary>
        /// <param name = "endPoint">The end point.</param>
        public void AddServer(MongoServerEndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            this.servers.Add(endPoint);
        }

        /// <summary>
        ///   Clears the servers.
        /// </summary>
        public void ClearServers()
        {
            this.servers.Clear();
        }

        /// <summary>
        ///   Adds the server with the given host and default port.
        /// </summary>
        /// <param name = "host">The host.</param>
        public void AddServer(string host)
        {
            this.AddServer(new MongoServerEndPoint(host));
        }

        /// <summary>
        ///   Adds the server with the given host and port.
        /// </summary>
        /// <param name = "host">The host.</param>
        /// <param name = "port">The port.</param>
        public void AddServer(string host, int port)
        {
            this.AddServer(new MongoServerEndPoint(host, port));
        }

        /// <summary>
        ///   Returns a
        ///   <see cref = "string" />
        ///   that represents this instance.
        /// </summary>
        /// <returns>A
        ///   <see cref = "string" />
        ///   that represents this instance.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            
            if (this.servers.Count > 0)
            {
                builder.Append("Server=");

                foreach (var server in this.servers)
                {
                    builder.Append(server.Host);

                    if (server.Port != MongoServerEndPoint.DefaultPort)
                        builder.AppendFormat(":{0}", server.Port);

                    builder.Append(',');
                }

                // remove last ,
                builder.Remove(builder.Length - 1, 1);

                builder.Append(';');
            }

            // remove last ;
            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}