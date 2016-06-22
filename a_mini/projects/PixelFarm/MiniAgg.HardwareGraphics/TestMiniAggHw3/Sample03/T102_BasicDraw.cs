//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "102")]
    [Info("T102_BasicDraw")]
    public class T102_BasicDraw : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
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
            Test2();
        }
        void Test2()
        {
            canvas2d.ClearColorBuffer();
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            float[] polygon2 = new float[]{
                250,400,
                450,400,
                325,550
            };
            ////line
            canvas2d.FillRect(PixelFarm.Drawing.Color.Green, 100, 100, 50, 50);
            canvas2d.DrawLine(50, 50, 200, 200);
            canvas2d.DrawRect(10, 10, 50, 50);
            canvas2d.FillPolygon(PixelFarm.Drawing.Color.Green, polygon2);
            canvas2d.DrawPolygon(polygon2, 3 * 2);
            ////polygon
            float[] polygon1 = new float[]{
                50,200,
                250,200,
                125,350
            };
            canvas2d.DrawPolygon(polygon1, 3 * 2);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Green;
            //--------------------------------------------
            canvas2d.DrawCircle(100, 100, 25);
            canvas2d.DrawEllipse(200, 200, 25, 50);
            //

            canvas2d.FillCircle(PixelFarm.Drawing.Color.OrangeRed, 100, 400, 25);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            canvas2d.DrawCircle(100, 400, 25);
            //
            canvas2d.FillEllipse(PixelFarm.Drawing.Color.OrangeRed, 200, 400, 25, 50);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            canvas2d.DrawEllipse(200, 400, 25, 50);
            miniGLControl.SwapBuffers();
        }
    }
}
