//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "102")]
    [Info("T102_BasicDraw")]
    public class T102_BasicDraw : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        PixelFarm.Drawing.RenderVx polygon1;
        PixelFarm.Drawing.RenderVx polygon2;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            int max = Math.Max(this.Width, this.Height); 
            polygon1 = painter.CreatePolygonRenderVx(new float[]
            {
                50,200,
                250,200,
                125,350
            });
            polygon2 = painter.CreatePolygonRenderVx(new float[]
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
            painter.StrokeWidth = 1;
            ////line
            painter.FillColor = PixelFarm.Drawing.Color.Green;
            painter.FillRect(100, 100, 50, 50);
            _glsx.DrawLine(50, 50, 200, 200);
            _glsx.DrawRect(10, 10, 50, 50);
            painter.FillRenderVx(polygon2);
            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
            painter.DrawRenderVx(polygon2);
            //-------------------------------------------
            ////polygon 
            painter.DrawRenderVx(polygon1);
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Green;
            ////--------------------------------------------
            painter.DrawCircle(100, 100, 25);
            painter.DrawEllipse(200, 200, 225, 250);
            ////
            painter.FillColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.FillCircle(100, 400, 25);
            _glsx.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.DrawCircle(100, 400, 25);
            ////
            painter.FillColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.FillEllipse(200, 400, 225, 450);
            _glsx.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.DrawEllipse(200, 400, 225, 450);
            //-------------------------------------------
            SwapBuffers();
        }
    }
}
