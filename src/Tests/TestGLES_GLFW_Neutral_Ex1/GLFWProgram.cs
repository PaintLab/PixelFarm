//MIT, 2016-present, WinterDev
using System;

using Pencil.Gaming;
using PixelFarm;
using PixelFarm.Forms;
using OpenTkEssTest;
using Typography.FontManagement;
using Mini;
namespace TestGlfw
{
    //Your implementation.
    //Application specific

    abstract class GlfwAppBase
    {
        public abstract void UpdateViewContent(RenderUpdateEventArgs formRenderUpdateEventArgs);
    }


    class MyApp : GlfwAppBase
    {

        static GLDemoContext _demoContext = null;
        static InstalledTypefaceCollection s_typefaceStore;
        static LayoutFarm.OpenFontTextService s_textServices;
        public MyApp()
        {
            s_typefaceStore = new InstalledTypefaceCollection();
            s_textServices = new LayoutFarm.OpenFontTextService();

        }
        public override void UpdateViewContent(RenderUpdateEventArgs formRenderUpdateEventArgs)
        {
            //1. create platform bitmap 
            // create the surface
            int w = 800;
            int h = 600;

            if (_demoContext == null)
            {

                //var demo = new T44_SimpleVertexShader(); 
                //var demo = new T42_ES2HelloTriangleDemo();
                _demoContext = new GLDemoContext(w, h);
                _demoContext.SetTextPrinter(painter =>
                {

                    var printer = new PixelFarm.DrawingGL.GLBitmapGlyphTextPrinter(painter, s_textServices);
                    painter.TextPrinter = printer;
                    //create text printer for opengl 
                    //----------------------
                    //1. win gdi based
                    //var printer = new WinGdiFontPrinter(canvas2d, w, h);
                    //canvasPainter.TextPrinter = printer;
                    //----------------------
                    //2. raw vxs
                    //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
                    //canvasPainter.TextPrinter = printer;
                    //----------------------
                    //3. agg texture based font texture
                    //var printer = new AggFontPrinter(canvasPainter, w, h);
                    //canvasPainter.TextPrinter = printer;
                    //----------------------
                    //4. texture atlas based font texture 
                    //------------
                    //resolve request font 
                    //var printer = new GLBmpGlyphTextPrinter(canvasPainter, YourImplementation.BootStrapWinGdi.myFontLoader);
                    //canvasPainter.TextPrinter = printer;

                });
                //demoContext2.LoadDemo(new T45_TextureWrap());
                //demoContext2.LoadDemo(new T48_MultiTexture());
                //demoContext2.LoadDemo(new T107_1_DrawImages());
                //demoContext2.LoadDemo(new T110_DrawText());
                _demoContext.LoadDemo(new T108_LionFill());
            }
            _demoContext.Render();
        }
    }



    class GLFWProgram
    {

        class LocalFileStorageProvider : PixelFarm.Platforms.StorageServiceProvider
        {
            public override bool DataExists(string dataName)
            {
                //implement with file
                return System.IO.File.Exists(dataName);
            }
            public override byte[] ReadData(string dataName)
            {
                return System.IO.File.ReadAllBytes(dataName);
            }
            public override void SaveData(string dataName, byte[] content)
            {
                System.IO.File.WriteAllBytes(dataName, content);
            }

        }


        static LocalFileStorageProvider s_LocalStorageProvider = new LocalFileStorageProvider();
        public static void Start()
        {

            PixelFarm.Platforms.StorageService.RegisterProvider(s_LocalStorageProvider);
            //---------------------------------------------------
            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new ImgCodecMemBitmapIO();
            //PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            if (!GLFWPlatforms.Init())
            {
                System.Diagnostics.Debug.WriteLine("can't init glfw");
                return;
            }

            bool useMyGLFWForm = true;
            if (!useMyGLFWForm)
            {
                GlFwForm form1 = new GlFwForm(800, 600, "PixelFarm on GLfw and GLES2"); 
                MyApp glfwApp = new MyApp();
                form1.SetDrawFrameDelegate(e => glfwApp.UpdateViewContent(e));
            }
            else
            {
                MyGLFWForm form1 = new MyGLFWForm(800, 600, "PixelFarm on GLfw and GLES2");
                MyApp glfwApp = new MyApp();
                form1.SetDrawFrameDelegate(e => glfwApp.UpdateViewContent(e));
            }
            GlfwApp.RunMainLoop();
        }
    }



}