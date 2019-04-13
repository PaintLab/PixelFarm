//MIT, 2016-present, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class BlurShader : SimpleRectTextureShader
    {
         
        ShaderUniformVar1 _blur_x;
        ShaderUniformVar1 _blur_y; 
        public BlurShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            BuildShaderV3();
        }
        void BuildShaderV3()
        {
            //glsl credit: http://xissburg.com/faster-gaussian-blur-in-glsl/
            //--------------------------------------------------------------------------
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                uniform vec2 u_ortho_offset;
                uniform mat4 u_mvpMatrix;                 
                varying vec2 v_texCoord;  
                void main()
                {
                    gl_Position = u_mvpMatrix* (a_position +vec4(u_ortho_offset,0,0));
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
                      uniform float blur_x;
                      uniform float blur_y;

                      varying vec2 v_texCoord;
                      void main()
                      {                        
                          
                        float v_texCoord1= v_texCoord[1];
                        float v_texCoord0 =v_texCoord[0];
                         
                        gl_FragColor = 
                                 texture2D(s_texture,vec2(v_texCoord0-0.028*blur_x,v_texCoord1-0.028*blur_y))*0.0044299121055113265 +
                                 texture2D(s_texture,vec2(v_texCoord0-0.024*blur_x,v_texCoord1-0.024*blur_y))*0.00895781211794 +
                                 texture2D(s_texture,vec2(v_texCoord0-0.020*blur_x,v_texCoord1-0.020*blur_y))*0.0215963866053 +
                                 texture2D(s_texture,vec2(v_texCoord0-0.016*blur_x,v_texCoord1-0.016*blur_y))*0.0443683338718 +
                                 texture2D(s_texture,vec2(v_texCoord0-0.012*blur_x,v_texCoord1-0.012*blur_y))*0.0776744219933 +
                                 texture2D(s_texture,vec2(v_texCoord0-0.008*blur_x,v_texCoord1-0.008*blur_y))*0.115876621105 +
                                 texture2D(s_texture,vec2(v_texCoord0-0.004*blur_x,v_texCoord1-0.004*blur_y))*0.147308056121 +
                                 texture2D(s_texture, v_texCoord         )*0.159576912161 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.004*blur_x,v_texCoord1+0.004*blur_y))*0.147308056121 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.008*blur_x,v_texCoord1+0.008*blur_y))*0.115876621105 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.012*blur_x,v_texCoord1+0.012*blur_y))*0.0776744219933 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.016*blur_x,v_texCoord1+0.016*blur_y))*0.0443683338718 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.020*blur_x,v_texCoord1+0.020*blur_y))*0.0215963866053 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.024*blur_x,v_texCoord1+0.024*blur_y))*0.00895781211794 +
                                 texture2D(s_texture,vec2(v_texCoord0+0.028*blur_x,v_texCoord1+0.028*blur_y))*0.0044299121055113265; 
                      }
                ";

            //old version
            //string fs = @"
            //          precision mediump float;

            //          uniform sampler2D s_texture;
            //          uniform int isBigEndian;
            //          uniform int blur_horizontal; 
            //          uniform vec2 u_blur_direction;  
            //          varying vec2 v_texCoord;
            //          void main()
            //          {                         
            //            vec4 c = vec4(0.0);
            //            float v_texCoord1= v_texCoord[1];
            //            float v_texCoord0 =v_texCoord[0];
            //            if(blur_horizontal==1){
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.028,v_texCoord1))*0.0044299121055113265;
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.024,v_texCoord1))*0.00895781211794;
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.020,v_texCoord1))*0.0215963866053;
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.016,v_texCoord1))*0.0443683338718;
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.012,v_texCoord1))*0.0776744219933;
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.008,v_texCoord1))*0.115876621105;
            //                c += texture2D(s_texture,vec2(v_texCoord0-0.004,v_texCoord1))*0.147308056121;
            //                c += texture2D(s_texture, v_texCoord         )*0.159576912161;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.004,v_texCoord1))*0.147308056121;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.008,v_texCoord1))*0.115876621105;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.012,v_texCoord1))*0.0776744219933;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.016,v_texCoord1))*0.0443683338718;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.020,v_texCoord1))*0.0215963866053;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.024,v_texCoord1))*0.00895781211794;
            //                c += texture2D(s_texture,vec2(v_texCoord0+0.028,v_texCoord1))*0.0044299121055113265;
            //            }else{
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.028))*0.0044299121055113265;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.024))*0.00895781211794;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.020))*0.0215963866053;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.016))*0.0443683338718;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.012))*0.0776744219933;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.008))*0.115876621105;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1-0.004))*0.147308056121;
            //                c += texture2D(s_texture, v_texCoord         )*0.159576912161;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.004))*0.147308056121;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.008))*0.115876621105;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.012))*0.0776744219933;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.016))*0.0443683338718;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.020))*0.0215963866053;
            //                c += texture2D(s_texture,vec2(v_texCoord0,v_texCoord1+0.024))*0.00895781211794;
            //                c += texture2D(s_texture,vec2(v_texCoord0 ,v_texCoord1+0.028))*0.0044299121055113265; 
            //            }
            //            if(isBigEndian ==1){
            //                gl_FragColor = c;
            //            }else{
            //                gl_FragColor = vec4(c[2],c[1],c[0],c[3]);
            //            }
            //          }
            //    ";
            BuildProgram(vs, fs);
        }
        protected override void OnProgramBuilt()
        {
            //TODO: review here
            //temp fixed for big vs little-endian

            _blur_x = _shaderProgram.GetUniform1("blur_x");
            _blur_y = _shaderProgram.GetUniform1("blur_y");
        }
        public bool IsHorizontal { get; set; }

        protected override void SetVarsBeforeRender()
        {
            if (IsHorizontal)
            {
                _blur_x.SetValue(1f);
                _blur_y.SetValue(0f);
            }
            else
            {
                _blur_x.SetValue(0f);
                _blur_y.SetValue(1f);
            }
        }
    }



    public static class Mat3x3ConvGen
    {
        //credit: http://webglfundamentals.org/webgl/webgl-2d-image-3x3-convolution.html

        public static readonly
            float[] normal = new float[] {
              0,0,0,
              0,1,0,
              0,0,0
        };
        public static readonly
            float[] gaussianBlur = new float[] {
              0.045f, 0.122f, 0.045f,
              0.122f, 0.332f, 0.122f,
              0.045f, 0.122f, 0.045f
        };
        public static readonly
            float[] gaussianBlur2 = new float[] {
              1, 2, 1,
              2, 4, 2,
              1, 2, 1
        };
        public static readonly
            float[] gaussianBlur3 = new float[] {
              0.045f, 0.122f, 0.045f,
              0.122f, 0.332f, 0.122f,
              0.045f, 0.122f, 0.045f
        };
        public static readonly
            float[] unsharpen = new float[]
        {
              -1, -1, -1,
              -1,  9, -1,
              -1, -1, -1
        };
        public static readonly
            float[] sharpness = new float[]
            {
              -1, -1, -1,
              -1,  5, -1,
              -1, -1, -1
            };
        public static readonly
            float[] sharpen = new float[]
          {
              -1, -1, -1,
              -1,  16, -1,
              -1, -1, -1
          };
        public static readonly
          float[] edgeDetect = new float[]
        {
              -0.125f, -0.125f, -0.125f,
             -0.125f,  1,     -0.125f,
             -0.125f, -0.125f, -0.125f
        };
        public static readonly
          float[] edgeDetect2 = new float[]
          {
               -1, -1, -1,
               -1,  8, -1,
               -1, -1, -1
          };
        public static readonly
          float[] edgeDetect3 = new float[]
          {
              -5, 0, 0,
               0, 0, 0,
               0, 0, 5
          };
        public static readonly
          float[] edgeDetect4 = new float[]
          {
              -1,-1,-1,
               0, 0, 0,
               1, 1, 1
          };
        public static readonly
          float[] edgeDetect5 = new float[]
          {
              -1,-1,-1,
               2, 2,2,
              -1,-1,-1,
          };
        public static readonly
         float[] edgeDetect6 = new float[]
         {
              -5, -5, -5,
              -5, 39, -5,
              -5, -5, -5
         };
        public static readonly
        float[] sobelHorizontal = new float[]
        {
              1,  2,  1,
              0,  0,  0,
             -1, -2, -1
        };
        public static readonly
         float[] sobelVertical = new float[]
         {
             1,  0, -1,
             2,  0, -2,
             1,  0, -1
         };
        public static readonly
         float[] previtHorizontal = new float[]
         {
             1,  1,  1,
             0,  0,  0,
             -1, -1, -1
         };
        public static readonly
          float[] previtVertical = new float[]
          {
            1,  0, -1,
            1,  0, -1,
            1,  0, -1
          };
        public static readonly
          float[] boxBlur = new float[]
          {
            0.111f, 0.111f, 0.111f,
            0.111f, 0.111f, 0.111f,
            0.111f, 0.111f, 0.111f
          };
        public static readonly
          float[] triangleBlur = new float[]
          {
              0.0625f, 0.125f, 0.0625f,
              0.125f,  0.25f,  0.125f,
              0.0625f, 0.125f, 0.0625f
          };
        public static readonly
         float[] emboss = new float[]
         {
               -2, -1,  0,
               -1,  1,  1,
                0,  1,  2
         };
    }



    class Conv3x3TextureShader : SimpleRectTextureShader
    {
        //credit: http://webglfundamentals.org/webgl/webgl-2d-image-3x3-convolution.html

        ShaderUniformVar2 u_onepix_xy;
        ShaderUniformMatrix3 u_convKernel;
        ShaderUniformVar1 u_kernelWeight;
        float[] _kernels;
        float _kernelWeight;
        float _toDrawImgW = 1, _toDrawImgH = 1;
        bool _kernelChanged;
        bool _imgSizeChanged;
        public Conv3x3TextureShader(ShaderSharedResource shareRes)
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
                    gl_Position = u_mvpMatrix* (a_position +vec4(u_ortho_offset,0,0));
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
                      uniform mat3 convKernel;
                      uniform float kernelWeight;  
                      uniform vec2 onepix_xy;
                        
                      varying vec2 v_texCoord; 
                      void main()
                      {                         
                        
                        float v_texCoord1= v_texCoord[1];
                        float v_texCoord0 =v_texCoord[0];
                        float one_x=onepix_xy[0];               
                        float one_y=onepix_xy[1];

                        gl_FragColor = ( texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1+one_y))*convKernel[0][0]+
                                         texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1+one_y))*convKernel[0][1]+
                                         texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1+one_y))*convKernel[0][2]+

                                         texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1))*convKernel[1][0]+
                                         texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1))*convKernel[1][1]+
                                         texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1))*convKernel[1][2]+

                                         texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1-one_y))*convKernel[2][0]+
                                         texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1-one_y))*convKernel[2][1]+
                                         texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1-one_y))*convKernel[2][2]) / kernelWeight; 
                      }
                ";
            //string fs = @"
            //          precision mediump float;

            //          uniform sampler2D s_texture;
            //          uniform int isBigEndian;
            //          uniform mat3 convKernel;
            //          uniform float kernelWeight;  
            //          uniform vec2 onepix_xy;

            //          varying vec2 v_texCoord; 
            //          void main()
            //          {                         

            //            float v_texCoord1= v_texCoord[1];
            //            float v_texCoord0 =v_texCoord[0];
            //            float one_x=onepix_xy[0];               
            //            float one_y=onepix_xy[1];

            //            vec4 c = (texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1+one_y))*convKernel[0][0]+
            //                     texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1+one_y))*convKernel[0][1]+
            //                     texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1+one_y))*convKernel[0][2]+

            //                     texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1))*convKernel[1][0]+
            //                     texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1))*convKernel[1][1]+
            //                     texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1))*convKernel[1][2]+

            //                     texture2D(s_texture,vec2(v_texCoord0-one_x,v_texCoord1-one_y))*convKernel[2][0]+
            //                     texture2D(s_texture,vec2(v_texCoord0      ,v_texCoord1-one_y))*convKernel[2][1]+
            //                     texture2D(s_texture,vec2(v_texCoord0+one_x,v_texCoord1-one_y))*convKernel[2][2]) / kernelWeight;

            //            if(isBigEndian ==1){
            //                gl_FragColor = vec4(c[0],c[1],c[2],1);
            //            }else{
            //                gl_FragColor = vec4(c[2],c[1],c[0],1);
            //            }
            //          }
            //    ";
            BuildProgram(vs, fs);
            SetConvolutionKernel(Mat3x3ConvGen.gaussianBlur);
            _imgSizeChanged = true;
        }
        public void SetConvolutionKernel(float[] kernels)
        {
            _kernels = kernels;
            //calculate kernel weight
            float total = 0;
            for (int i = kernels.Length - 1; i >= 0; --i)
            {
                total += kernels[i];
            }
            if (total <= 0)
            {
                total = 1;
            }
            _kernelWeight = total;
            _kernelChanged = true;
        }

        public void SetBitmapSize(int w, int h)
        {
            if (_toDrawImgW != w || _toDrawImgH != h)
            {
                _toDrawImgW = w;
                _toDrawImgH = h;
                _imgSizeChanged = true;
            }
        }
        //
        protected override void OnProgramBuilt()
        {           
            u_convKernel = _shaderProgram.GetUniformMat3("convKernel");
            u_onepix_xy = _shaderProgram.GetUniform2("onepix_xy");
            u_kernelWeight = _shaderProgram.GetUniform1("kernelWeight");
        }
        protected override void SetVarsBeforeRender()
        {
            if (_kernelChanged)
            {
                u_convKernel.SetData(_kernels);
                u_kernelWeight.SetValue(_kernelWeight);
                _kernelChanged = false;
            }
            if (_imgSizeChanged)
            {
                u_onepix_xy.SetValue(1f / _toDrawImgW, 1f / _toDrawImgH);
                _imgSizeChanged = false;
            }

        }

    }
}