﻿// ReSharper disable InconsistentNaming
namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    public enum WM : uint
    {
        NULL = 0x0,
        CREATE = 0x1,
        DESTROY = 0x2,
        MOVE = 0x3,
        SIZE = 0x5,
        ACTIVATE = 0x6,
        SETFOCUS = 0x7,
        KILLFOCUS = 0x8,
        ENABLE = 0xa,
        SETREDRAW = 0xb,
        SETTEXT = 0xc,
        GETTEXT = 0xd,
        GETTEXTLENGTH = 0xe,
        PAINT = 0xf,
        CLOSE = 0x10,
        QUERYENDSESSION = 0x11,
        QUERYOPEN = 0x13,
        ENDSESSION = 0x16,
        QUIT = 0x12,
        ERASEBKGND = 0x14,
        SYSCOLORCHANGE = 0x15,
        SHOWWINDOW = 0x18,
        WININICHANGE = 0x1a,
        SETTINGCHANGE = WININICHANGE,
        DEVMODECHANGE = 0x1b,
        ACTIVATEAPP = 0x1c,
        FONTCHANGE = 0x1d,
        TIMECHANGE = 0x1e,
        CANCELMODE = 0x1f,
        SETCURSOR = 0x20,
        MOUSEACTIVATE = 0x21,
        CHILDACTIVATE = 0x22,
        QUEUESYNC = 0x23,
        GETMINMAXINFO = 0x24,
        PAINTICON = 0x26,
        ICONERASEBKGND = 0x27,
        NEXTDLGCTL = 0x28,
        SPOOLERSTATUS = 0x2a,
        DRAWITEM = 0x2b,
        MEASUREITEM = 0x2c,
        DELETEITEM = 0x2d,
        VKEYTOITEM = 0x2e,
        CHARTOITEM = 0x2f,
        SETFONT = 0x30,
        GETFONT = 0x31,
        SETHOTKEY = 0x32,
        GETHOTKEY = 0x33,
        QUERYDRAGICON = 0x37,
        COMPAREITEM = 0x39,
        GETOBJECT = 0x3d,
        COMPACTING = 0x41,
        COMMNOTIFY = 0x44,
        WINDOWPOSCHANGING = 0x46,
        WINDOWPOSCHANGED = 0x47,
        POWER = 0x48,
        COPYDATA = 0x4a,
        CANCELJOURNAL = 0x4b,
        NOTIFY = 0x4e,
        INPUTLANGCHANGEREQUEST = 0x50,
        INPUTLANGCHANGE = 0x51,
        TCARD = 0x52,
        HELP = 0x53,
        USERCHANGED = 0x54,
        NOTIFYFORMAT = 0x55,
        CONTEXTMENU = 0x7b,
        STYLECHANGING = 0x7c,
        STYLECHANGED = 0x7d,
        DISPLAYCHANGE = 0x7e,
        GETICON = 0x7f,
        SETICON = 0x80,
        NCCREATE = 0x81,
        NCDESTROY = 0x82,
        NCCALCSIZE = 0x83,
        NCHITTEST = 0x84,
        NCPAINT = 0x85,
        NCACTIVATE = 0x86,
        GETDLGCODE = 0x87,
        SYNCPAINT = 0x88,
        NCMOUSEMOVE = 0xa0,
        NCLBUTTONDOWN = 0xa1,
        NCLBUTTONUP = 0xa2,
        NCLBUTTONDBLCLK = 0xa3,
        NCRBUTTONDOWN = 0xa4,
        NCRBUTTONUP = 0xa5,
        NCRBUTTONDBLCLK = 0xa6,
        NCMBUTTONDOWN = 0xa7,
        NCMBUTTONUP = 0xa8,
        NCMBUTTONDBLCLK = 0xa9,
        NCXBUTTONDOWN = 0xab,
        NCXBUTTONUP = 0xac,
        NCXBUTTONDBLCLK = 0xad,
        INPUT = 0xff,
        KEYFIRST = 0x100,
        KEYDOWN = 0x100,
        KEYUP = 0x101,
        CHAR = 0x102,
        DEADCHAR = 0x103,
        SYSKEYDOWN = 0x104,
        SYSKEYUP = 0x105,
        SYSCHAR = 0x106,
        SYSDEADCHAR = 0x107,
        UNICHAR = 0x109,
        KEYLAST = 0x108,
        IME_STARTCOMPOSITION = 0x10d,
        IME_ENDCOMPOSITION = 0x10e,
        IME_COMPOSITION = 0x10f,
        IME_KEYLAST = 0x10f,
        INITDIALOG = 0x110,
        COMMAND = 0x111,
        SYSCOMMAND = 0x112,
        TIMER = 0x113,
        HSCROLL = 0x114,
        VSCROLL = 0x115,
        INITMENU = 0x116,
        INITMENUPOPUP = 0x117,
        MENUSELECT = 0x11f,
        MENUCHAR = 0x120,
        ENTERIDLE = 0x121,
        MENURBUTTONUP = 0x122,
        MENUDRAG = 0x123,
        MENUGETOBJECT = 0x124,
        UNINITMENUPOPUP = 0x125,
        MENUCOMMAND = 0x126,
        CHANGEUISTATE = 0x127,
        UPDATEUISTATE = 0x128,
        QUERYUISTATE = 0x129,
        CTLCOLOR = 0x19,
        CTLCOLORMSGBOX = 0x132,
        CTLCOLOREDIT = 0x133,
        CTLCOLORLISTBOX = 0x134,
        CTLCOLORBTN = 0x135,
        CTLCOLORDLG = 0x136,
        CTLCOLORSCROLLBAR = 0x137,
        CTLCOLORSTATIC = 0x138,
        MOUSEFIRST = 0x200,
        MOUSEMOVE = 0x200,
        LBUTTONDOWN = 0x201,
        LBUTTONUP = 0x202,
        LBUTTONDBLCLK = 0x203,
        RBUTTONDOWN = 0x204,
        RBUTTONUP = 0x205,
        RBUTTONDBLCLK = 0x206,
        MBUTTONDOWN = 0x207,
        MBUTTONUP = 0x208,
        MBUTTONDBLCLK = 0x209,
        MOUSEWHEEL = 0x20a,
        XBUTTONDOWN = 0x20b,
        XBUTTONUP = 0x20c,
        XBUTTONDBLCLK = 0x20d,
        MOUSELAST = 0x20d,
        PARENTNOTIFY = 0x210,
        ENTERMENULOOP = 0x211,
        EXITMENULOOP = 0x212,
        NEXTMENU = 0x213,
        SIZING = 0x214,
        CAPTURECHANGED = 0x215,
        MOVING = 0x216,
        POWERBROADCAST = 0x218,
        DEVICECHANGE = 0x219,
        MDICREATE = 0x220,
        MDIDESTROY = 0x221,
        MDIACTIVATE = 0x222,
        MDIRESTORE = 0x223,
        MDINEXT = 0x224,
        MDIMAXIMIZE = 0x225,
        MDITILE = 0x226,
        MDICASCADE = 0x227,
        MDIICONARRANGE = 0x228,
        MDIGETACTIVE = 0x229,
        MDISETMENU = 0x230,
        ENTERSIZEMOVE = 0x231,
        EXITSIZEMOVE = 0x232,
        DROPFILES = 0x233,
        MDIREFRESHMENU = 0x234,
        IME_SETCONTEXT = 0x281,
        IME_NOTIFY = 0x282,
        IME_CONTROL = 0x283,
        IME_COMPOSITIONFULL = 0x284,
        IME_SELECT = 0x285,
        IME_CHAR = 0x286,
        IME_REQUEST = 0x288,
        IME_KEYDOWN = 0x290,
        IME_KEYUP = 0x291,
        MOUSEHOVER = 0x2a1,
        MOUSELEAVE = 0x2a3,
        NCMOUSELEAVE = 0x2a2,
        WTSSESSION_CHANGE = 0x2b1,
        TABLET_FIRST = 0x2c0,
        TABLET_LAST = 0x2df,
        CUT = 0x300,
        COPY = 0x301,
        PASTE = 0x302,
        CLEAR = 0x303,
        UNDO = 0x304,
        RENDERFORMAT = 0x305,
        RENDERALLFORMATS = 0x306,
        DESTROYCLIPBOARD = 0x307,
        DRAWCLIPBOARD = 0x308,
        PAINTCLIPBOARD = 0x309,
        VSCROLLCLIPBOARD = 0x30a,
        SIZECLIPBOARD = 0x30b,
        ASKCBFORMATNAME = 0x30c,
        CHANGECBCHAIN = 0x30d,
        HSCROLLCLIPBOARD = 0x30e,
        QUERYNEWPALETTE = 0x30f,
        PALETTEISCHANGING = 0x310,
        PALETTECHANGED = 0x311,
        HOTKEY = 0x312,
        PRINT = 0x317,
        PRINTCLIENT = 0x318,
        APPCOMMAND = 0x319,
        THEMECHANGED = 0x31a,
        HANDHELDFIRST = 0x358,
        HANDHELDLAST = 0x35f,
        AFXFIRST = 0x360,
        AFXLAST = 0x37f,
        PENWINFIRST = 0x380,
        PENWINLAST = 0x38f,
        USER = 0x400,
        REFLECT = 0x2000,
        APP = 0x8000,
        DWMCOMPOSITIONCHANGED = 0x031E,
        
        // These two messages aren't defined in winuser.h, but they are sent to windows
        // with captions. They appear to paint the window caption and frame.
        // Unfortunately if you override the standard non-client rendering as we do
        // with CustomFrameWindow, sometimes Windows (not deterministically
        // reproducibly but definitely frequently) will send these messages to the
        // window and paint the standard caption/title over the top of the custom one.
        // So we need to handle these messages in CustomFrameWindow to prevent this
        // from happening.
        NCUAHDRAWCAPTION = 0xAE,
        NCUAHDRAWFRAME = 0xAF,
    }
}
