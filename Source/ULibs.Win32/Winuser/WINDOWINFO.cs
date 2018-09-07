using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWINFO
    {
        internal int cbSize;
        internal RECT rcWindow;
        internal RECT rcClient;
        internal int dwStyle;
        internal int dwExStyle;
        internal uint dwWindowStatus;
        internal uint cxWindowBorders;
        internal uint cyWindowBorders;
        internal ushort atomWindowType;
        internal ushort wCreatorVersion;
    }
}