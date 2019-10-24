using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Borderless.Api;
using Borderless.Flags;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borderless.Deprecated
{
    [Obsolete("This usage has been deprecated.", false)]
    public class WindowStyle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static WindowStyle Instance;

        [Header("Window Settings")] public Vector4Int borderSize;
        public Vector2Int aspectRatio;
        public Vector2Int defaultWindowSize;
        public Vector2Int minWindowSize;
        public Vector2Int maxWindowSize;


        public bool keepAspectRatio;
        public bool resizable = true;
        public bool bordered = false;
        public bool maximized = false;
        public bool visible = true;
        public bool overlapped = false;
        public bool caption = false;
        public bool systemMenu = false;
        public bool minimizeBox = false;
        public bool maximizeBox = false;

        [Header("Cursors")] public List<CursorData> cursors = new List<CursorData>();


        [Header("Window Icons")] public Image maximizeIcon;

        [Header("Window Icons Sprites")] public Sprite maximizeIconMaximized;
        public Sprite maximizeIconNotMaximized;

        private bool _isMouseDown;

        private string _currentCursorName;
        private string _previousCursorName;

        private Vector2 _beginningCursorEdgeDistance;
        private HitTestValues _mouseDownCursorFlag;
        private Rect _lastScreenRect;

        public GameObject blackPanel;


        #region Window Procedure

        protected HandleRef HandledWindow;
        protected IntPtr OldWindowProcedurePtr;
        protected IntPtr NewWindowProcedurePtr;
        protected User32.WindowProcedureDelegate NewWindowProcedure;
        public TMP_InputField debug;

        #endregion


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

        ~WindowStyle()
        {
            TerminateWindowProcedure();
        }

        private int i;

        protected virtual IntPtr WindowProcedure(IntPtr handleWindow, uint message, IntPtr wParam, IntPtr lParam)
        {
            //todo: Check ForgroundWindow
//        i++;
//        debug.text = "Run" + i;

            if (message == (uint) WindowMessages.NcDestroy || message == (uint) WindowMessages.WindowPosChanging)
            {
                TerminateWindowProcedure();
            }

            if (message == (uint) WindowMessages.Size)
            {
                debug.text = ((WindowMessages) message).ToString();
            }

            if (message == (uint) WindowMessages.SizeClipboard)
            {
                debug.text = ((WindowMessages) message).ToString();
            }

            if (message == (uint) WindowMessages.NcCalcSize)
            {
                debug.text = ((WindowMessages) message).ToString();
            }

            if (message == (uint) WindowMessages.ExitSizeMove)
            {
                debug.text = ((WindowMessages) message).ToString();
            }

            if (message == (uint) WindowMessages.EnterSizeMove)
            {
                debug.text = ((WindowMessages) message).ToString();
            }

            if (message == (uint) WindowMessages.Sizing)
            {
                debug.text = ((WindowMessages) message).ToString();
            }

//        debug.text = ((WindowMessages) message).ToString();

            return User32.CallWindowProc(OldWindowProcedurePtr, handleWindow, message, wParam, lParam);
        }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            WindowManager.Init
            (
                borderSize,
                aspectRatio,
                minWindowSize,
                maxWindowSize,
                keepAspectRatio,
                resizable,
                bordered,
                maximized,
                visible,
                overlapped,
                caption,
                systemMenu,
                minimizeBox,
                maximizeBox
            );

            ResetWindowSize();
        }

        private void OnGUI()
        {
            if (HandledWindow.Handle == IntPtr.Zero)
            {
                InitializeWindowProcedure();
            }

            if (WindowManager.Resizable && !WindowManager.Bordered && !WindowManager.Maximized)
            {
                if (_isMouseDown)
                {
                    WindowManager.WindowSizeChange(_beginningCursorEdgeDistance, _mouseDownCursorFlag);
                }
                else
                {
                    SetCursorFlagIcon();
                }
            }
        }

        private void SetCursorFlagIcon()
        {
            var cursorPositionFlags = CursorManager.GetCursorPositionFlag();

            switch (cursorPositionFlags)
            {
                case HitTestValues.BottomBorder:
                case HitTestValues.TopBorder:
                    _currentCursorName = "Vertical";
                    break;
                case HitTestValues.LeftBorder:
                case HitTestValues.RightBorder:
                    _currentCursorName = "Horizontal";
                    break;
                case HitTestValues.TopLeftBorder:
                case HitTestValues.BottomRightBorder:
                    _currentCursorName = "Diagonal 1";
                    break;
                case HitTestValues.TopRightBorder:
                case HitTestValues.BottomLeftBorder:
                    _currentCursorName = "Diagonal 2";
                    break;
                default:
                    _currentCursorName = "Normal";
                    break;
            }

            if (_previousCursorName != _currentCursorName)
            {
                _previousCursorName = _currentCursorName;

                var cursor = cursors.First(x => x.name == _currentCursorName);
                if (cursor != null)
                {
                    UnityEngine.Cursor.SetCursor(cursor.image, cursor.offset, CursorMode.Auto);
                }
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (WindowManager.Resizable)
            {
                _beginningCursorEdgeDistance = CursorManager.GetEdgeDistance(out _mouseDownCursorFlag);
                _isMouseDown = true;

                if (!WindowManager.Bordered && !WindowManager.Maximized)
                {
                    if (_mouseDownCursorFlag != HitTestValues.Client)
                    {
                        blackPanel.SetActive(true);
                        WindowManager.LockWindowUpdate(true);
                    }
                }
            }
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (WindowManager.Resizable)
            {
                _beginningCursorEdgeDistance = CursorManager.GetEdgeDistance(out _mouseDownCursorFlag);
                _mouseDownCursorFlag = HitTestValues.Client;
                _isMouseDown = false;

                if (!WindowManager.Bordered && !WindowManager.Maximized)
                {
                    blackPanel.SetActive(false);
                    WindowManager.LockWindowUpdate(false);
                }
            }
        }

        public void Maximize(bool value)
        {
//        EventSystem.current.SetSelectedGameObject(null);

            WindowManager.Maximized = value;

            if (maximizeIcon != null)
            {
                maximizeIcon.sprite = WindowManager.Maximized ? maximizeIconMaximized : maximizeIconNotMaximized;
            }

//        if (!WindowManager.Bordered)
//        {
//            var tempRect = WindowManager.GetWindowRect();
//            
//
//            if (!WindowManager.Maximized)
//            {
//                if (_lastScreenRect.width == 0)
//                {
//                    _lastScreenRect.width = defaultWindowSize.x;
//                    _lastScreenRect.height = defaultWindowSize.y;
//                }
//                
//                WindowManager.MoveWindow(_lastScreenRect, true);
//            }
//            else
//            {
//                var rect = new Rect
//                {
//                    x = 0,
//                    y = 0,
//                    width = Screen.currentResolution.width,
//                    height = Screen.currentResolution.height
//                };
//                
//                WindowManager.MoveWindow(rect, false);
//            }
//
//            _lastScreenRect = tempRect;
//        }
//        else
//        {
            WindowManager.ShowWindow(WindowManager.Maximized
                ? WindowShowCommands.Maximize
                : WindowShowCommands.Restore);
//        }
        }


        #region UI Fonctions

        public void SetBorderActive(bool active)
        {
            if (WindowManager.Bordered == active) return;

            var rect = WindowManager.GetWindowRect();

            if (active)
            {
                WindowManager.Bordered = true;
                WindowManager.UpdateWindowStyle();
                rect.height -= borderSize.y + borderSize.w;
            }
            else
            {
                WindowManager.Bordered = false;
                WindowManager.UpdateWindowStyle();
                rect.height += borderSize.y + borderSize.w;
            }

            WindowManager.MoveWindow(rect, true);
        }

        public void ResetWindowSize()
        {
            var rect = WindowManager.GetWindowRect();

            rect.width = defaultWindowSize.x;
            rect.height = defaultWindowSize.y;

            WindowManager.MoveWindow(rect, true);
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        public void Minimize()
        {
            WindowManager.ShowWindow(WindowShowCommands.Minimize);
        }

        public void Maximize()
        {
//        EventSystem.current.SetSelectedGameObject(null);


            WindowManager.Maximized = !WindowManager.Maximized;

            Maximize(WindowManager.Maximized);
        }

        public void ResizableBorderless()
        {
//        WinApi.SetWindowLong(activeWindow,
//            (int) WindowLongIndex.Style,
//            (uint) WindowStyleFlags.Visible | (uint) WindowStyleFlags.Popup | (uint) WindowStyleFlags.SizeBox);


            WindowManager.Bordered = false;
        }

        #endregion
    }
}