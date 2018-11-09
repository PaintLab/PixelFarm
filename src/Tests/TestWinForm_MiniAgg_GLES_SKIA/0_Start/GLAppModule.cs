//BSD, 2014-present, WinterDev

using System;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;

using PixelFarm.Drawing;
using LayoutFarm;
using LayoutFarm.UI;


namespace Mini
{
    class GLAppModule
    {
        //this context is for WinForm

        int _myWidth;
        int _myHeight;
        UISurfaceViewportControl _surfaceViewport;
        InnerViewportKind _innerViewportKind;
        RootGraphic _rootGfx;


        DemoBase _demobase;
        OpenTK.MyGLControl _glControl;
        IntPtr _hh1;
        GLRenderSurface _glsx;
        GLPainter _canvasPainter;

        public GLAppModule() { }
        /// <summary>
        /// agg on gles surface
        /// </summary>
        public bool AggOnGLES { get; set; }

        public void BindSurface(LayoutFarm.UI.UISurfaceViewportControl surfaceViewport)
        {
            _myWidth = 800;
            _myHeight = 600;

            _innerViewportKind = surfaceViewport.InnerViewportKind;
            _surfaceViewport = surfaceViewport;
            _rootGfx = surfaceViewport.RootGfx;


            //----------------------
            this._glControl = surfaceViewport.GetOpenTKControl();
            if (AggOnGLES)
            {

                SetupAggPainter();
                _glControl.SetGLPaintHandler(HandleAggOnGLESPaint);
            }
            else
            {
                _glControl.SetGLPaintHandler(HandleGLPaint);
            }

            _hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();
        }


        public void LoadExample(DemoBase demobase)
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



        //This is a simple UIElement for testing only
        class DemoUI : UIElement
        {
            DemoBase _exampleBase;
            CpuBlitAggCanvasRenderElement _canvasRenderE;
            int _width;
            int _height;
            public DemoUI(DemoBase exBase, int width, int height)
            {
                _width = width;
                _height = height;

                _exampleBase = exBase;
            }
            public override RenderElement CurrentPrimaryRenderElement
            {
                get
                {
                    return _canvasRenderE;
                }
            }

            protected override bool HasReadyRenderElement => _canvasRenderE != null;


            public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
            {
                if (_canvasRenderE == null)
                {
                    _canvasRenderE = new CpuBlitAggCanvasRenderElement(rootgfx, _width, _height);
                    _canvasRenderE.SetController(this); //connect to event system
                    _canvasRenderE.LoadDemo(_exampleBase);
                }
                return _canvasRenderE;
            }

            public override void InvalidateGraphics()
            {

            }

            public override void Walk(UIVisitor visitor)
            {

            }

            //handle event
            protected override void OnMouseDown(UIMouseEventArgs e)
            {
                _exampleBase.MouseDown(e.X, e.Y, e.Button == UIMouseButtons.Right);
                base.OnMouseDown(e);
            }
            protected override void OnMouseMove(UIMouseEventArgs e)
            {
                if (e.IsDragging)
                {
                    _canvasRenderE.InvalidateGraphics();
                    _exampleBase.MouseDrag(e.X, e.Y);
                    _canvasRenderE.InvalidateGraphics();
                }
                base.OnMouseMove(e);
            }
            protected override void OnMouseUp(UIMouseEventArgs e)
            {
                _exampleBase.MouseUp(e.X, e.Y);
                base.OnMouseUp(e);
            }
        }


        //For testing only
        //Implement simple render element***
        class CpuBlitAggCanvasRenderElement : LayoutFarm.RenderElement, IDisposable
        {
            Win32.NativeWin32MemoryDC _nativeWin32DC; //use this as gdi back buffer
            DemoBase _demo;
            ActualBitmap _actualImage;
            Painter _painter;
            public CpuBlitAggCanvasRenderElement(RootGraphic rootgfx, int w, int h)
                : base(rootgfx, w, h)
            {

                //TODO: check if we can access raw rootGraphics buffer or not
                //1. gdi+ create backbuffer
                _nativeWin32DC = new Win32.NativeWin32MemoryDC(w, h);
                //2. create actual bitmap that share bitmap data from native _nativeWin32Dc
                _actualImage = new ActualBitmap(w, h, _nativeWin32DC.PPVBits);
                //----------------------------------------------------------------
                //3. create render surface from bitmap => provide basic bitmap fill operations
                AggRenderSurface aggsx = new AggRenderSurface(_actualImage);
                //4. painter wraps the render surface  => provide advance operations
                AggPainter aggPainter = new AggPainter(aggsx);
                aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);
                _painter = aggPainter;
                //----------------------------------------------------------------             
            }
            public void LoadDemo(DemoBase demo)
            {
                _demo = demo;
                if (_painter != null)
                {
                    DemoBase.InvokePainterReady(_demo, _painter);
                }
            }
            public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
            {
                //
                //TODO: review here again
                //in pure agg, we could bypass the cache/resolve process
                //and render directly to the target canvas
                //
                //if img changed then clear cache and render again
                ActualBitmap.ClearCache(_actualImage);
                ActualBitmap.SetCacheInnerImage(_actualImage, _nativeWin32DC);
                _demo.Draw(_painter);
                //copy from actual image and paint to canvas 
                canvas.DrawImage(_actualImage, 0, 0);
            }
            public override void ResetRootGraphics(RootGraphic rootgfx)
            {

            }
            public void Dispose()
            {
                if (_nativeWin32DC != null)
                {
                    _nativeWin32DC.Dispose();
                    _nativeWin32DC = null;
                }
            }
        }

    }

}