using System;
using System.Runtime.InteropServices;

namespace Borderless
{
    public class WinApi
    {
        #region Window

        #region GetActiveWindow

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        #endregion

        #region ShowWindow

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr window, int flag);

        #endregion

        #region LockWindowUpdate

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr window);

        #endregion

        #region GetWindowRect

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr window, out WindowRect rect);

        #endregion

        #region MoveWindow

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr window, int x, int y, int width, int height, bool repaint);

        #endregion

        #region SetWindowPosition

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr window, int insertAfter, int x, int y, int width,
            int height,
            uint flags);

        #endregion

        #region GetWindowLong

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr window, int windowLongIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr window, int windowLongIndex);

        public static IntPtr GetWindowLongPtr(IntPtr window, int windowLongIndex)
        {
            return IntPtr.Size == 8
                ? GetWindowLongPtr64(window, windowLongIndex)
                : GetWindowLongPtr32(window, windowLongIndex);
        }

        #endregion

        #region SetWindowLong

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef window, int windowLongIndex, int windowStyleFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef window, int windowLongIndex, IntPtr windowStyleFlags);

        public static IntPtr SetWindowLongPtr(HandleRef window, int windowLongIndex, IntPtr windowStyleFlags)
        {
            return IntPtr.Size == 8
                ? SetWindowLongPtr64(window, windowLongIndex, windowStyleFlags)
                : new IntPtr(SetWindowLong32(window, windowLongIndex, windowStyleFlags.ToInt32()));
        }

        #endregion

        #region DefWindowProc

        [DllImport("user32.dll", EntryPoint = "DefWindowProcA")]
        public static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        #endregion

        #region EndDialog

        [DllImport("user32.dll")]
        public static extern bool EndDialog(IntPtr window, IntPtr nResult);

        #endregion

        #region BeginPaint

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr window, out PAINTSTRUCT lpPaint);

        #endregion

        #region EndPaint

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr window, [In] ref PAINTSTRUCT lpPaint);

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

}
