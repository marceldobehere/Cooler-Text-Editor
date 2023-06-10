using Cooler_Text_Editor.ComponentStuff.TextStuff;
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
    public class TabComponent : BasicComponent
    {
        public ViewComponent TabBox;
        public PixelColor BackgroundColor;
        public PixelColor DefaultTabBackgroundColor, SelectedTabBackgroundColor;
        public PixelColor DefaultTabForegroundColor, SelectedTabForegroundColor;

        public class TabThing
        {
            public CenterTextComponent TabTitle;
            public BasicComponent TabBody;

            public TabThing(BasicComponent tab)
            {
                TabTitle = new CenterTextComponent();
                TabBody = tab;
                UpdateTitle();
            }

            public void UpdateTitle()
            {
                TabTitle.Text = TabBody.Title;
            }
        }

        public void UpdateSelected(TabThing thing)
        {
            thing.TabBody.Visible = thing == CurrentTab;
            if (thing.TabBody.Visible)
            {
                thing.TabTitle.TextComp.DefaultBackgroundColor = SelectedTabBackgroundColor;
                thing.TabTitle.TextComp.DefaultForegroundColor = SelectedTabForegroundColor;
                thing.UpdateTitle();
                thing.TabBody.Position = new Position2D(0, 1);
                thing.TabBody.Size = new Size2D(Size.Width, Size.Height - 1);
            }
            else
            {
                thing.TabTitle.TextComp.DefaultBackgroundColor = DefaultTabBackgroundColor;
                thing.TabTitle.TextComp.DefaultForegroundColor = DefaultTabForegroundColor;
            }
        }

        public void AddTab(BasicComponent tabComp)
        {
            TabThing tab = new TabThing(tabComp);
            Tabs.Add(tab);
            CurrentTab = tab;
            UpdateTabDisplay();
        }

        public void RemoveTab(TabThing tab)
        {
            if (Tabs.Contains(tab))
            {
                Tabs.Remove(tab);
                if (CurrentTab == tab)
                {
                    if (Tabs.Count > 0)
                        CurrentTab = Tabs[0];
                    else
                        CurrentTab = null;
                }
            }
            UpdateTabDisplay();
        }

        public void UpdateTabDisplay()
        {
            foreach (var tab in Tabs)
            {
                UpdateSelected(tab);
            }

            //if (CurrentTab == null)
            //    return;

            if (Tabs.Count < 1)
                return;

            List<TabThing> wantedTabs = new List<TabThing>();
            wantedTabs.AddRange(Tabs); // just for now


            {
                List<BasicComponent> wantedTabTitles = new List<BasicComponent>();
                wantedTabTitles.AddRange(Tabs.Select((tab) => tab.TabTitle));

                for (int i = 0; i < TabBox.Children.Count; i++)
                {
                    var thing = TabBox.Children[i];
                    if (!wantedTabTitles.Contains(thing) &&
                        (CurrentTab == null || CurrentTab.TabBody != thing))
                    {
                        TabBox.RemoveChild(thing);
                        i--;
                    }
                }

                foreach (var tab in wantedTabs)
                    if (!TabBox.Children.Contains(tab.TabTitle))
                        TabBox.AddChild(tab.TabTitle);

                if (CurrentTab != null)
                    if (!TabBox.Children.Contains(CurrentTab.TabBody))
                        TabBox.AddChild(CurrentTab.TabBody);
            }

            int spacePerTabTitle = (Size.Width - wantedTabs.Count) / wantedTabs.Count;
            if (spacePerTabTitle < 5)
                spacePerTabTitle = 5;

            int currentX = 0;
            foreach (var tab in wantedTabs)
            {
                tab.TabTitle.Position = new Position2D(currentX, 0);
                tab.TabTitle.Size = new Size2D(spacePerTabTitle, 1);
                currentX += spacePerTabTitle + 1;
            }
        }

        public List<TabThing> Tabs;
        public TabThing CurrentTab;

        public TabComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            //OldBackgroundColor = BackgroundColor;
            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);
            ComponentCursor.CursorShown = false;


            DefaultTabBackgroundColor = PixelColor.Black;
            SelectedTabBackgroundColor = PixelColor.DarkGray;
            DefaultTabForegroundColor = PixelColor.White;
            SelectedTabForegroundColor = PixelColor.Yellow;


            BackgroundColor = Pixel.DefaultBackgroundColor;
            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;

            TabBox = new ViewComponent(Size2D.UseParent);
            TabBox.Position = Position;
            TabBox.Parent = this;

            CurrentTab = null;
            Tabs = new List<TabThing>();
        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Tab && info.Modifiers == ConsoleModifiers.Control)
            {
                int indx = Tabs.IndexOf(CurrentTab) + Tabs.Count;
                indx++;
                if (Tabs.Count > 0)
                {
                    indx = indx % Tabs.Count;
                    CurrentTab = Tabs[indx];
                    return true;
                }
            }

            if (CurrentTab != null)
                return CurrentTab.TabBody.HandleKey(info);
            return false;
        }

        protected override void InternalUpdate()
        {
            TabBox.Position = Position;
            UpdateTabDisplay();
            TabBox.Update();

            if (CurrentTab != null)
            {
                ComponentCursor.OverwriteCursor(CurrentTab.TabBody.ComponentCursor);
                ComponentCursor.CursorPosition += CurrentTab.TabBody.Position;
            }
            else
                ComponentCursor.CursorShown = false;

            RenderedScreen = TabBox.RenderedScreen;
            if (Parent != null)
                foreach (var tempField in UpdateFields)
                    Parent.UpdateFields.Add(tempField);
            UpdateFields.Clear();
        }
    }
}
