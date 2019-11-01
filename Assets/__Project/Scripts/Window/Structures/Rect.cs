using System.Drawing;
using System.Runtime.InteropServices;

namespace Window
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rect(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }

        public int X
        {
            get => Left;
            set
            {
                Right -= (Left - value);
                Left = value;
            }
        }

        public int Y
        {
            get => Top;
            set
            {
                Bottom -= (Top - value);
                Top = value;
            }
        }

        public int Height
        {
            get => Bottom - Top;
            set => Bottom = value + Top;
        }

        public int Width
        {
            get => Right - Left;
            set => Right = value + Left;
        }

        public Point Location
        {
            get => new Point(Left, Top);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public static implicit operator Rectangle(Rect r)
        {
            return new Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator Rect(Rectangle r)
        {
            return new Rect(r);
        }

        public static bool operator ==(Rect r1, Rect r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Rect r1, Rect r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(Rect r)
        {
            return r.Left == Left &&
                   r.Top == Top &&
                   r.Right == Right &&
                   r.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Rect rect:
                    return Equals(rect);
                case Rectangle rectangle:
                    return Equals(new Rect(rectangle));
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return ((Rectangle) this).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format
            (
                System.Globalization.CultureInfo.CurrentCulture,
                "{{Left={0},Top={1},Right={2},Bottom={3}}}",
                Left,
                Top,
                Right,
                Bottom
            );
        }
    }
}