// Copyright (c) 2007 Atif Aziz. All rights reserved.
//
// C# implementation of JsonPath[1]
// [1] http://goessner.net/articles/JsonPath/
//
// The MIT License
//
// Copyright (c) 2007 Atif Aziz . All rights reserved.
// Portions Copyright (c) 2007 Stefan Goessner (goessner.net)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace NAME.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    internal interface IJsonPathValueSystem
    {
        bool HasMember(object value, string member);
        object GetMemberValue(object value, string member);
        IEnumerable<string> GetMembers(object value);
        bool IsObject(object value);
        bool IsArray(object value);
        bool IsPrimitive(object value);
    }

    internal sealed class JsonPathContext
    {
        public static readonly JsonPathContext Default = new JsonPathContext();

        public Func<string /* script  */,
                    object /* value   */,
                    string /* context */,
                    object /* result  */>
            ScriptEvaluator
        { get; set; }

        public IJsonPathValueSystem ValueSystem { get; set; }

        public IEnumerable<object> Select(object obj, string expr)
        {
            return this.SelectNodes(obj, expr, (v, _) => v);
        }

        public IEnumerable<T> SelectNodes<T>(object obj, string expr, Func<object, string, T> resultor)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (resultor == null)
                throw new ArgumentNullException("resultor");

            var i = new Interpreter(this.ValueSystem, this.ScriptEvaluator);

            expr = Normalize(expr);

            if (expr.Length >= 1 && expr[0] == '$') // ^\$:?
                expr = expr.Substring(expr.Length >= 2 && expr[1] == ';' ? 2 : 1);

            return i.Trace(expr, obj, "$", (value, path) => resultor(value, AsBracketNotation(path)));
        }

        private static string Normalize(string expr)
        {
            var subx = new List<string>();
            expr = RegExp.Replace(expr, @"[\['](\??\(.*?\))[\]']", m =>
            {
                subx.Add(m.Groups[1].Value);
                return "[#" + subx.Count.ToString(CultureInfo.InvariantCulture) + "]";
            });
            expr = RegExp.Replace(expr, @"'?\.'?|\['?", ";");
            expr = RegExp.Replace(expr, @";;;|;;", ";..;");
            expr = RegExp.Replace(expr, @";$|'?\]|'$", string.Empty);
            expr = RegExp.Replace(expr, @"#([0-9]+)", m =>
            {
                var index = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                return subx[index];
            });
            return expr;
        }

        public static string AsBracketNotation(string[] indicies)
        {
            if (indicies == null)
                throw new ArgumentNullException("indicies");

            var sb = new StringBuilder();

            foreach (var index in indicies)
            {
                if (sb.Length == 0)
                {
                    sb.Append('$');
                }
                else
                {
                    sb.Append('[');
                    if (RegExp.IsMatch(index, @"^[0-9*]+$"))
                        sb.Append(index);
                    else
                        sb.Append('\'').Append(index).Append('\'');
                    sb.Append(']');
                }
            }

            return sb.ToString();
        }

        private static int ParseInt(string str, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            try
            {
                return int.Parse(str, NumberStyles.None, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
        }

        private sealed class Interpreter
        {
            private readonly Func<string, object, string, object> eval;
            private readonly IJsonPathValueSystem system;

            private static readonly IJsonPathValueSystem DefaultValueSystem = new BasicValueSystem();

            private static readonly char[] Colon = { ':' };
            private static readonly char[] Semicolon = { ';' };

            private delegate void WalkCallback(object member, string loc, string expr, object value, string path);

            public Interpreter(IJsonPathValueSystem valueSystem, Func<string, object, string, object> eval)
            {
                if (eval == null)
                    this.eval = (a, b, c) => null;
                else
                    this.eval = eval;
                this.system = valueSystem ?? DefaultValueSystem;
            }

            private sealed class TraceArgs
            {
                public string Expr { get; }
                public object Value { get; }
                public string Path { get; }

                public TraceArgs(string expr, object value, string path)
                {
                    this.Expr = expr;
                    this.Value = value;
                    this.Path = path;
                }
            }

            public IEnumerable<T> Trace<T>(string expr, object value, string path, Func<object, string[], T> resultor)
            {
                return this.Trace(Args(expr, value, path), resultor);
            }

            private static TraceArgs Args(string expr, object value, string path)
            {
                return new TraceArgs(expr, value, path);
            }

            private IEnumerable<T> Trace<T>(TraceArgs args, Func<object, string[], T> resultor)
            {
                var stack = new Stack<TraceArgs>();
                stack.Push(args);

                while (stack.Count > 0)
                {
                    var popped = stack.Pop();
                    var expr = popped.Expr;
                    var value = popped.Value;
                    var path = popped.Path;

                    if (string.IsNullOrEmpty(expr))
                    {
                        if (path != null)
                            yield return resultor(value, path.Split(Semicolon));
                        continue;
                    }

                    var i = expr.IndexOf(';');
                    var atom = i >= 0 ? expr.Substring(0, i) : expr;
                    var tail = i >= 0 ? expr.Substring(i + 1) : string.Empty;

                    if (value != null && this.system.HasMember(value, atom))
                    {
                        stack.Push(Args(tail, this.Index(value, atom), path + ";" + atom));
                    }
                    else if (atom == "*")
                    {
                        this.Walk(atom, tail, value, path, (m, l, x, v, p) => stack.Push(Args(m + ";" + x, v, p)));
                    }
                    else if (atom == "..")
                    {
                        this.Walk(atom, tail, value, path, (m, l, x, v, p) =>
                        {
                            var result = this.Index(v, m.ToString());
                            if (result != null && !this.system.IsPrimitive(result))
                                stack.Push(Args("..;" + x, result, p + ";" + m));
                        });
                        stack.Push(Args(tail, value, path));
                    }
                    else if (atom.Length > 2 && atom[0] == '(' && atom[atom.Length - 1] == ')')
                    {
                        stack.Push(Args(this.eval(atom, value, path.Substring(path.LastIndexOf(';') + 1)) + ";" + tail, value, path));
                    }
                    else if (atom.Length > 3 && atom[0] == '?' && atom[1] == '(' && atom[atom.Length - 1] == ')')
                    {
                        this.Walk(
                            atom,
                            tail,
                            value,
                            path,
                            (m, l, x, v, p) =>
                            {
                                var result = this.eval(
                                    RegExp.Replace(l, @"^\?\((.*?)\)$", "$1"),
                                    this.Index(v, m.ToString()),
                                    m.ToString());

                                if (Convert.ToBoolean(result, CultureInfo.InvariantCulture))
                                    stack.Push(Args(m + ";" + x, v, p));
                            });
                    }
                    else if (RegExp.IsMatch(atom, @"^(-?[0-9]*):(-?[0-9]*):?([0-9]*)$"))
                    {
                        foreach (var a in Slice(atom, tail, value, path).Reverse())
                            stack.Push(a);
                    }
                    else if (atom.IndexOf(',') >= 0)
                    {
                        foreach (var part in RegExp.Split(atom, @"'?,'?").Reverse())
                            stack.Push(Args(part + ";" + tail, value, path));
                    }
                }
            }

            private void Walk(string loc, string expr, object value, string path, WalkCallback callback)
            {
                if (this.system.IsPrimitive(value))
                    return;

                if (this.system.IsArray(value))
                {
                    var list = (IList)value;
                    for (var i = list.Count - 1; i >= 0; i--)
                        callback(i, loc, expr, value, path);
                }
                else if (this.system.IsObject(value))
                {
                    foreach (var key in this.system.GetMembers(value).Reverse())
                        callback(key, loc, expr, value, path);
                }
            }

            private static IEnumerable<TraceArgs> Slice(string loc, string expr, object value, string path)
            {
                var list = value as IList;

                if (list == null)
                    yield break;

                var length = list.Count;
                var parts = loc.Split(Colon);
                var start = ParseInt(parts[0]);
                var end = ParseInt(parts[1], list.Count);
                var step = parts.Length > 2 ? ParseInt(parts[2], 1) : 1;
                start = (start < 0) ? Math.Max(0, start + length) : Math.Min(length, start);
                end = (end < 0) ? Math.Max(0, end + length) : Math.Min(length, end);
                for (var i = start; i < end; i += step)
                    yield return Args(i + ";" + expr, value, path);
            }

            private object Index(object obj, string member)
            {
                return this.system.GetMemberValue(obj, member);
            }
        }

        private static class RegExp
        {
            private const RegexOptions Options = RegexOptions.ECMAScript;

            public static bool IsMatch(string input, string pattern)
            {
                return Regex.IsMatch(input, pattern, Options);
            }

            public static string Replace(string input, string pattern, string replacement)
            {
                return Regex.Replace(input, pattern, replacement, Options);
            }

            public static string Replace(string input, string pattern, MatchEvaluator evaluator)
            {
                return Regex.Replace(input, pattern, evaluator, Options);
            }

            public static IEnumerable<string> Split(string input, string pattern)
            {
                return Regex.Split(input, pattern, Options);
            }
        }

        internal sealed class BasicValueSystem : IJsonPathValueSystem
        {
            public bool HasMember(object value, string member)
            {
                if (this.IsPrimitive(value))
                    return false;

                var dict = value as IDictionary;
                if (dict != null)
                    return dict.Contains(member);

                var list = value as IList;
                if (list != null)
                {
                    var index = ParseInt(member, -1);
                    return index >= 0 && index < list.Count;
                }

                return false;
            }

            public object GetMemberValue(object value, string member)
            {
                if (this.IsPrimitive(value))
                    throw new ArgumentException("value");

                var dict = value as IDictionary;
                if (dict != null)
                    return dict[member];

                var list = (IList)value;
                var index = ParseInt(member, -1);
                if (index >= 0 && index < list.Count)
                    return list[index];

                return null;
            }

            public IEnumerable<string> GetMembers(object value)
            {
                return ((IDictionary)value).Keys.Cast<string>();
            }

            public bool IsObject(object value)
            {
                return value is IDictionary;
            }

            public bool IsArray(object value)
            {
                return value is IList;
            }

            public bool IsPrimitive(object value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                return Type.GetTypeCode(value.GetType()) != TypeCode.Object;
            }
        }
    }
}