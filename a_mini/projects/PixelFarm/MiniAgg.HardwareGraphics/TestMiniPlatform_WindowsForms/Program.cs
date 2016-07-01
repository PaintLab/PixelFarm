using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PixelFarm;

namespace TestMiniPlatform_WindowsForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //------------------------------------
            if (!GLPlatforms.Init())
            {
                Console.WriteLine("can't init");
            }
            //------------------------------------

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
