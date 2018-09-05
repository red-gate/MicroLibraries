using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Fileapi;

namespace /***$rootnamespace$.***/ULibs.Win32
{
    public static class Kernel32
    {
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetFileAttributesEx(string name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.None, SetLastError = true)]
        public static extern bool SetThreadErrorMode(int newMode, out int oldMode);
    }
}
