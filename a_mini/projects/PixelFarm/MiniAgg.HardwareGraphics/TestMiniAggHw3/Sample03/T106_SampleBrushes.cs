
using System;
using PixelFarm.Drawing;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "106")]
    [Info("T106_SampleBrushes")]
    public class T106_SampleBrushes : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            //square viewport
            GL.Viewport(0, 0, max, max);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.FillRect(PixelFarm.Drawing.Color.Black, 0, 0, 150, 150);
            var linearGrBrush2 = new LinearGradientBrush(
                  new PixelFarm.Drawing.PointF(0, 50),
                  PixelFarm.Drawing.Color.Red,
                  new PixelFarm.Drawing.PointF(400, 100),
                  PixelFarm.Drawing.Color.Black);
            //2.fill polygon with gradient brush

            canvas2d.FillPolygon(
                linearGrBrush2,
                new float[]
                {
                    0,50,
                    50,50,
                    10,100
                }, 3 * 2);
            //another  ...                
            canvas2d.FillRect(PixelFarm.Drawing.Color.Yellow, 200, 0, 150, 150);
            canvas2d.FillPolygon(
                linearGrBrush2, new float[] {
                        200, 50,
                        250, 50,
                        210, 100}, 3 * 2);
            canvas2d.FillPolygon(
                   linearGrBrush2, new float[] {
                       400, 50,
                       450, 50,
                       410, 100}, 3 * 2);
            //------------------------------------------------------------------------- 

            miniGLControl.SwapBuffers();
        }
    }
}

