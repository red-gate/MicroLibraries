using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Wingdi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RGBQUAD
    {
        internal byte rgbBlue;
        internal byte rgbGreen;
        internal byte rgbRed;
        internal byte rgbReserved;
    }
}