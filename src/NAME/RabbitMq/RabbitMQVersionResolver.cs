using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NAME.RabbitMq
{
    /// <summary>
    /// Provides a mechanism to get the version of RabbitMq.
    /// </summary>
    ///// <seealso cref="IVersionResolver{TVersion}" />
    public class RabbitMqVersionResolver : ConnectedVersionResolver
    {
        private IConnectionStringProvider connectionStringProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqVersionResolver" /> class.
        /// </summary>
        /// <param name="connectionStringProvider">The connection string provider.</param>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="readWriteTimeout">The read write timeout.</param>
        public RabbitMqVersionResolver(IConnectionStringProvider connectionStringProvider, int connectTimeout, int readWriteTimeout)
            : base(connectTimeout, readWriteTimeout)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <returns>
        /// Returns an enumerable with only an item, containing the RabbitMq version.
        /// </returns>
        /// <exception cref="NAMEException">An unexpected exception happened. See inner exception for details.</exception>
        public override async Task<IEnumerable<DependencyVersion>> GetVersions()
        {
            if (!this.connectionStringProvider.TryGetConnectionString(out var rabbitConnectionString))
                throw new ConnectionStringNotFoundException(this.connectionStringProvider.ToString());

            var result = new List<DependencyVersion>();
            foreach (string connectionString in rabbitConnectionString.Split(','))
            {
                Uri rabbitMQUri = new Uri(connectionString.Trim());
                using (var client = await this.OpenTcpClient(rabbitMQUri.Host, rabbitMQUri.Port, SupportedDependencies.RabbitMq.ToString()).ConfigureAwait(false))
                {
                    SendConnectionHeader(client);
                    string version = GetVersionFromServerReponse(client);
                    result.Add(DependencyVersionParser.Parse(version, false));
                }
            }
            return result;
        }

        private static string GetVersionFromServerReponse(TcpClient client)
        {
            EndianAwareBinaryReader reader = new EndianAwareBinaryReader(client.GetStream(), false);
            try
            {
                byte type = reader.ReadByte();
                if (type != 1)
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server responded with wrong type ({type}).", NAMEStatusLevel.Error);
                //Skip channel
                reader.ReadUInt16();
                //Read Length
                uint length = reader.ReadUInt32();
                //Read the rest of the message into memory. The length of Connection.Start will never be bigger than int.max
                byte[] buffer = reader.ReadBytes((int)length);
                reader.Dispose();
                reader = new EndianAwareBinaryReader(new MemoryStream(buffer), false);
                //Read class (should be Connection (10))
                ushort rClass = reader.ReadUInt16();
                if (rClass != 10)
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server responded with wrong class ({rClass}).", NAMEStatusLevel.Error);
                //Read method (should be Start (10))
                ushort rMethod = reader.ReadUInt16();
                if (rMethod != 10)
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server responded with wrong method ({rMethod}).", NAMEStatusLevel.Error);
                //Read AMQP major version (should be 0)
                byte major = reader.ReadByte();
                if (major != 0)
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server responded with wrong AMQP major version ({major}).", NAMEStatusLevel.Error);
                //Read AMQP minor version (should be 9)
                byte minor = reader.ReadByte();
                if (major != 0)
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server responded with wrong AMQP minor version ({minor}).", NAMEStatusLevel.Error);
                IDictionary<string, object> serverProperties = AmqpTypesReader.ReadTable(reader);
                if (!serverProperties.ContainsKey("version"))
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server did not send a server-properties table!", NAMEStatusLevel.Error);
                if (!(serverProperties["version"] is byte[]))
                    throw new NAMEException($"{SupportedDependencies.RabbitMq}: Server returned a version which is not a string!", NAMEStatusLevel.Error);
                var versionStr = Encoding.UTF8.GetString((byte[])serverProperties["version"]);
                return versionStr;
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        private static void SendConnectionHeader(System.Net.Sockets.TcpClient client)
        {
            using (BinaryWriter writer = new BinaryWriter(client.GetStream(), Encoding.UTF8, true))
            {
                writer.Write(Encoding.UTF8.GetBytes("AMQP"));
                writer.Write('\x00');
                writer.Write('\x00');
                writer.Write('\x09');
                writer.Write('\x01');
            }
        }
    }
}
