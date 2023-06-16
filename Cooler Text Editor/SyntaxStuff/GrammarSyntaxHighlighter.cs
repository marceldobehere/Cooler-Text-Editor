using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        public StyleSet GetDefaultColorFromExtension(string ext)
        {
            LanguageRuleset set = null;
            foreach (var tempSet in Rulesets)
                if (tempSet.Extensions.Contains(ext))
                {
                    set = tempSet;
                    break;
                }

            if (set == null)
                return null;

            var style = set.Styles.Find((x) => x.DEF_TYPE == "DEFAULT");
            return style;
        }

        public class GrammarKeyword
        {
            public List<string> Words;
            public string Name;

            public GrammarKeyword(string name)
            {
                Words = new List<string>();
                Name = name;
            }
        }

        public class StyleSet
        {
            public string DEF_TYPE;
            public PixelColor FG;
            public PixelColor BG;
            public string KW_CLASS;

            public StyleSet(string name, PixelColor fg, PixelColor bg, string kwClass)
            {
                DEF_TYPE = name;
                FG = fg;
                BG = bg;
                KW_CLASS = kwClass;
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
            public List<StyleSet> Styles;


            public LanguageRuleset()
            {
                Extensions = new List<string>();
                Keywords = new List<GrammarKeyword>();
                Styles = new List<StyleSet>();
            }
        }

        public List<LanguageRuleset> Rulesets = new List<LanguageRuleset>();
        public void ReadRuleFile(string path)
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
                            lastKW = new GrammarKeyword(kwName);
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
                            Console.WriteLine($"ERROR: IDK \"{nam}\"");
                            Console.ReadLine();
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

        public void ReadStyleFile(string path)
        {
            if (!File.Exists(path))
                return;

            //Rulesets = new List<LanguageRuleset>();

            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    //Console.Clear();
                    bool ignore = false;
                    LanguageRuleset curr = null;
                    string lastNam = "";
                    //GrammarKeyword lastKW = null;
                    while (reader.Read())
                    {
                        if (reader.Name == "LexerType")
                        {
                            ignore = false;
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

                        if (nam == "LexerType")
                        {
                            // "LexerType": "" - name: actionscript, desc: ActionScript, ext: ,

                            if (tempAttr.FindIndex((x) => x.name == "name") != -1)
                            {
                                string langName = tempAttr.Find((x) => x.name == "name").val;
                                ;

                                if (Rulesets.FindIndex((x) => x.Name == langName) != 1)
                                {
                                    var ruleset = Rulesets.Find((x) => x.Name == langName);
                                    curr = ruleset;
                                }
                            }
                            ;
                        }
                        else if (nam == "WordsStyle")
                        {
                            //  - "WordsStyle": "" - name: OPCODE, styleID: 6, fgColor: 0000FF, bgColor: FFFFFF, fontName: , fontStyle: 1, fontSize: , keywordClass: instre1,
                            if (tempAttr.FindIndex((x) => x.name == "name") != -1)
                            {
                                string kwName = tempAttr.Find((x) => x.name == "name").val;
                                string fgStr = tempAttr.Find((x) => x.name == "fgColor").val;
                                string bgStr = tempAttr.Find((x) => x.name == "bgColor").val;
                                string kwClass = "";
                                if (tempAttr.FindIndex((x) => x.name == "keywordClass") != -1)
                                {
                                    kwClass = tempAttr.Find((x) => x.name == "keywordClass").val;
                                }



                                PixelColor fg = PixelColor.Transparent;
                                if (fgStr != null && fgStr.Length >= 6)
                                {
                                    int r = Convert.ToInt32(fgStr.Substring(0, 2), 16);
                                    int g = Convert.ToInt32(fgStr.Substring(2, 2), 16);
                                    int b = Convert.ToInt32(fgStr.Substring(4, 2), 16);

                                    fg = new PixelColor(r, g, b);
                                }

                                PixelColor bg = PixelColor.Transparent;
                                if (bgStr != null && bgStr.Length >= 6)
                                {
                                    int r = Convert.ToInt32(bgStr.Substring(0, 2), 16);
                                    int g = Convert.ToInt32(bgStr.Substring(2, 2), 16);
                                    int b = Convert.ToInt32(bgStr.Substring(4, 2), 16);

                                    bg = new PixelColor(r, g, b);

                                    //bg = PixelColor.Transparent;
                                }

                                if (kwName == "TAG")
                                    kwName = "KEYWORD";


                                if (kwName == "VALUE")
                                    kwName = "NUMBER";

                                var style = new StyleSet(kwName, fg, bg, kwClass);

                                if (curr != null)
                                    curr.Styles.Add(style);
                            }
                        }
                        else if (nam == "WidgetStyle")
                        {

                        }
                        else if (nam == "" || nam == "xml" || nam == "LexerStyles" || nam == "GlobalStyles" || nam == "NotepadPlus")
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

        public static (string typeName, string kwClass) GetTokenType(LanguageRuleset set, GrammarToken tok)
        {
            foreach (var kw in set.Keywords)
            {
                if (kw.Words.Contains(tok.Text))
                {
                    return ("KEYWORD", kw.Name);
                }
            }

            if (tok.Text.StartsWith("\"") || tok.Text.EndsWith("\""))
                return ("STRING", "<IDK>");

            if (tok.Text.StartsWith("\'") || tok.Text.EndsWith("\'"))
                return ("CHARACTER", "<IDK>");

            if (float.TryParse(tok.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                return ("NUMBER", "<IDK>");

            if (tok.Text.StartsWith("//"))
                return ("COMMENT", "<IDK>");

            if ("[]{}<>+-.,:;()?%|&~*/&=".Contains(tok.Text))
                return ("OPERATOR", "<IDK>");

            return ("DEFAULT", "<IDK>");
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
                char strChr = 'X';

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
                    if (chr == '"' && (!inString || strChr == chr))
                    {
                        inString = !inString;
                        if (inString)
                        {
                            addStrIfNeeded(x - 1);
                            strChr = chr;
                        }
                        tempStr += chr;
                        if (!inString)
                            addStrIfNeeded(x);
                    }
                    else if (chr == '\'' && (!inString || strChr == chr))
                    {
                        inString = !inString;
                        if (inString)
                        {
                            addStrIfNeeded(x - 1);
                            strChr = chr;
                        }
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
                var typeInfo = GetTokenType(set, tok);

                var styleInfoType = set.Styles.Find((x) => x.DEF_TYPE == typeInfo.typeName);
                var styleInfoClass = set.Styles.Find((x) => x.KW_CLASS == typeInfo.kwClass);

                if (styleInfoClass != null)
                {
                    tok.FG = styleInfoClass.FG;
                    tok.BG = styleInfoClass.BG;
                }
                else if (styleInfoType != null)
                {
                    tok.FG = styleInfoType.FG;
                    tok.BG = styleInfoType.BG;
                }

                tok.BG = defaultBG;
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
