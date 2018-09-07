using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Windef
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        internal int cx;
        internal int cy;

        internal SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }
}