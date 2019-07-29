using System;
using System.Runtime.InteropServices;
using Borderless.Flags;


namespace Borderless.Api.Structures
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int Length;
        public int Flags;
        public ShowWindowCommands ShowCommand;
        public System.Drawing.Point MinPosition;
        public System.Drawing.Point MaxPosition;
        public System.Drawing.Rectangle NormalPosition;
    }
}