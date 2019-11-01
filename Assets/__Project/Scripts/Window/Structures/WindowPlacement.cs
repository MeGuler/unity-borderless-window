using System;
using System.Runtime.InteropServices;


using Window.Flags;


namespace Window.Structures
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int Length;
        public int Flags;
        public WindowShowCommands windowShowCommand;
        public System.Drawing.Point MinPosition;
        public System.Drawing.Point MaxPosition;
        public System.Drawing.Rectangle NormalPosition;
    }
}