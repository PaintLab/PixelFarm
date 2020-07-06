//MIT, 2016-present, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{

    sealed class SolidColorFillShader : ColorFillShaderBase
    {
        ShaderVtxAttrib2f a_position;
        ShaderUniformVar4 u_solidColor;
        Drawing.Color _fillColor; //latest fill color

        public SolidColorFillShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //NOTE: during development, 
            //new shader source may not recompile if you don't clear cache or disable cache feature
            //like...
            //EnableProgramBinaryCache = false;

            if (!LoadCompiledShader())
            {

                //vertex shader source
                string vs = @"        
                    attribute vec2 a_position; 
                    uniform vec2 u_ortho_offset;
                    uniform mat4 u_mvpMatrix; 
                    void main()
                    {
                        gl_Position = u_mvpMatrix * vec4(u_ortho_offset+ a_position,0,1); 
                    }
                ";

                //fragment source
                string fs = @"
                    precision mediump float;
                    uniform vec4 u_solidColor;
                    void main()
                    {
                        gl_FragColor = u_solidColor;
                    }
                ";

                if (!_shaderProgram.Build(vs, fs))
                {
                    throw new NotSupportedException();
                }
                //
                SaveCompiledShader();
            }

            a_position = _shaderProgram.GetAttrV2f("a_position");
            u_ortho_offset = _shaderProgram.GetUniform2("u_ortho_offset");
            u_matrix = _shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = _shaderProgram.GetUniform4("u_solidColor");
        }
        void SetColor(Drawing.Color c)
        {
            if (_fillColor != c)
            {
                _fillColor = c;
                u_solidColor.SetValue(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
            }
        }
        public void FillTriangleStripWithVertexBuffer(float[] linesBuffer, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            SetColor(color);
            a_position.LoadPureV2f(linesBuffer);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, nelements);
        }
        //-------------------------------------------- 
        public void FillTriangles(float[] polygon2dVertices, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------  
            SetColor(color);

            a_position.LoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public void FillTriangles(float[] polygon2dVertices, ushort[] indices, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------  

            SetColor(color);
            a_position.LoadPureV2f(polygon2dVertices);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedShort, indices);
        }

        public void FillTriangles(int first, int count, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------    
            SetColor(color);
            //vbo.Bind();
            a_position.LoadLatest();
            //GL.DrawElements(BeginMode.Triangles, nelements, DrawElementsType.UnsignedShort, 0);
            GL.DrawArrays(BeginMode.Triangles, first, count);
            //vbo.UnBind(); //important, call unbind after finish call.
        }
        //public void FillTriangles(VBOPart vboPart, Drawing.Color color)
        //{
        //    SetCurrent();
        //    CheckViewMatrix();
        //    //--------------------------------------------  
        //    u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);

        //    //--------------------------------------------  
        //    //note (A):
        //    //from https://www.khronos.org/registry/OpenGL-Refpages/es2.0/xhtml/glVertexAttribPointer.xml
        //    //... If a non-zero named buffer object is bound to the GL_ARRAY_BUFFER target (see glBindBuffer)
        //    //while a generic vertex attribute array is specified,
        //    //pointer is treated as **a byte offset** into the buffer object's data store. 

        //    vboPart.vbo.Bind();
        //    a_position.LoadLatest(vboPart.partRange.beginVertexAt * 4); //*4 => see note (A) above, so offset => beginVertexAt * sizeof(float)
        //    GL.DrawElements(BeginMode.Triangles,
        //        vboPart.partRange.elemCount,
        //        DrawElementsType.UnsignedShort,
        //        vboPart.partRange.beginElemIndexAt * 2);  //*2 => see note (A) above, so offset=> beginElemIndexAt *sizeof(ushort)
        //    vboPart.vbo.UnBind();

        //}

        public unsafe void DrawLineLoopWithVertexBuffer(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------
            SetColor(color);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.LineLoop, 0, nelements);
        }
        public unsafe void FillTriangleFan(float* polygon2dVertices, int nelements, Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            SetColor(color);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.TriangleFan, 0, nelements);
        }
        public void DrawLine(float x1, float y1, float x2, float y2, PixelFarm.Drawing.Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            SetColor(color);
            unsafe
            {
                float* vtx = stackalloc float[] { x1, y1, x2, y2 };
                a_position.UnsafeLoadPureV2f(vtx);
            }
            GL.DrawArrays(BeginMode.LineStrip, 0, 2);
        }
    }
}