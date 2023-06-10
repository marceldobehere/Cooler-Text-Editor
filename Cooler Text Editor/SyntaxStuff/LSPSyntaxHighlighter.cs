using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Cooler_Text_Editor.SyntaxStuff
{
    public class LSPSyntaxHighlighter : BasicSyntaxHighlighter
    {
        
        public override async Task<List<List<Pixel>>> SyntaxHighlight(List<string> data, string ext, PixelColor defaultFG, PixelColor defaultBG)
        {
            await Task.Delay(50);

            var opt = new LanguageClientOptions();
            
            LanguageClient client = LanguageClient.Create(opt);



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
        }
    }
}
