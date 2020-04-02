//sealed class BGRAImageTextureWithTransparentShader : SimpleRectTextureShader
//{
//    ShaderUniformVar4 u_transparentColor;
//    PixelFarm.Drawing.Color _transparentColor;
//    bool _colorChanged;
//    public BGRAImageTextureWithTransparentShader(ShaderSharedResource shareRes)
//        : base(shareRes)
//    {
//        string vs = @"
//                attribute vec4 a_position;
//                attribute vec2 a_texCoord;
//                uniform vec2 u_ortho_offset;
//                uniform mat4 u_mvpMatrix; 
//                varying vec2 v_texCoord;
//                void main()
//                {
//                    gl_Position = u_mvpMatrix* (a_position+vec4(u_ortho_offset,0,0));
//                    v_texCoord =  a_texCoord;
//                 }	 
//                ";
//        //in fs, angle on windows 
//        //we need to switch color component
//        //because we store value in memory as BGRA
//        //and gl expect input in RGBA

//        //TODO: review here
//        string fs = @"
//                      precision mediump float;
//                      varying vec2 v_texCoord;
//                      uniform sampler2D s_texture;
//                      uniform vec4 u_transparentColor;

//                      void main()
//                      {
//                         vec4 c = texture2D(s_texture, v_texCoord); 
//                         if(c == u_transparentColor){
//                            discard;
//                         }else{                                                   
//                            gl_FragColor =  vec4(c[2],c[1],c[0],c[3]);  
//                         }
//                      }
//                ";
//        BuildProgram(vs, fs);

//    }
//    protected override void OnProgramBuilt()
//    {
//        u_transparentColor = _shaderProgram.GetUniform4("u_transparentColor");
//        SetTransparentColor(PixelFarm.Drawing.Color.White);//default
//    }
//    public void SetTransparentColor(PixelFarm.Drawing.Color transparentColor)
//    {
//        if (_transparentColor != transparentColor)
//        {
//            _transparentColor = transparentColor;
//            _colorChanged = true;
//        }
//    }
//    protected override void OnSetVarsBeforeRenderer()
//    {
//        if (_colorChanged)
//        {
//            u_transparentColor.SetValue(
//                _transparentColor.R / 255f,
//                _transparentColor.G / 255f,
//                _transparentColor.B / 255f,
//                _transparentColor.A / 255f);
//            _colorChanged = false;
//        }
//    }
//}



//old
//class GlyphImageStecilShader : SimpleRectTextureShader
//{
//    //similar to GdiImageTextureWithWhiteTransparentShader
//    float _color_a = 1f;
//    float _color_r;
//    float _color_g;
//    float _color_b;

//    ShaderUniformVar4 _d_color; //drawing color
//    public GlyphImageStecilShader(ShaderSharedResource shareRes)
//        : base(shareRes)
//    {
//        string vs = @"
//            attribute vec4 a_position;
//            attribute vec2 a_texCoord;
//            uniform mat4 u_mvpMatrix; 
//            varying vec2 v_texCoord;
//            void main()
//            {
//                gl_Position = u_mvpMatrix* a_position;
//                v_texCoord =  a_texCoord;
//             }	 
//            ";
//        //in fs, angle on windows 
//        //we need to switch color component
//        //because we store value in memory as BGRA
//        //and gl expect input in RGBA
//        string fs = @"
//                  precision mediump float;
//                  varying vec2 v_texCoord;
//                  uniform sampler2D s_texture;
//                  uniform vec4 d_color;
//                  void main()
//                  {
//                     vec4 c = texture2D(s_texture, v_texCoord); 
//                     if((c[2] ==1.0) && (c[1]==1.0) && (c[0]== 1.0) && (c[3] == 1.0)){
//                        discard;
//                     }else{                                                 

//                        gl_FragColor =  vec4(d_color[0],d_color[1],d_color[2],c[3]);  
//                     }
//                  }
//            ";
//        BuildProgram(vs, fs);
//    }
//    public void SetColor(PixelFarm.Drawing.Color c)
//    {
//        _color_a = c.A / 255f;
//        _color_r = c.R / 255f;
//        _color_g = c.G / 255f;
//        _color_b = c.B / 255f;
//    }
//    protected override void OnProgramBuilt()
//    {
//        _d_color = shaderProgram.GetUniform4("d_color");
//    }
//    protected override void OnSetVarsBeforeRenderer()
//    {
//        _d_color.SetValue(_color_r, _color_g, _color_b, _color_a);
//    }
//}