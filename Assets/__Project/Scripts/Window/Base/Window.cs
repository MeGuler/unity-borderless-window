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
        public bool KeepAspectRatio { get; protected set; }

        #endregion

        private bool _isMouseOver;
        private bool _initialized;


        private Vector2 _aspectRatio;

        public TMP_InputField debug;

        protected virtual void Awake()
        {
            SettingMinMaxWindowSize();

            HandleWindow();
            InitializeWindowProcedure();
            SetWindowStyle();
            SetWindowDefaultSize();

            var aspectRatioX = StartWindowSize.x / (float) StartWindowSize.y;
            var aspectRatioY = StartWindowSize.y / (float) StartWindowSize.x;

            _aspectRatio.x = aspectRatioX;
            _aspectRatio.y = aspectRatioY;

//            debug.text += "\n" + _aspectRatio.x + " / " + _aspectRatio.y;
        }

        protected virtual void OnGUI()
        {
            if (!_initialized)
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

        protected void HandleWindow()
        {
            //https://gist.github.com/mattbenic/908483ad0bedbc62ab17
            var threadId = kernel32.GetCurrentThreadId();
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

        protected virtual void InitializeWindowProcedure()
        {
            if (NewWindowProcedure != null) return;

            _initialized = true;

            NewWindowProcedure = WindowProcedure;
            NewWindowProcedurePtr = Marshal.GetFunctionPointerForDelegate(NewWindowProcedure);
            OldWindowProcedurePtr = User32.SetWindowLongPtr(HandledWindow, -4, NewWindowProcedurePtr);
        }

        protected virtual void TerminateWindowProcedure()
        {
            if (NewWindowProcedure == null) return;

            _initialized = false;

            User32.SetWindowLongPtr(HandledWindow, -4, OldWindowProcedurePtr);
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

            if (message == (uint) WindowMessages.NcDestroy || message == (uint) WindowMessages.WindowPosChanging)
            {
                TerminateWindowProcedure();
            }

            if (message == (uint) WindowMessages.GetMinMaxInfo)
            {
                GetMinMaxInfo(lParam);
            }

            if (message == (uint) WindowMessages.Sizing)
            {
                if (KeepAspectRatio)
                {
                    Sizing(wParam, lParam);
                }
            }

            if (message == (uint) WindowMessages.ShowWindow)
            {
                debug.text += WindowMessages.ShowWindow + "\n";
            }

            if (message == (uint) WindowMessages.WindowPosChanged)
            {
                debug.text += WindowMessages.WindowPosChanged + "\n";
            }

            if (message == (uint) WindowMessages.WindowPosChanging)
            {
                debug.text += WindowMessages.WindowPosChanging + "\n";
            }
            
            if (message == (uint) WindowMessages.NcHitTest)
            {
                if (!_isMouseOver)
                {
                    var resize = NonClientHitTest();

                    if (resize != (int) HitTestValues.Client)
                    {
                        return (IntPtr) resize;
                    }
                }
            }


            return User32.CallWindowProc(OldWindowProcedurePtr, handleWindow, message, wParam, lParam);
        }


        #region Procedures

        protected virtual void GetMinMaxInfo(IntPtr inValue)
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

        protected virtual void Sizing(IntPtr wParam, IntPtr lParam)
        {
            var rect = (Rect) Marshal.PtrToStructure(lParam, typeof(Rect));
            var sizingSide = wParam.ToInt32();

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

            Marshal.StructureToPtr(rect, lParam, true);
        }

        protected virtual int NonClientHitTest()
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

//            debug.text += "\n" + "Monitor Width: " + info.MonitorName.Length;
//            debug.text += "\n" + "Monitor Width: " + info.MonitorRect.Width;
//            debug.text += "\n" + "Monitor Height: " + info.MonitorRect.Height;
//            debug.text += "\n" + "StartWindowSize.x: " + StartWindowSize.x;
//            debug.text += "\n" + "StartWindowSize.y: " + StartWindowSize.y;
//            debug.text += "\n" + "centerWindow.x: " + centerWindow.x;
//            debug.text += "\n" + "centerWindow.y: " + centerWindow.y;

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