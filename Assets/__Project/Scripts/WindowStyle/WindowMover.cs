using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMover : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 _deltaValue = Vector2.zero;

    private Rect _rect;
    
    public void OnDrag(PointerEventData eventData)
    {
        if (WindowManager.Maximized)
        {
            return;
        }
        
        _deltaValue += eventData.delta;

        if (eventData.dragging)
        {
//            if (WindowManager.Maximized)
//            {
//                Debug.Log("Maximized");
//                var cursorPositionRelative = CursorManager.GetCursorPositionRelative();
//                var cursorPosition = CursorManager.GetCursorPosition();
//
//                var percent = cursorPositionRelative.x / _rect.width;
//
//                WindowStyle.Instance.Maximize(false);
//
//                var normalRect = WindowManager.GetWindowRect();
//
//                var offset = normalRect.width * percent;
//                normalRect.position = new Vector2(cursorPosition.x - offset, normalRect.position.y);
//                
//                _rect = normalRect;
//            }
            
            
            _rect.x += _deltaValue.x;
            _rect.y -= _deltaValue.y;
            WindowManager.MoveWindow(_rect, false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rect = WindowManager.GetWindowRect();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}