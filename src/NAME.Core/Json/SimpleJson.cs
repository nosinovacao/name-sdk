/* * * * *
 * A simple Json Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple Json parser. It can build a Json string
 * from the node-tree, or generate a node tree from any valid Json string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Modified by oPless, 2014-09-21 to round-trip properly
 *
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JsonArray), objects(JsonClass) and values(JsonData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed Json is parsed the result is undefined
 * 
 * 
 * 2012-12-17 Update:
 * - Added internal JsonLazyCreator class which simplifies the construction of a Json tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JsonLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * * * * */

// Originally taken from https://github.com/opless/SimpleJSON
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace NAME.Json
{
    internal enum JsonBinaryTag
    {
        Array = 1,
        Class = 2,
        Value = 3,
        IntValue = 4,
        DoubleValue = 5,
        BoolValue = 6,
        FloatValue = 7,
    }

    internal abstract class JsonNode
    {
        public virtual void Add(string aKey, JsonNode aItem)
        {
        }
        
        public virtual JsonNode this[int aIndex]
        {
            get { return null; }
            set { }
        }

        public virtual JsonNode this[string aKey]
        {
            get { return null; }
            set { }
        }

        public virtual string Value
        {
            get { return string.Empty; }
            set { }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual void Add(JsonNode aItem)
        {
            this.Add(string.Empty, aItem);
        }

        public virtual JsonNode Remove(string aKey)
        {
            return null;
        }

        public virtual JsonNode Remove(int aIndex)
        {
            return null;
        }

        public virtual JsonNode Remove(JsonNode aNode)
        {
            return aNode;
        }

        public virtual IEnumerable<JsonNode> Children
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<JsonNode> DeepChildren
        {
            get
            {
                foreach (var c in this.Children)
                {
                    foreach (var d in c.DeepChildren)
                        yield return d;
                }
            }
        }

        public override string ToString()
        {
            return "JsonNode";
        }

        public virtual string ToString(string aPrefix)
        {
            return "JsonNode";
        }

        public abstract string ToJson(int prefix);

        public virtual JsonBinaryTag Tag { get; set; }

        public virtual int AsInt
        {
            get
            {
                int v = 0;
                if (int.TryParse(this.Value, out v))
                    return v;
                return 0;
            }
            set
            {
                this.Value = value.ToString();
                this.Tag = JsonBinaryTag.IntValue;
            }
        }

        public virtual long AsLong
        {
            get
            {
                long v = 0;
                if (long.TryParse(this.Value, out v))
                    return v;
                return 0;
            }
            set
            {
                this.Value = value.ToString();
                this.Tag = JsonBinaryTag.IntValue;
            }
        }

        public virtual float AsFloat
        {
            get
            {
                float v = 0.0f;
                if (float.TryParse(this.Value, out v))
                    return v;
                return 0.0f;
            }
            set
            {
                this.Value = value.ToString();
                this.Tag = JsonBinaryTag.FloatValue;
            }
        }

        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(this.Value, out v))
                    return v;
                return 0.0;
            }
            set
            {
                this.Value = value.ToString();
                this.Tag = JsonBinaryTag.DoubleValue;

            }
        }

        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(this.Value, out v))
                    return v;
                return !string.IsNullOrEmpty(this.Value);
            }
            set
            {
                this.Value = value ? "true" : "false";
                this.Tag = JsonBinaryTag.BoolValue;

            }
        }

        public virtual JsonArray AsArray
        {
            get
            {
                return this as JsonArray;
            }
        }

        public virtual JsonClass AsObject
        {
            get
            {
                return this as JsonClass;
            }
        }


        public static implicit operator JsonNode(string s)
        {
            return new JsonData(s);
        }

        public static implicit operator string(JsonNode d)
        {
            return (d == null) ? null : d.Value;
        }

        public static bool operator ==(JsonNode a, object b)
        {
            if (b == null && a is JsonLazyCreator)
                return true;
            return object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JsonNode a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal static string Escape(string aText)
        {
            string result = string.Empty;
            if (aText != null)
            {
                foreach (char c in aText)
                {
                    switch (c)
                    {
                        case '\\':
                            result += "\\\\";
                            break;
                        case '\"':
                            result += "\\\"";
                            break;
                        case '\n':
                            result += "\\n";
                            break;
                        case '\r':
                            result += "\\r";
                            break;
                        case '\t':
                            result += "\\t";
                            break;
                        case '\b':
                            result += "\\b";
                            break;
                        case '\f':
                            result += "\\f";
                            break;
                        default:
                            result += c;
                            break;
                    }
                }
            }
            return result;
        }

        private static JsonData Numberize(string token)
        {
            bool flag = false;
            int integer = 0;
            double real = 0;

            if (int.TryParse(token, out integer))
            {
                return new JsonData(integer);
            }

            if (double.TryParse(token, out real))
            {
                return new JsonData(real);
            }

            if (bool.TryParse(token, out flag))
            {
                return new JsonData(flag);
            }

            throw new NotImplementedException(token);
        }

        private static void AddElement(JsonNode ctx, string token, string tokenName, bool tokenIsString)
        {
            if (tokenIsString)
            {
                if (ctx is JsonArray)
                    ctx.Add(token);
                else
                    ctx.Add(tokenName, token); // assume dictionary/object
            }
            else
            {
                JsonData number = Numberize(token);
                if (ctx is JsonArray)
                    ctx.Add(number);
                else
                    ctx.Add(tokenName, number);

            }
        }

        public static JsonNode Parse(string aJson)
        {
            Stack<JsonNode> stack = new Stack<JsonNode>();
            JsonNode ctx = null;
            int i = 0;
            string token = string.Empty;
            string tokenName = string.Empty;
            bool quoteMode = false;
            bool tokenIsString = false;
            while (i < aJson.Length)
            {
                switch (aJson[i])
                {
                    case '{':
                        if (quoteMode)
                        {
                            token += aJson[i];
                            break;
                        }
                        stack.Push(new JsonClass());
                        if (ctx != null)
                        {
                            tokenName = tokenName.Trim();
                            if (ctx is JsonArray)
                                ctx.Add(stack.Peek());
                            else if (tokenName != string.Empty)
                                ctx.Add(tokenName, stack.Peek());
                        }
                        tokenName = string.Empty;
                        token = string.Empty;
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (quoteMode)
                        {
                            token += aJson[i];
                            break;
                        }

                        stack.Push(new JsonArray());
                        if (ctx != null)
                        {
                            tokenName = tokenName.Trim();

                            if (ctx is JsonArray)
                                ctx.Add(stack.Peek());
                            else if (tokenName != string.Empty)
                                ctx.Add(tokenName, stack.Peek());
                        }
                        tokenName = string.Empty;
                        token = string.Empty;
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (quoteMode)
                        {
                            token += aJson[i];
                            break;
                        }
                        if (stack.Count == 0)
                            throw new Exception("Json Parse: Too many closing brackets");

                        stack.Pop();
                        if (token != string.Empty)
                        {
                            tokenName = tokenName.Trim();
                            /*
                            if (ctx is JsonArray)
                                ctx.Add (Token);
                            else if (TokenName != string.Empty)
                                ctx.Add (TokenName, Token);
                                */
                            AddElement(ctx, token, tokenName, tokenIsString);
                            tokenIsString = false;
                        }
                        tokenName = string.Empty;
                        token = string.Empty;
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (quoteMode)
                        {
                            token += aJson[i];
                            break;
                        }
                        tokenName = token;
                        token = string.Empty;
                        tokenIsString = false;
                        break;

                    case '"':
                        quoteMode ^= true;
                        tokenIsString = quoteMode == true ? true : tokenIsString;
                        break;

                    case ',':
                        if (quoteMode)
                        {
                            token += aJson[i];
                            break;
                        }
                        if (token != string.Empty)
                        {
                            /*
                            if (ctx is JsonArray) {
                                ctx.Add (Token);
                            } else if (TokenName != string.Empty) {
                                ctx.Add (TokenName, Token);
                            }
                            */
                            AddElement(ctx, token, tokenName, tokenIsString);
                            tokenIsString = false;

                        }
                        tokenName = string.Empty;
                        token = string.Empty;
                        tokenIsString = false;
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (quoteMode)
                            token += aJson[i];
                        break;

                    case '\\':
                        ++i;
                        if (quoteMode)
                        {
                            char c = aJson[i];
                            switch (c)
                            {
                                case 't':
                                    token += '\t';
                                    break;
                                case 'r':
                                    token += '\r';
                                    break;
                                case 'n':
                                    token += '\n';
                                    break;
                                case 'b':
                                    token += '\b';
                                    break;
                                case 'f':
                                    token += '\f';
                                    break;
                                case 'u':
                                    {
                                        string s = aJson.Substring(i + 1, 4);
                                        token += (char)int.Parse(
                                            s,
                                            System.Globalization.NumberStyles.AllowHexSpecifier);
                                        i += 4;
                                        break;
                                    }
                                default:
                                    token += c;
                                    break;
                            }
                        }
                        break;

                    default:
                        token += aJson[i];
                        break;
                }
                ++i;
            }
            if (quoteMode)
            {
                throw new Exception("Json Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }

        public virtual void Serialize(System.IO.BinaryWriter aWriter)
        {
        }

        public void SaveToStream(System.IO.Stream aData)
        {
            var w = new System.IO.BinaryWriter(aData);
            this.Serialize(w);
        }

#if USE_SharpZipLib
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
            {
                gzipOut.IsStreamOwner = false;
                SaveToStream(gzipOut);
                gzipOut.Close();
            }
        }
 
        public void SaveToCompressedFile(string aFileName)
        {
 
#if USE_FileIO
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using(var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToCompressedStream(F);
            }
 
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }
        public string SaveToCompressedBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToCompressedStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }
 
#else
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJson");
        }

        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJson");
        }

        public string SaveToCompressedBase64()
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJson");
        }
#endif

        public void SaveToFile(string aFileName)
        {
#if USE_FileIO
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using (var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }

        public string SaveToBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                this.SaveToStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

        public static JsonNode Deserialize(System.IO.BinaryReader aReader)
        {
            JsonBinaryTag type = (JsonBinaryTag)aReader.ReadByte();
            switch (type)
            {
                case JsonBinaryTag.Array:
                    {
                        int count = aReader.ReadInt32();
                        JsonArray tmp = new JsonArray();
                        for (int i = 0; i < count; i++)
                            tmp.Add(Deserialize(aReader));
                        return tmp;
                    }
                case JsonBinaryTag.Class:
                    {
                        int count = aReader.ReadInt32();
                        JsonClass tmp = new JsonClass();
                        for (int i = 0; i < count; i++)
                        {
                            string key = aReader.ReadString();
                            var val = Deserialize(aReader);
                            tmp.Add(key, val);
                        }
                        return tmp;
                    }
                case JsonBinaryTag.Value:
                    {
                        return new JsonData(aReader.ReadString());
                    }
                case JsonBinaryTag.IntValue:
                    {
                        return new JsonData(aReader.ReadInt32());
                    }
                case JsonBinaryTag.DoubleValue:
                    {
                        return new JsonData(aReader.ReadDouble());
                    }
                case JsonBinaryTag.BoolValue:
                    {
                        return new JsonData(aReader.ReadBoolean());
                    }
                case JsonBinaryTag.FloatValue:
                    {
                        return new JsonData(aReader.ReadSingle());
                    }

                default:
                    {
                        throw new Exception("Error deserializing Json. Unknown tag: " + type);
                    }
            }
        }

#if USE_SharpZipLib
        public static JsonNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
            return LoadFromStream(zin);
        }
        public static JsonNode LoadFromCompressedFile(string aFileName)
        {
#if USE_FileIO
            using(var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromCompressedStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }
        public static JsonNode LoadFromCompressedBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromCompressedStream(stream);
        }
#else
        public static JsonNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJson");
        }

        public static JsonNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJson");
        }

        public static JsonNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJson");
        }
#endif

        public static JsonNode LoadFromStream(System.IO.Stream aData)
        {
            using (var r = new System.IO.BinaryReader(aData))
            {
                return Deserialize(r);
            }
        }

        public static JsonNode LoadFromFile(string aFileName)
        {
#if USE_FileIO
            using (var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }

        public static JsonNode LoadFromBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromStream(stream);
        }
    }
    // End of JsonNode

    internal class JsonArray : JsonNode, IEnumerable, IList
    {
        private List<JsonNode> list = new List<JsonNode>();

        public override JsonNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= this.list.Count)
                    return null;
                return this.list[aIndex];
            }
            set
            {
                if (aIndex < 0 || aIndex >= this.list.Count)
                    this.list.Add(value);
                else
                    this.list[aIndex] = value;
            }
        }

        public override JsonNode this[string aKey]
        {
            get { return new JsonLazyCreator(this); }
            set { this.list.Add(value); }
        }

        public override int Count
        {
            get { return this.list.Count; }
        }

        public override void Add(string aKey, JsonNode aItem)
        {
            this.list.Add(aItem);
        }

        public override JsonNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= this.list.Count)
                return null;
            JsonNode tmp = this.list[aIndex];
            this.list.RemoveAt(aIndex);
            return tmp;
        }

        public override JsonNode Remove(JsonNode aNode)
        {
            this.list.Remove(aNode);
            return aNode;
        }

        public override IEnumerable<JsonNode> Children
        {
            get
            {
                foreach (JsonNode n in this.list)
                    yield return n;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (JsonNode n in this.list)
                yield return n;
        }

        public override string ToString()
        {
            string result = "[ ";
            foreach (JsonNode n in this.list)
            {
                if (result.Length > 2)
                    result += ", ";
                result += n.ToString();
            }
            result += " ]";
            return result;
        }

        public override string ToString(string aPrefix)
        {
            string result = "[ ";
            foreach (JsonNode n in this.list)
            {
                if (result.Length > 3)
                    result += ", ";
                result += "\n" + aPrefix + "   ";
                result += n.ToString(aPrefix + "   ");
            }
            result += "\n" + aPrefix + "]";
            return result;
        }

        public override string ToJson(int prefix)
        {
            string s = new string(' ', (prefix + 1) * 2);
            string ret = "[ ";
            foreach (JsonNode n in this.list)
            {
                if (ret.Length > 3)
                    ret += ", ";
                ret += "\n" + s;
                ret += n.ToJson(prefix + 1);

            }
            ret += "\n" + s + "]";
            return ret;
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonBinaryTag.Array);
            aWriter.Write(this.list.Count);
            for (int i = 0; i < this.list.Count; i++)
            {
                this.list[i].Serialize(aWriter);
            }
        }

        public int Add(object value)
        {
            if (!(value is JsonNode))
                return -1;
            this.list.Add(value as JsonNode);
            return this.list.Count - 1;
        }

        public bool Contains(object value)
        {
            return this.list.Any(o => o == value);
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public int IndexOf(object value)
        {
            if (!(value is JsonNode))
                return -1;
            return this.list.IndexOf(value as JsonNode);
        }

        public void Insert(int index, object value)
        {
            this.list.Insert(index, (JsonNode)value);
        }

        public void Remove(object value)
        {
            this.list.Remove((JsonNode)value);
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            JsonNode[] nodes = new JsonNode[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                nodes[i] = (JsonNode)array.GetValue(i);
            }
            this.list.CopyTo(nodes, index);
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;
        public object SyncRoot
        {
            get
            {
                return this.list;
            }
        }

        public bool IsSynchronized => false;

        object IList.this[int index]
        {
            get
            {
                return this.list[index];
            }

            set
            {
                this.list[index] = (JsonNode)value;
            }
        }
    }
    // End of JsonArray

    internal class JsonClass : JsonNode, IEnumerable
    {
        private Dictionary<string, JsonNode> dict = new Dictionary<string, JsonNode>();

        public override JsonNode this[string aKey]
        {
            get
            {
                if (this.dict.ContainsKey(aKey))
                    return this.dict[aKey];
                else
                    return null;
            }
            set
            {
                if (this.dict.ContainsKey(aKey))
                    this.dict[aKey] = value;
                else
                    this.dict.Add(aKey, value);
            }
        }

        public override JsonNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= this.dict.Count)
                    return null;
                return this.dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= this.dict.Count)
                    return;
                string key = this.dict.ElementAt(aIndex).Key;
                this.dict[key] = value;
            }
        }

        public override int Count
        {
            get { return this.dict.Count; }
        }


        public override void Add(string aKey, JsonNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (this.dict.ContainsKey(aKey))
                    this.dict[aKey] = aItem;
                else
                    this.dict.Add(aKey, aItem);
            }
            else
            {
                this.dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public override JsonNode Remove(string aKey)
        {
            if (!this.dict.ContainsKey(aKey))
                return null;
            JsonNode tmp = this.dict[aKey];
            this.dict.Remove(aKey);
            return tmp;
        }

        public override JsonNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= this.dict.Count)
                return null;
            var item = this.dict.ElementAt(aIndex);
            this.dict.Remove(item.Key);
            return item.Value;
        }

        public override JsonNode Remove(JsonNode aNode)
        {
            try
            {
                var item = this.dict.Where(k => k.Value == aNode).First();
                this.dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override IEnumerable<JsonNode> Children
        {
            get
            {
                foreach (KeyValuePair<string, JsonNode> n in this.dict)
                    yield return n.Value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JsonNode> n in this.dict)
                yield return n;
        }

        public override string ToString()
        {
            string result = "{";
            foreach (KeyValuePair<string, JsonNode> n in this.dict)
            {
                if (result.Length > 2)
                    result += ", ";
                result += "\"" + Escape(n.Key) + "\":" + (n.Value?.ToString() ?? "\"\"");
            }
            result += "}";
            return result;
        }

        public override string ToString(string aPrefix)
        {
            string result = "{ ";
            foreach (KeyValuePair<string, JsonNode> n in this.dict)
            {
                if (result.Length > 3)
                    result += ", ";
                result += "\n" + aPrefix + "   ";
                result += "\"" + Escape(n.Key) + "\" : " + n.Value.ToString(aPrefix + "   ");
            }
            result += "\n" + aPrefix + "}";
            return result;
        }

        public override string ToJson(int prefix)
        {
            string s = new string(' ', (prefix + 1) * 2);
            string ret = "{ ";
            foreach (KeyValuePair<string, JsonNode> n in this.dict)
            {
                if (ret.Length > 3)
                    ret += ", ";
                ret += "\n" + s;
                ret += string.Format("\"{0}\": {1}", n.Key, n.Value.ToJson(prefix + 1));
            }
            ret += "\n" + s + "}";
            return ret;
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonBinaryTag.Class);
            aWriter.Write(this.dict.Count);
            foreach (string k in this.dict.Keys)
            {
                aWriter.Write(k);
                this.dict[k].Serialize(aWriter);
            }
        }
    }
    // End of JsonClass

    internal class JsonData : JsonNode
    {
        private string data;


        public override string Value
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
                this.Tag = JsonBinaryTag.Value;
            }
        }

        public JsonData(string aData)
        {
            this.data = aData;
            this.Tag = JsonBinaryTag.Value;
        }

        public JsonData(float aData)
        {
            this.AsFloat = aData;
        }

        public JsonData(double aData)
        {
            this.AsDouble = aData;
        }

        public JsonData(bool aData)
        {
            this.AsBool = aData;
        }

        public JsonData(int aData)
        {
            this.AsInt = aData;
        }

        public override string ToString()
        {
            return "\"" + Escape(this.data) + "\"";
        }

        public override string ToString(string aPrefix)
        {
            return "\"" + Escape(this.data) + "\"";
        }

        public override string ToJson(int prefix)
        {
            switch (this.Tag)
            {
                case JsonBinaryTag.DoubleValue:
                case JsonBinaryTag.FloatValue:
                case JsonBinaryTag.IntValue:
                    return this.data;
                case JsonBinaryTag.Value:
                    return string.Format("\"{0}\"", Escape(this.data));
                default:
                    throw new NotSupportedException("This shouldn't be here: " + this.Tag.ToString());
            }
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            var tmp = new JsonData(string.Empty);

            tmp.AsInt = this.AsInt;
            if (tmp.data == this.data)
            {
                aWriter.Write((byte)JsonBinaryTag.IntValue);
                aWriter.Write(this.AsInt);
                return;
            }
            tmp.AsFloat = this.AsFloat;
            if (tmp.data == this.data)
            {
                aWriter.Write((byte)JsonBinaryTag.FloatValue);
                aWriter.Write(this.AsFloat);
                return;
            }
            tmp.AsDouble = this.AsDouble;
            if (tmp.data == this.data)
            {
                aWriter.Write((byte)JsonBinaryTag.DoubleValue);
                aWriter.Write(this.AsDouble);
                return;
            }

            tmp.AsBool = this.AsBool;
            if (tmp.data == this.data)
            {
                aWriter.Write((byte)JsonBinaryTag.BoolValue);
                aWriter.Write(this.AsBool);
                return;
            }
            aWriter.Write((byte)JsonBinaryTag.Value);
            aWriter.Write(this.data);
        }
    }
    // End of JsonData

    internal class JsonLazyCreator : JsonNode
    {
        private JsonNode node = null;
        private string key = null;

        public JsonLazyCreator(JsonNode aNode)
        {
            this.node = aNode;
            this.key = null;
        }

        public JsonLazyCreator(JsonNode aNode, string aKey)
        {
            this.node = aNode;
            this.key = aKey;
        }

        private void Set(JsonNode aVal)
        {
            if (this.key == null)
            {
                this.node.Add(aVal);
            }
            else
            {
                this.node.Add(this.key, aVal);
            }
            this.node = null; // Be GC friendly.
        }

        public override JsonNode this[int aIndex]
        {
            get
            {
                return new JsonLazyCreator(this);
            }
            set
            {
                var tmp = new JsonArray();
                tmp.Add(value);
                this.Set(tmp);
            }
        }

        public override JsonNode this[string aKey]
        {
            get
            {
                return new JsonLazyCreator(this, aKey);
            }
            set
            {
                var tmp = new JsonClass();
                tmp.Add(aKey, value);
                this.Set(tmp);
            }
        }

        public override void Add(JsonNode aItem)
        {
            var tmp = new JsonArray();
            tmp.Add(aItem);
            this.Set(tmp);
        }

        public override void Add(string aKey, JsonNode aItem)
        {
            var tmp = new JsonClass();
            tmp.Add(aKey, aItem);
            this.Set(tmp);
        }

        public static bool operator ==(JsonLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JsonLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public override string ToString(string aPrefix)
        {
            return string.Empty;
        }

        public override string ToJson(int prefix)
        {
            return string.Empty;
        }

        public override int AsInt
        {
            get
            {
                JsonData tmp = new JsonData(0);
                this.Set(tmp);
                return 0;
            }
            set
            {
                JsonData tmp = new JsonData(value);
                this.Set(tmp);
            }
        }

        public override float AsFloat
        {
            get
            {
                JsonData tmp = new JsonData(0.0f);
                this.Set(tmp);
                return 0.0f;
            }
            set
            {
                JsonData tmp = new JsonData(value);
                this.Set(tmp);
            }
        }

        public override double AsDouble
        {
            get
            {
                JsonData tmp = new JsonData(0.0);
                this.Set(tmp);
                return 0.0;
            }
            set
            {
                JsonData tmp = new JsonData(value);
                this.Set(tmp);
            }
        }

        public override bool AsBool
        {
            get
            {
                JsonData tmp = new JsonData(false);
                this.Set(tmp);
                return false;
            }
            set
            {
                JsonData tmp = new JsonData(value);
                this.Set(tmp);
            }
        }

        public override JsonArray AsArray
        {
            get
            {
                JsonArray tmp = new JsonArray();
                this.Set(tmp);
                return tmp;
            }
        }

        public override JsonClass AsObject
        {
            get
            {
                JsonClass tmp = new JsonClass();
                this.Set(tmp);
                return tmp;
            }
        }
    }
    // End of JsonLazyCreator

    internal static class Json
    {
        public static JsonNode Parse(string aJson)
        {
            return JsonNode.Parse(aJson);
        }
    }
}