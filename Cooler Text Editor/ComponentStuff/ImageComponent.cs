using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class ImageComponent : BasicComponent
    {
        public Bitmap Image;
        public Size2D OldSize;

        public ImageComponent(Bitmap image)
        {
            Image = image;
            if (image == null)
                Size = new Size2D(1, 1);
            else
                Size = new Size2D(image.Width, image.Height / 2);
            Image = image;

            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            //OldBackgroundColor = BackgroundColor;
            UpdateFields = new List<Field2D>();
            ComponentCursor = new Cursor(this);

            UpdateScreen();
        }

        public void UpdateScreen()
        {
            RenderedScreen = new Pixel[Size.Width, Size.Height];
            if (Image == null)
            {
                Pixel bgPixel = Pixel.Transparent;
                for (int y = 0; y < Size.Height; y++)
                    for (int x = 0; x < Size.Width; x++)
                        RenderedScreen[x, y] = bgPixel;
                return;
            }

            int iWidth = Image.Width;
            int iHeight = Image.Height;

            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    int y2 = y * 2;
                    int aY1 = (y2 * iHeight) / (Size.Height * 2);
                    int aY2 = ((y2 + 1) * iHeight) / (Size.Height * 2);
                    int aX = (x * iWidth) / Size.Width;

                    Color color1 = Image.GetPixel(aX, aY1);
                    Color color2 = Image.GetPixel(aX, aY2);

                    PixelColor col1 = new PixelColor(color1.R, color1.G, color1.B);
                    PixelColor col2 = new PixelColor(color2.R, color2.G, color2.B);

                    RenderedScreen[x, y] = Pixel.Create2DColPixel(col1, col2);
                }
            OldSize = Size;
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            //throw new NotImplementedException();
        }


        protected override void InternalUpdate()
        {
            if (OldSize != Size)
            {
                UpdateScreen();
                if (Parent != null)
                    Parent.UpdateFields.Add(GetField());
            }
        }
    }
}
