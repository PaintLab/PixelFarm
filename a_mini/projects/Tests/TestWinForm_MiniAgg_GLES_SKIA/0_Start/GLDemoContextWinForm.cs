//BSD, 2014-2017, WinterDev

using System;
using PixelFarm.DrawingGL;

namespace Mini
{
    class GLDemoContextWinForm
    {
        //this context is for WinForm

        DemoBase demobase;
        OpenTK.MyGLControl glControl;
        IntPtr hh1;
        RenderSurface canvas2d;
        GLCanvasPainter canvasPainter;

        public void LoadGLControl(OpenTK.MyGLControl glControl)
        {
            //----------------------
            this.glControl = glControl;
            glControl.SetGLPaintHandler(HandleGLPaint);
            hh1 = glControl.Handle; //ensure that contrl handler is created
            glControl.MakeCurrent();
        }
        public void LoadSample(DemoBase demobase)
        {
            this.demobase = demobase;
            //1.
            //note:when we init,
            //no glcanvas/ painter are created
            demobase.Init();
            //-----------------------------------------------
            //2. check if demo will create canvas/painter context
            //or let this GLDemoContext create for it

            hh1 = glControl.Handle; //ensure that contrl handler is created
            glControl.MakeCurrent();

            demobase.BuildCustomDemoGLContext(out this.canvas2d, out this.canvasPainter);
            //
            if (this.canvas2d == null)
            {
                //if demo not create canvas and painter
                //the we create for it
                int max = Math.Max(glControl.Width, glControl.Height);
                canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                canvas2d.SmoothMode = CanvasSmoothMode.Smooth;//set anti-alias  
                canvasPainter = new GLCanvasPainter(canvas2d);
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
                //var printer = new GLBmpGlyphTextPrinter(canvasPainter, YourImplementation.BootStrapOpenGLES2.myFontLoader);
                //canvasPainter.TextPrinter = printer;
            }

            //-----------------------------------------------
            demobase.SetEssentialGLHandlers(
                () => this.glControl.SwapBuffers(),
                () => this.glControl.GetEglDisplay(),
                () => this.glControl.GetEglSurface()
            );
            //-----------------------------------------------
            this.glControl.SetGLPaintHandler((s, e) =>
            {
                demobase.InvokeGLPaint();
            });
            DemoBase.InvokeGLContextReady(demobase, this.canvas2d, this.canvasPainter);
            DemoBase.InvokePainterReady(demobase, this.canvasPainter);
        }
        void HandleGLPaint(object sender, System.EventArgs e)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Black;
            canvas2d.ClearColorBuffer();
            //example
            canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            canvasPainter.FillRectLBWH(20, 20, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
            if (demobase != null)
            {
                demobase.Draw(canvasPainter);
            }
            glControl.SwapBuffers();
        }
        public void CloseDemo()
        {
            demobase.CloseDemo();
        }

    }

}