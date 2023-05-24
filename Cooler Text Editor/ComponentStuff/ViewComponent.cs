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
            UpdateFields = new List<Field2D>();

            Pixel bgPixel = new Pixel(BackgroundColor);
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;
        }

        public void AddChild(BasicComponent child)
        {
            child.Parent = this;
            Children.Add(child);
            OldFields.Add(new Field2D());
        }

        protected override void InternalUpdate()
        {

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Update();
                Field2D tempField = Children[i].GetField();
                if (OldFields[i] != tempField)
                {
                    if (!UpdateFields.Contains(OldFields[i]))
                        UpdateFields.Add(OldFields[i]);
                    if (!UpdateFields.Contains(tempField))
                        UpdateFields.Add(tempField);
                    OldFields[i] = tempField;
                }
            }

            if (BackgroundColor != OldBackgroundColor)
            {
                OldBackgroundColor = BackgroundColor;
                UpdateFields.Clear();
                UpdateFields.Add(new Field2D(new Position2D(), Size));
            }

            Pixel bgPixel = new Pixel(BackgroundColor);

            for (int i = 0; i < UpdateFields.Count; i++)
            {
                Field2D tempField = UpdateFields[i];

                Rendering.FillPixel(RenderedScreen, tempField, bgPixel);

                foreach (var child in Children)
                    child.RenderTo(RenderedScreen, tempField);

                if (Parent != null)
                    Parent.UpdateFields.Add(tempField + Position);
            }

            UpdateFields.Clear();
        }
    }
}