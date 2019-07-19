using System;
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
        

        protected override void OnGUI()
        {
//            _pointerEventData = new PointerEventData(EventSystem.current);
//            _pointerEventData.position = Input.mousePosition;
//
//            _raycastResult.Clear();
//
//            raycaster.Raycast(_pointerEventData, _raycastResult);
//
//            ClickThrough = _raycastResult.Count > 0;
//
//            if (ClickThrough != PreviousClickThrough)
//            {
//                if (!ClickThrough)
//                {
//                    InitializeWindowProcedure();
//                }
//                else
//                {
//                    TerminateWindowProcedure();
//                }
//
//                PreviousClickThrough = ClickThrough;
//            }
//            
//            Debug.Log(ClickThrough);
//            
//            if (!ClickThrough)
//            {
                if (_handledWindow.Handle == IntPtr.Zero)
                {
                    InitializeWindowProcedure();
                }
//            }


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