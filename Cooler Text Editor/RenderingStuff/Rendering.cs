using Cooler_Text_Editor.ComponentStuff;
using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cooler_Text_Editor.HelperStuff.Cursor;

namespace Cooler_Text_Editor
{
    public class Rendering
    {
        public static Pixel[,] ScreenBackbuffer = new Pixel[0, 0];

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
            Field2D Scr1 = new Field2D(new Size2D(ScreenBackbuffer));
            Field2D Scr2 = new Field2D(new Size2D(screen));
            Field2D Scr3 = Scr1.MergeMinField(Scr2);
            updateField = Scr3.MergeMinField(updateField);

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

        public static void RenderFields(Pixel[,] screen, HashSet<Field2D> fieldUpdates)
        {
            List<Position2D> updatedPixels = new List<Position2D>();
            foreach (Field2D field in fieldUpdates)
                RenderInternalField(screen, field, updatedPixels);

            DoActualRender(updatedPixels);
        }

        public static void InitCursor()
        {
            Position2D actualCursorPos = GetActualCursorPosition(MainCursor, false);
            Position2D inbounds = GetInBoundCursor(actualCursorPos);

            OldCursorPosition = actualCursorPos;
            OldCursorMode = MainCursor.CursorMode;
            OldCursorShown = MainCursor.CursorShown;


            StringBuilder renderString = new StringBuilder();
            renderString.Append("\x1b[" + (0 + 1) + ";" + (0 + 1) + "H");
            //ESC [ ? 12 l
            //renderString.Append("\x1b[?25l");

            // ESC [ 6 SP q	
            renderString.Append($"\x1b[{(int)OldCursorMode} q");
            renderString.Append("\x1b[" + (inbounds.X + 1) + ";" + (inbounds.Y + 1) + "H");


            Console.Write(renderString);
        }

        public static Position2D OldCursorPosition;
        public static CursorModeEnum OldCursorMode;
        public static bool OldCursorShown;

        public static Position2D GetActualCursorPosition(Cursor thing, bool allowOutOfComponentBounds)
        {
            if (allowOutOfComponentBounds)
                return thing.CursorPosition + thing.CursorComponent.GetAbsolutePosition();
            else
            {
                //return GetInBoundCursor(thing.CursorPosition, new Field2D(thing.CursorComponent.Size)) + thing.CursorComponent.GetAbsolutePosition();
                Position2D mPos = thing.CursorPosition;
                BasicComponent tParent = thing.CursorComponent;
                while (tParent != null)
                {
                    mPos = GetInBoundCursor(mPos + tParent.Position, tParent.GetField());
                    tParent = tParent.Parent;
                }

                return mPos;
            }
        }

        public static Position2D GetInBoundCursor(Position2D cursor)
        {
            return GetInBoundCursor(cursor, new Field2D(new Size2D(ScreenBackbuffer)));
        }

        public static Position2D GetInBoundCursor(Position2D cursor, Field2D field)
        {
            Position2D tempCursor = cursor;
            if (tempCursor.X < field.TL.X)
                tempCursor.X = field.TL.X;
            if (tempCursor.Y < field.TL.Y)
                tempCursor.Y = field.TL.Y;
            if (tempCursor.X > field.BR.X)
                tempCursor.X = field.BR.X;
            if (tempCursor.Y > field.BR.Y)
                tempCursor.Y = field.BR.Y;

            return tempCursor;
        }

        static int Bruh = 0;
        public static void DoActualRender(List<Position2D> updatedPoints)
        {
            Position2D actualCursorPos = GetActualCursorPosition(MainCursor, false);
            Position2D inbounds = GetInBoundCursor(actualCursorPos);
            Bruh = (Bruh + 16) % 100;
            if (updatedPoints.Count == 0)
            {
                StringBuilder tBuilder = new StringBuilder();
                if (MainCursor.CursorMode != OldCursorMode)
                {
                    tBuilder.Append($"\x1b[{(int)MainCursor.CursorMode} q");
                    OldCursorMode = MainCursor.CursorMode;
                }

                if (actualCursorPos != OldCursorPosition)
                {
                    tBuilder.Append("\x1b[" + (inbounds.Y + 1) + ";" + (inbounds.X + 1) + "H");
                    OldCursorPosition = actualCursorPos;
                }

                if (MainCursor.CursorShown != OldCursorShown)
                {
                    if (MainCursor.CursorShown)
                        tBuilder.Append("\x1b[?25h");
                    else
                        tBuilder.Append("\x1b[?25l");
                    OldCursorShown = MainCursor.CursorShown;
                }

                if (tBuilder.Length > 0)
                    Console.Write(tBuilder);
                return;
            }


            StringBuilder renderString = new StringBuilder();
            //renderString.Append("\x1b[" + (inbounds.Y) + ";" + (inbounds.X) + "H");
            //ESC [ ? 12 l
            renderString.Append("\x1b[?25l");

            int lastX = updatedPoints[0].X + 100;
            int lastY = updatedPoints[0].Y + 100;
            PixelColor lastFG = PixelColor.Transparent;
            PixelColor lastBG = PixelColor.Transparent;

            foreach (Position2D pos in updatedPoints)
            {
                int x = pos.X;
                int y = pos.Y;

                Pixel pxl = ScreenBackbuffer[x, y];
                if (y != lastY || x != lastX + 1)
                    renderString.Append("\x1b[" + (y + 1) + ";" + (x + 1) + "H");

                if (pxl.ForegroundColor != lastFG)
                    renderString.Append($"\u001b[38;{pxl.ForegroundColor.GetAnsiColorString()}");
                if (pxl.BackgroundColor != lastBG)
                    renderString.Append($"\u001b[48;{pxl.BackgroundColor.GetAnsiColorString()}");
                PixelColor Bruh2 = pxl.BackgroundColor;
                Bruh2.R = 0;
                Bruh2.G = Bruh;
                Bruh2.B = 0;
                //renderString.Append($"\u001b[48;{Bruh2.GetAnsiColorString()}");
                if ("\0\n\r\t\u001b\b".Contains(pxl.Character))
                    renderString.Append(" ");
                else
                    renderString.Append(pxl.Character);
                //renderString.Append("?");

                lastX = x;
                lastY = y;
                lastFG = pxl.ForegroundColor;
                lastBG = pxl.BackgroundColor;
            }

            renderString.Append("\x1b[" + (inbounds.Y + 1) + ";" + (inbounds.X + 1) + "H");
            renderString.Append($"\u001b[38;{Pixel.DefaultForegroundColor.GetAnsiColorString()}");
            renderString.Append($"\u001b[48;{Pixel.DefaultBackgroundColor.GetAnsiColorString()}");
            if (MainCursor.CursorShown)
                renderString.Append("\x1b[?25h");
            else
                renderString.Append("\x1b[?25l");

            Console.Write(renderString);
        }

        public static void FillPixel(Pixel[,] screen, Field2D field, Pixel pxl)
        {
            field = new Field2D(new Size2D(screen)).MergeMinField(field);

            for (int y = field.TL.Y; y <= field.BR.Y; y++)
                for (int x = field.TL.X; x <= field.BR.X; x++)
                    screen[x, y] = pxl;
        }

        public static void FillOverPixel(Pixel[,] screen, Field2D field, Pixel pxl)
        {
            field = new Field2D(new Size2D(screen)).MergeMinField(field);

            for (int y = field.TL.Y; y <= field.BR.Y; y++)
                for (int x = field.TL.X; x <= field.BR.X; x++)
                    screen[x, y].WriteOver(pxl);
        }
    }
}
