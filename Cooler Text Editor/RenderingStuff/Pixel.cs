using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public struct Pixel
    {
        public static PixelColor DefaultForegroundColor = PixelColor.White;
        private static PixelColor _DefaultBackgroundColor = PixelColor.Black;
        public static PixelColor DefaultBackgroundColor
        {
            get { return _DefaultBackgroundColor; }
            set
            {
                _DefaultBackgroundColor = value;
                Empty = new Pixel(' ', DefaultForegroundColor, DefaultBackgroundColor);
            }
        }
        public static Pixel Empty = new Pixel(' ', DefaultForegroundColor, DefaultBackgroundColor);
        public static Pixel Transparent = new Pixel(' ', PixelColor.Transparent, PixelColor.Transparent);



        public PixelColor ForegroundColor, BackgroundColor;
        public char Character;

        public Pixel(char character, PixelColor foregroundColor, PixelColor backgroundColor)
        {
            Character = character;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

        public Pixel(char character, PixelColor col)
        {
            Character = character;
            ForegroundColor = col;
            BackgroundColor = DefaultBackgroundColor;
        }

        public Pixel(char character)
        {
            Character = character;
            ForegroundColor = DefaultForegroundColor;
            BackgroundColor = DefaultBackgroundColor;
        }

        public Pixel(PixelColor col)
        {
            Character = ' ';
            ForegroundColor = PixelColor.Transparent;
            BackgroundColor = col;
        }

        public Pixel(PixelColor fg, PixelColor bg)
        {
            Character = ' ';
            ForegroundColor = fg;
            BackgroundColor = bg;
        }

        public Pixel()
        {
            Character = ' ';
            ForegroundColor = DefaultForegroundColor;
            BackgroundColor = DefaultBackgroundColor;
        }

        public static bool operator== (Pixel left, Pixel right)
        {
            if (left.Character == right.Character && 
                left.Character == ' ')
                return left.BackgroundColor == right.BackgroundColor;

            return left.Character == right.Character && 
                left.ForegroundColor == right.ForegroundColor && 
                left.BackgroundColor == right.BackgroundColor;
        }

        public static bool operator!= (Pixel left, Pixel right)
        {
            if (left.Character == right.Character &&
                left.Character == ' ')
                return left.BackgroundColor != right.BackgroundColor;

            return left.Character != right.Character ||
                left.ForegroundColor != right.ForegroundColor ||
                left.BackgroundColor != right.BackgroundColor;
        }

        public override string ToString()
        {
            return $"('{Character}', {ForegroundColor}, {BackgroundColor})";
        }

        public void WriteOver(Pixel other)
        {
            if (!other.BackgroundColor.IsTransparent)
                BackgroundColor = other.BackgroundColor;
            if (!other.ForegroundColor.IsTransparent)
            {
                Character = other.Character;
                ForegroundColor = other.ForegroundColor;
            }
            
        }

        public static Pixel Create2DColPixel(PixelColor top, PixelColor bottom)
        {
            return new Pixel('▄', bottom, top);
        }
    }
}
