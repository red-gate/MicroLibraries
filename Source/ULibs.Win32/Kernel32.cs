using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Fileapi;

namespace /***$rootnamespace$.***/ULibs.Win32
{
    internal static class Kernel32
    {
        [DllImport("Kernel32.dll")]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetFileAttributesEx(string name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.None, SetLastError = true)]
        internal static extern bool SetThreadErrorMode(int newMode, out int oldMode);
    }
}
