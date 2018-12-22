//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;

namespace OpenTkEssTest
{
    [Info(OrderCode = "115")]
    [Info("T115_VBO_01", SupportedOn = AvailableOn.GLES)]
    public class T115_VBO_01 : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        PixelFarm.Drawing.RenderVx _polygon1, _polygon2, _polygon3;
        bool _isInit;

        protected override void OnGLPainterReady(GLPainterContext pcx, GLPainter painter)
        {
            _pcx = pcx;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            _polygon1 = _painter.CreatePolygonRenderVx(new float[]
            {
                50,200,
                250,200,
                125,350
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
                 410, 100,
                 350, 100,
                 200,50,
                 100,20
              });
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {

            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.Clear(PixelFarm.Drawing.Color.White);
            _pcx.ClearColorBuffer();
            //-------------------------------
            if (!_isInit)
            {
                _isInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.Blue);
            _painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            _painter.StrokeWidth = 2;
            //-------------------------------
            //painter.FillColor = PixelFarm.Drawing.Color.Yellow;
            //painter.FillRenderVx(polygon1);
            //-------------------------------
            //painter.FillColor = PixelFarm.Drawing.Color.Red;
            //painter.FillRenderVx(polygon2);
            //////-------------------------------
            _painter.FillColor = PixelFarm.Drawing.Color.Magenta;
            try
            {
                _painter.FillRenderVx(_polygon3);
                SwapBuffers();
            }
            catch (Exception ex)
            {
            }
        }
    }
}

