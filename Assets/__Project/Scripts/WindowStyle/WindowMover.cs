using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMover : MonoBehaviour, IDragHandler
{
    private Vector2 _deltaValue = Vector2.zero;
    
    
    
    private bool _maximizedAfter;
    private Rect _maximizedAfterRect;
    
    
    

    public void OnDrag(PointerEventData eventData)
    {
        _deltaValue += eventData.delta;

        if (eventData.dragging)
        {
            var windowRect = WindowManager.GetWindowRect();

            var rect = new Rect
            {
                x = windowRect.x + _deltaValue.x,
                y = windowRect.y - _deltaValue.y,
                width = windowRect.width,
                height = windowRect.height
            };

//            if (WindowManager.Maximized)
//            {
//                var cursorPositionRelative = CursorManager.GetCursorPositionRelative();
//                var cursorPosition = CursorManager.GetCursorPosition();
//
//
//                Debug.Log("Cursor Position: " + cursorPosition);
//                Debug.Log("Maximized Window Rect: " + windowRect);
//
//                var percent = cursorPositionRelative.x / windowRect.width;
//                Debug.Log("Percent: " + percent);
//
//                WindowStyle.Instance.Maximize(false);
//
//                var normalRect = WindowManager.GetWindowRect();
//                Debug.Log("Normal Window Rect: " + normalRect);
//
//
//                var offset = normalRect.width * percent;
//                Debug.Log("Offset: " + offset);
//
//                normalRect.position = new Vector2(cursorPosition.x - offset, rect.position.y);
//                Debug.Log("Rect Position : " + rect.position);
//                _maximizedAfter = true;
//
//                _maximizedAfterRect = normalRect;
//                return;
//            }
//
//            
//            if (_maximizedAfter)
//            {
//                _maximizedAfter = false;
//                rect = _maximizedAfterRect; 
//                WindowManager.MoveWindow(rect);
//                Debug.Log(rect);
//                return;
//            }
            
            WindowManager.MoveWindow(rect, true);

           
        }
    }
}