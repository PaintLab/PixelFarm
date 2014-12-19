 
#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using Examples.Tutorial;
using Mini;

#endregion


namespace OpenTkEssTest
{
    [Info(OrderCode = "52")]
    [Info("T52_HelloTriangle2")]
    public class T52_HelloTriangle2 : PrebuiltGLControlDemoBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position;
            attribute vec4 a_color;
            
            varying vec4 v_color;
 
            void main()
            {
                gl_Position = vec4(a_position[0],a_position[1],0,1);
                v_color = a_color;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color; 
                void main()
                {
                    gl_FragColor = v_color;
                }
            ";

            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }


            a_position = shaderProgram.GetAttribVar("a_position");
            a_color = shaderProgram.GetAttribVar("a_color");

            GL.ClearColor(0, 0, 0, 0);
        }
        protected override void DemoClosing()
        {
            shaderProgram.DeleteMe();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------
            int width = miniGLControl.Width;
            int height = miniGLControl.Height;

            float[] vertices =
            {
                     0.0f,  0.5f, //2d coord
                     1, 0, 0, 1,//r
                    -0.5f, -0.5f,  //2d coord
                     0,1,0,1,//g
                     0.5f, -0.5f,  //2d corrd
                     0,0,1,1, //b
            };

            GL.Viewport(0, 0, width, height);

            // Set the viewport
            //glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Clear the color buffer
            // glClear(GL_COLOR_BUFFER_BIT); 
            // Use the program object
            //glUseProgram(mProgram);

            shaderProgram.UseProgram();
            // Load the vertex data
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, vertices);
            //GL.EnableVertexAttribArray(0); 
            a_position.LoadV2f(vertices, 6, 0);
            a_color.LoadV3f(vertices, 6, 2);
            GL.DrawArrays(BeginMode.Triangles, 0, 3);
            //glDrawArrays(GL_TRIANGLES, 0, 3); 
            miniGLControl.SwapBuffers();

            
        }
        //-------------------------------
        ShaderAttribute a_position;
        ShaderAttribute a_color;

    }


}
