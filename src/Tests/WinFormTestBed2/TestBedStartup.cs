//Apache2, 2014-present, WinterDev
//#define GL_ENABLE
using System;
using System.Windows.Forms;
using LayoutFarm.UI;
namespace YourImplementation
{
    public static class TestBedStartup
    {
        public static void Setup()
        {

            CommonTextServiceSetup.SetupDefaultValues();
#if GL_ENABLE
            FrameworkInitGLES.SetupDefaultValues();
#endif
            //you can use your font loader 
            PixelFarm.CpuBlit.Imaging.PngImageWriter.InstallImageSaveToFileService((IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            {

                using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    PixelFarm.CpuBlit.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
                    //save
                    newBmp.Save(filename);
                }
            });

            //you can use your font loader
            YourImplementation.FrameworkInitWinGDI.SetupDefaultValues();
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

#if DEBUG
        public static bool dbugShowLayoutInspectorForm { get; set; }
#endif

        public static void RunSpecificDemo(LayoutFarm.App demo, InnerViewportKind innerViewportKind = InnerViewportKind.GdiPlusOnGLES)
        {
            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ////------------------------------- 
            //1. select view port kind

            
            System.Drawing.Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            Form formCanvas = FormCanvasHelper.CreateNewFormCanvas(
               workingArea.Width,
               workingArea.Height,
               innerViewportKind,
               out UISurfaceViewportControl latestviewport);
#if DEBUG
            formCanvas.Text = innerViewportKind.ToString();
#endif

            demo.Start(new LayoutFarm.AppHostWinForm(latestviewport));
            latestviewport.TopDownRecalculateContent();
            //==================================================  
            latestviewport.PaintMe();

            //formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
#if DEBUG
            if (dbugShowLayoutInspectorForm)
            {
                LayoutInspectorUtils.ShowFormLayoutInspector(latestviewport);

            }
#endif


            //got specfic example
            Application.Run(formCanvas);
        }
    }

    public static class LayoutInspectorUtils
    {

        public static void ShowFormLayoutInspector(LayoutFarm.UI.UISurfaceViewportControl viewport)
        {
            var formLayoutInspector = new LayoutFarm.Dev.FormLayoutInspector();
            formLayoutInspector.Show();

            formLayoutInspector.Connect(viewport);
            formLayoutInspector.Show();
        }
    }

    public static class DemoFormCreatorHelper
    {
        public static void CreateReadyForm(
         InnerViewportKind innerViewportKind,
         out LayoutFarm.UI.UISurfaceViewportControl viewport,
         out Form formCanvas)
        {

            //1. select view port kind  

            var workingArea = Screen.PrimaryScreen.WorkingArea;

            formCanvas = FormCanvasHelper.CreateNewFormCanvas(
              workingArea.Width,
              workingArea.Height,
              innerViewportKind,
              out viewport);

            formCanvas.Text = "FormCanvas 1 :" + innerViewportKind;

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        }
    }
}