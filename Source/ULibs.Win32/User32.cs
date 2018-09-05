using System;
using System.Runtime.InteropServices;
using ULibs.Win32.Windef;
using ULibs.Win32.Winuser;

namespace /***$rootnamespace$.***/ULibs.Win32
{
    public static class User32
    {
        private static readonly bool s_Is64BitProcess = 8 == IntPtr.Size;

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);
        
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ReleaseCapture();

        // ReSharper disable once InconsistentNaming
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        // ReSharper disable once InconsistentNaming
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr TrackPopupMenuEx(IntPtr hmenu, TPM fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);
       
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
        {
            return s_Is64BitProcess ? SetWindowLongPtr64(hwnd, nIndex, dwNewLong) : new IntPtr(SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);
        
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern int GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);
        
        public static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
        {
            return s_Is64BitProcess ? GetWindowLongPtr64(hwnd, nIndex) : new IntPtr(GetWindowLongPtr32(hwnd, nIndex));
        }

        public static void ModifyStyle(IntPtr hwnd, WS removeStyle, WS addStyle)
        {
            var intPtr = GetWindowLongPtr(hwnd, GWL.STYLE);
            var dwStyle = (WS)(s_Is64BitProcess ? intPtr.ToInt64() : intPtr.ToInt32());
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;

            if (dwStyle != dwNewStyle)
            {
                SetWindowLongPtr(hwnd, GWL.STYLE, new IntPtr((int)dwNewStyle));
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, MONITOR flags);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hWnd, ref WINDOWINFO pwi);

        // ReSharper disable once InconsistentNaming
        [DllImport("user32.dll")]
        public static extern MF EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable);

        [DllImport("user32.dll", EntryPoint = "SetClassLong", SetLastError = true)]
        public static extern uint SetClassLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetClassLongPtr", SetLastError = true)]
        public static extern IntPtr SetClassLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetClassLongPtr(IntPtr hwnd, int nIndex, IntPtr dwNewLong)
        {
            return s_Is64BitProcess ? SetClassLongPtr64(hwnd, nIndex, dwNewLong) : new IntPtr(SetClassLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong", SetLastError = true)]
        public static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr", SetLastError = true)]
        public static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);

        public static IntPtr GetClassLongPtr(IntPtr hwnd, int nIndex)
        {
            return s_Is64BitProcess ? GetClassLong64(hwnd, nIndex) : new IntPtr(GetClassLong32(hwnd, nIndex));
        }

        public static void ModifyClass(IntPtr hwnd, CS removeClass, CS addClass)
        {
            var classValue = GetClassLongPtr(hwnd, (int)GCL.STYLE);
            var currentClass = (CS)(s_Is64BitProcess ? classValue.ToInt64() : classValue.ToInt32());
            var newClass = (currentClass & ~removeClass) | addClass;

            if (currentClass != newClass)
            {
                SetClassLongPtr(hwnd, (int)GCL.STYLE, new IntPtr((int)newClass));
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowEx(WS_EX dwExStyle, string lpClassName, string lpWindowName, WS dwStyle, 
            int x, int y, int nWidth, int nHeight, 
            IntPtr hwndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        // ReSharper disable once InconsistentNaming
        [DllImport("user32.dll")]
        public static extern bool FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

        [DllImport("user32.dll")]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, COLORREF crKey, ref BLENDFUNCTION pblend, ULW dwFlags);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, COLORREF crKey, byte bAlpha, LWA dwFlags);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern IntPtr GetCursor();

        [DllImport("user32.dll", EntryPoint = "LoadImage", SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, IMAGE uType, int cxDesired, int cyDesired, LR fuLoad);

        [DllImport("user32.dll", EntryPoint = "LoadImage", SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, IMAGE uType, int cxDesired, int cyDesired, LR fuLoad);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        // ReSharper disable once InconsistentNaming
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        // ReSharper disable once InconsistentNaming
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
    }
}
