using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace /***$rootnamespace$.***/ULibs.TinyJsonDeser
{
    /// <summary>
    /// Simple json deserializer class that's capable of parsing any of the following from a json input string,
    /// as described at http://json.org.
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       An arbitrary <see cref="TryParseValue(string,out object)">value</see>, as an <c>object</c>.
    ///       The object can be any of the subsequent more specific types.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       An <see cref="TryParseObject(string,out IDictionary{string,object})">object</see>, as an <c>IDictionary&lt;string, object&gt;</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       An <see cref="TryParseArray(string,out object[])">array</see>, as an <c>object[]</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       A <see cref="TryParseString(string,out string)">string</see>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       A <see cref="TryParseNumber(string,out double)">number</see>, as a <c>double</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       A <see cref="TryParseBool(string,out bool)">boolean literal</see>, as a <c>bool</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       The <see cref="TryParseNull(string)">null literal</see>, as a <c>null</c> reference.
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    /***[ExcludeFromCodeCoverage]***/
    internal sealed class JsonDeserializer
    {
        /// <summary>
        /// Tries to parse a json string to an object.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <param name="output">If the parse succeeded, this will hold the parsed result, which will be either a
        /// <c>bool</c>, a <c>string</c>, a <c>double</c>, an <c>object[]</c>, an <c>IDictionary&lt;string, object&gt;</c>
        /// or <c>null</c>.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseValue(string json, out object? output)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseValue(json, ref offset, out output)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        /// <summary>
        /// Tries to parse a json string to boolean literal.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <param name="output">If the parse succeeded, this will hold the parsed result.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseBool(string json, out bool output)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseBool(json, ref offset, out output)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        /// <summary>
        /// Tries to parse a json string as the null literal.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseNull(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseNull(json, ref offset)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        /// <summary>
        /// Tries to parse a json string as a number.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <param name="output">If the parse succeeded, this will hold the parsed result.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseNumber(string json, out double output)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseNumber(json, ref offset, out output)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        /// <summary>
        /// Tries to parse an escaped json string.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <param name="output">If the parse succeeded, this will hold the parsed result.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseString(string json, [NotNullWhen(true)]out string? output)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseString(json, ref offset, out output)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        /// <summary>
        /// Tries to parse a json array.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <param name="output">If the parse succeeded, this will hold the parsed result.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseArray(string json, out object?[]? output)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseArray(json, ref offset, out output)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        /// <summary>
        /// Tries to parse a json object.
        /// </summary>
        /// <param name="json">The json string to parse.</param>
        /// <param name="output">If the parse succeeded, this will hold the parsed result.</param>
        /// <returns>Whether or not the parse attempt succeeded.</returns>
        public bool TryParseObject(string json, out IDictionary<string, object?>? output)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var offset = 0;
            SkipWhiteSpace(json, ref offset);
            if (!TryParseObject(json, ref offset, out output)) return false;
            SkipWhiteSpace(json, ref offset);
            return offset == json.Length;
        }

        private static void SkipWhiteSpace(string json, ref int offset)
        {
            while (offset < json.Length && char.IsWhiteSpace(json[offset])) offset++;
        }

        private static bool TryParseValue(string json, ref int offset, out object? output)
        {
            if (offset < json.Length)
            {
                switch (json[offset])
                {
                    case 't':
                    case 'f':
                        if (TryParseBool(json, ref offset, out var outputBool))
                        {
                            output = outputBool;
                            return true;
                        }

                        break;

                    case 'n':
                        output = null;
                        return TryParseNull(json, ref offset);

                    case '"':
                        if (TryParseString(json, ref offset, out var outputString))
                        {
                            output = outputString;
                            return true;
                        }

                        break;

                    case '[':
                        if (TryParseArray(json, ref offset, out var outputArray))
                        {
                            output = outputArray;
                            return true;
                        }

                        break;

                    case '{':
                        if (TryParseObject(json, ref offset, out var outputObject))
                        {
                            output = outputObject;
                            return true;
                        }

                        break;

                    default:
                        if (TryParseNumber(json, ref offset, out var outputNumber))
                        {
                            output = outputNumber;
                            return true;
                        }

                        break;
                }
            }

            output = null;
            return false;
        }

        private static bool TryParseBool(string json, ref int offset, out bool output)
        {
            output = false;
            if (offset < json.Length)
            {
                switch (json[offset])
                {
                    case 't':
                        if (offset < json.Length - 3 && json.Substring(offset, 4) == "true")
                        {
                            offset += 4;
                            output = true;
                            return true;
                        }

                        break;

                    case 'f':
                        if (offset < json.Length - 4 && json.Substring(offset, 5) == "false")
                        {
                            offset += 5;
                            return true;
                        }

                        break;
                }
            }

            return false;
        }

        private static bool TryParseNull(string json, ref int offset)
        {
            if (offset < json.Length - 3 && json.Substring(offset, 4) == "null")
            {
                offset += 4;
                return true;
            }

            return false;
        }

        private static bool TryParseNumber(string json, ref int offset, out double output)
        {
            var index = offset;
            if (TryParseNumberIPart(json, ref index))
            {
                SkipNumberFPart(json, ref index);
                SkipNumberExponent(json, ref index);

                var number = json.Substring(offset, index - offset);
                if (double.TryParse(number, out output))
                {
                    offset = index;
                    return true;
                }
            }

            output = default(double);
            return false;
        }

        private static bool TryParseNumberIPart(string json, ref int offset)
        {
            var index = offset;
            if (index < json.Length && json[index] == '-')
            {
                index++;
            }

            if (index < json.Length)
            {
                var firstDigit = json[index];
                if (IsDigit(firstDigit))
                {
                    index++;
                    if (firstDigit > '0')
                    {
                        while (index < json.Length && IsDigit(json[index])) index++;
                    }

                    offset = index;
                    return true;
                }
            }

            return false;
        }

        private static void SkipNumberFPart(string json, ref int offset)
        {
            var index = offset;
            if (index < json.Length && json[index] == '.')
            {
                index++;
                if (index < json.Length && IsDigit(json[index]))
                {
                    index++;
                    while (index < json.Length && IsDigit(json[index])) index++;
                    offset = index;
                }
            }
        }

        private static void SkipNumberExponent(string json, ref int offset)
        {
            var index = offset;
            if (index < json.Length && (json[index] == 'e' || json[index] == 'E'))
            {
                index++;
                if (index < json.Length && (json[index] == '+' || json[index] == '-'))
                {
                    index++;
                }

                if (index < json.Length && IsDigit(json[index]))
                {
                    index++;
                    while (index < json.Length && IsDigit(json[index])) index++;
                    offset = index;
                }
            }
        }

        private static bool IsDigit(char ch) => ch >= '0' && ch <= '9';

        private static bool TryParseString(string json, ref int offset, [NotNullWhen(true)]out string? output)
        {
            output = null;
            var index = offset;
            if (index < json.Length && json[index] == '"')
            {
                index++;

                var builder = new StringBuilder();
                while (index < json.Length)
                {
                    var nextChar = json[index];
                    index++;
                    switch (nextChar)
                    {
                        case '"':
                            output = builder.ToString();
                            offset = index;
                            return true;

                        case '\\':
                            if (TryParseStringEscapeBody(json, ref index, out var escapedChar))
                            {
                                builder.Append(escapedChar);
                            }
                            else
                            {
                                output = null;
                                return false;
                            }

                            break;

                        case var ch when ch < ' ':
                            output = null;
                            return false;

                        default:
                            builder.Append(nextChar);
                            break;
                    }
                }
            }

            output = null;
            return false;
        }

        private static bool TryParseStringEscapeBody(string json, ref int offset, out char output)
        {
            if (offset < json.Length)
            {
                switch (json[offset])
                {
                    case '"':
                        output = '"';
                        break;
                    case '\\':
                        output = '\\';
                        break;
                    case '/':
                        output = '/';
                        break;
                    case 'b':
                        output = '\b';
                        break;
                    case 'f':
                        output = '\f';
                        break;
                    case 'n':
                        output = '\n';
                        break;
                    case 'r':
                        output = '\r';
                        break;
                    case 't':
                        output = '\t';
                        break;
                    case 'u':
                        if (offset < json.Length - 4 &&
                            IsHexDigit(json[offset + 1]) &&
                            IsHexDigit(json[offset + 2]) &&
                            IsHexDigit(json[offset + 3]) &&
                            IsHexDigit(json[offset + 4]))
                        {
                            var hexChars = json.Substring(offset + 1, 4);
                            if (uint.TryParse(hexChars,
                                              NumberStyles.AllowHexSpecifier,
                                              NumberFormatInfo.InvariantInfo,
                                              out var value))
                            {
                                output = (char) value;
                                offset += 5;
                                return true;
                            }
                        }

                        output = default(char);
                        return false;
                    default:
                        output = default(char);
                        return false;
                }

                offset++;
                return true;
            }

            output = default(char);
            return false;
        }

        private static bool IsHexDigit(char ch) =>
            (ch >= '0' && ch <= '9') | (ch >= 'a' && ch <= 'f') | (ch >= 'A' && ch <= 'F');

        private static bool TryParseArray(string json, ref int offset, out object?[]? output)
        {
            var index = offset;
            if (index < json.Length && json[index] == '[')
            {
                index++;

                SkipWhiteSpace(json, ref index);

                if (index < json.Length)
                {
                    if (json[index] == ']')
                    {
                        output = new object[0];
                        offset = index + 1;
                        return true;
                    }

                    var list = new List<object?>();

                    SkipWhiteSpace(json, ref index);
                    if (TryParseValue(json, ref index, out var firstElement))
                    {
                        list.Add(firstElement);
                    }
                    else
                    {
                        output = null;
                        return false;
                    }

                    while (index < json.Length)
                    {
                        SkipWhiteSpace(json, ref index);
                        if (index < json.Length)
                        {
                            switch (json[index])
                            {
                                case ']':
                                    output = list.ToArray();
                                    offset = index + 1;
                                    return true;

                                case ',':
                                    index++;
                                    SkipWhiteSpace(json, ref index);
                                    if (TryParseValue(json, ref index, out var nextElement))
                                    {
                                        list.Add(nextElement);
                                    }
                                    else
                                    {
                                        output = null;
                                        return false;
                                    }

                                    break;

                                default:
                                    output = null;
                                    return false;
                            }
                        }
                        else
                        {
                            output = null;
                            return false;
                        }
                    }
                }
            }

            output = null;
            return false;
        }

        private static bool TryParseObject(string json, ref int offset, out IDictionary<string, object?>? output)
        {
            var index = offset;
            if (index < json.Length && json[index] == '{')
            {
                index++;

                SkipWhiteSpace(json, ref index);

                if (index < json.Length)
                {
                    var map = new SortedDictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);

                    if (json[index] == '}')
                    {
                        output = map;
                        offset = index + 1;
                        return true;
                    }

                    SkipWhiteSpace(json, ref index);
                    if (TryParseObjectKeyValuePair(json, ref index, out var firstElement))
                    {
                        map.Add(firstElement.Key, firstElement.Value);
                    }
                    else
                    {
                        output = null;
                        return false;
                    }

                    while (index < json.Length)
                    {
                        SkipWhiteSpace(json, ref index);
                        if (index < json.Length)
                        {
                            switch (json[index])
                            {
                                case '}':
                                    output = map.ToDictionary(x => x.Key, x => x.Value);
                                    offset = index + 1;
                                    return true;

                                case ',':
                                    index++;
                                    SkipWhiteSpace(json, ref index);
                                    if (TryParseObjectKeyValuePair(json, ref index, out var nextElement) &&
                                        !map.ContainsKey(nextElement.Key))
                                    {
                                        map.Add(nextElement.Key, nextElement.Value);
                                    }
                                    else
                                    {
                                        output = null;
                                        return false;
                                    }

                                    break;

                                default:
                                    output = null;
                                    return false;
                            }
                        }
                        else
                        {
                            output = null;
                            return false;
                        }
                    }
                }
            }

            output = null;
            return false;
        }

        private static bool TryParseObjectKeyValuePair(string json, ref int offset,
                                                       out KeyValuePair<string, object?> output)
        {
            var index = offset;
            if (TryParseString(json, ref index, out var key))
            {
                SkipWhiteSpace(json, ref index);
                if (index < json.Length && json[index] == ':')
                {
                    index++;
                    SkipWhiteSpace(json, ref index);
                    if (TryParseValue(json, ref index, out var value))
                    {
                        output = new KeyValuePair<string, object?>(key, value);
                        offset = index;
                        return true;
                    }
                }
            }

            output = default;
            return false;
        }
    }
}
