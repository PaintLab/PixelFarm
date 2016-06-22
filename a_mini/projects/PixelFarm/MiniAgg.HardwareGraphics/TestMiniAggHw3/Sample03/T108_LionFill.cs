//MIT, 2014-2016,WinterDev
using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Agg;

namespace OpenTkEssTest
{
    [Info(OrderCode = "108")]
    [Info("T108_LionFill")]
    public class T108_LionFill : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        SpriteShape lionShape;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);

            lionShape = new SpriteShape();
            lionShape.ParseLion();
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

            int npath = lionShape.NumPaths;
            int[] pathIndexList = lionShape.PathIndexList;

            //-------------------------------
            miniGLControl.SwapBuffers();
        }

    }
}

