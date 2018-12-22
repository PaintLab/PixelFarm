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
    [Info("T109_LionFillWithRenderVx", SupportedOn = AvailableOn.GLES)]
    public class T109_LionFillWithRenderVx : DemoBase
    {
        GLPainterContext _glsx;
        SpriteShape _lionShape;
        VertexStore _lionVxs;
        GLPainter _painter;
        List<RenderVx> _lionRenderVxList = new List<RenderVx>();
        int _tmpDrawVersion = 0;

        protected override void OnGLSurfaceReady(GLPainterContext glsx, GLPainter painter)
        {
            _glsx = glsx;
            _painter = painter;
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
            if (_tmpDrawVersion == 2)
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

