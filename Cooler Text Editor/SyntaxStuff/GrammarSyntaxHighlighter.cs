using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using static Cooler_Text_Editor.SyntaxStuff.GrammarSyntaxHighlighter;
using static System.Net.Mime.MediaTypeNames;

namespace Cooler_Text_Editor.SyntaxStuff
{
    public class GrammarSyntaxHighlighter : BasicSyntaxHighlighter
    {
        public Dictionary<string, PixelColor> ColorStrings = new Dictionary<string, PixelColor>()
        {
            {"White", PixelColor.White},
            {"Black", PixelColor.Black},
            {"Red", PixelColor.Red},
            {"Green", PixelColor.Green},
            {"Green2", PixelColor.Green2},
            {"Orange", PixelColor.Orange},
            {"Blue", PixelColor.Blue},
            {"Blue2", PixelColor.Blue2},
            {"Blue3", PixelColor.Blue3},
            {"Yellow", PixelColor.Yellow},
            {"Yellow2", PixelColor.Yellow2},
            {"Magenta", PixelColor.Magenta},
            {"Magenta2", PixelColor.Magenta2},
            {"Cyan", PixelColor.Cyan},
            {"Gray", PixelColor.Gray},
            {"Gray2", PixelColor.Gray2},
            {"DarkGray", PixelColor.DarkGray},
            {"DarkRed", PixelColor.DarkRed},
            {"DarkGreen", PixelColor.DarkGreen},
            {"DarkBlue", PixelColor.DarkBlue},
            {"DarkYellow", PixelColor.DarkYellow},
            {"DarkMagenta", PixelColor.DarkMagenta},
            {"DarkCyan", PixelColor.DarkCyan},
            {"Transparent", PixelColor.Transparent}
        };

        public class GrammarKeyword
        {
            public PixelColor? FG;
            public PixelColor? BG;
            public List<string> Words;
            public string Name;

            public GrammarKeyword(string name, PixelColor? fg, PixelColor? bg)
            {
                FG = fg;
                BG = bg;
                Words = new List<string>();
                Name = name;
            }
        }

        public class LanguageRuleset
        {
            public List<string> Extensions;
            public string Name;
            public List<GrammarKeyword> Keywords;
            public string CommentLine = "//";
            public PixelColor? CommentFG = PixelColor.Green;
            public PixelColor? CommentBG = null;


            public LanguageRuleset()
            {
                Extensions = new List<string>();
                Keywords = new List<GrammarKeyword>();
            }
        }

        public List<LanguageRuleset> Rulesets = new List<LanguageRuleset>();
        public void ReadFile(string path)
        {
            if (!File.Exists(path))
                return;

            Rulesets = new List<LanguageRuleset>();

            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    //Console.Clear();
                    bool ignore = true;
                    LanguageRuleset curr = null;
                    string lastNam = "";
                    GrammarKeyword lastKW = null;
                    while (reader.Read())
                    {
                        if (reader.Name == "Language")
                        {
                            ignore = false;
                            if (curr != null)
                                Rulesets.Add(curr);
                            curr = new LanguageRuleset();
                        }
                        if (ignore)
                            continue;

                        string nam = reader.Name;
                        string val = reader.Value;
                        string bruh = "";
                        List<(string name, string val)> tempAttr = new List<(string name, string val)>();
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToNextAttribute();
                            tempAttr.Add((reader.Name, reader.Value));
                            bruh += $"{reader.Name}: {reader.Value}, ";
                        }
                        //Console.WriteLine($" - \"{nam}\": \"{val}\" - {bruh}");

                        if (nam == "Language")
                        {
                            if (tempAttr.FindIndex((x) => x.name == "name") != -1)
                            {
                                curr.Name = tempAttr.Find((x) => x.name == "name").val;
                                string[] exts = tempAttr.Find((x) => x.name == "ext").val.Split(' ');
                                foreach (var ext in exts)
                                    curr.Extensions.Add($".{ext}");

                                // add commentLine, commentStart, commentEnd
                            }
                            ;
                        }
                        else if (nam == "Keywords")
                        {
                            string kwName = tempAttr.Find((x) => x.name == "name").val;
                            lastKW = new GrammarKeyword(kwName, PixelColor.Red, null);
                            curr.Keywords.Add(lastKW);

                        }
                        else if (lastNam == "Keywords")
                        {
                            lastKW.Words.AddRange(val.Split(' '));
                            // add col
                            ;
                        }
                        else if (nam == "" || nam == "Languages" || nam == "NotepadPlus")
                        {

                        }
                        else
                        {
                            //Console.WriteLine($"ERROR: IDK \"{nam}\"");
                            //Console.ReadLine();
                        }

                        lastNam = nam;
                    }
                }




                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError:");
                Console.WriteLine(e);
                Console.ReadLine();
            }
            //Console.ReadLine();

        }

        public static async Task<List<List<Pixel>>> DoDefaultSyntaxHighlight(List<string> data, PixelColor defaultFG, PixelColor defaultBG)
        {
            List<List<Pixel>> res = new List<List<Pixel>>();
            // add default color
            for (int i = 0; i < data.Count; i++)
            {
                res.Add(new List<Pixel>());
                for (int j = 0; j < data[i].Length; j++)
                    res[i].Add(new Pixel(data[i][j], defaultFG, defaultBG));
            }
            return res;
        }

        public class GrammarToken
        {
            public string Text;
            public (int x, int y) Position;

            public PixelColor FG;
            public PixelColor BG;

            public GrammarToken(string text, (int x, int y) pos, PixelColor fg, PixelColor bg)
            {
                Text = text;
                Position = pos;
                FG = fg;
                BG = bg;
            }
        }

        public override async Task<List<List<Pixel>>> SyntaxHighlight(List<string> data, string ext, PixelColor defaultFG, PixelColor defaultBG)
        {
            LanguageRuleset set = null;
            foreach (var tempSet in Rulesets)
                if (tempSet.Extensions.Contains(ext))
                {
                    set = tempSet;
                    break;
                }

            if (set == null)
                return await DoDefaultSyntaxHighlight(data, defaultFG, defaultBG);
            await Task.Delay(50);

            List<GrammarToken> toks = new List<GrammarToken>();

            {
                int x = 0;
                int y = 0;
                string tempStr = "";
                bool inString = false;

                void addStrIfNeeded(int lastX, char ch = ' ', bool add = false)
                {
                    int x2 = lastX + 1;
                    if (tempStr != "")
                    {
                        toks.Add(new GrammarToken(tempStr, (x2 - tempStr.Length, y), defaultFG, defaultBG));
                        tempStr = "";
                    }
                    tempStr = "";
                    if (add)
                    {
                        tempStr = ch.ToString();
                        addStrIfNeeded(lastX + 1);
                    }
                }

                while (y < data.Count)
                {
                    if (x >= data[y].Length)
                    {
                        addStrIfNeeded(x - 1);
                        x = 0;
                        y++;
                        continue;
                    }

                    if (set.CommentLine != null &&
                        data[y].Substring(x).StartsWith(set.CommentLine))
                    {
                        addStrIfNeeded(x - 1);
                        toks.Add(new GrammarToken(data[y].Substring(x), (x, y), set.CommentFG ?? defaultFG, set.CommentBG ?? defaultBG));
                        x = 0;
                        y++;
                        continue;
                    }

                    char chr = data[y][x];
                    if (chr == '"')
                    {
                        inString = !inString;
                        if (inString)
                            addStrIfNeeded(x - 1);
                        tempStr += chr;
                        if (!inString)
                            addStrIfNeeded(x);
                    }
                    else if (chr == ' ')
                    {
                        if (inString)
                            tempStr += chr;
                        else
                            addStrIfNeeded(x - 1, chr, true);
                    }
                    else if ("[]{}<>+-.,:;()?%|&~*/&=".Contains(chr))
                    {
                        if (inString)
                            tempStr += chr;
                        else if (chr == '-' && (x + 1 < data[y].Length) &&
                            "0123456789".Contains(data[y][x + 1]))
                            tempStr += chr;
                        else if (chr == '.' && (x > 0) &&
                            "0123456789".Contains(data[y][x - 1]))
                            tempStr += chr;
                        else
                            addStrIfNeeded(x - 1, chr, true);
                    }
                    else if (chr == '\\')
                    {
                        if (inString)
                        {
                            if (x + 1 < data[y].Length)
                            {
                                tempStr += data[y][x + 1];
                                x++;
                            }
                        }
                        else
                            tempStr += chr;
                    }
                    else
                    {
                        tempStr += chr;
                    }


                    x++;
                }
                addStrIfNeeded(x);
            }

            foreach (var tok in toks)
            {
                bool f = false;
                foreach (var kw in set.Keywords)
                {
                    if (kw.Words.Contains(tok.Text))
                    {
                        tok.FG = kw.FG ?? tok.FG;
                        tok.BG = kw.BG ?? tok.BG;
                        f = true;
                        break;
                    }
                }
                if (f)
                    continue;

                if (tok.Text.StartsWith("\"") || tok.Text.EndsWith("\""))
                {
                    tok.FG = PixelColor.Orange;
                    continue;
                }

                if (float.TryParse(tok.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
                {
                    if (num > 0)
                        tok.FG = PixelColor.Green2;
                    else if (num == 0)
                        tok.FG = PixelColor.Cyan;
                    else
                        tok.FG = PixelColor.Red2;
                    continue;
                }
            }


            List<List<Pixel>> res = new List<List<Pixel>>();
            for (int i = 0; i < data.Count; i++)
            {
                res.Add(new List<Pixel>());
                for (int j = 0; j < data[i].Length; j++)
                    res[i].Add(new Pixel(data[i][j], defaultFG, defaultBG));
            }

            foreach (var tok in toks)
            {
                if (tok.Position.y >= res.Count || tok.Position.y < 0)
                    continue;
                if (tok.Position.x < 0)
                    continue;
                for (int i = 0; i < tok.Text.Length; i++)
                {
                    if ((tok.Position.x + i) >= res[tok.Position.y].Count)
                        break;

                    var col = res[tok.Position.y][tok.Position.x + i];
                    col.ForegroundColor = tok.FG;
                    col.BackgroundColor = tok.BG;
                    res[tok.Position.y][tok.Position.x + i] = col;
                }
            }


            return res;
        }
    }
}
