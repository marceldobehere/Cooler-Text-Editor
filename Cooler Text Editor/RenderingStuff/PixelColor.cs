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

        public PixelColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static Dictionary<ConsoleColor, PixelColor> ConsoleColorToPixelColor = new Dictionary<ConsoleColor, PixelColor>()
        {
            { ConsoleColor.Black, new PixelColor(0, 0, 0) },
            { ConsoleColor.DarkBlue, new PixelColor(0, 0, 128) },
            { ConsoleColor.DarkGreen, new PixelColor(0, 128, 0) },
            { ConsoleColor.DarkCyan, new PixelColor(0, 128, 128) },
            { ConsoleColor.DarkRed, new PixelColor(128, 0, 0) },
            { ConsoleColor.DarkMagenta, new PixelColor(128, 0, 128) },
            { ConsoleColor.DarkYellow, new PixelColor(128, 128, 0) },
            { ConsoleColor.Gray, new PixelColor(192, 192, 192) },
            { ConsoleColor.DarkGray, new PixelColor(128, 128, 128) },
            { ConsoleColor.Blue, new PixelColor(0, 0, 255) },
            { ConsoleColor.Green, new PixelColor(0, 255, 0) },
            { ConsoleColor.Cyan, new PixelColor(0, 255, 255) },
            { ConsoleColor.Red, new PixelColor(255, 0, 0) },
            { ConsoleColor.Magenta, new PixelColor(255, 0, 255) },
            { ConsoleColor.Yellow, new PixelColor(255, 255, 0) },
            { ConsoleColor.White, new PixelColor(255, 255, 255) },
        };

        public static Dictionary<PixelColor, ConsoleColor> PixelColorToConsoleColor = new Dictionary<PixelColor, ConsoleColor>()
        {
            { new PixelColor(0, 0, 0), ConsoleColor.Black },
            { new PixelColor(0, 0, 128), ConsoleColor.DarkBlue },
            { new PixelColor(0, 128, 0), ConsoleColor.DarkGreen },
            { new PixelColor(0, 128, 128), ConsoleColor.DarkCyan },
            { new PixelColor(128, 0, 0), ConsoleColor.DarkRed },
            { new PixelColor(128, 0, 128), ConsoleColor.DarkMagenta },
            { new PixelColor(128, 128, 0), ConsoleColor.DarkYellow },
            { new PixelColor(192, 192, 192), ConsoleColor.Gray },
            { new PixelColor(128, 128, 128), ConsoleColor.DarkGray },
            { new PixelColor(0, 0, 255), ConsoleColor.Blue },
            { new PixelColor(0, 255, 0), ConsoleColor.Green },
            { new PixelColor(0, 255, 255), ConsoleColor.Cyan },
            { new PixelColor(255, 0, 0), ConsoleColor.Red },
            { new PixelColor(255, 0, 255), ConsoleColor.Magenta },
            { new PixelColor(255, 255, 0), ConsoleColor.Yellow },
            { new PixelColor(255, 255, 255), ConsoleColor.White },
        };


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
            return $"({R}, {G}, {B})";
        }
    }
}
