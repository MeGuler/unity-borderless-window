﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.EventSystems;
using Borderless.Api;
using Borderless.Api.Structures;
using Borderless.Flags;
using TMPro;
using UnityEditor;

namespace Borderless
{
    public class Window : MonoBehaviour
    {
        #region Window Procedure Properties

        public HandleRef HandledWindow { get; protected set; }
        public IntPtr OldWindowProcedurePtr { get; protected set; }
        public IntPtr NewWindowProcedurePtr { get; protected set; }
        public User32.WindowProcedureDelegate NewWindowProcedure { get; protected set; }

        #endregion

        #region Window Settings

        private readonly Vector2Int _defaultMinWindowSize = new Vector2Int(200, 200);

        public Vector4Int ResizeBorderSize { get; protected set; }
        public Vector2Int StartWindowSize { get; protected set; }
        public Vector2Int MinWindowSize { get; protected set; }
        public Vector2Int MaxWindowSize { get; protected set; }
        public int CaptionHeight { get; protected set; }

        #endregion

        private bool _isMouseOver;

        public TMP_InputField debug;

        protected virtual void Awake()
        {
            SettingMinMaxWindowSize();

            InitializeWindowProcedure();
            SetWindowStyle();
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

        #region Window Settings Methods

        protected void SettingMinMaxWindowSize()
        {
            var minWindowSize = MinWindowSize;
            var maxWindowSize = MaxWindowSize;

            //Check Negative 
            minWindowSize.x = minWindowSize.x < 0 ? 0 : minWindowSize.x;
            minWindowSize.y = minWindowSize.y < 0 ? 0 : minWindowSize.y;
            maxWindowSize.x = maxWindowSize.x < 0 ? 0 : maxWindowSize.x;
            maxWindowSize.y = maxWindowSize.y < 0 ? 0 : maxWindowSize.y;

            //Check Default min
            minWindowSize.x = minWindowSize.x > 0 ? minWindowSize.x : _defaultMinWindowSize.x;
            minWindowSize.y = minWindowSize.y > 0 ? minWindowSize.y : _defaultMinWindowSize.y;

            MinWindowSize = minWindowSize;
            MaxWindowSize = maxWindowSize;
        }

        #endregion

        #region Window Procedure Methods

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
                MinMaxSize(lParam);
            }

            if (message == (uint) WindowMessages.SIZING)
            {
            }

            if (message == (uint) WindowMessages.NCHITTEST)
            {
                if (!_isMouseOver)
                {
                    var resize = Resize();

                    if (resize != (int) HitTestValues.Client)
                    {
                        return (IntPtr) resize;
                    }
                }
            }

            return User32.CallWindowProc(OldWindowProcedurePtr, handleWindow, message, wParam, lParam);
        }


        #region Procedures

        protected virtual void MinMaxSize(IntPtr inValue)
        {
            var minMaxInfo = (MinMaxInfo) Marshal.PtrToStructure(inValue, typeof(MinMaxInfo));

            minMaxInfo.ptMinTrackSize.X = MinWindowSize.x;
            minMaxInfo.ptMinTrackSize.Y = MinWindowSize.y;

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
        }

        protected virtual int Resize()
        {
            var cursorPositionFlag = Cursor.GetCursorPositionFlag(this);

            return (int) cursorPositionFlag;
        }

        #endregion

        #endregion

        #region Window Manager Methods

        protected virtual void ShowWindow(ShowWindowCommands showWindowStatus)
        {
            User32.ShowWindow(HandledWindow.Handle, (int) showWindowStatus);
        }

        protected virtual void SetWindowStyle()
        {
            User32.SetWindowLongPtr(HandledWindow, (int) WindowLongIndex.Style, (IntPtr) WindowStyleFlags.Visible);

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

            User32.SetWindowPos
            (
                HandledWindow.Handle,
                0,
                (int) rect.x,
                (int) rect.y,
                (int) rect.width,
                (int) rect.height, message
            );
        }

        public UnityEngine.Rect GetWindowRect()
        {
            User32.GetWindowRect(HandledWindow.Handle, out var windowRect);

            var rect = new UnityEngine.Rect
            {
                x = windowRect.Left,
                y = windowRect.Top,
                width = windowRect.Right - windowRect.Left,
                height = windowRect.Bottom - windowRect.Top
            };

            return rect;
        }

        public Rect GetWindowRectPoints()
        {
            User32.GetWindowRect(HandledWindow.Handle, out var windowRect);
            return windowRect;
        }

        public WindowPlacement GetWindowPlacement()
        {
            var placement = new WindowPlacement();
            placement.Length = Marshal.SizeOf(placement);
            User32.GetWindowPlacement(HandledWindow.Handle, ref placement);
            return placement;
        }

        #endregion
    }
}