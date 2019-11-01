using System;

namespace Window.Flags
{
    [Flags]
    public enum WindowStyleFlags : uint
    {
        Overlapped = 0x00000000,
        Popup = 0x80000000,
        Child = 0x40000000,
        Minimize = 0x20000000,
        Visible = 0x10000000,
        Disabled = 0x08000000,
        ClipSiblings = 0x04000000,
        ClipChildren = 0x02000000,
        Maximize = 0x01000000,
        Border = 0x00800000,
        DialogFrame = 0x00400000,
        Vscroll = 0x00200000,
        Hscroll = 0x00100000,
        SystemMenu = 0x00080000,
        ThickFrame = 0x00040000,
        Group = 0x00020000,
        Tabstop = 0x00010000,

        MinimizeBox = 0x00020000,
        MaximizeBox = 0x00010000,

        Caption = Border | DialogFrame,
        Tiled = Overlapped,
        Iconic = Minimize,
        SizeBox = ThickFrame,
        TiledWindow = Overlapped,

        OverlappedWindow = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,
        ChildWindow = Child,

        ExtendedDlgModalFrame = 0x00000001,
        ExtendedNoParentNotify = 0x00000004,
        ExtendedTopmost = 0x00000008,
        ExtendedAcceptFiles = 0x00000010,
        ExtendedTransparent = 0x00000020,
        ExtendedMDIChild = 0x00000040,
        ExtendedToolWindow = 0x00000080,
        ExtendedWindowEdge = 0x00000100,
        ExtendedClientEdge = 0x00000200,
        ExtendedContextHelp = 0x00000400,
        ExtendedRight = 0x00001000,
        ExtendedLeft = 0x00000000,
        ExtendedRTLReading = 0x00002000,
        ExtendedLTRReading = 0x00000000,
        ExtendedLeftScrollbar = 0x00004000,
        ExtendedRightScrollbar = 0x00000000,
        ExtendedControlParent = 0x00010000,
        ExtendedStaticEdge = 0x00020000,
        ExtendedAppWindow = 0x00040000,
        ExtendedOverlappedWindow = ExtendedWindowEdge | ExtendedClientEdge,
        ExtendedPaletteWindow = ExtendedWindowEdge | ExtendedToolWindow | ExtendedTopmost,
        ExtendedLayered = 0x00080000,
        ExtendedNoinheritLayout = 0x00100000,
        ExtendedLayoutRTL = 0x00400000,
        ExtendedComposited = 0x02000000,
        ExtendedNoActivate = 0x08000000
    }
}