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
            if (consoleKeyInfo.Modifiers == ConsoleModifiers.Alt &&
                consoleKeyInfo.Key == ConsoleKey.X)
            {
                Program.Exit = true;
                return true;
            }

            if (consoleKeyInfo.Modifiers == ConsoleModifiers.Control &&
                consoleKeyInfo.Key == ConsoleKey.Backspace)
            {
                if (Cursor.MainCursor.CursorComponent.Parent != null &&
                    Cursor.MainCursor.CursorComponent.Parent.ComponentCursor != null)
                {
                    Cursor.MainCursor.CursorComponent.HandleExitFocus();
                    Cursor.MainCursor = Cursor.MainCursor.CursorComponent.Parent.ComponentCursor;
                    Cursor.MainCursor.CursorComponent.HandleEnterFocus();
                    return true;
                }
            }

            if (consoleKeyInfo.Modifiers == ConsoleModifiers.Control &&
                consoleKeyInfo.Key == ConsoleKey.Enter)
            {
                if (Cursor.MainCursor.HoverComponent != null &&
                    Cursor.MainCursor.HoverComponent.ComponentCursor != null)
                {
                    Cursor.MainCursor.CursorComponent.HandleExitFocus();
                    Cursor.MainCursor = Cursor.MainCursor.HoverComponent.ComponentCursor;
                    Cursor.MainCursor.CursorComponent.HandleEnterFocus();
                    return true;
                }
            }

            if (Cursor.MainCursor == null ||
                Cursor.MainCursor.CursorComponent == null)
                return true;


            Cursor.MainCursor.CursorComponent.HandleKey(consoleKeyInfo);

            if (consoleKeyInfo.Key == ConsoleKey.LeftArrow ||
                consoleKeyInfo.Key == ConsoleKey.RightArrow ||
                consoleKeyInfo.Key == ConsoleKey.UpArrow ||
                consoleKeyInfo.Key == ConsoleKey.DownArrow)
                return false;


            //Console.Write(consoleKeyInfo.KeyChar);

            return true;
        }
    }
}
