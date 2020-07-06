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
        public void DrawSubImage2(in CpuBlit.VertexProcessing.Quad2f quad,
          in PixelFarm.Drawing.RectangleF maskSrc,
          float targetLeft, float targetTop)
        {
            //-------------------------------------------------------------------------------------
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------           
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(maskSrc.Left, maskSrc.Top, maskSrc.Width, maskSrc.Height, _latestBmpW, _latestBmpH);

            unsafe
            {
                float* imgVertices = stackalloc float[5 * 4];
                AssignVertice5_4(quad, srcRect, imgVertices, !_latestBmpYFlipped);

                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
            }
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }
}