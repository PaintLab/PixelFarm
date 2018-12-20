//MIT, 2014-present,WinterDev

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
        bool _resInit;
        GLBitmap _glbmp;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            _glsx = glsx;

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
            if (!_resInit)
            {
                //glbmp = LoadTexture(@"..\logo-dark.jpg");
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");

                _resInit = true;
            }

            _glsx.DrawSubImage(_glbmp, 10, 10, 100, 100, 200, 400);
            _glsx.DrawImage(_glbmp, 0, 300);
            _glsx.DrawImageWithBlurX(_glbmp, 0, 600);

        }
    }
}

