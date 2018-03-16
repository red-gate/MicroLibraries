using System;
using System.Collections.Generic;
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
            => target => Newtonsoft.Json.JsonConvert.SerializeObject(target, new JsonSerializerSettings
            {
                Formatting = indented ? Formatting.Indented : Formatting.None,
                DateFormatString = "yyyy-MM-ddTHH:mm:ssZ",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] {new Newtonsoft.Json.Converters.StringEnumConverter()}
            });

        private static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(true, null).SetName("null");

            yield return new TestCaseData(true, true).SetName("true");
            yield return new TestCaseData(true, false).SetName("false");

            yield return new TestCaseData(true, (byte) 5).SetName("byte");
            yield return new TestCaseData(true, byte.MinValue).SetName("byte (min value)");
            yield return new TestCaseData(true, byte.MaxValue).SetName("byte (max value)");

            yield return new TestCaseData(true, (short) 5).SetName("short");
            yield return new TestCaseData(true, (short) 0).SetName("short (zero)");
            yield return new TestCaseData(true, short.MinValue).SetName("short (min value)");
            yield return new TestCaseData(true, short.MaxValue).SetName("short (max value)");

            yield return new TestCaseData(true, (ushort) 5).SetName("ushort");
            yield return new TestCaseData(true, (ushort) 0).SetName("ushort (zero)");
            yield return new TestCaseData(true, ushort.MinValue).SetName("ushort (min value)");
            yield return new TestCaseData(true, ushort.MaxValue).SetName("ushort (max value)");

            yield return new TestCaseData(true, 5).SetName("int");
            yield return new TestCaseData(true, 0).SetName("int (zero)");
            yield return new TestCaseData(true, int.MinValue).SetName("int (min value)");
            yield return new TestCaseData(true, int.MaxValue).SetName("int (max value)");

            yield return new TestCaseData(true, (uint) 5).SetName("uint");
            yield return new TestCaseData(true, (uint) 0).SetName("uint (zero)");
            yield return new TestCaseData(true, uint.MinValue).SetName("uint (min value)");
            yield return new TestCaseData(true, uint.MaxValue).SetName("uint (max value)");

            yield return new TestCaseData(true, 5L).SetName("long");
            yield return new TestCaseData(true, 0L).SetName("long (zero)");
            yield return new TestCaseData(true, long.MinValue).SetName("long (min value)");
            yield return new TestCaseData(true, long.MaxValue).SetName("long (max value)");

            yield return new TestCaseData(true, (ulong) 5).SetName("ulong");
            yield return new TestCaseData(true, (ulong) 0).SetName("ulong (zero)");
            yield return new TestCaseData(true, ulong.MinValue).SetName("ulong (min value)");
            yield return new TestCaseData(true, ulong.MaxValue).SetName("ulong (max value)");

            yield return new TestCaseData(true, (float) 5.5).SetName("float");
            yield return new TestCaseData(true, (float) 0.0).SetName("float (zero)");
            yield return new TestCaseData(true, (float) -0.0).SetName("float (minus zero)");
            yield return new TestCaseData(false, float.Epsilon).SetName("float (epsilon)");
            yield return new TestCaseData(false, -float.Epsilon).SetName("float (minus epsilon)");
            yield return new TestCaseData(true, float.MinValue).SetName("float (min value)");
            yield return new TestCaseData(true, float.MaxValue).SetName("float (max value)");
            yield return new TestCaseData(true, float.PositiveInfinity).SetName("float (posititve infinity)");
            yield return new TestCaseData(true, float.NegativeInfinity).SetName("float (negative infinity)");
            yield return new TestCaseData(true, float.NaN).SetName("float (not a number)");

            yield return new TestCaseData(true, 5.5).SetName("double");
            yield return new TestCaseData(true, 0.0).SetName("double (zero)");
            yield return new TestCaseData(true, -0.0).SetName("double (minus zero)");
            yield return new TestCaseData(false, double.Epsilon).SetName("double (epsilon)");
            yield return new TestCaseData(false, -double.Epsilon).SetName("double (minus epsilon)");
            yield return new TestCaseData(true, double.MinValue).SetName("double (min value)");
            yield return new TestCaseData(true, double.MaxValue).SetName("double (max value)");
            yield return new TestCaseData(true, double.PositiveInfinity).SetName("double (posititve infinity)");
            yield return new TestCaseData(true, double.NegativeInfinity).SetName("double (negative infinity)");
            yield return new TestCaseData(true, double.NaN).SetName("double (not a number)");

            yield return new TestCaseData(true, 5.5D).SetName("decimal");
            yield return new TestCaseData(true, 0.0D).SetName("decimal (zero)");
            yield return new TestCaseData(true, -0.0D).SetName("decimal (minus zero)");
            yield return new TestCaseData(true, Decimal.MinValue).SetName("decimal (min value)");
            yield return new TestCaseData(true, Decimal.MaxValue).SetName("decimal (max value)");

            yield return new TestCaseData(true, 'a').SetName("char");
            yield return new TestCaseData(true, (char) 0).SetName("char (zero)");
            yield return new TestCaseData(true, char.MinValue).SetName("char (min value)");
            yield return new TestCaseData(false, char.MaxValue).SetName("char (max value)");
            for (int i = 0; i < 256; i++)
            {
                // We only expect an exact string match for the basic ascii chars. Beyond that, the new serializer is
                // more agressive about encoding characters than the old Json.NET serializer.
                var expectExactStringMatch = i < 0x7f;
                yield return new TestCaseData(expectExactStringMatch, (char) i).SetName($"char (ascii {i:x2})");
            }

            yield return new TestCaseData(true, new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Utc)).SetName(
                "DateTime (UTC)");
            yield return new TestCaseData(true, new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Local)).SetName(
                "DateTime (Local)");
            yield return new TestCaseData(true, new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Unspecified)).SetName(
                "DateTime (Unspecified)");
            yield return new TestCaseData(true, DateTime.MinValue).SetName("DateTime (min value)");
            yield return new TestCaseData(true, DateTime.MaxValue).SetName("DateTime (max value)");

            yield return new TestCaseData(
                    true, new DateTimeOffset(new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Utc)))
               .SetName("DateTimeOffset (UTC)");
            yield return new TestCaseData(
                    true, new DateTimeOffset(new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Local)))
               .SetName("DateTimeOffset (Local)");
            yield return new TestCaseData(
                    true, new DateTimeOffset(new DateTime(1975, 4, 23, 8, 14, 23, DateTimeKind.Unspecified)))
               .SetName("DateTimeOffset (Unspecified)");
            yield return new TestCaseData(true, DateTimeOffset.MinValue).SetName("DateTimeOffset (min value)");
            yield return new TestCaseData(true, DateTimeOffset.MaxValue).SetName("DateTimeOffset (max value)");

            yield return new TestCaseData(true, Guid.NewGuid()).SetName("guid");

            yield return new TestCaseData(true, ByteEnum.One).SetName("enum (underlying byte)");
            yield return new TestCaseData(true, default(ByteEnum)).SetName("enum (underlying byte, default value)");
            yield return new TestCaseData(true, (ByteEnum) 5).SetName("enum (underlying byte, invalid value)");

            yield return new TestCaseData(true, SByteEnum.One).SetName("enum (underlying sbyte)");
            yield return new TestCaseData(true, default(SByteEnum)).SetName("enum (underlying sbyte, default value)");
            yield return new TestCaseData(true, (SByteEnum) 5).SetName("enum (underlying sbyte, invalid value)");

            yield return new TestCaseData(true, ShortEnum.One).SetName("enum (underlying short)");
            yield return new TestCaseData(true, default(ShortEnum)).SetName("enum (underlying short, default value)");
            yield return new TestCaseData(true, (ShortEnum) 5).SetName("enum (underlying short, invalid value)");

            yield return new TestCaseData(true, UShortEnum.One).SetName("enum (underlying ushort)");
            yield return new TestCaseData(true, default(UShortEnum)).SetName("enum (underlying ushort, default value)");
            yield return new TestCaseData(true, (UShortEnum) 5).SetName("enum (underlying ushort, invalid value)");

            yield return new TestCaseData(true, IntEnum.One).SetName("enum (underlying int)");
            yield return new TestCaseData(true, default(IntEnum)).SetName("enum (underlying int, default value)");
            yield return new TestCaseData(true, (IntEnum) 5).SetName("enum (underlying int, invalid value)");

            yield return new TestCaseData(true, UIntEnum.One).SetName("enum (underlying uint)");
            yield return new TestCaseData(true, default(UIntEnum)).SetName("enum (underlying uint, default value)");
            yield return new TestCaseData(true, (UIntEnum) 5).SetName("enum (underlying uint, invalid value)");

            yield return new TestCaseData(true, LongEnum.One).SetName("enum (underlying long)");
            yield return new TestCaseData(true, default(LongEnum)).SetName("enum (underlying long, default value)");
            yield return new TestCaseData(true, (LongEnum) 5).SetName("enum (underlying long, invalid value)");

            yield return new TestCaseData(true, ULongEnum.One).SetName("enum (underlying ulong)");
            yield return new TestCaseData(true, default(ULongEnum)).SetName("enum (underlying ulong, default value)");
            yield return new TestCaseData(true, (ULongEnum) 5).SetName("enum (underlying ulong, invalid value)");

            yield return new TestCaseData(true, FlagsEnum.Alpha).SetName("enum (flags, single value)");
            yield return new TestCaseData(true, FlagsEnum.Alpha | FlagsEnum.Bravo).SetName(
                "enum (flags, combined value)");
            yield return new TestCaseData(true, (FlagsEnum) 6).SetName("enum (flags, combined value cast from int)");
            yield return new TestCaseData(true, default(FlagsEnum)).SetName("enum (flags, default value)");
            yield return new TestCaseData(true, (FlagsEnum) 13).SetName("enum (flags, invalid value)");

            yield return new TestCaseData(true, "abc").SetName("string");
            yield return new TestCaseData(true, "").SetName("string (empty)");
            for (int i = 0; i < 32; i++)
            {
                yield return new TestCaseData(true, new string(new[] {(char) i})).SetName(
                    $"string (control code {i:x2})");
            }

            for (int i = 32; i <= 255; i++)
            {
                // We only expect an exact string match for the basic ascii chars. Beyond that, the new serializer is
                // more agressive about encoding characters than the old Json.NET serializer.
                var expectExactStringMatch = i < 0x7f;

                var str = new string(new[] {(char) i});
                yield return new TestCaseData(expectExactStringMatch, str).SetName($"string (ascii {i:x2} = '{str}')");
            }

            yield return new TestCaseData(false, "\U0001F01C").SetName("string (surrogate pairs)");

            yield return new TestCaseData(true, new object[] {"abc"}).SetName("array (of object, one element)");
            yield return new TestCaseData(true, new object[] {"abc", 123}).SetName("array (of object, two elements)");
            yield return new TestCaseData(true, new object[] { }).SetName("array (of object, empty)");
            yield return new TestCaseData(true, new object[] {"abc", new object[] {true, false}, 123}).SetName(
                "array (nested elements)");

            yield return new TestCaseData(true, new[] {1, 2, 3}).SetName("array (of int)");
            yield return new TestCaseData(true, new[] {"a", "b", "c"}).SetName("array (of string)");

            yield return new TestCaseData(true, new List<object> {"abc", 123}).SetName("list (of object)");
            yield return new TestCaseData(true, new List<int> {1, 2, 3}).SetName("list (of int)");
            yield return new TestCaseData(true, new List<string> {"a", "b", "c"}).SetName("list (of string)");

            object nested = null;
            for (int i = 0; i < 50; i++)
            {
                nested = new[] {i, nested};
            }

            yield return new TestCaseData(true, nested).SetName("highly nested");

            yield return new TestCaseData(true, new { }).SetName("object (empty)");
            yield return new TestCaseData(true, new {abc = 123}).SetName("object (single property)");
            yield return new TestCaseData(true, new {abc = 123, def = 456}).SetName("object (two properties)");
            yield return new TestCaseData(true, new {abc = 123, def = new {ghi = 456}}).SetName(
                "object (nested objects)");
            yield return new TestCaseData(true, new {ABC = 123}).SetName("object (camel-case, all uppercase)");
            yield return new TestCaseData(true, new {FooBar = 123}).SetName(
                "object (camel-case, two words, both with capital letter)");
            yield return new TestCaseData(true, new {SQLServer = 123}).SetName(
                "object (camel-case, two words, first is all uppercase)");

            yield return
                new TestCaseData(true, new Dictionary<string, object> {["abc"] = 123, ["def"] = "ghi"}).SetName(
                    "dictionary (of string and object)");
            yield return new TestCaseData(true, new Dictionary<string, object>()).SetName(
                "dictionary (of string and object - empty)");
            yield return
                new TestCaseData(true, new Dictionary<object, object> {["abc"] = 123, ["def"] = "ghi"}).SetName(
                    "dictionary (of object and object)");
            yield return new TestCaseData(true, new Dictionary<object, object> {[10] = 123, [20] = "ghi"}).SetName(
                "dictionary (of object and object, with non-string keys)");
            yield return new TestCaseData(true,
                                          new Dictionary<string, object>
                                          {
                                              ["abc"] = 123,
                                              ["def"] = new Dictionary<string, object>
                                              {
                                                  ["pqr"] = 123,
                                                  ["stu"] = "ghi"
                                              },
                                              ["ghi"] = false
                                          }).SetName("dictionary (with nested elements)");
        }

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

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CheckCompact(bool expectExactStringMatch, object target) =>
            Check(expectExactStringMatch, target, false);

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CheckIndented(bool expectExactStringMatch, object target) =>
            Check(expectExactStringMatch, target, true);

        private void Check(bool expectExactStringMatch, object target, bool indented)
        {
            var referenceSerializer = CreateReferenceSerializer(indented);
            var tinyJsonSerializer = CreateTinyJsonSerializer(indented);

            var referenceJson = referenceSerializer(target);
            var actualJson = tinyJsonSerializer(target);

            Console.WriteLine($"Reference value: '{referenceJson}'");
            Console.WriteLine($"Our value:       '{actualJson}'");

            if (expectExactStringMatch)
            {
                Assert.That(actualJson, Is.EqualTo(referenceJson));
            }
            else
            {
                // We could assert that the json strings are equal, but there are minor differences in the json encoding between the
                // reference and new serializer. For example, object properties may not retain the same ordering, and the new
                // serializer is more strict about encoding non-safe characters in strings.

                var referenceJToken = JToken.Parse(referenceJson);
                var actualJToken = JToken.Parse(actualJson);

                Assert.That(actualJToken,
                            Is.EqualTo(referenceJToken).Using((JToken a, JToken b) => JToken.DeepEquals(a, b) ? 0 : 1));
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