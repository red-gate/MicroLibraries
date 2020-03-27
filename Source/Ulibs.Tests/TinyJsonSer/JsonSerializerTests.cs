using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ulibs.Tests.TinyJsonSer
{
    [TestFixture]
    public class JsonSerializerTests
    {
        [Test]
        public void SerializeToStringBuilder()
        {
            var builder = new StringBuilder();
            new ULibs.TinyJsonSer.JsonSerializer(false).Serialize(new object[] {123, "abc"}, builder);
            var json = builder.ToString();
            Assert.That(json, Is.EqualTo("[123,\"abc\"]"));
        }

        [Test]
        public void SerializeToStringBuilder_NullBuilderCheck()
        {
            var serializer = new ULibs.TinyJsonSer.JsonSerializer(false);
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize("abc", (StringBuilder) null));
        }

        [Test]
        public void SerializeToTextWriter()
        {
            using (var writer = new StringWriter())
            {
                new ULibs.TinyJsonSer.JsonSerializer(false).Serialize(new object[] { 123, "abc" }, writer);
                var json = writer.ToString();
                Assert.That(json, Is.EqualTo("[123,\"abc\"]"));
            }
        }

        [Test]
        public void SerializeToTextWriter_NullWriterCheck()
        {
            var serializer = new ULibs.TinyJsonSer.JsonSerializer(false);
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize("abc", (TextWriter) null));
        }

        /// <summary>
        /// Factory method that creates a new serializer to be tested.
        /// </summary>
        private Func<object, string> CreateTinyJsonSerializer(bool indented) =>
            new ULibs.TinyJsonSer.JsonSerializer(indented).Serialize;

        /// <summary>
        /// Factory method that creates a reference serializer based on the Newtonsoft.Json code.
        /// </summary>
        private Func<object, string> CreateReferenceSerializer(bool indented)
            => target => JsonConvert.SerializeObject(target, new JsonSerializerSettings
            {
                Formatting = indented ? Formatting.Indented : Formatting.None,
                DateFormatString = "yyyy-MM-ddTHH:mm:ssZ",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] {new Newtonsoft.Json.Converters.StringEnumConverter()}
            });

        private static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(ComparisonType.ExactMatch, null).SetName("null");

            yield return new TestCaseData(ComparisonType.ExactMatch, true).SetName("true");
            yield return new TestCaseData(ComparisonType.ExactMatch, false).SetName("false");

            yield return new TestCaseData(ComparisonType.ExactMatch, (byte) 5).SetName("byte");
            yield return new TestCaseData(ComparisonType.ExactMatch, byte.MinValue).SetName("byte (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, byte.MaxValue).SetName("byte (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, (short) 5).SetName("short");
            yield return new TestCaseData(ComparisonType.ExactMatch, (short) 0).SetName("short (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, short.MinValue).SetName("short (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, short.MaxValue).SetName("short (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, (ushort) 5).SetName("ushort");
            yield return new TestCaseData(ComparisonType.ExactMatch, (ushort) 0).SetName("ushort (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, ushort.MinValue).SetName("ushort (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, ushort.MaxValue).SetName("ushort (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, 5).SetName("int");
            yield return new TestCaseData(ComparisonType.ExactMatch, 0).SetName("int (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, int.MinValue).SetName("int (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, int.MaxValue).SetName("int (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, (uint) 5).SetName("uint");
            yield return new TestCaseData(ComparisonType.ExactMatch, (uint) 0).SetName("uint (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, uint.MinValue).SetName("uint (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, uint.MaxValue).SetName("uint (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, 5L).SetName("long");
            yield return new TestCaseData(ComparisonType.ExactMatch, 0L).SetName("long (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, long.MinValue).SetName("long (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, long.MaxValue).SetName("long (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, (ulong) 5).SetName("ulong");
            yield return new TestCaseData(ComparisonType.ExactMatch, (ulong) 0).SetName("ulong (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, ulong.MinValue).SetName("ulong (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, ulong.MaxValue).SetName("ulong (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, (float) 5.5).SetName("float");
            yield return new TestCaseData(ComparisonType.ExactMatch, (float) 0.0).SetName("float (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (float) -0.0).SetName("float (minus zero)");
            yield return new TestCaseData(ComparisonType.DeepEquals, float.Epsilon).SetName("float (epsilon)");
            yield return new TestCaseData(ComparisonType.DeepEquals, -float.Epsilon).SetName("float (minus epsilon)");
            yield return new TestCaseData(
                ComparisonType.RegexMatch(@"^(-3\.40282347E\+38)|(-3\.402823466E\+38)$"),
                float.MinValue)
               .SetName("float (min value)");
            yield return new TestCaseData(
                ComparisonType.RegexMatch(@"^(3\.40282347E\+38)|(3\.402823466E\+38)$"),
                float.MaxValue)
               .SetName("float (max value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, float.PositiveInfinity)
               .SetName("float (posititve infinity)");
            yield return new TestCaseData(ComparisonType.ExactMatch, float.NegativeInfinity)
               .SetName("float (negative infinity)");
            yield return new TestCaseData(ComparisonType.ExactMatch, float.NaN).SetName("float (not a number)");

            yield return new TestCaseData(ComparisonType.ExactMatch, 5.5).SetName("double");
            yield return new TestCaseData(ComparisonType.ExactMatch, 0.0).SetName("double (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, -0.0).SetName("double (minus zero)");
            yield return new TestCaseData(ComparisonType.DeepEquals, double.Epsilon).SetName("double (epsilon)");
            yield return new TestCaseData(ComparisonType.DeepEquals, -double.Epsilon).SetName("double (minus epsilon)");
            yield return new TestCaseData(ComparisonType.ExactMatch, double.MinValue).SetName("double (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, double.MaxValue).SetName("double (max value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, double.PositiveInfinity)
               .SetName("double (posititve infinity)");
            yield return new TestCaseData(ComparisonType.ExactMatch, double.NegativeInfinity)
               .SetName("double (negative infinity)");
            yield return new TestCaseData(ComparisonType.ExactMatch, double.NaN).SetName("double (not a number)");

            yield return new TestCaseData(ComparisonType.ExactMatch, 5.5D).SetName("decimal");
            yield return new TestCaseData(ComparisonType.ExactMatch, 0.0D).SetName("decimal (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, -0.0D).SetName("decimal (minus zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, Decimal.MinValue).SetName("decimal (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, Decimal.MaxValue).SetName("decimal (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, 'a').SetName("char");
            yield return new TestCaseData(ComparisonType.ExactMatch, (char) 0).SetName("char (zero)");
            yield return new TestCaseData(ComparisonType.ExactMatch, char.MinValue).SetName("char (min value)");
            yield return new TestCaseData(ComparisonType.DeepEquals, char.MaxValue).SetName("char (max value)");
            for (int i = 0; i < 256; i++)
            {
                // We only expect an exact string match for the basic ascii chars. Beyond that, the new serializer is
                // more aggressive about encoding characters than the old Json.NET serializer.
                var comparisonType = i < 0x7f ? ComparisonType.ExactMatch : ComparisonType.DeepEquals;
                yield return new TestCaseData(comparisonType, (char) i).SetName($"char (ascii {i:x2})");
            }

            yield return new TestCaseData(
                ComparisonType.ExactMatch,
                new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Utc)).SetName("DateTime (UTC)");
            yield return new TestCaseData(
                ComparisonType.ExactMatch,
                new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Local)).SetName("DateTime (Local)");
            yield return new TestCaseData(
                ComparisonType.ExactMatch,
                new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Unspecified)).SetName("DateTime (Unspecified)");
            yield return new TestCaseData(ComparisonType.ExactMatch, DateTime.MinValue).SetName("DateTime (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, DateTime.MaxValue).SetName("DateTime (max value)");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new DateTimeOffset(new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Utc)))
               .SetName("DateTimeOffset (UTC)");
            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new DateTimeOffset(new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Local)))
               .SetName("DateTimeOffset (Local)");
            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new DateTimeOffset(new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Unspecified)))
               .SetName("DateTimeOffset (Unspecified)");
            yield return new TestCaseData(ComparisonType.ExactMatch, DateTimeOffset.MinValue)
               .SetName("DateTimeOffset (min value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, DateTimeOffset.MaxValue)
               .SetName("DateTimeOffset (max value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, Guid.NewGuid()).SetName("guid");

            yield return new TestCaseData(ComparisonType.ExactMatch, ByteEnum.One).SetName("enum (underlying byte)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(ByteEnum))
               .SetName("enum (underlying byte, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (ByteEnum) 5)
               .SetName("enum (underlying byte, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, SByteEnum.One).SetName("enum (underlying sbyte)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(SByteEnum))
               .SetName("enum (underlying sbyte, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (SByteEnum) 5)
               .SetName("enum (underlying sbyte, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, ShortEnum.One).SetName("enum (underlying short)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(ShortEnum))
               .SetName("enum (underlying short, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (ShortEnum) 5)
               .SetName("enum (underlying short, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, UShortEnum.One)
               .SetName("enum (underlying ushort)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(UShortEnum))
               .SetName("enum (underlying ushort, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (UShortEnum) 5)
               .SetName("enum (underlying ushort, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, IntEnum.One).SetName("enum (underlying int)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(IntEnum))
               .SetName("enum (underlying int, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (IntEnum) 5)
               .SetName("enum (underlying int, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, UIntEnum.One).SetName("enum (underlying uint)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(UIntEnum))
               .SetName("enum (underlying uint, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (UIntEnum) 5)
               .SetName("enum (underlying uint, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, LongEnum.One).SetName("enum (underlying long)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(LongEnum))
               .SetName("enum (underlying long, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (LongEnum) 5)
               .SetName("enum (underlying long, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, ULongEnum.One).SetName("enum (underlying ulong)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(ULongEnum))
               .SetName("enum (underlying ulong, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (ULongEnum) 5)
               .SetName("enum (underlying ulong, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, FlagsEnum.Alpha)
               .SetName("enum (flags, single value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, FlagsEnum.Alpha | FlagsEnum.Bravo)
               .SetName("enum (flags, combined value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (FlagsEnum) 6)
               .SetName("enum (flags, combined value cast from int)");
            yield return new TestCaseData(ComparisonType.ExactMatch, default(FlagsEnum))
               .SetName("enum (flags, default value)");
            yield return new TestCaseData(ComparisonType.ExactMatch, (FlagsEnum) 13)
               .SetName("enum (flags, invalid value)");

            yield return new TestCaseData(ComparisonType.ExactMatch, "abc").SetName("string");
            yield return new TestCaseData(ComparisonType.ExactMatch, "").SetName("string (empty)");
            for (int i = 0; i < 32; i++)
            {
                yield return new TestCaseData(ComparisonType.ExactMatch, new string(new[] {(char) i}))
                   .SetName($"string (control code {i:x2})");
            }

            for (int i = 32; i <= 255; i++)
            {
                // We only expect an exact string match for the basic ascii chars. Beyond that, the new serializer is
                // more agressive about encoding characters than the old Json.NET serializer.

                var str = new string(new[] {(char) i});
                var comparisonType = i < 0x7f ? ComparisonType.ExactMatch : ComparisonType.DeepEquals;
                yield return new TestCaseData(comparisonType, str).SetName($"string (ascii {i:x2} = '{str}')");
            }

            yield return new TestCaseData(ComparisonType.DeepEquals, "\U0001F01C").SetName("string (surrogate pairs)");

            yield return new TestCaseData(ComparisonType.ExactMatch, new object[] {"abc"})
               .SetName("array (of object, one element)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new object[] {"abc", 123})
               .SetName("array (of object, two elements)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new object[] { })
               .SetName("array (of object, empty)");
            yield return new TestCaseData(
                ComparisonType.ExactMatch, new object[] {"abc", new object[] {true, false}, 123})
               .SetName("array (nested elements)");

            yield return new TestCaseData(ComparisonType.ExactMatch, new[] {1, 2, 3}).SetName("array (of int)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new[] {"a", "b", "c"})
               .SetName("array (of string)");

            yield return new TestCaseData(ComparisonType.ExactMatch, new List<object> {"abc", 123})
               .SetName("list (of object)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new List<int> {1, 2, 3}).SetName("list (of int)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new List<string> {"a", "b", "c"})
               .SetName("list (of string)");

            object nested = null;
            for (int i = 0; i < 50; i++)
            {
                nested = new[] {i, nested};
            }

            yield return new TestCaseData(ComparisonType.ExactMatch, nested).SetName("highly nested");

            yield return new TestCaseData(ComparisonType.ExactMatch, new { }).SetName("object (empty)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new {abc = 123})
               .SetName("object (single property)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new {abc = 123, def = 456})
               .SetName("object (two properties)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new {abc = 123, def = new {ghi = 456}})
               .SetName("object (nested objects)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new {ABC = 123})
               .SetName("object (camel-case, all uppercase)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new {FooBar = 123})
               .SetName("object (camel-case, two words, both with capital letter)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new {SQLServer = 123})
               .SetName("object (camel-case, two words, first is all uppercase)");

            yield return new TestCaseData(
                ComparisonType.ExactMatch, new Dictionary<string, object> {["abc"] = 123, ["def"] = "ghi"})
               .SetName("dictionary (of string and object)");
            yield return new TestCaseData(ComparisonType.ExactMatch, new Dictionary<string, object>())
               .SetName("dictionary (of string and object - empty)");
            yield return new TestCaseData(
                ComparisonType.ExactMatch, new Dictionary<object, object> {["abc"] = 123, ["def"] = "ghi"})
               .SetName("dictionary (of object and object)");
            yield return new TestCaseData(
                ComparisonType.ExactMatch, new Dictionary<object, object> {[10] = 123, [20] = "ghi"})
               .SetName("dictionary (of object and object, with non-string keys)");
            yield return new TestCaseData(
                ComparisonType.ExactMatch,
                new Dictionary<string, object>
                {
                    ["abc"] = 123,
                    ["def"] = new Dictionary<string, object>
                    {
                        ["pqr"] = 123,
                        ["stu"] = "ghi"
                        
                    },
                    ["ghi"] = false
                    
                })
               .SetName("dictionary (with nested elements)");
            dynamic expando = new ExpandoObject();
            expando.SomeProperty = 123;
            yield return new TestCaseData(ComparisonType.ExactMatch, expando).SetName("ExpandoObject");

            yield return new TestCaseData(ComparisonType.ExactMatch, new CustomEnumerable<string>("ABC"))
               .SetName("Custom IEnumerable<string>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomEnumerable<KeyValuePair<string, object>>(new KeyValuePair<string, object>("ABC", 123)))
               .SetName("Custom IEnumerable<KeyValuePair<string, object>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomEnumerable<KeyValuePair<string, string>>(new KeyValuePair<string, string>("ABC", "123")))
               .SetName("Custom IEnumerable<KeyValuePair<string, string>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomEnumerable<KeyValuePair<object, object>>(new KeyValuePair<object, object>("ABC", 123)))
               .SetName("Custom IEnumerable<KeyValuePair<object, object>>");

            yield return new TestCaseData(ComparisonType.ExactMatch, new CustomReadOnlyCollection<string>("ABC"))
               .SetName("Custom IReadOnlyCollection<string>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyCollection<KeyValuePair<string, object>>(
                        new KeyValuePair<string, object>("ABC", 123)))
               .SetName("Custom IReadOnlyCollection<KeyValuePair<string, object>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyCollection<KeyValuePair<string, string>>(
                        new KeyValuePair<string, string>("ABC", "123")))
               .SetName("Custom IReadOnlyCollection<KeyValuePair<string, string>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyCollection<KeyValuePair<object, object>>(
                        new KeyValuePair<object, object>("ABC", "123")))
               .SetName("Custom IReadOnlyCollection<KeyValuePair<object, object>>");

            yield return new TestCaseData(ComparisonType.ExactMatch, new CustomReadOnlyList<string>("ABC"))
               .SetName("Custom IReadOnlyList<string>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyList<KeyValuePair<string, object>>(new KeyValuePair<string, object>("ABC", 123)))
               .SetName("Custom IReadOnlyList<KeyValuePair<string, object>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyList<KeyValuePair<string, string>>(
                        new KeyValuePair<string, string>("ABC", "123")))
               .SetName("Custom IReadOnlyList<KeyValuePair<string, string>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyList<KeyValuePair<object, object>>(
                        new KeyValuePair<object, object>("ABC", "123")))
               .SetName("Custom IReadOnlyList<KeyValuePair<object, object>>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyDictionary<string, object>("ABC", 123))
               .SetName("Custom IReadOnlyDictionary<string, object>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyDictionary<string, string>("ABC", "123"))
               .SetName("Custom IReadOnlyDictionary<string, string>");

            yield return new TestCaseData(
                    ComparisonType.ExactMatch,
                    new CustomReadOnlyDictionary<object, object>("ABC", "123"))
               .SetName("Custom IReadOnlyDictionary<object, object>");

            yield return new TestCaseData(ComparisonType.ExactMatch, new CustomDictionary<string, object>("ABC", 123))
               .SetName("Custom IDictionary<string, object>");

            yield return new TestCaseData(ComparisonType.ExactMatch, new CustomDictionary<string, string>("ABC", "123"))
               .SetName("Custom IDictionary<string, string>");

            yield return new TestCaseData(ComparisonType.ExactMatch, new CustomDictionary<object, object>("ABC", "123"))
               .SetName("Custom IDictionary<object, object>");
        }

        #region ComparisonTypes
        
        /// <summary>
        /// Base type for a "discriminated union" set of classes that describe how to perform an assertion on the
        /// behaviour of the serializer in a test. 
        /// </summary>
        public abstract class ComparisonType
        {
            /// <summary>
            /// See <see cref="JsonSerializerTests.ExactMatch"/>. 
            /// </summary>
            public static readonly ComparisonType ExactMatch = new ExactMatch();
            
            /// <summary>
            /// See <see cref="DeepComparison"/>.
            /// </summary>
            public static readonly ComparisonType DeepEquals = new DeepComparison();
            
            /// <summary>
            /// See <see cref="JsonSerializerTests.RegexMatch"/>.
            /// </summary>
            /// <param name="pattern"></param>
            /// <returns></returns>
            public static ComparisonType RegexMatch(string pattern) => new RegexMatch(pattern);
        }

        /// <summary>
        /// <see cref="ComparisonType"/> subclass that indicates that JsonSerializer is expected to produce json that is
        /// identical to that of the reference Newtonsoft.Json serializer. 
        /// </summary>
        private class ExactMatch : ComparisonType
        {
        }

        /// <summary>
        /// <see cref="ComparisonType"/> subclass that indicates that JsonSerializer is expected to produce json that,
        /// whilst not identical to that of the reference Newtonsoft.Json serializer, it will be semantically
        /// equivalent. For example, our serializer always escapes non-ascii characters, whilst the Newtonsoft.Json
        /// serializer does not. In this case, the output json for both JsonSerializer and the Newtonsoft.Json reference
        /// are reparsed by Newtonsoft.Json to a JToken, and we then rely on its DeepEquals method for comparison.  
        /// </summary>
        private class DeepComparison : ComparisonType
        {
        }

        /// <summary>
        /// <see cref="ComparisonType"/> subclass that indicates that JsonSerializer is expected to produce json that
        /// should match a specific regex. This is used in a tiny handful of cases where the serialization behaviour is
        /// affected by the .NET runtime (currently there are minute differences between .NET CORE and .NET Framework in
        /// the way float.MinValue and float.MaxValue are expressed). 
        /// </summary>
        private class RegexMatch : ComparisonType
        {
            public string Pattern { get; }

            public RegexMatch(string pattern)
            {
                Pattern = pattern;
            }
        }

        #endregion
        
        #region Test enum types

        [Flags]
        private enum FlagsEnum
        {
            Alpha = 2,
            Bravo = 4
        }

        private enum ByteEnum : byte
        {
            One = 1
        }

        private enum SByteEnum : sbyte
        {
            One = 1
        }

        private enum ShortEnum : short
        {
            One = 1
        }

        private enum UShortEnum : ushort
        {
            One = 1
        }

        private enum IntEnum
        {
            One = 1
        }

        private enum UIntEnum : uint
        {
            One = 1
        }

        private enum LongEnum : long
        {
            One = 1
        }

        private enum ULongEnum : ulong
        {
            One = 1
        }

        #endregion
        
        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CheckCompact(ComparisonType comparisonType, object target) =>
            Check(comparisonType, target, false);

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CheckIndented(ComparisonType comparisonType, object target) =>
            Check(comparisonType, target, true);

        private void Check(ComparisonType comparisonType, object target, bool indented)
        {
            var tinyJsonSerializer = CreateTinyJsonSerializer(indented);
            var actualJson = tinyJsonSerializer(target);

            switch (comparisonType)
            {
                case ExactMatch _:
                    {
                        var referenceSerializer = CreateReferenceSerializer(indented);
                        var referenceJson = referenceSerializer(target);

                        Console.WriteLine($"Reference value: '{referenceJson}'");
                        Console.WriteLine($"Our value:       '{actualJson}'");

                        Assert.That(actualJson, Is.EqualTo(referenceJson));
                    }
                    break;

                case DeepComparison _:
                    {
                        var referenceSerializer = CreateReferenceSerializer(indented);
                        var referenceJson = referenceSerializer(target);

                        Console.WriteLine($"Reference value: '{referenceJson}'");
                        Console.WriteLine($"Our value:       '{actualJson}'");
                        
                        var referenceJToken = JToken.Parse(referenceJson);
                        var actualJToken = JToken.Parse(actualJson);

                        Assert.That(
                            actualJToken,
                            Is.EqualTo(referenceJToken).Using((JToken a, JToken b) => JToken.DeepEquals(a, b) ? 0 : 1));
                    }
                    break;

                case RegexMatch match:
                    {
                        Console.WriteLine($"Reference regex: '{match.Pattern}'");
                        Console.WriteLine($"Our value:       '{actualJson}'");
                        
                        Assert.That(actualJson, Does.Match(match.Pattern));
                    }
                    break;
                
                default:
                    Assert.Fail($"Unhandled comparison type {comparisonType.GetType().FullName}");
                    break;
            }
        }

        [Test]
        public void CheckCyclicReference()
        {
            var a = new object[] {1, 2, 3};
            var b = new object[] {10, 20, 30};
            a[1] = b;
            b[1] = a;

            var tinyJsonSerializer = CreateTinyJsonSerializer(false);
            Assert.That(() => tinyJsonSerializer(a), Throws.InstanceOf<InvalidOperationException>());
        }
    }
}