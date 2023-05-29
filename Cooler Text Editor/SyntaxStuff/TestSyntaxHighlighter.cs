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
                        ".", ",", "(", ")", ";"
                    },
                    PixelColor.Gray
                },
                {
                    new List<string>()
                    {
                        "+", "-", "*", "/", "=", "==", "!=", ">", "<", ">=", "<=", "%"
                    },
                    PixelColor.Yellow
                },
                {
                    new List<string>()
                    {
                        "public","static","abstract","private", "protected", "virtual", "class", "namespace", "struct", "using"
                    },
                    PixelColor.Cyan
                },
                {
                    new List<string>()
                    {
                        "void", "int","double","string","double", "bool", "char", "float", "long", "short", "byte", "uint", "ulong", "ushort", "sbyte", "this", "base", "true", "false", "null"
                    },
                    PixelColor.Blue2
                },
                {
                    new List<string>()
                    {
                        "foreach", "for", "if", "else", "break", "continue", "return", "while", "do", "switch", "case", "default", "try", "catch", "finally", "throw", "new"
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
                            if (str.Substring(x).StartsWith(test))
                            {
                                found = test;
                                fCol = testo.Value;
                                break;
                            }
                        }
                        if (found != null)
                            break;
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
