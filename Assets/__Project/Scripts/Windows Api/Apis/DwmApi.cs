using System;
using System.Runtime.InteropServices;
using Borderless.Api.Structures;

namespace Borderless.Api
{
    public static class DwmApi
    {
        #region DwmExtendFrameIntoClientArea

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr handledWindow, ref Margins margins);

        #endregion
    }
}