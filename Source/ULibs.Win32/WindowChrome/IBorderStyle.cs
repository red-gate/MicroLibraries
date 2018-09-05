using System;
using ULibs.Win32.Windef;

namespace /***$rootnamespace$.***/ULibs.Win32.WindowChrome
{
    internal interface IBorderStyle : IDisposable
    {
        void DrawBottom(IntPtr hdcWindowBuffer, RECT bounds);
        void DrawLeft(IntPtr hdcWindowBuffer, RECT bounds);
        void DrawTop(IntPtr hdcWindowBuffer, RECT bounds);
        void DrawRight(IntPtr hdcWindowBuffer, RECT bounds);
    }
}