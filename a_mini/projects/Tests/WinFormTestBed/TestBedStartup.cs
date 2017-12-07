using System;
using System.Windows.Forms;
namespace YourImplementation
{
    public static class TestBedStartup
    {
        public static void Setup()
        {
#if GL_ENABLE
            OpenTK.Toolkit.Init();
#endif
            //you can use your font loader
            YourImplementation.BootStrapWinGdi.SetupDefaultValues();

#if GL_ENABLE
            YourImplementation.BootStrapOpenGLES2.SetupDefaultValues();
#endif
            //default text breaker, this bridge between 
            LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
        }
        public static void RunDemoList(Type mainType)
        {
            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ////------------------------------- 
            var formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(mainType);
            Application.Run(formDemoList);
        }
    }
}