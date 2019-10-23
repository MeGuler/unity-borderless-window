using System.Runtime.InteropServices;

namespace Borderless.Api
{
    public static class kernel32
    {
        #region Get Current Thread ID

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        #endregion
    }
}