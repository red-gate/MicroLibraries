using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Uxtheme
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int LeftWidth;
        public int RightWidth;
        public int TopHeight;
        public int BottomHeight;

        public MARGINS(int border) : this()
        {
            LeftWidth = border;
            RightWidth = border;
            TopHeight = border;
            BottomHeight = border;
        }
    }
}