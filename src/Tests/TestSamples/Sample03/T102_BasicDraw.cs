//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "102")]
    [Info("T102_BasicDraw", SupportedOn = AvailableOn.GLES)]
    public class T102_BasicDraw : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter _painter;
        PixelFarm.Drawing.RenderVx _polygon1;
        PixelFarm.Drawing.RenderVx _polygon2;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            _glsx = glsx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            int max = Math.Max(this.Width, this.Height);
            _polygon1 = _painter.CreatePolygonRenderVx(new float[]
            {
                50,200,
                250,200,
                125,350
            });
            _polygon2 = _painter.CreatePolygonRenderVx(new float[]
            {
                250,400,
                450,400,
                325,550
            });
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            Test2();
        }
        void Test2()
        {
            _glsx.ClearColorBuffer();
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.StrokeWidth = 1;
            _painter.StrokeWidth = 1;
            ////line
            _painter.FillColor = PixelFarm.Drawing.Color.Green;
            _painter.FillRect(100, 100, 50, 50);
            _glsx.DrawLine(50, 50, 200, 200);

            _painter.FillRenderVx(_polygon2);
            _painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _painter.DrawRenderVx(_polygon2);
            //-------------------------------------------
            ////polygon 
            _painter.DrawRenderVx(_polygon1);
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Green;
            ////--------------------------------------------
            _painter.DrawCircle(100, 100, 25);
            _painter.DrawEllipse(200, 200, 225, 250);
            ////
            _painter.FillColor = PixelFarm.Drawing.Color.OrangeRed;
            _painter.FillCircle(100, 400, 25);
            _glsx.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            _painter.DrawCircle(100, 400, 25);
            ////
            _painter.FillColor = PixelFarm.Drawing.Color.OrangeRed;
            _painter.FillEllipse(200, 400, 225, 450);
            _glsx.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            _painter.DrawEllipse(200, 400, 225, 450);
            //-------------------------------------------
            SwapBuffers();
        }
    }
}
