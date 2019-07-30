using System;
using Borderless.Api;
using Borderless.Api.Structures;
using Borderless.Flags;
using UnityEngine;


namespace Borderless
{
    public class BorderlessWindow : Window
    {
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
//            debug.text = MathExtension.GreatestCommonDivisor(1920, 1080).ToString();

//            return;
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
                placement.ShowCommand = ShowWindowCommands.Restore;
            }
            else
            {
                placement.ShowCommand = ShowWindowCommands.Maximize;
            }

            User32.SetWindowPlacement(HandledWindow.Handle, ref placement);
        }
    }
}