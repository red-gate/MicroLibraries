using System;
/***using System.Diagnostics.CodeAnalysis;***/
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace /***$rootnamespace$.***/ULibs.FullExceptionString
{
    /// <summary>
    /// Extension methods used derive a full text representation of an <see cref="Exception"/>,
    /// as a more comprehensive alternative to <c>Exception.ToString()</c>
    /// </summary>
    /***[ExcludeFromCodeCoverage]***/
    internal static class Extensions
    {
        /// <summary>
        /// Returns a comprehensive string representation of an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="indent">An optional prefix applied to each line of the string representation of the exception.</param>
        /// <returns>A comprehensive string representation of an exception.</returns>
        /// <remarks>
        /// Each line of the description ends with a newline sequence. If this is undesirable for the final line,
        /// use <c>.TrimEnd()</c> on the returned value.
        /// </remarks>
        public static string ToFullExceptionString(this Exception exception, string indent = "")
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (indent == null) throw new ArgumentNullException(nameof(indent));

            return new StringBuilder().AppendFullExceptionString(exception, indent).ToString();
        }

        /// <summary>
        /// Appends a comprehensive string representation of an exception to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="indent">An optional prefix applied to each line of the string representation of the exception.</param>
        /// <returns>The <paramref name="builder"/>, to enable method chaining.</returns>
        /// <remarks>Each line of the description ends with a newline sequence, including the final line.</remarks>
        public static StringBuilder AppendFullExceptionString(this StringBuilder builder, Exception exception,
                                                              string indent = "")
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (indent == null) throw new ArgumentNullException(nameof(indent));

            PrintRecursiveExceptionDetails(exception, line => builder.AppendLine(line), indent);

            return builder;
        }

        /// <summary>
        /// Writes a comprehensive string representation of an exception to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The text writer.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="indent">An optional prefix applied to each line of the string representation of the exception.</param>
        /// <remarks>Each line of the description ends with a newline sequence, including the final line.</remarks>
        public static void WriteFullExceptionString(this TextWriter writer, Exception exception, string indent = "")
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (indent == null) throw new ArgumentNullException(nameof(indent));

            PrintRecursiveExceptionDetails(exception, writer.WriteLine, indent);
        }

        private static void PrintRecursiveExceptionDetails(Exception exception, Action<string> writeLine, string indent)
        {
            const string indentToken = "  ";

            void WriteLine(object item) => writeLine(indent + item);

            WriteLine($"{exception.GetType()}: {exception.Message}");
            var exceptionStacktrace = exception.StackTrace;
            if (exceptionStacktrace != null)
            {
                foreach (var line in exceptionStacktrace.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None))
                {
                    WriteLine(line);
                }
            }

            switch (exception)
            {
                case AggregateException aggregateException:
                    var innerExceptions = aggregateException.InnerExceptions;
                    if (innerExceptions.Any())
                    {
                        WriteLine(innerExceptions.Count == 1 ? "Contains exception:" : "Contains exceptions:");
                        foreach (var innerException in innerExceptions)
                        {
                            PrintRecursiveExceptionDetails(innerException, writeLine, indent + indentToken);
                        }
                    }
                    break;

                case ReflectionTypeLoadException reflectionTypeLoadException:
                    var loaderExceptions = reflectionTypeLoadException.LoaderExceptions;
                    if (loaderExceptions.Any())
                    {
                        WriteLine(loaderExceptions.Length == 1 ? "Loader exception:" : "Loader exceptions:");
                        foreach (var innerException in loaderExceptions)
                        {
                            PrintRecursiveExceptionDetails(innerException, writeLine, indent + indentToken);
                        }
                    }
                    break;
            }

            if (exception.InnerException != null)
            {
                WriteLine("Caused by:");
                PrintRecursiveExceptionDetails(exception.InnerException, writeLine, indent + indentToken);
            }
        }
    }
}