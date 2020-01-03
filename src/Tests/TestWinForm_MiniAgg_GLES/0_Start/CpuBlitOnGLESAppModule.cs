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
        //FOR DEMO PROJECT
        //hardware renderer part=> GLES
        //software renderer part => Pure Agg

        int _myWidth;
        int _myHeight;
        GraphicsViewRoot _surfaceViewport;
        RootGraphic _rootGfx;

        //
        CpuBlitGLESUIElement _bridgeUI;
        DemoBase _demoBase;

        IGpuOpenGLSurfaceView _nativeWindow;

        public CpuBlitOnGLESAppModule() { }
        public void BindSurface(LayoutFarm.UI.GraphicsViewRoot surfaceViewport)
        {
            _myWidth = 800;
            _myHeight = 600;

            _surfaceViewport = surfaceViewport;
            _rootGfx = surfaceViewport.RootGfx;
            //----------------------

            _nativeWindow = surfaceViewport.MyNativeWindow;
            _nativeWindow.MakeCurrent();
        }

        public bool WithGdiPlusDrawBoard { get; set; }

        public void LoadExample(DemoBase demoBase)
        {
            _nativeWindow.MakeCurrent();

            _demoBase = demoBase;
            demoBase.Init();

            if (WithGdiPlusDrawBoard)
            {
                _bridgeUI = new GdiOnGLESUIElement(_myWidth, _myHeight);
            }
            else
            {
                //pure agg's cpu blit 
                _bridgeUI = new CpuBlitGLESUIElement(_myWidth, _myHeight);
            }
            //
            _bridgeUI.SetUpdateCpuBlitSurfaceDelegate((p, updateArea) => _demoBase.Draw(p));

            DemoBase.InvokePainterReady(_demoBase, _bridgeUI.GetAggPainter());
            //
            //use existing GLRenderSurface and GLPainter
            //see=>UISurfaceViewportControl.InitRootGraphics()

            GLPainterContext pcx = _surfaceViewport.GetGLRenderSurface();
            GLPainter glPainter = _surfaceViewport.GetGLPainter();
            _bridgeUI.CreatePrimaryRenderElement(pcx, glPainter, _rootGfx);
            //-----------------------------------------------


            demoBase.SetEssentialGLHandlers(
                 _nativeWindow.SwapBuffers,
                 _nativeWindow.GetEglDisplay,
                 _nativeWindow.GetEglSurface
            );
            //-----------------------------------------------
            DemoBase.InvokeGLPainterReady(demoBase, pcx, glPainter);
            //Add to RenderTree
            _rootGfx.AddChild(_bridgeUI.GetPrimaryRenderElement(_rootGfx));
            //-----------------------------------------------
            //***
            GeneralEventListener genEvListener = new GeneralEventListener();
            genEvListener.MouseDown += e =>
            {
                _demoBase.MouseDown(e.X, e.Y, e.Button == UIMouseButtons.Right);
                _bridgeUI.InvalidateGraphics();
            };
            genEvListener.MouseMove += e =>
            {
                if (e.IsDragging)
                {
                    _bridgeUI.InvalidateGraphics();
                    _demoBase.MouseDrag(e.X, e.Y);
                    _bridgeUI.InvalidateGraphics();
                }
            };
            genEvListener.MouseUp += e =>
            {
                _demoBase.MouseUp(e.X, e.Y);
            };
            //-----------------------------------------------
            _bridgeUI.AttachExternalEventListener(genEvListener);
        }

        public void Close()
        {
            _demoBase.CloseDemo();
            _demoBase = null;

            if (_surfaceViewport != null)
            {
                _surfaceViewport.Close();
                _surfaceViewport = null;
            }
            _rootGfx = null;

        }
    }


}