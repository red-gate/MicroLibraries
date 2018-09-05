using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ULibs.Win32.Shellapi;
using ULibs.Win32.Windef;
using ULibs.Win32.Wingdi;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32.WindowChrome
{
    /// <summary>
    /// This class helps with being able to custom draw windows. You can set window style to none but
    /// That has some drawback like no longer having the window border to make the window resizable,
    /// it stop window animations like the minimize animation and you can get hold of the system menu 
    /// anymore. This class makes the client area take up the whole window so can custom draw the top 
    /// of your window but it keeps the window animations working and also adds 4 windows around this 
    /// windows which provide a drop shadow and let you do the resizing. It also adds some easy methods
    /// for showing things like the system menu.
    /// 
    /// To use it call WindowChromeWorker.Start to create the class after your window handle has been 
    /// created. You must then call WindowChromeWorker.WndProc with the messages from the message loop 
    /// so it can handle several of the window messages.
    /// </summary>
    public class WindowChromeWorker
    {
        /// <summary>
        /// This creates the WindowChromeWorker and attaches it to the window
        /// </summary>
        /// <param name="hwnd">The handle of the parent window</param>
        /// <returns></returns>
        public static WindowChromeWorker Start(IntPtr hwnd)
        {
            var wcw = new WindowChromeWorker(hwnd);
            wcw.UpdateWindowRegion(hwnd);
            wcw.UpdateWindowButtons();
            RefreshWindowChrome(hwnd);
            wcw.m_WindowResizeBorders = WindowResizeBorders.Start(hwnd);

            return wcw;
        }

        private readonly IntPtr m_Hwnd;
        private WindowResizeBorders m_WindowResizeBorders;
        private bool m_ShowMinimizeBox = true;
        private bool m_ShowMaximizeBox = true;
        private RECT m_LastWindowRegion;
        
        /// <summary>
        /// Show or hide the minimize box in the system menu and 
        /// add / remove them from the window style
        /// </summary>
        public bool ShowMinimizeBox
        {
            get { return m_ShowMinimizeBox; }
            set
            {
                m_ShowMinimizeBox = value;
                UpdateWindowButtons();
            }
        }

        /// <summary>
        /// Show or hide the maximize box in the system menu and 
        /// add / remove them from the window style
        /// </summary>
        public bool ShowMaximizeBox
        {
            get { return m_ShowMaximizeBox; }
            set
            {
                m_ShowMaximizeBox = value;
                UpdateWindowButtons();
            }
        }

        /// <summary>
        /// The width of sides of the glowing border around the window
        /// </summary>
        public int GlowWidth
        {
            get { return m_WindowResizeBorders.GlowWidth; }
            set { m_WindowResizeBorders.GlowWidth = value; }
        }

        /// <summary>
        /// The height of top and bottom of the glowing border around the window
        /// </summary>
        public int GlowHeight
        {
            get { return m_WindowResizeBorders.GlowHeight; }
            set { m_WindowResizeBorders.GlowHeight = value; }
        }

        /// <summary>
        /// The color of the border around the window when the window is the
        /// foreground window 
        /// </summary>
        public COLORREF ActiveBorderColor
        {
            get { return m_WindowResizeBorders.ActiveBorderColor; }
            set { m_WindowResizeBorders.ActiveBorderColor = value; }
        }

        /// <summary>
        /// This controls how dark the glow effect is around the window when
        /// the window is the foreground window
        /// </summary>
        public byte ActiveGlowAlpha
        {
            get { return m_WindowResizeBorders.ActiveGlowAlpha; }
            set { m_WindowResizeBorders.ActiveGlowAlpha = value; }
        }

        /// <summary>
        /// The color of the border around the window when the window is not the
        /// foreground window 
        /// </summary>
        public COLORREF InactiveBorderColor
        {
            get { return m_WindowResizeBorders.InactiveBorderColor; }
            set { m_WindowResizeBorders.InactiveBorderColor = value; }
        }

        /// <summary>
        /// This controls how dark the glow effect is around the window when
        /// the window is not the foreground window
        /// </summary>
        public byte InactiveGlowAlpha
        {
            get { return m_WindowResizeBorders.InactiveGlowAlpha; }
            set { m_WindowResizeBorders.InactiveGlowAlpha = value; }
        }


        /// <summary>
        /// The color of the glow around the window when the window is active
        /// </summary>
        public COLORREF ActiveGlowColor
        {
            get { return m_WindowResizeBorders.ActiveGlowColor; }
            set { m_WindowResizeBorders.ActiveGlowColor = value; }
        }

        /// <summary>
        /// The color of the glow around teh window when the window is not active
        /// </summary>
        public COLORREF InactiveGlowColor
        {
            get { return m_WindowResizeBorders.InactiveGlowColor; }
            set { m_WindowResizeBorders.InactiveGlowColor = value; }
        }

        private WindowChromeWorker(IntPtr hwnd)
        {
            m_Hwnd = hwnd;
        }

        /// <summary>
        /// Show the system menu at the screen pont (x, y)
        /// </summary>
        /// <param name="x">x position to show the system menu in screen coordinates</param>
        /// <param name="y">y position to show the system menu in screen coordinates</param>
        public void ShowSystemMenu(int x, int y)
        {
            if (m_Hwnd == IntPtr.Zero || !User32.IsWindow(m_Hwnd))
            {
                return;
            }

            var hmenu = User32.GetSystemMenu(m_Hwnd, false);

            if (hmenu != IntPtr.Zero)
            {
                var cmd = User32.TrackPopupMenuEx(hmenu, TPM.LEFTBUTTON | TPM.RETURNCMD, x, y, m_Hwnd, IntPtr.Zero);

                if (cmd != IntPtr.Zero)
                {
                    User32.PostMessage(m_Hwnd, WM.SYSCOMMAND, cmd, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        /// If the window is maximized this will restore the window to its normal size
        /// </summary>
        public void Restore()
        {
            SendSystemCommand(SC.RESTORE);
        }

        /// <summary>
        /// Maximize the window
        /// </summary>
        public void Maximize()
        {
            SendSystemCommand(SC.MAXIMIZE);
        }

        /// <summary>
        /// Minimize the window
        /// </summary>
        public void Minimize()
        {
            SendSystemCommand(SC.MINIMIZE);
        }

        /// <summary>
        /// When somebody clicks on the title bar the can drag the location of the window around. To implement
        /// this call this method in the onmousedown event and it will let the user drag the window around.
        /// </summary>
        public void MoveWindow()
        {
            User32.ReleaseCapture();
            User32.SendMessage(m_Hwnd, WM.NCLBUTTONDOWN, new IntPtr((int)HitTest.HTCAPTION), IntPtr.Zero);
        }

        /// <summary>
        /// This class needs to process message for the parent window for the chrome to work so you need to 
        /// pass the wndproc message to this method.
        /// </summary>
        /// <param name="hwnd">Handle of the window this message is destined to</param>
        /// <param name="msg">They type of message</param>
        /// <param name="wParam">First param of the message</param>
        /// <param name="lParam">Second param of the message</param>
        /// <param name="handled">True if you should send this message to the default 
        /// wndproc false if otherwise</param>
        /// <returns></returns>
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            switch (msg)
            {
                case (int)WM.SYSCOMMAND:
                    // To try and stop the user from maximizing and minimizing the form 
                    // We listen the command events and stop them if we need too. This is not
                    // fool proof unfortunately some window functions just reposition
                    // your window on maximize rather than go via WM_SYSCOMMAND I can't find 
                    // a better way of doing this though
                    var sc = wParam.ToInt32() & 0xFFF0;
                    switch (sc)
                    {
                        case (int)SC.MAXIMIZE:
                            if (!ShowMaximizeBox)
                            {
                                handled = true;
                                return IntPtr.Zero;
                            }
                            break;
                        case (int)SC.MINIMIZE:
                            if (!ShowMinimizeBox)
                            {
                                handled = true;
                                return IntPtr.Zero;
                            }
                            break;
                    }
                    break;
                case (int)WM.NCCALCSIZE:
                    // This is the magic that extends the window client area into the non client area
                    // which lets us custom draw all of the window.
                    var wndpl = new WINDOWPLACEMENT
                    {
                        length = Marshal.SizeOf(typeof(WINDOWPLACEMENT))
                    };
                    var windowInfo = new WINDOWINFO()
                    {
                        cbSize = Marshal.SizeOf(typeof(WINDOWINFO))
                    };

                    // If the window is maximized then the borders of the window are actually off the screen
                    // and then not displayed. Since we are drawing over all of the window this means that 
                    // bit of the window will be clipped. To get around this when the form is maximized we
                    // shrink the form so it all appears on the screen
                    if (User32.GetWindowPlacement(hwnd, ref wndpl) 
                        && wndpl.showCmd == SW.MAXIMIZE
                        && User32.GetWindowInfo(hwnd, ref windowInfo))
                    {
                        // Get the deafult size from the base wndproc
                        var originalRect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                        User32.DefWindowProc(hwnd, (int)WM.NCCALCSIZE, wParam, lParam);

                        // Shrink the window as the window is posistioned so the borders are drawn off the screen
                        var rectFromDefaultWinProc = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                        rectFromDefaultWinProc = new RECT(rectFromDefaultWinProc.Left,
                            (int)(originalRect.Top + windowInfo.cyWindowBorders),
                            rectFromDefaultWinProc.Right,
                            rectFromDefaultWinProc.Bottom);

                        // If somebody has got auto hide on their task bar then we need to shrink the window a little
                        // so there is space between the app and the taskbar otherwise we pinch all the mouse events 
                        // and teh task bar will never be unhidden 
                        var mon = User32.MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);

                        var mi = new MONITORINFOEX
                        {
                            cbSize = Marshal.SizeOf(typeof(MONITORINFOEX))
                        };

                        if (!User32.GetMonitorInfo(mon, ref mi))
                        {
                            // If the task bas is auto hiding we might need to trim a little bit of the window
                            if (mi.rcMonitor.Height == mi.rcWork.Height && mi.rcMonitor.Width == mi.rcWork.Width)
                            {
                                rectFromDefaultWinProc = AdjustWorkingAreaForAutoHide(mon, rectFromDefaultWinProc);
                            }
                        }

                        Marshal.StructureToPtr(rectFromDefaultWinProc, lParam, false);

                        handled = true;
                        return new IntPtr((int)(WVR.VALIDRECTS | WVR.REDRAW));
                    }

                    // This removes the border and the title bar from the window and extends the client area into them
                    if (wParam != IntPtr.Zero)
                    {
                        handled = true;
                        return IntPtr.Zero;
                    }
                    break;
                case (int)WM.NCUAHDRAWCAPTION:
                case (int)WM.NCUAHDRAWFRAME:
                case (int)WM.NCPAINT:
                    // Make sure windows never paints the non client region probably unnecessary but best to be sure
                    handled = true;
                    return IntPtr.Zero;
                case (int)WM.NCLBUTTONDOWN:
                    // This is based on a concept in chromium to work on windows 2008 - instead of trying to make it not visible (which doesn't work) we just turn off the caption drawing
                    // otherwise when you click to drag the window in 2008 the default minimize/maximize/close buttons are scribbled all over the window
                    // This may in fact be a "non-aero" problem. On Windows 2003 the Dwmapi isn't available so check for OS version and also defensively check Dwmapi.
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        try
                        {
                            bool aeroEnabled;
                            Dwmapi.DwmIsCompositionEnabled(out aeroEnabled);
                            if (aeroEnabled)
                            {
                                handled = false;
                                return IntPtr.Zero;
                            }
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                    IntPtr result;
                    var style = (WS)User32.GetWindowLongPtr(hwnd, GWL.STYLE).ToInt32();
                    try
                    {
                        User32.SetWindowLongPtr(hwnd, GWL.STYLE, (IntPtr)(style & ~WS.CAPTION));
                        result = User32.DefWindowProc(hwnd, msg, wParam, lParam);
                    }
                    finally
                    {
                        User32.SetWindowLongPtr(hwnd, GWL.STYLE, (IntPtr)style);
                    }
                    handled = true;
                    return result;
                case (int)WM.WINDOWPOSCHANGING:
                    {
                        // One annoyance is that by default windows have rounded corners. Now we want our windows to be square. This means we have to set
                        // The window region to be square. We need to do this whenever the window resizes as the window needs to be the correct size
                        var windowPosition = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof (WINDOWPOS));
                        var bounds = new RECT(windowPosition.x, windowPosition.y, windowPosition.x + windowPosition.cx, windowPosition.y + windowPosition.cy);

                        if ((windowPosition.flags & (int) SWP.NOSIZE) != (int) SWP.NOSIZE)
                        {
                            UpdateWindowRegion(hwnd, bounds);
                        }
                        break;
                    }
                case (int)WM.WINDOWPOSCHANGED:
                    {
                        // everytime the window moves we also need to update the 4 windows we have around the
                        // current window that we use to resize it so they are in the right location
                        var windowPosition = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof (WINDOWPOS));
                        var bounds = new RECT(windowPosition.x, windowPosition.y, windowPosition.x + windowPosition.cx, windowPosition.y + windowPosition.cy);

                        if ((windowPosition.flags & (int) SWP.NOSIZE) != (int) SWP.NOSIZE || (windowPosition.flags & (int) SWP.NOMOVE) != (int) SWP.NOMOVE || (windowPosition.flags & (int)SWP.SHOWWINDOW) == (int)SWP.SHOWWINDOW)
                        {
                            var isVisible = (windowPosition.flags & (int) SWP.HIDEWINDOW) != (int)SWP.HIDEWINDOW
                                && User32.IsWindowVisible(hwnd);
                            bool isActive = User32.GetActiveWindow() == hwnd;

                            m_WindowResizeBorders.OnParentStateUpdating(bounds, isActive, isVisible);
                            UpdateWindowRegion(hwnd, bounds);
                        }
                        break;
                    }
                case (int)WM.ACTIVATE:
                    {
                        // We render the border differently when the window is active / inactive
                        // so we need to let the border know when this changes
                        var isActive = wParam.ToInt32() != (int) WA.INACTIVE;
                        RECT bounds;
                        if (User32.GetWindowRect(hwnd, out bounds))
                        {
                            var isVisible = User32.IsWindowVisible(hwnd);
                            m_WindowResizeBorders.OnParentStateUpdating(bounds, isActive, isVisible);
                        }
                        break;
                    }
                case (int)WM.NCACTIVATE:
                    // We swallow this message to stop windows drawing the title of the form 
                    // when the form deactivates
                    var ret = User32.DefWindowProc(hwnd, (int)WM.NCACTIVATE, wParam, new IntPtr(-1));
                    handled = true;
                    return ret;
                case (int)WM.DESTROY:
                    // When the window is destroed free the resources help by the windo border
                    m_WindowResizeBorders.Dispose();
                    break;
            }

            handled = false;
            return IntPtr.Zero;
        }

        private void SendSystemCommand(SC command)
        {
            User32.PostMessage(m_Hwnd, WM.SYSCOMMAND, new IntPtr((int)command), IntPtr.Zero);
        }

        private void UpdateWindowButtons()
        {
            // If the state of the window buttons have changed we need to update the window style to reflect
            // The fact the are or are not there. We also need to make sure that the system menu is 
            // updated with their state
            WS styleToAdd = 0;
            WS styleToRemove = 0;
            if (ShowMaximizeBox)
            {
                styleToAdd |= WS.MAXIMIZEBOX;
            }
            else
            {
                styleToRemove |= WS.MAXIMIZEBOX;
            }

            if (ShowMinimizeBox)
            {
                styleToAdd |= WS.MINIMIZEBOX;
            }
            else
            {
                styleToRemove |= WS.MINIMIZEBOX;
            }

            EnableDisableSystemMenuItem(m_Hwnd, SC.MAXIMIZE, ShowMaximizeBox);
            EnableDisableSystemMenuItem(m_Hwnd, SC.MINIMIZE, ShowMinimizeBox);

            User32.ModifyStyle(m_Hwnd, styleToRemove, styleToAdd);
            RefreshWindowChrome(m_Hwnd);
        }

        private static void EnableDisableSystemMenuItem(IntPtr hwnd, SC menuItem, bool enable)
        {
            var menuHandle = User32.GetSystemMenu(hwnd, false);
            if (menuHandle != IntPtr.Zero)
            {
                var uEnable = enable ? MF.BYCOMMAND | MF.ENABLED : MF.BYCOMMAND | MF.GRAYED;
                User32.EnableMenuItem(menuHandle, menuItem, uEnable);
            }
        }

        private static void RefreshWindowChrome(IntPtr hwnd)
        {
            // This causes all the frame to be redrawn and all the properties around the nonclient area to be refreshed.
            User32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.DRAWFRAME | SWP.FRAMECHANGED | SWP.NOSIZE | SWP.NOMOVE | SWP.NOZORDER | SWP.NOOWNERZORDER | SWP.NOACTIVATE);
        }

        /// <summary>
        /// When we set the window style to WS.CAPTION the window has rounded corners.
        /// We need the window to be square. If you just clear the region windows just puts 
        /// it back again so we actually need to set the window to be square. Windows then
        /// leaves this window as square
        /// </summary>
        /// <param name="hwnd">Handle of the window</param>
        private void UpdateWindowRegion(IntPtr hwnd)
        {
            RECT windowRect;
            if (User32.GetWindowRect(hwnd, out windowRect))
            {
                UpdateWindowRegion(hwnd, windowRect);
            }
        }

        private void UpdateWindowRegion(IntPtr hwnd, RECT windowRect)
        {
            var windowPlacement = new WINDOWPLACEMENT
            {
                length = Marshal.SizeOf(typeof(WINDOWPLACEMENT))
            };

            var windowInfo = new WINDOWINFO()
            {
                cbSize = Marshal.SizeOf(typeof(WINDOWINFO))
            };

            if (!User32.GetWindowPlacement(hwnd, ref windowPlacement))
            {
                return;
            }

            // When the window is maximized the window is actually bigger than the screen. this is to make sure that the 
            // window borders are not shown. If we set the client area to this size then the windows is actually visible 
            // screen(s) next to where the window is maximized to we need to make sure the client area is only as big 
            // as the current screen
            RECT region;
            if(windowPlacement.showCmd == SW.MAXIMIZE)
            {
                if(!User32.GetWindowInfo(hwnd, ref windowInfo))
                {
                    return;
                }

                var borderWidth = (int)windowInfo.cxWindowBorders;
                var borderHeight = (int)windowInfo.cyWindowBorders;
                region = new RECT(borderWidth, borderHeight, windowRect.Width - borderWidth, windowRect.Height - borderHeight);
            }
            // When the window is minimized we don't need to do anything to the region
            // On windows 8 infact it sets the window to be the min size when minimizing 
            // Which breaks alt tabbing and the preview so we need to not set the 
            // region when minimizing. Only SW.SHOWMINIMIZED seems to be used but the other ones 
            // are here just in case
            else if (windowPlacement.showCmd == SW.FORCEMINIMIZE
                || windowPlacement.showCmd == SW.MINIMIZE
                || windowPlacement.showCmd == SW.SHOWMINIMIZED
                || windowPlacement.showCmd == SW.SHOWMINNOACTIVE)
            {
                return;
            }
            else
            {
                region = new RECT(0, 0, windowRect.Width, windowRect.Height);
            }

            // only update the region if it has changed
            if (region != m_LastWindowRegion)
            {
                m_LastWindowRegion = region;
                var rectRegion = IntPtr.Zero;
                try
                {
                    rectRegion = Gdi32.CreateRectRgn(region.Left, region.Top, region.Right, region.Bottom);
                    if (rectRegion != IntPtr.Zero)
                    {
                        User32.SetWindowRgn(hwnd, rectRegion, User32.IsWindowVisible(hwnd));
                    }
                }
                finally
                {
                    if (rectRegion != IntPtr.Zero)
                    {
                        Gdi32.DeleteObject(rectRegion);
                    }
                }
            }
        }

        private static bool EdgeHasAutoHideTaskbar(ABE edge, IntPtr applicationMonitor)
        {
            // This method detects if the taskbar is on the current screen and if it is in autohide mode
            var taskbarData = new APPBARDATA
            {
                cbSize = Marshal.SizeOf(typeof(APPBARDATA)),
                uEdge = edge
            };

            // Get the handle of the taskbar
            var taskbarhWnd = Shell32.SHAppBarMessage(ABM.GETAUTOHIDEBAR, ref taskbarData);

            if (taskbarhWnd == IntPtr.Zero)
            {
                return false;
            }

            if (!User32.IsWindow(taskbarhWnd))
            {
                return false;
            }

            // check to see if the taskbar is on the same monitor as us
            var taskBarMonitor = User32.MonitorFromWindow(taskbarhWnd, MONITOR.DEFAULTTONEAREST);
            if (taskBarMonitor != applicationMonitor)
            {
                return false;
            }

            var getIsAutoHide = new APPBARDATA()
            {
                cbSize = Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = taskbarhWnd
            };

            // See if the taskbar is set to autohide
            var appBarState = Shell32.SHAppBarMessage(ABM.GETSTATE, ref getIsAutoHide).ToInt32();
            if (appBarState != (int)ABS.AUTOHIDEANDONTOP && appBarState != (int)ABS.AUTOHIDE)
            {
                return false;
            }

            return true;
        }

        private static RECT AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, RECT area)
        {
            // There is a really anoying problem where if the user has set the task bar to 
            // autohide and we maximize we cover it so it doesn't unhide when the user moves their
            // mouse over it. The solution is to shrink the window a little so the window doesn't
            // cover the taskbar
            int left = area.Left, right = area.Right, top = area.Top, bottom = area.Bottom;

            if (EdgeHasAutoHideTaskbar(ABE.LEFT, monitorContainingApplication))
            {
                left += 2;
            }

            if (EdgeHasAutoHideTaskbar(ABE.RIGHT, monitorContainingApplication))
            {
                right -= 2;
            }

            if (EdgeHasAutoHideTaskbar(ABE.TOP, monitorContainingApplication))
            {
                top += 2;
            }

            if (EdgeHasAutoHideTaskbar(ABE.BOTTOM, monitorContainingApplication))
            {
                bottom -= 2;
            }

            return new RECT(left, top, right, bottom);
        }
    }
}