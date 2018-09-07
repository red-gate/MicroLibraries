using System;
using ULibs.Win32.Windef;
using ULibs.Win32.Wingdi;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32.WindowChrome
{
    /// <summary>
    /// This class controls how a border looks as the border drawing is delegated to this class. In this class
    /// We store all the different types of gradients we need to draw the border in bitmaps and then we
    /// </summary>
    internal class GradientBorderStyle : IBorderStyle
    {
        // This is how much data we store for the sides
        // If this is too short for the side we just repate the 
        // image again.
        private const int c_StripLength = 250;

        private GradientBitmap m_Left;
        private GradientBitmap m_TopLeft;
        private GradientBitmap m_Top;
        private GradientBitmap m_TopRight;
        private GradientBitmap m_Right;
        private GradientBitmap m_BottomLeft;
        private GradientBitmap m_Bottom;
        private GradientBitmap m_BottomRight;

        internal int BorderWidth { get; private set; }
        internal int BorderHeight { get; private set; }

        internal static GradientBorderStyle Create(int width, int height, byte glowAlpha, COLORREF glowColor, COLORREF borderColor)
        {
            var gbs = new GradientBorderStyle
            {
                BorderWidth = width,
                BorderHeight = height,


                m_Left = GradientBitmap.Create(GradientBitmapDirection.West, width, c_StripLength, borderColor, glowColor, glowAlpha),
                m_Top = GradientBitmap.Create(GradientBitmapDirection.North, c_StripLength, height, borderColor, glowColor, glowAlpha),
                m_Right = GradientBitmap.Create(GradientBitmapDirection.East, width, c_StripLength, borderColor, glowColor, glowAlpha),
                m_Bottom = GradientBitmap.Create(GradientBitmapDirection.South, c_StripLength, height, borderColor, glowColor, glowAlpha),

                m_TopLeft = GradientBitmap.Create(GradientBitmapDirection.NorthWest, width, height, borderColor, glowColor, glowAlpha),
                m_TopRight = GradientBitmap.Create(GradientBitmapDirection.NorthEast, width, height, borderColor, glowColor, glowAlpha),
                m_BottomLeft = GradientBitmap.Create(GradientBitmapDirection.SouthWest, width, height, borderColor, glowColor, glowAlpha),
                m_BottomRight = GradientBitmap.Create(GradientBitmapDirection.SouthEast, width, height, borderColor, glowColor, glowAlpha)
            };
            
            return gbs;
        }

        // In all the following function Gdi32.AlphaBlend works like BitBlat but it supports alpha transparency

        public void DrawTop(IntPtr hdcWindowBuffer, RECT bounds)
        {
            var bf = new BLENDFUNCTION()
            {
                AlphaFormat = (byte)AC.SRC_ALPHA,
                SourceConstantAlpha = 255,
                BlendFlags = 0,
                BlendOp = (byte)AC.SRC_OVER
            };

            Gdi32.AlphaBlend(hdcWindowBuffer,
                            0, 0,
                            m_TopLeft.Width,
                            m_TopLeft.Height,
                            m_TopLeft.Handle,
                            0, 0,
                            m_TopLeft.Width,
                            m_TopLeft.Height,
                            bf);

            var x = m_TopLeft.Width;
            while (x < bounds.Width - m_TopRight.Width)
            {
                var img = m_Top;

                var width = Math.Min(img.Width, bounds.Width - x - m_TopRight.Width);
                var height = img.Height;

                Gdi32.AlphaBlend(hdcWindowBuffer, x, 0, width, height, img.Handle, 0, 0, width, height, bf);

                x += width;
            }

            Gdi32.AlphaBlend(hdcWindowBuffer,
                bounds.Width - m_TopRight.Width, 0,
                m_TopRight.Width,
                m_TopRight.Height,
                m_TopRight.Handle,
                0, 0,
                m_TopRight.Width,
                m_TopRight.Height,
                bf);
        }

        public void DrawBottom(IntPtr hdcWindowBuffer, RECT bounds)
        {
            var bf = new BLENDFUNCTION()
            {
                AlphaFormat = (byte)AC.SRC_ALPHA,
                SourceConstantAlpha = 255,
                BlendFlags = 0,
                BlendOp = (byte)AC.SRC_OVER
            };

            Gdi32.AlphaBlend(hdcWindowBuffer,
                            0, 0,
                            m_BottomLeft.Width,
                            m_BottomLeft.Height,
                            m_BottomLeft.Handle,
                            0, 0,
                            m_BottomLeft.Width,
                            m_BottomLeft.Height,
                            bf);

            var x = m_BottomLeft.Width;
            while (x < bounds.Width - m_BottomRight.Width)
            {
                var img = m_Bottom;

                var width = Math.Min(img.Width, bounds.Width - x - m_BottomRight.Width);
                var height = img.Height;

                Gdi32.AlphaBlend(hdcWindowBuffer, x, 0, width, height, img.Handle, 0, 0, width, height, bf);

                x += width;
            }

            Gdi32.AlphaBlend(hdcWindowBuffer,
                bounds.Width - m_BottomRight.Width, 0,
                m_BottomRight.Width,
                m_BottomRight.Height,
                m_BottomRight.Handle,
                0, 0,
                m_BottomRight.Width,
                m_BottomRight.Height,
                bf);
        }

        public void DrawLeft(IntPtr hdcWindowBuffer, RECT bounds)
        {
            var bf = new BLENDFUNCTION()
            {
                AlphaFormat = (byte)AC.SRC_ALPHA,
                SourceConstantAlpha = 255,
                BlendFlags = 0,
                BlendOp = (byte)AC.SRC_OVER
            };

            var y = 0;
            while (y < bounds.Height)
            {
                var img = m_Left;

                var width = img.Width;
                var height = Math.Min(img.Height, bounds.Height - y);

                Gdi32.AlphaBlend(hdcWindowBuffer, 0, y, width, height, img.Handle, 0, 0, width, height, bf);

                y += height;
            }
        }

        public void DrawRight(IntPtr hdcWindowBuffer, RECT bounds)
        {            
            var bf = new BLENDFUNCTION()
            {
                AlphaFormat = (byte)AC.SRC_ALPHA,
                SourceConstantAlpha = 255,
                BlendFlags = 0,
                BlendOp = (byte)AC.SRC_OVER
            };

            var y = 0;
            while (y < bounds.Height)
            {
                var img = m_Right;

                var height = Math.Min(img.Height, bounds.Height - y);
                var width = img.Width;

                Gdi32.AlphaBlend(hdcWindowBuffer, 0, y, width, height, img.Handle, 0, 0, width, height, bf);

                y += height;
            }
        }

        public void Dispose()
        {
            m_Left.Dispose();
            m_TopLeft.Dispose();
            m_Top.Dispose();
            m_TopRight.Dispose();
            m_Right.Dispose();
            m_BottomLeft.Dispose();
            m_Bottom.Dispose();
            m_BottomRight.Dispose();
        }
    }
}