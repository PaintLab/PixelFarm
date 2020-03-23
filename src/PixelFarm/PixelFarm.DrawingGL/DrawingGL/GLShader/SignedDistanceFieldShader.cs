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
           
            var quad = new CpuBlit.VertexProcessing.Quad2f(targetLeft, targetTop, maskSrc.Width * scale, -maskSrc.Height * scale);
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(maskSrc.Left, maskSrc.Top, maskSrc.Width, maskSrc.Height, _latestBmpW, _latestBmpH);
            var colorSrc = new CpuBlit.VertexProcessing.Quad2f(colorSrcX, colorSrcY, maskSrc.Width, maskSrc.Height, _colorBmpW, _colorBmpH);

            unsafe
            {
                float* imgVertices = stackalloc float[7 * 4];
                AssignVertice7_4(quad, srcRect, colorSrc, imgVertices, !_latestBmpYFlipped);

                a_position.UnsafeLoadMixedV3f(imgVertices, 7);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 7);
                _texCoord_color.UnsafeLoadMixedV2f(imgVertices + 5, 7);
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