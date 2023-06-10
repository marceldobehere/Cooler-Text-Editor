using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Cooler_Text_Editor.ComponentStuff.TextStuff
{
    public class CenterTextComponent : BasicComponent
    {
        public TextComponent TextComp;
        public ViewComponent ViewComp;
        public bool EnableScrolling;

        public CenterTextComponent()
        {
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();
            Size = new Size2D();
            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);
            ComponentCursor.CursorShown = false;


            RenderedScreen = new Pixel[0, 0];

            TextComp = new TextComponent();
            TitleScrollX = 0;
            LastMS = 0;
            DoTitleScroll = false;
            Text = "";
            EnableScrolling = true;

            ViewComp = new ViewComponent(new Size2D((Size2D parent) => parent));
            ViewComp.Parent = this;
            ViewComp.AddChild(TextComp);

            UpdateTitle();
        }

        public string Text;

        public int TitleScrollX;
        public long LastMS;
        public bool DoTitleScroll;
        public void UpdateTitle()
        {
            string newTitle = Text;
            if (newTitle == null)
                newTitle = "";


            TextComp.Size = new Size2D(newTitle.Length, 1);
            TextComp.Clear();
            TextComp.WriteLineText(newTitle);

            if (newTitle.Length < Size.Width || !EnableScrolling)
            {
                int titleLength = newTitle.Length;
                int w = Size.Width;
                int x = (w - titleLength) / 2;

                TextComp.Position = new Position2D(x, 0);
                DoTitleScroll = false;
            }
            else
            {
                const int padding = 5;
                long tempTime = (long)(uint)Environment.TickCount;
                if (!DoTitleScroll)
                {
                    DoTitleScroll = true;
                    TitleScrollX = 0;
                    LastMS = tempTime;
                }

                if (tempTime > LastMS + 150)
                {
                    if (TitleScrollX == 0)
                    {
                        if (tempTime > LastMS + 1000)
                        {
                            TitleScrollX++;
                            LastMS = tempTime;
                        }
                    }
                    else
                    {
                        int maxW = newTitle.Length - Size.Width + 2 * padding;
                        if (TitleScrollX >= maxW)
                        {
                            if (tempTime > LastMS + 1000)
                            {
                                TitleScrollX = 0;
                                LastMS = tempTime;
                            }
                            else
                                TitleScrollX = maxW;
                        }
                        else
                        {
                            TitleScrollX++;
                            LastMS = tempTime;
                        }
                    }
                }

                TextComp.Position = new Position2D(padding - TitleScrollX, 0);
                //TextComp.Update();
            }
        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            return false;
        }

        protected override void InternalUpdate()
        {
            UpdateTitle();
            ViewComp.Update();

            RenderedScreen = ViewComp.RenderedScreen;
            if (Parent != null)
                foreach (var tempField in UpdateFields)
                    Parent.UpdateFields.Add(tempField);
            UpdateFields.Clear();
        }
    }
}
