// ReSharper disable InconsistentNaming
using System;

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [Flags]
    internal enum MF : uint
    {
        DOES_NOT_EXIST = unchecked((uint)-1),

        ENABLED = 0,
        BYCOMMAND = 0,
        GRAYED = 1,
        DISABLED = 2,
    }
}