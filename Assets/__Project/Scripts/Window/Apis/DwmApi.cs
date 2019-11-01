using System;
using System.Runtime.InteropServices;
using Window.Structures;

namespace Window.Apis
{
    public static class DwmApi
    {
        #region DwmExtendFrameIntoClientArea

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr handledWindow, ref Margins margins);

        #endregion
    }
}