using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPLACEMENT
    {
        internal int length;
        internal int flags;
        internal SW showCmd;
        internal POINT ptMinPosition;
        internal POINT ptMaxPosition;
        internal RECT rcNormalPosition;
    }
}