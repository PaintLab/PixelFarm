//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "107")]
    [Info("T107_SampleDrawImage")]
    public class T107_SampleDrawImage : DemoBase
    {
        GLRenderSurface _glsf;
        bool resInit;
        GLBitmap glbmp;
        protected override void OnGLSurfaceReady(GLRenderSurface glsf, GLCanvasPainter painter)
        {
            this._glsf = glsf;

        }

        protected override void DemoClosing()
        {
            _glsf.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsf.SmoothMode = CanvasSmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.ClearColorBuffer();
            if (!resInit)
            {
                //glbmp = LoadTexture(@"..\logo-dark.jpg");
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
               
                resInit = true;
            }

            _glsf.DrawSubImage(glbmp, 10, 10, 100, 100, 200, 400);
            _glsf.DrawImage(glbmp, 0, 300);
            _glsf.DrawImageWithBlurX(glbmp, 0, 600);
            SwapBuffers();
        }
    }
}

