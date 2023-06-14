using Cooler_Text_Editor.ComponentStuff.TextStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class VerticalSplitterComponent : ViewComponent
    {
        public VerticalSplitterComponent(Size2D size) : base(size)
        {

        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.W && info.Modifiers == ConsoleModifiers.Control)
            {
                if (ComponentCursor.HoverComponent != null)
                {
                    RemoveChild(ComponentCursor.HoverComponent);
                    ComponentCursor.HoverComponent = null;
                    if (Children.Count > 0)
                        ComponentCursor.HoverComponent = Children[Children.Count - 1];
                }

                return true;
            }

            if (info.Key == ConsoleKey.T && (info.Modifiers & ConsoleModifiers.Control) != 0)
            {
                BasicComponent newComp = null;
                {
                    newComp = new HorizontalSplitterComponent(new Size2D(10, 10));
                }
                if (ComponentCursor.HoverComponent == null ||
                    !Children.Contains(ComponentCursor.HoverComponent))
                {
                    AddChild(newComp);
                }
                else
                {
                    int indx = Children.IndexOf(ComponentCursor.HoverComponent);
                    if ((info.Modifiers & ConsoleModifiers.Shift) != 0)
                        indx++;

                    Children.Insert(indx, newComp);
                    OldFields.Insert(indx, new Field2D());
                    newComp.Parent = this;
                }

                ComponentCursor.HoverComponent = newComp;
                return true;
            }

            return base.InternalHandleKey(info);
        }

        protected override void InternalUpdate()
        {
            int x = 0;
            foreach (var comp in Children)
            {
                comp.Size.Height = (Size.Height - Children.Count * 2) / Children.Count;
                comp.Size.Width = Size.Width - 2;
                comp.Position.X = 1;
                comp.Position.Y = ((Size.Height * x) / Children.Count) + 1;
                x++;
            }
            base.InternalUpdate();
        }
    }
}
