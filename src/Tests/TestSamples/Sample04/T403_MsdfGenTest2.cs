//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "403")]
    [Info("T403_MsdfGenTest2", AvailableOn = AvailableOn.GLES)]
    public class T403_MsdfGenTest2 : DemoBase
    {
        GLPainterContext _pcx;
        bool _resInit;
        GLBitmap _msdf_bmp;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
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
            if (!_resInit)
            {
                _msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png");
                _resInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.White);

            //canvas2d.DrawImageWithMsdf(msdf_bmp, 0, 400, 6);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 500, 0.5f);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 520, 0.4f);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 550, 0.3f);
            //canvas2d.DrawImage(msdf_bmp, 150, 400);

            //_pcx.draw(_msdf_bmp, 200, 500, 15f, );
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 500, 0.5f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 520, 0.4f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 550, 0.3f);

            _pcx.DrawImageWithMsdf(_msdf_bmp, 300, 500, 0.5f, PixelFarm.Drawing.Color.Black);
            ////
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 400, 6);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 500, 0.5f);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 520, 0.4f);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 550, 0.3f);
            _pcx.DrawImage(_msdf_bmp, 100, 300);

            SwapBuffers();
        }
    }
}

