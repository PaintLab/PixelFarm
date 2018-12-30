//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "401", AvailableOn = AvailableOn.GLES)]
    [Info("T401_MsdfTest1")]
    public class T401_MsdfTest1 : DemoBase
    {
        GLPainterContext _pcx;
        bool _resInit;
        GLBitmap _msdf_bmp;
        GLBitmap _sdf_bmp;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;

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
            if (!_resInit)
            {

                _msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png");
                _sdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\sdf_75.png");
                _resInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.White);

            _pcx.DrawImageWithMsdf(_msdf_bmp, 0, 400, 6);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 500, 0.5f);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 520, 0.4f);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 550, 0.3f);
            _pcx.DrawImage(_msdf_bmp, 150, 400);

            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 200, 400, 6);
            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 500, 0.5f);
            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 520, 0.4f);
            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 550, 0.3f);

            //
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 400, 6);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 500, 0.5f);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 520, 0.4f);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 550, 0.3f);
            _pcx.DrawImage(_sdf_bmp, 400, 300);

            SwapBuffers();
        }
    }
}

