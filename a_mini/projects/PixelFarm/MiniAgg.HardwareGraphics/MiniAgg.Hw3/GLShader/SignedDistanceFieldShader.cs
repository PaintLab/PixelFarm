//MIT 2016, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class SingleChannelSdf : SimpleRectTextureShader
    {
        public SingleChannelSdf(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
        }
        protected override void OnProgramBuilt()
        {
            base.OnProgramBuilt();
        }
    }
    class MultiChannelSdf : SimpleRectTextureShader
    {
        ShaderUniformVar4 _bgColor;
        ShaderUniformVar4 _fgColor;
        public MultiChannelSdf(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            //credit: https://github.com/Chlumsky/msdfgen 

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
                        uniform vec4 bgColor;
                        uniform vec4 fgColor;

                        float median(float r, float g, float b) {
                            return max(min(r, g), min(max(r, g), b));
                        }

                        void main() {
                            vec4 sample = texture2D(s_texture, v_texCoord);
                            float sigDist = median(sample[0], sample[1], sample[2]) - 0.5;
                            float opacity = clamp(sigDist/fwidth(sigDist) + 0.5, 0.0, 1.0);
                            gl_FragColor = mix(bgColor, fgColor, opacity);
                        }
             ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _bgColor = shaderProgram.GetUniform4("bgColor");
            _fgColor = shaderProgram.GetUniform4("fgColor");
        }
        public PixelFarm.Drawing.Color BackgroundColor;
        public PixelFarm.Drawing.Color ForegroundColor;
        protected override void OnSetVarsBeforeRenderer()
        {
            PixelFarm.Drawing.Color bgColor = BackgroundColor;
            PixelFarm.Drawing.Color fgColor = ForegroundColor;

            _bgColor.SetValue((float)bgColor.R / 255f, (float)bgColor.G / 255f, (float)bgColor.B / 255f, (float)bgColor.A / 255f);
            _fgColor.SetValue((float)bgColor.R / 255f, (float)bgColor.G / 255f, (float)bgColor.B / 255f, (float)bgColor.A / 255f);
        }
    }
}