using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class TextComponent : BasicComponent
    {
        public List<List<Pixel>> Text;
        public bool updateInternal;
        public Size2D OldSize;
        public Position2D OldPosition;

        public Position2D Scroll;
        public Position2D OldScroll;

        public TextComponent()
        {
            Text = new List<List<Pixel>>();
            Position = new Position2D();
            Size = new Size2D();
            Scroll = new Position2D();
            UpdateFields = new List<Field2D>();

            OldSize = Size;
            OldPosition = Position;
            OldScroll = Scroll;

            RenderedScreen = new Pixel[0, 0];
            updateInternal = false;
        }

        public override void Update()
        {
            if (OldSize != Size || OldPosition != Position ||
                OldScroll != Scroll)
            {
                OldSize = Size;
                OldPosition = Position;
                updateInternal = true;
            }

            if (!updateInternal)
                return;
            updateInternal = false;

            RenderedScreen = new Pixel[Size.Width, Size.Height];

            bool tU = false;

            for (int y = Scroll.Y; y < Scroll.Y + Size.Height; y++)
            {
                for (int x = Scroll.X; x < Scroll.X + Size.Width; x++)
                {
                    Pixel t = Pixel.Empty;
                    if (y < Text.Count && y >= 0 && 
                        x < Text[y].Count && x >= 0)
                        t = Text[y - Scroll.Y][x - Scroll.X]; ;

                    if (RenderedScreen[x, y] != t)
                    {
                        RenderedScreen[x, y] = t;
                        tU = true;
                    }
                }
            }

            if (tU && Parent != null)
                Parent.UpdateFields.Add(GetField());
        }
    }
}
