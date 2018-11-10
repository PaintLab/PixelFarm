//BSD, 2014-present, WinterDev

using System;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;

using PixelFarm.Drawing;
using LayoutFarm;
using LayoutFarm.UI;


namespace Mini
{
    class CpuBlitOnGLESAppModule
    {
        //this context is for WinForm

        int _myWidth;
        int _myHeight;
        UISurfaceViewportControl _surfaceViewport;
        InnerViewportKind _innerViewportKind;
        RootGraphic _rootGfx;

        DemoUI _demoUI;
        GLCanvasRenderElement _canvasRenderElement;

        DemoBase _demoBase;
        OpenTK.MyGLControl _glControl;
        IntPtr _hh1;
        GLRenderSurface _glsx;
        GLPainter _canvasPainter;
        GLBitmap _glBmp;


        public CpuBlitOnGLESAppModule() { }


        public void BindSurface(LayoutFarm.UI.UISurfaceViewportControl surfaceViewport)
        {
            _myWidth = 800;
            _myHeight = 600;

            _innerViewportKind = surfaceViewport.InnerViewportKind;
            _surfaceViewport = surfaceViewport;
            _rootGfx = surfaceViewport.RootGfx;


            //----------------------
            this._glControl = surfaceViewport.GetOpenTKControl();
            _glControl.SetGLPaintHandler(HandleAggOnGLESPaint);

            _hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();
        }


        public void LoadExample(DemoBase demoBase)
        {


            _hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();

            //
            this._demoBase = demoBase;
            demoBase.Init();

            _demoUI = new DemoUI(demoBase, _myWidth, _myHeight);


            //use existing GLRenderSurface and GLPainter
            //see=>UISurfaceViewportControl.InitRootGraphics()

            _glsx = _surfaceViewport.GetGLRenderSurface();
            _canvasPainter = _surfaceViewport.GetGLPainter();

            //-----------------------------------------------
            demoBase.SetEssentialGLHandlers(
                () => this._glControl.SwapBuffers(),
                () => this._glControl.GetEglDisplay(),
                () => this._glControl.GetEglSurface()
            );
            //-----------------------------------------------

            DemoBase.InvokeGLContextReady(demoBase, this._glsx, this._canvasPainter);
            DemoBase.InvokePainterReady(demoBase, this._canvasPainter);


            _canvasRenderElement = (GLCanvasRenderElement)_demoUI.GetPrimaryRenderElement(_rootGfx);
            //Add to RenderTree
            _rootGfx.TopWindowRenderBox.AddChild(_canvasRenderElement);


        }


        public void CloseDemo()
        {
            _demoBase.CloseDemo();
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

            _canvasRenderElement.UpdateCpuBlitSurface();

            if (_glBmp != null)
            {
                _glBmp.Dispose();
                _glBmp = null;
            }
            //------------------------------------------------------------------------- 
            //copy from 
            if (_glBmp == null)
            {
                _glBmp = new GLBitmap(_canvasRenderElement.ActualBmp);
                _glBmp.IsInvert = false;
            }
            _glsx.DrawImage(_glBmp, 0, _canvasRenderElement.ActualBmp.Height);

            //test print text from our GLTextPrinter

            _canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            _canvasPainter.DrawString("Hello2", 0, 400);

            //------------------------------------------------------------------------- 
            //_glControl.SwapBuffers();
        }



        //This is a simple UIElement for testing only
        class DemoUI : UIElement
        {
            DemoBase _demoBase;
            RenderElement _canvasRenderE;
            int _width;
            int _height;
            GLRenderSurface _glsx;
            //
            GLPainter _painter;
            public DemoUI(DemoBase demobase, int width, int height)
            {
                _width = width;
                _height = height;
                _demoBase = demobase;
            }

            public void SetCanvasPainter(GLRenderSurface glsx, GLPainter painter)
            {
                _glsx = glsx;
                _painter = painter;
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

                    var glRenderElem = new GLCanvasRenderElement(rootgfx, _width, _height);
                    glRenderElem.SetPainter(_painter);
                    glRenderElem.SetController(this); //connect to event system
                    glRenderElem.LoadDemo(_demoBase);
                    _canvasRenderE = glRenderElem;


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
                _demoBase.MouseDown(e.X, e.Y, e.Button == UIMouseButtons.Right);
                base.OnMouseDown(e);
            }
            protected override void OnMouseMove(UIMouseEventArgs e)
            {
                if (e.IsDragging)
                {
                    _canvasRenderE.InvalidateGraphics();
                    _demoBase.MouseDrag(e.X, e.Y);
                    _canvasRenderE.InvalidateGraphics();
                }
                base.OnMouseMove(e);
            }
            protected override void OnMouseUp(UIMouseEventArgs e)
            {
                _demoBase.MouseUp(e.X, e.Y);
                base.OnMouseUp(e);
            }
        }

        class GLCanvasRenderElement : RenderElement, IDisposable
        {
            ActualBitmap _aggBmp;
            AggPainter _aggPainter;
            DemoBase _demo;
            Painter _painter;


            GLPainter _glPainter;
            public GLCanvasRenderElement(RootGraphic rootgfx, int w, int h)
                : base(rootgfx, w, h)
            {
                _aggBmp = new ActualBitmap(800, 600);
                _aggPainter = AggPainter.Create(_aggBmp);

                //optional if we want to print text on agg surface
                _aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
                var aggTextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(_aggPainter);
                _aggPainter.TextPrinter = aggTextPrinter;
                // 
            }
            public ActualBitmap ActualBmp => _aggBmp;
            public void UpdateCpuBlitSurface()
            {
                _aggPainter.Clear(PixelFarm.Drawing.Color.White);
                _demo.Draw(_aggPainter);

                //test print some text
                _aggPainter.FillColor = PixelFarm.Drawing.Color.Black; //set font 'fill' color
                _aggPainter.DrawString("Hello! 12345", 0, 500);
            }
            public void SetPainter(GLPainter canvasPainter)
            {
                _glPainter = canvasPainter;
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
                //_demo.Draw(_painter);
            }
            public override void ResetRootGraphics(RootGraphic rootgfx)
            {

            }
            public void Dispose()
            {

            }
        }

    }

}