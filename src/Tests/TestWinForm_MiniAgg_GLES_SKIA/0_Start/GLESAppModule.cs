﻿//BSD, 2014-present, WinterDev

using System;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;

using PixelFarm.Drawing;
using LayoutFarm;
using LayoutFarm.UI;


namespace Mini
{
    class GLESAppModule
    {
        //this context is for WinForm
        DemoUI _demoUI;
        int _myWidth;
        int _myHeight;
        UISurfaceViewportControl _surfaceViewport;
        InnerViewportKind _innerViewportKind;
        RootGraphic _rootGfx;


        DemoBase _demoBase;
        OpenTK.MyGLControl _glControl;
        IntPtr _hh1;
        GLRenderSurface _glsx;
        GLPainter _canvasPainter;

        public GLESAppModule() { }


        public void BindSurface(LayoutFarm.UI.UISurfaceViewportControl surfaceViewport)
        {


            _innerViewportKind = surfaceViewport.InnerViewportKind;
            _surfaceViewport = surfaceViewport;
            _rootGfx = surfaceViewport.RootGfx;

            //----------------------
            this._glControl = surfaceViewport.GetOpenTKControl();
            _myWidth = _glControl.Width;
            _myHeight = _glControl.Height;

            //if (AggOnGLES)
            //{
            //    //SetupAggPainter();
            //    //_glControl.SetGLPaintHandler(HandleAggOnGLESPaint);
            //}
            //else
            //{
            //    //_glControl.SetGLPaintHandler(HandleGLPaint);
            //}

            _hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();
        }


        public void LoadExample(DemoBase demoBase)
        {
            int max = Math.Max(_myWidth, _myHeight);
            _glsx = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, _myWidth, _myHeight);
            _glsx.SmoothMode = SmoothMode.Smooth;//set anti-alias  
            _canvasPainter = new GLPainter(_glsx);
            //create text printer for opengl  
            demoBase.Init();
            _demoBase = demoBase;
            // 
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

            _demoUI = new DemoUI(demoBase, _myWidth, _myHeight); 
            _demoUI.SetCanvasPainter(_glsx, _canvasPainter);
            //-----------------------------------------------
            demoBase.SetEssentialGLHandlers(
                () => this._glControl.SwapBuffers(),
                () => this._glControl.GetEglDisplay(),
                () => this._glControl.GetEglSurface()
            );
            //----------------------------------------------- 

            DemoBase.InvokeGLContextReady(demoBase, this._glsx, this._canvasPainter);
            DemoBase.InvokePainterReady(demoBase, this._canvasPainter);


            //Add to RenderTree
            _rootGfx.TopWindowRenderBox.AddChild(_demoUI.GetPrimaryRenderElement(_surfaceViewport.RootGfx));

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
            DemoBase _demo;
            Painter _painter;
            public GLCanvasRenderElement(RootGraphic rootgfx, int w, int h)
                : base(rootgfx, w, h)
            { 
            }
            public void SetPainter(GLPainter canvasPainter)
            {
                _painter = canvasPainter;
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
                _demo.Draw(_painter);
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