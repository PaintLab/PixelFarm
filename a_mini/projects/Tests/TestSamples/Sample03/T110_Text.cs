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
        GLRenderSurface _glsf;
        GLPainter painter;
        protected override void OnGLSurfaceReady(GLRenderSurface glsf, GLPainter painter)
        {
            this._glsf = glsf;
            this.painter = painter; 
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _glsf.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsf.SmoothMode = SmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.ClearColorBuffer();
            _glsf.Clear(PixelFarm.Drawing.Color.Red);

            //-------------------------------
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.DrawString("OK", 0, 100);
            painter.DrawString("1234567890", 0, 200);
            //-------------------------------
            SwapBuffers();
        }
    }
}

