using System;
using System.Data.Common;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Borderless;
using UnityEngine;
using Color = UnityEngine.Color;

public class WindowManager
{
    //public static HandleRef HandledWindow;
//    private static IntPtr _oldWindowProcessesPtr;
//    private static IntPtr _newWindowProcessesPtr;
//    private static WndProcDelegate _newWindowProcesses;

    public static Vector4Int BorderSize;
    public static Vector2Int AspectRatio;
    public static Vector2Int MinWindowSize;
    public static Vector2Int MaxWindowSize;

    public static bool KeepAspectRatio;
    public static bool Resizable;
    public static bool Bordered = true;
    public static bool Maximized = false;
    public static bool Visible = true;
    public static bool Overlapped = true;
    public static bool Caption = true;
    public static bool SystemMenu = true;
    public static bool MinimizeBox = true;
    public static bool MaximizeBox = true;


    public static bool LockedWindowUpdate;

    private static readonly Vector2Int DefaultMinSize = new Vector2Int(50, 50);

    public static void Init
    (
        Vector4Int borderSize,
        Vector2Int aspectRatio,
        Vector2Int minWindowSize,
        Vector2Int maxWindowSize,
        bool keepAspectRatio,
        bool resizable,
        bool bordered,
        bool maximized,
        bool visible,
        bool overlapped,
        bool caption,
        bool systemMenu,
        bool minimizeBox,
        bool maximizeBox
    )
    {
        BorderSize = borderSize;
        AspectRatio = aspectRatio;
        MinWindowSize = minWindowSize;
        MaxWindowSize = maxWindowSize;

        KeepAspectRatio = keepAspectRatio;
        Resizable = resizable;
        Bordered = bordered;
        Maximized = maximized;
        Visible = visible;
        Overlapped = overlapped;
        Caption = caption;
        SystemMenu = systemMenu;
        MinimizeBox = minimizeBox;
        MaximizeBox = maximizeBox;

//        HandledWindow = new HandleRef(null, WinApi.GetActiveWindow());
//        _newWindowProcesses = WindowProcesses;
//        _newWindowProcessesPtr = Marshal.GetFunctionPointerForDelegate(_newWindowProcesses);
//        _oldWindowProcessesPtr = WinApi.SetWindowLongPtr(HandledWindow, -4, _newWindowProcessesPtr);


        #region Checksize Size

        //Check Min Size
        if (maxWindowSize.x != 0)
        {
            if (minWindowSize.x > maxWindowSize.x)
            {
                minWindowSize.x = maxWindowSize.x;
            }

            if (minWindowSize.y > maxWindowSize.y)
            {
                minWindowSize.y = maxWindowSize.y;
            }
        }

        //Check Default Min
        if (minWindowSize.x < DefaultMinSize.x)
        {
            minWindowSize.x = DefaultMinSize.x;
        }

        if (minWindowSize.y < DefaultMinSize.y)
        {
            minWindowSize.y = DefaultMinSize.y;
        }

        //Check Max Size
        if (maxWindowSize.x != 0)
        {
            if (maxWindowSize.x < minWindowSize.x)
            {
                maxWindowSize.x = minWindowSize.x;
            }
        }

        if (maxWindowSize.y != 0)
        {
            if (maxWindowSize.y < minWindowSize.y)
            {
                maxWindowSize.y = minWindowSize.y;
            }
        }

        #endregion


        UpdateWindowStyle();
    }

//    private static void Term()
//    {
//        WinApi.SetWindowLongPtr(HandledWindow, -4, _oldWindowProcessesPtr);
//        HandledWindow = new HandleRef(null, IntPtr.Zero);
//        _oldWindowProcessesPtr = IntPtr.Zero;
//        _newWindowProcessesPtr = IntPtr.Zero;
//        _newWindowProcesses = null;
//    }


//    ~WindowManager()
//    {
//        Term();
//    }


    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    private static IntPtr WindowProcesses(IntPtr handleWindow, uint message, IntPtr wParam, IntPtr lParam)
    {
//        if (message == (uint) WindowMessages.NCDESTROY || message == (uint) WindowMessages.WINDOWPOSCHANGING)
//        {
//            Term();
//        }

        Debug.Log((WindowMessages) message);

//        switch (message)
//        {
//            case (uint) WindowMessages.NCCALCSIZE:
//                if ((int) wParam == 1)
//                {
//                    WinApi.SetWindowLongPtr(HandledWindow, (int) WindowLongIndex.Style, (IntPtr) 0);
//                }
//
//                return (IntPtr) 0;
//
//
//            case (uint) WindowMessages.PAINT:
//
////                var hdc = BeginPaint(handleWindow, out var ps);
////
////                var gfx = XGraphic.FromHwnd(hdc);
////                var brush = new SolidBrush(Color.White);
////
////                gfx.FillRectangle(brush,
////                    ps.rcPaint.Left,
////                    ps.rcPaint.Top,
////                    ps.rcPaint.Right - ps.rcPaint.Left,
////                    ps.rcPaint.Bottom - ps.rcPaint.Top);
////
////                EndPaint(handleWindow, ref ps);
//                return (IntPtr) 1;
//
//            case (uint) WindowMessages.NCHITTEST:
//                var aa = (uint) WindowStyleFlags.Caption |
//                         (uint) WindowStyleFlags.Visible;
//
//                WinApi.SetWindowLongPtr(HandledWindow, (int) WindowLongIndex.Style, (IntPtr) aa);
//                return (IntPtr) 1;
//
//            case (uint) WindowMessages.COMMAND:
//
//                var id = wParam.ToInt32();
//                if (id == (int) DialogBoxCommandID.IDOk || id == (int) DialogBoxCommandID.IDCancel)
//                {
//                    WinApi.EndDialog(handleWindow, (IntPtr) id);
//                    return (IntPtr) 1;
//                }
//
//                return (IntPtr) 0;
//            default:
//                return WinApi.DefWindowProc(handleWindow, message, wParam, lParam);
//        }

        return WinApi.DefWindowProc(handleWindow, message, wParam, lParam);
        return (IntPtr) 0;
    }

    public static void LockWindowUpdate(bool lockValue)
    {
        if (lockValue)
        {
            if (!LockedWindowUpdate)
            {
                var activeWindow = WinApi.GetActiveWindow();
                WinApi.LockWindowUpdate(activeWindow);
                LockedWindowUpdate = true;
            }
        }
        else
        {
            if (LockedWindowUpdate)
            {
                WinApi.LockWindowUpdate(IntPtr.Zero);
                LockedWindowUpdate = false;
            }
        }
    }

    public static void UpdateWindowStyle()
    {
        var style = GetWindowStyleFlags();
        var extendedStyle = GetExtendedWindowStyleFlags();

        var handledWindow = new HandleRef(null, WinApi.GetActiveWindow());

        WinApi.SetWindowLongPtr(handledWindow, (int) WindowLongIndex.Style, (IntPtr) style);
//        MyWinApi.SetWindowLongPtr(handledWindow, (int) WindowLongIndex.ExtendedStyle, (IntPtr) 0);

//        var lStyle = (uint)WinApi.GetWindowLongPtr(HandledWindow.Handle, (int)WindowLongIndex.Style);
//        lStyle &= ~(
//            (uint) WindowStyleFlags.Caption | 
//            (uint) WindowStyleFlags.ThickFrame |
//            (uint) WindowStyleFlags.Minimize |
//            (uint) WindowStyleFlags.Maximize |
//            (uint) WindowStyleFlags.SystemMenu);
//        
//        WinApi.SetWindowLongPtr(HandledWindow, (int)WindowLongIndex.Style, (IntPtr)lStyle);
//        
//        var lExStyle = (uint)WinApi.GetWindowLongPtr(HandledWindow.Handle, (int)WindowLongIndex.ExtendedStyle);
//        lExStyle &= ~(
//            (uint) WindowStyleFlags.ExtendedDlgModalFrame | 
//            (uint) WindowStyleFlags.ExtendedClientEdge | 
//            (uint) WindowStyleFlags.ExtendedStaticEdge);
//        
//        WinApi.SetWindowLongPtr(HandledWindow, (int)WindowLongIndex.ExtendedStyle, (IntPtr)lExStyle);
//        
//        
//        const uint message = (int) SetWindowPosFlags.FrameChanged |
//                             (int) SetWindowPosFlags.NoMove |
//                             (int) SetWindowPosFlags.NoSize |
//                             (int) SetWindowPosFlags.NoZOrder |
//                             (int) SetWindowPosFlags.NoOwnerZOrder;
//        
//        
//        WinApi.SetWindowPos(HandledWindow.Handle, 0, 0, 0, 0, 0, message);
    }

    public static void ShowWindow(WindowShowStyle windowStatus)
    {
        var activeWindow = WinApi.GetActiveWindow();

        var status = (int) windowStatus;

        WinApi.ShowWindow(activeWindow, status);
    }

    public static void MoveWindow(Rect rect, bool checkLimits)
    {
        SetWindowPos(rect, checkLimits);
        return;

        var activeWindow = WinApi.GetActiveWindow();

        if (checkLimits)
        {
            rect = CheckWindowSize(rect);
        }

        var x = (int) rect.x;
        var y = (int) rect.y;
        var width = (int) rect.width;
        var height = (int) rect.height;


        WinApi.MoveWindow(activeWindow, x, y, width, height, false);
    }


    public static void SetWindowPos(Rect rect, bool checkLimits)
    {
        var activeWindow = WinApi.GetActiveWindow();

        if (checkLimits)
        {
            rect = CheckWindowSize(rect);
        }


        var x = (int) rect.x;
        var y = (int) rect.y;
        var width = (int) rect.width;
        var height = (int) rect.height;

        UpdateWindowStyle();

        const uint message = (int) SetWindowPosFlags.ShowWindow /*|
                             (int) SetWindowPosFlags.AsyncWindowPos*/;


        WinApi.SetWindowPos(activeWindow, 0, x, y, width, height, message);
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

    public static WindowRect GetWindowRectPoints()
    {
        var activeWindow = WinApi.GetActiveWindow();
        WinApi.GetWindowRect(activeWindow, out var windowRect);
        return windowRect;
    }

    public static uint GetWindowStyleFlags()
    {
        uint style = 0;

        if (Visible)
        {
            style |= (uint) WindowStyleFlags.Visible;
        }


        if (Bordered)
        {
            style |= (uint) WindowStyleFlags.Border;
        }
        else
        {
            //style |= (uint) WindowStyleFlags.Popup;
        }

        if (Resizable && Bordered)
        {
            style |= (uint) WindowStyleFlags.ThickFrame;
        }

        if (Maximized)
        {
            style |= (uint) WindowStyleFlags.Maximize;
        }

        if (SystemMenu)
        {
            style |= (uint) WindowStyleFlags.SystemMenu;
        }


        if (Caption)
        {
            style |= (uint) WindowStyleFlags.Caption;
        }


        if (Overlapped)
        {
            style |= (uint) WindowStyleFlags.Overlapped;
        }

        if (MinimizeBox && Caption)
        {
            style |= (uint) WindowStyleFlags.MinimizeBox;
        }


        if (MaximizeBox && Caption)
        {
            style |= (uint) WindowStyleFlags.MaximizeBox;
        }

        return style;
    }

    public static uint GetExtendedWindowStyleFlags()
    {
        uint style = 0;


//        if (Visible)
//        {
//            style |= (uint) WindowStyleFlags.;
//        }
//
//
//        if (Bordered)
//        {
//            style |= (uint) WindowStyleFlags.Border;
//        }
//        else
//        {
//            //style |= (uint) WindowStyleFlags.Popup;
//        }
//
//        if (Resizable && Bordered)
//        {
//            style |= (uint) WindowStyleFlags.ThickFrame;
//        }
//
//        if (Maximized)
//        {
//            style |= (uint) WindowStyleFlags.Maximize;
//        }
//
//        if (SystemMenu)
//        {
//            style |= (uint) WindowStyleFlags.SystemMenu;
//        }
//
//
//        if (Caption)
//        {
//            style |= (uint) WindowStyleFlags.Caption;
//        }
//
//
//        if (Overlapped)
//        {
//            style |= (uint) WindowStyleFlags.Overlapped;
//        }
//
//        if (MinimizeBox && Caption)
//        {
//            style |= (uint) WindowStyleFlags.MinimizeBox;
//        }
//
//
//        if (MaximizeBox && Caption)
//        {
//            style |= (uint) WindowStyleFlags.MaximizeBox;
//        }

        return style;
    }

    #region Size

    public static void WindowSizeChange(Vector2 beginningCursorEdgeDistance, CursorPositionFlags beginningCursorFlag)
    {
        if (beginningCursorFlag == CursorPositionFlags.Main)
        {
            return;
        }

        var globalCursorPosition = CursorManager.GetCursorPosition();
        var windowRectPoints = GetWindowRectPoints();

        var newWindowRect = GetWindowRect();


        if (KeepAspectRatio)
        {
            float aspectSizeValue;
            switch (beginningCursorFlag)
            {
                case CursorPositionFlags.Left:
                case CursorPositionFlags.TopLeft:
                case CursorPositionFlags.BottomLeft:
                    newWindowRect = WindowRectResizeLeft
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );

                    aspectSizeValue = newWindowRect.width / AspectRatio.x;
                    newWindowRect.height = aspectSizeValue * AspectRatio.y;
                    break;
                case CursorPositionFlags.Right:
                case CursorPositionFlags.BottomRight:
                case CursorPositionFlags.TopRight:
                    newWindowRect = WindowRectResizeRight
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );

                    aspectSizeValue = newWindowRect.width / AspectRatio.x;
                    newWindowRect.height = aspectSizeValue * AspectRatio.y;
                    break;
                case CursorPositionFlags.Top:
                    newWindowRect = WindowRectResizeTop
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );

                    aspectSizeValue = newWindowRect.height / AspectRatio.y;
                    newWindowRect.width = aspectSizeValue * AspectRatio.x;
                    break;
                case CursorPositionFlags.Bottom:
                    newWindowRect = WindowRectResizeBottom
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );

                    aspectSizeValue = newWindowRect.height / AspectRatio.y;
                    newWindowRect.width = aspectSizeValue * AspectRatio.x;
                    break;
            }
        }
        else
        {
            switch (beginningCursorFlag)
            {
                case CursorPositionFlags.Left:
                {
                    newWindowRect = WindowRectResizeLeft
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                }

                case CursorPositionFlags.Right:
                {
                    newWindowRect = WindowRectResizeRight
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                }

                case CursorPositionFlags.Top:
                {
                    newWindowRect = WindowRectResizeTop
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                }

                case CursorPositionFlags.Bottom:
                {
                    newWindowRect = WindowRectResizeBottom
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                }

                case CursorPositionFlags.TopLeft:
                    newWindowRect = WindowRectResizeLeft
                    (beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    newWindowRect = WindowRectResizeTop
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                case CursorPositionFlags.TopRight:
                    newWindowRect = WindowRectResizeRight
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    newWindowRect = WindowRectResizeTop
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                case CursorPositionFlags.BottomLeft:
                    newWindowRect = WindowRectResizeLeft
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );

                    newWindowRect = WindowRectResizeBottom
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
                case CursorPositionFlags.BottomRight:
                    newWindowRect = WindowRectResizeRight
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    newWindowRect = WindowRectResizeBottom
                    (
                        beginningCursorEdgeDistance,
                        windowRectPoints,
                        globalCursorPosition,
                        newWindowRect
                    );
                    break;
            }
        }


        MoveWindow(newWindowRect, true);
    }

    private static Rect CheckWindowSize(Rect windowRect)
    {
        if (windowRect.width < MinWindowSize.x)
        {
            windowRect.width = MinWindowSize.x;
        }

        if (windowRect.height < MinWindowSize.y)
        {
            windowRect.height = MinWindowSize.y;
        }

        if (MaxWindowSize.x != 0)
        {
            if (windowRect.width > MaxWindowSize.x)
            {
                windowRect.width = MaxWindowSize.x;
            }
        }

        if (MaxWindowSize.y != 0)
        {
            if (windowRect.height > MaxWindowSize.y)
            {
                windowRect.height = MaxWindowSize.y;
            }
        }

        return windowRect;
    }

    private static Rect WindowRectResizeBottom(Vector2 beginningCursorEdgeDistance, WindowRect windowRectPoints,
        Vector2 globalCursorPosition, Rect newWindowRect)
    {
        var globalClickPoint = windowRectPoints.Bottom - beginningCursorEdgeDistance.y;
        var distance = Mathf.Abs(globalCursorPosition.y - globalClickPoint);
        //Bottom Expand
        if (globalClickPoint > globalCursorPosition.y)
        {
            newWindowRect.height -= distance;
        }
        //Bottom Shrink
        else if (globalClickPoint < globalCursorPosition.y)
        {
            newWindowRect.height += distance;
        }

        return newWindowRect;
    }

    private static Rect WindowRectResizeTop(Vector2 beginningCursorEdgeDistance, WindowRect windowRectPoints,
        Vector2 globalCursorPosition, Rect newWindowRect)
    {
        var globalClickPoint = windowRectPoints.Top + beginningCursorEdgeDistance.y;
        var distance = Mathf.Abs(globalCursorPosition.y - globalClickPoint);
        //Top Expand
        if (globalClickPoint > globalCursorPosition.y)
        {
//            newWindowRect.y -= distance;
//            newWindowRect.height += distance;

            if (MaxWindowSize.y == 0)
            {
                newWindowRect.y -= distance;
                newWindowRect.height += distance;
            }
            else if (newWindowRect.height < MaxWindowSize.y)
            {
                newWindowRect.y -= distance;
                newWindowRect.height += distance;
            }

            if (newWindowRect.height > MaxWindowSize.y)
            {
                var difference = newWindowRect.height - MaxWindowSize.y;
                newWindowRect.y += difference;
            }
        }
        //Top Shrink
        else if (globalClickPoint < globalCursorPosition.y)
        {
//            newWindowRect.y += distance;
//            newWindowRect.height -= distance;

            if (newWindowRect.height > MinWindowSize.y)
            {
                newWindowRect.y += distance;
                newWindowRect.height -= distance;
            }

            if (newWindowRect.height < MinWindowSize.y)
            {
                var difference = MinWindowSize.y - newWindowRect.height;
                newWindowRect.y -= difference;
            }
        }

        return newWindowRect;
    }

    private static Rect WindowRectResizeRight(Vector2 beginningCursorEdgeDistance, WindowRect windowRectPoints,
        Vector2 globalCursorPosition, Rect newWindowRect)
    {
        var globalClickPoint = windowRectPoints.Right - beginningCursorEdgeDistance.x;
        var distance = Mathf.Abs(globalCursorPosition.x - globalClickPoint);
        //Right Expand
        if (globalClickPoint > globalCursorPosition.x)
        {
            newWindowRect.width -= distance;
        }
        //Right Shrink
        else if (globalClickPoint < globalCursorPosition.x)
        {
            newWindowRect.width += distance;
        }

        return newWindowRect;
    }

    private static Rect WindowRectResizeLeft(Vector2 beginningCursorEdgeDistance, WindowRect windowRectPoints,
        Vector2 globalCursorPosition, Rect newWindowRect)
    {
        var globalClickPoint = windowRectPoints.Left + beginningCursorEdgeDistance.x;
        var distance = Mathf.Abs(globalCursorPosition.x - globalClickPoint);
        //Left Expand
        if (globalClickPoint > globalCursorPosition.x)
        {
            if (MaxWindowSize.x == 0)
            {
                newWindowRect.x -= distance;
                newWindowRect.width += distance;
            }
            else if (newWindowRect.width < MaxWindowSize.x)
            {
                newWindowRect.x -= distance;
                newWindowRect.width += distance;
            }

            if (newWindowRect.width > MaxWindowSize.x)
            {
                var difference = newWindowRect.width - MaxWindowSize.x;
                newWindowRect.x += difference;
            }
        }

        //Left Shrink
        else if (globalClickPoint < globalCursorPosition.x)
        {
            if (newWindowRect.width > MinWindowSize.x)
            {
                newWindowRect.x += distance;
                newWindowRect.width -= distance;
            }

            if (newWindowRect.width < MinWindowSize.x)
            {
                var difference = MinWindowSize.x - newWindowRect.width;
                newWindowRect.x -= difference;
            }
        }

        return newWindowRect;
    }

    #endregion
}