using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32.WindowChrome
{
    /// <summary>
    /// This class contains the resize borders for the window. We handle 
    /// resizing by moving 4 windows around the side of the parent window
    /// these windows contain the drop shadow, the border and also let you 
    /// resize the window. This is a bit easier than trying to do this in the 
    /// border of the form as doing it that way would mean making the form 
    /// support transparency and also means keeping controls away from the 
    /// borders as the swallow the windows messages. This is only a problem with
    /// windows formas but we have a lot of windows forms stuff.
    /// </summary>
    internal class WindowResizeBorders : IDisposable
    {
        private readonly IntPtr m_ParentHwnd;
        
        private RECT m_LastBounds;
        private bool m_LastIsActive;
        private bool m_LastIsVisible;

        private GlowWindow m_LeftWindow;
        private GlowWindow m_TopWindow;
        private GlowWindow m_RightWindow;
        private GlowWindow m_BottomWindow;

        private IBorderStyle m_ActiveBorderStyle;
        private IBorderStyle m_InactiveBorderStyle;

        private COLORREF m_ActiveBorderColor;
        private COLORREF m_ActiveGlowColor;
        private byte m_ActiveGlowAlpha;
        private COLORREF m_InactiveBorderColor;
        private COLORREF m_InactiveGlowColor;
        private byte m_InactiveGlowAlpha;
        private int m_GlowWidth;
        private int m_GlowHeight;
        
        public int GlowWidth
        {
            get { return m_GlowWidth; }
            set
            {
                m_GlowWidth = value;
                RegenerateBorderStyle();
            }
        }

        public int GlowHeight
        {
            get { return m_GlowHeight; }
            set
            {
                m_GlowHeight = value;
                RegenerateBorderStyle();
            }
        }

        public COLORREF ActiveBorderColor
        {
            get { return m_ActiveBorderColor; }
            set
            {
                m_ActiveBorderColor = value;
                RegenerateBorderStyle();
            }
        }

        public COLORREF ActiveGlowColor
        {
            get { return m_ActiveGlowColor; }
            set
            {
                m_ActiveGlowColor = value;
                RegenerateBorderStyle();
            }
        }

        public byte ActiveGlowAlpha
        {
            get { return m_ActiveGlowAlpha; }
            set
            {
                m_ActiveGlowAlpha = value;
                RegenerateBorderStyle();
            }
        }

        public COLORREF InactiveBorderColor
        {
            get { return m_InactiveBorderColor; }
            set
            {
                m_InactiveBorderColor = value;
                RegenerateBorderStyle();
            }
        }
        public COLORREF InactiveGlowColor
        {
            get { return m_InactiveGlowColor; }
            set
            {
                m_InactiveGlowColor = value;
                RegenerateBorderStyle();
            }
        }

        public byte InactiveGlowAlpha
        {
            get { return m_InactiveGlowAlpha; }
            set
            {
                m_InactiveGlowAlpha = value;
                RegenerateBorderStyle();
            }
        }

        private WindowResizeBorders(IntPtr hwnd)
        {
            m_ParentHwnd = hwnd;
            m_GlowWidth = 6;
            m_GlowHeight = 6;

            ActiveGlowColor = new COLORREF
            {
                R = 255,
                G = 56,
                B = 228
            };
            ActiveBorderColor = new COLORREF
            {
                R = 0x5E,
                G = 0x7F,
                B = 0xDA
            };
            ActiveGlowAlpha = 100;

            InactiveGlowColor = new COLORREF
            {
                R = 0,
                G = 255,
                B = 255
            };
            InactiveBorderColor = new COLORREF
            {
                R = 58,
                G = 58,
                B = 50
            };
            InactiveGlowAlpha = 50;
        }

        public static WindowResizeBorders Start(IntPtr hwnd)
        {
            var wcw = new WindowResizeBorders(hwnd);

            wcw.CreateResizeWindows();
            wcw.RegenerateBorderStyle();

            return wcw;
        }

        internal void OnParentStateUpdating(RECT parentBounds, bool isActive, bool isVisible)
        {
            var windowPlacement = new WINDOWPLACEMENT
            {
                length = Marshal.SizeOf(typeof(WINDOWPLACEMENT))
            };

            // If the window is visible and is in normal mode (not maximized or minimized)
            // then we show the resize borders ortherwise we hide them
            if (isVisible 
                && User32.GetWindowPlacement(m_ParentHwnd, ref windowPlacement)
                && windowPlacement.showCmd == SW.NORMAL)
            {
                // We only need to update the borders if the visibility has changed, the size has changed or
                // the window has changed from being active to inactive or vice versa
                if (parentBounds != m_LastBounds || isActive != m_LastIsActive || isVisible != m_LastIsVisible)
                {
                    m_LastBounds = parentBounds;
                    m_LastIsActive = isActive;
                    m_LastIsVisible = isVisible;

                    // The border style defines how the windows get drawn
                    var borderStyle = isActive ? m_ActiveBorderStyle : m_InactiveBorderStyle;

                    // Move the windows and redraw them
                    m_LeftWindow?.UpdateWindow(borderStyle, new RECT(
                        parentBounds.Left - m_GlowWidth,
                        parentBounds.Top,
                        parentBounds.Left,
                        parentBounds.Bottom
                        ));

                    m_TopWindow?.UpdateWindow(borderStyle, new RECT(
                        parentBounds.Left - m_GlowWidth,
                        parentBounds.Top - m_GlowHeight,
                        parentBounds.Right + m_GlowWidth,
                        parentBounds.Top
                        ));

                    m_RightWindow?.UpdateWindow(borderStyle, new RECT(
                        parentBounds.Right,
                        parentBounds.Top,
                        parentBounds.Right + m_GlowWidth,
                        parentBounds.Bottom
                        ));

                    m_BottomWindow?.UpdateWindow(borderStyle, new RECT(
                        parentBounds.Left - m_GlowWidth,
                        parentBounds.Bottom,
                        parentBounds.Right + m_GlowWidth,
                        parentBounds.Bottom + m_GlowHeight
                        ));
                }
            }
            else
            {
                m_LastBounds = new RECT();
                m_LastIsActive = false;
                m_LastIsVisible = false;

                m_LeftWindow?.Hide();
                m_TopWindow?.Hide();
                m_RightWindow?.Hide();
                m_BottomWindow?.Hide();
            }
        }

        private void CreateResizeWindows()
        {
            m_LeftWindow = GlowWindow.Create(m_ParentHwnd, GlowWindowType.Left);
            m_TopWindow = GlowWindow.Create(m_ParentHwnd, GlowWindowType.Top);
            m_RightWindow = GlowWindow.Create(m_ParentHwnd, GlowWindowType.Right);
            m_BottomWindow = GlowWindow.Create(m_ParentHwnd, GlowWindowType.Bottom);
        }

        private void RegenerateBorderStyle()
        {
            m_ActiveBorderStyle?.Dispose();
            m_ActiveBorderStyle = GradientBorderStyle.Create(m_GlowWidth, m_GlowHeight, ActiveGlowAlpha, ActiveGlowColor, ActiveBorderColor);
            m_InactiveBorderStyle?.Dispose();
            m_InactiveBorderStyle = GradientBorderStyle.Create(m_GlowWidth, m_GlowHeight, InactiveGlowAlpha, InactiveGlowColor, InactiveBorderColor);
        }

        public void Dispose()
        {
            m_ActiveBorderStyle?.Dispose();
            m_InactiveBorderStyle?.Dispose();
        }
    }
}