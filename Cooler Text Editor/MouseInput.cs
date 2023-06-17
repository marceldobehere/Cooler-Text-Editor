using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor
{
    public class MouseInput
    {
        // Taken from https://github.com/gui-cs/Terminal.Gui


        /// <summary>
        /// Represents the CSI (Control Sequence Introducer).
        /// </summary>
        public static readonly string KeyCSI = $"\u001b[";
        /// <summary>
        /// Represents the CSI for enable any mouse event tracking.
        /// </summary>
        public static readonly string CSI_EnableAnyEventMouse = KeyCSI + "?1003h";
        /// <summary>
        /// Represents the CSI for enable SGR (Select Graphic Rendition).
        /// </summary>
        public static readonly string CSI_EnableSgrExtModeMouse = KeyCSI + "?1006h";
        /// <summary>
        /// Represents the CSI for enable URXVT (Unicode Extended Virtual Terminal).
        /// </summary>
        public static readonly string CSI_EnableUrxvtExtModeMouse = KeyCSI + "?1015h";
        /// <summary>
        /// Represents the CSI for disable any mouse event tracking.
        /// </summary>
        public static readonly string CSI_DisableAnyEventMouse = KeyCSI + "?1003l";
        /// <summary>
        /// Represents the CSI for disable SGR (Select Graphic Rendition).
        /// </summary>
        public static readonly string CSI_DisableSgrExtModeMouse = KeyCSI + "?1006l";
        /// <summary>
        /// Represents the CSI for disable URXVT (Unicode Extended Virtual Terminal).
        /// </summary>
        public static readonly string CSI_DisableUrxvtExtModeMouse = KeyCSI + "?1015l";

        /// <summary>
        /// Control sequence for enable mouse events.
        /// </summary>
        public static string EnableMouseEvents { get; set; } =
            CSI_EnableAnyEventMouse + CSI_EnableUrxvtExtModeMouse + CSI_EnableSgrExtModeMouse;
        /// <summary>
        /// Control sequence for disable mouse events.
        /// </summary>
        public static string DisableMouseEvents { get; set; } =
            CSI_DisableAnyEventMouse + CSI_DisableUrxvtExtModeMouse + CSI_DisableSgrExtModeMouse;

        public static void Init()
        {
            Console.Out.Write(EnableMouseEvents);
        }
    }
}
