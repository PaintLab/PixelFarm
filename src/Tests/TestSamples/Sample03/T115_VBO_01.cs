//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;

namespace OpenTkEssTest
{
    [Info(OrderCode = "115")]
    [Info("T115_VBO_01")]
    public class T115_VBO_01 : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        PixelFarm.Drawing.RenderVx polygon1, polygon2, polygon3;
        bool isInit;
        bool frameBufferNeedUpdate;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            frameBufferNeedUpdate = true;
            polygon1 = painter.CreatePolygonRenderVx(new float[]
            {
                50,200,
                250,200,
                125,350
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
                 410, 100,
                 350, 100,
                 200,50,
                 100,20
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
            _glsx.Clear(PixelFarm.Drawing.Color.White);
            _glsx.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                isInit = true;
            }
            _glsx.Clear(PixelFarm.Drawing.Color.Blue);
            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            painter.StrokeWidth = 2;
            //-------------------------------
            //painter.FillColor = PixelFarm.Drawing.Color.Yellow;
            //painter.FillRenderVx(polygon1);
            //-------------------------------
            //painter.FillColor = PixelFarm.Drawing.Color.Red;
            //painter.FillRenderVx(polygon2);
            //////-------------------------------
            painter.FillColor = PixelFarm.Drawing.Color.Magenta;
            try
            {
                painter.FillRenderVx(polygon2);
                SwapBuffers();
            }
            catch (Exception ex)
            {
            }
        }
    }
}

