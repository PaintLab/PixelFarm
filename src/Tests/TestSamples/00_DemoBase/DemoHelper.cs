//MIT, 2014-2017, WinterDev
using System;
using PixelFarm.DrawingGL;
using Typography.TextServices;

namespace Mini
{

    public delegate void SetupPainterDel(GLPainter painter);

    public class GLDemoContext
    {
        Mini.DemoBase demo;
        int w, h;
        SetupPainterDel _getTextPrinterDel;

        public GLDemoContext(int w, int h)
        {
            this.w = w;
            this.h = h;
        }
        public void SetTextPrinter(SetupPainterDel del)
        {
            _getTextPrinterDel = del;
        }
        public void Close()
        {
            demo.CloseDemo();
        }
        public void LoadDemo(Mini.DemoBase demo)
        {
            this.demo = demo;
            demo.Init();

            int max = Math.Max(w, h);

            demo.Width = w;
            demo.Height = h;
            GLRenderSurface glsx;
            GLPainter canvasPainter;
            demo.BuildCustomDemoGLContext(out glsx, out canvasPainter);
            if (glsx == null)
            {
                //if demo not create canvas and painter
                //the we create for it
                //int max = Math.Max(w, h);
                //canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                //canvasPainter = new GLCanvasPainter(canvas2d, max, max);

                //canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(w, h);
                glsx = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, w, h);
                canvasPainter = new GLPainter(glsx);

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

                if (_getTextPrinterDel != null)
                {
                    _getTextPrinterDel(canvasPainter);
                }

            }

            demo.SetEssentialGLHandlers(
                () => { },
                () => IntPtr.Zero,
                () => IntPtr.Zero);

            DemoBase.InvokeGLContextReady(demo, glsx, canvasPainter);
            DemoBase.InvokePainterReady(demo, canvasPainter);
        }
        public void Render()
        {
            demo.InvokeGLPaint();
        }
    }



    public delegate PixelFarm.Agg.ActualImage LoadImageDelegate(string filename);

    public static class DemoHelper
    {
        static LoadImageDelegate s_LoadImgDel;
        //static IInstalledFontProvider s_fontProvider;
        //public static void RegisterFontProvider(IInstalledFontProvider fontProvider)
        //{
        //    s_fontProvider = fontProvider;
        //}
        public static void RegisterImageLoader(LoadImageDelegate loadImgDel)
        {
            s_LoadImgDel = loadImgDel;
        }
        //public static IInstalledFontProvider GetRegisterInstalledFontProvider()
        //{
        //    return s_fontProvider;
        //}
        public static PixelFarm.Agg.ActualImage LoadImage(string imgFileName)
        {
            return s_LoadImgDel(imgFileName);
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        {
            return LoadTexture(s_LoadImgDel(imgFileName));
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(PixelFarm.Agg.ActualImage actualImg)
        {
            return new PixelFarm.DrawingGL.GLBitmap(actualImg)
            { IsBigEndianPixel = actualImg.IsBigEndian };

        }

        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(PixelFarm.Drawing.Image bmp)
        {
            return null;

            //var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //    System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //int stride = bmpdata.Stride;
            //byte[] buffer = new byte[stride * bmp.Height];
            //System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
            //bmp.UnlockBits(bmpdata);
            ////---------------------------
            ////if we are on Little-endian  machine,
            ////
            ////---------------------------
            //return new PixelFarm.DrawingGL.GLBitmap(bmp.Width, bmp.Height, buffer, false);
        }
    }

}