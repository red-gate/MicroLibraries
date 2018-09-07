using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MONITORINFOEX
    {
        internal int cbSize;
        internal RECT rcMonitor;
        internal RECT rcWork;
        internal int dwFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        internal byte[] szDevice;
    }
}