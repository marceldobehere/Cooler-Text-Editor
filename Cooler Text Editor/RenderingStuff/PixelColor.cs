using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public struct PixelColor
    {
        public int R, G, B;
        public bool IsTransparent;

        public PixelColor()
        {
            IsTransparent = true;

            R = 0;
            G = 0;
            B = 0;
        }

        public PixelColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;

            IsTransparent = false;
        }

        public static bool operator ==(PixelColor left, PixelColor right)
        {
            return left.R == right.R && left.G == right.G && left.B == right.B;
        }

        public static bool operator !=(PixelColor left, PixelColor right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PixelColor))
                return false;
            PixelColor pixelColor = (PixelColor)obj;
            return this == pixelColor;
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public string GetAnsiColorString()
        {
            return $"2;{R};{G};{B}m";
        }

        public override string ToString()
        {
            if (IsTransparent)
                return $"(Transparent)";
            return $"({R}, {G}, {B})";
        }

        public static PixelColor White = new PixelColor(255, 255, 255);
        public static PixelColor Black = new PixelColor(0, 0, 0);
        public static PixelColor Red = new PixelColor(255, 0, 0);
        public static PixelColor Green = new PixelColor(0, 255, 0);
        public static PixelColor Blue = new PixelColor(0, 0, 255);
        public static PixelColor Yellow = new PixelColor(255, 255, 0);
        public static PixelColor Magenta = new PixelColor(255, 0, 255);
        public static PixelColor Cyan = new PixelColor(0, 255, 255);
        public static PixelColor Gray = new PixelColor(128, 128, 128);
        public static PixelColor DarkGray = new PixelColor(64, 64, 64);
        public static PixelColor DarkRed = new PixelColor(128, 0, 0);
        public static PixelColor DarkGreen = new PixelColor(0, 128, 0);
        public static PixelColor DarkBlue = new PixelColor(0, 0, 128);
        public static PixelColor DarkYellow = new PixelColor(128, 128, 0);
        public static PixelColor DarkMagenta = new PixelColor(128, 0, 128);
        public static PixelColor DarkCyan = new PixelColor(0, 128, 128);
        public static PixelColor Transparent = new PixelColor();
        

    }
}
