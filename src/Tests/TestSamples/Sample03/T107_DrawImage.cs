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
        GLRenderSurface _glsx;
        bool resInit;
        GLBitmap glbmp;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;

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
            if (!resInit)
            {
                //glbmp = LoadTexture(@"..\logo-dark.jpg");
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
               
                resInit = true;
            }

            _glsx.DrawSubImage(glbmp, 10, 10, 100, 100, 200, 400);
            _glsx.DrawImage(glbmp, 0, 300);
            _glsx.DrawImageWithBlurX(glbmp, 0, 600);
            SwapBuffers();
        }
    }
}

