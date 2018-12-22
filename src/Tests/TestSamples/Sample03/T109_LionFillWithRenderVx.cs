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
        GLPainterContext _pcx;
        SpriteShape _lionShape;
        VertexStore _lionVxs;
        GLPainter _painter;
        List<RenderVx> _lionRenderVxList = new List<RenderVx>();
        int _tmpDrawVersion = 0;

        protected override void OnGLPainterReady(GLPainterContext pcx, GLPainter painter)
        {
            _pcx = pcx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
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

