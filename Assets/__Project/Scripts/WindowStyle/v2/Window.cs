using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borderless
{
    public class Window : MonoBehaviour
    {
        #region Window Procedure

        protected HandleRef HandledWindow;
        protected IntPtr OldWindowProcedurePtr;
        protected IntPtr NewWindowProcedurePtr;
        protected WndProcDelegate NewWindowProcedure;

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


        public TMP_InputField debug;

        protected virtual void Start()
        {
            InitializeWindowProcedure();
            UpdateStyle();
            var window = WinApi.GetActiveWindow();

            var rect = GetWindowRect();

//            WinApi.MoveWindow(window, (int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height, true);

            const uint message = (uint) (SetWindowPosFlags.FrameChanged);

            WinApi.SetWindowPos(window, 0, (int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height, message);
        }

        public static Rect GetWindowRect()
        {
            var activeWindow = WinApi.GetActiveWindow();
            WinApi.GetWindowRect(activeWindow, out var windowRect);

            var rect = new Rect
            {
                x = windowRect.Left,
                y = windowRect.Top,
                width = windowRect.Right - windowRect.Left,
                height = windowRect.Bottom - windowRect.Top
            };

            return rect;
        }

        protected virtual void OnGUI()
        {
            if (HandledWindow.Handle == IntPtr.Zero)
            {
                InitializeWindowProcedure();
            }
        }

        protected virtual void InitializeWindowProcedure()
        {
            if (NewWindowProcedure != null) return;

            HandledWindow = new HandleRef(null, WinApi.GetActiveWindow());
            NewWindowProcedure = WindowProcedure;
            NewWindowProcedurePtr = Marshal.GetFunctionPointerForDelegate(NewWindowProcedure);
            OldWindowProcedurePtr = WinApi.SetWindowLongPtr(HandledWindow, -4, NewWindowProcedurePtr);
        }

        protected virtual void TerminateWindowProcedure()
        {
            if (NewWindowProcedure == null) return;

            WinApi.SetWindowLongPtr(HandledWindow, -4, OldWindowProcedurePtr);
            HandledWindow = new HandleRef(null, IntPtr.Zero);
            OldWindowProcedurePtr = IntPtr.Zero;
            NewWindowProcedurePtr = IntPtr.Zero;
            NewWindowProcedure = null;
        }

        ~Window()
        {
            TerminateWindowProcedure();
        }

        protected virtual void UpdateStyle()
        {
            WinApi.SetWindowLongPtr(HandledWindow, (int) WindowLongIndex.Style, (IntPtr) WindowStyleFlags.Visible);
        }


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected virtual IntPtr WindowProcedure(IntPtr handleWindow, uint message, IntPtr outValue, IntPtr inValue)
        {
            var resize = IntPtr.Zero;

            if (message)
            {
                
            }
            
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
                resize = Resize(handleWindow, message, outValue, inValue);

                debug.text = resize.ToString();
//                return resize;
            }






            return WinApi.CallWindowProc(OldWindowProcedurePtr, handleWindow, message, outValue, inValue);
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

        protected virtual IntPtr Resize(IntPtr handleWindow, uint message, IntPtr outValue, IntPtr inValue)
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
                    debug.text = "window: " + handleWindow + "\n" +
                                 "message: " + (WindowMessages) message + "\n" +
                                 "wParam: " + outValue + "\n" +
                                 "lParam: " + inValue;
                    return (IntPtr) 10; /*Hit Left*/
                }

                if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
                {
                    if (cursorPositionRelative.Y <= CaptionHeight)
                    {
                        return (IntPtr) 2; /*Hit Caption*/
                    }

                    return (IntPtr) 19;
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

        protected virtual void RefreshWindow()
        {
            const uint message = (uint)
            (
                SetWindowPosFlags.FrameChanged |
                SetWindowPosFlags.NoMove |
                SetWindowPosFlags.NoSize |
                SetWindowPosFlags.NoZOrder |
                SetWindowPosFlags.NoOwnerZOrder
            );

            WinApi.SetWindowPos(HandledWindow.Handle, 0, 0, 0, 0, 0, message);
        }
    }
}


//            if (message == (uint) WindowMessages.NCLBUTTONDOWN)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//
//                PointerEventData = new PointerEventData(EventSystem.current);
//                PointerEventData.position = Input.mousePosition;
//                PointerEventData.button = PointerEventData.InputButton.Left;
////                var pointerCurrentRaycast = PointerEventData.pointerCurrentRaycast;
//
//
//                RaycastResult.Clear();
//
//                raycaster.Raycast(PointerEventData, RaycastResult);
//
//                foreach (var raycastResult in RaycastResult)
//                {
//                    debug.text += "\n" + "name: " + raycastResult.gameObject.name;
//                }
//            }
//
//            if (message == (uint) WindowMessages.SIZING)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//            }

//            if (message == (uint) WindowMessages.SIZE)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//            }
//
//            if (message == (uint) WindowMessages.SIZECLIPBOARD)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//            }
//
//            if (message == (uint) WindowMessages.NCCALCSIZE)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//            }
//
//            if (message == (uint) WindowMessages.EXITSIZEMOVE)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//            }

//            if (message == (uint) WindowMessages.ENTERSIZEMOVE)
//            {
//                debug.text = "window: " + handleWindow + "\n" +
//                             "message: " + (WindowMessages) message + "\n" +
//                             "wParam: " + outValue + "\n" +
//                             "lParam: " + inValue;
//            }