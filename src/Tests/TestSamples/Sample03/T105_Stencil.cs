//MIT, 2014-present,WinterDev

using System;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "105")]
    [Info("T105_Stencil", SupportedOn = AvailableOn.GLES)]
    public class T105_Stencil : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        PixelFarm.Drawing.RenderVx _stencilPolygon;
        PixelFarm.Drawing.RenderVx _rectPolygon;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            _stencilPolygon = _painter.CreatePolygonRenderVx(new float[]
                {
                    20,20,
                    100,20,
                    60,80
                });
            _rectPolygon = _painter.CreatePolygonRenderVx(new float[]
            {
                    5,5,
                    100,5,
                    100,100,
                    5,100
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
            //-----------------------------
            //see:  lazyfoo.net/tutorials/OpenGL/26_the_stencil_buffer/index.php
            //-----------------------------

            _pcx.Clear(PixelFarm.Drawing.Color.White);
            //-------------------
            //disable rendering to color buffer
            GL.ColorMask(false, false, false, false);
            //start using stencil
            GL.Enable(EnableCap.StencilTest);
            //place a 1 where rendered
            GL.StencilFunc(StencilFunction.Always, 1, 1);
            //replace where rendered
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            //render  to stencill buffer

            _painter.FillColor = PixelFarm.Drawing.Color.Black;
            _painter.FillRenderVx(_stencilPolygon);
            _painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            //render color
            GL.ColorMask(true, true, true, true);
            //where a 1 was not rendered
            GL.StencilFunc(StencilFunction.Equal, 1, 1);
            //keep the pixel
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            //draw  
            _painter.FillColor = PixelFarm.Drawing.Color.Red;
            _painter.FillRenderVx(_rectPolygon);
            GL.Disable(EnableCap.StencilTest);
            //-----------------------------------------------------------
            SwapBuffers();
        }
    }
}

