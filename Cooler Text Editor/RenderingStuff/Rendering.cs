using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public class Rendering
    {
        public static Pixel[,] ScreenBackbuffer = new Pixel[1, 1];
        
        public static bool CheckWindowResize()
        {
            int cSizeX = Console.WindowWidth;
            if (cSizeX < 1)
                cSizeX = 1;
            int cSizeY = Console.WindowHeight;
            if (cSizeY < 1)
                cSizeY = 1;

            if (cSizeX == Program.MainScreen.Size.Width && 
                cSizeY == Program.MainScreen.Size.Height)
                return false;

            // Might Remove Later
            StringBuilder renderString = new StringBuilder();
            renderString.Append($"\u001b[38;{Pixel.DefaultForegroundColor.GetAnsiColorString()}");
            renderString.Append($"\u001b[48;{Pixel.DefaultBackgroundColor.GetAnsiColorString()}");
            renderString.Append("\u001b[2J");
            Console.Write(renderString);

            //Program.MainScreen.Resize(cSizeX, cSizeY);
            Program.MainScreen.Resize(cSizeX, cSizeY);

            ScreenBackbuffer = new Pixel[cSizeX, cSizeY];
            for (int y = 0; y < cSizeY; y++)
                for (int x = 0; x < cSizeX; x++)
                    ScreenBackbuffer[x, y] = Pixel.Empty;

            return true;
        }

        private static void RenderInternalField(Pixel[,] screen, Field2D updateField, List<Position2D> updatedPixels)
        {
            if (updateField.TL.X < 0)
                updateField.TL.X = 0;
            if (updateField.TL.Y < 0)
                updateField.TL.Y = 0;
            if (updateField.BR.X > ScreenBackbuffer.GetLength(0) - 1)
                updateField.BR.X = ScreenBackbuffer.GetLength(0) - 1;
            if (updateField.BR.Y > ScreenBackbuffer.GetLength(1) - 1)
                updateField.BR.Y = ScreenBackbuffer.GetLength(1) - 1;
            if (updateField.BR.X > screen.GetLength(0) - 1)
                updateField.BR.X = screen.GetLength(0) - 1;
            if (updateField.BR.Y > screen.GetLength(1) - 1)
                updateField.BR.Y = screen.GetLength(1) - 1;

            for (int y = updateField.TL.Y; y <= updateField.BR.Y; y++)
                for (int x = updateField.TL.X; x <= updateField.BR.X; x++)
                {
                    Pixel newPixel = screen[x, y];
                    if (ScreenBackbuffer[x, y] != newPixel)
                    {
                        ScreenBackbuffer[x, y] = newPixel;
                        updatedPixels.Add(new Position2D(x, y));
                    }
                }
        }

        public static void RenderFields(Pixel[,] screen, List<Field2D> fieldUpdates)
        {
            List<Position2D> updatedPixels = new List<Position2D>();
            foreach (Field2D field in fieldUpdates)
                RenderInternalField(screen, field, updatedPixels);

            DoActualRender(updatedPixels);
        }

        public static void DoActualRender(List<Position2D> updatedPoints)
        {
            StringBuilder renderString = new StringBuilder();
            renderString.Append("\x1b[" + (0) + ";" + (0) + "H");

            foreach (Position2D pos in updatedPoints)
            {
                int x = pos.X;
                int y = pos.Y;

                Pixel pxl = ScreenBackbuffer[x, y];
                renderString.Append("\x1b[" + (y) + ";" + (x) + "H");
                renderString.Append($"\u001b[38;{pxl.ForegroundColor.GetAnsiColorString()}");
                renderString.Append($"\u001b[48;{pxl.BackgroundColor.GetAnsiColorString()}");
                renderString.Append(pxl.Character);
            }

            renderString.Append("\x1b[" + (0) + ";" + (0) + "H");
            renderString.Append($"\u001b[38;{Pixel.DefaultForegroundColor.GetAnsiColorString()}");
            renderString.Append($"\u001b[48;{Pixel.DefaultBackgroundColor.GetAnsiColorString()}");

            Console.Write(renderString);
        }

        //public static void RenderMainWindowOLD()
        //{
        //    StringBuilder renderString = new StringBuilder();
        //    renderString.Append("\x1b[" + (0) + ";" + (0) + "H");
        //    for (int y = 0; y < Program.MainWindow.Size.Height; y++)
        //        for (int x = 0; x < Program.MainWindow.Size.Width; x++)
        //            if (ScreenBackbuffer[x, y] != Program.MainWindow.Pixels[x, y])
        //            {
        //                Pixel pxl = Program.MainWindow.Pixels[x, y];
        //                renderString.Append("\x1b[" + (y) + ";" + (x) + "H");
        //                renderString.Append($"\u001b[38;{pxl.ForegroundColor.GetAnsiColorString()}");
        //                renderString.Append($"\u001b[48;{pxl.BackgroundColor.GetAnsiColorString()}");
        //                renderString.Append(pxl.Character);
        //                //renderString.Append("X");
        //                ScreenBackbuffer[x, y] = pxl;
        //            }

        //    renderString.Append("\x1b[" + (0) + ";" + (0) + "H");
        //    renderString.Append($"\u001b[38;{Pixel.DefaultForegroundColor.GetAnsiColorString()}");
        //    renderString.Append($"\u001b[48;{Pixel.DefaultBackgroundColor.GetAnsiColorString()}");

        //    Console.Write(renderString);
        //}

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
