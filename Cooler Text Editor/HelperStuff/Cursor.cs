﻿using Cooler_Text_Editor.ComponentStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.HelperStuff
{
    public class Cursor
    {
        public enum CursorModeEnum
        {
            BLINKING_BLOCK = 1,
            STEADY_BLOCK = 2,
            BLINKING_UNDERLINE = 3,
            STEADY_UNDERLINE = 4,
            BLINKING_VERTICAL_LINE = 5,
            STEADY_VERTICAL_LINE = 6
        }

        public static Cursor MainCursor;

        public BasicComponent CursorComponent;
        public BasicComponent HoverComponent;
        public Position2D CursorPosition;
        public CursorModeEnum CursorMode;
        public bool CursorShown;

        public Cursor(BasicComponent mainComponent)
        {
            CursorComponent = mainComponent;
            HoverComponent = null;
            CursorPosition = new Position2D();
            CursorMode = CursorModeEnum.STEADY_VERTICAL_LINE;
            CursorShown = true;
        }

        public void OverwriteCursor(Cursor other)
        {
            //CursorComponent = other.CursorComponent;
            //HoverComponent = other.HoverComponent;
            CursorPosition = other.CursorPosition;
            CursorMode = other.CursorMode;
            CursorShown = other.CursorShown;
        }
    }
}
