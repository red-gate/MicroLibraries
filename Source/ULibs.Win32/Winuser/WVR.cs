using System;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    /// <summary>
    /// This is the return code you send back when you process a WM_NCCALCSIZE message
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms632634%28v=vs.85%29.aspx
    /// </summary>
    [Flags]
    internal enum WVR
    {
        ALIGNTOP = 0x0010,
        ALIGNLEFT = 0x0020,
        ALIGNBOTTOM = 0x0040,
        ALIGNRIGHT = 0x0080,
        HREDRAW = 0x0100,
        VREDRAW = 0x0200,
        VALIDRECTS = 0x0400,
        REDRAW = HREDRAW | VREDRAW,
    }
}