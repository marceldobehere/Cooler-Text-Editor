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

            OldSize = Size;
            OldPosition = Position;
            OldScroll = Scroll;

            RenderedScreen = new Pixel[0, 0];
            updateInternal = false;
            Updated = true;
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
            Updated = true;
            updateInternal = false;

            RenderedScreen = new Pixel[Size.Width, Size.Height];

            for (int y = Scroll.Y; y < Scroll.Y + Size.Height; y++)
            {
                for (int x = Scroll.X; x < Scroll.X + Size.Width; x++)
                {
                    if (y >= Text.Count || y < 0 || 
                        x >= Text[y].Count || x < 0)
                        RenderedScreen[x, y] = Pixel.Empty;
                    else
                        RenderedScreen[x, y] = Text[y - Scroll.Y][x - Scroll.X];
                }
            }
        }
    }
}
