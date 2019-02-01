//MIT, 2016-present, WinterDev
using System;
//using SkiaSharp;

using Pencil.Gaming;
using PixelFarm;
using PixelFarm.Forms;
using OpenTK.Graphics.ES20;
using OpenTkEssTest;

using Typography.FontManagement;
using PixelFarm.CpuBlit;
using System.IO;

namespace TestGlfw
{

    //-------------------------------------------------------------------------
    //WITHOUT WinForms.
    //This demonstrate how to draw with 1) Skia  or 2) Glfw
    //-------------------------------------------------------------------------
    public enum BackEnd
    {
        GLES2,
        SKIA
    }

    abstract class GlfwAppBase
    {
        public abstract void UpdateViewContent(FormRenderUpdateEventArgs formRenderUpdateEventArgs);
    }
    
    class GlfwGLES2 : GlfwAppBase
    {

        static Mini.GLDemoContext demoContext2 = null;
        static InstalledTypefaceCollection s_typefaceStore;
        static LayoutFarm.OpenFontTextService s_textServices;
        public GlfwGLES2()
        {
            s_typefaceStore = new InstalledTypefaceCollection();
            s_textServices = new LayoutFarm.OpenFontTextService();

        }
        public override void UpdateViewContent(FormRenderUpdateEventArgs formRenderUpdateEventArgs)
        {
            //1. create platform bitmap 
            // create the surface
            int w = 800;
            int h = 600;

            if (demoContext2 == null)
            {

                //var demo = new T44_SimpleVertexShader(); 
                //var demo = new T42_ES2HelloTriangleDemo();
                demoContext2 = new Mini.GLDemoContext(w, h);
                demoContext2.SetTextPrinter(painter =>
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
                //demoContext2.LoadDemo(new T107_SampleDrawImage());

                demoContext2.LoadDemo(new T110_DrawText());
            }
            demoContext2.Render();

        }
    }

    class CustomMemBitmapIO : PixelFarm.CpuBlit.MemBitmapIO
    {
        public override MemBitmap LoadImage(string filename)
        {
            OutputImageFormat format;
            //TODO: review img loading, we should not use only its extension 
            string fileExt = System.IO.Path.GetExtension(filename).ToLower();
            switch (fileExt)
            {
                case ".png":
                    {
                        format = OutputImageFormat.Png;
                    }
                    break;
                case ".jpg":
                    {
                        format = OutputImageFormat.Jpeg;
                    }
                    break;
                default:
                    throw new System.NotSupportedException();
            }
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return LoadImage(fs, format);
            }
        }

        public override MemBitmap LoadImage(Stream input)
        {
            throw new NotImplementedException();
        }
        public MemBitmap LoadImage(Stream input, OutputImageFormat format)
        {
            ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
            //TODO: review img loading, we should not use only its extension
            switch (format)
            {
                case OutputImageFormat.Png:
                    {
                        var decoder = new ImageTools.IO.Png.PngDecoder();
                        extendedImg.Load(input, decoder);
                    }
                    break;
                case OutputImageFormat.Jpeg:
                    {
                        var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
                        extendedImg.Load(input, decoder);
                    }
                    break;
                default:
                    throw new System.NotSupportedException();

            }
            //var decoder = new ImageTools.IO.Png.PngDecoder();


            //assume 32 bit 

            PixelFarm.CpuBlit.MemBitmap memBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
                extendedImg.PixelWidth,
                extendedImg.PixelHeight,
                extendedImg.Pixels32
                );
            //the imgtools load data as BigEndian
            memBmp.IsBigEndian = true;
            return memBmp;
        }

        public override void SaveImage(MemBitmap bitmap, Stream output, OutputImageFormat outputFormat, object saveParameters)
        {
            throw new NotImplementedException();
        }

        public override void SaveImage(MemBitmap bitmap, string filename, OutputImageFormat outputFormat, object saveParameters)
        {
            throw new NotImplementedException();
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


        static LocalFileStorageProvider file_storageProvider = new LocalFileStorageProvider();
        public static void Start()
        {
            //---------------------------------------------------
            //register image loader

            PixelFarm.Platforms.StorageService.RegisterProvider(file_storageProvider);
            //---------------------------------------------------
            if (!Glfw.Init())
            {
                Console.WriteLine("can't init glfw");
                return;
            }
            //---------------------------------------------------
            //specific OpenGLES ***
            Glfw.WindowHint(WindowHint.GLFW_CLIENT_API, (int)OpenGLAPI.OpenGLESAPI);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_CREATION_API, (int)OpenGLContextCreationAPI.GLFW_EGL_CONTEXT_API);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MINOR, 0);
            //---------------------------------------------------


            Glfw.SwapInterval(1);
            GlFwForm form1 = GlfwApp.CreateGlfwForm(
                800,
                600,
                "PixelFarm + Skia on GLfw and OpenGLES2");
            form1.MakeCurrent();
            //------------------------------------
            //***
            GLFWPlatforms.CreateGLESContext();
            //------------------------------------
            form1.Activate();

            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);


            //------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //--------------------------------------------------------------------------------
            //setup viewport size
            //set up canvas  
            GL.Viewport(0, 0, max, max);

            FormRenderUpdateEventArgs formRenderUpdateEventArgs = new FormRenderUpdateEventArgs();
            formRenderUpdateEventArgs.form = form1;

            GlfwGLES2 glfwApp = new GlfwGLES2();

            form1.SetDrawFrameDelegate(() =>
            {
                glfwApp.UpdateViewContent(formRenderUpdateEventArgs);

            });



            while (!GlfwApp.ShouldClose())
            {
                //---------------
                //render phase and swap
                GlfwApp.UpdateWindowsFrame();
                /* Poll for and process events */
                Glfw.PollEvents();
            }

            Glfw.Terminate();
        }
    }


    class FormRenderUpdateEventArgs : EventArgs
    {
        public GlFwForm form;
    }
}