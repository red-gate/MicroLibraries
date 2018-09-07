// ReSharper disable InconsistentNaming
using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;

namespace /***$rootnamespace$.***/ULibs.Win32.Shellapi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct APPBARDATA
    {
        internal int cbSize;
        internal IntPtr hWnd;
        internal int uCallbackMessage;
        internal ABE uEdge;
        internal RECT rc;
        internal IntPtr lParam;
    }
}