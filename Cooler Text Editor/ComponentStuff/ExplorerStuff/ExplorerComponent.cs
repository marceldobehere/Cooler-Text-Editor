using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff.ExplorerStuff
{
    public class ExplorerComponent : BasicComponent
    {
        public string CurrentPath = null;
        public ExplorerComponent(Size2D size)
        {
            Size = size;
            CurrentPath = null;
        }

        public List<string> GetCurrentFolders()
        {
            List<string> res = new List<string>();

            return res;
        }

        public List<string> GetCurrentFiles()
        {
            List<string> res = new List<string>();

            return res;
        }

        public List<string> GetCurrentDrives()
        {
            List<string> res = new List<string>();

            return res;
        }




        public override void HandleKey(ConsoleKeyInfo info)
        {
            //throw new NotImplementedException();
        }

        protected override void InternalUpdate()
        {
            //throw new NotImplementedException();
        }
    }
}
