using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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
    private CursorPositionFlags _mouseDownCursorFlag;
    private Rect _lastScreenRect;

    //Will be Delete
    private int counterDown;
    private int counterUp;
    
    
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
        if (WindowManager.Resizable && !WindowManager.Bordered)
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
        
        GUI.Label(new Rect(10, 10, 100, 20), counterDown.ToString());
        GUI.Label(new Rect(10, 50, 100, 20), counterUp.ToString());
    }

    private void SetCursorFlagIcon()
    {
        var cursorPositionFlags = CursorManager.GetCursorPositionFlag();

        switch (cursorPositionFlags)
        {
            case CursorPositionFlags.Bottom:
            case CursorPositionFlags.Top:
                _currentCursorName = "Vertical";
                break;
            case CursorPositionFlags.Left:
            case CursorPositionFlags.Right:
                _currentCursorName = "Horizontal";
                break;
            case CursorPositionFlags.TopLeft:
            case CursorPositionFlags.BottomRight:
                _currentCursorName = "Diagonal 1";
                break;
            case CursorPositionFlags.TopRight:
            case CursorPositionFlags.BottomLeft:
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
                Cursor.SetCursor(cursor.image, cursor.offset, CursorMode.Auto);
            }
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (WindowManager.Resizable)
        {
            _beginningCursorEdgeDistance = CursorManager.GetEdgeDistance(out _mouseDownCursorFlag);
            _isMouseDown = true;
//            WindowManager.LockWindowUpdate(true);
//            Debug.Log("Down");
            counterDown++;
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (WindowManager.Resizable)
        {
            _beginningCursorEdgeDistance = CursorManager.GetEdgeDistance(out _mouseDownCursorFlag);
            _mouseDownCursorFlag = CursorPositionFlags.Main;
            _isMouseDown = false;
//            WindowManager.LockWindowUpdate(false);
//            Debug.Log("Up");
            counterUp++;
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

        if (!WindowManager.Bordered)
        {
            var tempRect = WindowManager.GetWindowRect();
            

            if (!WindowManager.Maximized)
            {
                if (_lastScreenRect.width == 0)
                {
                    _lastScreenRect.width = defaultWindowSize.x;
                    _lastScreenRect.height = defaultWindowSize.y;
                }
                
                WindowManager.MoveWindow(_lastScreenRect, true);
            }
            else
            {
                var rect = new Rect
                {
                    x = 0,
                    y = 0,
                    width = Screen.currentResolution.width,
                    height = Screen.currentResolution.height
                };
                
                WindowManager.MoveWindow(rect, false);
            }

            _lastScreenRect = tempRect;
        }
        else
        {
            WindowManager.ShowWindow(WindowManager.Maximized ? WindowShowStyle.Maximize : WindowShowStyle.Restore);
        }

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
        WindowManager.ShowWindow(WindowShowStyle.Minimize);
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