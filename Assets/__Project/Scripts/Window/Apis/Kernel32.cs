using System.Runtime.InteropServices;

namespace Window.Apis
{
    public static class Kernel32
    {
        #region Get Current Thread ID

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        #endregion
    }
}