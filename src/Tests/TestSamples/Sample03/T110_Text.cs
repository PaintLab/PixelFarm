//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "110")]
    [Info("T110_DrawText")]
    public class T110_DrawText : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
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
            _glsx.Clear(PixelFarm.Drawing.Color.White);

            //-------------------------------
            int line_top = 500;
            painter.FontFillColor = PixelFarm.Drawing.Color.Blue;
            painter.DrawString("ABCD", 0, line_top);
            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
            painter.DrawLine(0, line_top, 300, line_top);
            //
            line_top = 550;
            painter.DrawString("1234567890", 0, line_top);
            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
            painter.DrawLine(0, line_top, 300, line_top);
            //-------------------------------
            SwapBuffers();
        }
    }
}

