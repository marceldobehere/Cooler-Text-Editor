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
    public class ScreenComponent : BasicComponent
    {
        public ViewComponent MainView;

        public ScreenComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();
            
            UpdateFields = new List<Field2D>();
            ComponentCursor = new Cursor(this);
            

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = Pixel.Transparent;

            MainView = new ViewComponent(Size);
            MainView.Parent = this;
            MainView.Size.SizeBasedOnParent = (Size2D parent) => { return parent; };
            ComponentCursor.HoverComponent = MainView;
        }

        protected override void InternalUpdate()
        {
            //if (MainView.Size != Size)
            //    MainView.Resize(Size);
            MainView.Update();
        }

        public void RenderStuffToScreen()
        {
            //foreach (Field2D field in UpdateFields)
            //    MainView.RenderTo(screen, field);

            Rendering.RenderFields(MainView.RenderedScreen, UpdateFields);
            UpdateFields.Clear();
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            MainView.HandleKey(info);
        }
    }
}
