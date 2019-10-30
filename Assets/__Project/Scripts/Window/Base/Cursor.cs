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

        public static HitTestValues GetCursorAreaFlags(Window window)
        {
            var mousePosition = GetCursorPosition();
            var rectPoints = window.GetWindowRectPoints();
            var resizeBorderSize = window.ResizeBorderSize;


            return GetCursorAreaFlags(mousePosition, rectPoints, resizeBorderSize, window.CaptionHeight);
        }

        public static HitTestValues GetCursorAreaFlags(Vector2 cursorPoint, Window window)
        {
            var mousePosition = cursorPoint;
            var rectPoints = window.GetWindowRectPoints();
            var resizeBorderSize = window.ResizeBorderSize;


            return GetCursorAreaFlags(mousePosition, rectPoints, resizeBorderSize, window.CaptionHeight);
        }

        public static HitTestValues GetCursorAreaFlags(Vector2 cursorPoint, Rect windowRect,
            Vector4Int borderSize, int captionHeight)
        {
            HitTestValues cursorPositionFlags;

            //Left
            if (MathExtension.IsBetween
                (
                    cursorPoint.x,
                    windowRect.Left,
                    windowRect.Left + borderSize.x,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.LeftBorder;

                if (MathExtension.IsBetween
                    (
                        cursorPoint.y,
                        windowRect.Top,
                        windowRect.Top + borderSize.y,
                        false
                    )
                )
                {
                    cursorPositionFlags = HitTestValues.TopLeftBorder;
                }
                else if (MathExtension.IsBetween
                    (
                        cursorPoint.y,
                        windowRect.Bottom - borderSize.w,
                        windowRect.Bottom,
                        false
                    )
                )
                {
                    cursorPositionFlags = HitTestValues.BottomLeftBorder;
                }
            }
            //Right
            else if (MathExtension.IsBetween
                (
                    cursorPoint.x,
                    windowRect.Right - borderSize.z,
                    windowRect.Right,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.RightBorder;

                if (MathExtension.IsBetween
                    (
                        cursorPoint.y,
                        windowRect.Top,
                        windowRect.Top + borderSize.y,
                        false
                    )
                )
                {
                    cursorPositionFlags = HitTestValues.TopRightBorder;
                }
                else if (MathExtension.IsBetween
                    (
                        cursorPoint.y,
                        windowRect.Bottom - borderSize.w,
                        windowRect.Bottom,
                        false
                    )
                )
                {
                    cursorPositionFlags = HitTestValues.BottomRightBorder;
                }
            }
            //Top
            else if (MathExtension.IsBetween
                (
                    cursorPoint.y,
                    windowRect.Top,
                    windowRect.Top + borderSize.y,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.TopBorder;
            }
            else if (MathExtension.IsBetween
                (
                    cursorPoint.y,
                    windowRect.Top + borderSize.y,
                    windowRect.Top + captionHeight,
                    false
                )
            )
            {
                cursorPositionFlags = HitTestValues.Caption;
            }
            //Bottom
            else if (MathExtension.IsBetween
                (
                    cursorPoint.y,
                    windowRect.Bottom - borderSize.w,
                    windowRect.Bottom,
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