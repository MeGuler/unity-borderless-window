using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class WindowStyle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Window Settings")] public Vector4Int borderSize;
    public Vector2Int defaultWindowSize;
    public Vector2Int minWindowSize;
    public Vector2Int maxWindowSize;

    public bool resizable = true;
    public bool bordered = false;
    public bool maximized = false;
    public bool visible = true;
    public bool overlapped = false;
    public bool caption = false;
    public bool systemMenu = false;
    public bool minimizeBox =false;
    public bool maximizeBox = false;

    [Header("Cursors")] public List<CursorData> cursors = new List<CursorData>();


    private bool _isMouseDown;

    private string _currentCursorName;
    private string _previousCursorName;

    private Vector2 _beginninfCursorEdgeDistance;
    private CursorPositionFlags _mouseDownCursorFlag;


    private void Awake()
    {
        WindowManager.Init
        (
            borderSize,
            minWindowSize,
            maxWindowSize,
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
                WindowManager.WindowSizeChange(_beginninfCursorEdgeDistance, _mouseDownCursorFlag);
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

            if (_currentCursorName == "Normal")
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                var cursor = cursors.First(x => x.name == _currentCursorName);
                if (cursor != null)
                {
                    Cursor.SetCursor(cursor.image, cursor.offset, CursorMode.Auto);
                }
            }
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (WindowManager.Resizable)
        {
            _beginninfCursorEdgeDistance = CursorManager.GetEdgeDistance(out _mouseDownCursorFlag);
            _isMouseDown = true;
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (WindowManager.Resizable)
        {
            _beginninfCursorEdgeDistance = CursorManager.GetEdgeDistance(out _mouseDownCursorFlag);
            _mouseDownCursorFlag = CursorPositionFlags.Main;
            _isMouseDown = false;
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
            //rect.width -= borderSize.x + borderSize.z;
            rect.height -= borderSize.y + borderSize.w;
        }
        else
        {
            WindowManager.Bordered = false;
            WindowManager.UpdateWindowStyle();
            //rect.width += borderSize.x + borderSize.z;
            rect.height += borderSize.y + borderSize.w;
        }

        WindowManager.MoveWindow(rect);
    }

    public void ResetWindowSize()
    {
        var rect = WindowManager.GetWindowRect();

        rect.width = defaultWindowSize.x;
        rect.height = defaultWindowSize.y;

        WindowManager.MoveWindow(rect);
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
        EventSystem.current.SetSelectedGameObject(null);

        WindowManager.ShowWindow(WindowManager.Maximized
            ? WindowShowStyle.Restore
            : WindowShowStyle.Maximize);
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