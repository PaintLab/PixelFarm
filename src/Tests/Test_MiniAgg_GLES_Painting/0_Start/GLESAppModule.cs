//BSD, 2014-present, WinterDev

using System;
using PixelFarm.DrawingGL;

using PixelFarm.Drawing;
using LayoutFarm;
using LayoutFarm.UI;


namespace Mini
{



    class GLESAppModule
    {
        //FOR DEMO PROJECT

        //hardware renderer part=> GLES
        //software renderer part => not specific now

        int _myWidth;
        int _myHeight;
        GraphicsViewRoot _gfxViewRoot;

        RootGraphic _rootGfx;
        //
        DemoUI _demoUI;
        DemoBase _demoBase;


        public GLESAppModule()
        {
        }
        public void BindSurface(GraphicsViewRoot gfxViewRoot)
        {
            _gfxViewRoot = gfxViewRoot;
            _rootGfx = gfxViewRoot.RootGfx;

            _myWidth = gfxViewRoot.Width;
            _myHeight = gfxViewRoot.Height;

            gfxViewRoot.MakeCurrent();
        }

        public void LoadExample(DemoBase demoBase)
        {
            _gfxViewRoot.MakeCurrent();


            GLPainterCore pcx = _gfxViewRoot.GLPainterCore();
            GLPainter glPainter = _gfxViewRoot.GetGLPainter();

            pcx.SmoothMode = SmoothMode.Smooth;//set anti-alias  

            //create text printer for opengl  
            demoBase.Init();
            _demoBase = demoBase;

            _demoUI = new DemoUI(demoBase, _myWidth, _myHeight);
            _demoUI.SetCanvasPainter(pcx, glPainter);
            //-----------------------------------------------
            //demoBase.SetEssentialGLHandlers(
            //    () => _glControl.SwapBuffers(),
            //    () => _glControl.GetEglDisplay(),
            //    () => _glControl.GetEglSurface()
            //);
            //----------------------------------------------- 

            DemoBase.InvokeGLPainterReady(demoBase, pcx, glPainter);
            //Add to RenderTree
            _rootGfx.AddChild(_demoUI.GetPrimaryRenderElement(_gfxViewRoot.RootGfx));
        }
        public void Close()
        {

            _demoBase.CloseDemo();
            if (_gfxViewRoot != null)
            {
                _gfxViewRoot.Close();
                _gfxViewRoot = null;
            }
            _rootGfx = null;
        }
        //This is a simple UIElement for testing only
        class DemoUI : UIElement
        {
            DemoBase _demoBase;
            RenderElement _canvasRenderE;
            int _width;
            int _height;
            GLPainterCore _pcx;
            //
            GLPainter _painter;
            public DemoUI(DemoBase demobase, int width, int height)
            {
                _width = width;
                _height = height;
                _demoBase = demobase;
            }

            public void SetCanvasPainter(GLPainterCore pcx, GLPainter painter)
            {
                _pcx = pcx;
                _painter = painter;
            }

            public override RenderElement CurrentPrimaryRenderElement => _canvasRenderE;

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
            //handle event
            protected override void OnMouseDown(UIMouseDownEventArgs e)
            {
                _demoBase.MouseDown(e.X, e.Y, e.Buttons == UIMouseButtons.Right);
                base.OnMouseDown(e);
            }
            protected override void OnMouseMove(UIMouseMoveEventArgs e)
            {
                if (e.IsDragging)
                {
                    _canvasRenderE.InvalidateGraphics();
                    _demoBase.MouseDrag(e.X, e.Y);
                    _canvasRenderE.InvalidateGraphics();
                }
                base.OnMouseMove(e);
            }
            protected override void OnMouseUp(UIMouseUpEventArgs e)
            {
                _demoBase.MouseUp(e.X, e.Y);
                base.OnMouseUp(e);
            }
        }

        class GLCanvasRenderElement : RenderElement, IDisposable
        {
            DemoBase _demo;
            GLPainter _painter;
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
            protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
            {
                //****
                //because our demo may run the 'manual GL code', 
                //(out of state-control of the shader share resource/ current program/ render tree/ 
                //***
                _painter.DetachCurrentShader();
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