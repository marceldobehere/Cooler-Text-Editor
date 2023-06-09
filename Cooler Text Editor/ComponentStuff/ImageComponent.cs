using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.ColorSpaces;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class ImageComponent : BasicComponent
    {
        
        public Image InternalImage;
        public Size2D OldSize;
        public bool IsGif;
        public int frameDelay, currentFrame, oldFrame, delayLeft, lastTimeMS;
        PixelColor[][,] gifData;

        public ImageComponent(Image image)
        {
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            //OldBackgroundColor = BackgroundColor;
            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);
            ComponentCursor.CursorShown = false;

            LoadImage(image);
        }

        public void LoadImage(Image image)
        {
            InternalImage = image;
            if (image == null)
                Size = new Size2D(1, 1);
            else
                Size = new Size2D(image.Width, image.Height / 2);

            int fCount = InternalImage.Frames.Count;
            //if (InternalImage.PropertyIdList.Contains(0x5100))
            if (fCount > 1)
            {
                IsGif = true;

                int tDel = InternalImage.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay;
                int delay = tDel * 10; // Time is in milliseconds

                frameDelay = delay;
                currentFrame = 0;
            }
            else
            {
                IsGif = false;

                frameDelay = 1000;
                currentFrame = 0;
            }
            delayLeft = frameDelay;
            lastTimeMS = Environment.TickCount;


            gifData = new PixelColor[fCount][,];//[Image.Width, Image.Height];
            {
                int i = 0;
                foreach (var frame in InternalImage.Frames)
                    gifData[i++] = ConvBitmap(frame);
            }


            UpdateScreen();
        }

        public PixelColor[,] ConvBitmap(ImageFrame image)
        {
            PixelColor[,] pixels = new PixelColor[image.Width, image.Height];

            

            //BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //Console.WriteLine($"Image: {image.Width}x{image.Height} = {image.Width * image.Height} ({image.Width * image.Height * 4} bytes)");
            //byte[] imageData = new byte[image.Height * image.Width * 4];
            //try
            //{
            //unsafe
            //{
            //    byte* ptr = (byte*)bitmapData.Scan0;
            //    for (int i = 0; i < image.Height * image.Width * 4; i += 4)
            //    {
            //        byte R = ptr[i+2];
            //        byte G = ptr[i+1];
            //        byte B = ptr[i+0];
            //        byte A = ptr[i+3];

            //        pixels[(i / 4) % image.Width, (i / 4) / image.Width] = new PixelColor(R, G, B);
            //    }
            //}

            int imgH = image.Height;
            int imgW = image.Width;
            //image.Metadata.Get
            ImageFrame<Rgba32> img = (ImageFrame<Rgba32>)image;

            for (int y = 0; y < imgH; y++)
                for (int x = 0; x < imgW; x++)
                {
                    Rgba32 t = img[x, y];
                    pixels[x, y] = new PixelColor(t.R, t.G, t.B);
                }

            
            //unsafe
            //{
            //    byte* ptr = (byte*)bitmapData.Scan0;
            //    Parallel.For(0, imgH * imgW, i =>
            //    {
            //        i *= 4;
            //        byte R = ptr[i + 2];
            //        byte G = ptr[i + 1];
            //        byte B = ptr[i + 0];
            //        //byte A = ptr[i + 3];

            //        pixels[(i / 4) % imgW, (i / 4) / imgW] = new PixelColor(R, G, B);
            //    });
            //}

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Reading Image failed!");
            //}

            //image.UnlockBits(bitmapData);
            return pixels;
        }

        public void UpdateScreen()
        {
            if (RenderedScreen == null ||
                Size.Width != RenderedScreen.GetLength(0) || 
                Size.Height != RenderedScreen.GetLength(1))
                RenderedScreen = new Pixel[Size.Width, Size.Height];

            if (InternalImage == null)
            {
                Pixel bgPixel = Pixel.Transparent;
                for (int y = 0; y < Size.Height; y++)
                    for (int x = 0; x < Size.Width; x++)
                        RenderedScreen[x, y] = bgPixel;
                return;
            }

            currentFrame = currentFrame % gifData.Length;

            //if (IsGif)
            //{
            //    Image.SelectActiveFrame(FrameDimension.Time, currentFrame);
            //}

            oldFrame = currentFrame;

            int iWidth = InternalImage.Width;
            int iHeight = InternalImage.Height;

            //PixelColor[,] convImg = ConvBitmap(Image);

            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    int y2 = y * 2;
                    int aY1 = (y2 * iHeight) / (Size.Height * 2);
                    int aY2 = ((y2 + 1) * iHeight) / (Size.Height * 2);
                    int aX = (x * iWidth) / Size.Width;

                    //Color color1 = Image.GetPixel(aX, aY1);
                    //Color color2 = Image.GetPixel(aX, aY2);

                    //PixelColor col1 = new PixelColor(color1.R, color1.G, color1.B);
                    //PixelColor col2 = new PixelColor(color2.R, color2.G, color2.B);

                    //PixelColor col1 = convImg[aX, aY1];
                    //PixelColor col2 = convImg[aX, aY2];

                    PixelColor col1 = gifData[currentFrame][aX, aY1];
                    PixelColor col2 = gifData[currentFrame][aX, aY2];

                    //if (currentFrame != 0)
                    //    break;
                    RenderedScreen[x, y] = Pixel.Create2DColPixel(col1, col2);
                }
            OldSize = Size;
        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            //throw new NotImplementedException();

            return false;

        }


        protected override void InternalUpdate()
        {
            // update time and frame if neeeded
            if (IsGif)
            {
                int time = Environment.TickCount;
                int diff = time - lastTimeMS;
                lastTimeMS = time;

                delayLeft -= diff;
                while (delayLeft < 0)
                {
                    currentFrame++;
                    delayLeft += frameDelay;
                }
            }


            if (OldSize != Size || currentFrame != oldFrame)
            {
                UpdateScreen();
                if (Parent != null)
                    Parent.UpdateFields.Add(GetField());
            }
        }
    }
}
