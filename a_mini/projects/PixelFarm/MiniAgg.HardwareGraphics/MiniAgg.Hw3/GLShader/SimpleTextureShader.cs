//MIT 2016, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class SimpleTextureShader
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_texCoord;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 s_texture;
        MyMat4 orthoView;
        ushort[] indices = new ushort[] { 0, 1, 2, 3 };
        public SimpleTextureShader()
        {
            InitShader();
        }
        bool InitShader()
        {
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform mat4 u_mvpMatrix; 
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* a_position;
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //in fs, angle on windows 
            //we need to switch color component
            //because we store value in memory as BGRA
            //and gl expect input in RGBA
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      void main()
                      {
                         vec4 c = texture2D(s_texture, v_texCoord);                            
                         gl_FragColor =  vec4(c[2],c[1],c[0],c[3]);
                      }
                ";
            //---------------------
            if (!shaderProgram.Build(vs, fs))
            {
                return false;
            }
            //-----------------------

            a_position = shaderProgram.GetVtxAttrib("a_position");
            a_texCoord = shaderProgram.GetVtxAttrib("a_texCoord");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            s_texture = shaderProgram.GetUniform1("s_texture");
            return true;
        }
        public MyMat4 OrthoView
        {
            get { return orthoView; }
            set { orthoView = value; }
        }

        public void Render(GLBitmap bmp, float left, float top, float w, float h)
        {
            float[] imgVertices = new float[]
            {
                left, top,0, //coord 0
                0,0,      //texture 0
                //---------------------
                left,top-h,0, //coord 1
                0,1,    //texture 1
                //---------------------
                left+w,top,0, //coord 2
                1,0,
                //---------------------
                left+w, top -h,0, //corrd3
                1,1
            };
            shaderProgram.UseProgram();
            unsafe
            {
                fixed (float* imgvH = &imgVertices[0])
                {
                    a_position.UnsafeSubLoad3f(imgvH, 5);
                    a_texCoord.UnsafeSubLoad2f((imgvH + 3), 5);
                }
            }
            u_matrix.SetData(orthoView.data);
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.TextureId);
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(0);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }
}