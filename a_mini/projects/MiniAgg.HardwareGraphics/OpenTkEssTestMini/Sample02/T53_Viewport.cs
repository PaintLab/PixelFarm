
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
    [Info(OrderCode = "53")]
    [Info("T53_Viewport")]
    public class T53_Viewport : PrebuiltGLControlDemoBase
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();

        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position;
            attribute vec4 a_color; 
            uniform mat4 u_mvpMatrix;

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
            int mmm = GL.GetUniformLocation(shaderProgram.ProgramId, "u_mvpMatrix");
            uni_matrix = shaderProgram.GetUniform("u_mvpMatrix");

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);

            //setup viewport size
            int max = Math.Max(this.Width, this.Height);
            //square viewport
            GL.Viewport(0, 0, max, max);

            //GL.Viewport(0, 0, max, max);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, max, 0, max, 0.0, 100.0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity(); 
            //create othogonic viewport for gl control 

            MyMat4 orthoView = MyMat4.ortho(0, max, 0, max, 0, 100);
            ////MyMat4 modelMat = MyMat4.translate(new Vector3(0f, 0f, -2f)) *
            ////               MyMat4.rotate(mRotation, new Vector3(1, 0, 1));

            //MyMat4 viewMatrix = MyMat4.GetIdentityMat();

            //MyMat4 mvpMatrix = orthoView * viewMatrix * modelMat;

            //// Load the matrices
            //glUniformMatrix4fv(mMVPMatrixLoc, 1, GL_FALSE, mvpMatrix.data);
            GL.UniformMatrix4(uni_matrix.location, 1, false, orthoView.data);
        }
        protected override void DemoClosing()
        {
            shaderProgram.DeleteMe();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //------------------------------------------------------------------------------------------------


            float[] vertices =
            {
                     0.0f,  0.5f, //2d coord
                     1, 0, 0, 0.1f,//r
                    -0.5f, -0.5f,  //2d coord
                     0,1,0,0.1f,//g
                     0.5f, -0.5f,  //2d corrd
                     0,0,1,0.1f, //b
            };

            //---------------------------------------------------------


            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaderProgram.UseProgram();


            //triangle shape
            a_position.LoadV2f(vertices, 6, 0);
            a_color.LoadV3f(vertices, 6, 2);
            GL.DrawArrays(BeginMode.Triangles, 0, 3);
            //---------------------------------------------------------
            //rect shape
            float[] quadVertices = Create2dQuad(0.2f, 0.2f, 0.5f, 0.5f);
            a_position.LoadV2f(quadVertices, 6, 0);
            a_color.LoadV3f(quadVertices, 6, 2);

            GL.DrawArrays(BeginMode.Triangles, 0, 6);
            //---------------------------------------------------------
            miniGLControl.SwapBuffers();
        }
        static float[] Create2dQuad(float x, float y, float w, float h)
        {

            float[] vertices = new float[]{
                x, y,
                1,0,0,1,
                
                x+w,y,
                1,0,0,1,
                
                x+w,y-h,
                1,0,0,1,

                x+w,y-h,
                1,0,0,1,

                x, y - h, 
                1,0,0,1,

                x, y,
                1,0,0,1
            };
            return vertices;
        }
        //-------------------------------
        ShaderAttribute a_position;
        ShaderAttribute a_color;
        ShaderUniformVar uni_matrix;
    }


}
