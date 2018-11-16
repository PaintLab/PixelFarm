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
        //hardware renderer part=> GLES
        //software renderer part => Pure Agg

        int _myWidth;
        int _myHeight;
        UISurfaceViewportControl _surfaceViewport;
        RootGraphic _rootGfx;
        //
        CpuBlitGLESUIElement _demoUI;
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

            _demoUI = new CpuBlitGLESUIElement(demoBase, _myWidth, _myHeight);
            //use existing GLRenderSurface and GLPainter
            //see=>UISurfaceViewportControl.InitRootGraphics()

            GLRenderSurface glsx = _surfaceViewport.GetGLRenderSurface();
            GLPainter glPainter = _surfaceViewport.GetGLPainter();
            _demoUI.CreatePrimaryRenderElement(glsx, glPainter, _rootGfx);

            //-----------------------------------------------
            demoBase.SetEssentialGLHandlers(
                () => this._glControl.SwapBuffers(),
                () => this._glControl.GetEglDisplay(),
                () => this._glControl.GetEglSurface()
            );
            //-----------------------------------------------

            DemoBase.InvokeGLContextReady(demoBase, glsx, glPainter);
            //Add to RenderTree
            _rootGfx.TopWindowRenderBox.AddChild(_demoUI.GetPrimaryRenderElement(_rootGfx));

            //-----------------------------------------------
            GeneralEventListener genEvListener = new GeneralEventListener();
            genEvListener.MouseDown += e =>
            {
                _demoUI.ContentMayChanged = true;
                _demoBase.MouseDown(e.X, e.Y, e.Button == UIMouseButtons.Right);
                _demoUI.InvalidateGraphics();
            };
            genEvListener.MouseMove += e =>
            {
                if (e.IsDragging)
                {
                    _demoUI.InvalidateGraphics();
                    _demoUI.ContentMayChanged = true;
                    _demoBase.MouseDrag(e.X, e.Y);
                    _demoUI.InvalidateGraphics();
                }
            };
            genEvListener.MouseUp += e =>
            {
                _demoUI.ContentMayChanged = true;
                _demoBase.MouseUp(e.X, e.Y);
            };
            //-----------------------------------------------
            _demoUI.AttachExternalEventListener(genEvListener);
        }
        public void CloseDemo()
        {
            _demoBase.CloseDemo();
        }


        /// <summary>
        /// CpuBlit to GLES UIElement
        /// </summary>
        class CpuBlitGLESUIElement : UIElement
        {
            DemoBase _demoBase;
            int _width;
            int _height;
            CpuBlitGLCanvasRenderElement _canvasRenderE;

            public CpuBlitGLESUIElement(DemoBase demoBase, int width, int height)
            {
                _width = width;
                _height = height;
                _demoBase = demoBase;
                SetupAggCanvas();
                DemoBase.InvokePainterReady(_demoBase, _aggPainter);
                ContentMayChanged = true;
            }

            public void CreatePrimaryRenderElement(GLRenderSurface glsx, GLPainter painter, RootGraphic rootgfx)
            {
                if (_canvasRenderE == null)
                {
                    var glRenderElem = new CpuBlitGLCanvasRenderElement(rootgfx, _width, _height, _lazyImgProvider);
                    glRenderElem.SetPainter(glsx, painter);
                    glRenderElem.SetController(this); //connect to event system
                    glRenderElem.SetOwnerDemoUI(this);
                    _canvasRenderE = glRenderElem;
                }
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
                //for this Elem  => please call CreatePrimaryRenderElement first  
                return _canvasRenderE;
            }

            public override void InvalidateGraphics()
            {
                if (_canvasRenderE != null)
                {
                    _canvasRenderE.InvalidateGraphics();
                }
            }
            public override void Walk(UIVisitor visitor)
            {

            }
            //handle event
            public bool ContentMayChanged { get; set; }


            //---------------------

            ActualBitmap _aggBmp;
            AggPainter _aggPainter;
            LazyActualBitmapBufferProvider _lazyImgProvider;
            void SetupAggCanvas()
            {
                _aggBmp = new ActualBitmap(800, 600);
                _aggPainter = AggPainter.Create(_aggBmp);

                //optional if we want to print text on agg surface
                _aggPainter.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10);
                var aggTextPrinter = new PixelFarm.Drawing.Fonts.FontAtlasTextPrinter(_aggPainter);
                _aggPainter.TextPrinter = aggTextPrinter;
                _lazyImgProvider = new LazyActualBitmapBufferProvider(_aggBmp);
            }
            internal void UpdateCpuBlitSurface()
            {

                _aggPainter.Clear(PixelFarm.Drawing.Color.White);
                //-----------
                //TODO:
                //if the content of _aggBmp is not changed
                //we should not draw again  
                _demoBase.Draw(_aggPainter);
                //
                //test print some text
                _aggPainter.FillColor = PixelFarm.Drawing.Color.Black; //set font 'fill' color
                _aggPainter.DrawString("Hello! 12345", 0, 500);
            }

        }

        class CpuBlitGLCanvasRenderElement : RenderElement, IDisposable
        {

            CpuBlitGLESUIElement _demoUI;
            GLRenderSurface _glsx;
            GLPainter _glPainter;
            GLBitmap _glBmp;

            LazyActualBitmapBufferProvider _lzBmpProvider;
            public CpuBlitGLCanvasRenderElement(RootGraphic rootgfx, int w, int h, LazyActualBitmapBufferProvider lzBmpProvider)
                : base(rootgfx, w, h)
            {
                _lzBmpProvider = lzBmpProvider;// 
            }
            public void SetOwnerDemoUI(CpuBlitGLESUIElement demoUI)
            {
                _demoUI = demoUI;
            }
            public void SetPainter(GLRenderSurface glsx, GLPainter canvasPainter)
            {
                _glsx = glsx;
                _glPainter = canvasPainter;
            }

            public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
            {

                //TODO: 
                //1. if the content of glBmp is not changed
                //we should not render again 
                //2. if we only update some part of texture
                //may can transfer only that part to the glBmp
                //-------------------------------------------------------------------------  
                if (_demoUI.ContentMayChanged)
                {
                    _demoUI.UpdateCpuBlitSurface();
                    _demoUI.ContentMayChanged = false;

                    //load new glBmp 
                    if (_glBmp != null)
                    {
                        _glBmp.Dispose();
                        _glBmp = null;
                    }
                }

                //------------------------------------------------------------------------- 
                //copy from 
                if (_glBmp == null)
                {
                    _glBmp = new GLBitmap(_lzBmpProvider);
                    _glBmp.IsYFlipped = false;
                }


                //------------------------------------------------------------------------- 
                _glsx.DrawImage(_glBmp, 0, 0);
                //test print text from our GLTextPrinter 
                _glPainter.FillColor = PixelFarm.Drawing.Color.Black;
                _glPainter.DrawString("Hello2", 0, 400);
                //------------------------------------------------------------------------- 
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