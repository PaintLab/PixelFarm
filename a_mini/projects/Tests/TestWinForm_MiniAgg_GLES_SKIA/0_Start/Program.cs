//MIT, 2014-2017, WinterDev

using System;
using System.Windows.Forms;
namespace Mini
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
            Application.SetCompatibleTextRenderingDefault(false);
            RootDemoPath.Path = @"..\Data";
            //you can use your font loader
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);

            Application.Run(new FormDev());
        }
    }
}
