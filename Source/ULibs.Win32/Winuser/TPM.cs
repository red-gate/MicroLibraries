using System;
// ReSharper disable InconsistentNaming

namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    [Flags]
    internal enum TPM
    {
        CENTERALIGN = 0x0004,
        LEFTALIGN = 0x0000,
        RIGHTALIGN = 0x0008,
        
        BOTTOMALIGN = 0x0020,
        TOPALIGN = 0x0000,
        VCENTERALIGN = 0x0010,
        
        NONOTIFY = 0x0080,
        RETURNCMD = 0x0100,
        
        LEFTBUTTON = 0x0000,
        RIGHTBUTTON = 0x0002,
        
        HORNEGANIMATION = 0x0800,
        HORPOSANIMATION = 0x0400,
        NOANIMATION = 0x4000,
        VERNEGANIMATION = 0x2000,
        VERPOSANIMATION = 0x1000,
        
        HORIZONTAL = 0x0000,
        VERTICAL = 0x0040            
    }
}