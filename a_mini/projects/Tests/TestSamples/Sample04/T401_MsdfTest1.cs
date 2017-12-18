//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "401")]
    [Info("T401_MsdfTest1")]
    public class T401_MsdfTest1 : DemoBase
    {
        GLRenderSurface _glsf;
        bool resInit;
        GLBitmap msdf_bmp;
        GLBitmap sdf_bmp;
        protected override void OnGLSurfaceReady(GLRenderSurface glsf, GLCanvasPainter painter)
        {
            this._glsf = glsf;

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
            _glsf.SmoothMode = CanvasSmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.ClearColorBuffer();
            if (!resInit)
            {

                msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png");
                sdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\sdf_75.png");
                resInit = true;
            }
            _glsf.Clear(PixelFarm.Drawing.Color.White);

            _glsf.DrawImageWithMsdf(msdf_bmp, 0, 400, 6);
            _glsf.DrawImageWithMsdf(msdf_bmp, 100, 500, 0.5f);
            _glsf.DrawImageWithMsdf(msdf_bmp, 100, 520, 0.4f);
            _glsf.DrawImageWithMsdf(msdf_bmp, 100, 550, 0.3f);
            _glsf.DrawImage(msdf_bmp, 150, 400);

            _glsf.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 200, 400, 6);
            _glsf.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 500, 0.5f);
            _glsf.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 520, 0.4f);
            _glsf.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 550, 0.3f);

            //
            _glsf.DrawImageWithMsdf(sdf_bmp, 400, 400, 6);
            _glsf.DrawImageWithMsdf(sdf_bmp, 400, 500, 0.5f);
            _glsf.DrawImageWithMsdf(sdf_bmp, 400, 520, 0.4f);
            _glsf.DrawImageWithMsdf(sdf_bmp, 400, 550, 0.3f);
            _glsf.DrawImage(sdf_bmp, 400, 300);

            SwapBuffers();
        }
    }
}

