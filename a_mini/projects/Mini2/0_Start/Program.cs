using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mini2
{
    public static class Program
    {
        public static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //--------
            PixelFarm.Drawing.WinGdi.WinGdiPortal.Start();
            //--------
            Application.Run(new FormDev());
        }
    }

}
