//MIT 2016, WinterDev
//glsl credit: http://xissburg.com/faster-gaussian-blur-in-glsl/

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BlurShader : SimpleRectTextureShader
    {
        ShaderUniformVar1 _horizontal;
        public BlurShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            BuildShaderV3();
        }
        void BuildShaderV3()
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
                     
                      uniform sampler2D s_texture;
                      uniform int blur_horizontal; 
                      varying vec2 v_texCoord; 
                      void main()
                      {                         
                        vec4 c = vec4(0.0);
                        float v_texCoord1= v_texCoord[1];
                        float v_texCoord0 =v_texCoord[0];
                        if(blur_horizontal==1){
                            c += texture2D(s_texture,vec2(v_texCoord0-0.028,v_texCoord1))*0.0044299121055113265;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.024,v_texCoord1))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.020,v_texCoord1))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.016,v_texCoord1))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.012,v_texCoord1))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.008,v_texCoord1))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0-0.004,v_texCoord1))*0.147308056121;
                            c += texture2D(s_texture, v_texCoord         )*0.159576912161;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.004,v_texCoord1))*0.147308056121;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.008,v_texCoord1))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.012,v_texCoord1))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.016,v_texCoord1))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.020,v_texCoord1))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.024,v_texCoord1))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0+0.028,v_texCoord1))*0.0044299121055113265;
                        }else{
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.028))*0.0044299121055113265;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.024))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.020))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.016))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.012))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.008))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.004))*0.147308056121;
                            c += texture2D(s_texture, v_texCoord         )*0.159576912161;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.004))*0.147308056121;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.008))*0.115876621105;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.012))*0.0776744219933;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.016))*0.0443683338718;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.020))*0.0215963866053;
                            c += texture2D(s_texture,vec2(v_texCoord0,v_texCoord1+0.024))*0.00895781211794;
                            c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.028))*0.0044299121055113265; 
                        }
                        gl_FragColor =  vec4(c[2],c[1],c[0],c[3]);
                      }
                ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            _horizontal = shaderProgram.GetUniform1("blur_horizontal");
        }
        public bool IsHorizontal { get; set; }
        protected override void OnSetVarsBeforeRenderer()
        {
            if (IsHorizontal)
            {
                _horizontal.SetValue(1);
            }
            else
            {
                _horizontal.SetValue(0);
            }
        }
    }
}