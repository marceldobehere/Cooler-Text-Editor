using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public class Window
    {
        public int Width, Height;
        public Pixel[,] Pixels;
        public Position Pos;
        public List<Window> Windows;

        public Window(int width, int height)
        {
            Width = width;
            Height = height;
            Pos = new Position();
            Windows = new List<Window>();

            Pixels = new Pixel[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Pixels[x, y] = Pixel.Empty;
            
        }

        public void Resize(int nWidth, int nHeight)
        {
            Pixel[,] newPixels = new Pixel[nWidth, nHeight];
            for (int y = 0; y < nHeight; y++)
                for (int x = 0; x < nWidth; x++)
                    newPixels[x, y] = Pixel.Empty;
            for (int y = 0; y < Math.Min(Height, nHeight); y++)
                for (int x = 0; x < Math.Min(Width, nWidth); x++)
                    newPixels[x, y] = Pixels[x, y];
            Pixels = newPixels;
            Width = nWidth;
            Height = nHeight;
        }
    }
}
