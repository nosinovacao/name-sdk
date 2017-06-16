using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAME.Core.Utils
{
    /// <summary>
    /// Represents a <see cref="BinaryReader"/> that is aware to the Endianess of the Operating System. Performing bytes reversion on numeric values if needed.
    /// </summary>
    /// <seealso cref="System.IO.BinaryReader" />
    internal class EndianAwareBinaryReader : BinaryReader
    {
        private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];

        private bool isStreamLittleEndian;
        
        /// <summary>
        /// Endians the aware binary writer.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="isStreamLittleEndian">if set to <c>true</c> the stream is ordered in little endian byte order.</param>
        public EndianAwareBinaryReader(Stream s, bool isStreamLittleEndian)
            : this(s, new UTF8Encoding(false, true), isStreamLittleEndian)
        {
        }

        /// <summary>
        /// Endians the aware binary writer.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="e">The encoding.</param>
        /// <param name="isStreamLittleEndian">if set to <c>true</c> the stream is ordered in little endian byte order.</param>
        public EndianAwareBinaryReader(Stream s, Encoding e, bool isStreamLittleEndian)
            : this(s, e, false, isStreamLittleEndian)
        {
        }

        /// <summary>
        /// Endians the aware binary writer.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="e">The encoding.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.BinaryReader object is disposed; otherwise, false.</param>
        /// <param name="isStreamLittleEndian">if set to <c>true</c> the stream is ordered in little endian byte order.</param>
        public EndianAwareBinaryReader(Stream s, Encoding e, bool leaveOpen, bool isStreamLittleEndian)
            : base(s, e, leaveOpen)
        {
            this.isStreamLittleEndian = isStreamLittleEndian;
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight bytes.
        /// </summary>
        /// <returns>
        /// An 8-byte unsigned integer read from this stream.
        /// </returns>
        public override ulong ReadUInt64()
        {
            this.a64 = this.ReadBytes(8);
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(this.a64);
            return BitConverter.ToUInt64(this.a64, 0);
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <returns>
        /// A 2-byte signed integer read from the current stream.
        /// </returns>
        public override short ReadInt16()
        {
            this.a16 = this.ReadBytes(2);
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(this.a16);
            return BitConverter.ToInt16(this.a16, 0);
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 4-byte signed integer read from the current stream.
        /// </returns>
        public override int ReadInt32()
        {
            this.a32 = this.ReadBytes(4);
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(this.a32);
            return BitConverter.ToInt32(this.a32, 0);
        }

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns>
        /// An 8-byte signed integer read from the current stream.
        /// </returns>
        public override long ReadInt64()
        {
            this.a64 = this.ReadBytes(8);
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(this.a64);
            return BitConverter.ToInt64(this.a64, 0);
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>
        /// A 2-byte unsigned integer read from this stream.
        /// </returns>
        public override ushort ReadUInt16()
        {
            this.a16 = this.ReadBytes(2);
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(this.a16);
            return BitConverter.ToUInt16(this.a16, 0);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 4-byte unsigned integer read from this stream.
        /// </returns>
        public override uint ReadUInt32()
        {
            this.a32 = this.ReadBytes(4);
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(this.a32);
            return BitConverter.ToUInt32(this.a32, 0);
        }

    }
}
