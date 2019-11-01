using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Window.Apis;
using Window.Structures;
using Window.Flags;


namespace Window
{
    public class Window : MonoBehaviour
    {
        private readonly Vector2Int _defaultMinWindowSize = new Vector2Int(200, 200);
        public WindowProcedure windowProcedure;
        public HandleRef HandledWindow => windowProcedure.HandledWindow;


        #region Window Settings

        [Header("Window Settings")] public int captionHeight;

        public bool keepAspectRatio;
        public Vector2Int defaultWindowSize;
        public Vector2Int minWindowSize;
        public Vector2Int maxWindowSize;
        public Vector4Int resizeBorderSize;

        #endregion

        #region Maximize Icon

        [Header("Maximize Icon Settings")] public Image maximizeImage;
        public Sprite maximizeIconNormal;
        public Sprite maximizeIconMaximized;

        #endregion


        [Header("Debug")] public TMP_InputField debug;

        private Vector2 _aspectRatio;


        protected virtual void Awake()
        {
            SetMinMaxWindowSize();

            windowProcedure = new WindowProcedure(resizeBorderSize, captionHeight);
            windowProcedure.Sizing += Sizing;
            windowProcedure.GetMinMaxInfo += GetMinMaxInfo;
            windowProcedure.HShellGetMinRect += HShellGetMinRect;

            SetWindowStyle();
            SetWindowDefaultSize();

            _aspectRatio.x = defaultWindowSize.x / (float) defaultWindowSize.y;
            _aspectRatio.y = defaultWindowSize.y / (float) defaultWindowSize.x;
        }

        protected virtual void OnGUI()
        {
            CheckMouseOver();
        }

        protected virtual void SetMinMaxWindowSize()
        {
            var minWindowSize = this.minWindowSize;
            var maxWindowSize = this.maxWindowSize;

            //Check Negative 
            minWindowSize.x = minWindowSize.x < 0 ? 0 : minWindowSize.x;
            minWindowSize.y = minWindowSize.y < 0 ? 0 : minWindowSize.y;
            maxWindowSize.x = maxWindowSize.x < 0 ? 0 : maxWindowSize.x;
            maxWindowSize.y = maxWindowSize.y < 0 ? 0 : maxWindowSize.y;

            //Check Default min
            minWindowSize.x = minWindowSize.x > 0 ? minWindowSize.x : _defaultMinWindowSize.x;
            minWindowSize.y = minWindowSize.y > 0 ? minWindowSize.y : _defaultMinWindowSize.y;

            this.minWindowSize = minWindowSize;
            this.maxWindowSize = maxWindowSize;
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

        #region Window Button Methods

        public virtual void Minimize()
        {
            var placement = GetWindowPlacement();


            if (placement.windowShowCommand == WindowShowCommands.ShowMinimized)
            {
                placement.windowShowCommand = WindowShowCommands.Restore;
            }
            else
            {
                placement.windowShowCommand = WindowShowCommands.ShowMinimized;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }

        public virtual void Maximize()
        {
            var placement = GetWindowPlacement();

            if (placement.windowShowCommand == WindowShowCommands.Maximize)
            {
                placement.windowShowCommand = WindowShowCommands.Restore;
            }
            else
            {
                placement.windowShowCommand = WindowShowCommands.Maximize;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }

        public virtual void ExitApplication()
        {
            Application.Quit();
        }

        #endregion

        #region Window Procedures

        protected virtual void GetMinMaxInfo(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            var minMaxInfo = (MinMaxInfo) Marshal.PtrToStructure(secondParameter, typeof(MinMaxInfo));

            minMaxInfo.ptMinTrackSize.X = minWindowSize.x;
            minMaxInfo.ptMinTrackSize.Y = minWindowSize.y;

            if ((minWindowSize.x > 0 && maxWindowSize.x > minWindowSize.x) ||
                maxWindowSize.x > _defaultMinWindowSize.x)
            {
                minMaxInfo.ptMaxTrackSize.X = maxWindowSize.x;
            }

            if ((minWindowSize.y > 0 && maxWindowSize.y > minWindowSize.y) ||
                maxWindowSize.y > _defaultMinWindowSize.y)
            {
                minMaxInfo.ptMaxTrackSize.Y = maxWindowSize.y;
            }


            Marshal.StructureToPtr(minMaxInfo, secondParameter, false);
        }

        protected virtual void Sizing(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
        {
            if (!keepAspectRatio) return;

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

        protected virtual void HShellGetMinRect(IntPtr handledWindow, IntPtr firstParameter, IntPtr secondParameter)
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

        #region Set

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
            centerWindow.x = (info.MonitorRect.Width - defaultWindowSize.x) / 2f;
            centerWindow.y = (info.MonitorRect.Height - defaultWindowSize.y) / 2f;

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
                defaultWindowSize.x,
                defaultWindowSize.y,
                message
            );
        }

        #endregion

        #region Get

        public virtual MonitorInfo GetMonitorInfo()
        {
            var monitor = User32.MonitorFromWindow(HandledWindow.Handle, (int) MonitorFlag.DefaultToNearest);

            var info = new MonitorInfo
            {
                Size = Marshal.SizeOf(typeof(MonitorInfo))
            };

            User32.GetMonitorInfo(monitor, ref info);
            return info;
        }

        public virtual UnityEngine.Rect GetWindowRect()
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

        public virtual Rect GetWindowRectPoints()
        {
            User32.GetWindowRect(HandledWindow.Handle, out var windowRect);
            return windowRect;
        }

        public virtual WindowPlacement GetWindowPlacement()
        {
            var placement = new WindowPlacement();
            placement.Length = Marshal.SizeOf(placement);
            User32.GetWindowPlacement(HandledWindow.Handle, ref placement);
            return placement;
        }

        #endregion

        #endregion
    }
}