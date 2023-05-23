using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.WindowStuff
{
    public class Window
    {
        public Size2D Size;
        public Pixel[,] Pixels;
        public Position2D Pos;
        public List<Window> Windows;

        public Window(Size2D size)
        {
            Size = size;
            Pos = new Position2D();
            Windows = new List<Window>();

            Pixels = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    Pixels[x, y] = Pixel.Empty;

        }

        public void Resize(Size2D nSize)
        {
            Resize(nSize.Width, nSize.Height);
        }

        public void Resize(int nWidth, int nHeight)
        {
            Pixel[,] newPixels = new Pixel[nWidth, nHeight];
            for (int y = 0; y < nHeight; y++)
                for (int x = 0; x < nWidth; x++)
                    newPixels[x, y] = Pixel.Empty;
            for (int y = 0; y < Math.Min(Size.Height, nHeight); y++)
                for (int x = 0; x < Math.Min(Size.Width, nWidth); x++)
                    newPixels[x, y] = Pixels[x, y];
            Pixels = newPixels;
            Size.Width = nWidth;
            Size.Height = nHeight;
        }
    }
}
