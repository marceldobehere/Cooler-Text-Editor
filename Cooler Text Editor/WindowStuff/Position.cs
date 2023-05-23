using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.WindowStuff
{
    public struct Position2D
    {
        public int X;
        public int Y;

        public Position2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position2D()
        {
            X = 0;
            Y = 0;
        }

        public static Position2D operator +(Position2D a, Position2D b)
        {
            return new Position2D(a.X + b.X, a.Y + b.Y);
        }

        public static Position2D operator -(Position2D a, Position2D b)
        {
            return new Position2D(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Position2D a, Position2D b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Position2D a, Position2D b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position2D))
                return false;
            Position2D pos = (Position2D)obj;
            return pos.X == X && pos.Y == Y;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
