//MIT, 2020,WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mini;
namespace BitmapAtlasTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormBitmapAtlasBuilder());
        }
    }
}
