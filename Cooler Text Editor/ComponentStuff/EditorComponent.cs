using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class EditorComponent : BasicComponent
    {
        public PixelColor ForegroundColor, BackgroundColor;
        public TextComponent InternalTextComponent;
        public Cursor InternalCursor;

        public EditorComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            UpdateFields = new List<Field2D>();
            ComponentCursor = new Cursor(this);

            ForegroundColor = PixelColor.White;
            BackgroundColor = PixelColor.Transparent;

            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;


            InternalTextComponent = new TextComponent();
            InternalTextComponent.Size = new Size2D(((Size2D parent) => { return parent; }));
            InternalTextComponent.Position = new Position2D();
            InternalTextComponent.Parent = this;
            InternalTextComponent.BackgroundColor = BackgroundColor;
            InternalTextComponent.DefaultBackgroundColor = BackgroundColor;
            InternalTextComponent.DefaultForegroundColor = ForegroundColor;
            InternalTextComponent.updateInternal = true;
            InternalTextComponent.AutoYScroll = false;

            InternalCursor = InternalTextComponent.ComponentCursor;
        }


        public void CheckCursorBounds()
        {
            if (InternalCursor.CursorPosition.Y > InternalTextComponent.Text.Count - 1)
            {
                InternalCursor.CursorPosition.Y = InternalTextComponent.Text.Count - 1;

                if (InternalCursor.CursorPosition.Y >= 0 && InternalCursor.CursorPosition.Y < InternalTextComponent.Text.Count)
                    InternalCursor.CursorPosition.X = InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count;
            }

            if (InternalCursor.CursorPosition.Y >= 0 && InternalCursor.CursorPosition.Y < InternalTextComponent.Text.Count)
            {
                if (InternalCursor.CursorPosition.X > InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count)
                    InternalCursor.CursorPosition.X = InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count;
            }
            else
                InternalCursor.CursorPosition.X = 0;


            if (InternalCursor.CursorPosition.Y < 0)
                InternalCursor.CursorPosition.Y = 0;

            if (InternalCursor.CursorPosition.X < 0)
                InternalCursor.CursorPosition.X = 0;
        }

        public void CheckCursorScroll()
        {
            CheckCursorBounds();

            for (int i = 0; i < 5; i++)
            {
                if ((InternalCursor.CursorPosition.Y - InternalTextComponent.Scroll.Y) < 3)
                    InternalTextComponent.Scroll.Y--;
                if ((InternalCursor.CursorPosition.Y - InternalTextComponent.Scroll.Y) > InternalTextComponent.Size.Height - 3)
                    InternalTextComponent.Scroll.Y++;
            }
            if (InternalTextComponent.Scroll.Y < 0)
                InternalTextComponent.Scroll.Y = 0;

            for (int i = 0; i < 5; i++)
            {
                while ((InternalCursor.CursorPosition.X - InternalTextComponent.Scroll.X) < 3)
                    InternalTextComponent.Scroll.X--;
                while ((InternalCursor.CursorPosition.X - InternalTextComponent.Scroll.X) > InternalTextComponent.Size.Width - 3)
                    InternalTextComponent.Scroll.X++;
            }
            if (InternalTextComponent.Scroll.X < 0)
                InternalTextComponent.Scroll.X = 0;

        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.LeftArrow ||
                info.Key == ConsoleKey.UpArrow ||
                info.Key == ConsoleKey.RightArrow ||
                info.Key == ConsoleKey.DownArrow)
            {
                if (info.Key == ConsoleKey.LeftArrow)
                {
                    InternalCursor.CursorPosition.X--;

                    if (InternalCursor.CursorPosition.X < 0)
                    {
                        if (InternalCursor.CursorPosition.Y > 0 &&
                            InternalCursor.CursorPosition.Y < InternalTextComponent.Text.Count)
                        {
                            InternalCursor.CursorPosition.Y--;
                            InternalCursor.CursorPosition.X = InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count;
                        }
                    }
                }
                else if (info.Key == ConsoleKey.UpArrow)
                    InternalCursor.CursorPosition.Y--;
                else if (info.Key == ConsoleKey.RightArrow)
                {
                    InternalCursor.CursorPosition.X++;

                    if (InternalCursor.CursorPosition.Y >= 0 && InternalCursor.CursorPosition.Y < InternalTextComponent.Text.Count)
                    {
                        if (InternalCursor.CursorPosition.X > InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count)
                        {
                            if (InternalCursor.CursorPosition.Y < InternalTextComponent.Text.Count - 1)
                            {
                                InternalCursor.CursorPosition.X = 0;
                                InternalCursor.CursorPosition.Y++;
                            }
                        }
                    }
                    else
                        InternalCursor.CursorPosition.X = 0;
                }
                else if (info.Key == ConsoleKey.DownArrow)
                    InternalCursor.CursorPosition.Y++;


                CheckCursorBounds();

                return;
            }
            CheckCursorBounds();



            if (info.Key == ConsoleKey.Escape && info.Modifiers == ConsoleModifiers.Shift)
            {
                InternalTextComponent.Clear();
                InternalTextComponent.WriteLineText();
            }
            else if (info.Key == ConsoleKey.Enter)
            {
                //InternalTextComponent.WriteLineText();
                if (InternalCursor.CursorPosition.X == 0)
                {
                    InternalTextComponent.Text.Insert(InternalCursor.CursorPosition.Y, new List<Pixel>());
                    InternalCursor.CursorPosition.Y++;
                }
                else
                {
                    List<Pixel> temp = new List<Pixel>();
                    for (int i = InternalCursor.CursorPosition.X; i < InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count; i++)
                        temp.Add(InternalTextComponent.Text[InternalCursor.CursorPosition.Y][i]);

                    InternalTextComponent.Text[InternalCursor.CursorPosition.Y].RemoveRange(InternalCursor.CursorPosition.X, InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count - InternalCursor.CursorPosition.X);
                    InternalTextComponent.Text.Insert(InternalCursor.CursorPosition.Y + 1, temp);
                    InternalCursor.CursorPosition.Y++;
                    InternalCursor.CursorPosition.X = 0;
                }
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                //InternalTextComponent.WriteText(info.KeyChar.ToString(), ForegroundColor, BackgroundColor);
                int c = info.Modifiers == ConsoleModifiers.Shift ? 4 : 1;
                while (c-- > 0)
                {
                    if (InternalCursor.CursorPosition.X == 0)
                    {
                        if (InternalCursor.CursorPosition.Y > 0)
                        {
                            InternalCursor.CursorPosition.Y--;
                            InternalCursor.CursorPosition.X = InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Count;
                            InternalTextComponent.Text[InternalCursor.CursorPosition.Y].AddRange(InternalTextComponent.Text[InternalCursor.CursorPosition.Y + 1]);
                            InternalTextComponent.Text.RemoveAt(InternalCursor.CursorPosition.Y + 1);
                        }
                    }
                    else
                    {
                        InternalTextComponent.Text[InternalCursor.CursorPosition.Y].RemoveAt(InternalCursor.CursorPosition.X - 1);
                        InternalCursor.CursorPosition.X--;
                    }
                }
            }
            else if (info.Key == ConsoleKey.Tab)
            {
                if (info.Modifiers == ConsoleModifiers.Shift)
                {
                    if (InternalCursor.CursorPosition.X >= 4)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (InternalTextComponent.Text[InternalCursor.CursorPosition.Y][InternalCursor.CursorPosition.X - 1].Character == ' ')
                            {
                                InternalTextComponent.Text[InternalCursor.CursorPosition.Y].RemoveAt(InternalCursor.CursorPosition.X - 1);
                                InternalCursor.CursorPosition.X--;
                            }
                            else
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Insert(InternalCursor.CursorPosition.X, new Pixel(' ', ForegroundColor, BackgroundColor));
                        InternalCursor.CursorPosition.X++;
                    }
                }
            }
            else
            {
                InternalTextComponent.Text[InternalCursor.CursorPosition.Y].Insert(InternalCursor.CursorPosition.X, new Pixel(info.KeyChar, ForegroundColor, BackgroundColor));
                InternalCursor.CursorPosition.X++;
            }
        }


        protected override void InternalUpdate()
        {
            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

            CheckCursorScroll();
            InternalTextComponent.Update();


            for (int i = 0; i < UpdateFields.Count; i++)
            {
                Field2D tempField = UpdateFields[i];

                Rendering.FillPixel(RenderedScreen, tempField, bgPixel);

                InternalTextComponent.RenderTo(RenderedScreen, tempField);

                if (Parent != null)
                    Parent.UpdateFields.Add(tempField + Position);
            }

            UpdateFields.Clear();
            ComponentCursor.CursorPosition = InternalCursor.CursorPosition - InternalTextComponent.Scroll;
        }
    }
}
