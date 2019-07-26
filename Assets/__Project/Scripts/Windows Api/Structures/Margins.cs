using System.Runtime.InteropServices;

namespace Borderless.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }
}