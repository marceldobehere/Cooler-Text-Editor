﻿using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public abstract class BasicComponent
    {
        public Position2D Position;
        public Size2D Size;
        public Pixel[,] RenderedScreen;
        public BasicComponent Parent = null;
        public HashSet<Field2D> UpdateFields = new HashSet<Field2D>();
        public bool Visible, OldVisible;
        public PixelColor DefaultBorderColor = PixelColor.Transparent;
        public PixelColor CurrentBorderColor = PixelColor.Transparent;
        public PixelColor OldBorderColor = PixelColor.Transparent;
        public Cursor ComponentCursor = null;
        public bool OverwriteNavigationInput = false;
        

        public void Update()
        {
            if (Parent != null && Size.SizeBasedOnParent != null)
            {
                Size2D temp = Size.SizeBasedOnParent(Parent.Size);
                if (temp.Width < 0)
                    temp.Width = 0;
                if (temp.Height < 0)
                    temp.Height = 0;

                if (temp != Size)
                    Resize(temp);
            }

            if (Size.Width < 0)
                Size.Width = 0;
            if (Size.Height < 0)
                Size.Height = 0;

            if (Size.Width != RenderedScreen.GetLength(0) ||
                Size.Height != RenderedScreen.GetLength(1))
            {
                Resize(Size);
            }

            if (Visible != OldVisible)
            {
                OldVisible = Visible;
                UpdateFields.Clear();

                UpdateFields.Clear();
                UpdateFields.Add(GetLocalField());
            }

            if (this == Cursor.MainCursor.CursorComponent)
                CurrentBorderColor = new PixelColor(100, 120, 150);
            else if (this == Cursor.MainCursor.HoverComponent)
                CurrentBorderColor = new PixelColor(80, 130, 100);
            else
                CurrentBorderColor = DefaultBorderColor;

            if (CurrentBorderColor != OldBorderColor)
            {
                OldBorderColor = CurrentBorderColor;
                //UpdateFields.Clear();
                //UpdateFields.Add(GetLocalField());
                Field2D tField1 = GetPixelField();
                Field2D TopBorder = new Field2D(tField1.TL - new Position2D(1), new Position2D(tField1.BR.X + 1, tField1.TL.Y - 1));
                Field2D LeftBorder = new Field2D(tField1.TL - new Position2D(1), new Position2D(tField1.TL.X - 1, tField1.BR.Y + 1));
                Field2D RightBorder = new Field2D(new Position2D(tField1.BR.X + 1, tField1.TL.Y - 1), tField1.BR + new Position2D(1));
                Field2D BottomBorder = new Field2D(new Position2D(tField1.TL.X - 1, tField1.BR.Y + 1), tField1.BR + new Position2D(1));
                if (Parent != null)
                {
                    Parent.UpdateFields.Add(TopBorder);
                    Parent.UpdateFields.Add(LeftBorder);
                    Parent.UpdateFields.Add(RightBorder);
                    Parent.UpdateFields.Add(BottomBorder);
                }
            }

            InternalUpdate();
        }

        protected abstract void InternalUpdate();

        public void RenderTo(Pixel[,] screen, Field2D field)
        {
            if (!Visible)
                return;

            Field2D tField1 = GetPixelField();
            Field2D tField2 = field;
            Field2D tField3 = new Field2D(new Size2D(screen));
            Field2D internalField = tField3.MergeMinField(tField1.MergeMinField(tField2));
            internalField -= Position;

            for (int y = internalField.TL.Y; y <= internalField.BR.Y; y++)    
                for (int x = internalField.TL.X; x <= internalField.BR.X; x++)
                    screen[x + Position.X, y + Position.Y].WriteOver(RenderedScreen[x, y]);

            if (CurrentBorderColor.IsTransparent)
                return;
            Pixel borderPixel = new Pixel(CurrentBorderColor, CurrentBorderColor);


            Field2D TopBorder = new Field2D(tField1.TL - new Position2D(1), new Position2D(tField1.BR.X + 1, tField1.TL.Y - 1));
            Field2D LeftBorder = new Field2D(tField1.TL - new Position2D(1), new Position2D(tField1.TL.X - 1, tField1.BR.Y + 1));
            Field2D RightBorder = new Field2D(new Position2D(tField1.BR.X + 1, tField1.TL.Y - 1), tField1.BR + new Position2D(1));
            Field2D BottomBorder = new Field2D(new Position2D(tField1.TL.X - 1, tField1.BR.Y + 1), tField1.BR + new Position2D(1));
            {
                //Field2D TopBorder = new Field2D(tField1.TL, new Position2D(tField1.BR.X, tField1.TL.Y));
                TopBorder = tField3.MergeMinField(TopBorder.MergeMinField(tField2));
                Rendering.FillOverPixel(screen, TopBorder, borderPixel);
            }

            {
                //Field2D LeftBorder = new Field2D(tField1.TL, new Position2D(tField1.TL.X, tField1.BR.Y));
                LeftBorder = tField3.MergeMinField(LeftBorder.MergeMinField(tField2));
                Rendering.FillOverPixel(screen, LeftBorder, borderPixel);
            }

            {
                //Field2D RightBorder = new Field2D(new Position2D(tField1.BR.X, tField1.TL.Y), tField1.BR);
                RightBorder = tField3.MergeMinField(RightBorder.MergeMinField(tField2));
                Rendering.FillOverPixel(screen, RightBorder, borderPixel);
            }

            {
                //Field2D BottomBorder = new Field2D(new Position2D(tField1.TL.X, tField1.BR.Y), tField1.BR);
                BottomBorder = tField3.MergeMinField(BottomBorder.MergeMinField(tField2));
                Rendering.FillOverPixel(screen, BottomBorder, borderPixel);
            }
        }

        public Field2D GetField()
        {
            return new Field2D(Position - new Position2D(1, 1), Size + new Position2D(2, 2));
        }

        public Field2D GetLocalField()
        {
            return new Field2D(new Position2D(-1, -1), Size + new Position2D(2, 2));
        }

        public Field2D GetPixelField()
        {
            return new Field2D(Position, Size);
        }

        public Field2D GetLocalPixelField()
        {
            return new Field2D(Size);
        }

        public void Resize(Size2D nSize)
        {
            Resize(nSize.Width, nSize.Height);
        }

        public void Resize(int nWidth, int nHeight)
        {
            Pixel[,] newPixels = new Pixel[nWidth, nHeight];
            for (int y = 0; y < nHeight; y++)
                for (int x = 0; x < nWidth; x++)
                    newPixels[x, y] = Pixel.Empty;
            for (int y = 0; y < Math.Min(RenderedScreen.GetLength(1), nHeight); y++)
                for (int x = 0; x < Math.Min(RenderedScreen.GetLength(0), nWidth); x++)
                    newPixels[x, y] = RenderedScreen[x, y];
            RenderedScreen = newPixels;
            Size.Width = nWidth;
            Size.Height = nHeight;

            Field2D updateFieldYes = GetField();
            UpdateFields.Add(updateFieldYes - Position);
            if (Parent != null)
                Parent.UpdateFields.Add(updateFieldYes);
        }

        public virtual void HandleEnterFocus() { }
        public virtual void HandleExitFocus() { }
        public abstract bool HandleKey(ConsoleKeyInfo info);

        public Position2D GetAbsolutePosition()
        {
            Position2D pos = Position;
            for (var par = Parent; par != null; par = par.Parent)
                pos += par.Position;
            return pos;
        }
    }
}
