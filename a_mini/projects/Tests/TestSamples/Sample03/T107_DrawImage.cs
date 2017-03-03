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
        CanvasGL2d canvas2d;
        bool resInit;
        GLBitmap glbmp;
        protected override void OnGLContextReady(CanvasGL2d canvasGL, GLCanvasPainter painter)
        {
            this.canvas2d = canvasGL;

        }

        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            if (!resInit)
            {
                //glbmp = LoadTexture(@"..\logo-dark.jpg");
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
               
                resInit = true;
            }

            canvas2d.DrawSubImage(glbmp, 10, 10, 100, 100, 200, 400);
            canvas2d.DrawImage(glbmp, 0, 300);
            canvas2d.DrawImageWithBlurX(glbmp, 0, 600);
            SwapBuffers();
        }
    }
}

