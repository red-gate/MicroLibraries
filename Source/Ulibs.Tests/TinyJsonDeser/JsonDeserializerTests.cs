using System.Collections.Generic;
using NUnit.Framework;
using ULibs.TinyJsonDeser;

namespace Ulibs.Tests.TinyJsonDeser
{
    [TestFixture]
    public class JsonDeserializerTests
    {
        [Test]
        public void TestTryParseBool_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseBool(null, out var output), Throws.ArgumentNullException);
        }

        [TestCase("true", TestName = "True")]
        [TestCase("  true", TestName = "True with leading whitespace")]
        [TestCase("true  ", TestName = "True with trailing whitespace")]
        public void TestTryParseBool_Success_True(string json)
        {
            Assert.That(new JsonDeserializer().TryParseBool(json, out var output), Is.True);
            Assert.That(output, Is.True);
        }

        [TestCase("false", TestName = "False")]
        [TestCase("  false", TestName = "False with leading whitespace")]
        [TestCase("false  ", TestName = "False with trailing whitespace")]
        public void TestTryParseBool_Success_False(string json)
        {
            Assert.That(new JsonDeserializer().TryParseBool(json, out var output), Is.True);
            Assert.That(output, Is.False);
        }

        [TestCase("", TestName = "Empty string")]
        [TestCase("xtrue", TestName = "True with unexpected leading character")]
        [TestCase("xfalse", TestName = "True with unexpected leading character")]
        [TestCase("truex", TestName = "True with unexpected trailing character")]
        [TestCase("falsex", TestName = "True with unexpected trailing character")]
        [TestCase("True", TestName = "True with invalid casing")]
        [TestCase("False", TestName = "False with invalid casing")]
        [TestCase("tru", TestName = "True incomplete")]
        [TestCase("fals", TestName = "False incomplete")]
        public void TestTryParseBool_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseBool(json, out var output), Is.False);
        }

        [Test]
        public void TestTryParseNull_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseNull(null), Throws.ArgumentNullException);
        }

        [TestCase("null", TestName = "Null")]
        [TestCase("  null", TestName = "Null with leading whitespace")]
        [TestCase("null  ", TestName = "Null with trailing whitespace")]
        public void TestTryParseNull_Success(string json)
        {
            Assert.That(new JsonDeserializer().TryParseNull(json), Is.True);
        }

        [TestCase("", TestName = "Empty string")]
        [TestCase("null x", TestName = "Null with unexpected leading character")]
        [TestCase("x null", TestName = "Null with unexpected trailing character")]
        [TestCase("Null", TestName = "Null with invalid casing")]
        [TestCase("nul", TestName = "Null incomplete")]
        public void TestTryParseNull_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseNull(json), Is.False);
        }

        [Test]
        public void TestTryParseNumber_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseNumber(null, out var output), Throws.ArgumentNullException);
        }

        [TestCase("1", 1)]
        [TestCase("  1", 1)]
        [TestCase("1  ", 1)]
        [TestCase("-1", -1)]
        [TestCase("0", 0)]
        [TestCase("-0", 0)]
        [TestCase("12", 12)]
        [TestCase("12.5", 12.5)]
        [TestCase("12.25", 12.25)]
        [TestCase("1e2", 1e2)]
        [TestCase("1E2", 1E2)]
        [TestCase("1e+2", 1e+2)]
        [TestCase("1e-2", 1e-2)]
        [TestCase("1e12", 1e12)]
        [TestCase("1.5e2", 1.5e2)]
        [TestCase("1.25e2", 1.25e2)]
        public void TestTryParseNumber_Success(string json, double expected)
        {
            Assert.That(new JsonDeserializer().TryParseNumber(json, out var output), Is.True);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("01")]
        [TestCase("+1")]
        [TestCase("+1.2")]
        [TestCase("+1.2e3")]
        [TestCase("1.")]
        [TestCase("1.e2")]
        [TestCase("1.2e")]
        [TestCase(".2e2")]
        [TestCase("")]
        public void TestTryParseNumber_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseNumber(json, out var output), Is.False);
        }

        [Test]
        public void TestTryParseString_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseString(null, out var output), Throws.ArgumentNullException);
        }

        [TestCase("\"\"", "", TestName = "Empty string")]
        [TestCase("\"a\"", "a", TestName = "Single char")]
        [TestCase("\"ab\"", "ab", TestName = "Multiple chars")]
        [TestCase("\"\\\"\"", "\"", TestName = "Escaped double-quotes char")]
        [TestCase("\"a\\\"b\"", "a\"b", TestName = "Escaped double-quotes char within a string")]
        [TestCase("\"\\\\\"", "\\", TestName = "Escaped back-slash char")]
        [TestCase("\"a\\\\b\"", "a\\b", TestName = "Escaped back-slash char within a string")]
        [TestCase("\"\\/\"", "/", TestName = "Escaped forward-slash char")]
        [TestCase("\"a\\/b\"", "a/b", TestName = "Escaped forward-slash char within a string")]
        [TestCase("\"/\"", "/", TestName = "Non-escaped forward-slash char")]
        [TestCase("\"a/b\"", "a/b", TestName = "Non-escaped forward-slash char within a string")]
        [TestCase("\"\\b\"", "\b", TestName = "Escaped backspace char")]
        [TestCase("\"a\\bb\"", "a\bb", TestName = "Escaped backspace char within a string")]
        [TestCase("\"\\f\"", "\f", TestName = "Escaped formfeed char")]
        [TestCase("\"a\\fb\"", "a\fb", TestName = "Escaped formfeed char within a string")]
        [TestCase("\"\\n\"", "\n", TestName = "Escaped newline char")]
        [TestCase("\"a\\nb\"", "a\nb", TestName = "Escaped newline char within a string")]
        [TestCase("\"\\r\"", "\r", TestName = "Escaped carriage return char")]
        [TestCase("\"a\\rb\"", "a\rb", TestName = "Escaped carriage return char within a string")]
        [TestCase("\"\\t\"", "\t", TestName = "Escaped horizontal tab char")]
        [TestCase("\"a\\tb\"", "a\tb", TestName = "Escaped horizontal tab char within a string")]
        [TestCase("\"\\u03DD\"", "\u03DD", TestName = "Escaped upper-case unicode char")]
        [TestCase("\"a\\u03DDb\"", "a\u03DDb", TestName = "Escaped upper-case unicode char within a string")]
        [TestCase("\"\\u03dd\"", "\u03dd", TestName = "Escaped lower-case unicode char")]
        [TestCase("\"a\\u03ddb\"", "a\u03ddb", TestName = "Escaped lower-case unicode char within a string")]
        public void TestTryParseString_Success(string json, string expected)
        {
            Assert.That(new JsonDeserializer().TryParseString(json, out var output), Is.True);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("\"abc", TestName = "No clossing double-quotes")]
        [TestCase("\"\\z\"", TestName = "Unsupported escape")]
        [TestCase("\"\\uABC\"", TestName = "Incomplete unicode escape")]
        public void TryParseString_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseString(json, out var output), Is.False);
        }

        [Test]
        public void TestTryParseArray_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseArray(null, out var output), Throws.ArgumentNullException);
        }

        [TestCase("[]", new object[0], TestName = "Empty array")]
        [TestCase("  []", new object[0], TestName = "Empty array with leading whitespace")]
        [TestCase("[]  ", new object[0], TestName = "Empty array with trailing whitespace")]
        [TestCase("[  ]", new object[0], TestName = "Empty array with internal whitespace")]
        [TestCase("[1]", new object[] {1}, TestName = "Array with single element")]
        [TestCase("[ 1]", new object[] {1}, TestName = "Array with single element and internal leading whitespace")]
        [TestCase("[1 ]", new object[] {1}, TestName = "Array with single element and internal trailing whitespace")]
        [TestCase("[12]", new object[] {12}, TestName = "Array with single element having multiple characters")]
        [TestCase("[12]", new object[] {12}, TestName = "Array with single element having multiple characters")]
        [TestCase("[1,2]", new object[] {1, 2}, TestName = "Array with multiple elements")]
        [TestCase("[ 1 , 2 ]", new object[] {1, 2}, TestName = "Array with multiple elements and internal whitespace")]
        [TestCase("[null]", new object[] {null}, TestName = "Array with single null element")]
        [TestCase("[[1,2],[3,4]]", new object[] {new object[] {1, 2}, new object[] {3, 4}}, TestName = "Array with nested array elements")]
        public void TestTryParseArray_Sucess(string json, object[] expected)
        {
            Assert.That(new JsonDeserializer().TryParseArray(json, out var output), Is.True);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("[1, 2", TestName = "Missing closing bracket")]
        [TestCase("[1, 2}", TestName = "Invalid closing bracket")]
        [TestCase("[1 2]", TestName = "Missing delimiter")]
        [TestCase("[,2]", TestName = "Missing first element")]
        [TestCase("[1,]", TestName = "Missing last element")]
        [TestCase("[1,,3]", TestName = "Missing middle element")]
        public void TestTryParseArray_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseArray(json, out var output), Is.False);
        }

        [Test]
        public void TestTryParseObject_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseObject(null, out var output), Throws.ArgumentNullException);
        }

        private static IEnumerable<TestCaseData> TestTryParseObject_SucessTestCases()
        {
            yield return new TestCaseData("{}",
                                          new Dictionary<string, object>())
               .SetName("Empty object");
            yield return new TestCaseData("  {}",
                                          new Dictionary<string, object>())
               .SetName("Empty object with leading whitespace");
            yield return new TestCaseData("{}  ",
                                          new Dictionary<string, object>())
               .SetName("Empty object with trailing whitespace");
            yield return new TestCaseData("{  }",
                                          new Dictionary<string, object>())
               .SetName("Empty object with internal whitespace");
            yield return new TestCaseData("{\"abc\":123}",
                                          new Dictionary<string, object> {["abc"] = 123})
               .SetName("Object with single key-value pair");
            yield return new TestCaseData("{ \"abc\" : 123 }",
                                          new Dictionary<string, object> {["abc"] = 123})
               .SetName("Object with single key-value pair and internal whitespace");
            yield return new TestCaseData("{\"abc\":null}",
                                          new Dictionary<string, object> {["abc"] = null})
               .SetName("Object with single key-value pair and null value");
            yield return new TestCaseData("{\"abc\":123,\"def\":4.5}",
                                          new Dictionary<string, object> {["abc"] = 123, ["def"] = 4.5})
               .SetName("Object with multiple key-value pairs");
            yield return new TestCaseData("{ \"abc\" : 123 , \"def\" : 4.5 }",
                                          new Dictionary<string, object> {["abc"] = 123, ["def"] = 4.5})
               .SetName("Object with multiple key-value pairs and internal whitespace");
            yield return new TestCaseData("{\"ab\":{\"pq\":12},\"cd\":{\"rs\":34}}",
                                          new Dictionary<string, object>
                                          {
                                              ["ab"] = new Dictionary<string, object> {["pq"] = 12},
                                              ["cd"] = new Dictionary<string, object> {["rs"] = 34}
                                          })
               .SetName("Nested objects");
        }

        [Test, TestCaseSource(nameof(TestTryParseObject_SucessTestCases))]
        public void TestTryParseObject_Sucess(string json, IDictionary<string, object> expected)
        {
            Assert.That(new JsonDeserializer().TryParseObject(json, out var output), Is.True);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("{", TestName = "Missing closing brace")]
        [TestCase("{]", TestName = "Invalid closing brace")]
        [TestCase("{\"abc\"123}", TestName = "Missing key-value separator")]
        [TestCase("{\"abc\"::123}", TestName = "Duplicate key-value separator")]
        [TestCase("{\"abc\",123}", TestName = "Invalid key-value separator")]
        [TestCase("{\"abc\":123 \"def\":4.5}", TestName = "Missing key-value delimiter")]
        [TestCase("{,\"abc\":123,\"def\":4.5}", TestName = "Missing first key-value pair")]
        [TestCase("{\"abc\":123,\"def\":4.5,}", TestName = "Missing first key-value pair")]
        [TestCase("{\"abc\":123,,\"def\":4.5}", TestName = "Missing middle key-value pair")]
        [TestCase("{\"abc\":123,\"ABC\":4.5}", TestName = "Duplicate keys")]
        [TestCase("{null:123}", TestName = "Null key")]
        public void TestTryParseObject_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseObject(json, out var output), Is.False);
        }

        [Test]
        public void TestTryParseValue_NullArgCheck()
        {
            Assert.That(() => new JsonDeserializer().TryParseValue(null, out var output), Throws.ArgumentNullException);
        }

        private static IEnumerable<TestCaseData> TestTryParseValue_SucessTestCases()
        {
            yield return new TestCaseData("true", true).SetName("True");
            yield return new TestCaseData("false", false).SetName("False");
            yield return new TestCaseData("null", null).SetName("Null");
            yield return new TestCaseData("1234.5", 1234.5).SetName("Number");
            yield return new TestCaseData("\"abc\"", "abc").SetName("String");
            yield return new TestCaseData("[12,34]", new object[] {12, 34}).SetName("Array");
            yield return new TestCaseData("[null]", new object[] {null}).SetName("Array null");
            yield return new TestCaseData("{\"abc\":null}", new Dictionary<string, object>{["abc"] = null}).SetName("Null value");
            yield return new TestCaseData("{\"abc\":123}", new Dictionary<string, object>{["abc"] = 123}).SetName("Object");
        }

        [Test, TestCaseSource(nameof(TestTryParseValue_SucessTestCases))]
        public void TestTryParseValue_Sucess(string json, object expected)
        {
            Assert.That(new JsonDeserializer().TryParseValue(json, out var output), Is.True);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("moomin", TestName = "Unrecognized literal")]
        [TestCase("troo", TestName = "Unrecognized true literal")]
        [TestCase("faltz", TestName = "Unrecognized false literal")]
        [TestCase("nuul", TestName = "Unrecognized null literal")]
        public void TestTryParseValue_Failed(string json)
        {
            Assert.That(new JsonDeserializer().TryParseValue(json, out var output), Is.False);
        }
    }
}
