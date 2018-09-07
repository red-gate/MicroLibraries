using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Wingdi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        internal int biSize;
        internal int biWidth;
        internal int biHeight;
        internal short biPlanes;
        internal short biBitCount;
        internal BI biCompression;
        internal int biSizeImage;
        internal int biXPelsPerMeter;
        internal int biYPelsPerMeter;
        internal int biClrUsed;
        internal int biClrImportant;
    }
}