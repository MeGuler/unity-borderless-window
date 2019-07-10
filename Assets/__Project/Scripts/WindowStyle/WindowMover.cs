using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMover : MonoBehaviour, IDragHandler
{
    private Vector2 _deltaValue = Vector2.zero;

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

            WindowManager.MoveWindow(rect);
        }
    }
}