using Cooler_Text_Editor.ComponentStuff.TextStuff;
using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff.PopUp.ExplorerStuff
{
    public class ExplorerComponent : BasicPopUpComponent
    {
        public string CurrentPath = null, SearchQuery = "";
        public PixelColor ForegroundColor, BackgroundColor;
        public PixelColor PathForegroundColor, PathBackgroundColor;
        public PixelColor SearchForegroundColor, SearchBackgroundColor;
        public PixelColor SearchModeForegroundColor, SearchModeBackgroundColor;
        public PixelColor ListBackgroundColor, ListFileForegroundColor, ListFolderForegroundColor, ListDriveForegroundColor;
        public ViewComponent View;
        public EditorComponent PathComp, SearchComp, ResultComp;
        public TextComponent SearchModeComp;
        public Cursor InternalCursor;

        public Stack<string> QueryStack = new Stack<string>();
        public Stack<SearchModeEnum> SearchModeStack = new Stack<SearchModeEnum>();
        public Stack<Position2D> CursorPositionStack = new Stack<Position2D>();

        public static int GetPathLevel(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            DirectoryInfo info = new DirectoryInfo(path);
            int i = 0;
            for (; info != null; i++)
                info = info.Parent;
            return i;
        }

        public void SetPath(string path)
        {
            int lvl = GetPathLevel(path);

            QueryStack.Clear();
            SearchModeStack.Clear();
            CursorPositionStack.Clear();

            for (int i = 0; i < lvl; i++)
            {
                QueryStack.Push("");
                SearchModeStack.Push(SearchModeEnum.Contains);
                CursorPositionStack.Push(new Position2D(0, 0));
            }
            CurrentPath = path;
        }


        public SearchModeEnum SearchMode;
        public enum SearchModeEnum
        {
            Contains,
            ContainsCaseSens,
            StartsWith,
            StartsWithCaseSens,
            Regex
        }

        public ExplorerComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();
            Done = false;

            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);

            ForegroundColor = PixelColor.White;
            BackgroundColor = PixelColor.Black;
            PathForegroundColor = PixelColor.Yellow;
            PathBackgroundColor = PixelColor.Black;
            SearchForegroundColor = PixelColor.Green2;
            SearchBackgroundColor = PixelColor.Black;
            SearchModeForegroundColor = PixelColor.Magenta;
            SearchModeBackgroundColor = PixelColor.DarkGray;
            ListFileForegroundColor = PixelColor.White;
            ListFolderForegroundColor = PixelColor.Blue3;
            ListDriveForegroundColor = PixelColor.DarkGreen;
            ListBackgroundColor = PixelColor.Black;

            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;

            OverwriteNavigationInput = true;
            CurrentPath = null;
            SearchQuery = "";
            SearchMode = SearchModeEnum.Contains;



            Size2D sizeView = new Size2D((parent) => { return parent; });
            View = new ViewComponent(sizeView);
            View.Position = new Position2D(0, 0);
            View.Parent = this;

            Size2D sizePath = new Size2D((parent) => { return new Size2D(parent.Width, 1); });
            PathComp = new EditorComponent(sizePath);
            PathComp.Position = new Position2D(0, 0);
            View.AddChild(PathComp);

            Size2D sizeSearch = new Size2D((parent) => { return new Size2D(parent.Width - 1, 1); });
            SearchComp = new EditorComponent(sizeSearch);
            SearchComp.Position = new Position2D(0, 1);
            View.AddChild(SearchComp);

            Size2D sizeSearchMode = new Size2D((parent) => { return new Size2D(1, 1); });
            SearchModeComp = new TextComponent();
            SearchModeComp.Size = sizeSearchMode;
            SearchModeComp.Position = new Position2D(sizeSearch.Width - 2, 1);
            View.AddChild(SearchModeComp);

            Size2D sizeResult = new Size2D((parent) => { return new Size2D(parent.Width, parent.Height - 3); });
            ResultComp = new EditorComponent(sizeResult);
            ResultComp.Position = new Position2D(0, 3);
            ResultComp.EnableInput = false;
            ResultComp.EnableMovement = true;
            View.AddChild(ResultComp);



            UpdateInternalComponents();


            PathComp.InternalTextComponent.Clear();
            PathComp.InternalTextComponent.WriteLineText("<PATH>");

            SearchComp.InternalTextComponent.Clear();
            //SearchComp.InternalTextComponent.WriteLineText("<SEARCH>");

            UpdateSearchModeComp();

            ResultComp.InternalTextComponent.Clear();
            ResultComp.InternalTextComponent.WriteLineText("<RESULT 1>");
            ResultComp.InternalTextComponent.WriteLineText("<RESULT 2>");
            ResultComp.InternalTextComponent.WriteLineText("<RESULT 3>");

            InternalCursor = ResultComp.ComponentCursor;
            View.ComponentCursor.HoverComponent = ResultComp;
            RefreshFullList();

            QueryStack.Clear();
            SearchModeStack.Clear();
            CursorPositionStack.Clear();
        }

        public void UpdateSearchModeComp()
        {
            SearchModeComp.Clear();
            string searchModeStr = "";
            if (SearchMode == SearchModeEnum.Contains)
                searchModeStr = "c";
            else if (SearchMode == SearchModeEnum.ContainsCaseSens)
                searchModeStr = "C";
            else if (SearchMode == SearchModeEnum.StartsWith)
                searchModeStr = "s";
            else if (SearchMode == SearchModeEnum.StartsWithCaseSens)
                searchModeStr = "S";
            else if (SearchMode == SearchModeEnum.Regex)
                searchModeStr = "R";
            SearchModeComp.WriteLineText(searchModeStr);
        }

        public List<string> GetCurrentFolders()
        {
            List<string> res = new List<string>();
            if (CurrentPath == null || CurrentPath == "")
                return res;

            if (!Directory.Exists(CurrentPath))
                return res;

            try
            {
                foreach (var str in Directory.GetDirectories(CurrentPath))
                    res.Add(Path.GetFileName(str));
            }
            catch (Exception e)
            {

            }

            return res;
        }

        public List<string> GetCurrentFiles()
        {
            List<string> res = new List<string>();
            if (CurrentPath == null || CurrentPath == "")
                return res;

            if (!Directory.Exists(CurrentPath))
                return res;

            try
            {
                foreach (var str in Directory.GetFiles(CurrentPath))
                    res.Add(Path.GetFileName(str));
            }
            catch (Exception e)
            {

            }

            return res;
        }

        public List<string> GetCurrentDrives()
        {
            List<string> res = new List<string>();

            if (CurrentPath != null && CurrentPath != "")
                return res;

            try
            {
                res.AddRange(Directory.GetLogicalDrives());
            }
            catch (Exception e)
            {

            }

            return res;
        }

        public static bool FilterSingle(string input, string search, SearchModeEnum mode, Regex regex)
        {
            if (mode == SearchModeEnum.Contains)
                return input.ToLower().Contains(search.ToLower());
            else if (mode == SearchModeEnum.StartsWith)
                return input.ToLower().StartsWith(search.ToLower());
            else if (mode == SearchModeEnum.ContainsCaseSens)
                return input.Contains(search);
            else if (mode == SearchModeEnum.StartsWithCaseSens)
                return input.StartsWith(search);
            else if (mode == SearchModeEnum.Regex)
                return regex != null && regex.IsMatch(input);

            return false;
        }

        public static List<string> Filter(List<string> input, string searchStr, SearchModeEnum mode)
        {
            List<string> res = new List<string>();
            Regex regex = null;
            if (mode == SearchModeEnum.Regex)
            {
                try
                {
                    regex = new Regex(searchStr, RegexOptions.Compiled);
                }
                catch (Exception e)
                {

                }
            }

            foreach (string tempStr in input)
                if (FilterSingle(tempStr, searchStr, mode, regex))
                    res.Add(tempStr);

            return res;
        }

        public void RefreshFullList()
        {
            UpdateInternalComponents();


            PathComp.InternalTextComponent.Clear();
            if (CurrentPath != null)
                PathComp.InternalTextComponent.WriteText(CurrentPath);
            else
                PathComp.InternalTextComponent.WriteText("");
            PathComp.InternalCursor.CursorPosition = new Position2D(PathComp.InternalTextComponent.Text[0].Count, 0);

            //SearchComp.InternalTextComponent.Clear();
            //SearchComp.InternalTextComponent.WriteLineText(SearchQuery);

            UpdateSearchModeComp();


            List<(PixelColor col, List<string> entries)> results = new List<(PixelColor col, List<string> entries)>
            {
                (ListDriveForegroundColor, Filter(GetCurrentDrives(), SearchQuery, SearchMode)),
                (ListFolderForegroundColor, Filter(GetCurrentFolders(), SearchQuery, SearchMode)),
                (ListFileForegroundColor, Filter(GetCurrentFiles(), SearchQuery, SearchMode))
            };

            ResultComp.InternalTextComponent.Clear();
            foreach (var pair in results)
            {
                ResultComp.InternalTextComponent.DefaultForegroundColor = pair.col;
                foreach (string entry in pair.entries)
                    ResultComp.InternalTextComponent.WriteLineText(entry);
            }

            if (ResultComp.InternalTextComponent.Text.Count > 0)
                ResultComp.InternalTextComponent.Text.RemoveAt(ResultComp.InternalTextComponent.Text.Count - 1);
        }

        public void RestoreLastStack(string prev)
        {
            string searchQuery = "";
            SearchModeEnum searchMode = SearchModeEnum.Contains;
            Position2D cursorPos = new Position2D();

            if (QueryStack.Count > 0)
            {
                searchQuery = QueryStack.Pop();
                searchMode = SearchModeStack.Pop();
                cursorPos = CursorPositionStack.Pop();
            }

            CurrentPath = prev;
            SearchMode = searchMode;

            SearchQuery = searchQuery;
            SearchComp.InternalTextComponent.Clear();
            SearchComp.InternalTextComponent.WriteText(searchQuery);

            RefreshFullList();

            ResultComp.InternalCursor.CursorPosition = cursorPos;
        }

        public void SaveCurrStack(string next)
        {
            CurrentPath = next;

            QueryStack.Push(SearchQuery);
            SearchModeStack.Push(SearchMode);
            CursorPositionStack.Push(ResultComp.InternalCursor.CursorPosition);


            SearchQuery = "";
            SearchComp.InternalTextComponent.Clear();
            ResultComp.InternalCursor.CursorPosition = new Position2D();
            RefreshFullList();
        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Escape)
            {
                Done = true;
                return true;
            }


            if (
                (info.Modifiers == ConsoleModifiers.Control &&
                (info.Key == ConsoleKey.RightArrow ||
                info.Key == ConsoleKey.LeftArrow)) ||
                (info.Modifiers == ConsoleModifiers.Alt &&
                (info.Key == ConsoleKey.J ||
                info.Key == ConsoleKey.L)))
            {
                {
                    InternalCursor.CursorComponent.HandleExitFocus();
                    InternalCursor = InternalCursor.CursorComponent.Parent.ComponentCursor;
                    InternalCursor.CursorComponent.HandleEnterFocus();
                }
                View.HandleKey(info);
                {
                    InternalCursor.CursorComponent.HandleExitFocus();
                    InternalCursor = InternalCursor.HoverComponent.ComponentCursor;
                    InternalCursor.CursorComponent.HandleEnterFocus();
                }

                return true;

            }


            if (!View.HandleKey(info))
            {
                var comp = InternalCursor.CursorComponent;
                if (
                    info.Key == ConsoleKey.Backspace && info.Modifiers == ConsoleModifiers.Shift)
                {
                    if (CurrentPath != null && CurrentPath != "")
                    {
                        var dirInfo = Directory.GetParent(CurrentPath);
                        string t = null;
                        if (dirInfo != null)
                            t = dirInfo.FullName;
                        RestoreLastStack(t);
                        //RefreshFullList();
                        //ResultComp.InternalCursor.CursorPosition = new Position2D();
                    }
                }
                else if (info.Key == ConsoleKey.Enter)
                {
                    if (comp == SearchModeComp)
                    {
                        SwitchSearchMode();
                    }
                    else if (comp == PathComp)
                    {
                        SetPath(PathComp.InternalTextComponent.GetString());
                    }
                    else if (comp == SearchComp)
                    {
                        SearchQuery = SearchComp.InternalTextComponent.GetString();
                    }
                    else if (comp == ResultComp)
                    {
                        int y = ResultComp.InternalCursor.CursorPosition.Y;
                        if (y >= 0 && y < ResultComp.InternalTextComponent.Text.Count)
                        {
                            List<Pixel> line = ResultComp.InternalTextComponent.Text[y];
                            if (line.Count > 0)
                            {
                                string name = TextComponent.GetLineString(ResultComp.InternalTextComponent.Text[y]);
                                PixelColor col = ResultComp.InternalTextComponent.Text[y][0].ForegroundColor;
                                HandleClick(name, col);
                            }
                        }
                    }

                    RefreshFullList();

                    if (info.Modifiers == ConsoleModifiers.Shift &&
                        comp == SearchComp)
                    {
                        if (ResultComp.InternalTextComponent.Text.Count == 1 &&
                            ResultComp.InternalTextComponent.Text[0].Count > 0)
                        {
                            HandleClick(ResultComp.InternalTextComponent.GetString(),
                                ResultComp.InternalTextComponent.Text[0][0].ForegroundColor);
                            return true;
                        }
                    }
                }
                else
                {
                    if (InternalCursor.CursorComponent == SearchComp)
                    {
                        InternalCursor.CursorComponent.HandleKey(info);
                        SearchQuery = SearchComp.InternalTextComponent.GetString();
                        RefreshFullList();
                    }
                    else if (InternalCursor.CursorComponent == ResultComp)
                    {
                        if (!InternalCursor.CursorComponent.HandleKey(info))
                        {
                            SearchComp.HandleKey(info);
                            SearchQuery = SearchComp.InternalTextComponent.GetString();
                            RefreshFullList();
                        }
                    }
                    else
                        InternalCursor.CursorComponent.HandleKey(info);
                }
            }

            return true;

        }

        public Action<string> OnFileClicked;

        public void HandleClick(string name, PixelColor col)
        {
            if (col == ListFileForegroundColor)
            {
                //CurrentPath = name;
                if (OnFileClicked != null)
                    OnFileClicked(CurrentPath + "/" + name);
            }
            else if (col == ListFolderForegroundColor)
            {
                SaveCurrStack(CurrentPath + "/" + name);
            }
            else if (col == ListDriveForegroundColor)
            {
                CurrentPath = name;
                SearchQuery = "";
                SearchComp.InternalTextComponent.Clear();
                ResultComp.InternalCursor.CursorPosition = new Position2D();
            }
            RefreshFullList();
        }

        public void SwitchSearchMode()
        {
            if (SearchMode == SearchModeEnum.Contains)
                SearchMode = SearchModeEnum.ContainsCaseSens;
            else if (SearchMode == SearchModeEnum.ContainsCaseSens)
                SearchMode = SearchModeEnum.StartsWith;
            else if (SearchMode == SearchModeEnum.StartsWith)
                SearchMode = SearchModeEnum.StartsWithCaseSens;
            else if (SearchMode == SearchModeEnum.StartsWithCaseSens)
                SearchMode = SearchModeEnum.Regex;
            else if (SearchMode == SearchModeEnum.Regex)
                SearchMode = SearchModeEnum.Contains;
        }

        public void UpdateInternalComponents()
        {
            PathComp.ForegroundColor = PathForegroundColor;
            PathComp.BackgroundColor = PathBackgroundColor;
            PathComp.Update();

            SearchComp.ForegroundColor = SearchForegroundColor;
            SearchComp.BackgroundColor = SearchBackgroundColor;
            SearchComp.Update();

            SearchModeComp.DefaultForegroundColor = SearchModeForegroundColor;
            SearchModeComp.DefaultBackgroundColor = SearchModeBackgroundColor;
            SearchModeComp.BackgroundColor = SearchModeBackgroundColor;
            SearchModeComp.Position = new Position2D(Size.Width - 2, 1);
            SearchModeComp.Update();

            ResultComp.BackgroundColor = ListBackgroundColor;
            ResultComp.ForegroundColor = ListFileForegroundColor;
            ResultComp.Update();

            View.BackgroundColor = BackgroundColor;
        }

        protected override void InternalUpdate()
        {
            Title = $"Explorer";
            UpdateInternalComponents();

            Cursor temp = InternalCursor;
            if (InternalCursor.HoverComponent != null)
                temp = InternalCursor.HoverComponent.ComponentCursor;

            ComponentCursor.OverwriteCursor(temp);
            ComponentCursor.CursorPosition += temp.CursorComponent.Position;
            ComponentCursor.CursorShown = InternalCursor.CursorComponent != View || InternalCursor.HoverComponent != null;

            View.BackgroundColor = BackgroundColor;

            View.Update();
            RenderedScreen = View.RenderedScreen;


            if (Parent != null && UpdateFields.Count > 0)
                foreach (Field2D tempField in UpdateFields)
                    Parent.UpdateFields.Add(tempField + Position);

            UpdateFields.Clear();
        }
    }
}
