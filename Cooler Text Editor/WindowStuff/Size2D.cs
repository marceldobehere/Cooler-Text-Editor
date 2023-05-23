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
        }   

        public Size2D()
        {
            Width = 0;
            Height = 0;
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
    }
}
