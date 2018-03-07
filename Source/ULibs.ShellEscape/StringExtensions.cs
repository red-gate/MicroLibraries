using System;
/***using System.Diagnostics.CodeAnalysis;***/
using System.Text.RegularExpressions;

namespace /***$rootnamespace$.***/ULibs.ShellEscape
{
    /// <summary>
    /// Extensions to the <see cref="String"/> class.
    /// </summary>
    /***[ExcludeFromCodeCoverage]***/
    internal static class StringExtensions
    {
        private static readonly Regex QuotesWithPossibleLeadingBackslashes = new Regex(@"\\*\""");
        private static readonly Regex TrailingBackslashes = new Regex(@"\\+$");
        private static readonly Regex NeedsSurroundingQuotes = new Regex(@"(^$)|\s|\""");

        /// <summary>
        /// Escapes a string for use in the Windows shell, according to the escaping rules
        /// described at http://msdn.microsoft.com/en-us/library/a1y7w461.aspx. This involves
        /// surrounding the string in double-quotes if necessary, along with further escaping of
        /// any double-quotes and backslashes within the string.
        /// </summary>
        /// <param name="value">The string to escape.</param>
        /// <returns>A shell-escaped representation of the string.</returns>
        public static string ShellEscape(this string value)
        {
            // See http://msdn.microsoft.com/en-us/library/a1y7w461.aspx for details on the
            // necessary escaping.

            // If the string contains double-quotes, with possible leading backslash characters, we
            // need to double up the back-slash characters, and then escape the double-quotes with
            // a further leading backslash.
            var escaped = QuotesWithPossibleLeadingBackslashes.Replace(
                value ?? throw new ArgumentNullException(nameof(value)),
                match => new string('\\', 2 * match.Value.Length - 1) + "\"");

            // If the string ends with one or more trailing backslashes, we need to double them up.
            escaped = TrailingBackslashes.Replace(
                escaped,
                match => new string('\\', match.Value.Length * 2));

            // And finally, surround with quotes if necessary, because:
            // 1. The string is empty.
            // 2. Some escaping actually happened in the above replacement calls.
            // 3. The string contains some whitespace.
            if (escaped.Length != value.Length || NeedsSurroundingQuotes.IsMatch(value))
            {
                return "\"" + escaped + "\"";
            }

            return escaped;
        }
    }
}