// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2016 Pivotal Software, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMq.
//
//  The Initial Developer of the Original Code is Pivotal Software, Inc.
//  Copyright (c) 2007-2016 Pivotal Software, Inc.  All rights reserved.
//---------------------------------------------------------------------------

//Changes:
// - Removed write methods.
// - Removed RabbitMq namespaces declarations
// - Renamed class

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NAME.RabbitMq
{
    /// <summary>
    /// Provides a mechanism to read AMQP types.
    /// </summary>
    internal static class AmqpTypesReader
    {
        /// <summary>
        /// Converts and AMQP decimal to decimal.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <param name="unsignedMantissa">The unsigned mantissa.</param>
        /// <returns>Returns the decimal value.</returns>
        /// <exception cref="System.Exception">Unrepresentable AMQP decimal table field: scale=value</exception>
        public static decimal AmqpToDecimal(byte scale, uint unsignedMantissa)
        {
            if (scale > 28)
            {
                throw new Exception("Unrepresentable AMQP decimal table field: " +
                                      "scale=" + scale);
            }
            return new decimal(
                (int)(unsignedMantissa & 0x7FFFFFFF),
                0,
                0,
                ((unsignedMantissa & 0x80000000) == 0) ? false : true,
                scale);
        }

        /// <summary>
        /// Reads an array.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns a List with the array.</returns>
        public static IList ReadArray(BinaryReader reader)
        {
            IList array = new List<object>();
            long arrayLength = reader.ReadUInt32();
            Stream backingStream = reader.BaseStream;
            long startPosition = backingStream.Position;
            while ((backingStream.Position - startPosition) < arrayLength)
            {
                object value = ReadFieldValue(reader);
                array.Add(value);
            }
            return array;
        }

        /// <summary>
        /// Reads a decimal.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the decimal.</returns>
        public static decimal ReadDecimal(BinaryReader reader)
        {
            byte scale = ReadOctet(reader);
            uint unsignedMantissa = ReadLong(reader);
            return AmqpToDecimal(scale, unsignedMantissa);
        }

        /// <summary>
        /// Reads a field value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the value.</returns>
        public static object ReadFieldValue(BinaryReader reader)
        {
            object value = null;
            byte discriminator = reader.ReadByte();
            switch ((char)discriminator)
            {
                case 'S':
                    value = ReadLongstr(reader);
                    break;
                case 'I':
                    value = reader.ReadInt32();
                    break;
                case 'D':
                    value = ReadDecimal(reader);
                    break;
                //case 'T':
                //    value = ReadTimestamp(reader);
                //    break;
                case 'F':
                    value = ReadTable(reader);
                    break;

                case 'A':
                    value = ReadArray(reader);
                    break;
                case 'b':
                    value = reader.ReadSByte();
                    break;
                case 'd':
                    value = reader.ReadDouble();
                    break;
                case 'f':
                    value = reader.ReadSingle();
                    break;
                case 'l':
                    value = reader.ReadInt64();
                    break;
                case 's':
                    value = reader.ReadInt16();
                    break;
                case 't':
                    value = ReadOctet(reader) != 0;
                    break;
                //case 'x':
                //    value = new BinaryTableValue(ReadLongstr(reader));
                //    break;
                case 'V':
                    value = null;
                    break;
                default:
                    value = null;
                    break;
                    //throw new Exception("Unrecognised type in table: " +
                    //                      (char)discriminator);
            }
            return value;
        }

        /// <summary>
        /// Reads an AMQP long.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns an unsigned int.</returns>
        public static uint ReadLong(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Reads an AMQP longlong.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Return sna unsigned long.</returns>
        public static ulong ReadLonglong(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        /// <summary>
        /// Reads an AMQP longstr.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns an array containing the bytes.</returns>
        /// <exception cref="System.Exception">Long string too long;</exception>
        public static byte[] ReadLongstr(BinaryReader reader)
        {
            uint byteCount = reader.ReadUInt32();
            if (byteCount > int.MaxValue)
            {
                throw new Exception("Long string too long; " +
                                      "byte length=" + byteCount + ", max=" + int.MaxValue);
            }
            return reader.ReadBytes((int)byteCount);
        }

        /// <summary>
        /// Reads an AMQP octet.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the byte.</returns>
        public static byte ReadOctet(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// Reads an AMQP short.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the unsigned short.</returns>
        public static ushort ReadShort(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        /// <summary>
        /// Reads an AMQP shortstr.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the string.</returns>
        public static string ReadShortstr(BinaryReader reader)
        {
            int byteCount = reader.ReadByte();
            byte[] readBytes = reader.ReadBytes(byteCount);
            return Encoding.UTF8.GetString(readBytes, 0, readBytes.Length);
        }

        /// <summary>
        /// Reads an AMQP "table" definition from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// A <seealso cref="System.Collections.Generic.IDictionary{TKey,TValue}" />.
        /// </returns>
        /// <remarks>
        /// Supports the AMQP 0-8/0-9 standard entry types S, I, D, T
        /// and F, as well as the QPid-0-8 specific b, d, f, l, s, t,
        /// x and V types and the AMQP 0-9-1 A type.
        /// </remarks>
        public static IDictionary<string, object> ReadTable(BinaryReader reader)
        {
            IDictionary<string, object> table = new Dictionary<string, object>();
            long tableLength = reader.ReadUInt32();

            Stream backingStream = reader.BaseStream;
            long startPosition = backingStream.Position;
            while ((backingStream.Position - startPosition) < tableLength)
            {
                string key = ReadShortstr(reader);
                object value = ReadFieldValue(reader);

                if (!table.ContainsKey(key))
                {
                    table[key] = value;
                }
            }

            return table;
        }
    }
}
