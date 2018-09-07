using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
using ULibs.Win32.Wingdi;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32.WindowChrome
{
    /// <summary>
    /// This class represents one of the 4 windows around the outside of the parent window 
    /// that make up the glow around the outside. The window creation is done by hand so we
    /// can avoid taking a dependency on winform or wpf. This is because this library is used
    /// by both of them so I would like to avoid having to force the user of the frame work to
    /// reference the other one.
    /// </summary>
    internal class GlowWindow
    {
        private delegate IntPtr WndProcDelegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        private const string c_WindowClassName = "RedgateGlowWindow";
        private static IntPtr s_WindowClassAtom;

        private static WndProcDelegate s_GlowWindowDelegate;

        private static IntPtr s_CursorSizeNeSw;
        private static IntPtr s_CursorSizeNs;
        private static IntPtr s_CursorSizeNwSe;
        private static IntPtr s_CursorSizeWe;

        private static void CreateWindowClassIfNeeded()
        {
            // load the cursors usd to indicate that you can resize the window
            if(s_CursorSizeNeSw == IntPtr.Zero)
                s_CursorSizeNeSw = User32.LoadCursor(IntPtr.Zero, new IntPtr((int)OCR.SIZENESW));
            if (s_CursorSizeNs == IntPtr.Zero)
                s_CursorSizeNs = User32.LoadCursor(IntPtr.Zero, new IntPtr((int)OCR.SIZENS));
            if (s_CursorSizeNwSe == IntPtr.Zero)
                s_CursorSizeNwSe = User32.LoadCursor(IntPtr.Zero, new IntPtr((int)OCR.SIZENWSE));
            if (s_CursorSizeWe == IntPtr.Zero)
                s_CursorSizeWe = User32.LoadCursor(IntPtr.Zero, new IntPtr((int)OCR.SIZEWE));

            // If we haven't registered the window class or the registration had previously failed
            // this will be zero
            if (s_WindowClassAtom == IntPtr.Zero)
            {
                // You must keep this delegate around so it will not be GCed. If it does get GCed 
                // then you application will die the next time the callback happens
                s_GlowWindowDelegate = WndProc;

                var windowClass = new WNDCLASSEX
                {
                    cbSize = Marshal.SizeOf(typeof (WNDCLASSEX)),
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate(s_GlowWindowDelegate),
                    lpszClassName = c_WindowClassName
                };
                s_WindowClassAtom = User32.RegisterClassEx(ref windowClass);
            }
        }

        /// <summary>
        /// We use the same wndproc for all the windows rather subclassing it in each instance
        /// as is make the code slightly easier. If we don't use a static wndproc we have to be
        /// a lot more careful about the GC removing the wndproc delegate before we have finished 
        /// having callbacks on it.
        /// </summary>
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            // The parent windows is form we are being the drop shadow for
            var parentHwnd = User32.GetParent(hwnd);
            if (parentHwnd != IntPtr.Zero)
            {
                // We can work out if the parent is resizable from the parent window style
                var style = (WS) User32.GetWindowLongPtr(parentHwnd, GWL.STYLE).ToInt32();
                var isResizable = (style & WS.THICKFRAME) == WS.THICKFRAME;

                switch (msg)
                {
                    case (int) WM.MOUSEACTIVATE:
                        // This stop the glow window from getting focus when it is clicked on
                        return new IntPtr((int) MA.NOACTIVATE);
                    case (int) WM.LBUTTONDOWN:
                        if (isResizable)
                        {
                            // To get the resize to happen we send a WM_NCLBUTTONDOWN to the parent window
                            // This tells the parent that we have a mouse down event in the non client area
                            // and HitTestPoint says what to do with the mouse event which is going to resize 
                            // in the correct direction
                            var pt = new POINT((int) lParam & 0xFFFF, ((int) lParam >> 16) & 0xFFFF);
                            User32.PostMessage(parentHwnd, WM.NCLBUTTONDOWN,
                                new IntPtr((int) HitTestPoint(hwnd, parentHwnd, pt)), IntPtr.Zero);
                        }
                        break;
                    case (int) WM.MOUSEMOVE:
                        if(isResizable)
                        {
                            // This sets the mouse cursor to indicate that you can resize the window
                            var pt = new POINT((int) lParam & 0xFFFF, ((int) lParam >> 16) & 0xFFFF);
                            var hitTest = HitTestPoint(hwnd, parentHwnd, pt);
                            var cursor = IntPtr.Zero;
                            switch (hitTest)
                            {
                                case HitTest.HTTOPLEFT:
                                case HitTest.HTBOTTOMRIGHT:
                                    cursor = s_CursorSizeNwSe;
                                    break;
                                case HitTest.HTTOPRIGHT:
                                case HitTest.HTBOTTOMLEFT:
                                    cursor = s_CursorSizeNeSw;
                                    break;
                                case HitTest.HTLEFT:
                                case HitTest.HTRIGHT:
                                    cursor = s_CursorSizeWe;
                                    break;
                                case HitTest.HTTOP:
                                case HitTest.HTBOTTOM:
                                    cursor = s_CursorSizeNs;
                                    break;
                            }

                            if (cursor != IntPtr.Zero)
                            {
                                var currectCursor = User32.GetCursor();
                                if (currectCursor != cursor)
                                {
                                    User32.SetCursor(cursor);
                                }
                            }
                            
                        }
                        break;
                }
            }

            return User32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        /// <summary>
        /// Given a mouse point this function indicates if we are somewhere that lets the window be resized.
        /// HitTest.HTCLIENT just means no in this instance
        /// </summary>
        private static HitTest HitTestPoint(IntPtr hwnd, IntPtr parentHwnd, POINT point)
        {
            RECT clientRect;
            if (!User32.GetWindowRect(parentHwnd, out clientRect))
            {
                return HitTest.HTCLIENT;
            }
            if (!User32.ClientToScreen(hwnd, ref point))
            {
                return HitTest.HTCLIENT;
            }

            if (point.X <= clientRect.Left && point.Y <= clientRect.Top)
                return HitTest.HTTOPLEFT;
            if (point.X >= clientRect.Right && point.Y <= clientRect.Top)
                return HitTest.HTTOPRIGHT;
            if (point.X <= clientRect.Left && point.Y >= clientRect.Bottom)
                return HitTest.HTBOTTOMLEFT;
            if (point.X >= clientRect.Right && point.Y >= clientRect.Bottom)
                return HitTest.HTBOTTOMRIGHT;
            if (point.X <= clientRect.Left)
                return HitTest.HTLEFT;
            if (point.X >= clientRect.Right)
                return HitTest.HTRIGHT;
            if (point.Y <= clientRect.Top)
                return HitTest.HTTOP;
            if (point.Y >= clientRect.Bottom)
                return HitTest.HTBOTTOM;

            return HitTest.HTCLIENT;
        }

        /// <summary>
        ///  Create the a new glow window
        /// </summary>
        internal static GlowWindow Create(IntPtr parentHandle, GlowWindowType glowWindowType)
        {
            // Make sure that we have created a window class for the window
            CreateWindowClassIfNeeded();

            // I copied these styles from the glow windows around Visual Studio
            // The most imporant on is WS_EX.LAYERED which lets us use transparency you can read all about them here
            // https://msdn.microsoft.com/en-GB/library/ms997507.aspx
            var styleEx = WS_EX.LEFT | WS_EX.LTRREADING | WS_EX.RIGHTSCROLLBAR | WS_EX.TOOLWINDOW | WS_EX.LAYERED;
            var style = WS.POPUP | WS.CLIPSIBLINGS | WS.CLIPCHILDREN;

            var hwnd = User32.CreateWindowEx(styleEx, c_WindowClassName, "", style, 0, 0, 100, 100,
                parentHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if (hwnd != IntPtr.Zero)
            {
                return new GlowWindow(hwnd, glowWindowType);
            }

            return null;
        }

        private readonly IntPtr m_Hwnd;
        private readonly GlowWindowType m_GlowWindowType;

        internal bool IsVisible => User32.IsWindowVisible(m_Hwnd);

        internal GlowWindow(IntPtr hwnd, GlowWindowType glowWindowType)
        {
            m_Hwnd = hwnd;
            m_GlowWindowType = glowWindowType;
        }

        internal void Show()
        {
            if (!IsVisible)
            {
                User32.ShowWindow(m_Hwnd, SW.SHOWNA);
            }
        }

        internal void Hide()
        {
            if (IsVisible)
            {
                User32.ShowWindow(m_Hwnd, SW.HIDE);
            }
        }

        internal void UpdateWindow(IBorderStyle borderStyle, RECT rect)
        {
            int width = rect.Width, height = rect.Height;

            var parent = User32.GetParent(m_Hwnd);

            // Move and size the window
            User32.SetWindowPos(m_Hwnd, parent, rect.Left, rect.Top, rect.Width, rect.Height,
                    SWP.NOACTIVATE | SWP.SHOWWINDOW);

            var hdcScreen = IntPtr.Zero;
            var hdcBackBuffer = IntPtr.Zero;
            var hbmBackBuffer = IntPtr.Zero;
            var hbmOld = IntPtr.Zero;
            try
            {
                // Drawing layered windows is very magic. It doesn't work like WM_PAINT. 
                // What we do is draw the window to a bitmap and then say you look like this now.
                // This is what we do now

                // Here we create a new bitmap that stored what will be displayed in the window
                hdcScreen = User32.GetDC(IntPtr.Zero);
                if(hdcScreen == IntPtr.Zero)
                    return;
                hdcBackBuffer = Gdi32.CreateCompatibleDC(hdcScreen);
                if (hdcBackBuffer == IntPtr.Zero)
                    return;
                hbmBackBuffer = Gdi32.CreateCompatibleBitmap(hdcScreen, width, height);
                if (hbmBackBuffer == IntPtr.Zero)
                    return;
                hbmOld = Gdi32.SelectObject(hdcBackBuffer, hbmBackBuffer);

                // Draw the border into the back buffer window
                DrawWindow(borderStyle, hdcBackBuffer, rect);

                // Call UpdateLayeredWindow which tells the window "You look like this now!"
                var ptDst = new POINT(rect.Left, rect.Top);
                var ptSrc = new POINT(0, 0);
                var size = new SIZE(width, height);
                var colorRef = new COLORREF();
                var bf = new BLENDFUNCTION()
                {
                    AlphaFormat = (byte) AC.SRC_ALPHA,
                    SourceConstantAlpha = 255,
                    BlendFlags = 0,
                    BlendOp = (byte) AC.SRC_OVER
                };
                User32.UpdateLayeredWindow(m_Hwnd, hdcScreen, ref ptDst, ref size, hdcBackBuffer, ref ptSrc, colorRef,
                    ref bf, ULW.ALPHA);
            }
            finally
            {
                if (hdcBackBuffer != IntPtr.Zero)
                {
                    Gdi32.SelectObject(hdcBackBuffer, hbmOld);
                }
                if (hbmBackBuffer != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(hbmBackBuffer);
                }
                if (hdcBackBuffer != IntPtr.Zero)
                {
                    Gdi32.DeleteDC(hdcBackBuffer);
                }

                if (hdcScreen != IntPtr.Zero)
                {
                    User32.ReleaseDC(IntPtr.Zero, hdcScreen);
                }
            }
        }

        private void DrawWindow(IBorderStyle borderStyle, IntPtr hdcWindowBuffer, RECT bounds)
        {
            switch (m_GlowWindowType)
            {
                case GlowWindowType.Left:
                    borderStyle.DrawLeft(hdcWindowBuffer, bounds);
                    break;
                case GlowWindowType.Top:
                    borderStyle.DrawTop(hdcWindowBuffer, bounds);
                    break;
                case GlowWindowType.Right:
                    borderStyle.DrawRight(hdcWindowBuffer, bounds);
                    break;
                case GlowWindowType.Bottom:
                    borderStyle.DrawBottom(hdcWindowBuffer, bounds);
                    break;
            }
        }
    }
}