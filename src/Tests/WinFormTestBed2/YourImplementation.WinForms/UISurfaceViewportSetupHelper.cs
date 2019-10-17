//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;

namespace YourImplementation
{
    static class UISurfaceViewportSetupHelper
    {
        public static void SetUISurfaceViewportControl(LayoutFarm.AppHostConfig config, LayoutFarm.UI.UISurfaceViewportControl vw)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            System.Drawing.Rectangle primScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            config.ScreenW = primScreenWorkingArea.Width;
            config.ScreenH = primScreenWorkingArea.Height;
            config.RootGfx = vw.RootGfx;
            // 
            switch (vw.InnerViewportKind)
            {
                case InnerViewportKind.GdiPlusOnGLES:
                case InnerViewportKind.AggOnGLES:
                    SetUpSoftwareRendererOverGLSurface(
                        config,
                        vw.GetOpenTKControl(),
                        vw.GetGLRenderSurface(),
                        vw.GetGLPainter(),
                        vw.InnerViewportKind);
                    break;
            }
        }

        static void SetUpSoftwareRendererOverGLSurface(
          LayoutFarm.AppHostConfig config,
          OpenTK.GLControl glControl,
          PixelFarm.DrawingGL.GLPainterContext pcx,
          PixelFarm.DrawingGL.GLPainter glPainter,
          InnerViewportKind innerViewPortKind)
        {
            if (glControl == null) return;
            //TODO: review here
            //Temp:  
            //
            IntPtr hh1 = glControl.Handle; //ensure that contrl handler is created
            glControl.MakeCurrent();

            CpuBlitGLESUIElement _cpuBlitUIElem = (innerViewPortKind == InnerViewportKind.GdiPlusOnGLES) ?
                 new GdiOnGLESUIElement(glControl.Width, glControl.Height) :
                 new CpuBlitGLESUIElement(glControl.Width, glControl.Height);

            //optional***
            //_bridgeUI.SetUpdateCpuBlitSurfaceDelegate((p, area) =>
            //{
            //    _client.DrawToThisCanvas(_bridgeUI.GetDrawBoard(), area);
            //}); 

            LayoutFarm.RootGraphic rootGfx = config.RootGfx;
            _cpuBlitUIElem.CreatePrimaryRenderElement(pcx, glPainter, rootGfx);

            //*****
            LayoutFarm.RenderBoxBase renderE = (LayoutFarm.RenderBoxBase)_cpuBlitUIElem.GetPrimaryRenderElement(rootGfx);
            rootGfx.AddChild(renderE);
            rootGfx.SetPrimaryContainerElement(renderE);
            //***
        }


    }
}