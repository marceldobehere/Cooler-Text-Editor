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
            for (int i = 0; i < amt; i++)
                HandleInput();
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



            Console.Write(consoleKeyInfo.KeyChar);

            return true;
        }
    }
}
