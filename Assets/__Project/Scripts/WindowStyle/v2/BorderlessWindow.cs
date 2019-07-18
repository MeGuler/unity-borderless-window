using System;
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

        [SerializeField] private Camera mainCamera;




        protected override void Start()
        {
            MinWindowSize = minWindowSize;
            MaxWindowSize = maxWindowSize;
            ResizeHandleSize = resizeHandleSize;
            CaptionHeight = captionHeight;
            base.Start();
        }

        protected override void OnGUI()
        {
            
            
            
            var rayOrigin = mainCamera.ScreenPointToRay(Input.mousePosition).origin;
            var rayDirection = mainCamera.ScreenPointToRay(Input.mousePosition).direction;
            ClickThrough = !Physics.Raycast(rayOrigin, rayDirection, out _, 100, Physics.DefaultRaycastLayers);

            if (ClickThrough != PreviousClickThrough)
            {

//                if (ClickThrough)
//                {
////                    WinApi.SetWindowLongPtr(_handledWindow, -20, (IntPtr) ((uint) 524288 | (uint) 32));
//                    //other code
////                    WindowStyleFlags.
//                }
//                else
//                {
////                    WinApi.SetWindowLongPtr(_handledWindow, -20,
////                        (IntPtr) (WindowStyleFlags.Popup | WindowStyleFlags.Visible));
//                    //other code
//                }

                PreviousClickThrough = ClickThrough;
            }

            if (!ClickThrough)
            {
                if (_handledWindow.Handle == IntPtr.Zero)
                {
                    InitializeWindowProcedure();
                }
            }
            else
            {
                TerminateWindowProcedure();
            }
            
           
            

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