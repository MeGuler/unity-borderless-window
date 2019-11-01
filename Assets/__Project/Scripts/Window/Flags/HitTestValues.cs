namespace Window.Flags
{
    public enum HitTestValues
    {
        /// <summary>
        /// On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep to indicate an error).
        /// </summary>
        Error = -2,

        /// <summary>
        /// In a window currently covered by another window in the same thread (the message will be sent to underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).
        /// </summary>
        Transparent = -1,

        /// <summary>
        /// On the screen background or on a dividing line between windows.
        /// </summary>
        NoWhere = 0,

        /// <summary>
        /// In a client area.
        /// </summary>
        Client = 1,

        /// <summary>
        /// In a title bar.
        /// </summary>
        Caption = 2,

        /// <summary>
        /// In a window menu or in a Close button in a child window.
        /// </summary>
        SysMenu = 3,

        /// <summary>
        /// In a size box (same as Size).
        /// </summary>
        GrowBox = 4,

        /// <summary>
        /// In a menu.
        /// </summary>
        Menu = 5,

        /// <summary>
        /// In a horizontal scroll bar.
        /// </summary>
        HorizontalScroll = 6,

        /// <summary>
        /// In the vertical scroll bar.
        /// </summary>
        VerticalScroll = 7,

        /// <summary>
        /// In a Minimize button.
        /// </summary>
        MinimizeButton = 8,

        /// <summary>
        /// In a Maximize button.
        /// </summary>
        MaximizeButton = 9,

        /// <summary>
        /// In the left border of a resizable window (the user can click the mouse to resize the window horizontally).
        /// </summary>
        LeftBorder = 10,

        /// <summary>
        /// In the right border of a resizable window (the user can click the mouse to resize the window horizontally).
        /// </summary>
        RightBorder = 11,

        /// <summary>
        /// In the upper-horizontal border of a window.
        /// </summary>
        TopBorder = 12,

        /// <summary>
        /// In the upper-left corner of a window border.
        /// </summary>
        TopLeftBorder = 13,

        /// <summary>
        /// In the upper-right corner of a window border.
        /// </summary>
        TopRightBorder = 14,

        /// <summary>
        /// In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).
        /// </summary>
        BottomBorder = 15,

        /// <summary>
        /// In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
        /// </summary>
        BottomLeftBorder = 16,

        /// <summary>
        /// In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
        /// </summary>
        BottomRightBorder = 17,

        /// <summary>
        /// In the border of a window that does not have a sizing border.
        /// </summary>
        Border = 18,

        /*/// <summary>
        /// Not implemented.
        /// </summary>
         Object = 19, */

        /// <summary>
        /// In a Close button.
        /// </summary>
        CloseButton = 20,

        /// <summary>
        /// In a Help button.
        /// </summary>
        HelpButton = 21,

        /// <summary>
        /// In a Maximize button.
        /// </summary>
        Zoom = MaximizeButton,

        /// <summary>
        /// In a Minimize button.
        /// </summary>
        Reduce = MinimizeButton,

        /// <summary>
        /// In a size box (same as GrowBox).
        /// </summary>
        Size = GrowBox
    }
}