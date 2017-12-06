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

#if GL_ENABLE
            OpenTK.Toolkit.Init();
#endif
            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //test Typography's custom text break, 
            Typography.TextBreak.CustomBreakerBuilder.Setup(@"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries");
            //default text breaker, this bridge between 
            LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker(); 

            //you can use your font loader
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);

#if GL_ENABLE
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);
#endif
            ////------------------------------- 
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program));

            Application.Run(formDemoList);
        }


    }


}
