using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using TMPro;
using UnityEngine;
using XGraphic = System.Drawing.Graphics;

namespace Borderless
{
    public class Window : MonoBehaviour
    {
        #region Window Procedure

        protected HandleRef _handledWindow;
        protected IntPtr _oldWindowProcessesPtr;
        protected IntPtr _newWindowProcessesPtr;
        protected WndProcDelegate _newWindowProcesses;

        protected bool ClickThrough = true;
        protected bool PreviousClickThrough = true;

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
//            WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.Style, (IntPtr) WindowStyleFlags.Visible);
            WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.Style,
                (IntPtr) (WindowStyleFlags.Visible));
            WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.ExtendedStyle,
                (IntPtr) (WindowStyleFlags.ExtendedNoParentNotify | WindowStyleFlags.ExtendedTransparent));

            const uint message = (int) SetWindowPosFlags.FrameChanged |
                                 (int) SetWindowPosFlags.NoMove |
                                 (int) SetWindowPosFlags.NoSize |
                                 (int) SetWindowPosFlags.NoRePosition |
                                 (int) SetWindowPosFlags.NoZOrder |
                                 (int) SetWindowPosFlags.NoOwnerZOrder;

            WinApi.SetWindowPos(_handledWindow.Handle, 0, 0, 0, 0, 0, message);
        }


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected virtual IntPtr WindowProcedure(IntPtr handleWindow, uint message, IntPtr outValue, IntPtr inValue)
        {
//            Debug.Log(((WindowMessages) message).ToString());


            if (message == (uint) WindowMessages.NCDESTROY || message == (uint) WindowMessages.WINDOWPOSCHANGING)
            {
                TerminateWindowProcedure();
            }


            if (message == (uint) WindowMessages.GETMINMAXINFO)
            {
                if (ClickThrough)
                {
                    return MinMaxSize(inValue);
                }
            }

            if (message == (uint) WindowMessages.NCHITTEST)
            {
                if (ClickThrough)
                {
                    return Resize(inValue);
                }
            }

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

        protected virtual void ShowWindow(WindowShowStyle windowStatus)
        {
            var activeWindow = WinApi.GetActiveWindow();

            var status = (int) windowStatus;

            WinApi.ShowWindow(activeWindow, status);
        }
    }
}


//private bool m_aeroEnabled;

//private bool CheckAeroEnabled()
//{
//if (Environment.OSVersion.Version.Major < 6) return false;
//
//var aeroEnabled = 0;
//WinApi.DwmIsCompositionEnabled(ref aeroEnabled);
//return aeroEnabled == 1;
//}

//if (message == (uint) WindowMessages.NCPAINT)
//            {
//                debug.text += "\n" + "paint / " + m_aeroEnabled;
////                if (m_aeroEnabled)
//                {
//                    var v = 2;
//                    WinApi.DwmSetWindowAttribute(_handledWindow.Handle, 2, ref v, 4);
//                    var margins = new MARGINS()
//                    {
//                        bottomHeight = 5,
//                        leftWidth = 5,
//                        rightWidth = 5,
//                        topHeight = 5
//                    };
//                    WinApi.DwmExtendFrameIntoClientArea(_handledWindow.Handle, ref margins);
//                }
//            }