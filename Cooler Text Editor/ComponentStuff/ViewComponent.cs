using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class ViewComponent : BasicComponent
    {
        public List<BasicComponent> Children = new List<BasicComponent>();
        public List<Field2D> OldFields = new List<Field2D>();
        public PixelColor BackgroundColor = new PixelColor(10, 10, 10);
        public PixelColor OldBackgroundColor;

        public ViewComponent(Size2D size)
        {
            Size = size;
            Position = new Position2D();
            RenderedScreen = new Pixel[Size.Width, Size.Height];
            OldBackgroundColor = BackgroundColor;

            Pixel bgPixel = new Pixel(BackgroundColor);
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;
        }

        public void AddChild(BasicComponent child)
        {
            Children.Add(child);
            OldFields.Add(new Field2D());
        }

        public override void Update()
        {
            List<Field2D> fieldsToUpdate = new List<Field2D>();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Update();
                Field2D tempField = Children[i].GetField();
                if (OldFields[i] != tempField)
                {
                    if (!fieldsToUpdate.Contains(OldFields[i]))
                        fieldsToUpdate.Add(OldFields[i]);
                    if (!fieldsToUpdate.Contains(tempField))
                        fieldsToUpdate.Add(tempField);
                    OldFields[i] = tempField;
                }
                else if (Children[i].Updated)
                {
                    if (!fieldsToUpdate.Contains(tempField))
                        fieldsToUpdate.Add(tempField);
                }
            }

            if (BackgroundColor != OldBackgroundColor)
            {
                OldBackgroundColor = BackgroundColor;
                fieldsToUpdate.Clear();
                fieldsToUpdate.Add(new Field2D(new Position2D(), Size));
            }

            Pixel bgPixel = new Pixel(BackgroundColor);

            for (int i = 0; i < fieldsToUpdate.Count; i++)
            {
                Field2D tempField = fieldsToUpdate[i];

                Rendering.FillPixel(RenderedScreen, tempField, bgPixel);

                foreach (var child in Children)
                    child.RenderTo(RenderedScreen, tempField);
            }

            if (fieldsToUpdate.Count > 0)
                Updated = true;
        }
    }
}