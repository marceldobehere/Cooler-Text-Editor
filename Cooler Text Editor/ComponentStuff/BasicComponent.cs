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
        public bool Updated;

        public abstract void Update();

        public void RenderTo(Pixel[,] screen, Field2D field)
        {
            //if (!Updated)
            //    return;
            Updated = false;

            Field2D internalField = field - Position;

            if (internalField.TL.X < 0)
                internalField.TL.X = 0;
            if (internalField.TL.Y < 0)
                internalField.TL.Y = 0;
            if (internalField.BR.X > Size.Width - 1)
                internalField.BR.X = Size.Width - 1;
            if (internalField.BR.Y > Size.Height - 1)
                internalField.BR.Y = Size.Height - 1;


            // Need to check later
            if (internalField.TL.X + Position.X < 0)
                internalField.TL.X = -Position.X;
            if (internalField.TL.Y + Position.Y < 0)
                internalField.TL.Y = -Position.Y;
            if (internalField.BR.X + Position.X > screen.GetLength(0) - 1)
                internalField.BR.X = screen.GetLength(0) - 1 - Position.X;
            if (internalField.BR.Y + Position.Y > screen.GetLength(1) - 1)
                internalField.BR.Y = screen.GetLength(1) - 1 - Position.Y;

            ;

            for (int y = internalField.TL.Y; y <= internalField.BR.Y; y++)    
                for (int x = internalField.TL.X; x <= internalField.BR.X; x++)
                    screen[x + Position.X, y + Position.Y] = RenderedScreen[x, y];
        }

        public Field2D GetField()
        {
            return new Field2D(Position, Size);
        }
    }
}
