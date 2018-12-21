//MIT, 2014-present,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "106")]
    [Info("T106_SampleBrushes", SupportedOn = AvailableOn.GLES)]
    public class T106_SampleBrushes : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter _painter;
        RenderVx _polygon1;
        RenderVx _polygon2;
        RenderVx _polygon3;

        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            _glsx = glsx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            _polygon1 = _painter.CreatePolygonRenderVx(new float[]
                {
                    0,50,
                    50,50,
                    10,100
                });
            _polygon2 = _painter.CreatePolygonRenderVx(new float[]
              {
                   200, 50,
                   250, 50,
                   210, 100
              });
            _polygon3 = _painter.CreatePolygonRenderVx(new float[]
              {
                 400, 50,
                 450, 50,
                 410, 100
              });
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
            _painter.FillColor = PixelFarm.Drawing.Color.Black;
            _painter.FillRect(0, 0, 150, 150);
            GLBitmap glBmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
            var textureBrush = new TextureBrush(glBmp);
            _painter.FillRenderVx(textureBrush, _polygon1);
            //------------------------------------------------------------------------- 
            var linearGrBrush2 = new LinearGradientBrush(
              new PointF(0, 50), Color.Red,
              new PointF(0, 100), Color.White);
            //fill polygon with gradient brush  
            _painter.FillColor = Color.Yellow;
            _painter.FillRect(200, 0, 150, 150);
            _painter.FillRenderVx(linearGrBrush2, _polygon2);
            _painter.FillColor = Color.Black;
            _painter.FillRect(400, 0, 150, 150);
            //-------------------------------------------------------------------------  
            //another  ...                 
            _painter.FillRenderVx(linearGrBrush2, _polygon3);
            //------------------------------------------------------------------------- 


            SwapBuffers();
        }
    }
}

