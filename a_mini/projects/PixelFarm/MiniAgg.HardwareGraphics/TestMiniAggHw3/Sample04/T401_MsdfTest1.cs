//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "401")]
    [Info("T401_MsdfTest1")]
    public class T401_MsdfTest1 : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        bool resInit;
        GLBitmap glbmp;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
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
                //glbmp = LoadTexture(@"..\msdf_A.png");
                glbmp = LoadTexture(@"..\sdf_A.png");
                resInit = true;
            }
            //canvas2d.DrawImageWithMsdf(glbmp, 0, 0);
            canvas2d.DrawImageWithSdf(glbmp, 0, 0);
            canvas2d.DrawImage(glbmp, 0, 300);
            miniGLControl.SwapBuffers();
        }
    }
}

