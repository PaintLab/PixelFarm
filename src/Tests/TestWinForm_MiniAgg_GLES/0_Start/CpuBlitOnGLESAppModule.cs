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
        UISurfaceViewportControl _surfaceViewport;
        RootGraphic _rootGfx;

        //
        CpuBlitGLESUIElement _bridgeUI;
        DemoBase _demoBase;
        OpenTK.GLControl _glControl;

        public CpuBlitOnGLESAppModule() { }
        public void BindSurface(LayoutFarm.UI.UISurfaceViewportControl surfaceViewport)
        {
            _myWidth = 800;
            _myHeight = 600;

            _surfaceViewport = surfaceViewport;
            _rootGfx = surfaceViewport.RootGfx;
            //----------------------
            _glControl = surfaceViewport.GetOpenTKControl();            

            IntPtr hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.SurfaceControl.MakeCurrent();
        }

        public bool WithGdiPlusDrawBoard { get; set; }

        public void LoadExample(DemoBase demoBase)
        {
            _glControl.SurfaceControl.MakeCurrent();

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
                () => _glControl.SurfaceControl.SwapBuffers(),
                () => _glControl.SurfaceControl.GetEglDisplay(),
                () => _glControl.SurfaceControl.GetEglSurface()
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
            _glControl = null;


        }
    }


}