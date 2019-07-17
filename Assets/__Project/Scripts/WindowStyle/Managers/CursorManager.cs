using System;
using System.Drawing;
using Borderless;
using UnityEngine;
public static class CursorManager
{
    public static Vector2 GetCursorPosition()
    {
        WinApi.GetCursorPosition(out var point);
        return new Vector2(point.X, point.Y);
    }
    
    public static Vector2 GetCursorPositionRelative()
    {
        var point = GetCursorPosition();
        var windowRect = WindowManager.GetWindowRect();

        var relativePoint = new Vector2
        {
            x = point.x - (int)windowRect.x,
            y = point.y - (int)windowRect.y
        };
        
        return relativePoint;
    }
    
    public static Vector2 GetCursorPositionRelative(Vector2 point)
    {
        var windowRect = WindowManager.GetWindowRect();

        var relativePoint = new Vector2
        {
            x = point.x - (int)windowRect.x,
            y = point.y - (int)windowRect.y
        };
        
        return relativePoint;
    }
    
    public static Point GetCursorPositionRelative(Point point)
    {
        var windowRect = WindowManager.GetWindowRect();

        var relativePoint = new Point
        {
            X = point.X - (int)windowRect.x,
            Y = point.Y - (int)windowRect.y
        };
        
        return relativePoint;
    }

    public static CursorPositionFlags GetCursorPositionFlag()
    {
        var mousePosition = GetCursorPosition();

        return GetSpecificCursorPositionFlags(mousePosition);
    }

    public static CursorPositionFlags GetSpecificCursorPositionFlags(Vector2 cursorPoint)
    {
        
        var rectPoints = WindowManager.GetWindowRectPoints();
        var mousePosition = cursorPoint;
        var borderSize = WindowManager.BorderSize;
        
        CursorPositionFlags cursorPositionFlags;

        if (MathfExtension.IsBetween(mousePosition.x, rectPoints.Left, rectPoints.Left + borderSize.x, true))
        {
            cursorPositionFlags = CursorPositionFlags.Left;

            if (MathfExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + borderSize.y, true))
            {
                cursorPositionFlags = CursorPositionFlags.TopLeft;
            }
            else if (MathfExtension.IsBetween(mousePosition.y, rectPoints.Bottom - borderSize.w, rectPoints.Bottom,
                true))
            {
                cursorPositionFlags = CursorPositionFlags.BottomLeft;
            }
        }
        else if (MathfExtension.IsBetween(mousePosition.x, rectPoints.Right - borderSize.z, rectPoints.Right, true))
        {
            cursorPositionFlags = CursorPositionFlags.Right;

            if (MathfExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + borderSize.y, true))
            {
                cursorPositionFlags = CursorPositionFlags.TopRight;
            }
            else if (MathfExtension.IsBetween(mousePosition.y, rectPoints.Bottom - borderSize.w, rectPoints.Bottom,
                true))
            {
                cursorPositionFlags = CursorPositionFlags.BottomRight;
            }
        }
        else if (MathfExtension.IsBetween(mousePosition.y, rectPoints.Top, rectPoints.Top + borderSize.y, true))
        {
            cursorPositionFlags = CursorPositionFlags.Top;
        }
        else if (MathfExtension.IsBetween(mousePosition.y, rectPoints.Bottom - borderSize.w, rectPoints.Bottom, true))
        {
            cursorPositionFlags = CursorPositionFlags.Bottom;
        }
        else
        {
            cursorPositionFlags = CursorPositionFlags.Main;
        }

        return cursorPositionFlags;
    }
    
     public static Vector2 GetEdgeDistance(out CursorPositionFlags cursorPositionFlag)
    {
        var globalCursorPosition = GetCursorPosition();
        var windowRectPoints = WindowManager.GetWindowRectPoints();
        
        cursorPositionFlag = GetCursorPositionFlag();

        var distance = new Vector2();
        
        if (cursorPositionFlag == CursorPositionFlags.Left)
        {
            distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Left);
        }

        else if (cursorPositionFlag == CursorPositionFlags.Right)
        {
            distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Right);
        }

        else if (cursorPositionFlag == CursorPositionFlags.Top)
        {
            distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Top);
        }

        else if (cursorPositionFlag == CursorPositionFlags.Bottom)
        {
            distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Bottom);
        }

        else if (cursorPositionFlag == CursorPositionFlags.TopLeft)
        {
            distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Left);
            distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Top);
        }

        else if (cursorPositionFlag == CursorPositionFlags.TopRight)
        {
            distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Right);
            distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Top);
        }

        else if (cursorPositionFlag == CursorPositionFlags.BottomLeft)
        {
            distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Left);
            distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Bottom);
        }
        else if (cursorPositionFlag == CursorPositionFlags.BottomRight)
        {
            distance.x = Mathf.Abs(globalCursorPosition.x - windowRectPoints.Right);
            distance.y = Mathf.Abs(globalCursorPosition.y - windowRectPoints.Bottom);
        }

        return distance;
    }
}