using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Borderless.Api;
using Borderless.Api.Structures;
using Borderless.Flags;
using TMPro;
using UnityEngine.UI;


namespace Borderless
{
    public class TestWindow : MonoBehaviour
    {
        public WindowProcedure windowProcedure;
        public HandleRef HandledWindow => windowProcedure.HandledWindow;

        #region Window Settings

        private readonly Vector2Int _defaultMinWindowSize = new Vector2Int(200, 200);

        public Vector4Int ResizeBorderSize { get; protected set; }
        public Vector2Int StartWindowSize { get; protected set; }
        public Vector2Int MinWindowSize { get; protected set; }
        public Vector2Int MaxWindowSize { get; protected set; }
        public int CaptionHeight { get; protected set; }

        public bool KeepAspectRatio { get; protected set; }

        #endregion

        [Header("Maximize Icon Settings")] 
        public Image maximizeImage;
        public Sprite maximizeIconNormal;
        public Sprite maximizeIconMaximized;

        private Vector2 _aspectRatio;

        public TMP_InputField debug;

        protected virtual void Awake()
        {
            SettingMinMaxWindowSize();

            windowProcedure = new WindowProcedure(ResizeBorderSize, CaptionHeight);
            windowProcedure.Sizing += Sizing;
            windowProcedure.GetMinMaxInfo += GetMinMaxInfo;
            windowProcedure.HShellGetMinRect += HShellGetMinRect;

            SetWindowStyle();
            SetWindowDefaultSize();

            _aspectRatio.x = StartWindowSize.x / (float) StartWindowSize.y;
            _aspectRatio.y = StartWindowSize.y / (float) StartWindowSize.x;
        }

        protected virtual void OnGUI()
        {
//            if (!windowProcedure.Initialized)
//            {
//                windowProcedure.InitializeWindowProcedure();
//            }

            CheckMouseOver();
        }

        protected virtual void CheckMouseOver()
        {
            windowProcedure.IgnoreNonClientHitTest = false;

            //Check 3D
            if (Camera.main != null)
            {
                var origin = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
                var direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
                windowProcedure.IgnoreNonClientHitTest =
                    Physics.Raycast(origin, direction, out var hit, 100, Physics.DefaultRaycastLayers);

                if (windowProcedure.IgnoreNonClientHitTest)
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

            windowProcedure.IgnoreNonClientHitTest = results.Count > 0;
        }

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

        
        #region Window Processes

           protected virtual void GetMinMaxInfo(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            var minMaxInfo = (MinMaxInfo) Marshal.PtrToStructure(secondParameter, typeof(MinMaxInfo));

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


            Marshal.StructureToPtr(minMaxInfo, secondParameter, false);
        }

        protected virtual void Sizing(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            var rect = (Rect) Marshal.PtrToStructure(secondParameter, typeof(Rect));
            var sizingSide = firstParameter.ToInt32();

            if (sizingSide == (int) SizingWindowSide.Left || sizingSide == (int) SizingWindowSide.Right)
            {
                //Left or right resize -> adjust height (bottom)
                rect.Bottom = rect.Top + (int) (rect.Width / _aspectRatio.x);
            }
            else if (sizingSide == (int) SizingWindowSide.Top || sizingSide == (int) SizingWindowSide.Bottom)
            {
                //Up or down resize -> adjust width (right)
                rect.Right = rect.Left + (int) (rect.Height / _aspectRatio.y);
            }
            else if (sizingSide == (int) SizingWindowSide.BottomRight ||
                     sizingSide == (int) SizingWindowSide.BottomLeft)
            {
                //Lower-right corner resize -> adjust height (could have been width)
                rect.Bottom = rect.Top + (int) (rect.Width / _aspectRatio.x);
            }
            else if (sizingSide == (int) SizingWindowSide.TopLeft)
            {
                //Upper-left corner -> adjust width (could have been height)
                rect.Left = rect.Right - (int) (rect.Height / _aspectRatio.y);
            }
            else if (sizingSide == (int) SizingWindowSide.TopRight)
            {
                //Upper-right corner -> adjust width (could have been height)
                rect.Right = rect.Left + (int) (rect.Height / _aspectRatio.y);
            }

            Marshal.StructureToPtr(rect, secondParameter, true);
        }

        private void HShellGetMinRect(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            var flag = (HShellGetMinRectFlags) firstParameter;

            switch (flag)
            {
                case HShellGetMinRectFlags.Restore:
                    maximizeImage.sprite = maximizeIconNormal;
                    break;
                case HShellGetMinRectFlags.Minimize:
                    break;
                case HShellGetMinRectFlags.Maximize:
                    maximizeImage.sprite = maximizeIconMaximized;
                    break;
            }
        }

        #endregion
        
        #region Window Style Methods

        protected virtual void ShowWindow(WindowShowCommands windowShowStatus)
        {
            User32.ShowWindow(HandledWindow.Handle, (int) windowShowStatus);
        }

        protected virtual void SetWindowStyle()
        {
            User32.SetWindowLongPtr(HandledWindow, (int) WindowLongIndex.Style, (IntPtr) WindowStyleFlags.Visible);

//            var margins = new Margins
//            {
//                leftWidth = -1
//            };

//            DwmApi.DwmExtendFrameIntoClientArea(HandledWindow.Handle, ref margins);
        }

        protected virtual void SetWindowDefaultSize()
        {
            var info = GetMonitorInfo();

            Vector2 centerWindow;
            centerWindow.x = (info.MonitorRect.Width - StartWindowSize.x) / 2f;
            centerWindow.y = (info.MonitorRect.Height - StartWindowSize.y) / 2f;

            const uint message = (uint)
            (
                SetWindowPosFlags.FrameChanged
            );

            User32.SetWindowPos
            (
                HandledWindow.Handle,
                0,
                (int) centerWindow.x,
                (int) centerWindow.y,
                StartWindowSize.x,
                StartWindowSize.y,
                message
            );
        }

        public MonitorInfo GetMonitorInfo()
        {
            var monitor = User32.MonitorFromWindow(HandledWindow.Handle, (int) MonitorFlag.DefaultToNearest);

            var info = new MonitorInfo();
            info.Size = Marshal.SizeOf(typeof(MonitorInfo));
            User32.GetMonitorInfo(monitor, ref info);
            return info;
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