//MIT, 2016-2017, WinterDev
using System;
using SkiaSharp;

using Pencil.Gaming;
using PixelFarm;
using PixelFarm.Forms;
using OpenTK.Graphics.ES20;
using OpenTkEssTest;

using Typography.TextServices;

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
    class GlfwSkia : GlfwAppBase
    {
        static PixelFarm.DrawingGL.GLRenderSurface _glsf;
        static MyNativeRGBA32BitsImage myImg;
        public GlfwSkia()
        {
            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);
            _glsf = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, ww_w, ww_h);

        }
        public override void UpdateViewContent(FormRenderUpdateEventArgs formRenderUpdateEventArgs)
        {
            //1. create platform bitmap 
            // create the surface
            int w = 800;
            int h = 600;

            if (myImg == null)
            {

                myImg = new TestGlfw.MyNativeRGBA32BitsImage(w, h);
                //test1
                // create the surface
                var info = new SKImageInfo(w, h, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using (var surface = SKSurface.Create(info, myImg.Scan0, myImg.Stride))
                {
                    // start drawing
                    SKCanvas canvas = surface.Canvas;
                    DrawWithSkia(canvas);
                    surface.Canvas.Flush();
                }
            }

            var glBmp = new PixelFarm.DrawingGL.GLBitmap(w, h, myImg.Scan0);
            _glsf.DrawImage(glBmp, 0, 600);
            glBmp.Dispose();
        }
        static void DrawWithSkia(SKCanvas canvas)
        {
            canvas.Clear(new SKColor(255, 255, 255, 255));
            using (SKPaint p = new SKPaint())
            {
                p.TextSize = 36.0f;
                p.Color = (SKColor)0xFF4281A4;
                p.StrokeWidth = 2;
                p.IsAntialias = true;
                canvas.DrawLine(0, 0, 100, 100, p);
                p.Color = SKColors.Red;
                canvas.DrawText("Hello!", 20, 100, p);
            }
        }

        static PixelFarm.Agg.ActualImage LoadImage(string filename)
        {
            ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
            using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //TODO: review img loading, we should not use only its extension
                //
                string fileExt = System.IO.Path.GetExtension(filename).ToLower();
                switch (fileExt)
                {
                    case ".png":
                        {
                            var decoder = new ImageTools.IO.Png.PngDecoder();
                            extendedImg.Load(fs, decoder);
                        }
                        break;
                    case ".jpg":
                        {
                            var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
                            extendedImg.Load(fs, decoder);
                        }
                        break;
                    default:
                        throw new System.NotSupportedException();

                }
                //var decoder = new ImageTools.IO.Png.PngDecoder();

            }
            //assume 32 bit 

            PixelFarm.Agg.ActualImage actualImg = PixelFarm.Agg.ActualImage.CreateFromBuffer(
                extendedImg.PixelWidth,
                extendedImg.PixelHeight,
                PixelFarm.Agg.PixelFormat.ARGB32,
                extendedImg.Pixels
                );
            //the imgtools load data as BigEndian
            actualImg.IsBigEndian = true;
            return actualImg;
        }
    }

    class GlfwGLES2 : GlfwAppBase
    {

        static Mini.GLDemoContext demoContext2 = null;
        static OpenFontStore s_fontstore;

        public GlfwGLES2()
        {
            s_fontstore = new OpenFontStore();
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


                    var printer = new PixelFarm.DrawingGL.GLBmpGlyphTextPrinter(
                        painter,
                        s_fontstore);
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

    class GLFWProgram
    {
        static PixelFarm.Agg.ActualImage LoadImage(string filename)
        {
            ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
            using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //TODO: review img loading, we should not use only its extension
                //
                string fileExt = System.IO.Path.GetExtension(filename).ToLower();
                switch (fileExt)
                {
                    case ".png":
                        {
                            var decoder = new ImageTools.IO.Png.PngDecoder();
                            extendedImg.Load(fs, decoder);
                        }
                        break;
                    case ".jpg":
                        {
                            var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
                            extendedImg.Load(fs, decoder);
                        }
                        break;
                    default:
                        throw new System.NotSupportedException();

                }
                //var decoder = new ImageTools.IO.Png.PngDecoder();

            }
            //assume 32 bit 

            PixelFarm.Agg.ActualImage actualImg = PixelFarm.Agg.ActualImage.CreateFromBuffer(
                extendedImg.PixelWidth,
                extendedImg.PixelHeight,
                PixelFarm.Agg.PixelFormat.ARGB32,
                extendedImg.Pixels
                );
            //the imgtools load data as BigEndian
            actualImg.IsBigEndian = true;
            return actualImg;
        }
        public static void Start()
        {
            //---------------------------------------------------
            //register image loader
            Mini.DemoHelper.RegisterImageLoader(LoadImage);
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

    class MyNativeRGBA32BitsImage : IDisposable
    {
        int width;
        int height;
        int bitDepth;
        int stride;
        IntPtr unmanagedMem;
        public MyNativeRGBA32BitsImage(int width, int height)
        {
            //width and height must >0 
            this.width = width;
            this.height = height;
            this.bitDepth = 32;
            this.stride = width * (32 / 8);
            unmanagedMem = System.Runtime.InteropServices.Marshal.AllocHGlobal(stride * height);
            //this.pixelBuffer = new byte[stride * height];
        }
        public IntPtr Scan0
        {
            get { return this.unmanagedMem; }
        }
        public int Stride
        {
            get { return this.stride; }
        }
        public void Dispose()
        {
            if (unmanagedMem != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedMem);
                unmanagedMem = IntPtr.Zero;
            }
        }
    }

    class FormRenderUpdateEventArgs : EventArgs
    {
        public GlFwForm form;
    }
}