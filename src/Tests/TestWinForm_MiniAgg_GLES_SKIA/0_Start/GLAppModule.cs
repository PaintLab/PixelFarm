//BSD, 2014-present, WinterDev

using System;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
namespace Mini
{
    class GLAppModule
    {
        //this context is for WinForm

        DemoBase _demobase;
        OpenTK.MyGLControl _glControl;
        IntPtr _hh1;
        GLRenderSurface _glsx;
        GLPainter _canvasPainter;

        /// <summary>
        /// agg on gles surface
        /// </summary>
        public bool AggOnGLES { get; set; }

        public void LoadGLControl(OpenTK.MyGLControl glControl)
        {
            //----------------------
            this._glControl = glControl;
            if (AggOnGLES)
            {

                SetupAggPainter();
                glControl.SetGLPaintHandler(HandleAggOnGLESPaint);
            }
            else
            {
                glControl.SetGLPaintHandler(HandleGLPaint);
            }

            _hh1 = glControl.Handle; //ensure that contrl handler is created
            glControl.MakeCurrent();
        }

        public void LoadSample(DemoBase demobase)
        {
            this._demobase = demobase;
            //1.
            //note:when we init,
            //no glcanvas/ painter are created
            demobase.Init();
            //-----------------------------------------------
            //2. check if demo will create canvas/painter context
            //or let this GLDemoContext create for it

            _hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();

            demobase.BuildCustomDemoGLContext(out this._glsx, out this._canvasPainter);
            //
            if (this._glsx == null)
            {
                //if demo not create canvas and painter
                //the we create for it
                int max = Math.Max(_glControl.Width, _glControl.Height);
                _glsx = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, _glControl.Width, _glControl.Height);
                _glsx.SmoothMode = SmoothMode.Smooth;//set anti-alias  
                _canvasPainter = new GLPainter(_glsx);
                //create text printer for opengl 
                //----------------------
                //1. win gdi based
                //var printer = new WinGdiFontPrinter(_glsx, glControl.Width, glControl.Height);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //2. raw vxs
                //var openFontStore = new Typography.TextServices.OpenFontStore();
                //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter,openFontStore);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //3. agg texture based font texture

                //var printer = new AggTextSpanPrinter(canvasPainter, glControl.Width, 30);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //4. texture atlas based font texture 
                //------------
                //resolve request font 
                var printer = new GLBitmapGlyphTextPrinter(_canvasPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
                _canvasPainter.TextPrinter = printer;

                //var openFontStore = new Typography.TextServices.OpenFontStore();
                //var printer = new GLBmpGlyphTextPrinter(canvasPainter, openFontStore);
                //canvasPainter.TextPrinter = printer;
            }

            //-----------------------------------------------
            demobase.SetEssentialGLHandlers(
                () => this._glControl.SwapBuffers(),
                () => this._glControl.GetEglDisplay(),
                () => this._glControl.GetEglSurface()
            );
            //-----------------------------------------------
            if (AggOnGLES)
            {

            }
            else
            {
                this._glControl.SetGLPaintHandler((s, e) =>
                {
                    demobase.InvokeGLPaint();
                });
            }

            DemoBase.InvokeGLContextReady(demobase, this._glsx, this._canvasPainter);
            DemoBase.InvokePainterReady(demobase, this._canvasPainter);
        }
        void HandleGLPaint(object sender, System.EventArgs e)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Black;
            _glsx.ClearColorBuffer();
            //example
            _canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            _canvasPainter.FillRect(20, 20, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
            if (_demobase != null)
            {
                _demobase.Draw(_canvasPainter);
            }
            _glControl.SwapBuffers();
        }

        public void CloseDemo()
        {
            _demobase.CloseDemo();
        }



        ActualBitmap _aggBmp;
        AggPainter _aggPainter;
        GLBitmap _glBmp;
        void SetupAggPainter()
        {
            _aggBmp = new ActualBitmap(800, 600);
            _aggPainter = AggPainter.Create(_aggBmp);

            //optional if we want to print text on agg surface
            _aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
            var aggTextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(_aggPainter);
            _aggPainter.TextPrinter = aggTextPrinter;
            //


        }
        void HandleAggOnGLESPaint(object sender, System.EventArgs e)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Black;
            _glsx.ClearColorBuffer();
            //example
            //canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            //canvasPainter.FillRect(20, 20, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
            if (_demobase != null)
            {
                _aggPainter.Clear(PixelFarm.Drawing.Color.White);
                _demobase.Draw(_aggPainter);

                //test print some text
                _aggPainter.FillColor = PixelFarm.Drawing.Color.Black; //set font 'fill' color
                _aggPainter.DrawString("Hello! 12345", 0, 500);
            }
            //------------------------------------------------------------------------- 
            //copy from 
            if (_glBmp == null)
            {
                _glBmp = new GLBitmap(_aggBmp);
                _glBmp.IsInvert = false;
            }
            _glsx.DrawImage(_glBmp, 0, _aggBmp.Height);

            //test print text from our GLTextPrinter

            _canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            _canvasPainter.DrawString("Hello2", 0, 400);

            //------------------------------------------------------------------------- 
            _glControl.SwapBuffers();
        }
    }

}