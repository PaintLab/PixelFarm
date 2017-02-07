//MIT, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace Mini
{
    static class Program
    {
        internal static PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform _winGdiPlatForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //----------------------------
            OpenTK.Toolkit.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RootDemoPath.Path = @"..\Data";
            _winGdiPlatForm = new PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform();
            Application.Run(new FormDev());
        }
    }
    public static class RootDemoPath
    {
        public static string Path = "";
    }
}
