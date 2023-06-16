using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ConfigStuff
{
    public class ConfigManager
    {
        public static ConfigManager GlobalConfig;

        public string Filename;
        public string PrePath;
        public string StylePath;
        public string LangPath;

        public ConfigManager(string filename)
        {
            LoadFile(filename);
        }

        public void LoadFile(string filename)
        {
            Filename = filename;
            if (!File.Exists(filename))
                return;
            PrePath = Path.GetDirectoryName(filename) + @"\";

            using (StreamReader reader = new StreamReader(filename))
            {
                reader.ReadLine();
                LangPath = reader.ReadLine();
                reader.ReadLine();
                StylePath = reader.ReadLine();
            }
        }

        public void SaveFile(string filename)
        {
            Filename = filename;
            PrePath = Path.GetDirectoryName(filename) + @"\";

            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("[LANGS]");
                writer.WriteLine(LangPath);
                writer.WriteLine("[STYLE]");
                writer.WriteLine(StylePath);
            }
        }
    }
}
