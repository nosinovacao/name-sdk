using NAME.Core.Exceptions;
using NAME.Core.Utils;
using NAME.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NAME.SqlServer
{
    /// <summary>
    /// Provides a mechanism to get the version of a SQL Server.
    /// </summary>
    ///// <seealso cref="IVersionResolver{TVersion}" />
    public class SqlServerVersionResolver : ConnectedVersionResolver
    {
        private IConnectionStringProvider connectionStringProvider;
        private static Regex dataSourceRegex = new Regex("((Data Source)|(Server))=(.*?);", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerVersionResolver" /> class.
        /// </summary>
        /// <param name="connectionStringProvider">The connection string provider.</param>
        /// <param name="connectTimeout">The connect timeout.</param>
        /// <param name="readWriteTimeout">The read write timeout.</param>
        /// <exception cref="NAMEException">The SQL Server connection string must contain a Data Source or Server value.</exception>
        public SqlServerVersionResolver(IConnectionStringProvider connectionStringProvider, int connectTimeout, int readWriteTimeout)
            : base(connectTimeout, readWriteTimeout)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Gets the version of the SQL Server.
        /// </summary>
        /// <returns>
        /// Returns an enumerable with a single element containing the version of the SQL Server.
        /// </returns>
        /// <exception cref="NAMEException">An unexpected exception happened. See inner exception for details.</exception>
        public override async Task<IEnumerable<DependencyVersion>> GetVersions()
        {
            this.ExtractHostnameAndPort(out string hostname, out int port);

            byte[] message = this.CreatePreLoginMessage();

            using (var client = await this.OpenTcpClient(hostname, port, SupportedDependencies.SqlServer.ToString()))
            {
                await client.GetStream().WriteAsync(message, 0, message.Length, default(CancellationToken)).ConfigureAwait(false);
                string versionStr = this.GetServerVersionFromResponse(client.GetStream());
                return new List<DependencyVersion> { DependencyVersionParser.Parse(versionStr, false) };
            }
        }

        private void ExtractHostnameAndPort(out string hostname, out int port)
        {
            hostname = string.Empty;
            port = 1433;
            if (!this.connectionStringProvider.TryGetConnectionString(out string connectionString))
                throw new ConnectionStringNotFoundException(this.connectionStringProvider.ToString());

            var matches = dataSourceRegex.Matches(connectionString);

            string dataSource = null;
            for (int i = 0; i < matches.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(matches[0].Groups[4].Value))
                {
                    dataSource = matches[0].Groups[4].Value;
                    break;
                }
            }

            if (dataSource == null)
                throw new NAMEException("The SQL Server connection string must contain a Data Source or Server value.");

            var parts = dataSource.Split(',');
            hostname = parts[0];
            if (parts.Count() > 1)
            {
                if (!int.TryParse(parts[1], out port))
                    port = 1433;
            }
        }

        private string GetServerVersionFromResponse(Stream stream)
        {
            using (var reader = new EndianAwareBinaryReader(stream, Encoding.UTF8, true, false))
            {

                //Read Type (should be response = 4)
                int type = reader.ReadByte();
                if (type != 4)
                    throw new NAMEException($"{SupportedDependencies.SqlServer}: Server responded with wrong Type ({type}).");
                //Skip Status bit mask
                reader.ReadByte();
                //Read the full message length
                ushort length = reader.ReadUInt16();
                //Skip Channel
                reader.ReadUInt16();
                //Skip Packet Number
                reader.ReadByte();
                //Skip Window
                reader.ReadByte();
                //Read the rest of the message
                //preLoginBuffer = reader.ReadBytes(length - 2 - 1 - 1 - 2 - 1 - 1);
                //Read first option token (should be Version = 0)
                int token = reader.ReadByte();
                if (token != 0)
                    throw new NAMEException($"{SupportedDependencies.SqlServer}: Server responded with wrong Token ({token}).");
                //Read the offset
                ushort offset = reader.ReadUInt16();
                //Read the length (should be 6)
                ushort optionlength = reader.ReadUInt16();
                if (optionlength != 6)
                    throw new NAMEException($"{SupportedDependencies.SqlServer}: Server responded with an invalid version length ({length}).");
                //Skip everything until the version.
                reader.ReadBytes(offset - 2 - 2 - 1);
                int major = reader.ReadByte();
                int minor = reader.ReadByte();
                int build = reader.ReadUInt16();

                return $"{major}.{minor}.{build}";
            }
        }

        private byte[] CreatePreLoginMessage()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new EndianAwareBinaryWriter(ms, Encoding.UTF8, true, false))
            {
                // THE HEADER

                //Type (TDS7 pre-login message)
                writer.Write((byte)18);
                //Status bitmask (0x01)
                writer.Write((byte)1);
                //Length placeholder
                writer.Write((short)0);
                //SPID
                writer.Write((short)0);
                //Packet Number
                writer.Write((byte)1);
                //Window
                writer.Write((byte)0);


                // THE PRE-LOGIN MESSAGE

                //1st option
                //Option Token (Version = 0)
                writer.Write((byte)0);
                //Option value offset placeholder
                int versionValueOffsetIndex = (int)ms.Length;
                writer.Write((short)0);
                //Option length
                writer.Write((short)6);
                //Last option
                //Option Token (Terminator = 255)
                writer.Write((byte)255);
                //Set the option value index
                long versionValueOffset = ms.Length - versionValueOffsetIndex + 1;
                //Write the version (Mimic System.Data.SQLClient version = 4.6.1082.0)
                writer.Write((byte)4);
                writer.Write((byte)6);
                writer.Write((short)1082);
                writer.Write((short)0);

                writer.Flush();

                byte[] buffer = ms.ToArray();
                int bufferLength = (int)ms.Length;

                BitConverter.GetBytes((short)ms.Length).Reverse().ToArray().CopyTo(buffer, 2);
                BitConverter.GetBytes((short)versionValueOffset).Reverse().ToArray().CopyTo(buffer, versionValueOffsetIndex);

                return buffer;
            }
        }
    }
}
