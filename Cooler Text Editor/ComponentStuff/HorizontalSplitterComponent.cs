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
    public class HorizontalSplitterComponent : ViewComponent
    {
        public HorizontalSplitterComponent(Size2D size) : base(size)
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

            if ((info.Key == ConsoleKey.T || info.Key == ConsoleKey.Z) && (info.Modifiers & ConsoleModifiers.Control) != 0)
            {
                BasicComponent newComp = null;
                if (info.Key == ConsoleKey.T)
                    newComp = new FileEditorComponent(new Size2D(10, 10));
                else
                    newComp = new TabComponent(new Size2D(10, 10));
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
                comp.Size.Height = Size.Height - 2;
                comp.Size.Width = (Size.Width - Children.Count*2) / Children.Count;
                comp.Position.Y = 1;
                comp.Position.X = ((Size.Width * x) / Children.Count) + 1;
                x++;
            }
            base.InternalUpdate();
        }
    }
}
