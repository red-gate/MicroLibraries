using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CREATESTRUCT
    {
        internal IntPtr lpCreateParams;
        internal IntPtr hInstance;
        internal IntPtr hMenu;
        internal IntPtr hwndParent;
        internal int cy;
        internal int cx;
        internal int y;
        internal int x;
        internal WS style;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpszName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string lpszClass;
        internal WS_EX dwExStyle;
    }
}