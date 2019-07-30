using Borderless.Api;
using Borderless.Flags;
using UnityEngine;

namespace Borderless
{
    public static class Cursor
    {
        public static Vector2 GetCursorPosition()
        {
            User32.GetCursorPosition(out var point);
            return new Vector2(point.X, point.Y);
        }

        public static HitTestValues GetCursorPositionFlag(Window window)
        {
            var mousePosition = GetCursorPosition();

            return GetSpecificCursorPositionFlags(mousePosition, window);
        }


        public static HitTestValues GetSpecificCursorPositionFlags(Vector2 cursorPoint, Window window)
        {
            var rectPoints = window.GetWindowRectPoints();
            var mousePosition = cursorPoint;
            var resizeBorderSize = window.ResizeBorderSize;

            HitTestValues cursorPositionFlags;

            //Left
            if (MathExtension.IsBetween
                (
                    mousePosition.x,
                    rectPoints.Left,
                    rectPoints.Left + resizeBorderSize.x,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.LeftBorder;

                if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + resizeBorderSize.y,
                    false))
                {
                    cursorPositionFlags = HitTestValues.TopLeftBorder;
                }
                else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - resizeBorderSize.w,
                    rectPoints.Bottom,
                    false))
                {
                    cursorPositionFlags = HitTestValues.BottomLeftBorder;
                }
            }
            //Right
            else if (MathExtension.IsBetween
                (
                    mousePosition.x,
                    rectPoints.Right - resizeBorderSize.z,
                    rectPoints.Right,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.RightBorder;

                if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + resizeBorderSize.y,
                    false))
                {
                    cursorPositionFlags = HitTestValues.TopRightBorder;
                }
                else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - resizeBorderSize.w,
                    rectPoints.Bottom,
                    false))
                {
                    cursorPositionFlags = HitTestValues.BottomRightBorder;
                }
            }
            //Top
            else if (MathExtension.IsBetween
                (
                    mousePosition.y,
                    rectPoints.Top,
                    rectPoints.Top + resizeBorderSize.y,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.TopBorder;
            }
            else if (MathExtension.IsBetween
                (
                    mousePosition.y,
                    rectPoints.Top + resizeBorderSize.y,
                    rectPoints.Top + window.CaptionHeight,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.Caption;
            }
            //Bottom
            else if (MathExtension.IsBetween
                (
                    mousePosition.y,
                    rectPoints.Bottom - resizeBorderSize.w,
                    rectPoints.Bottom,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.BottomBorder;
            }
            //Main
            else
            {
                cursorPositionFlags = HitTestValues.Client;
            }

            return cursorPositionFlags;
        }
    }
}