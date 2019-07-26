using Borderless.Api.Structures;
using Borderless.Flags;
using UnityEngine;


namespace Borderless
{
    public class BorderlessWindow : Window
    {
        #region WindowSettings

        public Vector4Int resizeBorderSize;
        public Vector2Int minWindowSize;
        public Vector2Int maxWindowSize;
        public Vector4Int resizeHandleSize;
        public int captionHeight;

        #endregion

        protected override void Awake()
        {
            ResizeBorderSize = resizeBorderSize;
            MinWindowSize = minWindowSize;
            MaxWindowSize = maxWindowSize;
            ResizeHandleSize = resizeHandleSize;
            CaptionHeight = captionHeight;
            base.Awake();
        }
        
        public void ExitApplication()
        {
            Application.Quit();
        }

        public void Maximize()
        {
            ShowWindow(WindowShowStyle.Maximize);
        }
        
    }
}