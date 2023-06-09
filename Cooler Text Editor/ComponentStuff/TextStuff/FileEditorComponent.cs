﻿using Cooler_Text_Editor.ComponentStuff.PopUp.ExplorerStuff;
using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.SyntaxStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cooler_Text_Editor.SyntaxStuff.GrammarSyntaxHighlighter;

namespace Cooler_Text_Editor.ComponentStuff.TextStuff
{
    public class FileEditorComponent : BasicComponent
    {
        public ViewComponent View;
        public CenterTextComponent TitleBar;
        public TextComponent LineBox;
        public SyntaxHighlightingEditorComponent Editor;
        public PixelColor CurrForegroundColor, CurrBackgroundColor;
        public PixelColor DefForegroundColor, DefBackgroundColor;
        public PixelColor TitleForegroundColor, TitleBackgroundColor;
        public PixelColor LineForegroundColor, LineBackgroundColor;
        //public ExplorerComponent TempExplorerDialogue;
        public bool ShowLineNum = true;
        public bool UseSyntaxDefColors = true;

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

            CurrForegroundColor = PixelColor.White;
            CurrBackgroundColor = new PixelColor(10, 20, 30);
            DefForegroundColor = PixelColor.White;
            DefBackgroundColor = new PixelColor(10, 20, 30);
            TitleForegroundColor = PixelColor.Green;
            TitleBackgroundColor = PixelColor.Black;
            LineForegroundColor = PixelColor.Cyan;
            LineBackgroundColor = PixelColor.Black;

            Pixel bgPixel = new Pixel(CurrForegroundColor, CurrBackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;



            Size2D sizeView = new Size2D((parent) => { return parent; });
            View = new ViewComponent(sizeView);
            View.Position = new Position2D(0, 0);
            View.Parent = this;


            TitleBar = new CenterTextComponent();
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
            Program.SyntaxHighlightUpdate += YesUpdatePls;
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
                        LineBox.WriteLineText((i + 1).ToString().PadLeft(maxLineLen, ' '));
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
            Title = newTitle;

            TitleBar.Size = new Size2D(Size.Width, 1);
            TitleBar.Position = new Position2D();
            TitleBar.Text = newTitle;
        }



        public void LoadFile(string path)
        {
            if (path == null)
                return;

            if (!File.Exists(path))
                return;

            CurrentFilePath = path;

            if (CurrentFilePath != null)
                Editor.Extension = Path.GetExtension(CurrentFilePath);
            else
                Editor.Extension = ".cs";

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

            NeedToUpdateBcYes = true;
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


        public void DoDefColUpdate()
        {
            var highlighter = MultiSyntaxHighlighter.GrammarHighlighter;

            StyleSet set = highlighter.GetDefaultColorFromExtension(Editor.Extension);
            if (set == null)
            {
                CurrForegroundColor = DefForegroundColor;
                CurrBackgroundColor = DefBackgroundColor;
            }
            else
            {
                CurrForegroundColor = set.FG;
                CurrBackgroundColor = set.BG;
            }


            Editor.MainEditorComponent.ForegroundColor = CurrForegroundColor;
            Editor.MainEditorComponent.BackgroundColor = CurrBackgroundColor;

            Editor.ShadowEditorComponent.ForegroundColor = CurrForegroundColor;
            Editor.ShadowEditorComponent.BackgroundColor = CurrBackgroundColor;
        }

        public bool NeedToUpdateBcYes = false;
        public void YesUpdatePls()
        {
            NeedToUpdateBcYes = true;
        }

        protected override void InternalUpdate()
        {
            if (UseSyntaxDefColors)
                DoDefColUpdate();
            if (NeedToUpdateBcYes)
            {
                Editor.ForceUpdate();
                NeedToUpdateBcYes = false;
            }

            ComponentCursor.OverwriteCursor(Editor.ComponentCursor);
            ComponentCursor.CursorPosition += Editor.Position;

            Editor.MainEditorComponent.ForegroundColor = CurrForegroundColor;
            Editor.MainEditorComponent.BackgroundColor = CurrBackgroundColor;
            if (CurrentFilePath != null)
                Editor.Extension = Path.GetExtension(CurrentFilePath);
            else
                Editor.Extension = ".cs";

            LineBox.DefaultForegroundColor = LineForegroundColor;
            LineBox.DefaultBackgroundColor = LineBackgroundColor;
            LineBox.BackgroundColor = LineBackgroundColor;

            TitleBar.TextComp.DefaultForegroundColor = TitleForegroundColor;
            TitleBar.TextComp.DefaultBackgroundColor = TitleBackgroundColor;
            TitleBar.TextComp.BackgroundColor = TitleBackgroundColor;

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
