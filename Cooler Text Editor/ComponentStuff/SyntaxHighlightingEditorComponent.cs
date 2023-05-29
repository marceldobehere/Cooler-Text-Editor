using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff
{
    public class SyntaxHighlightingEditorComponent : BasicComponent
    {
        public EditorComponent MainEditorComponent, ShadowEditorComponent;

        public SyntaxHighlightingEditorComponent()
        {

        }


        public override void HandleKey(ConsoleKeyInfo info)
        {
            throw new NotImplementedException();
        }

        protected override void InternalUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
