using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff.TextStuff
{
    public class TextComponent : BasicComponent
    {
        public List<List<Pixel>> Text;
        public bool updateInternal;
        public Size2D OldSize;
        //public Position2D OldPosition;
        public PixelColor BackgroundColor;

        public Position2D Scroll;
        public Position2D OldScroll;

        public PixelColor DefaultForegroundColor, DefaultBackgroundColor;

        public bool AutoYScroll;

        public TextComponent()
        {
            Text = new List<List<Pixel>>();
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();
            Size = new Size2D();
            Scroll = new Position2D();
            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);
            ComponentCursor.CursorShown = false;

            AutoYScroll = false;

            OldSize = Size;
            //OldPosition = Position;
            OldScroll = Scroll;

            RenderedScreen = new Pixel[0, 0];
            updateInternal = false;

            BackgroundColor = new PixelColor();
            DefaultForegroundColor = new PixelColor(255, 255, 255);
            DefaultBackgroundColor = new PixelColor();
        }

        protected override void InternalUpdate()
        {
            while (AutoYScroll)
            {
                if (Text.Count - Scroll.Y > Size.Height)
                    Scroll.Y++;
                else if (Scroll.Y > 0 && Text.Count - Scroll.Y < Size.Height)
                    Scroll.Y--;
                else
                    break;
            }


            if (OldSize != Size || //OldPosition != Position ||
                OldScroll != Scroll)
            {
                OldScroll = Scroll;
                OldSize = Size;
                //OldPosition = Position;
                updateInternal = true;
            }

            if (updateInternal)
            {
                RenderedScreen = new Pixel[Size.Width, Size.Height];
            }



            Pixel tempBG = new Pixel(BackgroundColor, BackgroundColor);
            List<Field2D> tUpdates = new List<Field2D>();
            for (int y = Scroll.Y; y < Scroll.Y + Size.Height; y++)
            {
                for (int x = Scroll.X; x < Scroll.X + Size.Width; x++)
                {
                    Pixel t = tempBG;
                    if (y < Text.Count && y >= 0 &&
                        x < Text[y].Count && x >= 0)
                        t = Text[y][x];

                    int x2 = x - Scroll.X;
                    int y2 = y - Scroll.Y;

                    if (RenderedScreen[x2, y2] != t)
                    {
                        RenderedScreen[x2, y2] = t;

                        Field2D tUpdate = new Field2D(Position + new Position2D(0, y2), new Size2D(Size.Width, 1));
                        if (!tUpdates.Contains(tUpdate))
                            tUpdates.Add(tUpdate);
                    }
                }
            }

            if (Parent != null)
            {
                if (updateInternal)
                {
                    Parent.UpdateFields.Add(GetField());
                }
                else
                {
                    foreach (Field2D tUpdate in tUpdates)
                        Parent.UpdateFields.Add(tUpdate);
                }
            }

            updateInternal = false;
        }

        public void AddNewLineIfNotExist()
        {
            if (Text.Count == 0)
                Text.Add(new List<Pixel>());
        }

        public void WriteText(string txt)
        {
            WriteText(txt, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public void WriteText(string txt, PixelColor fg, PixelColor bg)
        {
            AddNewLineIfNotExist();
            List<Pixel> currentLine = Text.Last();
            for (int i = 0; i < txt.Length; i++)
            {
                char chr = txt[i];
                if (chr == '\n')
                {
                    currentLine = new List<Pixel>();
                    Text.Add(currentLine);
                    continue;
                }
                if (chr == '\b')
                {
                    if (currentLine.Count > 0)
                        currentLine.RemoveAt(currentLine.Count - 1);
                    else
                    {
                        if (Text.Count > 1)
                        {
                            Text.RemoveAt(Text.Count - 1);
                            currentLine = Text.Last();
                        }
                    }
                    continue;
                }
                if (chr == 0x0C)
                {
                    Clear();
                    return;
                }
                if (chr == '\u001b')// && i + 1 < txt.Length)
                {
                    //string after = txt.Substring(i);
                    // \u001b[2J


                    continue;
                }
                if (chr == '\t')
                {
                    for (int j = 0; j < 4; j++)
                        currentLine.Add(new Pixel(' ', fg, bg));
                    continue;
                }
                if ("\r".Contains(chr))
                {
                    continue;
                }


                currentLine.Add(new Pixel(chr, fg, bg));
            }
            updateInternal = true;
        }

        public void WriteLineText(string txt)
        {
            WriteLineText(txt, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public void WriteLineText(string txt, PixelColor fg, PixelColor bg)
        {
            AddNewLineIfNotExist();
            WriteText(txt, fg, bg);
            WriteLineText();
        }

        public void WriteLineText()
        {
            Text.Add(new List<Pixel>());
            updateInternal = true;
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            //throw new NotImplementedException();
        }
        public void Clear()
        {
            Text.Clear();
            updateInternal = true;
            if (AutoYScroll)
                Scroll.Y = 0;
        }
    }
}
