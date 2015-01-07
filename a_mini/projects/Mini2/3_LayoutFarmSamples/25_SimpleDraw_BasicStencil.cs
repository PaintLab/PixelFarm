using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;

using OpenTK.Graphics.OpenGL;

namespace Mini2
{
    [Info(OrderCode = "25")]
    [Info("DrawSample05_BasicStencil")]
    public class DrawSample05_BasicStencil : DemoBase
    {
        static DrawSample05_BasicStencil()
        {
            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);


            form.SetGLPaintHandler((o, s) =>
            {
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
                GL.Begin(BeginMode.Triangles);
                {

                    GL.Vertex2(20, 20);
                    GL.Vertex2(100, 20);
                    GL.Vertex2(60, 80);
                }
                GL.End();
                //-----------------
                //render color
                GL.ColorMask(true, true, true, true);
                //where a 1 was not rendered
                GL.StencilFunc(StencilFunction.Equal, 1, 1);
                //keep the pixel
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                //draw  
                GL.Begin(BeginMode.Quads);
                {

                    GL.Color3(1f, 0, 0);
                    GL.Vertex2(5, 5);

                    GL.Color3(1f, 1, 0);
                    GL.Vertex2(100, 5);

                    GL.Color3(1f, 0, 1);
                    GL.Vertex2(100, 100);

                    GL.Color3(1f, 1, 1);
                    GL.Vertex2(5, 100);
                }
                GL.End();
                GL.Disable(EnableCap.StencilTest);
            });
            form.Show();
        }
    }
}