using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using UnityEngine;
using XGraphic = System.Drawing.Graphics;

namespace Borderless
{
    public class Window : MonoBehaviour
    {
        #region Window Procedure

        private HandleRef _handledWindow;
        private IntPtr _oldWindowProcessesPtr;
        private IntPtr _newWindowProcessesPtr;
        private WndProcDelegate _newWindowProcesses;

        #endregion

        #region Window Settings

        private readonly Vector2Int _defaultMinWindowSize = new Vector2Int(200, 200);

        protected Vector2Int MinWindowSize;
        protected Vector2Int MaxWindowSize;
        protected Vector4Int ResizeHandleSize;
        protected int CaptionHeight;

        #endregion


        protected virtual void Start()
        {
            InitializeWindowProcedure();
            UpdateStyle();
        }

        protected virtual void OnGUI()
        {
            if (_handledWindow.Handle == IntPtr.Zero)
            {
                InitializeWindowProcedure();
            }
        }

        protected virtual void InitializeWindowProcedure()
        {
            Debug.Log("Init");
            _handledWindow = new HandleRef(null, WinApi.GetActiveWindow());
            _newWindowProcesses = WindowProcedure;
            _newWindowProcessesPtr = Marshal.GetFunctionPointerForDelegate(_newWindowProcesses);
            _oldWindowProcessesPtr = WinApi.SetWindowLongPtr(_handledWindow, -4, _newWindowProcessesPtr);
        }

        protected virtual void TerminateWindowProcedure()
        {
            WinApi.SetWindowLongPtr(_handledWindow, -4, _oldWindowProcessesPtr);
            _handledWindow = new HandleRef(null, IntPtr.Zero);
            _oldWindowProcessesPtr = IntPtr.Zero;
            _newWindowProcessesPtr = IntPtr.Zero;
            _newWindowProcesses = null;
        }

        ~Window()
        {
            TerminateWindowProcedure();
        }

        protected virtual void UpdateStyle()
        {
            WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.Style, (IntPtr) WindowStyleFlags.Visible);
        }


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected virtual IntPtr WindowProcedure(IntPtr handleWindow, uint message, IntPtr outValue, IntPtr inValue)
        {
            if (message == (uint) WindowMessages.NCDESTROY || message == (uint) WindowMessages.WINDOWPOSCHANGING)
            {
                TerminateWindowProcedure();
            }

            if (message == (uint) WindowMessages.GETMINMAXINFO)
            {
                return MinMaxSize(inValue);
            }

            if (message == (uint) WindowMessages.NCHITTEST)
            {
                return Resize(inValue);
            }

//        switch (message)
//        {
//            case (uint) WindowMessages.PAINT:
//
//                var hdc = BeginPaint(handleWindow, out var ps);
//
//                var gfx = XGraphic.FromHwnd(hdc);
//                var brush = new SolidBrush(Color.White);
//
//                gfx.FillRectangle(brush,
//                    ps.rcPaint.Left,
//                    ps.rcPaint.Top,
//                    ps.rcPaint.Right - ps.rcPaint.Left,
//                    ps.rcPaint.Bottom - ps.rcPaint.Top);
//
//                EndPaint(handleWindow, ref ps);
//                return (IntPtr) 1;
//
//            case (uint) WindowMessages.NCHITTEST:
//                var aa = (uint) WindowStyleFlags.Caption |
//                         (uint) WindowStyleFlags.Visible;
//
//                WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.WindowProc, (IntPtr) aa);
//                return (IntPtr) 1;
//
//            case (uint) WindowMessages.COMMAND:
//
//                var id = wParam.ToInt32();
//                if (id == (int) DialogBoxCommandID.IDOk || id == (int) DialogBoxCommandID.IDCancel)
//                {
//                    EndDialog(handleWindow, (IntPtr) id);
//                    return (IntPtr) 1;
//                }
//
//                return (IntPtr) 0;
//        }

            return WinApi.DefWindowProc(handleWindow, message, outValue, inValue);
        }

        protected virtual IntPtr MinMaxSize(IntPtr inValue)
        {
            var minMaxInfo = (MINMAXINFO) Marshal.PtrToStructure(inValue, typeof(MINMAXINFO));


            minMaxInfo.ptMinTrackSize.X = MinWindowSize.x > 0 ? MinWindowSize.x : _defaultMinWindowSize.x;


            minMaxInfo.ptMinTrackSize.Y = MinWindowSize.y > 0 ? MinWindowSize.y : _defaultMinWindowSize.y;

            if ((MinWindowSize.x > 0 && MaxWindowSize.x > MinWindowSize.x) ||
                MaxWindowSize.x > _defaultMinWindowSize.x)
            {
                minMaxInfo.ptMaxTrackSize.X = MaxWindowSize.x;
            }

            if ((MinWindowSize.y > 0 && MaxWindowSize.y > MinWindowSize.y) ||
                MaxWindowSize.y > _defaultMinWindowSize.y)
            {
                minMaxInfo.ptMaxTrackSize.Y = MaxWindowSize.y;
            }

            Marshal.StructureToPtr(minMaxInfo, inValue, false);
            return (IntPtr) 0;
        }

        protected virtual IntPtr Resize(IntPtr inValue)
        {
            var windowRect = WindowManager.GetWindowRect();

            var cursorPosition = new Point(inValue.ToInt32());
            var cursorPositionRelative = CursorManager.GetCursorPositionRelative(cursorPosition);

            if (cursorPositionRelative.Y <= ResizeHandleSize.y)
            {
                if (cursorPositionRelative.X <= ResizeHandleSize.x)
                {
                    return (IntPtr) 13; /*Hit Top Left*/
                }

                if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
                {
                    return (IntPtr) 12; /*Hit Top*/
                }

                return (IntPtr) 14; /*Hit Top Right*/
            }

            if (cursorPositionRelative.Y <= windowRect.height - ResizeHandleSize.y)
            {
                if (cursorPositionRelative.X <= ResizeHandleSize.x)
                {
                    return (IntPtr) 10; /*Hit Left*/
                }

                if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
                {
                    if (cursorPositionRelative.Y <= CaptionHeight)
                    {
                        return (IntPtr) 2; /*Hit Caption*/
                    }

                    return (IntPtr) 0;
                }

                return (IntPtr) 11; /*Hit Right*/
            }

            if (cursorPositionRelative.X <= ResizeHandleSize.x)
            {
                return (IntPtr) 16; /*Hit Bottom Left*/
            }

            if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
            {
                return (IntPtr) 15; /*Hit Bottom*/
            }

            return (IntPtr) 17; /*Hit Bottom Right*/
        }
    }
}