// ReSharper disable InconsistentNaming
using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;

namespace /***$rootnamespace$.***/ULibs.Win32.Shellapi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uCallbackMessage;
        public ABE uEdge;
        public RECT rc;
        public IntPtr lParam;
    }
}