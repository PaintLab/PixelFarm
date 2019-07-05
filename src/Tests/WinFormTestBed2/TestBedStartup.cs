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

            PixelFarm.DrawingGL.CachedBinaryShaderIO.SetActualImpl(
               () => new PixelFarm.DrawingGL.LocalFileCachedBinaryShaderIO(Application.CommonAppDataPath));

            FrameworkInitGLES.SetupDefaultValues();


            PixelFarm.CpuBlit.Imaging.PngImageWriter.InstallImageSaveToFileService((IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            {

                using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
                    //save
                    newBmp.Save(filename);
                }
            });

            //you can use your font loader
            YourImplementation.FrameworkInitWinGDI.SetupDefaultValues();
            //default text breaker, this bridge between 
            LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
        }
        public static void Close()
        {
            LayoutFarm.UIPlatform.Close();
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
        /// <summary>
        /// close our application system
        /// </summary>

#if DEBUG
        public static bool dbugShowLayoutInspectorForm { get; set; }
#endif

        public static Form RunSpecificDemo(LayoutFarm.App demo, LayoutFarm.AppHostWinForm appHost, InnerViewportKind innerViewportKind = InnerViewportKind.GdiPlusOnGLES)
        {
            System.Drawing.Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            Form formCanvas = FormCanvasHelper.CreateNewFormCanvas(
               workingArea.Width,
               workingArea.Height,
               innerViewportKind,
               out UISurfaceViewportControl latestviewport);
#if DEBUG
            formCanvas.Text = innerViewportKind.ToString();
#endif
            

            formCanvas.FormClosed += (s, e) =>
            {
                demo.OnClosing();
                demo.OnClosed();
            };

            appHost.SetUISurfaceViewportControl(latestviewport);
            appHost.StartApp(demo);
            //
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

            //Application.Run(formCanvas);

            formCanvas.Show();
            return formCanvas;

        }

        public struct DemoAppInitInfo
        {
            public LayoutFarm.App App;
            public InnerViewportKind InnerViewportKind;
            public PixelFarm.Drawing.Rectangle Area;
        }

        public static Form RunSpecificDemo(DemoAppInitInfo[] demoInitArr)
        {

            System.Drawing.Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;


            //1st form

            DemoAppInitInfo appInitInfo = demoInitArr[0];

            //
            Form formCanvas = FormCanvasHelper.CreateNewFormCanvas(
                appInitInfo.Area.Left,
                appInitInfo.Area.Top,
                appInitInfo.Area.Width,
                appInitInfo.Area.Height,
                appInitInfo.InnerViewportKind,
            out UISurfaceViewportControl latestviewport);



            latestviewport.PaintMe();
            {
                LayoutFarm.App demo = appInitInfo.App;

                var appHost = new LayoutFarm.AppHostWinForm();
                appHost.SetUISurfaceViewportControl(latestviewport);
                appHost.StartApp(demo);

                latestviewport.TopDownRecalculateContent();

                formCanvas.FormClosed += (s, e) =>
                {
                    demo.OnClosing();
                    demo.OnClosed();
                };

            }
            //==================================================  

            for (int i = 1; i < demoInitArr.Length; ++i)
            {

                appInitInfo = demoInitArr[i];
                LayoutFarm.App demo = appInitInfo.App;

                FormCanvasHelper.CreateCanvasControlOnExistingControl(
                    formCanvas,
                    appInitInfo.Area.Left,
                    appInitInfo.Area.Top,
                    appInitInfo.Area.Width,
                    appInitInfo.Area.Height,
                    appInitInfo.InnerViewportKind,
                    out latestviewport);

                formCanvas.FormClosed += (s, e) =>
                {
                    demo.OnClosing();
                    demo.OnClosed();
                };

                latestviewport.PaintMe();
                var appHost = new LayoutFarm.AppHostWinForm();
                appHost.SetUISurfaceViewportControl(latestviewport);
                appHost.StartApp(demo);

                latestviewport.TopDownRecalculateContent();
            }

            formCanvas.Show();
            return formCanvas;
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