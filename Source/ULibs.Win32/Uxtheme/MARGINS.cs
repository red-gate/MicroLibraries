using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Uxtheme
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MARGINS
    {
        internal int LeftWidth;
        internal int RightWidth;
        internal int TopHeight;
        internal int BottomHeight;

        internal MARGINS(int border) : this()
        {
            LeftWidth = border;
            RightWidth = border;
            TopHeight = border;
            BottomHeight = border;
        }
    }
}