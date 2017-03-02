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
            Application.EnableVisualStyles();
            //----------------------------
            OpenTK.Toolkit.Init();
            RootDemoPath.Path = @"..\Data";

            var formDev = new FormDev2();
            Application.Run(formDev);
        }
    }
}
