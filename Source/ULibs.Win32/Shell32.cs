using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Shellapi;

namespace /***$rootnamespace$.***/ULibs.Win32
{
    public static class Shell32
    {
        [DllImport("shell32.dll")]
        public static extern IntPtr SHAppBarMessage(ABM msg, ref APPBARDATA data);
    }
}
