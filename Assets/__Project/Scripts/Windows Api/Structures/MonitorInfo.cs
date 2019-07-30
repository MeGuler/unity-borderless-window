using System.Runtime.InteropServices;

namespace Borderless.Api.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MonitorInfo
    {
        public int Size;
        public Rect MonitorRect;
        public Rect WorkRect;
        public int Flags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] MonitorName;
    }
}