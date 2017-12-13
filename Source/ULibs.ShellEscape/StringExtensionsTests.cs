using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace ULibs.ShellEscape
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private static IEnumerable<TestCaseData> CheckShellEscape_TestData()
        {
            Func<string, string, TestCaseData> create = (value, name) => new TestCaseData(value).SetName(name);

            yield return create("", "Empty string");

            yield return create(" ", "Space");
            yield return create(" abc", "Space prefixing a word");
            yield return create("abc ", "Space following a word");
            yield return create("ab cd", "Space between two words");

            yield return create("\t", "Tab");
            yield return create("\tabc", "Tab prefixing a word");
            yield return create("abc\t", "Tab following a word");
            yield return create("ab\tcd", "Tab between two words");

            yield return create("ab\t ", "Two adjacent whitespace (tab and space)");

            yield return create(@"a b c", @"a b c");
            yield return create(@"ab""c", @"ab""c");

            yield return create(@"a\b", @"a\b");
            yield return create(@"a\\b", @"a\\b");
            yield return create(@"a\\\b", @"a\\\b");

            yield return create(@"a\", @"a\");
            yield return create(@"a\\", @"a\\");
            yield return create(@"a\\\", @"a\\\");

            yield return create(@"a b\", @"a b\");
            yield return create(@"a b\\", @"a b\\");
            yield return create(@"a b\\\", @"a b\\\");

            yield return create(@"a""b", @"a""b");
            yield return create(@"a\""b", @"a\""b");
            yield return create(@"a\\""b", @"a\\""b");
            yield return create(@"a\\\""b", @"a\\\""b");

            yield return create(@"a""", @"a""");
            yield return create(@"a\""", @"a\""");
            yield return create(@"a\\""", @"a\\""");
            yield return create(@"a\\\""", @"a\\\""");

            yield return create(@"a\\b c", @"a\\b c");

            yield return create("\"", "Double-quotes");
            yield return create("\"\"", "Pair of double-quotes");
            yield return create("\"abc", "Double-quotes prefixing a word");
            yield return create("abc\"", "Double-quotes following a word");
            yield return create("ab\"cd", "Double-quotes between two words");
            yield return create("ab\"\"cd", "Pair of double-quotes between two words");

            yield return create("\'", "Single-quotes");
            yield return create("\'\'", "Pair of single-quotes");
            yield return create("\'abc", "Single-quotes prefixing a word");
            yield return create("abc\'", "Single-quotes following a word");
            yield return create("ab\'cd", "Single-quotes between two words");
            yield return create("ab\'\'cd", "Pair of single-quotes between two words");

            yield return create("\"\'\"'", "A mixture of double and single-quotes");

            yield return create(@"abc\", "Trailing backslash");
            yield return create(@"""abc\""", "Quoted string with a trailing backslash");
            yield return create(@"ab\""cd", "Backslash followed by a quote in the middle of the string");
            yield return create(@"c:\temp", "Folder path");
            yield return create(@"c:\temp\", "Folder path with trailing backslash");
            yield return create(@"c:\another temp", "Folder path with a space in it");
            yield return create(@"c:\another temp\", "Folder path with a space in it and a trailing backslash");

            yield return create(new string(Enumerable.Range(1, 127).Select(i => (char)i).ToArray()), "Chars from 1 to 127");

            yield return create(@"£$%^&<>{}=+-~#`", "Common chars");
            yield return create(@"ùûüÿ€ »« œôîïëêèéçæâà", "French");
            yield return create(@"äüß€„“–—ö", "German");
            yield return create(@"あぁかさりぎげゑめねけとのんえう", "Japanese");
            yield return create(@"Μισό πρωτάθλημα  μισό πρωτάθλημα", "Arabic");
            yield return create(@"前半戦、後半戦、 ", "Chinese");
            yield return create(@"ασδξθηβωψζ¥©ξλ", "Greek");
            yield return create(@"ए कालिया, कितने आदमी थे?", "Hindi");
        }

        [Test, TestCaseSource(nameof(CheckShellEscape_TestData))]
        public void CheckShellEscape(string value)
        {
            // ACT
            var escapedArguments = value.ShellEscape();

            // ASSERT
            string[] args = ToMainMethodArgsArray(escapedArguments);
            Assert.That(args, Is.EqualTo(new[] { value }));
        }

        private static IEnumerable<TestCaseData> CheckShellEscape_WithMultipleArguments_TestData()
        {
            Func<string[], string, TestCaseData> create =
                (value, name) => new TestCaseData((object)value).SetName(name);

            yield return create(new[] { "abc", "def" }, "Two words");

            yield return create(new[] { "ab\"cd", "ef" }, "First word contains double-quotes");
            yield return create(new[] { "ab", "cd\"ef" }, "Second word contains double-quotes");

            yield return create(new[] { "ab\'cd", "ef" }, "First word contains single-quotes");
            yield return create(new[] { "ab", "cd\'ef" }, "Second word contains single-quotes");

            yield return create(new[] { "\"\'", "\'\"" }, "A mixture of double and single-quotes");

            yield return create(new[] { "ab cd", "ef gh" }, "Spaces inside both words");
            yield return create(new[] { " abc", " def" }, "Spaces before both words");
            yield return create(new[] { "abc ", "def " }, "Spaces after both words");
        }

        [Test, TestCaseSource(nameof(CheckShellEscape_WithMultipleArguments_TestData))]
        public void CheckShellEscape_WithMultipleArguments(string[] values)
        {
            // ACT
            var escapedArguments = string.Join(" ", values.Select(x => x.ShellEscape()).ToArray());

            // ASSERT
            string[] args = ToMainMethodArgsArray(escapedArguments);
            Assert.That(args, Is.EqualTo(values));
        }

        /// <summary>
        /// Given an arguments string, such as
        /// <see cref="ProcessStartInfo.Arguments">ProcessStartInfo.Arguments</see>, this method
        /// returns the string array that would be passed to the static Main method entry point of
        /// a .NET process.
        /// </summary>
        /// <param name="processArguments">The full arguments string.</param>
        /// <returns>The corresponding arguments string array that would be seen by the static Main
        /// method entry point of a .NET process.</returns>
        private static string[] ToMainMethodArgsArray(string processArguments)
        {
            int argc;

            // We need to pass in an exe for this imported function to work.
            var argv = CommandLineToArgvW("a.exe " + processArguments, out argc);
            if (argv == IntPtr.Zero)
            {
                throw new Exception($"Failed to convert '{processArguments}' to args array");
            }

            try
            {
                // We don't care about the first argument it returns is a.exe
                var args = new string[argc - 1];
                for (var i = 0; i < args.Length; i++)
                {
                    // Get the (i+1)th returned value
                    // that is the ith argument
                    // changing the IntPtr to a string
                    var p = Marshal.ReadIntPtr(argv, (i + 1) * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        /// <summary>
        /// Writes the arg array of the given string to sequential memory, the
        /// first value being the exe to call.
        /// </summary>
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
    }
}