﻿// See https://aka.ms/new-console-template for more information
using Cooler_Text_Editor;
using Cooler_Text_Editor.ComponentStuff;
using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System.Diagnostics;
using System.Drawing;

public class Program
{
    public static bool Exit;
    public static double FPS;
    public static ScreenComponent MainScreen;

    public static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();

        //Console.WriteLine("TEST");

        //Console.WriteLine("\x1b[36mTEST\x1b[0m");

        Exit = false;
        MainScreen = new ScreenComponent(new Size2D(Console.WindowWidth, Console.WindowHeight));
        Rendering.ScreenBackbuffer = new Pixel[MainScreen.Size.Width, MainScreen.Size.Height];
        for (int y = 0; y < MainScreen.Size.Height; y++)
            for (int x = 0; x < MainScreen.Size.Width; x++)
                Rendering.ScreenBackbuffer[x, y] = Pixel.Empty;

        Cursor.MainCursor = MainScreen.MainView.ComponentCursor;

        Rendering.InitCursor();

        ViewComponent viewComponent = MainScreen.MainView;
        ViewComponent tViewComp1, tViewComp2, tViewComp3;
        {
            ViewComponent tViewComp = new ViewComponent(new Size2D(60, 20));
            tViewComp1 = tViewComp;
            tViewComp.Position = new Position2D(5, 5);
            tViewComp.BackgroundColor = new PixelColor(200, 60, 90);
            viewComponent.AddChild(tViewComp);

            {
                TextComponent txtComp = new TextComponent();
                tViewComp.AddChild(txtComp);
                
                txtComp.Position = new Position2D(5, 5);
                txtComp.Size = new Size2D((Size2D parent) => { return parent - txtComp.Position; });

                txtComp.Text.Clear();
                txtComp.WriteLineText("Hello, World!");
                txtComp.WriteLineText();
                txtComp.WriteLineText("How are you?");

                //txtComp.Visible = false;
            }
        }
        {
            ViewComponent tViewComp = new ViewComponent(new Size2D(30, 30));
            tViewComp2 = tViewComp;
            tViewComp.Position = new Position2D(70, 5);
            tViewComp.BackgroundColor = new PixelColor(90, 100, 40);
            viewComponent.AddChild(tViewComp);

            {
                TextComponent txtComp = new TextComponent();
                tViewComp.AddChild(txtComp);
                txtComp.Size = new Size2D((Size2D parent) => { return new Size2D(parent.Width, parent.Height / 2); });
                txtComp.Position = new Position2D(4, 2);
                txtComp.Text.Clear();
                for (int br = 0; br < 4; br++)
                {
                    List<Pixel> tLine = new List<Pixel>();
                    txtComp.Text.Add(tLine);
                    string tStr = "HOI :)  :)";
                    for (int i = 0; i < tStr.Length; i++)
                    {
                        tLine.Add(new Pixel(tStr[i], new PixelColor(0, i * 50, 200), new PixelColor()));
                    }
                }
                //txtComp.Visible = false;
            }

            {
                TextComponent txtComp = new TextComponent();
                tViewComp.AddChild(txtComp);
                txtComp.Size = new Size2D((Size2D parent) => { return new Size2D(parent.Width, parent.Height / 2); });
                txtComp.Position = new Position2D(4, 17);
                txtComp.Text.Clear();
                for (int br = 0; br < 4; br++)
                {
                    List<Pixel> tLine = new List<Pixel>();
                    txtComp.Text.Add(tLine);
                    string tStr = "YAY :O  :)";
                    for (int i = 0; i < tStr.Length; i++)
                    {
                        tLine.Add(new Pixel(tStr[i], new PixelColor(0, i * 50, 200), new PixelColor()));
                    }
                }
                //txtComp.Visible = false;
            }
        }

        {
            ImageComponent imgComp = new ImageComponent(new Bitmap("../../testImages/rocc.png"));
            viewComponent.AddChild(imgComp);
            imgComp.Position = new Position2D(10, 3);
            imgComp.Size = new Size2D(60, 30);
            imgComp.UpdateScreen();

        }

        {
            TerminalComponent terminalComponent = new TerminalComponent(new Size2D((Size2D parent) => { return new Size2D(parent.Width, parent.Height); }));
            terminalComponent.Size = new Size2D(90, 20);
            terminalComponent.Position = new Position2D(15, 35);
            viewComponent.AddChild(terminalComponent);
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

                //tViewComp2.Position.X = frame % 30 + 10;
                //tViewComp2.Position.Y = frame / 9;

                //tViewComp1.Position.X = frame / 3 - 10;
                //tViewComp1.Position.Y = frame / 7;

                ////tViewComp2.Visible = false;
                //tViewComp2.Position.X = 0;
                //tViewComp2.Position.Y = 0;

                ////tViewComp1.Visible = false;
                //tViewComp1.Position.X = 1;
                //tViewComp1.Position.Y = 1;


                //CursorThing.MainCursor.CursorPosition.X = frame % 20;
                //CursorThing.MainCursor.CursorPosition.Y = frame / 6;
                //tViewComp1.Visible = frame % 2 == 0;

                //Cursor.MainCursor.CursorPosition.X = 1;
                //Cursor.MainCursor.CursorPosition.Y = 2;

                MainScreen.Update();
                if (Rendering.CheckWindowResize())
                {
                    MainScreen.UpdateFields.Clear();
                    MainScreen.UpdateFields.Add(new Field2D(MainScreen.Size));
                }
                MainScreen.RenderStuffToScreen();

                Task.Delay(10).Wait(); // Limit yes
            }
            FPS = frameCount / fpsWatch.Elapsed.TotalSeconds;
            Console.Title = $"Cooler Text Editor - {Math.Round(FPS, 2)} FPS";
        }


    }
}