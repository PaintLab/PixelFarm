//MIT 2016, WinterDev

using System;
using PixelFarm.Agg;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BasicFillShader
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_color;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar4 u_solidColor;
        MyMat4 orthoView;
        public BasicFillShader()
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position;
            attribute vec4 a_color; 

            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;              

            varying vec4 v_color;
 
            void main()
            {
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                if(u_useSolidColor !=0)
                {
                    v_color= u_solidColor;
                }
                else
                {
                    v_color = a_color;
                }
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


            a_position = shaderProgram.GetVtxAttrib("a_position");
            a_color = shaderProgram.GetVtxAttrib("a_color");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
        }
        public MyMat4 OrthoView
        {
            get { return orthoView; }
            set { orthoView = value; }
        }
        public void FillTrianglesWithVertexBuffer(float[] linesBuffer, int nelements, PixelFarm.Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            u_matrix.SetData(orthoView.data);
            a_position.LoadV2f(linesBuffer, 2, 0);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public void FillTriangleStripWithVertexBuffer(float[] linesBuffer, int nelements, PixelFarm.Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            u_matrix.SetData(orthoView.data);
            a_position.LoadV2f(linesBuffer, 2, 0);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, nelements);
        }
        public unsafe void FillTriangles(float* polygon2dVertices, int nelements, PixelFarm.Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(polygon2dVertices, 2, 0);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public unsafe void DrawLineLoopWithVertexBuffer(float* polygon2dVertices, int nelements, PixelFarm.Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(polygon2dVertices, 2, 0);
            GL.DrawArrays(BeginMode.LineLoop, 0, nelements);
        }
        public unsafe void FillTriangleFan(float* polygon2dVertices, int nelements, PixelFarm.Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadV2f(polygon2dVertices, 2, 0);
            GL.DrawArrays(BeginMode.TriangleFan, 0, nelements);
        }
    }
}