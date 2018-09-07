﻿using System;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [Flags]
    internal enum WS_EX : uint
    {
        None = 0,

        ACCEPTFILES = 0x00000010,
        APPWINDOW = 0x00040000,
        CLIENTEDGE = 0x00000200,
        COMPOSITED = 0x02000000,
        CONTEXTHELP = 0x00000400,
        CONTROLPARENT = 0x00010000,
        DLGMODALFRAME = 0x00000001,
        LAYERED = 0x00080000,
        LAYOUTRTL = 0x00400000,
        LEFT = 0x00000000,
        LEFTSCROLLBAR = 0x00004000,
        LTRREADING = 0x00000000,
        MDICHILD = 0x00000040,
        NOACTIVATE = 0x08000000,
        NOINHERITLAYOUT = 0x00100000,
        NOPARENTNOTIFY = 0x00000004,
        NOREDIRECTIONBITMAP = 0x00200000,
        OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,
        PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,
        RIGHT = 0x00001000,
        RIGHTSCROLLBAR = 0x00000000,
        RTLREADING = 0x00002000,
        STATICEDGE = 0x00020000,
        TOOLWINDOW = 0x00000080,
        TOPMOST = 0x00000008,
        TRANSPARENT = 0x00000020,
        WINDOWEDGE = 0x00000100

    }
}