
#region Using Directives

using System;
using OpenTK.Graphics.ES20;
using Mini;
#endregion

using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "105")]
    [Info("T105_Stencil")]
    public class T105_Stencil : PrebuiltGLControlDemoBase
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
            //-----------------------------
            //see:  lazyfoo.net/tutorials/OpenGL/26_the_stencil_buffer/index.php
            //-----------------------------
            //test gradient brush
            //set value for clear color
            GLHelper.ClearColor(PixelFarm.Drawing.Color.White);
            GL.ClearStencil(0); //set value for clearing stencil buffer 
                                //actual clear here
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);
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
            float[] stencilPolygon = new float[]
                {
                    20,20,
                    100,20,
                    60,80
                };
            canvas2d.FillPolygon(PixelFarm.Drawing.Color.Black, stencilPolygon);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Black;
            //render color
            GL.ColorMask(true, true, true, true);
            //where a 1 was not rendered
            GL.StencilFunc(StencilFunction.Equal, 1, 1);
            //keep the pixel
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            //draw  
            canvas2d.FillPolygon(PixelFarm.Drawing.Color.Red,
              new float[]
              {
                    5,5,
                    100,5,
                    100,100,
                    5,100
              });
            GL.Disable(EnableCap.StencilTest);
            //
            miniGLControl.SwapBuffers();
        }
    }
}

