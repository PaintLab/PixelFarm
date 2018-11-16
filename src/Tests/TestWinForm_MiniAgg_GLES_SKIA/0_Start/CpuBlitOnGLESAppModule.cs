//MIT, 2014-present, WinterDev
//MIT, 2018-present, WinterDev

using System;
using PixelFarm.DrawingGL;
using LayoutFarm;
using LayoutFarm.UI;
using YourImplementation;
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

        public bool WithGdiPlusDrawBoard { get; set; }

        public void LoadExample(DemoBase demoBase)
        {
            _glControl.MakeCurrent();

            this._demoBase = demoBase;
            demoBase.Init();

            if (WithGdiPlusDrawBoard)
            {
                _demoUI = new GdiOnGLESUIElement(_myWidth, _myHeight);
            }
            else
            {
                //pure agg's cpu blit 
                _demoUI = new CpuBlitGLESUIElement(_myWidth, _myHeight);
            }
            _demoUI.SetUpdateCpuBlitSurfaceDelegate(p => _demoBase.Draw(p));
            DemoBase.InvokePainterReady(_demoBase, _demoUI.GetAggPainter());
            //
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
            //***
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
    }


}