//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing; 
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "106")]
    [Info("T106_SampleBrushes")]
    public class T106_SampleBrushes : DemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        RenderVx polygon1;
        RenderVx polygon2;
        RenderVx polygon3;

        public override void OnSetupDemoGLContext(CanvasGL2d canvasGL, GLCanvasPainter painter)
        {
            this.canvas2d = canvasGL;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {   
            polygon1 = painter.CreatePolygonRenderVx(new float[]
                {
                    0,50,
                    50,50,
                    10,100
                });
            polygon2 = painter.CreatePolygonRenderVx(new float[]
              {
                   200, 50,
                   250, 50,
                   210, 100
              });
            polygon3 = painter.CreatePolygonRenderVx(new float[]
              {
                 400, 50,
                 450, 50,
                 410, 100
              });
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
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.FillRectLBWH(0, 0, 150, 150);
            GLBitmap glBmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
            var textureBrush = new TextureBrush(glBmp);
            painter.FillRenderVx(textureBrush, polygon1);
            //------------------------------------------------------------------------- 
            var linearGrBrush2 = new LinearGradientBrush(
              new PointF(0, 50), Color.Red,
              new PointF(400, 100), Color.White);
            //fill polygon with gradient brush  
            painter.FillColor = Color.Yellow;
            painter.FillRectLBWH(200, 0, 150, 150);
            painter.FillRenderVx(linearGrBrush2, polygon2);
            painter.FillColor = Color.Black;
            painter.FillRectLBWH(400, 0, 150, 150);
            //-------------------------------------------------------------------------  
            //another  ...                 
            painter.FillRenderVx(linearGrBrush2, polygon3);
            //------------------------------------------------------------------------- 


            SwapBuffers();
        }
    }
}

