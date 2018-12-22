//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "403")]
    [Info("T403_MsdfGenTest2", SupportedOn = AvailableOn.GLES)]
    public class T403_MsdfGenTest2 : DemoBase
    {
        GLPainterContext _glsx;
        bool _resInit;
        GLBitmap _msdf_bmp;

        protected override void OnGLSurfaceReady(GLPainterContext glsx, GLPainter painter)
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
                _msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png"); 
                _resInit = true;
            }
            _glsx.Clear(PixelFarm.Drawing.Color.White);

            //canvas2d.DrawImageWithMsdf(msdf_bmp, 0, 400, 6);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 500, 0.5f);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 520, 0.4f);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 550, 0.3f);
            //canvas2d.DrawImage(msdf_bmp, 150, 400);

            _glsx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 200, 500, 15f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 500, 0.5f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 520, 0.4f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 550, 0.3f);

            ////
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 400, 6);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 500, 0.5f);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 520, 0.4f);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 550, 0.3f);
            _glsx.DrawImage(_msdf_bmp, 100, 300);

            SwapBuffers();
        }
    }
}

