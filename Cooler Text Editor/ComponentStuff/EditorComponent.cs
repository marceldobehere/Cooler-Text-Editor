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

                return;
            }


            if (info.Key == ConsoleKey.Enter)
            {
                InternalTextComponent.WriteLineText();
            }
            else if (info.Key == ConsoleKey.Backspace)
            {

                InternalTextComponent.WriteText(info.KeyChar.ToString(), ForegroundColor, BackgroundColor);
            }
            else if (info.Key == ConsoleKey.Escape && info.Modifiers == ConsoleModifiers.Shift)
            {
                InternalTextComponent.Clear();
            }
            else
            {
                InternalTextComponent.WriteText(info.KeyChar.ToString(), ForegroundColor, BackgroundColor);
            }
        }


        protected override void InternalUpdate()
        {
            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

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
