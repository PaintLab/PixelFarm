//MIT, 2016-present, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    abstract class SimpleRectTextureShader : ColorFillShaderBase
    {
        protected ShaderVtxAttrib3f a_position;
        protected ShaderVtxAttrib2f a_texCoord;

        protected ShaderUniformVar1 s_texture;
        protected static readonly ushort[] indices = new ushort[] { 0, 1, 2, 3 };

        public SimpleRectTextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {

        }

        //-----------------------------------------
        protected float _latestBmpW;
        protected float _latestBmpH;
        protected bool _latestBmpYFlipped;

        /// <summary>
        /// load glbmp before draw
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadGLBitmap(GLBitmap bmp)
        {

            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...

            TextureContainter container = _shareRes.LoadGLBitmap(bmp);
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(container.TextureUnitNo);


            _latestBmpW = bmp.Width;
            _latestBmpH = bmp.Height;
            _latestBmpYFlipped = bmp.IsYFlipped;
        }
        //internal void SetAssociatedTextureInfo(GLBitmap bmp)
        //{
        //    _latestBmpW = bmp.Width;
        //    _latestBmpH = bmp.Height;
        //    _latestBmpYFlipped = bmp.IsYFlipped;
        //}
        internal unsafe void UnsafeDrawSubImages(float* srcDestList, int arrLen, float scale)
        {
            //-------------------------------------------------------------------------------------
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------          
            float orgBmpW = _latestBmpW;
            float orgBmpH = _latestBmpH;
            for (int i = 0; i < arrLen;)
            {

                float srcLeft = srcDestList[i];
                float srcTop = srcDestList[i + 1];
                float srcW = srcDestList[i + 2];
                float srcH = srcDestList[i + 3];
                float targetLeft = srcDestList[i + 4];
                float targetTop = srcDestList[i + 5];

                i += 6;//***
                //-------------------------------
                float srcBottom = srcTop + srcH;
                float srcRight = srcLeft + srcW;


                unsafe
                {
                    if (!_latestBmpYFlipped)
                    {
                        float* imgVertices = stackalloc float[5 * 4];
                        {
                            imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)
                            imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                            //---------------------
                            imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                            imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcTop / orgBmpH; //texture coord 1  (left,top)

                            //---------------------
                            imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                            imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcBottom / orgBmpH; //texture coord 2  (right,bottom)

                            //---------------------
                            imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                            imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcTop / orgBmpH; //texture coord 3 (right,top)
                        }
                        a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                        a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                    }
                    else
                    {
                        float* imgVertices = stackalloc float[5 * 4];
                        {
                            imgVertices[0] = targetLeft;        /**/imgVertices[1] = targetTop;                   /**/imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                            imgVertices[3] = srcLeft / orgBmpW; /**/imgVertices[4] = srcTop / orgBmpH; /**/                               //texture coord 0 (left,top)

                            //---------------------
                            imgVertices[5] = targetLeft;        /**/imgVertices[6] = targetTop - (srcH * scale);  /**/imgVertices[7] = 0; //coord 1 (left,bottom)
                            imgVertices[8] = srcLeft / orgBmpW; /**/imgVertices[9] = srcBottom / orgBmpH;         /**/                     //texture coord 1 (left,bottom)

                            //---------------------
                            imgVertices[10] = targetLeft + (srcW * scale); /**/imgVertices[11] = targetTop;       /**/imgVertices[12] = 0; //coord 2 (right,top)
                            imgVertices[13] = srcRight / orgBmpW;          /**/imgVertices[14] = srcTop / orgBmpH;                         //texture coord 2 (right,top)

                            //---------------------
                            imgVertices[15] = targetLeft + (srcW * scale); /**/imgVertices[16] = targetTop - (srcH * scale);    /**/imgVertices[17] = 0; //coord 3 (right, bottom)
                            imgVertices[18] = srcRight / orgBmpW;          /**/imgVertices[19] = srcBottom / orgBmpH;               //texture coord 3  (right,bottom)
                        }
                        a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                        a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                    }
                }
                GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
            }
        }

        public void DrawWithVBO(TextureCoordVboBuilder vboBuilder)
        {
            float[] vboList = vboBuilder._buffer.UnsafeInternalArray;
            ushort[] indexList = vboBuilder._indexList.UnsafeInternalArray;

            SetCurrent();
            CheckViewMatrix();
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------       
            unsafe
            {
                fixed (float* imgVertices = &vboList[0])
                {
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }
            GL.DrawElements(BeginMode.TriangleStrip, vboBuilder._indexList.Count, DrawElementsType.UnsignedShort, indexList);
        }

        public void Render(GLBitmap bmp, float left, float top, float w, float h)
        {
            Render(bmp, left, top, w, h, bmp.IsYFlipped);
        }
        public void Render(GLBitmap bmp,
            float left_top_x, float left_top_y,
            float right_top_x, float right_top_y,
            float right_bottom_x, float right_bottom_y,
            float left_bottom_x, float left_bottom_y, bool flipY = false)

        {

            bool isFlipped = flipY;
            unsafe
            {
                //user's coord
                //(left,top) ----- (right,top)
                //  |                   |
                //  |                   |
                //  |                   |
                //(left,bottom) ---(right,bottom)

                // 
                //(0,1) ------------ (1,1)
                //  |                   |
                //  |   texture-img     |
                //  |                   |
                //(0,0) -------------(1,0)


                if (isFlipped)
                {
                    //since this is fliped in Y axis
                    //so we map 
                    //| user's coord    | texture-img |
                    //----------------------------------
                    //| left            | left
                    //| right           | right 
                    //----------------------------------
                    //| top             | bottom
                    //| bottom          | top
                    //----------------------------------

                    float* imgVertices = stackalloc float[5 * 4];
                    {

                        imgVertices[0] = left_top_x; imgVertices[1] = left_top_y; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                        //---------------------
                        imgVertices[5] = left_bottom_x; imgVertices[6] = left_bottom_y; imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                        //---------------------
                        imgVertices[10] = right_top_x; imgVertices[11] = right_top_y; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[15] = right_bottom_x; imgVertices[16] = right_bottom_y; imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {    //since this is NOT fliped in Y axis
                    //so we map 
                    //| user's coord    | texture-img |
                    //----------------------------------
                    //| left            | left
                    //| right           | right 
                    //----------------------------------
                    //| top             | top
                    //| bottom          | bottom
                    //----------------------------------
                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = left_top_x; imgVertices[1] = left_top_y; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = 0; imgVertices[4] = 1; //texture coord 0 (left,top)

                        //---------------------
                        imgVertices[5] = left_bottom_x; imgVertices[6] = left_bottom_y; imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = 0; imgVertices[9] = 0; //texture coord 1 (left,bottom)

                        //---------------------
                        imgVertices[10] = right_top_x; imgVertices[11] = right_top_y; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = 1; imgVertices[14] = 1; //texture coord 2 (right,top)

                        //---------------------
                        imgVertices[15] = right_bottom_x; imgVertices[16] = right_bottom_y; imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = 1; imgVertices[19] = 0; //texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...

            TextureContainter textureContainer = _shareRes.LoadGLBitmap(bmp);
            //GL.ActiveTexture(TextureUnit.Texture0);
            //GL.BindTexture(TextureTarget.Texture2D, textureId);
            //// Set the texture sampler to texture unit to 0     
            //s_texture.SetValue(0);

            s_texture.SetValue(textureContainer.TextureUnitNo);
            SetVarsBeforeRender();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }

        public void Render(GLBitmap glBmp, float left, float top, float w, float h, bool isFlipped = false)
        {
            SetCurrent();
            CheckViewMatrix();
            unsafe
            {
                //user's coord
                //(left,top) ----- (right,top)
                //  |                   |
                //  |                   |
                //  |                   |
                //(left,bottom) ---(right,bottom)

                // 
                //(0,1) ------------ (1,1)
                //  |                   |
                //  |   texture-img     |
                //  |                   |
                //(0,0) -------------(1,0)


                if (isFlipped)
                {
                    //since this is fliped in Y axis
                    //so we map 
                    //| user's coord    | texture-img |
                    //----------------------------------
                    //| left            | left
                    //| right           | right 
                    //----------------------------------
                    //| top             | bottom
                    //| bottom          | top
                    //----------------------------------

                    float* imgVertices = stackalloc float[5 * 4];
                    {

                        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                        //---------------------
                        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                        //---------------------
                        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {    //since this is NOT fliped in Y axis
                    //so we map 
                    //| user's coord    | texture-img |
                    //----------------------------------
                    //| left            | left
                    //| right           | right 
                    //----------------------------------
                    //| top             | top
                    //| bottom          | bottom
                    //----------------------------------
                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = 0; imgVertices[4] = 1; //texture coord 0 (left,top)

                        //---------------------
                        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = 0; imgVertices[9] = 0; //texture coord 1 (left,bottom)

                        //---------------------
                        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = 1; imgVertices[14] = 1; //texture coord 2 (right,top)

                        //---------------------
                        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = 1; imgVertices[19] = 0; //texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            TextureContainter container = _shareRes.LoadGLBitmap(glBmp);

            //GL.ActiveTexture(TextureUnit.Texture0);
            //GL.BindTexture(TextureTarget.Texture2D, textureId);
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(container.TextureUnitNo);

            SetVarsBeforeRender();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
        protected bool BuildProgram(string vs, string fs)
        {
            //NOTE: during development, 
            //new shader source may not recompile if you don't clear cache or disable cache feature
            //like...
            //EnableProgramBinaryCache = false;

            //---------------------
            if (!LoadCompiledShader())
            {
                if (!_shaderProgram.Build(vs, fs))
                {
                    return false;
                }
                SaveCompiledShader();
            }

            //-----------------------
            a_position = _shaderProgram.GetAttrV3f("a_position");
            a_texCoord = _shaderProgram.GetAttrV2f("a_texCoord");
            u_matrix = _shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_orthov_offset = _shaderProgram.GetUniform2("u_ortho_offset");
            s_texture = _shaderProgram.GetUniform1("s_texture");
            OnProgramBuilt();
            return true;
        }
        protected virtual void SetVarsBeforeRender()
        {
        }
        protected virtual void OnProgramBuilt()
        {
        }
    }

    /// <summary>
    /// for 32 bits texture/image  in BGR format (Windows GDI,with no alpha)d, we can specific A component later
    /// </summary>
    sealed class BGRImageTextureShader : SimpleRectTextureShader
    {
        ShaderUniformVar1 u_alpha;//** alpha component to apply with original img
        public BGRImageTextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            Alpha = 1f;//default 
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix;

                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //in fs, angle on windows 
            //we need to switch color component ***
            //because we store value in memory as BGRA
            //and gl expect input in RGBA
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      uniform float u_alpha;
                      void main()
                      {
                         vec4 c = texture2D(s_texture, v_texCoord);                            
                         gl_FragColor = vec4(c[2],c[1],c[0],u_alpha);
                      }
                ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            u_alpha = _shaderProgram.GetUniform1("u_alpha");
        }
        protected override void SetVarsBeforeRender()
        {
            u_alpha.SetValue(Alpha);
        }

        float _alpha;
        /// <summary>
        /// 00-1.0f
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set
            {
                //clamp 0-1
                if (_alpha < 0)
                {
                    _alpha = 0;
                }
                else if (_alpha > 1)
                {
                    _alpha = 1;
                }
                else
                {
                    _alpha = value;
                }
            }
        }
    }


    /// <summary>
    /// for 32 bits texture/image in BGRA format (eg. Windows version of CpuBlit's MemBitmap)
    /// </summary>
    sealed class BGRAImageTextureShader : SimpleRectTextureShader
    {

        public BGRAImageTextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix;                  
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //in fs, angle on windows 
            //we need to switch color component ***
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
            BuildProgram(vs, fs);
        }
    }



    /// <summary>
    /// for 32 bits texture/image  in RGBA format
    /// </summary>
    sealed class RGBATextureShader : SimpleRectTextureShader
    {
        public RGBATextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix; 
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //this case we not need to swap 
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      void main()
                      {
                         gl_FragColor = texture2D(s_texture, v_texCoord);        
                      }
                ";
            BuildProgram(vs, fs);
        }
    }

    /// <summary>
    /// for 32 bits texture/image  in RGB format
    /// </summary>
    sealed class RGBTextureShader : SimpleRectTextureShader
    {
        public RGBTextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix; 
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //this case we not need to swap 
            //ignore alpha from original src
            //TODO: review this again
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      void main()
                      { 
                         vec4 t_color= texture2D(s_texture, v_texCoord);                           
                         gl_FragColor = vec4(t_color.r,t_color.g,t_color.b,1.0);
                      }
                ";
            BuildProgram(vs, fs);
        }
    }


    sealed class GlyphImageStecilShader : SimpleRectTextureShader
    {

        //we share the texture with the subpixel-lcd effect texture
        //which is 32 bits bitmap.
        //backgrond color= black,
        //font =white + subpixel value

        //see the glyph texture example at https://github.com/PaintLab/PixelFarm/issues/16  

        ShaderUniformVar4 _d_color;
        PixelFarm.Drawing.Color _fillColor;
        bool _fillColorChanged;

        public GlyphImageStecilShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix; 
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //in fs, angle on windows 
            //we need to switch color component
            //because we store value in memory as BGRA
            //and gl expect input in RGBA

            //use vector's member-wise multiplication ***
            string fs = @"
                      precision mediump float;
                      varying vec2 v_texCoord;
                      uniform sampler2D s_texture;
                      uniform vec4 d_color;
                      void main()
                      {
                         gl_FragColor = vec4(1.0,1.0,1.0,texture2D(s_texture, v_texCoord)[1]) * d_color;                        
                      }
                ";
            BuildProgram(vs, fs);
        }
        public void SetColor(PixelFarm.Drawing.Color c)
        {
            if (_fillColor != c)
            {
                _fillColor = c;
                _fillColorChanged = true;
            }
        }
        protected override void OnProgramBuilt()
        {
            _d_color = _shaderProgram.GetUniform4("d_color");
        }
        protected override void SetVarsBeforeRender()
        {
            if (_fillColorChanged)
            {
                _d_color.SetValue(
                    _fillColor.R / 255f,
                    _fillColor.G / 255f,
                    _fillColor.B / 255f,
                    _fillColor.A / 255f);
                _fillColorChanged = false;
            }
        }
    }

    /// <summary>
    /// texture-based, lcd-subpix rendering shader for any background
    /// </summary>
    sealed class LcdSubPixShader : SimpleRectTextureShader
    {
        //this shader is designed for subpixel shader
        //for transparent background        

        ShaderUniformVar2 _offset;
        ShaderUniformVar3 _u_compo3;
        ShaderUniformVar4 _u_color;

        bool _hasSomeOffset;

        float _color_a = 1f;
        float _color_r;
        float _color_g;
        float _color_b;

        public enum ColorCompo : byte
        {
            C0,
            C1,
            C2,
            C_ALL,
        }

        public LcdSubPixShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;

                uniform vec2 u_ortho_offset;
                uniform vec2 u_offset;                
                uniform mat4 u_mvpMatrix; 

                varying vec2 v_texCoord;
                void main()
                {                      
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_offset+u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";
            //in fs, angle on windows 
            //we need to switch color component
            //because we store value in memory as BGRA
            //and gl expect input in RGBA


            //***  ON one of my machines this is OK ***
            //but on some of my machine this is error => 'Index Expression must be constant' ***
            //string fs = @"
            //          precision mediump float; 
            //          uniform sampler2D s_texture;
            //          uniform int c_compo;
            //          uniform vec4 d_color; 
            //          varying vec2 v_texCoord; 
            //          void main()
            //          {   
            //             vec4 c= texture2D(s_texture,v_texCoord);
            //             gl_FragColor = vec4(d_color[0],d_color[1],d_color[2],(c[c_compo]* d_color[3])); 
            //          }
            //    ";

            //SO I create another version here
            //string fs = @"
            //          precision mediump float; 
            //          uniform sampler2D s_texture;
            //          uniform vec3 c_compo3;
            //          uniform vec4 d_color; 
            //          varying vec2 v_texCoord; 
            //          void main()
            //          {   
            //             vec4 c= texture2D(s_texture,v_texCoord);
            //             gl_FragColor = vec4(d_color[0],d_color[1],d_color[2],
            //                                ((c[0] * c_compo3[0] + c[1] * c_compo3[1] +  c[2] * c_compo3[2])* d_color[3])); 
            //          }
            //    ";


            //and an more compact one here ... (component-wise vector multiplication)

            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture;
                      uniform vec3 u_compo3;
                      uniform vec4 u_color; 
                      varying vec2 v_texCoord; 
                      void main()
                      {   
                         vec3 c= vec3(texture2D(s_texture,v_texCoord)) * u_compo3;
                         gl_FragColor = vec4(u_color[0],u_color[1],u_color[2],
                                            ((c[0]+c[1]+c[2])* u_color[3])); 
                      }
                ";


            //old version
            //string fs = @"
            //          precision mediump float; 
            //          uniform sampler2D s_texture;
            //          uniform int isBigEndian;
            //          uniform int c_compo;
            //          uniform vec4 d_color; 
            //          varying vec2 v_texCoord; 
            //          void main()
            //          {   
            //             vec4 c= texture2D(s_texture,v_texCoord);    
            //             if(c_compo==0){ 
            //                gl_FragColor = vec4(0,0,d_color[2],(c[0]* d_color[3]) );
            //             }else if(c_compo==1){ 
            //                gl_FragColor = vec4(0,d_color[1],0,(c[1]* d_color[3]) );
            //             }else if(c_compo==2){ 
            //                gl_FragColor = vec4(d_color[0],0,0,(c[2]* d_color[3]) );                         
            //             }else if(c_compo==4){
            //                gl_FragColor =vec4(d_color[0],d_color[1],d_color[2],(c[1]* d_color[3]) );
            //             }else if(c_compo==5){
            //                gl_FragColor =vec4(c[2],c[1], c[0],c[3]);
            //             }
            //          }
            //    ";
            BuildProgram(vs, fs);
        }
        public void SetColor(PixelFarm.Drawing.Color c)
        {
            _color_a = c.A / 255f;
            _color_r = c.R / 255f;
            _color_g = c.G / 255f;
            _color_b = c.B / 255f;
        }

        public void SetCompo(ColorCompo compo)
        {
            switch (compo)
            {
                default: throw new System.NotSupportedException();
                case ColorCompo.C0:
                    _u_color.SetValue(0, 0, _color_b, _color_a);
                    _u_compo3.SetValue(1f, 0f, 0f);
                    break;
                case ColorCompo.C1:
                    _u_color.SetValue(0, _color_g, 0, _color_a);
                    _u_compo3.SetValue(0f, 1f, 0f);
                    break;
                case ColorCompo.C2:
                    _u_color.SetValue(_color_r, 0, 0, _color_a);
                    _u_compo3.SetValue(0f, 0f, 1f);
                    break;
                case ColorCompo.C_ALL:
                    _u_color.SetValue(_color_r, _color_g, _color_b, _color_a);
                    _u_compo3.SetValue(1 / 3f, 1 / 3f, 1 / 3f);
                    break;
            }
        }

        protected override void OnProgramBuilt()
        {
            _u_color = _shaderProgram.GetUniform4("u_color");
            _u_compo3 = _shaderProgram.GetUniform3("u_compo3");
            _offset = _shaderProgram.GetUniform2("u_offset");
        }
        protected override void SetVarsBeforeRender() { }


        public void NewDrawSubImage4FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
        {
            SetCurrent();
            CheckViewMatrix();
            LoadGLBitmap(glBmp);
            //
            _offset.SetValue(x, y);
            _hasSomeOffset = true;
            //-------------------------------------------------------------------------------------          
            //each vertex has 5 element (x,y,z,u,v), //interleave data
            //(x,y,z) 3d location 
            //(u,v) 2d texture coord  

            vbo.Bind();
            a_position.LoadLatest(5, 0);
            a_texCoord.LoadLatest(5, 3 * 4);

            //*** 

            //_isBigEndian.SetValue(IsBigEndian);

            //version 1
            //0. B , yellow  result
            GL.ColorMask(false, false, true, false);
            SetCompo(ColorCompo.C0);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            //1. G , magenta result
            GL.ColorMask(false, true, false, false);
            SetCompo(ColorCompo.C1);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            //2. R , cyan result 
            GL.ColorMask(true, false, false, false);//                  
            SetCompo(ColorCompo.C2);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            //restore
            GL.ColorMask(true, true, true, true);

            vbo.UnBind();

        }
        public void NewDrawSubImageStencilFromVBO(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
        {
            SetCurrent();
            CheckViewMatrix();
            LoadGLBitmap(glBmp);
            //
            _offset.SetValue(x, y);
            _hasSomeOffset = true;
            //-------------------------------------------------------------------------------------          
            //each vertex has 5 element (x,y,z,u,v), //interleave data
            //(x,y,z) 3d location 
            //(u,v) 2d texture coord  

            vbo.Bind();
            a_position.LoadLatest(5, 0);
            a_texCoord.LoadLatest(5, 3 * 4);
            SetCompo(ColorCompo.C_ALL);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            vbo.UnBind();

        }
        /// <summary>
        /// DrawElements, use vertex-buffer and index-list
        /// </summary>
        /// <param name="vboList"></param>
        /// <param name="indexList"></param>
        public void DrawSubImages(GLBitmap glBmp, TextureCoordVboBuilder vboBuilder)
        {
            SetCurrent();
            CheckViewMatrix();

            LoadGLBitmap(glBmp);

            if (_hasSomeOffset)
            {
                _offset.SetValue(0f, 0f);
                _hasSomeOffset = false;//reset
            }
            // ------------------------------------------------------------------------------------- 
            unsafe
            {
                float[] vboList = vboBuilder._buffer.UnsafeInternalArray; //***
                fixed (float* imgVertices = &vboList[0])
                {
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }

            //SHARED ARRAY 
            ushort[] indexList = vboBuilder._indexList.UnsafeInternalArray; //***
            int count1 = vboBuilder._indexList.Count; //***

#if DEBUG
            System.Diagnostics.Debug.WriteLine("lcd3steps:");
#endif

            //version 1
            //0. B , yellow  result
            GL.ColorMask(false, false, true, false);
            SetCompo(ColorCompo.C0);
            GL.DrawElements(BeginMode.TriangleStrip, count1, DrawElementsType.UnsignedShort, indexList);

            //1. G , magenta result
            GL.ColorMask(false, true, false, false);
            SetCompo(ColorCompo.C1);
            GL.DrawElements(BeginMode.TriangleStrip, count1, DrawElementsType.UnsignedShort, indexList);

            //2. R , cyan result 
            GL.ColorMask(true, false, false, false);//     
            SetCompo(ColorCompo.C2);
            GL.DrawElements(BeginMode.TriangleStrip, count1, DrawElementsType.UnsignedShort, indexList);

            //restore
            GL.ColorMask(true, true, true, true);
        }

        public void DrawSubImage(float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            SetCurrent();
            CheckViewMatrix();
            if (_hasSomeOffset)
            {
                _offset.SetValue(0f, 0f);
                _hasSomeOffset = false;//reset
            }

            //-------------------------------------------------------------------------------------          
            float orgBmpW = _latestBmpW;
            float orgBmpH = _latestBmpH;
            float scale = 1;

            //-------------------------------
            float srcBottom = srcTop + srcH;
            float srcRight = srcLeft + srcW;

            unsafe
            {
                if (!_latestBmpYFlipped)
                {

                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                        //---------------------
                        imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcTop / orgBmpH; //texture coord 1  (left,top)

                        //---------------------
                        imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcBottom / orgBmpH; //texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcTop / orgBmpH; //texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {
                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcTop / orgBmpH; //texture coord 0 (left,top)

                        //---------------------
                        imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcBottom / orgBmpH; //texture coord 1 (left,bottom)

                        //---------------------
                        imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcTop / orgBmpH; //texture coord 2 (right,top)

                        //---------------------
                        imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcBottom / orgBmpH; //texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }

            ////version 1
            ////0. B , yellow  result
            GL.ColorMask(false, false, true, false);
            SetCompo(ColorCompo.C0);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);

            ////1. G , magenta result
            GL.ColorMask(false, true, false, false);
            SetCompo(ColorCompo.C1);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);

            //2. R , cyan result 
            GL.ColorMask(true, false, false, false);//     
            SetCompo(ColorCompo.C2);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
            //restore
            GL.ColorMask(true, true, true, true);
        }



        public void DrawSubImageWithStencil(GLBitmap glBmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            SetCurrent();
            CheckViewMatrix();

            LoadGLBitmap(glBmp);

            if (_hasSomeOffset)
            {
                _offset.SetValue(0f, 0f);
                _hasSomeOffset = false;//reset
            }

            //-------------------------------------------------------------------------------------          
            float orgBmpW = _latestBmpW;
            float orgBmpH = _latestBmpH;
            float scale = 1;

            //-------------------------------
            float srcBottom = srcTop + srcH;
            float srcRight = srcLeft + srcW;

            unsafe
            {
                if (!_latestBmpYFlipped)
                {

                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                        //---------------------
                        imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcTop / orgBmpH; //texture coord 1  (left,top)

                        //---------------------
                        imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcBottom / orgBmpH; //texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcTop / orgBmpH; //texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {
                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcTop / orgBmpH; //texture coord 0 (left,top)

                        //---------------------
                        imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcBottom / orgBmpH; //texture coord 1 (left,bottom)

                        //---------------------
                        imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcTop / orgBmpH; //texture coord 2 (right,top)

                        //---------------------
                        imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcBottom / orgBmpH; //texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }

            SetCompo(ColorCompo.C_ALL);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }




    /// <summary>
    /// texture-based, lcd-subpix rendering shader for solid-color-background 
    /// </summary>
    sealed class LcdSubPixShaderForSolidBg : SimpleRectTextureShader
    {

        ShaderUniformVar2 _offset;
        ShaderUniformVar4 _u_color;
        ShaderUniformVar4 _u_bg;

        bool _hasSomeOffset;

        //-------
        PixelFarm.Drawing.Color _textColor;
        float _fillR, _fillG, _fillB, _fillA;
        bool _fillChanged;
        //-------

        PixelFarm.Drawing.Color _bgColor;
        float _bgR, _bgG, _bgB;
        bool _bgChanged;

        public LcdSubPixShaderForSolidBg(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;

                uniform vec2 u_ortho_offset;
                uniform vec2 u_offset;                
                uniform mat4 u_mvpMatrix; 

                varying vec2 v_texCoord;
                void main()
                {                      
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_offset+u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";

            //the same concept as the 'LcdEffectSubPixelRenderingShader'
            //but this for solid color background.
            //since we know the background-color so we can calculate the result.
            //in this shader we call 1 DrawElements() call ( the original needs 3 calls)


            //description here...

            //1. we think a 'c' is an 'coverage area'.
            //if filling color (u_color) has alpha component, the we apply it to the 'c' value.
            //vec4 c = texture2D(s_texture, v_texCoord)*u_color[3]; 

            //2. we apply each color component with alpha blend equation per channel

            //equation, ch_x= (bg_x *(1-src_alpha_x) + src_x* src_alpha_x)       

            //gl_FragColor = vec4((u_bg[0] * (1.0 - c[0]) + u_color[0] * c[0]),
            //                    (u_bg[1] * (1.0 - c[1]) + u_color[1] * c[1]),
            //                    (u_bg[2] * (1.0 - c[2]) + u_color[2] * c[2]),
            //                    1.0);           

            //-----------
            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture;
                      uniform vec4 u_color; 
                      uniform vec4 u_bg;                    
                      varying vec2 v_texCoord; 
                      void main()
                      {   
                         vec4 c = texture2D(s_texture,v_texCoord)*u_color[3];   

                         gl_FragColor =vec4(u_bg[0]*(1.0-c[0]) + u_color[0]*c[0],
                                            u_bg[1]*(1.0-c[1]) + u_color[1]*c[1],
                                            u_bg[2]*(1.0-c[2]) + u_color[2]*c[2],
                                            1.0);
                      }
                ";
            BuildProgram(vs, fs);

            SetBackgroundColor(PixelFarm.Drawing.Color.White);//default
            SetTextColor(PixelFarm.Drawing.Color.Black);//default
        }

        protected override void OnProgramBuilt()
        {
            _u_color = _shaderProgram.GetUniform4("u_color");
            _u_bg = _shaderProgram.GetUniform4("u_bg");
            _offset = _shaderProgram.GetUniform2("u_offset");
        }

        public void SetBackgroundColor(PixelFarm.Drawing.Color c)
        {
            if (_bgColor == c)
            {
                //no change
                return;
            }

            _bgColor = c;
            _bgChanged = true;

            _bgR = c.R / 255f;
            _bgG = c.G / 255f;
            _bgB = c.B / 255f;

            //the background color must be opaque color
            //(no alpha channel for bg color)
        }
        public void SetTextColor(PixelFarm.Drawing.Color c)
        {
            if (_textColor == c)
            {
                //no change
                return;
            }

            _textColor = c;
            _fillChanged = true;

            _fillR = c.R / 255f;
            _fillG = c.G / 255f;
            _fillB = c.B / 255f;
            _fillA = c.A / 255f;
        }
        protected override void SetVarsBeforeRender() { }

        public void DrawSubImageWithStencil(GLBitmap glBmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            SetCurrent();
            CheckViewMatrix();

            LoadGLBitmap(glBmp);

            if (_hasSomeOffset)
            {
                _offset.SetValue(0f, 0f);
                _hasSomeOffset = false;//reset
            }

            //-------------------------------------------------------------------------------------          
            float orgBmpW = _latestBmpW;
            float orgBmpH = _latestBmpH;
            float scale = 1;

            //-------------------------------
            float srcBottom = srcTop + srcH;
            float srcRight = srcLeft + srcW;

            unsafe
            {
                if (!_latestBmpYFlipped)
                {

                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                        //---------------------
                        imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcTop / orgBmpH; //texture coord 1  (left,top)

                        //---------------------
                        imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcBottom / orgBmpH; //texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcTop / orgBmpH; //texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {
                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = targetLeft; imgVertices[1] = targetTop; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcTop / orgBmpH; //texture coord 0 (left,top)

                        //---------------------
                        imgVertices[5] = targetLeft; imgVertices[6] = targetTop - (srcH * scale); imgVertices[7] = 0; //coord 1 (left,bottom)
                        imgVertices[8] = srcLeft / orgBmpW; imgVertices[9] = srcBottom / orgBmpH; //texture coord 1 (left,bottom)

                        //---------------------
                        imgVertices[10] = targetLeft + (srcW * scale); imgVertices[11] = targetTop; imgVertices[12] = 0; //coord 2 (right,top)
                        imgVertices[13] = srcRight / orgBmpW; imgVertices[14] = srcTop / orgBmpH; //texture coord 2 (right,top)

                        //---------------------
                        imgVertices[15] = targetLeft + (srcW * scale); imgVertices[16] = targetTop - (srcH * scale); imgVertices[17] = 0; //coord 3 (right, bottom)
                        imgVertices[18] = srcRight / orgBmpW; imgVertices[19] = srcBottom / orgBmpH; //texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }



            if (_fillChanged)
            {
                _u_color.SetValue(_fillR, _fillG, _fillB, _fillA);
                _fillChanged = false;
            }

            if (_bgChanged)
            {
                _u_bg.SetValue(_bgR, _bgG, _bgB, 1.0f);
                _bgChanged = false;
            }

            //in this shader,
            //we will calculate final blend value manually
            //so temp disable it.
            GL.Disable(EnableCap.Blend);

            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);

            GL.Enable(EnableCap.Blend);//restore
        }
    }


    /// <summary>
    ///texture-based, lcd-subpix rendering shader for word-strip creation on solid-color-background
    /// </summary>
    sealed class LcdSubPixShaderForWordStripCreation : SimpleRectTextureShader
    {
        //this shader is designed for subpixel shader
        //for transparent background        

        ShaderUniformVar2 _offset;
        public LcdSubPixShaderForWordStripCreation(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;

                uniform vec2 u_ortho_offset;
                uniform vec2 u_offset;                
                uniform mat4 u_mvpMatrix; 

                varying vec2 v_texCoord;
                void main()
                {                      
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_offset+u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                 }	 
                ";

            //
            //gl_FragColor = vec4(u_bg[0] * (1.0 - c[0]) + u_color[0] * c[0],
            //                    u_bg[1] * (1.0 - c[1]) + u_color[1] * c[1],
            //                    u_bg[2] * (1.0 - c[2]) + u_color[2] * c[2],
            //                    1.0);

            //-----------
            //please note that 
            //1. we swap color channel R and B from input texture
            //2. in this version, not support overlapped glyphs
            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture;
                      varying vec2 v_texCoord; 
                      void main()
                      {   
                            vec4 c = texture2D(s_texture,v_texCoord);  
                            gl_FragColor = vec4(c[2],c[1],c[0],c[3]);
                      }
                ";

            BuildProgram(vs, fs);
        }

        protected override void OnProgramBuilt()
        {
            _offset = _shaderProgram.GetUniform2("u_offset");
        }
        protected override void SetVarsBeforeRender() { }
        public void NewDrawSubImage4FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
        {
            SetCurrent();
            CheckViewMatrix();
            LoadGLBitmap(glBmp);
            //
            _offset.SetValue(x, y);

            //-------------------------------------------------------------------------------------          
            //each vertex has 5 element (x,y,z,u,v), //interleave data
            //(x,y,z) 3d location 
            //(u,v) 2d texture coord  

            vbo.Bind();
            a_position.LoadLatest(5, 0);
            a_texCoord.LoadLatest(5, 3 * 4);

            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            vbo.UnBind();
        }

    }



    //--------------------------------------------------------
    static class SimpleRectTextureShaderExtensions
    {

        public static void DrawSubImage(this SimpleRectTextureShader shader, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            unsafe
            {
                float* srcDestList = stackalloc float[6];
                {
                    srcDestList[0] = srcLeft;
                    srcDestList[1] = srcTop;
                    srcDestList[2] = srcW;
                    srcDestList[3] = srcH;
                    srcDestList[4] = targetLeft;
                    srcDestList[5] = targetTop;
                }
                shader.UnsafeDrawSubImages(srcDestList, 6, 1);
            }
        }

        public static void DrawSubImage(this SimpleRectTextureShader shader, GLBitmap bmp,
            float srcLeft, float srcTop,
            float srcW, float srcH,
            float targetLeft, float targetTop,
            float scale = 1)
        {

            unsafe
            {
                float* srcDestList = stackalloc float[6];
                {
                    srcDestList[0] = srcLeft;
                    srcDestList[1] = srcTop;
                    srcDestList[2] = srcW;
                    srcDestList[3] = srcH;
                    srcDestList[4] = targetLeft;
                    srcDestList[5] = targetTop;
                }
                shader.LoadGLBitmap(bmp);
                shader.UnsafeDrawSubImages(srcDestList, 6, scale);
            }
        }
        public static void DrawSubImages(this SimpleRectTextureShader shader, float[] srcDestList, float scale)
        {
            unsafe
            {
                fixed (float* head = &srcDestList[0])
                {
                    shader.UnsafeDrawSubImages(head, srcDestList.Length, scale);
                }
            }
        }
        public static void DrawSubImages(this SimpleRectTextureShader shader, GLBitmap bmp, float[] srcDestList, float scale)
        {
            shader.LoadGLBitmap(bmp);
            shader.DrawSubImages(srcDestList, scale);
        }
    }

}