using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using ULibs.FullExceptionString;

namespace Ulibs.Tests.FullExceptionString
{
    [TestFixture]
    public sealed class ExtensionsTests
    {
        #region Argument check tests

        [Test]
        public void ToFullExceptionString_WithNonNullException_ShouldNotRaiseAnException()
        {
            var exception = new Exception();
            Assert.That(() => exception.ToFullExceptionString(), Throws.Nothing);
        }

        [Test]
        public void ToFullExceptionString_WithNullException_ShouldRaiseAnArgumentNullException()
        {
            var exception = (Exception) null;
            Assert.That(() => exception.ToFullExceptionString(), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void ToFullExceptionString_WithNullIndent_ShouldRaiseAnArgumentNullException()
        {
            var exception = new Exception();
            Assert.That(() => exception.ToFullExceptionString(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendFullExceptionString_WithNonNullArguments_ShouldNotRaiseAnException()
        {
            var builder = new StringBuilder();
            var exception = new Exception();
            Assert.That(() => builder.AppendFullExceptionString(exception), Throws.Nothing);
        }

        [Test]
        public void AppendFullExceptionString_WithNullBuilder_ShouldRaiseAnArgumentNullException()
        {
            var builder = (StringBuilder) null;
            var exception = new Exception();
            Assert.That(() => builder.AppendFullExceptionString(exception), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendFullExceptionString_WithNullException_ShouldRaiseAnArgumentNullException()
        {
            var builder = new StringBuilder();
            var exception = (Exception) null;
            Assert.That(() => builder.AppendFullExceptionString(exception), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendFullExceptionString_WithNullIndent_ShouldRaiseAnArgumentNullException()
        {
            var builder = new StringBuilder();
            var exception = new Exception();
            Assert.That(() => builder.AppendFullExceptionString(exception, null),
                        Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void WriteFullExceptionString_WithNonNullArguments_ShouldNotRaiseAnException()
        {
            var writer = new StringWriter();
            var exception = new Exception();
            Assert.That(() => writer.WriteFullExceptionString(exception), Throws.Nothing);
        }

        [Test]
        public void WriteFullExceptionString_WithNullWriter_ShouldRaiseAnArgumentNullException()
        {
            var writer = (StringWriter) null;
            var exception = new Exception();
            Assert.That(() => writer.WriteFullExceptionString(exception), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void WriteFullExceptionString_WithNullException_ShouldRaiseAnArgumentNullException()
        {
            var writer = new StringWriter();
            var exception = (Exception) null;
            Assert.That(() => writer.WriteFullExceptionString(exception), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void WriteFullExceptionString_WithNullIndent_ShouldRaiseAnArgumentNullException()
        {
            var writer = new StringWriter();
            var exception = new Exception();
            Assert.That(() => writer.WriteFullExceptionString(exception, null),
                        Throws.InstanceOf<ArgumentNullException>());
        }

        #endregion

        private static string NormaliseFormatting(string input) =>
            string.Join(
                Environment.NewLine,
                input.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None)
                     .Where(line => !string.IsNullOrWhiteSpace(line))
                     .Select(line => line.TrimStart()));

        private static T FillInStackTrace<T>(T exception) where T : Exception
        {
            try
            {
                throw exception;
            }
            catch
            {
                return exception;
            }
        }

        [Test]
        public void ToFullExceptionString_WithSimpleException_ShouldMatchExceptionToString()
        {
            var exception = FillInStackTrace(new Exception("message"));

            var fullExceptionString = exception.ToFullExceptionString();

            Assert.That(NormaliseFormatting(fullExceptionString),
                        Is.EqualTo(NormaliseFormatting(exception.ToString())));
        }

        [Test]
        public void AppendFullExceptionString_WithSimpleException_ShouldMatchExceptionToString()
        {
            var exception = FillInStackTrace(new Exception("message"));

            var builder = new StringBuilder();
            builder.AppendFullExceptionString(exception);
            var fullExceptionString = builder.ToString();

            Assert.That(NormaliseFormatting(fullExceptionString),
                        Is.EqualTo(NormaliseFormatting(exception.ToString())));
        }

        [Test]
        public void WriteFullExceptionString_WithSimpleException_ShouldMatchExceptionToString()
        {
            var exception = FillInStackTrace(new Exception("message"));

            var writer = new StringWriter();
            writer.WriteFullExceptionString(exception);
            var fullExceptionString = writer.ToString();
            writer.Dispose();

            Assert.That(NormaliseFormatting(fullExceptionString),
                        Is.EqualTo(NormaliseFormatting(exception.ToString())));
        }

        [Test]
        public void ToFullExceptionString_WithInnerException_ShouldContainInnerExceptionToString()
        {
            var innerException = FillInStackTrace(new Exception("inner-message"));
            var outerException = FillInStackTrace(new Exception("outer-message", innerException));

            var fullExceptionString = outerException.ToFullExceptionString();

            Assert.That(fullExceptionString, Contains.Substring("Caused by:"));
            Assert.That(NormaliseFormatting(fullExceptionString),
                        Contains.Substring(NormaliseFormatting(innerException.ToString())));
        }

        [Test]
        public void ToFullExceptionString_WithAggregatedExceptions_ShouldContainAggregatedExceptionsToString()
        {
            var firstChildException = FillInStackTrace(new Exception("first-child-message"));
            var secondChildException = FillInStackTrace(new Exception("second-child-message"));
            var outerException = FillInStackTrace(new AggregateException(firstChildException, secondChildException));

            var fullExceptionString = outerException.ToFullExceptionString();

            Assert.That(fullExceptionString, Contains.Substring("Contains exceptions:"));
            Assert.That(NormaliseFormatting(fullExceptionString),
                        Contains.Substring(NormaliseFormatting(firstChildException.ToString())));
            Assert.That(NormaliseFormatting(fullExceptionString),
                        Contains.Substring(NormaliseFormatting(secondChildException.ToString())));
        }

        [Test]
        public void ToFullExceptionString_WithReflectionTypeLoadException_AndLoaderExceptions_ShouldContainAggregatedExceptionsToString()
        {
            var firstChildException = FillInStackTrace(new Exception("first-child-message"));
            var secondChildException = FillInStackTrace(new Exception("second-child-message"));
            var outerException = FillInStackTrace(new ReflectionTypeLoadException(new Type[0], new [] {firstChildException, secondChildException}));

            var fullExceptionString = outerException.ToFullExceptionString();

            Assert.That(fullExceptionString, Contains.Substring("Loader exceptions:"));
            Assert.That(NormaliseFormatting(fullExceptionString),
                        Contains.Substring(NormaliseFormatting(firstChildException.ToString())));
            Assert.That(NormaliseFormatting(fullExceptionString),
                        Contains.Substring(NormaliseFormatting(secondChildException.ToString())));
        }
    }
}