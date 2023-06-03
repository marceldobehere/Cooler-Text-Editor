using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.SyntaxStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class FileEditorComponent : BasicComponent
    {
        public ViewComponent View;
        public TextComponent TitleBar;
        public TextComponent LineBox;
        public SyntaxHighlightingEditorComponent Editor;
        public PixelColor ForegroundColor, BackgroundColor;
        public PixelColor TitleForegroundColor, TitleBackgroundColor;

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

            Pixel bgPixel = new Pixel(ForegroundColor, BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;



            Size2D sizeView = new Size2D((Size2D parent) => { return parent; });
            View = new ViewComponent(sizeView);
            View.Position = new Position2D(0, 0);
            View.Parent = this;


            Size2D sizeTitle = new Size2D((Size2D parent) => { return new Size2D(parent.Width, 1); });
            TitleBar = new TextComponent();
            TitleBar.Size = sizeTitle;
            TitleBar.Position = new Position2D(0, 0);
            View.AddChild(TitleBar);



            Size2D sizeEditor = new Size2D((Size2D parent) => { return new Size2D(parent.Width, parent.Height - 2); });
            Editor = new SyntaxHighlightingEditorComponent(sizeEditor);
            Editor.Position = new Position2D(0, 2);
            View.AddChild(Editor);


            // ignoring the line box for now





            UpdateTitle();
        }
        
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


            TitleBar.Clear();
            TitleBar.WriteLineText(newTitle);

            {
                int titleLength = newTitle.Length;
                int w = View.Size.Width;
                int x = (w - titleLength) / 2;

                TitleBar.Position = new Position2D(x, 0);
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

            string[] lines = File.ReadAllLines(path);

            txt.Clear();
            foreach (var line in lines)
                txt.WriteLineText(line);
        }

        public void SaveFile()
        {
            if (CurrentFilePath == null)
                CurrentFilePath = SaveFileDialog();
            
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

        public string OpenFileDialog()
        {
            return null;
        }

        public string SaveFileDialog()
        {
            return null;
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Modifiers == ConsoleModifiers.Alt && info.Key == ConsoleKey.O)
            {
                string path = OpenFileDialog();
                LoadFile(path);
                return;
            }
            if (info.Modifiers == ConsoleModifiers.Alt && info.Key == ConsoleKey.S)
            {
                SaveFile();
                return;
            }
            if (info.Modifiers == (ConsoleModifiers.Alt | ConsoleModifiers.Shift) && info.Key == ConsoleKey.S)
            {
                string path = OpenFileDialog();
                SaveFile(path);
                return;
            }


            Editor.HandleKey(info);
        }
        
        protected override void InternalUpdate()
        {
            ComponentCursor.OverwriteCursor(Editor.ComponentCursor);
            ComponentCursor.CursorPosition += Editor.Position;

            Editor.MainEditorComponent.ForegroundColor = ForegroundColor;
            Editor.MainEditorComponent.BackgroundColor = BackgroundColor;
         

            TitleBar.DefaultForegroundColor = TitleForegroundColor;
            TitleBar.DefaultBackgroundColor = TitleBackgroundColor;
            TitleBar.BackgroundColor = TitleBackgroundColor;
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
