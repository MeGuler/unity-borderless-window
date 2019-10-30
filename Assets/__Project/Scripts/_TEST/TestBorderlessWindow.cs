using Borderless.Api;
using Borderless.Flags;
using UnityEngine;



namespace Borderless
{
    public class TestBorderlessWindow : TestWindow
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Maximize();
            }
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public void Maximize()
        {
            var placement = GetWindowPlacement();

            if (placement.windowShowCommand == WindowShowCommands.Maximize)
            {
                placement.windowShowCommand = WindowShowCommands.Restore;
            }
            else
            {
                placement.windowShowCommand = WindowShowCommands.Maximize;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }

        public void Minimize()
        {
            var placement = GetWindowPlacement();


            if (placement.windowShowCommand == WindowShowCommands.ShowMinimized)
            {
                placement.windowShowCommand = WindowShowCommands.Restore;
            }
            else
            {
                placement.windowShowCommand = WindowShowCommands.ShowMinimized;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            GUI.color = Color.red;
            GUI.Label(new UnityEngine.Rect(50, 50, 150, 50), "Window Handle: " + HandledWindow.Handle);
        }
    }
}