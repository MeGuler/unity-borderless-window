using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borderless
{
    public class BorderlessWindow : Window
    {
        public Vector2Int minWindowSize;
        public Vector2Int maxWindowSize;
        public Vector4Int resizeHandleSize;
        public int captionHeight;

        protected override void Start()
        {
            MinWindowSize = minWindowSize;
            MaxWindowSize = maxWindowSize;
            ResizeHandleSize = resizeHandleSize;
            CaptionHeight = captionHeight;
            base.Start();
        }

        public void ExitApplication()
        {
            Application.Quit();
        }
    }
    
    
    
}
