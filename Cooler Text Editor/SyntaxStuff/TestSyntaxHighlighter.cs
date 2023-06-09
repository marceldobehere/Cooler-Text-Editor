using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Cooler_Text_Editor.SyntaxStuff
{
    public class TestSyntaxHighlighter : BasicSyntaxHighlighter
    {
        Dictionary<List<string>, PixelColor> tCols = new Dictionary<List<string>, PixelColor>()
            {
                {
                    new List<string>()
                    {
                        ".", ",", "(", ")", ";", "[", "]"
                    },
                    PixelColor.Gray2
                },
                {
                    new List<string>()
                    {
                        "+", "-", "*", "/", "=", "==", "!=", ">", "<", ">=", "<=", "%", "$", "|", "&", "!", "~", "^"
                    },
                    PixelColor.Yellow
                },
                {
                    new List<string>()
                    {
                        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", 
                    },
                    PixelColor.Cyan
                },
                {
                    new List<string>()
                    {
                        "public ","static ","abstract ","private ", "protected ", "virtual ", "class ", "namespace ", "struct ", "using ", "enum ", "internal "
                    },
                    PixelColor.Blue3
                },
                {
                    new List<string>()
                    {
                        "void ", "int ","double ","string ","double ", "bool ", "char ", "float ", "long ", "short ", "byte ", "uint ", "ulong ", "ushort ", "sbyte ", "this", "base", "true", "false", "null" 
                    },
                    PixelColor.Blue2
                },
                {
                    new List<string>()
                    {
                        "foreach ", "for ", "in ", "if ", "else", "break;", "continue;", "return;", "return ", "while ", "do ", "switch ", "case ", "default ", "try ", "catch ", "finally ", "throw ", "new ", "async "
                    },
                    PixelColor.Magenta2
                },

            };

        public override async Task<List<List<Pixel>>> SyntaxHighlight(List<string> data, PixelColor defaultFG, PixelColor defaultBG)
        {
            await Task.Delay(50);
            List<List<Pixel>> res = new List<List<Pixel>>();

            PixelColor tCol = defaultFG;



            for (int y = 0; y < data.Count; y++)
            {
                res.Add(new List<Pixel>());
                string str = data[y];
                for (int x = 0; x < data[y].Length; x++)
                {
                    string str2 = str.Substring(x);
                    string found = null;
                    PixelColor fCol = PixelColor.White;
                    foreach (var testo in tCols)
                    {
                        foreach (string test in testo.Key)
                        {
                            if (str2.StartsWith(test))
                            {
                                found = test;
                                fCol = testo.Value;
                                break;
                            }
                        }
                        if (found != null)
                            break;
                    }
                    if (str2.StartsWith("//"))
                    {
                        found = str2;
                        fCol = PixelColor.Green;
                    }
                    if (str2.StartsWith("#"))
                    {
                        found = str2;
                        fCol = PixelColor.Gray;
                    }
                    else if (str2.StartsWith("\""))
                    {
                        int tI = str2.Substring(1).IndexOf('\"');
                        if (tI == -1)
                            found = str2;
                        else
                            found = str2.Substring(0, tI + 2);

                        fCol = PixelColor.Orange;
                    }
                    else if (str2.StartsWith("\'"))
                    {
                        int tI = str2.Substring(1).IndexOf('\'');
                        if (tI == -1)
                            found = str2;
                        else
                            found = str2.Substring(0, tI + 2);

                        fCol = PixelColor.Orange;
                    }

                    if (found == null)
                    {
                        res[y].Add(new Pixel(data[y][x], defaultFG, defaultBG));
                    }
                    else
                    {
                        for (int i = 0; i < found.Length; i++)
                        {
                            res[y].Add(new Pixel(found[i], fCol, defaultBG));
                        }
                        x += found.Length - 1;
                    }
                }
            }

            return res;
        }
    }
}
