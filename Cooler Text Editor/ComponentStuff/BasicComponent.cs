using Cooler_Text_Editor.HelperStuff;
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
        public List<Field2D> UpdateFields = new List<Field2D>();
        public bool Visible, OldVisible;
        // Maybe add Fixed OldSize here and auto Resize
        public PixelColor BorderColor = PixelColor.Red;
        public PixelColor OldBorderColor = PixelColor.Transparent;
        public Cursor ComponentCursor = null;
        

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

            if (Visible != OldVisible)
            {
                OldVisible = Visible;
                UpdateFields.Clear();

                UpdateFields.Clear();
                UpdateFields.Add(GetLocalField());
            }

            if (BorderColor != OldBorderColor)
            {
                OldBorderColor = BorderColor;
                UpdateFields.Clear();
                UpdateFields.Add(GetLocalField());
            }

            InternalUpdate();
        }

        protected abstract void InternalUpdate();

        public void RenderTo(Pixel[,] screen, Field2D field)
        {
            if (!Visible)
                return;

            Field2D tField1 = GetField();
            Field2D tField2 = field;
            Field2D tField3 = new Field2D(new Size2D(screen));
            Field2D internalField = tField3.MergeMinField(tField1.MergeMinField(tField2));
            internalField -= Position;

            //Field2D internalField = field - Position;

            //if (internalField.TL.X < 0)
            //    internalField.TL.X = 0;
            //if (internalField.TL.Y < 0)
            //    internalField.TL.Y = 0;
            //if (internalField.BR.X > Size.Width - 1)
            //    internalField.BR.X = Size.Width - 1;
            //if (internalField.BR.Y > Size.Height - 1)
            //    internalField.BR.Y = Size.Height - 1;


            //// Need to check later
            //if (internalField.TL.X + Position.X < 0)
            //    internalField.TL.X = -Position.X;
            //if (internalField.TL.Y + Position.Y < 0)
            //    internalField.TL.Y = -Position.Y;
            //if (internalField.BR.X + Position.X > screen.GetLength(0) - 1)
            //    internalField.BR.X = screen.GetLength(0) - 1 - Position.X;
            //if (internalField.BR.Y + Position.Y > screen.GetLength(1) - 1)
            //    internalField.BR.Y = screen.GetLength(1) - 1 - Position.Y;


            for (int y = internalField.TL.Y; y <= internalField.BR.Y; y++)    
                for (int x = internalField.TL.X; x <= internalField.BR.X; x++)
                    screen[x + Position.X, y + Position.Y].WriteOver(RenderedScreen[x, y]);

            if (BorderColor.IsTransparent)
                return;
        }

        public Field2D GetField()
        {
            return new Field2D(Position, Size);
        }

        public Field2D GetLocalField()
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
        public abstract void HandleKey(ConsoleKeyInfo info);

        public Position2D GetAbsolutePosition()
        {
            Position2D pos = Position;
            for (var par = Parent; par != null; par = par.Parent)
                pos += par.Position;
            return pos;
        }
    }
}
