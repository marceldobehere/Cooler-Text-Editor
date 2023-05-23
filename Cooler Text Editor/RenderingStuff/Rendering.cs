﻿using Cooler_Text_Editor.RenderingStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public class Rendering
    {
        public static Pixel[,] ScreenBackbuffer = new Pixel[1, 1];
        
        public static void CheckWindowResize()
        {
            int cSizeX = Console.WindowWidth;
            if (cSizeX < 1)
                cSizeX = 1;
            int cSizeY = Console.WindowHeight;
            if (cSizeY < 1)
                cSizeY = 1;

            if (cSizeX == Program.MainWindow.Size.Width && 
                cSizeY == Program.MainWindow.Size.Height)
                return;

            // Might Remove Later
            Console.Write("\u001b[2J");

            Program.MainWindow.Resize(cSizeX, cSizeY);

            ScreenBackbuffer = new Pixel[cSizeX, cSizeY];
            for (int y = 0; y < cSizeY; y++)
                for (int x = 0; x < cSizeX; x++)
                    ScreenBackbuffer[x, y] = Pixel.Empty;
        }

        public static void RenderMainWindow()
        {
            StringBuilder renderString = new StringBuilder();
            renderString.Append("\x1b[" + (0) + ";" + (0) + "H");
            for (int y = 0; y < Program.MainWindow.Size.Height; y++)
                for (int x = 0; x < Program.MainWindow.Size.Width; x++)
                    if (ScreenBackbuffer[x, y] != Program.MainWindow.Pixels[x, y])
                    {
                        Pixel pxl = Program.MainWindow.Pixels[x, y];
                        renderString.Append("\x1b[" + (y) + ";" + (x) + "H");
                        renderString.Append($"\u001b[38;{pxl.ForegroundColor.GetAnsiColorString()}");
                        renderString.Append($"\u001b[48;{pxl.BackgroundColor.GetAnsiColorString()}");
                        renderString.Append(pxl.Character);
                        //renderString.Append("X");
                        ScreenBackbuffer[x, y] = pxl;
                    }
            Console.Write(renderString);
        }

        public static void FillPixel(Pixel[,] screen, Field2D field, Pixel pxl)
        {
            if (field.BR.X < 0)
                field.BR.X = 0;
            if (field.BR.Y < 0)
                field.BR.Y = 0;
            if (field.TL.X >= screen.GetLength(0))
                field.TL.X = screen.GetLength(0) - 1;
            if (field.TL.Y >= screen.GetLength(1))
                field.TL.Y = screen.GetLength(1) - 1;

            for (int y = field.TL.Y; y < field.BR.Y; y++)
                for (int x = field.TL.X; x < field.BR.X; x++)
                    screen[x, y] = pxl;
        }
    }
}
