using Cooler_Text_Editor.HelperStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public class Input
    {
        public static void HandleInputs(int amt)
        {
            for (int i = 0; i < amt && HandleInput(); i++)
                ;
        }
        public static bool HandleInput()
        {
            if (!Console.KeyAvailable)
                return false;

            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
            if (consoleKeyInfo.Modifiers == ConsoleModifiers.Alt && consoleKeyInfo.Key == ConsoleKey.X)
            {
                Program.Exit = true;
                return true;
            }

            if (consoleKeyInfo.Modifiers == ConsoleModifiers.Alt)
            {
                if (Cursor.MainCursor != null &&
                    Cursor.MainCursor.CursorComponent != null)
                {
                    if (consoleKeyInfo.Key == ConsoleKey.LeftArrow)
                    {

                    }
                    else if (consoleKeyInfo.Key == ConsoleKey.RightArrow)
                    {

                    }
                    else if (consoleKeyInfo.Key == ConsoleKey.UpArrow)
                    {
                        if (Cursor.MainCursor.CursorComponent.Parent != null &&
                            Cursor.MainCursor.CursorComponent.Parent.ComponentCursor != null)
                            Cursor.MainCursor = Cursor.MainCursor.CursorComponent.Parent.ComponentCursor;
                    }
                    else if (consoleKeyInfo.Key == ConsoleKey.DownArrow)
                    {

                    }
                }

                return true;
            }

            if (Cursor.MainCursor == null ||
                Cursor.MainCursor.CursorComponent == null)
                return true;


            Cursor.MainCursor.CursorComponent.HandleKey(consoleKeyInfo);


            //Console.Write(consoleKeyInfo.KeyChar);

            return true;
        }
    }
}
