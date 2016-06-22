//MIT 2016, WinterDev 

using System;
using PixelFarm.Agg;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class RectFillShader
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib2f a_position;
        ShaderVtxAttrib4f a_color;
        ShaderUniformMatrix4 u_matrix;
        MyMat4 orthoView;
        public RectFillShader()
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
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1); 
                v_color= a_color;
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

            a_position = shaderProgram.GetAttrV2f("a_position");
            a_color = shaderProgram.GetAttrV4f("a_color");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
        }
        public MyMat4 OrthoView
        {
            get { return orthoView; }
            set { orthoView = value; }
        }
        public void Render(float[] v2fArray, float[] colors)
        {
            shaderProgram.UseProgram();
            u_matrix.SetData(orthoView.data);
            a_position.LoadPureV2f(v2fArray);
            a_color.LoadPureV4f(colors);
            GL.DrawArrays(BeginMode.Triangles, 0, 18);
        }
    }
}