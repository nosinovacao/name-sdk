using NAME.Core;
using NAME.Core.Exceptions;
using NAME.MongoDb.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAME.MongoDb
{
    /// <summary>
    /// Provides a mechanism to get the version of MongoDb.
    /// </summary>
    public class MongoDbVersionResolver : ConnectedVersionResolver
    {
        private const short MAX_RETRY_COUNT = 3;
        private IConnectionStringProvider connectionStringProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbVersionResolver" /> class.
        /// </summary>
        /// <param name="connectionStringProvider">The connection string provider.</param>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="readWriteTimeout">The read write timeout.</param>
        public MongoDbVersionResolver(IConnectionStringProvider connectionStringProvider, int connectTimeout, int readWriteTimeout)
            : base(connectTimeout, readWriteTimeout)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <returns>
        /// Returns an enumerable with the MongoDb versions.
        /// </returns>
        /// <exception cref="NAMEException">Thrown when an unexpected exception happens.</exception>
        public override async Task<IEnumerable<DependencyVersion>> GetVersions()
        {
            var versions = new List<DependencyVersion>();
            string connectionString;
            if (!this.connectionStringProvider.TryGetConnectionString(out connectionString))
                throw new ConnectionStringNotFoundException(this.connectionStringProvider.ToString());

            var connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);

            byte[] message = this.CreateServerStatusMessagePayload(connectionStringBuilder);

            foreach (var mongoEndpoint in connectionStringBuilder.Servers)
            {
                using (var client = await this.OpenTcpClient(mongoEndpoint.Host, mongoEndpoint.Port, SupportedDependencies.MongoDb.ToString()))
                {
                    await client.GetStream().WriteAsync(message, 0, message.Length, default(CancellationToken)).ConfigureAwait(false);
                    await Task.Delay(100).ConfigureAwait(false);
                    BSONObject obj = ExtractServerStatusFromResponse(client);
                    versions.Add(DependencyVersion.Parse(obj["version"].stringValue));
                }
            }

            return versions;
        }

        private static BSONObject ExtractServerStatusFromResponse(TcpClient client)
        {
            using (var binaryReader = new BinaryReader(client.GetStream(), new UTF8Encoding(), true))
            {
                //We don't care about the message length and DB RequestId, skip 2 int32's.
                binaryReader.ReadBytes(8);

                //Response To
                int responseTo = binaryReader.ReadInt32();
                if (responseTo != 0)
                    throw new NAMEException($"{SupportedDependencies.MongoDb}: The server responed with an unexpected response code ({responseTo}).");

                //Op Code
                int opCode = binaryReader.ReadInt32();
                if (opCode != 1)
                    throw new NAMEException($"{SupportedDependencies.MongoDb}: The server responed with an unexpected operation code ({opCode}).");

                //We don't care about responseFlags, cursorID or startingFrom. Skip them.
                binaryReader.ReadBytes(4 + 8 + 4);

                //Number of documents
                var numberOfDocuments = binaryReader.ReadInt32();
                if (numberOfDocuments != 1)
                    throw new NAMEException($"{SupportedDependencies.MongoDb}: The server responded with an unexpected number of documents ({numberOfDocuments}).");

                //The ServerStatus document
                int size = binaryReader.ReadInt32();
                byte[] buffer = new byte[size];
                BitConverter.GetBytes(size).CopyTo(buffer, 0);
                binaryReader.Read(buffer, 4, size - 4);

                BSONObject obj = SimpleBSON.Load(buffer);
                return obj;
            }
        }

        private byte[] CreateServerStatusMessagePayload(MongoConnectionStringBuilder connectionStringBuilder)
        {
            byte[] message;
            int commandLength;
            //Write the message first because we need the it's length
            using (var commandMemoryStream = new MemoryStream())
            using (var commandWriter = new BinaryWriter(commandMemoryStream, new UTF8Encoding(), true))
            {
                //Query options
                commandWriter.Write(0);
                //Collection name
                commandWriter.Write($"{connectionStringBuilder.Database}.$cmd".ToArray());
                //cstring's require a \x00 at the end.
                commandWriter.Write('\x00');
                //Number to skip
                commandWriter.Write(0);
                //Number to return
                commandWriter.Write(-1);

                var commandBSON = new BSONObject();
                commandBSON["serverStatus"] = 1.0;
                commandWriter.Write(SimpleBSON.Dump(commandBSON));
                commandWriter.Flush();

                commandLength = (int)commandMemoryStream.Length;
                message = new byte[16 + commandLength];
                Array.Copy(commandMemoryStream.ToArray(), 0, message, 16, commandLength);
            }

            using (var messageMemoryStream = new MemoryStream(message))
            using (var completeMessageWriter = new BinaryWriter(messageMemoryStream, new UTF8Encoding(), true))
            {
                //Message length
                completeMessageWriter.Write(16 + commandLength);
                //Request Id
                completeMessageWriter.Write(0);
                //Response To
                completeMessageWriter.Write(0);
                //Operation Code
                completeMessageWriter.Write(2004);
                completeMessageWriter.Flush();
            }
            return message;
        }
    }
}