// See https://aka.ms/new-console-template for more information
using Cooler_Text_Editor;
using Cooler_Text_Editor.ComponentStuff;
using Cooler_Text_Editor.ComponentStuff.TextStuff;
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
    public static int UpdateCount;

    public static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs args)
    {
        //args.SpecialKey == ConsoleSpecialKey.ControlC;
        Console.Title = $"CTRL + C";
        args.Cancel = true;
    }

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.CancelKeyPress += HandleCancelKeyPress;
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
            tViewComp1.Visible = false;

            //{
            //    TextComponent txtComp = new TextComponent();
            //    tViewComp.AddChild(txtComp);

            //    txtComp.Position = new Position2D(5, 5);
            //    txtComp.Size = new Size2D((Size2D parent) => { return parent - txtComp.Position; });

            //    txtComp.Text.Clear();
            //    txtComp.WriteLineText("Hello, World!");
            //    txtComp.WriteLineText();
            //    txtComp.WriteLineText("How are you?");

            //    //txtComp.Visible = false;
            //}
        }


        {
            FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
            viewComponent.AddChild(fileComp);
            //fileComp.Size = new Size2D((Size2D parent) => { return new Size2D(parent.Width - 40, parent.Height - 20); });

            fileComp.Position = new Position2D(5, 5);
            fileComp.BackgroundColor = new PixelColor(10, 20, 30);

            Cursor.MainCursor = fileComp.ComponentCursor;
            MainScreen.MainView.ComponentCursor.HoverComponent = fileComp;
        }

        //{
        //    FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
        //    viewComponent.AddChild(fileComp);
        //    //fileComp.Size = new Size2D((Size2D parent) => { return new Size2D(parent.Width / 3, parent.Height / 3); });

        //    fileComp.Position = new Position2D(90, 5);
        //    fileComp.BackgroundColor = new PixelColor(10, 20, 30);
        //}

        {
            TabComponent tabComp = new TabComponent(new Size2D(80, 30));
            viewComponent.AddChild(tabComp);
            tabComp.Position = new Position2D(90, 5);

            for (int i = 0; i < 2; i++)
            {
                FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
                tabComp.AddTab(fileComp);
            }

            //{
            //    ImageComponent imgComp = new ImageComponent(Image.Load<Rgba32>("../../testImages/wat.gif"));
            //    //imgComp.UpdateScreen();
            //    tabComp.AddTab(imgComp);
            //}

            //{
            //    FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
            //    fileComp.BackgroundColor = new PixelColor(10, 20, 30);
            //    tabComp.AddTab(fileComp);
            //}

            //{
            //    TabComponent tabComp2 = new TabComponent(new Size2D(80, 30));

            //    {
            //        FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
            //        fileComp.BackgroundColor = new PixelColor(10, 20, 30);
            //        tabComp2.AddTab(fileComp);
            //    }

            //    {
            //        FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
            //        fileComp.BackgroundColor = new PixelColor(10, 20, 30);
            //        tabComp2.AddTab(fileComp);
            //    }
            //    tabComp.AddTab(tabComp2);
            //}
        }



        {
            VerticalSplitterComponent vSplitter = new VerticalSplitterComponent(new Size2D(120, 60));
            viewComponent.AddChild(vSplitter);
            vSplitter.Position = new Position2D(1, 1);

            for (int i2 = 0; i2 < 3; i2++)
            {
                {
                    HorizontalSplitterComponent hSplitter = new HorizontalSplitterComponent(new Size2D(160, 30));
                    vSplitter.AddChild(hSplitter);

                    for (int i = 0; i < 2; i++)
                    {
                        FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
                        hSplitter.AddChild(fileComp);

                        //TabComponent tabComp = new TabComponent(new Size2D(80, 30));
                        //hSplitter.AddChild(tabComp);
                    }
                }
            }

            //{
            //    {
            //        string str = "../../testImages/rocc.png";
            //        if (!File.Exists(str))
            //            str = "./test/testImages/rocc.png";

            //        ImageComponent imgComp = new ImageComponent(Image.Load<Rgba32>(str));
            //        //viewComponent.AddChild(imgComp);
            //        (vSplitter.Children[0] as ViewComponent).Children[1] = imgComp;
            //        imgComp.Position = new Position2D(120, 50);
            //        imgComp.Size = new Size2D(60, 30);// new Size2D((Size2D parent) => { return new Size2D(parent.Width / 4, parent.Height / 4); });
            //        imgComp.UpdateScreen();
            //    }
            //}

        }

        {
            ViewComponent tViewComp = new ViewComponent(new Size2D(30, 30));
            tViewComp2 = tViewComp;
            tViewComp.Position = new Position2D(126, 40);
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
            //txtComp.InternalTextComponent.WriteLineText("Hello, World!");
            //txtComp.WriteLineText();
            //txtComp.WriteLineText("How are you?");
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
                        tLine.Add(new Pixel(tStr[i], new PixelColor(0, 200, i * 30), new PixelColor()));
                    }
                }
                //txtComp.Visible = false;
            }
        }

        {
            string str = "../../testImages/rocc.png";
            if (!File.Exists(str))
                str = "./test/testImages/rocc.png";

            ImageComponent imgComp = new ImageComponent(Image.Load<Rgba32>(str));
            viewComponent.AddChild(imgComp);
            imgComp.Position = new Position2D(120, 50);
            imgComp.Size = new Size2D(60, 30);// new Size2D((Size2D parent) => { return new Size2D(parent.Width / 4, parent.Height / 4); });
            imgComp.UpdateScreen();
        }

        //{
        //    ImageComponent imgComp = new ImageComponent(Image.Load<Rgba32>("../../testImages/wat.gif"));
        //    viewComponent.AddChild(imgComp);
        //    imgComp.Position = new Position2D(0, 0);
        //    imgComp.Size = new Size2D((Size2D parent) => { return new Size2D(parent.Width / 2, parent.Height / 2); });
        //    imgComp.UpdateScreen();
        //}

        //{
        //    TerminalComponent terminalComponent = new TerminalComponent(new Size2D((Size2D parent) => { return new Size2D(parent.Width, parent.Height); }));
        //    terminalComponent.Size = new Size2D(80, 20);
        //    terminalComponent.Position = new Position2D(15, 35);
        //    viewComponent.AddChild(terminalComponent);
        //}





        if (true)
        {
            viewComponent.Children.Clear();
            viewComponent.OldFields.Clear();

            {
                VerticalSplitterComponent vSplitter = new VerticalSplitterComponent(Size2D.UseParentMinusBorder);
                vSplitter.Position = new Position2D(1, 1);
                viewComponent.AddChild(vSplitter);
                vSplitter.Position = new Position2D(1, 1);

                for (int i2 = 0; i2 < 2; i2++)
                {
                    {
                        HorizontalSplitterComponent hSplitter = new HorizontalSplitterComponent(new Size2D(160, 30));
                        vSplitter.AddChild(hSplitter);

                        for (int i = 0; i < 3; i++)
                        {
                            FileEditorComponent fileComp = new FileEditorComponent(new Size2D(80, 30));
                            hSplitter.AddChild(fileComp);

                            //TabComponent tabComp = new TabComponent(new Size2D(80, 30));
                            //hSplitter.AddChild(tabComp);
                        }
                    }
                }

                Cursor.MainCursor = (vSplitter.Children[0] as ViewComponent).Children[1].ComponentCursor;
            }
        }






        Stopwatch fpsWatch = new Stopwatch();
        int frameCount = 60;
        FPS = 1;

        while (!Exit)
        {
            fpsWatch.Restart();
            for (int frame = 0; frame < frameCount && !Exit; frame++)
            {
                UpdateCount = 0;
                Input.HandleInputs(100);

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

                Task.Delay(5).Wait(); // Limit yes
            }
            FPS = frameCount / fpsWatch.Elapsed.TotalSeconds;
            MainScreen.MainView.Title = $"Cooler Text Editor - {Math.Round(FPS, 2)} FPS ({UpdateCount} Updates)";
            MainScreen.Title = MainScreen.MainView.Title;
            if (Cursor.MainCursor != null &&
                Cursor.MainCursor.CursorComponent != null)
                Console.Title = Cursor.MainCursor.CursorComponent.Title;
        }


        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
    }
}