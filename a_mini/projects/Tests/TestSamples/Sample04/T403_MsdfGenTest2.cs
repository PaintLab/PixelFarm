//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "403")]
    [Info("T403_MsdfGenTest2")]
    public class T403_MsdfGenTest2 : DemoBase
    {
        GLRenderSurface _glsf;
        bool resInit;
        GLBitmap msdf_bmp;

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
            _glsf.SmoothMode = SmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.ClearColorBuffer();
            if (!resInit)
            {
                msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png"); 
                resInit = true;
            }
            _glsf.Clear(PixelFarm.Drawing.Color.White);

            //canvas2d.DrawImageWithMsdf(msdf_bmp, 0, 400, 6);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 500, 0.5f);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 520, 0.4f);
            //canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 550, 0.3f);
            //canvas2d.DrawImage(msdf_bmp, 150, 400);

            _glsf.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 200, 500, 15f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 500, 0.5f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 520, 0.4f);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 550, 0.3f);

            ////
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 400, 6);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 500, 0.5f);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 520, 0.4f);
            //canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 550, 0.3f);
            _glsf.DrawImage(msdf_bmp, 100, 300);

            SwapBuffers();
        }
    }
}

