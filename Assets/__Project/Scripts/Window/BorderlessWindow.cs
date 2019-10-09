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
    }
}