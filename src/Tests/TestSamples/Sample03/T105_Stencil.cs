//MIT, 2014-2016,WinterDev

using System;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "105")]
    [Info("T105_Stencil")]
    public class T105_Stencil : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        PixelFarm.Drawing.RenderVx stencilPolygon;
        PixelFarm.Drawing.RenderVx rectPolygon;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            stencilPolygon = painter.CreatePolygonRenderVx(new float[]
                {
                    20,20,
                    100,20,
                    60,80
                });
            rectPolygon = painter.CreatePolygonRenderVx(new float[]
            {
                    5,5,
                    100,5,
                    100,100,
                    5,100
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
            //-----------------------------
            //see:  lazyfoo.net/tutorials/OpenGL/26_the_stencil_buffer/index.php
            //-----------------------------

            _glsx.Clear(PixelFarm.Drawing.Color.White);
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

            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.FillRenderVx(stencilPolygon);
            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            //render color
            GL.ColorMask(true, true, true, true);
            //where a 1 was not rendered
            GL.StencilFunc(StencilFunction.Equal, 1, 1);
            //keep the pixel
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            //draw  
            painter.FillColor = PixelFarm.Drawing.Color.Red;
            painter.FillRenderVx(rectPolygon);
            GL.Disable(EnableCap.StencilTest);
            //-----------------------------------------------------------
            SwapBuffers();
        }
    }
}

