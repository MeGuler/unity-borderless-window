using System;
using System.Runtime.InteropServices;
using System.Text;
using Borderless.Api;
using Borderless.Flags;
using UnityEngine;
using UnityEngine.UI;


namespace Borderless
{
    public class BorderlessWindow : Window
    {
        [Header("Window Settings")]

        #region WindowSettings

        public Vector4Int resizeBorderSize;

        public Vector2Int startWindowSize;
        public Vector2Int minWindowSize;
        public Vector2Int maxWindowSize;
        public int captionHeight;
        public bool keepAspectRatio;

        #endregion

        [Header("Other")] public Image maximizeImage;
        public Sprite maximizeIcon1;
        public Sprite maximizeIcon2;

        protected override void Awake()
        {
            ResizeBorderSize = resizeBorderSize;
            StartWindowSize = startWindowSize;
            MinWindowSize = minWindowSize;
            MaxWindowSize = maxWindowSize;
            CaptionHeight = captionHeight;
            KeepAspectRatio = keepAspectRatio;
            base.Awake();
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public void Maximize()
        {
            var placement = GetWindowPlacement();


            if (placement.ShowCommand == ShowWindowCommands.Maximize)
            {
                maximizeImage.sprite = maximizeIcon1;
                placement.ShowCommand = ShowWindowCommands.Restore;
            }
            else
            {
                maximizeImage.sprite = maximizeIcon2;
                placement.ShowCommand = ShowWindowCommands.Maximize;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }

        public void Minimize()
        {
            var placement = GetWindowPlacement();


            if (placement.ShowCommand == ShowWindowCommands.ShowMinimized)
            {
                placement.ShowCommand = ShowWindowCommands.Restore;
            }
            else
            {
                placement.ShowCommand = ShowWindowCommands.ShowMinimized;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }


//        #region DLL Imports
//        private const string UnityWindowClassName = "UnityWndClass";

//        [DllImport("kernel32.dll")]
//        static extern uint GetCurrentThreadId();

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

//        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

//        [DllImport("user32.dll")]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
//        #endregion

//        #region Private fields
//        private static IntPtr windowHandle = IntPtr.Zero;
//        #endregion

//        #region Monobehavior implementation
//        /// <summary>
//        /// Called when this component is initialized
//        /// </summary>
        void Start()
        {
//            HandleWindow();

            Debug.Log(string.Format("Window Handle: {0}", HandledWindow.Handle));
        }

       

        protected override void OnGUI()
        {
            base.OnGUI();
            GUILayout.Label("Window Handle: " + HandledWindow.Handle);
        }

//        #endregion
    }
}