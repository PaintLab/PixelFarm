//MIT 2016, WinterDev


using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class SimpleTextureShader : ShaderBase
    {
        ShaderVtxAttrib3f a_position;
        ShaderVtxAttrib2f a_texCoord;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 s_texture;
        static readonly ushort[] indices = new ushort[] { 0, 1, 2, 3 };
        public SimpleTextureShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
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
                return;
            }
            //-----------------------

            a_position = shaderProgram.GetAttrV3f("a_position");
            a_texCoord = shaderProgram.GetAttrV2f("a_texCoord");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            s_texture = shaderProgram.GetUniform1("s_texture");
        }


        int orthoviewVersion = -1;
        void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _canvasShareResource.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_canvasShareResource.OrthoView.data);
            }
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
            unsafe
            {
                fixed (float* imgvH = &imgVertices[0])
                {
                    a_position.UnsafeLoadMixedV3f(imgvH, 5);
                    a_texCoord.UnsafeLoadMixedV2f((imgvH + 3), 5);
                }
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.TextureId);
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(0);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }
}