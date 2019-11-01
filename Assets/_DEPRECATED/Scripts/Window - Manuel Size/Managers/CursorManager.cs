using System;
using System.Drawing;
using Window;
using UnityEngine;
using Window.Apis;
using Window.Flags;

namespace Window.Deprecated
{
    [Obsolete("This usage has been deprecated.", false)]
    public static class CursorManager
    {
        public static Vector2 GetCursorPosition()
        {
            User32.GetCursorPosition(out var point);
            return new Vector2(point.X, point.Y);
        }

        public static Vector2 GetCursorPositionRelative()
        {
            var point = GetCursorPosition();
            var windowRect = WindowManager.GetWindowRect();

            var relativePoint = new Vector2
            {
                x = point.x - (int) windowRect.x,
                y = point.y - (int) windowRect.y
            };

            return relativePoint;
        }

        public static Vector2 GetCursorPositionRelative(Vector2 point)
        {
            var windowRect = WindowManager.GetWindowRect();

            var relativePoint = new Vector2
            {
                x = point.x - (int) windowRect.x,
                y = point.y - (int) windowRect.y
            };

            return relativePoint;
        }

        public static Point GetCursorPositionRelative(Point point)
        {
            var windowRect = WindowManager.GetWindowRect();

            var relativePoint = new Point
            {
                X = point.X - (int) windowRect.x,
                Y = point.Y - (int) windowRect.y
            };

            return relativePoint;
        }

        public static HitTestValues GetCursorPositionFlag()
        {
            var mousePosition = GetCursorPosition();

            return GetSpecificCursorPositionFlags(mousePosition);
        }

        public static HitTestValues GetSpecificCursorPositionFlags(Vector2 cursorPoint)
        {
            var rectPoints = WindowManager.GetWindowRectPoints();
            var mousePosition = cursorPoint;
            var borderSize = WindowManager.BorderSize;

            HitTestValues cursorPositionFlags;

            if (MathExtension.IsBetween(mousePosition.x, rectPoints.Left, rectPoints.Left + borderSize.x, true))
            {
                cursorPositionFlags = HitTestValues.LeftBorder;

                if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + borderSize.y, true))
                {
                    cursorPositionFlags = HitTestValues.TopLeftBorder;
                }
                else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - borderSize.w, rectPoints.Bottom,
                    true))
                {
                    cursorPositionFlags = HitTestValues.BottomLeftBorder;
                }
            }
            else if (MathExtension.IsBetween(mousePosition.x, rectPoints.Right - borderSize.z, rectPoints.Right, true))
            {
                cursorPositionFlags = HitTestValues.RightBorder;

                if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + borderSize.y, true))
                {
                    cursorPositionFlags = HitTestValues.TopRightBorder;
                }
                else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - borderSize.w, rectPoints.Bottom,
                    true))
                {
                    cursorPositionFlags = HitTestValues.BottomRightBorder;
                }
            }
            else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + borderSize.y, true))
            {
                cursorPositionFlags = HitTestValues.TopBorder;
            }
            else if (MathExtension.IsBetween(mousePosition.y, rectPoints.Bottom - borderSize.w, rectPoints.Bottom,
                true))
            {
                cursorPositionFlags = HitTestValues.BottomBorder;
            }
            else
            {
                cursorPositionFlags = HitTestValues.Client;
            }

            return cursorPositionFlags;
        }

        public static Vector2 GetEdgeDistance(out HitTestValues cursorPositionFlag)
        {
            var globalCursorPosition = GetCursorPosition();
            var windowRectPoints = WindowManager.GetWindowRectPoints();

            cursorPositionFlag = GetCursorPositionFlag();

            var distance = new Vector2();

            if (cursorPositionFlag == HitTestValues.LeftBorder)
            {
                distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Left);
            }

            else if (cursorPositionFlag == HitTestValues.RightBorder)
            {
                distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Right);
            }

            else if (cursorPositionFlag == HitTestValues.TopBorder)
            {
                distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Top);
            }

            else if (cursorPositionFlag == HitTestValues.BottomBorder)
            {
                distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Bottom);
            }

            else if (cursorPositionFlag == HitTestValues.TopLeftBorder)
            {
                distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Left);
                distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Top);
            }

            else if (cursorPositionFlag == HitTestValues.TopRightBorder)
            {
                distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Right);
                distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Top);
            }

            else if (cursorPositionFlag == HitTestValues.BottomLeftBorder)
            {
                distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Left);
                distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Bottom);
            }
            else if (cursorPositionFlag == HitTestValues.BottomRightBorder)
            {
                distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Right);
                distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Bottom);
            }

            return distance;
        }
    }

}
