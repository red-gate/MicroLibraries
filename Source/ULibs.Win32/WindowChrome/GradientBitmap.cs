using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
using ULibs.Win32.Wingdi;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32.WindowChrome
{
    /// <summary>
    /// To make the drawing of the border quicker we cache the gradient in these bitmaps
    /// and then just copy the gradients in these bitmaps to the borders
    /// </summary>
    internal class GradientBitmap : IDisposable
    {
        private IntPtr m_HBitmap;

        internal IntPtr Handle { get; private set; }
        internal int Width { get; private set; }
        internal int Height { get; private set; }

        private GradientBitmap(IntPtr hwnd, IntPtr hBitmap, int width, int height)
        {
            Handle = hwnd;
            m_HBitmap = hBitmap;
            Width = width;
            Height = height;
        }

        internal static GradientBitmap Create(GradientBitmapDirection direction, int width, int height, COLORREF borderColor, COLORREF glowColor, byte alpha)
        {
            // Create the bitmap to store the data in
            var hdcScreen = IntPtr.Zero;
            var hBitmap = IntPtr.Zero;
            var hdc = IntPtr.Zero;

            try
            {
                hdcScreen = User32.GetDC(IntPtr.Zero);
                if (hdcScreen != IntPtr.Zero)
                {
                    // Create a device context that is compatible with the screen
                    hdc = Gdi32.CreateCompatibleDC(hdcScreen);

                    if (hdc != IntPtr.Zero)
                    {
                        var bmi = new BITMAPINFO
                        {
                            bmiHeader =
                            {
                                biSize = Marshal.SizeOf(typeof (BITMAPINFOHEADER)),
                                biWidth = width,
                                biHeight = height,
                                biPlanes = 1,
                                biBitCount = 32,
                                biCompression = BI.RGB,
                                biSizeImage = width*height*4
                            }
                        };

                        // Create the bitmap. ppvBits points to the image data and you can just write to that pointer 
                        // to update the image
                        IntPtr ppvBits;
                        hBitmap = Gdi32.CreateDIBSection(hdc, ref bmi, DIB.RGB_COLORS, out ppvBits, IntPtr.Zero, 0x0);

                        if (hBitmap != IntPtr.Zero)
                        {
                            // Link the bitmap to device context
                            Gdi32.SelectObject(hdc, hBitmap);

                            if (ppvBits != IntPtr.Zero)
                            {
                                // draw the gradient
                                DrawGradient(direction, width, height, ppvBits, borderColor, glowColor, alpha);
                            }
                        }
                    }
                }
            }
            finally
            {
                // Make sure we release th screen device context
                if (hdcScreen != IntPtr.Zero)
                {
                    User32.ReleaseDC(IntPtr.Zero, hdcScreen);
                }
            }

            return new GradientBitmap(hdc, hBitmap, width, height);
        }

        /// <summary>
        /// We draw the gradients but just manipulating the memory through ppvBits. This means we don't need to 
        /// take any dependency on a drawing library and gdi doesn't seem to support gradients or alpha
        /// transparency
        /// </summary>
        private static void DrawGradient(GradientBitmapDirection direction, int width, int height, IntPtr ppvBits, COLORREF borderColor, COLORREF glowColor, byte alpha)
        {
            // The fill function renders the gradient. This is done by just filling the bitmap 
            // with the same color and then changing the alpha channel. The alpha channel is just
            // calculated by running the delegate on every pixel.

            // The next step is to draw the border ontop of the gradient. This means when we copy
            // into the bitmap you will get the border and the gradient so we don't need to paint 
            // it on afterwards. The border is just draw by setting the individual pixels on
            switch (direction)
            {
                // The following gradients have a border all down one side with a linear gradient
                case GradientBitmapDirection.West:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => x / w * a);
                    for (var y = 0; y < height; y++)
                    {
                        SetPixel(ppvBits, width, width - 1, y, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    }
                    break;
                case GradientBitmapDirection.North:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => (h - y) / h * a);
                    for (var x = 0; x < width; x++)
                    {
                        SetPixel(ppvBits, width, x, 0, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    }
                    break;
                case GradientBitmapDirection.East:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => (w - x) / w * a);
                    for (var y = 0; y < height; y++)
                    {
                        SetPixel(ppvBits, width, 0, y, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    }
                    break;
                case GradientBitmapDirection.South:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => y / h * a);
                    for (var x = 0; x < width; x++)
                    {
                        SetPixel(ppvBits, width, x, height - 1, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    }
                    break;
                    // For the following gradents we just have on pixel of border in the corner 
                    // we need to turn on. They have a 45 diagonal gradient away from the corner
                case GradientBitmapDirection.NorthEast:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) =>  (w - (x + y)) / w * a);
                    SetPixel(ppvBits, width, 0, 0, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    break;
                case GradientBitmapDirection.NorthWest:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => (w - (w - x + y)) / w * a);
                    SetPixel(ppvBits, width, width - 1, 0, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    break;
                case GradientBitmapDirection.SouthWest:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => (w - (w - x + (h - y))) / w * a);
                    SetPixel(ppvBits, width, width - 1, height - 1, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    break;
                case GradientBitmapDirection.SouthEast:
                    Fill(ppvBits, width, height, glowColor.R, glowColor.G, glowColor.B, alpha,
                        (w, h, a, x, y) => (w - (x + (h - y))) / w * a);
                    SetPixel(ppvBits, width, 0, height - 1, 0xff, borderColor.R, borderColor.G, borderColor.B);
                    break;
            }
        }

        private delegate float CalculateAlpha(float width, float height, float alpha, float x, float y);

        private static void Fill(IntPtr ppvBits, int width, int height, int ubRed, int ubGreen, int ubBlue, int a, CalculateAlpha alpha)
        {
            // This function fills the bitmap with one color but calls the alpha delegate on 
            // each pixel to so the caller can specify how the gradient changes
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var calculatedAlpha = alpha(width, height, a, x, y);
                    
                    // If a pixel is total transparent then you don't get mouse events 
                    // for it so we need to make sure that the minimal alpha value is 1
                    if (calculatedAlpha < 1)
                    {
                        calculatedAlpha = 1;
                    }
                    var ubAlpha = (int) calculatedAlpha & 0xff;

                    // The bitmap format means we have to use a premultiplied alpha
                    var color = CalculatePremultipliedAlpha(ubAlpha, ubRed, ubGreen, ubBlue);

                    // Write out the pixel
                    Marshal.WriteInt32(ppvBits, (x + y * width) * 4, color);
                }
            }
        }

        private static void SetPixel(IntPtr ppvBits, int width, int x, int y, int a, int r, int g, int b)
        {
            var color = CalculatePremultipliedAlpha(a, r, g, b);
            Marshal.WriteInt32(ppvBits, (x + y * width) * 4, color);
        }

        private static int CalculatePremultipliedAlpha(int a, int r, int g, int b)
        {
            var alphaFactor = a / (float)0xff;

            int pmA = (byte)a;
            int pmR = (byte)(r * alphaFactor);
            int pmG = (byte)(g * alphaFactor);
            int pmB = (byte)(b * alphaFactor);

            return pmA << 24 | pmR << 16 | pmG << 8 | pmB;
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                Gdi32.DeleteDC(Handle);
                Handle = IntPtr.Zero;
            }

            if (m_HBitmap != IntPtr.Zero)
            {
                Gdi32.DeleteObject(m_HBitmap);
                m_HBitmap = IntPtr.Zero;
            }
        }
    }
}