using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public struct Pixel
    {
        public static PixelColor DefaultForegroundColor = PixelColor.ConsoleColorToPixelColor[ConsoleColor.White];
        private static PixelColor _DefaultBackgroundColor = PixelColor.ConsoleColorToPixelColor[ConsoleColor.Black];
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
        public static Pixel EmptyButReplace = new Pixel('X', DefaultForegroundColor, DefaultBackgroundColor);



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
            return !(left == right); 
        }

    }
}
