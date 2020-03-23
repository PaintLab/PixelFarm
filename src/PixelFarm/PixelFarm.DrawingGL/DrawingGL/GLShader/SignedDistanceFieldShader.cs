//MIT, 2016-present, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    sealed class SingleChannelSdf : SimpleRectTextureShader
    {
        //note not correct
        //TODO: fix 

        ShaderUniformVar4 _u_color;
        ShaderUniformVar1 _u_buffer;
        ShaderUniformVar1 _u_gamma;

        PixelFarm.Drawing.Color _color;
        bool _colorChanged;
        bool _initGammaAndBuffer;

        public SingleChannelSdf(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //credit: https://www.mapbox.com/blog/text-signed-distance-fields/
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform mat4 u_mvpMatrix;  
                uniform vec2 u_ortho_offset;
                varying vec2 v_texCoord;  
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord; 
                 }	 
                ";
            string fs = @"
                precision mediump float;

                uniform sampler2D s_texture;
                uniform vec4 u_color;
                uniform float u_buffer;
                uniform float u_gamma;

                varying vec2 v_texCoord;

                void main() {
                    float dist = texture2D(s_texture, v_texCoord).r;
                    float alpha = smoothstep(u_buffer - u_gamma, u_buffer + u_gamma, dist);
                    gl_FragColor = vec4(u_color.rgb, alpha * u_color.a); 
                } 
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _u_color = _shaderProgram.GetUniform4("u_color");
            _u_buffer = _shaderProgram.GetUniform1("u_buffer");
            _u_gamma = _shaderProgram.GetUniform1("u_gamma");
        }
        protected override void SetVarsBeforeRender()
        {

            if (_colorChanged)
            {
                _u_color.SetValue(
                     _color.R / 255f,
                     _color.G / 255f,
                     _color.B / 255f,
                     _color.A / 255f);
                _colorChanged = false;//reset
            }

            if (!_initGammaAndBuffer)
            {
                _u_buffer.SetValue(192f / 256f);
                _u_gamma.SetValue(1f);
                _initGammaAndBuffer = true;
            }
        }
        public void SetColor(PixelFarm.Drawing.Color c)
        {
            if (_color != c)
            {
                _color = c;
                _colorChanged = true;
            }
        }
    }

    sealed class MsdfShader : SimpleRectTextureShader
    {
        ShaderUniformVar4 u_color;
        PixelFarm.Drawing.Color _color;
        bool _colorChanged;

        public MsdfShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //credit: https://github.com/Chlumsky/msdfgen 

            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix;  
                varying vec2 v_texCoord;  
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord; 
                 }	 
                ";
            //enable derivative extension  for fwidth() function
            //see 
            //https://www.khronos.org/registry/gles/extensions/OES/OES_standard_derivatives.txt
            //https://github.com/AnalyticalGraphicsInc/cesium/issues/745
            //https://developer.mozilla.org/en-US/docs/Web/API/OES_standard_derivatives
            //https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/Using_Extensions
            string fs = @"
                        #ifdef GL_OES_standard_derivatives
                            #extension GL_OES_standard_derivatives : enable
                        #endif  
                        precision mediump float; 
                        varying vec2 v_texCoord;                
                        uniform sampler2D s_texture; //msdf texture 
                        uniform vec4 u_color;

                        float median(float r, float g, float b) {
                            return max(min(r, g), min(max(r, g), b));
                        }
                        void main() {
                            vec4 sample = texture2D(s_texture, v_texCoord);
                            float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
                            float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  
                            gl_FragColor= vec4(u_color[0],u_color[1],u_color[2],opacity * u_color[3]);
                        }
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            u_color = _shaderProgram.GetUniform4("u_color");
        }
        public void SetColor(PixelFarm.Drawing.Color color)
        {
            if (_color != color)
            {
                _colorChanged = true;
                _color = color;
            }
        }

        protected override void SetVarsBeforeRender()
        {
            if (_colorChanged)
            {
                u_color.SetValue(
                    _color.R / 255f,
                    _color.G / 255f,
                    _color.B / 255f,
                    _color.A / 255f);

                _colorChanged = false;
            }
            base.SetVarsBeforeRender();
        }
    }

    sealed class MsdfMaskShader : SimpleRectTextureShader
    {

        //TODO: review its name
        //this shader is similar to original MsdfShader
        //but this shader use color source from image instead of solid color

        /// <summary>
        /// color texture
        /// </summary>
        ShaderUniformVar1 _u_color_src;
        ShaderVtxAttrib2f _texCoord_color;
        public MsdfMaskShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //credit: https://github.com/Chlumsky/msdfgen 

            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                attribute vec2 a_texCoord_color;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix;  
                varying vec2 v_texCoord;  
                varying vec2 v_color_texCoord; 

                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position+vec4(u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord; 
                    v_color_texCoord= a_texCoord_color;
                 }	 
                ";
            //enable derivative extension  for fwidth() function
            //see 
            //https://www.khronos.org/registry/gles/extensions/OES/OES_standard_derivatives.txt
            //https://github.com/AnalyticalGraphicsInc/cesium/issues/745
            //https://developer.mozilla.org/en-US/docs/Web/API/OES_standard_derivatives
            //https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/Using_Extensions


            //s_texture=> msdf texture
            //
            string fs = @"
                        #ifdef GL_OES_standard_derivatives
                            #extension GL_OES_standard_derivatives : enable
                        #endif  
                        precision mediump float; 
                        varying vec2 v_texCoord;                
                        uniform sampler2D s_texture;  
                        uniform sampler2D u_color_src;                         
                        varying vec2 v_color_texCoord;

                        float median(float r, float g, float b) {
                            return max(min(r, g), min(max(r, g), b));
                        }
                        void main() {
                            vec4 msk = texture2D(s_texture, v_texCoord);
                            vec4 color = texture2D(u_color_src, v_color_texCoord);

                            float sigDist = median(msk[0], msk[1], msk[2]) - 0.5;
                            float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);  
                            gl_FragColor= vec4(color[2],color[1],color[0],opacity * color[3]);                          
                        }
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _u_color_src = _shaderProgram.GetUniform1("u_color_src");

            _texCoord_color = _shaderProgram.GetAttrV2f("a_texCoord_color");
        }

        int _colorBmpW;
        int _colorBmpH;

        /// <summary>
        /// load glbmp before draw
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadColorSourceBitmap(GLBitmap bmp)
        {
            //load base bitmap first
            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            TextureContainter container = _shareRes.LoadGLBitmap(bmp);
            //set color source bitmap
            _u_color_src.SetValue(container.TextureUnitNo);

            _colorBmpW = bmp.Width;
            _colorBmpH = bmp.Height;
        }

        public void DrawSubImage2(in PixelFarm.Drawing.RectangleF maskSrc,
           float colorSrcX, float colorSrcY,
           float targetLeft, float targetTop,
           float scale)
        {
            //-------------------------------------------------------------------------------------
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------          
            float orgBmpW = _latestBmpW;
            float orgBmpH = _latestBmpH;


            //mask src----
            float srcLeft = maskSrc.Left;
            float srcTop = maskSrc.Top;
            float srcW = maskSrc.Width;
            float srcH = maskSrc.Height;
            float srcBottom = srcTop + srcH;
            float srcRight = srcLeft + srcW;


            //----
            //src color
            float color_bmp_W = _colorBmpW;
            float color_bmp_H = _colorBmpH;

            float color_left = colorSrcX;
            float color_top = colorSrcY;
            float color_right = color_left + srcW;
            float color_bottom = color_top + srcH;

            unsafe
            {
                if (!_latestBmpYFlipped)
                {
                    float* imgVertices = stackalloc float[7 * 4];
                    {
                        imgVertices[0] = targetLeft;                    /**/imgVertices[1] = targetTop;                  /**/imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = srcLeft / orgBmpW;             /**/imgVertices[4] = srcBottom / orgBmpH;        /**///texture coord 0  (left,bottom)
                        imgVertices[5] = color_left / color_bmp_W;      /**/imgVertices[6] = color_bottom / color_bmp_H; /**///texture coord 0  (left,bottom)

                        //---------------------
                        imgVertices[7] = targetLeft;                    /**/imgVertices[8] = targetTop - (srcH * scale); /**/imgVertices[9] = 0; //coord 1 (left,bottom)
                        imgVertices[10] = srcLeft / orgBmpW;            /**/ imgVertices[11] = srcTop / orgBmpH;         /**///texture coord 1  (left,top)
                        imgVertices[12] = color_left / color_bmp_W;     /**/ imgVertices[13] = color_top / color_bmp_H;  /**///texture coord 1  (left,top)

                        //---------------------
                        imgVertices[14] = targetLeft + (srcW * scale);  /**/ imgVertices[15] = targetTop;                /**/imgVertices[16] = 0; //coord 2 (right,top)
                        imgVertices[17] = srcRight / orgBmpW;           /**/ imgVertices[18] = srcBottom / orgBmpH;      /**///texture coord 2  (right,bottom)
                        imgVertices[19] = color_right / color_bmp_W;    /**/ imgVertices[20] = color_bottom / color_bmp_H;  /**///texture coord 2  (right,bottom)

                        //---------------------
                        imgVertices[21] = targetLeft + (srcW * scale);  /**/imgVertices[22] = targetTop - (srcH * scale); /**/imgVertices[23] = 0; //coord 3 (right, bottom)
                        imgVertices[24] = srcRight / orgBmpW;           /**/imgVertices[25] = srcTop / orgBmpH;          /**///texture coord 3 (right,top)
                        imgVertices[26] = color_right / color_bmp_W;    /**/ imgVertices[27] = color_top / color_bmp_H;  /**///texture coord 3 (right,top)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 7);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 7);
                    _texCoord_color.UnsafeLoadMixedV2f(imgVertices + 5, 7);

                }
                else
                {
                    float* imgVertices = stackalloc float[7 * 4];
                    {
                        imgVertices[0] = targetLeft;                    /**/imgVertices[1] = targetTop;                   /**/imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                        imgVertices[3] = srcLeft / orgBmpW;             /**/imgVertices[4] = srcTop / orgBmpH;            /**/ //texture coord 0 (left,top)
                        imgVertices[5] = color_left / color_bmp_W;      /**/imgVertices[6] = color_top / color_bmp_H;     /**/ //texture coord 0 (left,top)
                        //---------------------
                        imgVertices[7] = targetLeft;                    /**/imgVertices[8] = targetTop - (srcH * scale);  /**/imgVertices[9] = 0; //coord 1 (left,bottom)
                        imgVertices[10] = srcLeft / orgBmpW;            /**/imgVertices[11] = srcBottom / orgBmpH;        /**/ //texture coord 1 (left,bottom)
                        imgVertices[12] = color_left / color_bmp_W;     /**/imgVertices[13] = color_bottom / color_bmp_H; /**/   //texture coord 1 (left,bottom)
                        //---------------------
                        imgVertices[14] = targetLeft + (srcW * scale);  /**/imgVertices[15] = targetTop;                 /**/imgVertices[16] = 0; //coord 2 (right,top)
                        imgVertices[17] = srcRight / orgBmpW;           /**/imgVertices[18] = srcTop / orgBmpH;            /**/    //texture coord 2 (right,top)
                        imgVertices[19] = color_right / color_bmp_W;          /**/imgVertices[20] = color_top / color_bmp_H;  /**///texture coord 2 (right,top)
                        //---------------------
                        imgVertices[21] = targetLeft + (srcW * scale);  /**/imgVertices[22] = targetTop - (srcH * scale); /**/imgVertices[23] = 0; //coord 3 (right, bottom)
                        imgVertices[24] = srcRight / orgBmpW;           /**/imgVertices[25] = srcBottom / orgBmpH;            /**///texture coord 3  (right,bottom)
                        imgVertices[26] = color_right / color_bmp_W;    /**/imgVertices[27] = color_bottom / color_bmp_H;   /**///texture coord 3  (right,bottom)
                    }
                    a_position.UnsafeLoadMixedV3f(imgVertices, 7);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 7);
                    _texCoord_color.UnsafeLoadMixedV2f(imgVertices + 5, 7);
                }
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);


            //TODO: review, this 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);//restore, assume org is default
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);//restore, assume org is default

        }
    }

}