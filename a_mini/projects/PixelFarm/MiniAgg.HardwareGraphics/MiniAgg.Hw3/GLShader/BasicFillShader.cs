//MIT 2016, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BasicFillShader
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib2f a_position;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar4 u_solidColor;
        MyMat4 orthoView;
        public BasicFillShader()
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position; 
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor; 
            varying vec4 v_color;
 
            void main()
            {
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                v_color= u_solidColor;
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
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
        }
        public MyMat4 OrthoView
        {
            get { return orthoView; }
            set { orthoView = value; }
        }
        public void FillTrianglesWithVertexBuffer(float[] linesBuffer, int nelements, Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_solidColor.SetValue(
                 color.R / 255f,
                 color.G / 255f,
                 color.B / 255f,
                 color.A / 255f);
            u_matrix.SetData(orthoView.data);
            a_position.LoadPureV2f(linesBuffer);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public void FillTriangleStripWithVertexBuffer(float[] linesBuffer, int nelements, Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            u_matrix.SetData(orthoView.data);
            a_position.LoadPureV2f(linesBuffer);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, nelements);
        }
        public unsafe void FillTriangles(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            u_matrix.SetData(orthoView.data);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public unsafe void FillTriangles(float[] polygon2dVertices, int nelements, Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            u_matrix.SetData(orthoView.data);
            a_position.LoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public unsafe void DrawLineLoopWithVertexBuffer(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.LineLoop, 0, nelements);
        }
        public unsafe void FillTriangleFan(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            shaderProgram.UseProgram();
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.TriangleFan, 0, nelements);
        }
    }
}