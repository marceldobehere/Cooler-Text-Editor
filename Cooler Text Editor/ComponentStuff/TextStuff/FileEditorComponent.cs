using Cooler_Text_Editor.ComponentStuff.PopUp.ExplorerStuff;
using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.SyntaxStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff.TextStuff
{
    public class FileEditorComponent : BasicComponent
    {
        public ViewComponent View;
        public TextComponent TitleBar;
        public TextComponent LineBox;
        public SyntaxHighlightingEditorComponent Editor;
        public PixelColor ForegroundColor, BackgroundColor;
        public PixelColor TitleForegroundColor, TitleBackgroundColor;
        public PixelColor LineForegroundColor, LineBackgroundColor;
        //public ExplorerComponent TempExplorerDialogue;
        public bool ShowLineNum = true;

        public string CurrentFilePath = null;


        public FileEditorComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();
            CurrentFilePath = null;

            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);
            ComponentCursor.CursorShown = false;

            ForegroundColor = PixelColor.White;
            BackgroundColor = PixelColor.Black;
            TitleForegroundColor = PixelColor.Green;
            TitleBackgroundColor = PixelColor.Black;
            LineForegroundColor = PixelColor.Cyan;
            LineBackgroundColor = PixelColor.Black;

            Pixel bgPixel = new Pixel(ForegroundColor, BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;



            Size2D sizeView = new Size2D((parent) => { return parent; });
            View = new ViewComponent(sizeView);
            View.Position = new Position2D(0, 0);
            View.Parent = this;


            TitleBar = new TextComponent();
            TitleBar.Size = new Size2D(10, 1);
            TitleBar.Position = new Position2D(0, 0);
            View.AddChild(TitleBar);



            Size2D sizeEditor = new Size2D(Size.Width - 2, Size.Height - 2);//= new Size2D((parent) => { return new Size2D(parent.Width, parent.Height - 2); });
            Editor = new SyntaxHighlightingEditorComponent(sizeEditor);
            Editor.Position = new Position2D(2, 2);
            View.AddChild(Editor);


            // ignoring the line box for now
            Size2D sizeLineBox = new Size2D((parent) => { return new Size2D(2, parent.Height - 2); });
            LineBox = new TextComponent();
            LineBox.Size = new Size2D(2, Size.Height - 2);
            LineBox.Position = new Position2D(0, 2);
            View.AddChild(LineBox);


            //TempExplorerDialogue = null;

            LineBox.DefaultForegroundColor = LineForegroundColor;
            LineBox.DefaultBackgroundColor = LineBackgroundColor;
            LineBox.BackgroundColor = LineBackgroundColor;

            TitleScrollX = 0;
            LastMS = Environment.TickCount;
            DoTitleScroll = false;

            UpdateLineBox();
            UpdateTitle();
        }

        int lastStart = 0, lastEnd = 0;
        public void UpdateLineBox()
        {
            if (ShowLineNum)
            {
                int start = Editor.MainEditorComponent.InternalTextComponent.Scroll.Y;
                int end = start + Editor.Size.Height;
                int maxLineLen = end.ToString().Length;
                Size2D sizeLineBox = new Size2D(maxLineLen, Size.Height - 2);
                LineBox.Size = sizeLineBox;
                LineBox.Visible = true;
                Editor.Position.X = maxLineLen + 1;
                Editor.Size = new Size2D(Size.Width - maxLineLen - 1, Size.Height - 2);

                if (start != lastStart || end != lastEnd)
                {
                    lastStart = start;
                    lastEnd = end;

                    LineBox.Clear();
                    for (int i = start; i < end; i++)
                    {
                        LineBox.WriteLineText((i + 1).ToString().PadLeft(maxLineLen, '0'));
                    }
                }
            }
            else
            {
                LineBox.Visible = false;
                Editor.Position.X = 0;
                Editor.Size.Width = Size.Width;
            }
        }

        public int TitleScrollX;
        public long LastMS;
        public bool DoTitleScroll;
        public void UpdateTitle()
        {
            int lineCount = Editor.MainEditorComponent.InternalTextComponent.Text.Count;

            string newTitle;
            if (CurrentFilePath == null)
                newTitle = $"Untitled.txt";
            else
                newTitle = $"{CurrentFilePath}";

            string lineText;
            if (lineCount == 1)
                lineText = $" - ({lineCount} line)";
            else
                lineText = $" - ({lineCount} lines)";

            newTitle += lineText;


            TitleBar.Size = new Size2D(newTitle.Length, 1);
            TitleBar.Clear();
            TitleBar.WriteLineText(newTitle);

            if (newTitle.Length < View.Size.Width)
            {
                int titleLength = newTitle.Length;
                int w = View.Size.Width;
                int x = (w - titleLength) / 2;

                TitleBar.Position = new Position2D(x, 0);
                DoTitleScroll = false;
            }
            else
            {
                const int padding = 5;
                long tempTime = (long)(uint)Environment.TickCount;
                if (!DoTitleScroll)
                {
                    DoTitleScroll = true;
                    TitleScrollX = 0;
                    LastMS = tempTime;
                }

                if (tempTime > LastMS + 150)
                {
                    if (TitleScrollX == 0)
                    {
                        if (tempTime > LastMS + 1000)
                        {
                            TitleScrollX++;
                            LastMS = tempTime;
                        }
                    }
                    else
                    {
                        int maxW = newTitle.Length - View.Size.Width + 2 * padding;
                        if (TitleScrollX >= maxW)
                        {
                            if (tempTime > LastMS + 1000)
                            {
                                TitleScrollX = 0;
                                LastMS = tempTime;
                            }
                            else
                                TitleScrollX = maxW;
                        }
                        else
                        {
                            TitleScrollX++;
                            LastMS = tempTime;
                        }
                    }

                }

                TitleBar.Position = new Position2D(padding - TitleScrollX, 0);
            }
        }



        public void LoadFile(string path)
        {
            if (path == null)
                return;

            if (!File.Exists(path))
                return;

            CurrentFilePath = path;
            var txt = Editor.MainEditorComponent.InternalTextComponent;

            string[] lines = new string[1] { "" };
            try
            {
                lines = File.ReadAllLines(path);
            }
            catch (Exception e)
            {

            }

            txt.Clear();
            foreach (var line in lines)
                txt.WriteLineText(line);
            if (txt.Text.Count > 0)
                txt.Text.RemoveAt(txt.Text.Count - 1);

            Editor.MainEditorComponent.InternalCursor.CursorPosition = new Position2D();
            Editor.ForceUpdate();
        }

        public void SaveFile()
        {
            if (CurrentFilePath == null)
            {
                SaveFileDialog();
                return;
            }

            SaveFile(CurrentFilePath);
        }

        public void SaveFile(string path)
        {
            if (path == null)
                return;

            CurrentFilePath = path;
            var txt = Editor.MainEditorComponent.InternalTextComponent;

            StringBuilder outFileText = new StringBuilder();
            foreach (var line in txt.Text)
            {
                string t = "";
                foreach (var pxl in line)
                    t += pxl.Character;
                outFileText.AppendLine(t);
            }

            File.WriteAllText(path, outFileText.ToString());
        }

        public enum FileDialogueTypeEnum
        {
            Open,
            Save,
        }
        public FileDialogueTypeEnum FileDialogueType;
        public void HandleFileClickedYes(string path)
        {
            if (FileDialogueType == FileDialogueTypeEnum.Open)
                LoadFile(path);
            else if (FileDialogueType == FileDialogueTypeEnum.Save)
                SaveFile(path);
            PopUpComponent = null;
        }

        public void OpenFileDialog()
        {
            ExplorerComponent TempExplorerDialogue = new ExplorerComponent(new Size2D((Size2D parent) => { return new Size2D(parent.Width - 6, parent.Height - 4); }));
            TempExplorerDialogue.DefaultBorderColor = PixelColor.Yellow2;
            TempExplorerDialogue.OnFileClicked = HandleFileClickedYes;
            TempExplorerDialogue.Parent = Parent;
            if (CurrentFilePath != null)
            {
                TempExplorerDialogue.SetPath(Path.GetDirectoryName(CurrentFilePath));
                TempExplorerDialogue.RefreshFullList();
            }
            PopUpComponent = TempExplorerDialogue;
            FileDialogueType = FileDialogueTypeEnum.Open;
        }

        public void SaveFileDialog()
        {
            ExplorerComponent TempExplorerDialogue = new ExplorerComponent(new Size2D((Size2D parent) => { return new Size2D(parent.Width - 6, parent.Height - 4); }));
            TempExplorerDialogue.DefaultBorderColor = PixelColor.Yellow2;
            TempExplorerDialogue.OnFileClicked = HandleFileClickedYes;
            TempExplorerDialogue.Parent = Parent;
            if (CurrentFilePath != null)
            {
                TempExplorerDialogue.SetPath(Path.GetDirectoryName(CurrentFilePath));
                TempExplorerDialogue.RefreshFullList();
            }
            PopUpComponent = TempExplorerDialogue;
            FileDialogueType = FileDialogueTypeEnum.Save;
        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            if (info.Modifiers == ConsoleModifiers.Alt && info.Key == ConsoleKey.O)
            {
                OpenFileDialog();
                return true;
            }
            if (info.Modifiers == ConsoleModifiers.Alt && info.Key == ConsoleKey.S)
            {
                SaveFile();
                return true;
            }
            if (info.Modifiers == ConsoleModifiers.Alt && info.Key == ConsoleKey.L)
            {
                ShowLineNum = !ShowLineNum;
                return true;
            }
            if (info.Modifiers == (ConsoleModifiers.Alt | ConsoleModifiers.Shift) && info.Key == ConsoleKey.S)
            {
                SaveFileDialog();
                return true;
            }



            Editor.HandleKey(info);

            return true;

        }


        protected override void InternalUpdate()
        {
            ComponentCursor.OverwriteCursor(Editor.ComponentCursor);
            ComponentCursor.CursorPosition += Editor.Position;

            Editor.MainEditorComponent.ForegroundColor = ForegroundColor;
            Editor.MainEditorComponent.BackgroundColor = BackgroundColor;

            LineBox.DefaultForegroundColor = LineForegroundColor;
            LineBox.DefaultBackgroundColor = LineBackgroundColor;
            LineBox.BackgroundColor = LineBackgroundColor;

            TitleBar.DefaultForegroundColor = TitleForegroundColor;
            TitleBar.DefaultBackgroundColor = TitleBackgroundColor;
            TitleBar.BackgroundColor = TitleBackgroundColor;

            UpdateLineBox();
            UpdateTitle();





            View.BackgroundColor = TitleBackgroundColor;
            View.Update();
            RenderedScreen = View.RenderedScreen;


            if (Parent != null && UpdateFields.Count > 0)
                foreach (Field2D tempField in UpdateFields)
                    Parent.UpdateFields.Add(tempField + Position);

            UpdateFields.Clear();
        }
    }
}
