using System;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [Flags]
    // ReSharper disable InconsistentNaming
    public enum CS
    {
        BYTEALIGNCLIENT = 0x1000,
        BYTEALIGNWINDOW = 0x2000,
        CLASSDC = 0x0040,
        DBLCLKS = 0x0008,
        DROPSHADOW = 0x00020000, 
        GLOBALCLASS = 0x4000,
        HREDRAW = 0x0002,
        NOCLOSE = 0x0200,
        OWNDC = 0x0020,
        PARENTDC = 0x0080,
        SAVEBITS = 0x0800,
        VREDRAW = 0x0001
    }
    // ReSharper restore InconsistentNaming
}