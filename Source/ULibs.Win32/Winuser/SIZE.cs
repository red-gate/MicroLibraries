// ReSharper disable InconsistentNaming
namespace /***$rootnamespace$.***/ULibs.Win32.Winuser
{
    /// <summary>
    /// What you get in wParam when you recieve a WM_SIZE 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms632646%28v=vs.85%29.aspx
    /// </summary>
    public enum SIZE_TYPE
    {
        MAXHIDE = 4,
        MAXIMIZED = 2,
        MAXSHOW = 3,
        MINIMIZED = 1,
        RESTORED = 0,
    }
}