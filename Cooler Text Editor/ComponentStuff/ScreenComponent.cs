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
            Position = new Position2D();
            RenderedScreen = new Pixel[Size.Width, Size.Height];
            UpdateFields = new List<Field2D>();

            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = Pixel.Empty;

            MainView = new ViewComponent(Size);
            MainView.Parent = this;
        }

        public override void Update()
        {
            MainView.Update();
        }

        public void RenderStuffToScreen()
        {
            //foreach (Field2D field in UpdateFields)
            //    MainView.RenderTo(screen, field);

            Rendering.RenderFields(MainView.RenderedScreen, UpdateFields);
            UpdateFields.Clear();
        }

    }
}
