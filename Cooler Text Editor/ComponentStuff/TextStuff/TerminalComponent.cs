﻿using Cooler_Text_Editor.HelperStuff;
using Cooler_Text_Editor.RenderingStuff;
using Cooler_Text_Editor.WindowStuff;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Cooler_Text_Editor.ComponentStuff.TextStuff
{
    public class TerminalComponent : BasicComponent
    {
        public PixelColor ForegroundColor, BackgroundColor;
        public TextComponent InternalTextComponent;
        public Process PowershellProc;

        public TerminalComponent(Size2D size)
        {
            Size = size;
            Visible = true;
            OldVisible = Visible;
            Position = new Position2D();

            UpdateFields = new HashSet<Field2D>();
            ComponentCursor = new Cursor(this);

            ForegroundColor = PixelColor.White;
            BackgroundColor = PixelColor.Black;

            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

            RenderedScreen = new Pixel[Size.Width, Size.Height];
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                    RenderedScreen[x, y] = bgPixel;



            InternalTextComponent = new TextComponent();
            InternalTextComponent.Size = new Size2D((parent) => { return parent; });
            InternalTextComponent.Position = new Position2D();
            InternalTextComponent.Parent = this;
            InternalTextComponent.BackgroundColor = BackgroundColor;
            InternalTextComponent.DefaultBackgroundColor = BackgroundColor;
            InternalTextComponent.DefaultForegroundColor = ForegroundColor;
            InternalTextComponent.updateInternal = true;
            InternalTextComponent.AutoYScroll = true;


            RestartPowershellProc();
        }

        public void RestartPowershellProc()
        {
            PowershellProc = new Process();
            PowershellProc.StartInfo = new ProcessStartInfo("powershell.exe");
            PowershellProc.StartInfo.RedirectStandardOutput = true;
            PowershellProc.StartInfo.RedirectStandardInput = true;
            PowershellProc.StartInfo.UseShellExecute = false;
            PowershellProc.StartInfo.CreateNoWindow = true;
            PowershellProc.StartInfo.Arguments = "-NoExit";
            PowershellProc.StartInfo.RedirectStandardError = true;
            PowershellProc.Start();

            AttachClearHostAlias();
        }

        public void AttachClearHostAlias()
        {
            PowershellProc.StandardInput.WriteLine(@"
                function Invoke-CustomClearHost {" +
                    $"Write-Host '{(char)0x0C}'"
                 + @"}

                Set-Alias -Name Clear-Host -Value Invoke-CustomClearHost -Option AllScope");

            PowershellProc.StandardInput.WriteLine("cls");
        }

        public override bool InternalHandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Enter)
            {
                //InternalTextComponent.WriteLineText();
                PowershellProc.StandardInput.WriteLine();
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                PowershellProc.StandardInput.Write(info.KeyChar);
                //InternalTextComponent.WriteText(info.KeyChar.ToString(), ForegroundColor, BackgroundColor);
            }
            else if (info.Key == ConsoleKey.Escape && info.Modifiers == ConsoleModifiers.Shift)
            {
                InternalTextComponent.Clear();
            }
            else
            {
                //InternalTextComponent.WriteText(info.KeyChar.ToString(), ForegroundColor, BackgroundColor);
                PowershellProc.StandardInput.Write(info.KeyChar);
            }

            return true;
        }

        public Task<int> ReadTask = null;
        public const int CharCacheSize = 500;
        public char[] CharCache = new char[CharCacheSize];

        protected override void InternalUpdate()
        {
            if (PowershellProc == null || PowershellProc.HasExited)
            {
                RestartPowershellProc();
            }


            if (ReadTask == null)
            {
                ReadTask = PowershellProc.StandardOutput.ReadAsync(CharCache, 0, CharCacheSize);
            }
            else if (ReadTask.IsCanceled || ReadTask.IsFaulted)
            {
                ReadTask = null;
            }
            else if (ReadTask.IsCompletedSuccessfully)
            {
                for (int i2 = 0; i2 < ReadTask.Result; i2++)
                {
                    char res = CharCache[i2];
                    InternalTextComponent.WriteText(res.ToString(), ForegroundColor, BackgroundColor);
                }
                ReadTask = null;
            }

            InternalTextComponent.Update();
            Pixel bgPixel = new Pixel(BackgroundColor, BackgroundColor);

            foreach (Field2D tempField in UpdateFields)
            {
                Rendering.FillPixel(RenderedScreen, tempField, bgPixel);

                InternalTextComponent.RenderTo(RenderedScreen, tempField);

                if (Parent != null)
                    Parent.UpdateFields.Add(tempField + Position);
            }

            UpdateFields.Clear();
            ComponentCursor.CursorPosition.Y = InternalTextComponent.Text.Count - InternalTextComponent.Scroll.Y - 1;
            if (InternalTextComponent.Text.Count > 0)
            {
                ComponentCursor.CursorPosition.X = InternalTextComponent.Text.Last().Count - InternalTextComponent.Scroll.X;
            }
            else
                ComponentCursor.CursorPosition.X = 0;
        }
    }
}
