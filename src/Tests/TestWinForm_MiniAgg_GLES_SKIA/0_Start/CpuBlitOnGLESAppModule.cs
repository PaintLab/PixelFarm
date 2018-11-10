//MIT, 2014-present, WinterDev
//MIT, 2018-present, WinterDev

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
        RootGraphic _rootGfx;
        //
        DemoUI _demoUI;


        DemoBase _demoBase;
        OpenTK.MyGLControl _glControl;
        public CpuBlitOnGLESAppModule() { }


        public void BindSurface(LayoutFarm.UI.UISurfaceViewportControl surfaceViewport)
        {
            _myWidth = 800;
            _myHeight = 600;


            _surfaceViewport = surfaceViewport;
            _rootGfx = surfaceViewport.RootGfx;
            //----------------------
            this._glControl = surfaceViewport.GetOpenTKControl();
            _glControl.SetGLPaintHandler(null);

            IntPtr hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();
        }
        public void LoadExample(DemoBase demoBase)
        {
            _glControl.MakeCurrent();

            this._demoBase = demoBase;
            demoBase.Init();

            _demoUI = new DemoUI(demoBase, _myWidth, _myHeight);
            //use existing GLRenderSurface and GLPainter
            //see=>UISurfaceViewportControl.InitRootGraphics()

            GLRenderSurface glsx = _surfaceViewport.GetGLRenderSurface();
            GLPainter glPainter = _surfaceViewport.GetGLPainter();
            _demoUI.SetCanvasPainter(glsx, glPainter);

            //-----------------------------------------------
            demoBase.SetEssentialGLHandlers(
                () => this._glControl.SwapBuffers(),
                () => this._glControl.GetEglDisplay(),
                () => this._glControl.GetEglSurface()
            );
            //-----------------------------------------------

            DemoBase.InvokeGLContextReady(demoBase, glsx, glPainter);
            DemoBase.InvokePainterReady(demoBase, glPainter);

            //Add to RenderTree
            _rootGfx.TopWindowRenderBox.AddChild(_demoUI.GetPrimaryRenderElement(_rootGfx));
        }
        public void CloseDemo()
        {
            _demoBase.CloseDemo();
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
                    glRenderElem.SetPainter(_glsx, _painter);
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

            GLRenderSurface _glsx;
            GLPainter _glPainter;
            GLBitmap _glBmp;
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


            public void SetPainter(GLRenderSurface glsx, GLPainter canvasPainter)
            {
                _glsx = glsx;
                _glPainter = canvasPainter;
            }
            public void LoadDemo(DemoBase demo)
            {
                _demo = demo;
                if (_aggPainter != null)
                {
                    DemoBase.InvokePainterReady(_demo, _aggPainter);
                }
            }

            void UpdateCpuBlitSurface()
            {

                _aggPainter.Clear(PixelFarm.Drawing.Color.White);

                //TODO:
                //if the content of _aggBmp is not changed
                //we should not draw again

                _demo.Draw(_aggPainter);
                //test print some text
                _aggPainter.FillColor = PixelFarm.Drawing.Color.Black; //set font 'fill' color
                _aggPainter.DrawString("Hello! 12345", 0, 500);
            }

            public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
            {
                _glsx.SmoothMode = SmoothMode.Smooth;
                _glsx.StrokeColor = PixelFarm.Drawing.Color.Black;
                _glsx.ClearColorBuffer();
                //example
                //canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
                //canvasPainter.FillRect(20, 20, 150, 150);
                //load bmp image 
                //-------------------------------------------------------------------------  
                UpdateCpuBlitSurface();


                //TODO: 
                //1. if the content of glBmp is not changed
                //we should not render again 
                //2. if we only update some part of texture
                //may can transfer only that part to the glBmp

                if (_glBmp != null)
                {
                    _glBmp.Dispose();
                    _glBmp = null;
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
                _glPainter.FillColor = PixelFarm.Drawing.Color.Black;
                _glPainter.DrawString("Hello2", 0, 400);

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