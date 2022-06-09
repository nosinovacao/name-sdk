using NAME.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

//Taken from https://github.com/kernys/Kernys.Bson under the MIT license

namespace NAME.MongoDb.Bson
{
    internal class BSONValue
    {
        public enum ValueType
        {
            Double,
            String,
            Array,
            Binary,
            Boolean,
            UTCDateTime,
            None,
            Int32,
            Int64,
            Object,
            Decimal
        }

        private ValueType mValueType;
        private double tdouble;
        private decimal tdecimal;
        private string tstring;
        private byte[] binary;
        private bool tbool;
        private DateTime dateTime;
        private int int32;
        private long int64;

        /*
        protected static BSONValue convert(object obj) {
            if (obj as BSONValue != null)
                return obj as BSONValue;

            if (obj is Int32)
                return new BSONValue (obj as Int32);
            if (obj is Int64)
                return new BSONValue (obj as Int64);
            if (obj is byte[])
                return new BSONValue (obj as byte[]);
            if (obj is DateTime)
                return new BSONValue (obj as DateTime);
            if (obj is string)
                return new BSONValue (obj as string);
            if (obj is bool)
                return new BSONValue (obj as bool);
            if (obj is double)
                return new BSONValue (obj as double);

            throw new InvalidCastException();
        }
        */

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        /// Properties
        public ValueType valueType
        {
            get { return this.mValueType; }
        }

        public double doubleValue
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Int32:
                        return (double)this.int32;

                    case ValueType.Int64:
                        return (double)this.int64;

                    case ValueType.Double:
                        return this.tdouble;

                    case ValueType.Decimal:
                        return (double)this.tdecimal;

                    case ValueType.None:
                        return float.NaN;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to double", this.mValueType));
            }
        }

        public decimal decimalValue
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Double:
                        return (decimal)this.tdouble;

                    case ValueType.Int32:
                        return (decimal)this.int32;

                    case ValueType.Int64:
                        return (decimal)this.int64;

                    case ValueType.Decimal:
                        return this.tdecimal;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to decimal", this.mValueType));
            }
        }

        public int int32Value
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Int32:
                        return (int)this.int32;

                    case ValueType.Int64:
                        return (int)this.int64;

                    case ValueType.Double:
                        return (int)this.tdouble;

                    case ValueType.Decimal:
                        return (int)this.tdecimal;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to Int32", this.mValueType));
            }
        }

        public long int64Value
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Int32:
                        return (long)this.int32;

                    case ValueType.Int64:
                        return (long)this.int64;

                    case ValueType.Double:
                        return (long)this.tdouble;

                    case ValueType.Decimal:
                        return (long)this.tdecimal;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to Int64", this.mValueType));
            }
        }

        public byte[] binaryValue
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Binary:
                        return this.binary;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to binary", this.mValueType));
            }
        }

        public DateTime dateTimeValue
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.UTCDateTime:
                        return this.dateTime;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to DateTime", this.mValueType));
            }
        }

        public string stringValue
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Int32:
                        return Convert.ToString(this.int32);

                    case ValueType.Int64:
                        return Convert.ToString(this.int64);

                    case ValueType.Double:
                        return Convert.ToString(this.tdouble);

                    case ValueType.Decimal:
                        return Convert.ToString(this.tdecimal);

                    case ValueType.String:
                        return this.tstring != null ? this.tstring.TrimEnd(new char[] { (char)0 }) : null;

                    case ValueType.Binary:
                        return Encoding.UTF8.GetString(this.binary).TrimEnd(new char[] { (char)0 });
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to string", this.mValueType));
            }
        }

        public bool boolValue
        {
            get
            {
                switch (this.mValueType)
                {
                    case ValueType.Boolean:
                        return this.tbool;
                }

                throw new Exception(string.Format("Original type is {0}. Cannot convert from {0} to bool", this.mValueType));
            }
        }

        public bool isNone
        {
            get { return this.mValueType == ValueType.None; }
        }

        public virtual BSONValue this[string key]
        {
            get { return null; }
            set { }
        }

        public virtual BSONValue this[int index]
        {
            get { return null; }
            set { }
        }

        public virtual void Clear()
        {
        }

        public virtual void Add(string key, BSONValue value)
        {
        }

        public virtual void Add(BSONValue value)
        {
        }

        public virtual bool Contains(BSONValue v)
        {
            return false;
        }

        public virtual bool ContainsKey(string key)
        {
            return false;
        }

        public static implicit operator BSONValue(double v)
        {
            return new BSONValue(v);
        }

        public static implicit operator BSONValue(decimal v)
        {
            return new BSONValue(v);
        }

        public static implicit operator BSONValue(int v)
        {
            return new BSONValue(v);
        }

        public static implicit operator BSONValue(long v)
        {
            return new BSONValue(v);
        }

        public static implicit operator BSONValue(byte[] v)
        {
            return new BSONValue(v);
        }

        public static implicit operator BSONValue(DateTime v)
        {
            return new BSONValue(v);
        }

        public static implicit operator BSONValue(string v)
        {
            return new BSONValue(v);
        }

        public static implicit operator double(BSONValue v)
        {
            return v.doubleValue;
        }

        public static implicit operator int(BSONValue v)
        {
            return v.int32Value;
        }

        public static implicit operator long(BSONValue v)
        {
            return v.int64Value;
        }

        public static implicit operator byte[](BSONValue v)
        {
            return v.binaryValue;
        }

        public static implicit operator DateTime(BSONValue v)
        {
            return v.dateTimeValue;
        }

        public static implicit operator string(BSONValue v)
        {
            return v.stringValue;
        }

        protected BSONValue(ValueType valueType)
        {
            this.mValueType = valueType;
        }

        public BSONValue()
        {
            this.mValueType = ValueType.None;
        }

        public BSONValue(double v)
        {
            this.mValueType = ValueType.Double;
            this.tdouble = v;
        }

        public BSONValue(decimal v)
        {
            this.mValueType = ValueType.Decimal;
            this.tdecimal = v;
        }

        public BSONValue(string v)
        {
            this.mValueType = ValueType.String;
            this.tstring = v;
        }

        public BSONValue(byte[] v)
        {
            this.mValueType = ValueType.Binary;
            this.binary = v;
        }

        public BSONValue(bool v)
        {
            this.mValueType = ValueType.Boolean;
            this.tbool = v;
        }

        public BSONValue(DateTime dt)
        {
            this.mValueType = ValueType.UTCDateTime;
            this.dateTime = dt;
        }

        public BSONValue(int v)
        {
            this.mValueType = ValueType.Int32;
            this.int32 = v;
        }

        public BSONValue(long v)
        {
            this.mValueType = ValueType.Int64;
            this.int64 = v;
        }
    }

    internal class BSONObject : BSONValue, IEnumerable
    {
        private Dictionary<string, BSONValue> mMap = new Dictionary<string, BSONValue>();

        public BSONObject()
            : base(BSONValue.ValueType.Object)
        {
        }

        // Properties
        public ICollection<string> Keys
        {
            get { return this.mMap.Keys; }
        }

        public ICollection<BSONValue> Values
        {
            get { return this.mMap.Values; }
        }

        public int Count
        {
            get { return this.mMap.Count; }
        }

        // Indexer
        public override BSONValue this[string key]
        {
            get { return this.mMap[key]; }
            set { this.mMap[key] = value; }
        }

        // Methods
        public override void Clear()
        {
            this.mMap.Clear();
        }

        public override void Add(string key, BSONValue value)
        {
            this.mMap.Add(key, value);
        }

        public override bool Contains(BSONValue v)
        {
            return this.mMap.ContainsValue(v);
        }

        public override bool ContainsKey(string key)
        {
            return this.mMap.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return this.mMap.Remove(key);
        }

        public bool TryGetValue(string key, out BSONValue value)
        {
            return this.mMap.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.mMap.GetEnumerator();
        }
    }

    internal class BSONArray : BSONValue, IEnumerable
    {
        private List<BSONValue> mList = new List<BSONValue>();

        public BSONArray()
            : base(BSONValue.ValueType.Array)
        {
        }

        // Indexer
        public override BSONValue this[int index]
        {
            get { return this.mList[index]; }
            set { this.mList[index] = value; }
        }

        public int Count
        {
            get { return this.mList.Count; }
        }

        // Methods
        public override void Add(BSONValue v)
        {
            this.mList.Add(v);
        }

        public int IndexOf(BSONValue item)
        {
            return this.mList.IndexOf(item);
        }

        public void Insert(int index, BSONValue item)
        {
            this.mList.Insert(index, item);
        }

        public bool Remove(BSONValue v)
        {
            return this.mList.Remove(v);
        }

        public void RemoveAt(int index)
        {
            this.mList.RemoveAt(index);
        }

        public override void Clear()
        {
            this.mList.Clear();
        }

        public override bool Contains(BSONValue v)
        {
            return this.mList.Contains(v);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.mList.GetEnumerator();
        }
    }

    internal class SimpleBSON
    {
        private MemoryStream mMemoryStream;
        private BinaryReader mBinaryReader;
        private BinaryWriter mBinaryWriter;

        public static BSONObject Load(byte[] buf)
        {
            SimpleBSON bson = new SimpleBSON(buf);

            return bson.decodeDocument();
        }

        public static byte[] Dump(BSONObject obj)
        {
            SimpleBSON bson = new SimpleBSON();
            MemoryStream ms = new MemoryStream();

            bson.encodeDocument(ms, obj);

            byte[] buf = new byte[ms.Position];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(buf, 0, buf.Length);

            return buf;
        }

        private SimpleBSON(byte[] buf = null)
        {
            if (buf != null)
            {
                this.mMemoryStream = new MemoryStream(buf);
                this.mBinaryReader = new BinaryReader(this.mMemoryStream);
            }
            else
            {
                this.mMemoryStream = new MemoryStream();
                this.mBinaryWriter = new BinaryWriter(this.mMemoryStream);
            }
        }

        private BSONValue decodeElement(out string name)
        {
            byte elementType = this.mBinaryReader.ReadByte();

            // Ref.: https://bsonspec.org/spec.html
            switch (elementType)
            {
                case 0x01:
                    // Double
                    name = this.decodeCString();
                    return new BSONValue(this.mBinaryReader.ReadDouble());

                case 0x02:
                    // String
                    name = this.decodeCString();
                    return new BSONValue(this.decodeString());

                case 0x03:
                    // Document
                    name = this.decodeCString();
                    return this.decodeDocument();

                case 0x04:
                    // Array
                    name = this.decodeCString();
                    return this.decodeArray();

                case 0x05:
                    // Binary
                    name = this.decodeCString();
                    int length = this.mBinaryReader.ReadInt32();
                    byte binaryType = this.mBinaryReader.ReadByte();
                    return new BSONValue(this.mBinaryReader.ReadBytes(length));

                case 0x06:
                    // Undefined value (deprecated)
                    name = this.decodeCString();
                    return new BSONValue();

                case 0x07:
                    // ObjectId (12 bytes)
                    name = this.decodeCString();
                    return new BSONValue(this.mBinaryReader.ReadBytes(12));

                case 0x08:
                    // Boolean
                    name = this.decodeCString();
                    return new BSONValue(this.mBinaryReader.ReadBoolean());

                case 0x09:
                    // DateTime
                    name = this.decodeCString();
                    long time = this.mBinaryReader.ReadInt64();
                    return new BSONValue(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(time * 10000));

                case 0x0A:
                    // Null value
                    name = this.decodeCString();
                    return new BSONValue();

                case 0x0B:
                    // Regular expression
                    name = this.decodeCString();
                    var regexPattern = this.decodeCString();
                    var regexOptions = this.decodeCString();
                    return new BSONValue(regexPattern);

                case 0x0C:
                    // DBPointer (Deprecated)
                    break;

                case 0x0D:
                    // JavaScript code
                    name = this.decodeCString();
                    return new BSONValue(this.decodeString());

                case 0x0E:
                    // Symbol (Deprecated)
                    name = this.decodeCString();
                    return new BSONValue(this.decodeString());

                case 0x0F:
                    // JavaScript code w/ scope (Deprecated)
                    name = this.decodeCString();
                    var value = this.mBinaryReader.ReadInt32();
                    var text = this.decodeString();
                    var document = this.decodeDocument();
                    return new BSONValue(text);

                case 0x10:
                    // Int32
                    name = this.decodeCString();
                    return new BSONValue(this.mBinaryReader.ReadInt32());

                case 0x11:
                    // Timestamp
                    name = this.decodeCString();
                    return new BSONValue((double)this.mBinaryReader.ReadUInt64());

                case 0x12:
                    // Int64
                    name = this.decodeCString();
                    return new BSONValue(this.mBinaryReader.ReadInt64());

                case 0x13:
                    // Decimal (128 bits)
                    name = this.decodeCString();
                    return new BSONValue(this.mBinaryReader.ReadDecimal());

                case 0xFF:
                    // Min key
                    name = this.decodeCString();
                    return new BSONValue();

                case 0x7F:
                    // Max key
                    name = this.decodeCString();
                    return new BSONValue();
            }

            throw new Exception(string.Format("Don't know elementType={0}", elementType));
        }

        private BSONObject decodeDocument()
        {
            int length = this.mBinaryReader.ReadInt32() - 4;

            BSONObject obj = new BSONObject();

            int i = (int)this.mBinaryReader.BaseStream.Position;
            while (this.mBinaryReader.BaseStream.Position < i + length - 1)
            {
                BSONValue value = this.decodeElement(out string name);
                obj.Add(name, value);
            }

            this.mBinaryReader.ReadByte(); // zero
            return obj;
        }

        private BSONArray decodeArray()
        {
            BSONObject obj = this.decodeDocument();

            int i = 0;
            BSONArray array = new BSONArray();
            while (obj.ContainsKey(Convert.ToString(i)))
            {
                array.Add(obj[Convert.ToString(i)]);

                i += 1;
            }

            return array;
        }

        private string decodeString()
        {
            int length = this.mBinaryReader.ReadInt32();
            byte[] buf = this.mBinaryReader.ReadBytes(length);

            return Encoding.UTF8.GetString(buf);
        }

        private string decodeCString()
        {
            var ms = new MemoryStream();
            while (true)
            {
                byte buf = (byte)this.mBinaryReader.ReadByte();
                if (buf == 0)
                    break;
                ms.WriteByte(buf);
            }
            byte[] buffer;
#if NETSTANDARD2_0
            if (!ms.TryGetBuffer(out var tempBuffer))
                throw new NAMEException("This should not happen!", NAMEStatusLevel.Warn);
            buffer = tempBuffer.Array;
#else
            try
            {
                buffer = ms.GetBuffer();
            }
            catch (Exception ex)
            {
                throw new NAMEException("This should not happen!", ex, NAMEStatusLevel.Error);
            }
#endif

            return Encoding.UTF8.GetString(buffer, 0, (int)ms.Position);
        }

        private void encodeElement(MemoryStream ms, string name, BSONValue v)
        {
            switch (v.valueType)
            {
                case BSONValue.ValueType.Double:
                    ms.WriteByte(0x01);
                    this.encodeCString(ms, name);
                    this.encodeDouble(ms, v.doubleValue);
                    return;

                case BSONValue.ValueType.String:
                    ms.WriteByte(0x02);
                    this.encodeCString(ms, name);
                    this.encodeString(ms, v.stringValue);
                    return;

                case BSONValue.ValueType.Object:
                    ms.WriteByte(0x03);
                    this.encodeCString(ms, name);
                    this.encodeDocument(ms, v as BSONObject);
                    return;

                case BSONValue.ValueType.Array:
                    ms.WriteByte(0x04);
                    this.encodeCString(ms, name);
                    this.encodeArray(ms, v as BSONArray);
                    return;

                case BSONValue.ValueType.Binary:
                    ms.WriteByte(0x05);
                    this.encodeCString(ms, name);
                    this.encodeBinary(ms, v.binaryValue);
                    return;

                case BSONValue.ValueType.Boolean:
                    ms.WriteByte(0x08);
                    this.encodeCString(ms, name);
                    this.encodeBool(ms, v.boolValue);
                    return;

                case BSONValue.ValueType.UTCDateTime:
                    ms.WriteByte(0x09);
                    this.encodeCString(ms, name);
                    this.encodeUTCDateTime(ms, v.dateTimeValue);
                    return;

                case BSONValue.ValueType.None:
                    ms.WriteByte(0x0A);
                    this.encodeCString(ms, name);
                    return;

                case BSONValue.ValueType.Int32:
                    ms.WriteByte(0x10);
                    this.encodeCString(ms, name);
                    this.encodeInt32(ms, v.int32Value);
                    return;

                case BSONValue.ValueType.Int64:
                    ms.WriteByte(0x12);
                    this.encodeCString(ms, name);
                    this.encodeInt64(ms, v.int64Value);
                    return;
            }
        }

        private void encodeDocument(MemoryStream ms, BSONObject obj)
        {
            MemoryStream dms = new MemoryStream();
            foreach (string str in obj.Keys)
            {
                this.encodeElement(dms, str, obj[str]);
            }

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((int)(dms.Position + 4 + 1));

            byte[] buffer;
#if NETSTANDARD2_0
            if (!dms.TryGetBuffer(out var tempBuffer))
                throw new NAMEException("This should not happen!", NAMEStatusLevel.Error);
            buffer = tempBuffer.Array;
#else
            try
            {
                buffer = dms.GetBuffer();
            }
            catch (Exception ex)
            {
                throw new NAMEException("This should not happen!", ex, NAMEStatusLevel.Error);
            }
#endif
            bw.Write(buffer, 0, (int)dms.Position);
            bw.Write((byte)0);
        }

        private void encodeArray(MemoryStream ms, BSONArray lst)
        {
            var obj = new BSONObject();
            for (int i = 0; i < lst.Count; ++i)
            {
                obj.Add(Convert.ToString(i), lst[i]);
            }

            this.encodeDocument(ms, obj);
        }

        private void encodeBinary(MemoryStream ms, byte[] buf)
        {
            byte[] aBuf = BitConverter.GetBytes(buf.Length);
            ms.Write(aBuf, 0, aBuf.Length);
            ms.WriteByte(0);
            ms.Write(buf, 0, buf.Length);
        }

        private void encodeCString(MemoryStream ms, string v)
        {
            byte[] buf = new UTF8Encoding().GetBytes(v);
            ms.Write(buf, 0, buf.Length);
            ms.WriteByte(0);
        }

        private void encodeString(MemoryStream ms, string v)
        {
            byte[] strBuf = new UTF8Encoding().GetBytes(v);
            byte[] buf = BitConverter.GetBytes(strBuf.Length + 1);

            ms.Write(buf, 0, buf.Length);
            ms.Write(strBuf, 0, strBuf.Length);
            ms.WriteByte(0);
        }

        private void encodeDouble(MemoryStream ms, double v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            ms.Write(buf, 0, buf.Length);
        }

        private void encodeBool(MemoryStream ms, bool v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            ms.Write(buf, 0, buf.Length);
        }

        private void encodeInt32(MemoryStream ms, int v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            ms.Write(buf, 0, buf.Length);
        }

        private void encodeInt64(MemoryStream ms, long v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            ms.Write(buf, 0, buf.Length);
        }

        private void encodeUTCDateTime(MemoryStream ms, DateTime dt)
        {
            TimeSpan span;
            if (dt.Kind == DateTimeKind.Local)
            {
                span = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            }
            else
            {
                span = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            }
            byte[] buf = BitConverter.GetBytes((long)(span.TotalSeconds * 1000));
            ms.Write(buf, 0, buf.Length);
        }
    }
}