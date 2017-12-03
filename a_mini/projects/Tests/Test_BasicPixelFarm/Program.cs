//Apache2, 2014-2017, WinterDev

using System;
using System.Windows.Forms;
namespace TestGraphicPackage2
{
    static class Program
    {
        static LayoutFarm.Dev.FormDemoList formDemoList;
        [STAThread]
        static void Main()
        {

            OpenTK.Toolkit.Init();
            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //temp
            //TODO: fix this , 
            //set data dir before load
            Typography.TextBreak.CustomBreakerBuilder.DataDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries";

            LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
            //RootDemoPath.Path = @"..\Data";
            //you can use your font loader
          
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);
            ////------------------------------- 
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program));

            Application.Run(formDemoList);
        }


    }


}
