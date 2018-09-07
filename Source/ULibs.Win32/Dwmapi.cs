using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Uxtheme;

namespace /***$rootnamespace$.***/ULibs.Win32
{
    internal static class Dwmapi
    {
        [DllImport("dwmapi.dll")]
        internal static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmIsCompositionEnabled(out bool enabled);
    }
}
