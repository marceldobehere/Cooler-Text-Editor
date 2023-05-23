// See https://aka.ms/new-console-template for more information
using Cooler_Text_Editor;
using Cooler_Text_Editor.ComponentStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System.Diagnostics;

public class Program
{
    public static bool Exit;
    public static double FPS;
    public static ScreenComponent MainScreen;

    public static void Main()
    {
        Console.BackgroundColor = PixelColor.PixelColorToConsoleColor[Pixel.DefaultBackgroundColor];
        Console.ForegroundColor = PixelColor.PixelColorToConsoleColor[Pixel.DefaultForegroundColor];
        Console.Clear();

        //Console.WriteLine("TEST");

        //Console.WriteLine("\x1b[36mTEST\x1b[0m");

        Exit = false;
        MainScreen = new ScreenComponent(new Size2D(Console.WindowWidth, Console.WindowHeight));
        Rendering.ScreenBackbuffer = new Pixel[MainScreen.Size.Width, MainScreen.Size.Height];
        

        ViewComponent viewComponent = MainScreen.MainView;
        {
            ViewComponent tViewComp = new ViewComponent(new Size2D(10, 20));
            tViewComp.Position = new Position2D(5, 5);
            tViewComp.BackgroundColor = new PixelColor(200, 60, 90);
            viewComponent.AddChild(tViewComp);
        }
        {
            ViewComponent tViewComp = new ViewComponent(new Size2D(20, 20));
            tViewComp.Position = new Position2D(25, 5);
            tViewComp.BackgroundColor = new PixelColor(200, 200, 100);
            viewComponent.AddChild(tViewComp);
        }

        Stopwatch fpsWatch = new Stopwatch();
        int frameCount = 60;
        FPS = 1;

        while (!Exit)
        {
            fpsWatch.Restart();
            for (int frame = 0; frame < frameCount && !Exit; frame++)
            {
                Input.HandleInputs(20);


                MainScreen.Update();
                if (Rendering.CheckWindowResize())
                {
                    MainScreen.UpdateFields.Clear();
                    MainScreen.UpdateFields.Add(new Field2D(MainScreen.Size));
                }
                MainScreen.RenderStuffToScreen();
            }
            FPS = frameCount / fpsWatch.Elapsed.TotalSeconds;
            Console.Title = $"Cooler Text Editor - {Math.Round(FPS, 2)} FPS";
        }


    }
}