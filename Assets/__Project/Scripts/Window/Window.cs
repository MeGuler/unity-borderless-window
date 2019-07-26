using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Borderless.Api;
using Borderless.Api.Structures;
using Borderless.Flags;
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
        protected User32.WindowProcedureDelegate NewWindowProcedure;

        #endregion

        #region Window Settings

        private readonly Vector2Int _defaultMinWindowSize = new Vector2Int(200, 200);

        protected Vector4Int ResizeBorderSize;
        protected Vector2Int MinWindowSize;
        protected Vector2Int MaxWindowSize;
        protected Vector4Int ResizeHandleSize;
        protected int CaptionHeight;

        #endregion

        private bool _isMouseOver;

        #region Will Be Delete

        public TMP_InputField debug;

        #endregion

        protected virtual void Awake()
        {
            InitializeWindowProcedure();
            UpdateStyle();
            RefreshWindow();
        }

        protected virtual void OnGUI()
        {
            if (HandledWindow.Handle == IntPtr.Zero)
            {
                InitializeWindowProcedure();
            }

            CheckMouseOver();
        }

        protected virtual void CheckMouseOver()
        {
            _isMouseOver = false;

            //Check 3D
            if (Camera.main != null)
            {
                var origin = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
                var direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
                _isMouseOver = Physics.Raycast(origin, direction, out var hit, 100, Physics.DefaultRaycastLayers);

                if (_isMouseOver)
                {
                    return;
                }
            }

            //Check UI 
            var @event = EventSystem.current;

            if (@event == null)
            {
                return;
            }

            var pointerEventData = new PointerEventData(@event)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            _isMouseOver = results.Count > 0;
        }


        #region WindowProcedure

        protected virtual void InitializeWindowProcedure()
        {
            if (NewWindowProcedure != null) return;

            HandledWindow = new HandleRef(null, User32.GetForegroundWindow());
            NewWindowProcedure = WindowProcedure;
            NewWindowProcedurePtr = Marshal.GetFunctionPointerForDelegate(NewWindowProcedure);
            OldWindowProcedurePtr = User32.SetWindowLongPtr(HandledWindow, -4, NewWindowProcedurePtr);
        }

        protected virtual void TerminateWindowProcedure()
        {
            if (NewWindowProcedure == null) return;

            User32.SetWindowLongPtr(HandledWindow, -4, OldWindowProcedurePtr);
            HandledWindow = new HandleRef(null, IntPtr.Zero);
            OldWindowProcedurePtr = IntPtr.Zero;
            NewWindowProcedurePtr = IntPtr.Zero;
            NewWindowProcedure = null;
        }

        ~Window()
        {
            TerminateWindowProcedure();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected virtual IntPtr WindowProcedure(IntPtr handleWindow, uint message, IntPtr wParam, IntPtr lParam)
        {
            //todo: Check ForgroundWindow

            if (message == (uint) WindowMessages.NCDESTROY || message == (uint) WindowMessages.WINDOWPOSCHANGING)
            {
                TerminateWindowProcedure();
            }

            if (message == (uint) WindowMessages.GETMINMAXINFO)
            {
                return MinMaxSize(lParam);
            }

            if (message == (uint) WindowMessages.NCLBUTTONDOWN)
            {
            }

            if (message == (uint) WindowMessages.NCLBUTTONUP)
            {
            }

            if (message == (uint) WindowMessages.NCHITTEST)
            {
                if (!_isMouseOver)
                {
                    var resize = Resize(handleWindow, message, wParam, lParam);

                    if (resize != (int) CursorPositionFlags.Client)
                    {
                        return (IntPtr) resize;
                    }
                }
            }

            return User32.CallWindowProc(OldWindowProcedurePtr, handleWindow, message, wParam, lParam);
        }

        protected virtual IntPtr MinMaxSize(IntPtr inValue)
        {
            var minMaxInfo = (MinMaxInfo) Marshal.PtrToStructure(inValue, typeof(MinMaxInfo));


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

        protected virtual int Resize(IntPtr handleWindow, uint message, IntPtr outValue, IntPtr inValue)
        {
            var cursorPositionFlag = GetCursorPositionFlag();

            switch (cursorPositionFlag)
            {
                case CursorPositionFlags.Caption:
                    return 2;
                case CursorPositionFlags.Left:
                    return 10;
                case CursorPositionFlags.Right:
                    return 11;
                case CursorPositionFlags.Top:
                    return 12;
                case CursorPositionFlags.TopLeft:
                    return 13;
                case CursorPositionFlags.TopRight:
                    return 14;
                case CursorPositionFlags.Bottom:
                    return 15;
                case CursorPositionFlags.BottomLeft:
                    return 16;
                case CursorPositionFlags.BottomRight:
                    return 17;
                case CursorPositionFlags.Client:
                default:
                    return 19;
            }


//            var windowRect = GetWindowRect();
//
//            var cursorPosition = new Point(inValue.ToInt32());
//            var cursorPositionRelative = CursorManager.GetCursorPositionRelative(cursorPosition);
//
//
//            if (cursorPositionRelative.Y <= ResizeHandleSize.y)
//            {
//                if (cursorPositionRelative.X <= ResizeHandleSize.x)
//                {
//                    return (IntPtr) 13; /*Hit Top Left*/
//                }
//
//                if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
//                {
//                    return (IntPtr) 12; /*Hit Top*/
//                }
//
//                return (IntPtr) 14; /*Hit Top Right*/
//            }
//
//            if (cursorPositionRelative.Y <= windowRect.height - ResizeHandleSize.y)
//            {
//                if (cursorPositionRelative.X <= ResizeHandleSize.x)
//                {
//                    debug.text = "window: " + handleWindow + "\n" +
//                                 "message: " + (WindowMessages) message + "\n" +
//                                 "wParam: " + outValue + "\n" +
//                                 "lParam: " + inValue;
//                    return (IntPtr) 10; /*Hit Left*/
//                }
//
//                if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
//                {
//                    if (cursorPositionRelative.Y <= CaptionHeight)
//                    {
//                        return (IntPtr) 2; /*Hit Caption*/
//                    }
//
//                    return (IntPtr) 19;
//                }
//
//                return (IntPtr) 11; /*Hit Right*/
//            }
//
//            if (cursorPositionRelative.X <= ResizeHandleSize.x)
//            {
//                return (IntPtr) 16; /*Hit Bottom Left*/
//            }
//
//            if (cursorPositionRelative.X < windowRect.width - ResizeHandleSize.z)
//            {
//                return (IntPtr) 15; /*Hit Bottom*/
//            }
//
//            return (IntPtr) 17; /*Hit Bottom Right*/
        }

        #endregion

        protected virtual void ShowWindow(WindowShowStyle windowStatus)
        {
            var activeWindow = User32.GetForegroundWindow();

            var status = (int) windowStatus;

            User32.ShowWindow(activeWindow, status);
        }

        protected virtual void UpdateStyle()
        {
            User32.SetWindowLongPtr(HandledWindow, (int) WindowLongIndex.Style, (IntPtr) WindowStyleFlags.Visible);
//            WinApi.SetWindowLongPtr
//            (
//                HandledWindow,
//                (int) WindowLongIndex.ExtendedStyle,
//                (IntPtr)( WindowStyleFlags.ExtendedLayered | WindowStyleFlags.ExtendedTransparent)
//            );
//            
            var margins = new Margins
            {
                leftWidth = -1
            };

            DwmApi.DwmExtendFrameIntoClientArea(HandledWindow.Handle, ref margins);
        }

        protected virtual void RefreshWindow()
        {
            var rect = GetWindowRect();

            const uint message = (uint)
            (
                SetWindowPosFlags.FrameChanged
            );

//            var window = WinApi.GetForegroundWindow();

            User32.SetWindowPos(HandledWindow.Handle, 0, (int) rect.x, (int) rect.y, (int) rect.width,
                (int) rect.height, message);
        }

        public UnityEngine.Rect GetWindowRect()
        {
            var activeWindow = User32.GetForegroundWindow();
            User32.GetWindowRect(activeWindow, out var windowRect);

            var rect = new UnityEngine.Rect
            {
                x = windowRect.Left,
                y = windowRect.Top,
                width = windowRect.Right - windowRect.Left,
                height = windowRect.Bottom - windowRect.Top
            };

            return rect;
        }


        #region Cursor

        public Rect GetWindowRectPoints()
        {
            var activeWindow = User32.GetActiveWindow();
            User32.GetWindowRect(activeWindow, out var windowRect);
            return windowRect;
        }

        public static Vector2 GetCursorPosition()
        {
            User32.GetCursorPosition(out var point);
            return new Vector2(point.X, point.Y);
        }

        public CursorPositionFlags GetCursorPositionFlag()
        {
            var mousePosition = GetCursorPosition();

            return GetSpecificCursorPositionFlags(mousePosition);
        }


        public CursorPositionFlags GetSpecificCursorPositionFlags(Vector2 cursorPoint)
        {
            var rectPoints = GetWindowRectPoints();
            var mousePosition = cursorPoint;
//            var borderSize = ResizeBorderSize;

            CursorPositionFlags cursorPositionFlags;

            //Left
            if (MathExtension.IsBetween
                (
                    mousePosition.x,
                    rectPoints.Left,
                    rectPoints.Left + ResizeBorderSize.x,
                    true
                )
            )
            {
                cursorPositionFlags = CursorPositionFlags.Left;

                if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + ResizeBorderSize.y,
                    true))
                {
                    cursorPositionFlags = CursorPositionFlags.TopLeft;
                }
                else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - ResizeBorderSize.w,
                    rectPoints.Bottom,
                    true))
                {
                    cursorPositionFlags = CursorPositionFlags.BottomLeft;
                }
            }
            //Right
            else if (MathExtension.IsBetween
                (
                    mousePosition.x,
                    rectPoints.Right - ResizeBorderSize.z,
                    rectPoints.Right,
                    true
                )
            )
            {
                cursorPositionFlags = CursorPositionFlags.Right;

                if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + ResizeBorderSize.y,
                    true))
                {
                    cursorPositionFlags = CursorPositionFlags.TopRight;
                }
                else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - ResizeBorderSize.w,
                    rectPoints.Bottom,
                    true))
                {
                    cursorPositionFlags = CursorPositionFlags.BottomRight;
                }
            }
            //Top
            else if (MathExtension.IsBetween
                (
                    mousePosition.y,
                    rectPoints.Top,
                    rectPoints.Top + ResizeBorderSize.y,
                    true
                )
            )
            {
                cursorPositionFlags = CursorPositionFlags.Top;
            }
            else if (MathExtension.IsBetween
                (
                    mousePosition.y,
                    rectPoints.Top + ResizeBorderSize.y,
                    rectPoints.Top + CaptionHeight,
                    false
                )
            )
            {
                cursorPositionFlags = CursorPositionFlags.Caption;
            }
            //Bottom
            else if (MathExtension.IsBetween
                (
                    mousePosition.y,
                    rectPoints.Bottom - ResizeBorderSize.w,
                    rectPoints.Bottom,
                    true
                )
            )
            {
                cursorPositionFlags = CursorPositionFlags.Bottom;
            }
            //Main
            else
            {
                cursorPositionFlags = CursorPositionFlags.Client;
            }

            return cursorPositionFlags;
        }

        #endregion
    }
}