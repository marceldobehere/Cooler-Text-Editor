using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.RenderingStuff
{
    public struct Field2D
    {
        public Position2D TL;
        public Position2D BR;

        public Field2D(Position2D tl, Position2D br)
        {
            TL = tl;
            BR = br;
        }

        public Field2D(Position2D tl, Size2D size)
        {
            TL = tl;
            BR = tl + new Position2D(size.Width - 1, size.Height - 1);
        }

        public Field2D()
        {
            TL = new Position2D();
            BR = new Position2D();
        }

        public Field2D(Size2D size)
        {
            TL = new Position2D();
            BR = new Position2D(size.Width - 1, size.Height - 1);
        }

        public static Field2D operator +(Field2D a, Position2D b)
        {
            return new Field2D(a.TL + b, a.BR + b);
        }

        public static Field2D operator -(Field2D a, Position2D b)
        {
            return new Field2D(a.TL - b, a.BR - b);
        }

        public static bool operator ==(Field2D a, Field2D b)
        {
            return a.TL == b.TL && a.BR == b.BR;
        }

        public static bool operator !=(Field2D a, Field2D b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Field2D))
                return false;
            Field2D field = (Field2D)obj;
            return field.TL == TL && field.BR == BR;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TL.GetHashCode(), BR.GetHashCode());
        }

        public override string ToString()
        {
            return $"({TL} - {BR})";
        }

        public Field2D MergeMinField(Field2D smoller)
        {
            bool isValid = true;
            // check if invalid (only if the fields dont touch at all
            if (smoller.TL.X > BR.X || smoller.TL.Y > BR.Y || 
                smoller.BR.X < TL.X || smoller.BR.Y < TL.Y)
                isValid = false;

            if (smoller.TL.X < TL.X)
                smoller.TL.X = TL.X;
            if (smoller.TL.Y < TL.Y)
                smoller.TL.Y = TL.Y;
            if (smoller.BR.X > BR.X)
                smoller.BR.X = BR.X;
            if (smoller.BR.Y > BR.Y)
                smoller.BR.Y = BR.Y;

            if (!isValid)
                return new Field2D(new Size2D(-1, -1));

            return smoller;
        }
    }
}
