using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
using ULibs.Win32.Wingdi;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(COLORREF crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern COLORREF SetPixel(IntPtr hdc, int x, int y, COLORREF crColor);

        [DllImport("Msimg32.dll")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, BLENDFUNCTION ftn);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, DIB iUsage, out IntPtr ppvBits, IntPtr hSection, int dwOffset);

        [DllImport("gdi32.dll")]
        public static extern int SetMapMode(IntPtr hdc, MM fnMapMode);

        [DllImport("gdi32.dll")]
        public static extern int SetGraphicsMode(IntPtr hdc, GM iMode);
    }
}
