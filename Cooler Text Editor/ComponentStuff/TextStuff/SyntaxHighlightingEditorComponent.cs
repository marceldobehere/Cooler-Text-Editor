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
    public class SyntaxHighlightingEditorComponent : BasicComponent
    {
        public EditorComponent MainEditorComponent, ShadowEditorComponent;
        public List<ConsoleKeyInfo> KeyLog;
        public BasicSyntaxHighlighter syntaxHighlighter;
        Task<List<List<Pixel>>> syntaxTask;
        public bool CancelSyntaxUpdate;
        public bool TextChanged;

        public void ForceUpdate()
        {
            TextChanged = true;
            CancelSyntaxUpdate = false;
            Update();
        }

        public void LoadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                MainEditorComponent.InternalTextComponent.WriteText(reader.ReadToEnd());
            }
            ForceUpdate();
        }



        public SyntaxHighlightingEditorComponent(Size2D cSize)
        {
            Size = cSize;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);
            ComponentCursor.CursorShown = false;

            Pixel bgPixel = new Pixel(PixelColor.White, PixelColor.Black);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;


            Size2D size = new Size2D((parent) => { return parent; });

            MainEditorComponent = new EditorComponent(size);
            MainEditorComponent.Position = new Position2D(0, 0);
            MainEditorComponent.BackgroundColor = new PixelColor(10, 20, 30);
            MainEditorComponent.InternalTextComponent.Clear();
            MainEditorComponent.InternalTextComponent.WriteLineText();
            MainEditorComponent.Parent = this;

            ShadowEditorComponent = new EditorComponent(size);
            ShadowEditorComponent.Position = new Position2D(0, 0);
            ShadowEditorComponent.BackgroundColor = new PixelColor(10, 20, 30);
            ShadowEditorComponent.InternalTextComponent.Clear();
            ShadowEditorComponent.InternalTextComponent.WriteLineText();
            ShadowEditorComponent.Parent = this;

            syntaxHighlighter = new TestSyntaxHighlighter();
            syntaxTask = null;
            CancelSyntaxUpdate = true;
            TextChanged = false;

            KeyLog = new List<ConsoleKeyInfo>();
        }


        public override void HandleKey(ConsoleKeyInfo info)
        {
            MainEditorComponent.HandleKey(info);
            if (!(
                info.Key == ConsoleKey.DownArrow ||
                info.Key == ConsoleKey.UpArrow ||
                info.Key == ConsoleKey.LeftArrow ||
                info.Key == ConsoleKey.RightArrow))
                TextChanged = true;

            if (syntaxTask != null)
                KeyLog.Add(info);
            else if (KeyLog.Count > 0)
                KeyLog.Clear();
        }

        protected override void InternalUpdate()
        {
            ComponentCursor.OverwriteCursor(MainEditorComponent.ComponentCursor);
            //ShadowEditorComponent.Update();
            MainEditorComponent.Update();

            if (syntaxTask == null || syntaxTask.IsCompleted)
            {
                if (syntaxTask != null)
                {
                    if (!CancelSyntaxUpdate)
                    {
                        var res = syntaxTask.Result;
                        ShadowEditorComponent.InternalTextComponent.Text = res;


                        //ShadowEditorComponent.Update();
                        foreach (var key in KeyLog)
                            ShadowEditorComponent.HandleKey(key);
                        //ShadowEditorComponent.Update();

                        UpdateFields.Clear();
                        (MainEditorComponent, ShadowEditorComponent) = (ShadowEditorComponent, MainEditorComponent);

                        MainEditorComponent.Update();
                        ComponentCursor.OverwriteCursor(MainEditorComponent.ComponentCursor);
                    }
                    else
                        CancelSyntaxUpdate = false;

                    syntaxTask = null;
                }
                else
                {
                    if (TextChanged)//((UpdateFields.Count > 0)
                    {
                        TextChanged = false;

                        MainEditorComponent.Update();
                        KeyLog.Clear();
                        List<string> tList = new List<string>();
                        foreach (var tLine in MainEditorComponent.InternalTextComponent.Text)
                        {
                            string tStr = "";
                            foreach (var pxl in tLine)
                                tStr += pxl.Character;
                            tList.Add(tStr);
                        }
                        ShadowEditorComponent.ComponentCursor.OverwriteCursor(MainEditorComponent.ComponentCursor);
                        ShadowEditorComponent.InternalCursor.OverwriteCursor(MainEditorComponent.InternalCursor);
                        ShadowEditorComponent.InternalTextComponent.ComponentCursor.OverwriteCursor(MainEditorComponent.InternalTextComponent.ComponentCursor);
                        ShadowEditorComponent.InternalTextComponent.Scroll = MainEditorComponent.InternalTextComponent.Scroll;
                        MainEditorComponent.InternalTextComponent.RenderTo(ShadowEditorComponent.InternalTextComponent.RenderedScreen, GetLocalField());

                        syntaxTask = syntaxHighlighter.SyntaxHighlight(tList, MainEditorComponent.ForegroundColor, MainEditorComponent.BackgroundColor);
                    }
                }
            }
            else
            {

            }




            RenderedScreen = MainEditorComponent.RenderedScreen;

            if (Parent != null && UpdateFields.Count > 0)
                foreach (Field2D tempField in UpdateFields)
                    Parent.UpdateFields.Add(tempField + Position);

            UpdateFields.Clear();
        }
    }
}
