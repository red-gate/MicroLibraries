using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        internal POINT ptReserved;
        internal POINT ptMaxSize;
        internal POINT ptMaxPosition;
        internal POINT ptMinTrackSize;
        internal POINT ptMaxTrackSize;
    }
}