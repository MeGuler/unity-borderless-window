using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Borderless.Api.Structures;
using Point = Borderless.Api.Structures.Point;

namespace Borderless.Api
{
    public class User32
    {
        #region WindowProcedureDelegate

        public delegate IntPtr WindowProcedureDelegate
        (
            IntPtr handledWindow,
            uint message,
            IntPtr firstParameter,
            IntPtr secondParameter
        );

        #endregion

        
        #region GetCursorPosition

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPosition(out Point lpPoint);

        #endregion
        

        #region Window Methods

        #region GetActiveWindow

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        #endregion

        #region GetForegroundWindow

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        #endregion

        #region ShowWindow

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handledWindow, int flag);

        #endregion

        #region GetWindowRect

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr handledWindow, out Rect rect);

        #endregion

        #region MoveWindow

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr handledWindow, int x, int y, int width, int height, bool repaint);

        #endregion

        #region SetWindowPos

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos
        (
            IntPtr handledWindow,
            int handledWindowInsertAfter,
            int x,
            int y,
            int width,
            int height,
            uint flags
        );

        #endregion

        #region GetWindowLong

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr handledWindow, int windowLongIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr handledWindow, int windowLongIndex);

        public static IntPtr GetWindowLongPtr(IntPtr handledWindow, int windowLongIndex)
        {
            return IntPtr.Size == 8
                ? GetWindowLongPtr64(handledWindow, windowLongIndex)
                : GetWindowLongPtr32(handledWindow, windowLongIndex);
        }

        #endregion

        #region SetWindowLong

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef handledWindow, int windowLongIndex, int windowStyleFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef handledWindow, int windowLongIndex,
            IntPtr windowStyleFlags);

        public static IntPtr SetWindowLongPtr(HandleRef handledWindow, int windowLongIndex, IntPtr windowStyleFlags)
        {
            return IntPtr.Size == 8
                ? SetWindowLongPtr64(handledWindow, windowLongIndex, windowStyleFlags)
                : new IntPtr(SetWindowLong32(handledWindow, windowLongIndex, windowStyleFlags.ToInt32()));
        }

        #endregion

        #region CallWindowProc

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CallWindowProc
        (
            IntPtr previousWindowProcedure,
            IntPtr handledWindow,
            uint message,
            IntPtr wParam,
            IntPtr lParam
        );

        #endregion

        #region DefWindowProc

        [DllImport("user32.dll", EntryPoint = "DefWindowProcA")]
        public static extern IntPtr DefWindowProc
        (
            IntPtr handledWindow,
            uint message,
            IntPtr firstParameter,
            IntPtr secondParameter
        );

        #endregion

        #endregion
        

    }
}