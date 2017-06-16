using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAME.Core.Utils
{
    /// <summary>
    /// Represents a <see cref="BinaryWriter"/> that is aware to the Endianess of the Operating System. Performing bytes reversion on numeric values if needed.
    /// </summary>
    /// <seealso cref="System.IO.BinaryWriter" />
    internal class EndianAwareBinaryWriter : BinaryWriter
    {

        private bool isStreamLittleEndian;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="isStreamLittleEndian">if set to <c>true</c> the stream is ordered in little endian byte order.</param>
        public EndianAwareBinaryWriter(Stream s, bool isStreamLittleEndian)
            : this(s, new UTF8Encoding(false, true), isStreamLittleEndian)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="e">The encoding.</param>
        /// <param name="isStreamLittleEndian">if set to <c>true</c> the stream is ordered in little endian byte order.</param>
        public EndianAwareBinaryWriter(Stream s, Encoding e, bool isStreamLittleEndian)
            : this(s, e, false, isStreamLittleEndian)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAwareBinaryWriter"/> class.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="e">The encoding.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.BinaryReader object is disposed; otherwise, false.</param>
        /// <param name="isStreamLittleEndian">if set to <c>true</c> the stream is ordered in little endian byte order.</param>
        public EndianAwareBinaryWriter(Stream s, Encoding e, bool leaveOpen, bool isStreamLittleEndian)
            : base(s, e, leaveOpen)
        {
            this.isStreamLittleEndian = isStreamLittleEndian;
        }

        /// <summary>
        /// Writes a four-byte signed integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        public override void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.ReverseIfNeededAndWrite(bytes);
        }

        public override void Write(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.ReverseIfNeededAndWrite(bytes);
        }

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte signed integer to write.</param>
        public override void Write(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.ReverseIfNeededAndWrite(bytes);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        public override void Write(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.ReverseIfNeededAndWrite(bytes);
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        public override void Write(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.ReverseIfNeededAndWrite(bytes);
        }

        /// <summary>
        /// Writes a two-byte unsigned integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        public override void Write(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.ReverseIfNeededAndWrite(bytes);
        }

        private void ReverseIfNeededAndWrite(byte[] bytes)
        {
            if (this.isStreamLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.OutStream.Write(bytes, 0, bytes.Length);
        }

    }
}
