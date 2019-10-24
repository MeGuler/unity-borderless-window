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


            if (placement.windowShowCommand == WindowShowCommands.Maximize)
            {
                maximizeImage.sprite = maximizeIcon1;
                placement.windowShowCommand = WindowShowCommands.Restore;
            }
            else
            {
                maximizeImage.sprite = maximizeIcon2;
                placement.windowShowCommand = WindowShowCommands.Maximize;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
//            User32.ShowWindow(HandledWindow.Handle, (int) placement.windowShowCommand);
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