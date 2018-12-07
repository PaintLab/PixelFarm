//MIT, 2014-present,WinterDev

using System;
using System.Collections.Generic;

using Mini;

using PixelFarm.Drawing;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
namespace OpenTkEssTest
{
    [Info(OrderCode = "109")]
    [Info("T109_LionFillWithRenderVx")]
    public class T109_LionFillWithRenderVx : DemoBase
    {
        GLRenderSurface _glsx;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLPainter painter;
        List<RenderVx> lionRenderVxList = new List<RenderVx>();
        int tmpDrawVersion = 0;

        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            _glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
          
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
            //-------------------------------
            if (tmpDrawVersion == 2)
            {
                //TODO: impl this again
                //2018-08-01
                 

            }
            else
            {
                //TODO: impl this again
                //2018-08-01
         
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

