//MIT, 2014-2017, WinterDev
using System;
using PixelFarm.DrawingGL;

namespace Mini
{
    public class GLDemoContext2
    {
        Mini.DemoBase demo;
        int w, h;
        public GLDemoContext2(int w, int h)
        {
            this.w = w;
            this.h = h;
        }
        public void Close()
        {
            demo.CloseDemo();
        }
        public void LoadDemo(Mini.DemoBase demo)
        {
            this.demo = demo;
            demo.Init();
            demo.Width = w;
            demo.Height = h;
            CanvasGL2d canvas2d;
            GLCanvasPainter canvasPainter;
            demo.BuildCustomDemoGLContext(out canvas2d, out canvasPainter);
            if (canvas2d == null)
            {
                //if demo not create canvas and painter
                //the we create for it
                int max = Math.Max(w, h);
                canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                canvasPainter = new GLCanvasPainter(canvas2d, max, max);
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
            }

            demo.SetEssentialGLHandlers(
                () => { },
                () => IntPtr.Zero,
                () => IntPtr.Zero);

            Mini.DemoBase.InvokeGLContextReady(demo, canvas2d, canvasPainter);
        }
        public void Render()
        {
            demo.InvokeGLPaint();
        }
    }
    static class DemoHelper
    {

        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        {
            return null;
            //using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgFileName))
            //{
            //    return LoadTexture(bmp);
            //}
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(PixelFarm.Agg.ActualImage actualImg)
        {
            return new PixelFarm.DrawingGL.GLBitmap(actualImg.Width,
                actualImg.Height,
                PixelFarm.Agg.ActualImage.GetBuffer(actualImg), false);
        }
        //public static PixelFarm.DrawingGL.GLBitmap LoadTexture(System.Drawing.Bitmap bmp)
        //{
        //    return null;
        //    //var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
        //    //    System.Drawing.Imaging.ImageLockMode.ReadOnly,
        //    //    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    //int stride = bmpdata.Stride;
        //    //byte[] buffer = new byte[stride * bmp.Height];
        //    //System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
        //    //bmp.UnlockBits(bmpdata);
        //    ////---------------------------
        //    ////if we are on Little-endian  machine,
        //    ////
        //    ////---------------------------
        //    //return new PixelFarm.DrawingGL.GLBitmap(bmp.Width, bmp.Height, buffer, false);
        //}
    }

}