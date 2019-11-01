namespace Window.Flags
{
    /// <summary>When resizing a window, this is the side of the window being resized.</summary>
    public enum SizingWindowSide : uint
    {
        /// <summary>WMSZ_LEFT: Left edge</summary>
        Left = 1,

        /// <summary>WMSZ_RIGHT: Right edge</summary>
        Right = 2,

        /// <summary>WMSZ_TOP: Top edge</summary>
        Top = 3,

        /// <summary>WMSZ_TOPLEFT: Top-left corner</summary>
        TopLeft = 4,

        /// <summary>WMSZ_TOPRIGHT: Top-right corner</summary>
        TopRight = 5,

        /// <summary>WMSZ_BOTTOM: Bottom edge</summary>
        Bottom = 6,

        /// <summary>WMSZ_BOTTOMLEFT: Bottom-left corner</summary>
        BottomLeft = 7,

        /// <summary>WMSZ_BOTTOMRIGHT: Bottom-right corner</summary>
        BottomRight = 8,
    }
}