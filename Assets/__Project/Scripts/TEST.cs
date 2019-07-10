using UnityEngine;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using XGraphic = System.Drawing.Graphics;
using Color = System.Drawing.Color;


public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

class TEST : MonoBehaviour
{
    #region DLL Import

    [DllImport("user32.dll")]
    static extern bool EndDialog(IntPtr hDlg, IntPtr nResult);

    [DllImport("user32.dll")]
    static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

    [DllImport("dwmapi.dll")]
    static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);


    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowText(IntPtr hWnd, string header);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcA")]
    private static extern IntPtr DefWindowProc(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private static extern IntPtr SetWindowPos(IntPtr handleWindow, int insertAfter, int x, int y, int width,
        int height,
        uint flags);

    #endregion


//    private const uint HTLEFT = 10;
//    private const uint HTRIGHT = 11;
//    private const uint HTBOTTOMRIGHT = 17;
//    private const uint HTBOTTOM = 15;
//    private const uint HTBOTTOMLEFT = 16;
//    private const uint HTTOP = 12;
//    private const uint HTTOPLEFT = 13;
//    private const uint HTTOPRIGHT = 14;
//
//    private const int RESIZE_HANDLE_SIZE = 10;


    private HandleRef _handledWindow;
    private IntPtr _oldWindowProcessesPtr;
    private IntPtr _newWindowProcessesPtr;
    private WndProcDelegate _newWindowProcesses;

    private float hscroll;

    public void Start()
    {
        Init();
        UpdateStyle();
    }

    void OnGUI()
    {
        if (_handledWindow.Handle == IntPtr.Zero)
        {
            Init();
        }
    }

    private void Init()
    {
        Debug.Log("Init");
        _handledWindow = new HandleRef(null, GetActiveWindow());
        _newWindowProcesses = WindowProcesses;
        _newWindowProcessesPtr = Marshal.GetFunctionPointerForDelegate(_newWindowProcesses);
        _oldWindowProcessesPtr =  WinApi.SetWindowLongPtr(_handledWindow, -4, _newWindowProcessesPtr);
    }

    private void Term()
    {
        WinApi.SetWindowLongPtr(_handledWindow, -4, _oldWindowProcessesPtr);
        _handledWindow = new HandleRef(null, IntPtr.Zero);
        _oldWindowProcessesPtr = IntPtr.Zero;
        _newWindowProcessesPtr = IntPtr.Zero;
        _newWindowProcesses = null;
    }


    ~TEST()
    {
        Term();
    }


//    private static IntPtr SetWindowLongPtr(HandleRef handleWindow, int nIndex, IntPtr dwNewLong)
//    {
//        return IntPtr.Size == 8
//            ? WindowManager.SetWindowLongPtr64(handleWindow, nIndex, dwNewLong)
//            : new IntPtr(WindowManager.SetWindowLong32(handleWindow, nIndex, dwNewLong.ToInt32()));
////        return new IntPtr(SetWindowLong(handleWindow.Handle, nIndex, (uint) dwNewLong.ToInt32()));
//    }


    private void UpdateStyle()
    {
        // remove the border style
        var currentStyle = (uint)WinApi.GetWindowLongPtr(_handledWindow.Handle, (int)WindowLongIndex.Style);
        if ((currentStyle & (uint) WindowStyleFlags.Border) != 0)
        {
            currentStyle &= ~(uint) (WindowStyleFlags.Border);
            WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.Style, (IntPtr)currentStyle);
            WinApi.SetWindowPos(_handledWindow.Handle, 0, -1, -1, -1, -1,
                (int) (SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoSize | SetWindowPosFlags.NoMove |
                       SetWindowPosFlags.FrameChanged | SetWindowPosFlags.NoReDraw | SetWindowPosFlags.NoActivate));
        }
    }


    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    private IntPtr WindowProcesses(IntPtr handleWindow, uint message, IntPtr wParam, IntPtr lParam)
    {
        if (message == (uint) WindowMessages.NCDESTROY || message == (uint) WindowMessages.WINDOWPOSCHANGING)
        {
            Term();
        }

        Debug.Log((WindowMessages) message);

        switch (message)
        {
            case (uint) WindowMessages.INITDIALOG:

                SetWindowText(handleWindow, "Borderless Window with Shadow");

                var m = new MARGINS();
                m.leftWidth = 0;
                m.rightWidth = 0;
                m.topHeight = 0;
                m.leftWidth = 1;

                DwmExtendFrameIntoClientArea(handleWindow, ref m);
                SetWindowPos(handleWindow, 0, 0, 0, 0, 0,
                    (uint) SetWindowPosFlags.NoZOrder |
                    (uint) SetWindowPosFlags.NoOwnerZOrder |
                    (uint) SetWindowPosFlags.NoMove |
                    (uint) SetWindowPosFlags.NoSize |
                    (uint) SetWindowPosFlags.FrameChanged);
                return (IntPtr) 1;


            case (uint) WindowMessages.NCCALCSIZE:
                if ((int) wParam == 1)
                {
                    WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.WindowProc, (IntPtr) 0);
                }

                return (IntPtr) 0;


            case (uint) WindowMessages.PAINT:

                var hdc = BeginPaint(handleWindow, out var ps);

                var gfx = XGraphic.FromHwnd(hdc);
                var brush = new SolidBrush(Color.White);

                gfx.FillRectangle(brush,
                    ps.rcPaint.Left,
                    ps.rcPaint.Top,
                    ps.rcPaint.Right - ps.rcPaint.Left,
                    ps.rcPaint.Bottom - ps.rcPaint.Top);

                EndPaint(handleWindow, ref ps);
                return (IntPtr) 1;

            case (uint) WindowMessages.NCHITTEST:
                var aa = (uint) WindowStyleFlags.Caption |
                         (uint) WindowStyleFlags.Visible;

                WinApi.SetWindowLongPtr(_handledWindow, (int) WindowLongIndex.WindowProc, (IntPtr) aa);
                return (IntPtr) 1;

            case (uint) WindowMessages.COMMAND:

                var id = wParam.ToInt32();
                if (id == (int) DialogBoxCommandID.IDOk || id == (int) DialogBoxCommandID.IDCancel)
                {
                    EndDialog(handleWindow, (IntPtr) id);
                    return (IntPtr) 1;
                }

                return (IntPtr) 0;
        }

        return (IntPtr) 0;
        return DefWindowProc(handleWindow, message, wParam, lParam);
    }


//    private IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
//    {
//        if (msg == (uint) WindowMessages.NCDESTROY || msg == (uint) WindowMessages.WINDOWPOSCHANGING)
//        {
//            Term();
//        }
//
//        if (msg == (uint) WindowMessages.NCMOUSEHOVER || msg == (uint) WindowMessages.MOUSEHOVER)
//        {
//            
//            var screenPoint = new Point(lParam.ToInt32());
//            var cursorPoint = new Vector2(screenPoint.X, screenPoint.Y);
//
//            var cursorRelativePoint = CursorManager.GetCursorPositionRelative(cursorPoint);
//            var clientPoint = new Point((int)cursorRelativePoint.x, (int)cursorRelativePoint.y);
//
//            Debug.Log("HOVER : " + clientPoint.X + " / " + clientPoint.Y);
//        }
//        
//        if (msg == (uint) WindowMessages.NCHITTEST || msg == (uint) WindowMessages.MOUSEMOVE)
//        {
//            var windowRect = WindowManager.GetWindowRect();
//
//            var screenPoint = new Point(lParam.ToInt32());
//            var cursorPoint = new Vector2(screenPoint.X, screenPoint.Y);
//
//            var cursorRelativePoint = CursorManager.GetCursorPositionRelative(cursorPoint);
//            var clientPoint = new Point((int)cursorRelativePoint.x,(int) cursorRelativePoint.y);
//
//            Debug.Log("HITTEST : " + clientPoint.X + " / " + clientPoint.Y);
//            
//            var boxes = new Dictionary<uint, Rectangle>()
//            {
//                {
//                    HTBOTTOMLEFT,
//                    new Rectangle(0, (int) windowRect.height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE,
//                        RESIZE_HANDLE_SIZE)
//                },
//                {
//                    HTBOTTOM,
//                    new Rectangle(RESIZE_HANDLE_SIZE, (int) windowRect.height - RESIZE_HANDLE_SIZE,
//                        (int) windowRect.width - 2 * RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)
//                },
//                {
//                    HTBOTTOMRIGHT,
//                    new Rectangle((int) windowRect.width - RESIZE_HANDLE_SIZE,
//                        (int) windowRect.height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)
//                },
//                {
//                    HTRIGHT,
//                    new Rectangle((int) windowRect.width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE,
//                        (int) windowRect.height - 2 * RESIZE_HANDLE_SIZE)
//                },
//                {
//                    HTTOPRIGHT,
//                    new Rectangle((int) windowRect.width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE,
//                        RESIZE_HANDLE_SIZE)
//                },
//                {
//                    HTTOP,
//                    new Rectangle(RESIZE_HANDLE_SIZE, 0, (int) windowRect.width - 2 * RESIZE_HANDLE_SIZE,
//                        RESIZE_HANDLE_SIZE)
//                },
//                {HTTOPLEFT, new Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
//                {
//                    HTLEFT,
//                    new Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE,
//                        (int) windowRect.height - 2 * RESIZE_HANDLE_SIZE)
//                }
//            };
//
//            foreach (var hitBox in boxes)
//            {
//                if (hitBox.Value.Contains(clientPoint))
//                {
//                    wParam = (IntPtr) hitBox.Key;
//                    break;
//                }
//            }
//        }
//
////        Debug.Log("wndProc msg:0x" +msg.ToString("x4") +
////                  " wParam:0x" + wParam.ToString("x4") +
////                  " lParam:0x" + lParam.ToString("x4"));
//
//        return DefWindowProc(hWnd, msg, wParam, lParam);
//    }
}