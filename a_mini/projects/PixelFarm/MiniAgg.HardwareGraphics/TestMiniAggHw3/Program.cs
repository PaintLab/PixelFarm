using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mini;
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
            //----------------------------
            OpenTK.Toolkit.Init();
            Application.EnableVisualStyles();
            var formDev = new FormDev();
            Application.Run(formDev); 
        }
    }
}
