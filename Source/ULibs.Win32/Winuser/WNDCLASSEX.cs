using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WNDCLASSEX
    {
        internal int cbSize;
        internal CS style;
        internal IntPtr lpfnWndProc;
        internal int cbClsExtra;
        internal int cbWndExtra;
        internal IntPtr hInstance;
        internal IntPtr hIcon;
        internal IntPtr hCursor;
        internal IntPtr hbrBackground;
        internal string lpszMenuName;
        internal string lpszClassName;
        internal IntPtr hIconSm;
    }
}