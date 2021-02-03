//MIT, 2016-present, WinterDev

using OpenTK.Graphics.ES20;
using System;

namespace PixelFarm.DrawingGL
{
    abstract class SimpleRectTextureShader : ColorFillShaderBase
    {
        protected ShaderVtxAttrib3f a_position;
        protected ShaderVtxAttrib2f a_texCoord;

        protected ShaderUniformVar1 u_texture;
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

            u_texture.SetValue(container.TextureUnitNo);
            _latestBmpW = bmp.Width;
            _latestBmpH = bmp.Height;
            _latestBmpYFlipped = bmp.IsYFlipped;
        }

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

            CpuBlit.ArrayListSegment<float> vertexListSpan = vboBuilder.CreateVertextListSpan();
            CpuBlit.ArrayListSegment<ushort> indexListSpan = vboBuilder.CreateIndexListSpan();

            SetCurrent();
            CheckViewMatrix();
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------       
            unsafe
            {
                CpuBlit.ArrayListSegment<float>.UnsafeGetInternalArr(vertexListSpan, out float[] vertexArr);
                CpuBlit.ArrayListSegment<ushort>.UnsafeGetInternalArr(indexListSpan, out ushort[] indexArr);

                fixed (float* imgVertices = &vertexArr[vertexListSpan.beginAt])
                {
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                fixed (ushort* indexVertices = &indexArr[indexListSpan.beginAt])
                {
                    GL.DrawElements(PrimitiveType.TriangleStrip, indexListSpan.len, DrawElementsType.UnsignedShort, (IntPtr)indexVertices);
                }
            }

        }

        public void Render(GLBitmap bmp, float left, float top, float w, float h)
        {
            Render(bmp, left, top, w, h, bmp.IsYFlipped);
        }

        protected static unsafe void AssignVertice5_4(
              in PixelFarm.CpuBlit.VertexProcessing.Quad2f quad,
              float* imgVertices,
              bool flipY)
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


            if (flipY)
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

                imgVertices[0] = quad.left_top_x; imgVertices[1] = quad.left_top_y; imgVertices[2] = 0; //coord 0 (left,top)
                imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                                                        //---------------------
                imgVertices[5] = quad.left_bottom_x; imgVertices[6] = quad.left_bottom_y; imgVertices[7] = 0; //coord 1 (left,bottom)
                imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                //---------------------
                imgVertices[10] = quad.right_top_x; imgVertices[11] = quad.right_top_y; imgVertices[12] = 0; //coord 2 (right,top)
                imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                //---------------------
                imgVertices[15] = quad.right_bottom_x; imgVertices[16] = quad.right_bottom_y; imgVertices[17] = 0; //coord 3 (right, bottom)
                imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)

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

                imgVertices[0] = quad.left_top_x; imgVertices[1] = quad.left_top_y; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                imgVertices[3] = 0; imgVertices[4] = 1; //texture coord 0 (left,top)

                //---------------------
                imgVertices[5] = quad.left_bottom_x; imgVertices[6] = quad.left_bottom_y; imgVertices[7] = 0; //coord 1 (left,bottom)
                imgVertices[8] = 0; imgVertices[9] = 0; //texture coord 1 (left,bottom)

                //---------------------
                imgVertices[10] = quad.right_top_x; imgVertices[11] = quad.right_top_y; imgVertices[12] = 0; //coord 2 (right,top)
                imgVertices[13] = 1; imgVertices[14] = 1; //texture coord 2 (right,top)

                //---------------------
                imgVertices[15] = quad.right_bottom_x; imgVertices[16] = quad.right_bottom_y; imgVertices[17] = 0; //coord 3 (right, bottom)
                imgVertices[18] = 1; imgVertices[19] = 0; //texture coord 3  (right,bottom)

            }
        }

        protected static unsafe void AssignVertice5_4(
             in PixelFarm.CpuBlit.VertexProcessing.Quad2f quad,
             in PixelFarm.CpuBlit.VertexProcessing.Quad2f srcRect,
             float* imgVertices,
             bool flipY)
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


            if (flipY)
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


                imgVertices[0] = quad.left_top_x; imgVertices[1] = quad.left_top_y; imgVertices[2] = 0; //coord 0 (left,top)
                imgVertices[3] = srcRect.left_bottom_x; imgVertices[4] = srcRect.left_bottom_y; //texture coord 0 (left,bottom)
                                                                                                //---------------------
                imgVertices[5] = quad.left_bottom_x; imgVertices[6] = quad.left_bottom_y; imgVertices[7] = 0; //coord 1 (left,bottom)
                imgVertices[8] = srcRect.left_top_x; imgVertices[9] = srcRect.left_top_y; //texture coord 1  (left,top)

                //---------------------
                imgVertices[10] = quad.right_top_x; imgVertices[11] = quad.right_top_y; imgVertices[12] = 0; //coord 2 (right,top)
                imgVertices[13] = srcRect.right_bottom_x; imgVertices[14] = srcRect.right_bottom_y; //texture coord 2  (right,bottom)

                //---------------------
                imgVertices[15] = quad.right_bottom_x; imgVertices[16] = quad.right_bottom_y; imgVertices[17] = 0; //coord 3 (right, bottom)
                imgVertices[18] = srcRect.right_top_x; imgVertices[19] = srcRect.right_top_y; //texture coord 3 (right,top)

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

                imgVertices[0] = quad.left_top_x; imgVertices[1] = quad.left_top_y; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                imgVertices[3] = srcRect.left_top_x; imgVertices[4] = srcRect.left_top_y; //texture coord 0 (left,top)

                //---------------------
                imgVertices[5] = quad.left_bottom_x; imgVertices[6] = quad.left_bottom_y; imgVertices[7] = 0; //coord 1 (left,bottom)
                imgVertices[8] = srcRect.left_bottom_x; imgVertices[9] = srcRect.left_bottom_y; //texture coord 1 (left,bottom)

                //---------------------
                imgVertices[10] = quad.right_top_x; imgVertices[11] = quad.right_top_y; imgVertices[12] = 0; //coord 2 (right,top)
                imgVertices[13] = srcRect.right_top_x; imgVertices[14] = srcRect.right_top_y; //texture coord 2 (right,top)

                //---------------------
                imgVertices[15] = quad.right_bottom_x; imgVertices[16] = quad.right_bottom_y; imgVertices[17] = 0; //coord 3 (right, bottom)
                imgVertices[18] = srcRect.right_bottom_x; imgVertices[19] = srcRect.right_bottom_y; //texture coord 3  (right,bottom)

            }
        }
        protected static unsafe void AssignVertice7_4(
          in PixelFarm.CpuBlit.VertexProcessing.Quad2f quad,
          in PixelFarm.CpuBlit.VertexProcessing.Quad2f srcRect,
          in PixelFarm.CpuBlit.VertexProcessing.Quad2f colorRect,
          float* imgVertices,
          bool flipY)
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


            if (flipY)
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


                imgVertices[0] = quad.left_top_x; imgVertices[1] = quad.left_top_y; imgVertices[2] = 0; //coord 0 (left,top)
                imgVertices[3] = srcRect.left_bottom_x; imgVertices[4] = srcRect.left_bottom_y; //texture coord 0 (left,bottom)
                imgVertices[5] = colorRect.left_bottom_x; imgVertices[6] = colorRect.left_bottom_y; //texture coord 0 (left,bottom)

                //---------------------
                imgVertices[7] = quad.left_bottom_x; imgVertices[8] = quad.left_bottom_y; imgVertices[9] = 0; //coord 1 (left,bottom)
                imgVertices[10] = srcRect.left_top_x; imgVertices[11] = srcRect.left_top_y; //texture coord 1  (left,top)
                imgVertices[12] = colorRect.left_top_x; imgVertices[13] = colorRect.left_top_y; //texture coord 1  (left,top)
                //---------------------
                imgVertices[14] = quad.right_top_x; imgVertices[15] = quad.right_top_y; imgVertices[16] = 0; //coord 2 (right,top)
                imgVertices[17] = srcRect.right_bottom_x; imgVertices[18] = srcRect.right_bottom_y; //texture coord 2  (right,bottom)
                imgVertices[19] = colorRect.right_bottom_x; imgVertices[20] = colorRect.right_bottom_y; //texture coord 2  (right,bottom)
                //---------------------
                imgVertices[21] = quad.right_bottom_x; imgVertices[22] = quad.right_bottom_y; imgVertices[23] = 0; //coord 3 (right, bottom)
                imgVertices[24] = srcRect.right_top_x; imgVertices[25] = srcRect.right_top_y; //texture coord 3 (right,top)
                imgVertices[26] = colorRect.right_top_x; imgVertices[27] = colorRect.right_top_y; //texture coord 3 (right,top)

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

                imgVertices[0] = quad.left_top_x; imgVertices[1] = quad.left_top_y; imgVertices[2] = 0; //coord 0 (left,top)                                                                                                       
                imgVertices[3] = srcRect.left_top_x; imgVertices[4] = srcRect.left_top_y; //texture coord 0 (left,top)
                imgVertices[5] = colorRect.left_top_x; imgVertices[6] = colorRect.left_top_y; //texture coord 0 (left,top)
                //---------------------
                imgVertices[7] = quad.left_bottom_x; imgVertices[8] = quad.left_bottom_y; imgVertices[9] = 0; //coord 1 (left,bottom)
                imgVertices[10] = srcRect.left_bottom_x; imgVertices[11] = srcRect.left_bottom_y; //texture coord 1 (left,bottom)
                imgVertices[12] = colorRect.left_bottom_x; imgVertices[13] = colorRect.left_bottom_y; //texture coord 1 (left,bottom)
                //---------------------
                imgVertices[14] = quad.right_top_x; imgVertices[15] = quad.right_top_y; imgVertices[16] = 0; //coord 2 (right,top)
                imgVertices[17] = srcRect.right_top_x; imgVertices[18] = srcRect.right_top_y; //texture coord 2 (right,top)
                imgVertices[19] = colorRect.right_top_x; imgVertices[20] = colorRect.right_top_y; //texture coord 2 (right,top)
                //---------------------
                imgVertices[21] = quad.right_bottom_x; imgVertices[22] = quad.right_bottom_y; imgVertices[23] = 0; //coord 3 (right, bottom)
                imgVertices[24] = srcRect.right_bottom_x; imgVertices[25] = srcRect.right_bottom_y; //texture coord 3  (right,bottom)
                imgVertices[26] = colorRect.right_bottom_x; imgVertices[27] = colorRect.right_bottom_y; //texture coord 3  (right,bottom)

            }
        }

        public void Render(GLBitmap bmp,
            in PixelFarm.CpuBlit.VertexProcessing.Quad2f quad,
            bool flipY = false)
        {
            //good read
            //https://stackoverflow.com/questions/8679052/initialization-of-memory-allocated-with-stackalloc

            unsafe
            {
                float* imgVertices = stackalloc float[5 * 4];
                AssignVertice5_4(quad, imgVertices, flipY);
                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...

            TextureContainter textureContainer = _shareRes.LoadGLBitmap(bmp);

            u_texture.SetValue(textureContainer.TextureUnitNo);
            SetVarsBeforeRender();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }

        public void Render(GLBitmap glBmp, float left, float top, float w, float h, bool isFlipped = false)
        {
            unsafe
            {

                float* imgVertices = stackalloc float[5 * 4];
                var quad = new CpuBlit.VertexProcessing.Quad2f(left, top, w, -h); //** -h
                AssignVertice5_4(quad, imgVertices, isFlipped);
                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            TextureContainter container = _shareRes.LoadGLBitmap(glBmp);
            u_texture.SetValue(container.TextureUnitNo);
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
            u_ortho_offset = _shaderProgram.GetUniform2("u_ortho_offset");
            u_texture = _shaderProgram.GetUniform1("s_texture");
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
        ShaderUniformVar2 _offset;
        bool _hasSomeOffset = false;

        public BGRAImageTextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //--------------------------------------------------------------------------
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
        protected override void OnProgramBuilt()
        {
            base.OnProgramBuilt();
            _offset = _shaderProgram.GetUniform2("u_offset");
        }
        protected override void SetVarsBeforeRender()
        {
            if (_hasSomeOffset)
            {
                _offset.SetValue(0, 0);
                _hasSomeOffset = false;
            }
            base.SetVarsBeforeRender();
        }
        public void DrawSubImages(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
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

            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            vbo.UnBind();

        }
    }



    /// <summary>
    /// for 32 bits texture/image  in RGBA format
    /// </summary>
    sealed class RGBATextureShader : SimpleRectTextureShader
    {
        ShaderUniformVar2 _offset;
        bool _hasSomeOffset = false;
        public RGBATextureShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //--------------------------------------------------------------------------
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
        protected override void OnProgramBuilt()
        {
            base.OnProgramBuilt();
            _offset = _shaderProgram.GetUniform2("u_offset");
        }
        protected override void SetVarsBeforeRender()
        {
            base.SetVarsBeforeRender();
            if (_hasSomeOffset)
            {
                _offset.SetValue(0, 0);
                _hasSomeOffset = false;
            }
        }
        public void DrawSubImages(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
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

            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            vbo.UnBind();

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


        public void DrawSubImages(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
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

        /// <summary>
        /// DrawElements, use vertex-buffer and index-list
        /// </summary>
        /// <param name="vboList"></param>
        /// <param name="indexList"></param>
        public void DrawSubImages(GLBitmap glBmp, TextureCoordVboBuilder vboBuilder)
        {
            CpuBlit.ArrayListSegment<float> vertextListSpan = vboBuilder.CreateVertextListSpan();

            if (vertextListSpan.Count == 0) { return; }

            CpuBlit.ArrayListSegment<ushort> indexListSpan = vboBuilder.CreateIndexListSpan();

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
                CpuBlit.ArrayListSegment<float>.UnsafeGetInternalArr(vertextListSpan, out float[] v_arr);
                CpuBlit.ArrayListSegment<ushort>.UnsafeGetInternalArr(indexListSpan, out ushort[] i_arr);

                fixed (float* imgVertices = &v_arr[vertextListSpan.beginAt])
                {
                    a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                    a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }


                //SHARED ARRAY  
#if DEBUG
                System.Diagnostics.Debug.WriteLine("lcd3steps:");
#endif

                fixed (ushort* indexArrPtr = &i_arr[indexListSpan.beginAt])
                {
                    //version 1
                    //0. B , yellow  result
                    GL.ColorMask(false, false, true, false);
                    SetCompo(ColorCompo.C0);

                    GL.DrawElements(PrimitiveType.TriangleStrip, indexListSpan.len, DrawElementsType.UnsignedShort, (IntPtr)indexArrPtr);

                    //1. G , magenta result
                    GL.ColorMask(false, true, false, false);
                    SetCompo(ColorCompo.C1);
                    GL.DrawElements(PrimitiveType.TriangleStrip, indexListSpan.len, DrawElementsType.UnsignedShort, (IntPtr)indexArrPtr);

                    //2. R , cyan result 
                    GL.ColorMask(true, false, false, false);//     
                    SetCompo(ColorCompo.C2);
                    GL.DrawElements(PrimitiveType.TriangleStrip, indexListSpan.len, DrawElementsType.UnsignedShort, (IntPtr)indexArrPtr);

                    //restore
                    GL.ColorMask(true, true, true, true);
                }

            }
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

            float scale = 1;

            var quad = new CpuBlit.VertexProcessing.Quad2f(targetLeft, targetTop, srcW * scale, -srcH * scale);
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(srcLeft, srcTop, srcW, srcH, _latestBmpW, _latestBmpH);

            unsafe
            {

                float* imgVertices = stackalloc float[5 * 4];
                AssignVertice5_4(quad, srcRect, imgVertices, !_latestBmpYFlipped);

                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
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


        public void DrawSubImageNoLcdEffect(GLBitmap glBmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            SetCurrent();
            CheckViewMatrix();

            LoadGLBitmap(glBmp);

            if (_hasSomeOffset)
            {
                _offset.SetValue(0f, 0f);
                _hasSomeOffset = false;//reset
            }


            float scale = 1;
            var quad = new CpuBlit.VertexProcessing.Quad2f(targetLeft, targetTop, srcW * scale, -srcH * scale);
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(srcLeft, srcTop, srcW, srcH, _latestBmpW, _latestBmpH);

            unsafe
            {

                float* imgVertices = stackalloc float[5 * 4];
                AssignVertice5_4(quad, srcRect, imgVertices, !_latestBmpYFlipped);

                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
            }


            SetCompo(ColorCompo.C_ALL);
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
        public void DrawSubImagesNoLcdEffect(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
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

            float scale = 1;
            var quad = new CpuBlit.VertexProcessing.Quad2f(targetLeft, targetTop, srcW * scale, -srcH * scale);
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(srcLeft, srcTop, srcW, srcH, _latestBmpW, _latestBmpH);
            unsafe
            {

                float* imgVertices = stackalloc float[5 * 4];
                AssignVertice5_4(quad, srcRect, imgVertices, !_latestBmpYFlipped);

                a_position.UnsafeLoadMixedV3f(imgVertices, 5);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 5);
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
            //2. this should work on transparent BG too, we will test next time

            //3. for word-strip creation, we render white glyph on black bg, so u_color = vec4(1,1,1,1), we skip that
            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture; 
                      varying vec2 v_texCoord; 
                      void main()
                      {   
                            vec4 c = texture2D(s_texture,v_texCoord);
                            gl_FragColor= vec4(c[2],c[1],c[0],c[3]);
                      }
                ";
            BuildProgram(vs, fs);
        }

        protected override void OnProgramBuilt()
        {
            _offset = _shaderProgram.GetUniform2("u_offset");
        }
        protected override void SetVarsBeforeRender() { }
        public void DrawSubImages(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
        {
            SetCurrent();
            CheckViewMatrix();
            LoadGLBitmap(glBmp);
            //
            _offset.SetValue(x, y);

            vbo.Bind();
            a_position.LoadLatest(5, 0);
            a_texCoord.LoadLatest(5, 3 * 4);

            //we render this 2 times 
            GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.OneMinusSrcColor);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);
            // 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);//restore 
            vbo.UnBind();
        }
    }
    sealed class LcdSubPixShaderV2 : SimpleRectTextureShader
    {
        //this shader is designed for subpixel shader
        //for transparent background        

        ShaderUniformVar2 _offset;
        ShaderUniformVar4 _u_color;
        public LcdSubPixShaderV2(ShaderSharedResource shareRes)
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
            //2. this should work on transparent BG too, we will test next time
            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture; 
                     
                      uniform vec4 u_color;
                      varying vec2 v_texCoord; 
                      void main()
                      {   
                            vec4 c = texture2D(s_texture,v_texCoord);
                            gl_FragColor= vec4(c[2] * u_color[0] ,c[1] * u_color[1]  ,c[0]* u_color[2] ,c[3]* u_color[3]);
                      }
                ";
            BuildProgram(vs, fs);
        }

        protected override void OnProgramBuilt()
        {
            _offset = _shaderProgram.GetUniform2("u_offset");
            _u_color = _shaderProgram.GetUniform4("u_color");
            SetFillColor(Drawing.Color.Black);
        }
        protected override void SetVarsBeforeRender() { }

        float _r, _g, _b, _a;
        public void SetFillColor(PixelFarm.Drawing.Color color)
        {
            _r = color.R / 255f;
            _g = color.G / 255f;
            _b = color.B / 255f;
            _a = color.A / 255f;
        }
        public void DrawSubImages(GLBitmap glBmp, VertexBufferObject vbo, int elemCount, float x, float y)
        {
            SetCurrent();
            CheckViewMatrix();
            LoadGLBitmap(glBmp);
            //
            _offset.SetValue(x, y);

            vbo.Bind();
            a_position.LoadLatest(5, 0);
            a_texCoord.LoadLatest(5, 3 * 4);
            _u_color.SetValue(_r, _g, _b, _a);


            //we render this 2 times 
            GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.OneMinusSrcColor);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);

            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.DrawElements(BeginMode.TriangleStrip, elemCount, DrawElementsType.UnsignedShort, 0);
            // 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);//restore 
            vbo.UnBind();
        }
    }



    abstract class MaskShaderBase : SimpleRectTextureShader
    {
        //in this shader we have 2 textures
        //1. for mask (default, similar to other SimpleRectTextureShaders)
        //2. for color source (instead of vertex color) 

        ShaderUniformVar2 _offset;


        /// <summary>
        /// color texture for color src
        /// </summary>
        ShaderUniformVar1 _u_color_src;
        ShaderVtxAttrib2f _texCoord_color;


        ShaderUniformVar2 _colorSrcOffset;

        public MaskShaderBase(ShaderSharedResource shareRes)
            : base(shareRes)
        {
        }

        protected override void OnProgramBuilt()
        {
            _offset = _shaderProgram.GetUniform2("u_offset");
            _u_color_src = _shaderProgram.GetUniform1("s_color_src");
            _texCoord_color = _shaderProgram.GetAttrV2f("a_texCoord_color");
            _colorSrcOffset = _shaderProgram.GetUniform2("u_color_src_offset");
        }
        protected override void SetVarsBeforeRender()
        {
        }


        int _colorBmpW;
        int _colorBmpH;
        float _colorSrcOffsetX;
        float _colorSrcOffsetY;
        public void SetColorSourceOffset(float dx, float dy)
        {
            _colorSrcOffsetX = dx;
            _colorSrcOffsetY = dy;
        }
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
            float targetLeft, float targetTop)
        {
            //-------------------------------------------------------------------------------------
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------          

            var quad = new CpuBlit.VertexProcessing.Quad2f(targetLeft, targetTop, maskSrc.Width, -maskSrc.Height);
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(maskSrc.Left, maskSrc.Top, maskSrc.Width, maskSrc.Height, _latestBmpW, _latestBmpH);
            var colorSrc = new CpuBlit.VertexProcessing.Quad2f(colorSrcX, colorSrcY, maskSrc.Width, maskSrc.Height, _colorBmpW, _colorBmpH);

            unsafe
            {
                float* imgVertices = stackalloc float[7 * 4];
                AssignVertice7_4(quad, srcRect, colorSrc, imgVertices, !_latestBmpYFlipped);

                _colorSrcOffset.SetValue(_colorSrcOffsetX / maskSrc.Width, _colorSrcOffsetY / maskSrc.Height);
                a_position.UnsafeLoadMixedV3f(imgVertices, 7);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 7);
                _texCoord_color.UnsafeLoadMixedV2f(imgVertices + 5, 7);
            }


            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
        public void DrawSubImage2(in CpuBlit.VertexProcessing.Quad2f quad,
          in PixelFarm.Drawing.RectangleF maskSrc,
          float colorSrcX, float colorSrcY,
          float targetLeft, float targetTop)
        {

            //-------------------------------------------------------------------------------------
            SetVarsBeforeRender();
            //-------------------------------------------------------------------------------------           
            var srcRect = new CpuBlit.VertexProcessing.Quad2f(maskSrc.Left, maskSrc.Top, maskSrc.Width, maskSrc.Height, _latestBmpW, _latestBmpH);
            var colorSrc = new CpuBlit.VertexProcessing.Quad2f(colorSrcX, colorSrcY, maskSrc.Width, maskSrc.Height, _colorBmpW, _colorBmpH);

            unsafe
            {
                float* imgVertices = stackalloc float[7 * 4];
                AssignVertice7_4(quad, srcRect, colorSrc, imgVertices, !_latestBmpYFlipped);


                //_colorSrcOffset.SetValue(_colorSrcOffsetX / maskSrc.Width, _colorSrcOffsetY / maskSrc.Height);
                a_position.UnsafeLoadMixedV3f(imgVertices, 7);
                a_texCoord.UnsafeLoadMixedV2f(imgVertices + 3, 7);
                _texCoord_color.UnsafeLoadMixedV2f(imgVertices + 5, 7);
            }

            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }

    sealed class OneColorMaskShader : MaskShaderBase
    {
        public OneColorMaskShader(ShaderSharedResource shareRes)
           : base(shareRes)
        {

            string vs = @"                 
            attribute vec4 a_position;
            attribute vec2 a_texCoord;
            attribute vec2 a_texCoord_color;

            uniform vec2 u_ortho_offset;
            uniform vec2 u_offset;
            uniform mat4 u_mvpMatrix;            
            uniform vec2 u_color_src_offset;

            varying vec2 v_texCoord;
            varying vec2 v_color_texCoord;

            void main()
            {
                gl_Position = u_mvpMatrix * (a_position + vec4(u_offset + u_ortho_offset, 0, 0));
                v_texCoord = a_texCoord;
                v_color_texCoord = a_texCoord_color + u_color_src_offset;
            }
            ";
            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture;
                      uniform sampler2D s_color_src;

                      varying vec2 v_texCoord; 
                      varying vec2 v_color_texCoord;
                      void main()
                      {   
                            vec4 m = texture2D(s_texture,v_texCoord);
                            vec4 c = texture2D(s_color_src,v_color_texCoord);                            
                            gl_FragColor= vec4(c[0], c[1], c[2] , c[3] * m[0]);                            
                      }
                ";
            //debug
            //gl_FragColor= vec4(m[0],m[1],m[2],m[3]);
            //gl_FragColor= vec4(c[2],c[1],c[0],c[3]); 
            BuildProgram(vs, fs);
        }
    }

    sealed class TwoColorMaskShader : MaskShaderBase
    {
        //This is a special version of mask shader
        //background-color: black,
        //inside shape: white,
        //border: red  

        public TwoColorMaskShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vs = @"                 
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                attribute vec2 a_texCoord_color;

                uniform vec2 u_ortho_offset;
                uniform vec2 u_offset;                
                uniform mat4 u_mvpMatrix; 
                uniform vec2 u_color_src_offset;

                varying vec2 v_texCoord; 
                varying vec2 v_color_texCoord; 

                void main()
                {                      
                    gl_Position = u_mvpMatrix* (a_position+ vec4(u_offset+u_ortho_offset,0,0));
                    v_texCoord =  a_texCoord;
                    v_color_texCoord= a_texCoord_color +u_color_src_offset;
                }	 
               ";


            //version 2.2: this version does not use if else
            //if m[0]< 1.0 then we use m[0], not use m[2]
            //else we don't use m[0], we switch to use m[2],
            //so we use floor(m[0]), result will be 0 or 1 for m[2], (you see the code m[2]*c2_compo)
            //and if c2_compo=0, then => 1.0-c2_compo => 1.0 (here we use only m[1])           

            //TODO: test performance between 2.1 vs 2,2,
            //compare between if-else and not use if-else

            string fs = @"
                      precision mediump float; 
                      uniform sampler2D s_texture;
                      uniform sampler2D s_color_src;
                       
                      varying vec2 v_texCoord; 
                      varying vec2 v_color_texCoord;
                      void main()
                      {   
                            vec4 m = texture2D(s_texture,v_texCoord);
                            vec4 c = texture2D(s_color_src,v_color_texCoord);
                            float c2_compo= floor(m[0]);
                            gl_FragColor= vec4(c[0], c[1], c[2] , c[3] * (m[2]*c2_compo + m[0]*(1.0-c2_compo)));                            
                      }
                ";

            //old version 2.1 , use if-else
            //string fs = @"
            //          precision mediump float; 
            //          uniform sampler2D s_texture;
            //          uniform sampler2D s_color_src;

            //          varying vec2 v_texCoord; 
            //          varying vec2 v_color_texCoord;
            //          void main()
            //          {   
            //                vec4 m = texture2D(s_texture,v_texCoord);
            //                vec4 c = texture2D(s_color_src,v_color_texCoord);
            //                if(m[0]< 1.0){
            //                   gl_FragColor= vec4(c[2], c[1], c[0] , c[3] * m[0]);  
            //                }else{                              
            //                    gl_FragColor= vec4(c[2], c[1], c[0] , c[3] * m[2]);
            //                }
            //          }
            //    ";

            //debug
            //gl_FragColor= vec4(m[0],m[1],m[2],m[3]);
            //gl_FragColor= vec4(c[2],c[1],c[0],c[3]); 
            BuildProgram(vs, fs);
        }
    }

    //--------------------------------------------------------
    static class SimpleRectTextureShaderExtensions
    {

        public static void DrawSubImage(this SimpleRectTextureShader shader, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            unsafe
            {
                float* srcDestList = stackalloc float[]
                {
                    srcLeft,
                    srcTop,
                    srcW,
                    srcH,
                    targetLeft,
                    targetTop
                };
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
                float* srcDestList = stackalloc float[]
                {
                    srcLeft,
                    srcTop,
                    srcW,
                    srcH,
                    targetLeft,
                    targetTop
                };

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