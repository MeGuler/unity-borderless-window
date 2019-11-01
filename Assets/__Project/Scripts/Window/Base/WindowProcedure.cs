using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Window.Apis;
using Window.Flags;
using Window.Structures;

namespace Window
{
    public class WindowProcedure
    {
        ~WindowProcedure()
        {
            TerminateWindowProcedure();
        }

        public delegate void WindowProcedureEventDelegate(IntPtr handledWindow, IntPtr firstParameter,
            IntPtr secondParameter);
        
        #region Events

        public event WindowProcedureEventDelegate NcDestroy;
        public event WindowProcedureEventDelegate NcHitTest;
        public event WindowProcedureEventDelegate GetMinMaxInfo;
        public event WindowProcedureEventDelegate Sizing;
        public event WindowProcedureEventDelegate HShellGetMinRect;

        #endregion
        

        #region Window Procedure Properties

        public HandleRef HandledWindow { get; protected set; }
        public IntPtr OldWindowProcedurePtr { get; protected set; }
        public IntPtr NewWindowProcedurePtr { get; protected set; }
        public User32.WindowProcedureDelegate NewWindowProcedure { get; protected set; }

        #endregion

        public bool IgnoreNonClientHitTest;
        public bool Initialized;

        public readonly Vector4Int BorderSize;
        public readonly int CaptionHeight;

        public WindowProcedure(Vector4Int borderSize, int captionHeight)
        {
            BorderSize = borderSize;
            CaptionHeight = captionHeight;

            HandleWindow();
            InitializeWindowProcedure();
        }

        private HitTestValues GetCursorAreaFlags()
        {
            var windowRect = GetWindowRectPoints();
            var cursorPositionFlag = Cursor.GetCursorAreaFlags(Cursor.GetCursorPosition(), windowRect, BorderSize,
                CaptionHeight);

            return cursorPositionFlag;
        }

        public Rect GetWindowRectPoints()
        {
            User32.GetWindowRect(HandledWindow.Handle, out var windowRect);
            return windowRect;
        }

        
        #region Window Procedures

        private void HandleWindow()
        {
            //https://gist.github.com/mattbenic/908483ad0bedbc62ab17
            var threadId = Kernel32.GetCurrentThreadId();
            User32.EnumThreadWindows(threadId, (hWnd, lParam) =>
            {
                var classText = new StringBuilder(User32.UnityWindowClassName.Length + 1);
                User32.GetClassName(hWnd, classText, classText.Capacity);
                if (classText.ToString() == User32.UnityWindowClassName)
                {
                    HandledWindow = new HandleRef(null, hWnd);
                    return false;
                }

                return true;
            }, IntPtr.Zero);
        }

        private void InitializeWindowProcedure()
        {
            if (NewWindowProcedure != null) return;

            Initialized = true;

            NewWindowProcedure = WindowProcedures;
            NewWindowProcedurePtr = Marshal.GetFunctionPointerForDelegate(NewWindowProcedure);
            OldWindowProcedurePtr = User32.SetWindowLongPtr(HandledWindow, -4, NewWindowProcedurePtr);
        }

        private void TerminateWindowProcedure()
        {
            if (NewWindowProcedure == null) return;

            Initialized = false;

            User32.SetWindowLongPtr(HandledWindow, -4, OldWindowProcedurePtr);
            OldWindowProcedurePtr = IntPtr.Zero;
            NewWindowProcedurePtr = IntPtr.Zero;
            NewWindowProcedure = null;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private IntPtr WindowProcedures(IntPtr handleWindow, uint message, IntPtr wParam, IntPtr lParam)
        {
            if (handleWindow != HandledWindow.Handle)
            {
                return IntPtr.Zero;
            }

            if (message == (uint) WindowMessages.NcDestroy)
            {
                OnNcDestroy(handleWindow, wParam, lParam);
                TerminateWindowProcedure();
            }


            if (message == (uint) WindowMessages.GetMinMaxInfo)
            {
                OnGetMinMaxInfo(handleWindow, wParam, lParam);
            }

            if (message == (uint) WindowMessages.Sizing)
            {
                OnSizing(handleWindow, wParam, lParam);
            }

            if (message == (uint) WindowMessages.HShell_GetMinRect)
            {
                OnHShellGetMinRect(handleWindow, wParam, lParam);
            }

//            if (message != (uint) WindowMessages.Close &&
//                message != (uint) WindowMessages.Input &&
//                message != (uint) WindowMessages.WindowPosChanging &&
//                message != (uint) WindowMessages.MouseMove &&
//                message != (uint) WindowMessages.MouseActivate &&
//                message != (uint) WindowMessages.MouseHover &&
//                message != (uint) WindowMessages.MouseFirst &&
//                message != (uint) WindowMessages.MouseLast &&
//                message != (uint) WindowMessages.MouseLeave &&
//                message != (uint) WindowMessages.MouseWheel &&
//                message != (uint) WindowMessages.NC_MouseMove &&
//                message != (uint) WindowMessages.NC_MouseHover &&
//                message != (uint) WindowMessages.NC_MouseLeave &&
//                message != (uint) WindowMessages.LeftButtonDown &&
//                message != (uint) WindowMessages.LeftButtonUp &&
//                message != (uint) WindowMessages.LeftButtonDoubleClick &&
//                message != (uint) WindowMessages.NC_LeftButtonDown &&
//                message != (uint) WindowMessages.NC_LeftButtonUp &&
//                message != (uint) WindowMessages.NC_LeftButtonDoubleClick &&
//                message != (uint) WindowMessages.SetCursor &&
//                message != (uint) WindowMessages.NcHitTest &&
//                message != (uint) WindowMessages.Moving &&
//                message != (uint) WindowMessages.IME_SetContext &&
////                message != (uint) WindowMessages.CaptureChanged &&
//                message != (uint) WindowMessages.GetIcon &&
//                message != (uint) WindowMessages.KeyDown &&
//                message != (uint) WindowMessages.Char &&
//                message != (uint) WindowMessages.KeyUp &&
//                //===================================
//                message != (uint) WindowMessages.HShell_Language &&
//                message != (uint) WindowMessages.ActivateApp &&
//                message != (uint) WindowMessages.SystemKeyDown &&
//                message != (uint) WindowMessages.CaptureChanged &&
//                message != (uint) WindowMessages.WindowPosChanged &&
//                message != (uint) WindowMessages.HShell_ActivateShellWindow
////                message != (uint) WindowMessages.Char &&
//            )
//            {
//            }


            //Non Client Hit Test
            if (message == (uint) WindowMessages.NcHitTest)
            {
                OnNcHitTest(handleWindow, wParam, lParam);

                if (!IgnoreNonClientHitTest)
                {
                    //Get Cursor Area
                    var resize = GetCursorAreaFlags();

                    if (resize != HitTestValues.Client)
                    {
                        return (IntPtr) resize;
                    }
                }
            }

            return User32.CallWindowProc(OldWindowProcedurePtr, handleWindow, message, wParam, lParam);
        }

        #endregion

        #region Event Invokes

        private void OnNcDestroy(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            NcDestroy?.Invoke(handledWindow, firstParameter, secondParameter);
        }

        private void OnNcHitTest(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            NcHitTest?.Invoke(handledWindow, firstParameter, secondParameter);
        }

        private void OnGetMinMaxInfo(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            GetMinMaxInfo?.Invoke(handledWindow, firstParameter, secondParameter);
        }

        private void OnSizing(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            Sizing?.Invoke(handledWindow, firstParameter, secondParameter);
        }

        private void OnHShellGetMinRect(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            HShellGetMinRect?.Invoke(handledWindow, firstParameter, secondParameter);
        }

        #endregion
    }
}