using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Wingdi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFO
    {
        internal BITMAPINFOHEADER bmiHeader;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
        internal RGBQUAD[] bmiColors;
    }
}