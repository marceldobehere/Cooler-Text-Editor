using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.SyntaxStuff
{
    public abstract class BasicSyntaxHighlighter
    {
        public abstract Task<List<List<Pixel>>> SyntaxHighlight(List<string> data, PixelColor defaultFG, PixelColor defaultBG);
    }
}
