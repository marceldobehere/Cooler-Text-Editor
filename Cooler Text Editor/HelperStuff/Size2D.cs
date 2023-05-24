using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.WindowStuff
{
    public struct Size2D
    {
        public int Width;
        public int Height;

        public Size2D(int width, int height)
        {
            Width = width;
            Height = height;
            SizeBasedOnParent = null;
        }   

        public Size2D(Pixel[,] arr)
        {
            Width = arr.GetLength(0);
            Height = arr.GetLength(1);
            SizeBasedOnParent = null;
        }

        public Size2D()
        {
            Width = 0;
            Height = 0;
            SizeBasedOnParent = null;
        }

        public Size2D(Func<Size2D, Size2D> sizeBasedOnParent)
        {
            Width = 0;
            Height = 0;
            SizeBasedOnParent = sizeBasedOnParent;
        }

        public static Size2D operator +(Size2D a, Size2D b)
        {
            return new Size2D(a.Width + b.Width, a.Height + b.Height);
        }

        public static Size2D operator -(Size2D a, Size2D b)
        {
            return new Size2D(a.Width - b.Width, a.Height - b.Height);
        }

        public static bool operator ==(Size2D a, Size2D b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(Size2D a, Size2D b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"({Width}, {Height})";
        }

        public static Size2D operator +(Size2D a, Position2D b)
        {
            return new Size2D(a.Width + b.X, a.Height + b.Y);
        }

        public static Size2D operator -(Size2D a, Position2D b)
        {
            return new Size2D(a.Width - b.X, a.Height - b.Y);
        }




        public Func<Size2D, Size2D> SizeBasedOnParent;
    }
}
