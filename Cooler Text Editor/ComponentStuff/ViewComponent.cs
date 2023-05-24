﻿using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class ViewComponent : BasicComponent
    {
        public List<BasicComponent> Children = new List<BasicComponent>();
        public List<Field2D> OldFields = new List<Field2D>();
        public PixelColor BackgroundColor = Pixel.DefaultBackgroundColor;// new PixelColor(10, 10, 10);
        public PixelColor OldBackgroundColor;

        public ViewComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            OldBackgroundColor = BackgroundColor;
            UpdateFields = new List<Field2D>();
            ComponentCursor = new Cursor(this);

            Pixel bgPixel = new Pixel(BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;
        }

        public void AddChild(BasicComponent child)
        {
            child.Parent = this;
            Children.Add(child);
            OldFields.Add(new Field2D(new Size2D(-1, -1)));

        }

        public void RemoveChild(BasicComponent child)
        {
            if (child == null)
                return;
            if (child.Parent != this)
                return;
            if (!Children.Contains(child))
                return;

            UpdateFields.Add(child.GetField());
            child.Parent = null;
            OldFields.RemoveAt(Children.IndexOf(child));
            Children.Remove(child);
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Modifiers == ConsoleModifiers.Control &&
                (info.Key == ConsoleKey.RightArrow ||
                info.Key == ConsoleKey.LeftArrow))
            {
                if (Children.Count == 0)
                {
                    ComponentCursor.HoverComponent = null;
                    return;
                }

                if (Children.Contains(ComponentCursor.HoverComponent))
                {
                    int cIndex = Children.IndexOf(ComponentCursor.HoverComponent) + Children.Count;

                    if (info.Modifiers == ConsoleModifiers.Control &&
                        info.Key == ConsoleKey.RightArrow)
                        cIndex++;
                    else if (info.Modifiers == ConsoleModifiers.Control &&
                        info.Key == ConsoleKey.LeftArrow)
                        cIndex--;
                    cIndex = cIndex % Children.Count;
                    ComponentCursor.HoverComponent = Children[cIndex];
                }
                else
                {
                    ComponentCursor.HoverComponent = Children[0];
                }
            }

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
                UpdateFields.Add(GetLocalField());
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