using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;

public struct WindowRect
{
    public readonly int
        Left,
        Top,
        Right,
        Bottom;

    public WindowRect(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}

public struct CursorPoint
{
    public int X;
    public int Y;


    public CursorPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public CursorPoint(CursorPoint cursorPoint)
    {
        X = cursorPoint.X;
        Y = cursorPoint.Y;
    }

    public static CursorPoint operator +(CursorPoint cursorPoint1, CursorPoint cursorPoint2)
    {
        var cursorPoint = new CursorPoint
        {
            X = cursorPoint1.X + cursorPoint2.X,
            Y = cursorPoint1.Y + cursorPoint2.Y
        };


        return cursorPoint;
    }

    public static CursorPoint operator -(CursorPoint cursorPoint1, CursorPoint cursorPoint2)
    {
        var cursorPoint = new CursorPoint
        {
            X = cursorPoint1.X - cursorPoint2.X,
            Y = cursorPoint1.Y - cursorPoint2.Y
        };

        return cursorPoint;
    }

    public static CursorPoint operator /(CursorPoint cursorPoint1, CursorPoint cursorPoint2)
    {
        var cursorPoint = new CursorPoint
        {
            X = cursorPoint1.X / cursorPoint2.X,
            Y = cursorPoint1.Y / cursorPoint2.Y
        };

        return cursorPoint;
    }

    public static CursorPoint operator *(CursorPoint cursorPoint1, CursorPoint cursorPoint2)
    {
        var cursorPoint = new CursorPoint
        {
            X = cursorPoint1.X * cursorPoint2.X,
            Y = cursorPoint1.Y * cursorPoint2.Y
        };

        return cursorPoint;
    }
}

[Serializable]
public class CursorData
{
    public string name;
    public Texture2D image;
    public Vector2 offset;
}

[Serializable]
public class Vector4Int
{
    [Description("Left")] public int x;
    [Description("Top")] public int y;
    [Description("Right")] public int z;
    [Description("Bottom")] public int w;
}



[StructLayout(LayoutKind.Sequential)]
public struct PAINTSTRUCT
{
    public IntPtr hdc;
    public bool fErase;
    public RECT rcPaint;
    public bool fRestore;
    public bool fIncUpdate;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] rgbReserved;
}

[StructLayout(LayoutKind.Sequential)]
public struct MARGINS
{
    public int leftWidth;
    public int rightWidth;
    public int topHeight;
    public int bottomHeight;
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left, Top, Right, Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
    {
    }

    public int X
    {
        get { return Left; }
        set
        {
            Right -= (Left - value);
            Left = value;
        }
    }

    public int Y
    {
        get { return Top; }
        set
        {
            Bottom -= (Top - value);
            Top = value;
        }
    }

    public int Height
    {
        get { return Bottom - Top; }
        set { Bottom = value + Top; }
    }

    public int Width
    {
        get { return Right - Left; }
        set { Right = value + Left; }
    }

    public System.Drawing.Point Location
    {
        get { return new System.Drawing.Point(Left, Top); }
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public System.Drawing.Size Size
    {
        get { return new System.Drawing.Size(Width, Height); }
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public static implicit operator System.Drawing.Rectangle(RECT r)
    {
        return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
    }

    public static implicit operator RECT(System.Drawing.Rectangle r)
    {
        return new RECT(r);
    }

    public static bool operator ==(RECT r1, RECT r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(RECT r1, RECT r2)
    {
        return !r1.Equals(r2);
    }

    public bool Equals(RECT r)
    {
        return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
    }

    public override bool Equals(object obj)
    {
        if (obj is RECT)
            return Equals((RECT) obj);
        else if (obj is System.Drawing.Rectangle)
            return Equals(new RECT((System.Drawing.Rectangle) obj));
        return false;
    }

    public override int GetHashCode()
    {
        return ((System.Drawing.Rectangle) this).GetHashCode();
    }

    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.CurrentCulture,
            "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static implicit operator System.Drawing.Point(POINT p)
    {
        return new System.Drawing.Point(p.X, p.Y);
    }

    public static implicit operator POINT(System.Drawing.Point p)
    {
        return new POINT(p.X, p.Y);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct MINMAXINFO
{
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxPosition;
    public POINT ptMinTrackSize;
    public POINT ptMaxTrackSize;
}
