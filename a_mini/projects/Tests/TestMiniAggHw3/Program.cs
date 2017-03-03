using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mini;
using Pencil.Gaming;

namespace OpenTkEssTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            OpenTK.Toolkit.Init();
            DemoHelper.RegisterFontProvider(new PixelFarm.Drawing.InstallFontsProviderWin32());
            Application.EnableVisualStyles();
            //----------------------------

            RootDemoPath.Path = @"..\Data";

            var formDev = new FormDev();
            Application.Run(formDev);
        }
    }
}
