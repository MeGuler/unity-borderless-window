using System;
using System.Runtime.InteropServices;

public class WinApi
{
    #region Window

    #region GetActiveWindow

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    #endregion

    #region ShowWindow

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr handleWindow, int flag);

    #endregion

    #region GetWindowRect

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr handleWindow, out WindowRect rect);

    #endregion

    #region MoveWindow

    [DllImport("user32.dll")]
    public static extern bool MoveWindow(IntPtr handleWindow, int x, int y, int width, int height, bool repaint);

    #endregion

    #region SetWindowPosition

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr handleWindow, int insertAfter, int x, int y, int width,
        int height,
        uint flags);

    #endregion

    #region GetWindowLong

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
    }

    #endregion

    #region SetWindowLong

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    #endregion

    #region DefWindowProc

    [DllImport("user32.dll", EntryPoint = "DefWindowProcA")]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

    #endregion

    #region EndDialog
    
    [DllImport("user32.dll")]
    public static extern bool EndDialog(IntPtr hDlg, IntPtr nResult);
    
    #endregion
    
    #region BeginPaint

    [DllImport("user32.dll")]
    public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);
    
    #endregion
    
    #region EndPaint

    [DllImport("user32.dll")]
    public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);
    
    #endregion

    #endregion

    #region Cursor

    #region GetCursorPosition

    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
    public static extern bool GetCursorPosition(out CursorPoint lpPoint);

    #endregion

    #region SetSystemCursor

    [DllImport("user32.dll")]
    public static extern bool SetSystemCursor(IntPtr handleCursor, uint id);

    #endregion

    #region LoadCursor

    [DllImport("user32.dll")]
    public static extern IntPtr LoadCursor(IntPtr handleInstance, int lpCursorName);

    #endregion

    #region CopyIcon

    [DllImport("user32.dll")]
    public static extern IntPtr CopyIcon(IntPtr cursor);

    #endregion

    #endregion
}