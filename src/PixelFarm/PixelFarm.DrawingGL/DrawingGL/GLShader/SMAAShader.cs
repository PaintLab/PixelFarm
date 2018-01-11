//MIT, 2018, mpk (http://polko.me/)
//from three.js
///**
// * @author mpk / http://polko.me/
// *
// * WebGL port of Subpixel Morphological Antialiasing (SMAA) v2.8 
// * https://github.com/iryoku/smaa/releases/tag/v2.8
// */
//
//MIT, 2018,  https://github.com/iryoku/smaa/releases/tag/v2.8
//

using System;
using System.Text;
using OpenTK.Graphics.ES20;

namespace PixelFarm.DrawingGL
{

    static class StringArrayConcatHelper
    {
        public static string JoinWithNewLine(this string[] strArr)
        {
            return Join(strArr, "\n");
        }
        public static string Join(this string[] strArr, string sep)
        {
            StringBuilder stbuilder = new StringBuilder();
            if (sep == null)
            {
                sep = "";
            }

            int j = strArr.Length;
            for (int i = 0; i < j; ++i)
            {
                if (i > 0)
                {
                    stbuilder.Append(sep);
                }
                stbuilder.Append(strArr[i]);
            }

            return stbuilder.ToString();
        }
    }



    class SMAAPass
    {
        int _width;
        int _height;

        FrameBuffer _edgesRT;//edge render target
        FrameBuffer _weightsRT; //weight render target

        SMAAColorEdgeDetectionShader _edgeDetect; //edge
        SMAABlendingWeightCalculationShader _blendWeight; //weight
        SMAANeighborhoodBlendingShader _nbBlending; //lend


        public SMAAPass(int width, int height, ShaderSharedResource shareRes)
        {
            _width = width;
            _height = height;


            this._edgesRT = new FrameBuffer(width, height, new FrameBufferCreationParameters()
            {
                depthBuffer = false,
                stencilBuffer = false,
                generateMipMaps = false,
                minFilter = TextureMinFilter.Linear,
                pixelFormat = PixelFormat.Rgb//*** RGB
            });
            //
            this._weightsRT = new FrameBuffer(width, height, new FrameBufferCreationParameters()
            {
                depthBuffer = false,
                stencilBuffer = false,
                generateMipMaps = false,
                minFilter = TextureMinFilter.Linear,
                pixelFormat = PixelFormat.Rgba//*** RGBA

            });

            _edgeDetect = new SMAAColorEdgeDetectionShader(shareRes);
            _edgeDetect.SetResolution(1 / width, 1 / height);
            //
            _blendWeight = new SMAABlendingWeightCalculationShader(shareRes);
            _blendWeight.SetResolution(1 / width, 1 / height);
            //_blendWeight.LoadDiffuseTexture(null);
            //_blendWeight.LoadAreaTexture(null);
            //_blendWeight.LoadSearchTexture(null);


            //
            _nbBlending = new SMAANeighborhoodBlendingShader(shareRes);
            _nbBlending.SetResolution(1 / width, 1 / height);
            //_nbBlending.LoadDiffuseTexture(null);
        }
        public void Render()
        {

        }

    }

    abstract class SMAAShaderBase : ShaderBase
    {
        protected static readonly ushort[] indices = new ushort[] { 0, 1, 2, 3 };
        protected ShaderUniformVar1 tDiffuse; //texture diffuse
        protected ShaderUniformMatrix4 u_matrix;
        protected float resolution_x = 1 / 1024f;
        protected float resolution_y = 1 / 512f;
        public SMAAShaderBase(ShaderSharedResource shareRes)
            : base(shareRes)
        {
        }
        protected bool BuildProgram(string vs, string fs)
        {
            //---------------------
            if (!shaderProgram.Build(vs, fs))
            {
                return false;
            }
            //-----------------------
            //a_position = shaderProgram.GetAttrV3f("a_position");
            //a_texCoord = shaderProgram.GetAttrV2f("a_texCoord");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            tDiffuse = shaderProgram.GetUniform1("tDiffuse");
            OnProgramBuilt();
            return true;
        }
        protected virtual void OnProgramBuilt()
        {
        }
        public void SetResolution(float resolution_x, float resolution_y)
        {
            this.resolution_x = resolution_x;
            this.resolution_y = resolution_y;
        }
        protected virtual void OnSetVarsBeforeRenderer()
        {

        }


        int orthoviewVersion = -1;
        protected void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _shareRes.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_shareRes.OrthoView.data);
            }
        }



//        //-----------------------------------------
//#if DEBUG
//        float _latestBmpW;
//        float _latestBmpH;
//        bool _latestBmpInverted;
//#endif
//        /// <summary>
//        /// load glbmp before draw
//        /// </summary>
//        /// <param name="bmp"></param>
//        public void LoadDiffuseTexture(GLBitmap bmp)
//        {
//            //load before use with RenderSubImage
//            SetCurrent();
//            CheckViewMatrix();
//            //-------------------------------------------------------------------------------------
//            // Bind the texture...
//            GL.ActiveTexture(TextureUnit.Texture0);
//            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
//            // Set the texture sampler to texture unit to 0                  
//            tDiffuse.SetValue(0);
//#if DEBUG
//            this._latestBmpW = bmp.Width;
//            this._latestBmpH = bmp.Height;
//            this._latestBmpInverted = bmp.IsInvert;
//#endif
//        }

    }

    /// <summary>
    /// SMAA EdgeDetection shader
    /// </summary>
    class SMAAColorEdgeDetectionShader : SMAAShaderBase
    {
        ShaderVtxAttrib3f position;
        ShaderVtxAttrib2f uv;//uv texture coord
        ShaderUniformVar2 u_resolution;
        public SMAAColorEdgeDetectionShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {

            //vertex shader
            string vertexShader = new[] {
                "#define SMAA_THRESHOLD 0.1",
                "precision mediump float;",

                "attribute vec3 position;",
                "attribute vec2 uv;",

                "uniform vec2 resolution;",
                "uniform sampler2D tDiffuse;",
                "uniform mat4 u_mvpMatrix;",

                "varying vec2 vUv;",
                "varying vec4 vOffset[ 3 ];",

                "void SMAAEdgeDetectionVS( vec2 texcoord ) {",
                    "vOffset[ 0 ] = texcoord.xyxy + resolution.xyxy * vec4( -1.0, 0.0, 0.0,  1.0 );", // WebGL port note: Changed sign in W component
			        "vOffset[ 1 ] = texcoord.xyxy + resolution.xyxy * vec4(  1.0, 0.0, 0.0, -1.0 );", // WebGL port note: Changed sign in W component
			        "vOffset[ 2 ] = texcoord.xyxy + resolution.xyxy * vec4( -2.0, 0.0, 0.0,  2.0 );", // WebGL port note: Changed sign in W component
		        "}",

                "void main() {",

                    "vUv = uv;",

                    "SMAAEdgeDetectionVS( vUv );",

                     "gl_Position = u_mvpMatrix * vec4( position, 1.0 );",
                    
                    //"gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",

                "}"

            }.JoinWithNewLine();


            string fragmentShader = new[] {
                "#define SMAA_THRESHOLD 0.1",
                "precision mediump float;",

                "uniform sampler2D tDiffuse;",

                "varying vec2 vUv;",
                "varying vec4 vOffset[ 3 ];",

                "vec4 SMAAColorEdgeDetectionPS( vec2 texcoord, vec4 offset[3], sampler2D colorTex ) {",
                    "vec2 threshold = vec2( SMAA_THRESHOLD, SMAA_THRESHOLD );",

			        // Calculate color deltas:
			        "vec4 delta;",
                    "vec3 C = texture2D( colorTex, texcoord ).rgb;",

                    "vec3 Cleft = texture2D( colorTex, offset[0].xy ).rgb;",
                    "vec3 t = abs( C - Cleft );",
                    "delta.x = max( max( t.r, t.g ), t.b );",

                    "vec3 Ctop = texture2D( colorTex, offset[0].zw ).rgb;",
                    "t = abs( C - Ctop );",
                    "delta.y = max( max( t.r, t.g ), t.b );",

			        // We do the usual threshold:
			        "vec2 edges = step( threshold, delta.xy );",

			        // Then discard if there is no edge:
			        "if ( dot( edges, vec2( 1.0, 1.0 ) ) == 0.0 )",
                        "discard;",

			        // Calculate right and bottom deltas:
			        "vec3 Cright = texture2D( colorTex, offset[1].xy ).rgb;",
                    "t = abs( C - Cright );",
                    "delta.z = max( max( t.r, t.g ), t.b );",

                    "vec3 Cbottom  = texture2D( colorTex, offset[1].zw ).rgb;",
                    "t = abs( C - Cbottom );",
                    "delta.w = max( max( t.r, t.g ), t.b );",

			        // Calculate the maximum delta in the direct neighborhood:
			        "float maxDelta = max( max( max( delta.x, delta.y ), delta.z ), delta.w );",

			        // Calculate left-left and top-top deltas:
			        "vec3 Cleftleft  = texture2D( colorTex, offset[2].xy ).rgb;",
                    "t = abs( C - Cleftleft );",
                    "delta.z = max( max( t.r, t.g ), t.b );",

                    "vec3 Ctoptop = texture2D( colorTex, offset[2].zw ).rgb;",
                    "t = abs( C - Ctoptop );",
                    "delta.w = max( max( t.r, t.g ), t.b );",

			        // Calculate the final maximum delta:
			        "maxDelta = max( max( maxDelta, delta.z ), delta.w );",

			        // Local contrast adaptation in action:
			        "edges.xy *= step( 0.5 * maxDelta, delta.xy );",

                    "return vec4( edges, 0.0, 0.0 );",
                "}",

                "void main() {",

                    "gl_FragColor = SMAAColorEdgeDetectionPS( vUv, vOffset, tDiffuse );",

                "}"
            }.JoinWithNewLine();
            BuildProgram(vertexShader, fragmentShader);
        }
        protected override void OnProgramBuilt()
        {
            position = shaderProgram.GetAttrV3f("position");
            uv = shaderProgram.GetAttrV2f("uv");

            u_resolution = shaderProgram.GetUniform2("resolution");

            base.OnProgramBuilt();
        }
        protected override void OnSetVarsBeforeRenderer()
        {
            u_resolution.SetValue(resolution_x, resolution_y);
        }



        public void Render(GLBitmap bmp, float left, float top, float w, float h)
        {
            unsafe
            {
                if (bmp.IsInvert)
                {

                    float* imgVertices = stackalloc float[5 * 4];
                    {
                        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)
                        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                        //imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

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
                    position.UnsafeLoadMixedV3f(imgVertices, 5);
                    uv.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
                else
                {
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
                    position.UnsafeLoadMixedV3f(imgVertices, 5);
                    uv.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                }
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0     
           
            tDiffuse.SetValue(0);
            OnSetVarsBeforeRenderer();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }

    /// <summary>
    /// SMAA BlendingWeightCalculation shader
    /// </summary>
    class SMAABlendingWeightCalculationShader : SMAAShaderBase
    {
        ShaderUniformVar2 u_resolution;
        ShaderUniformVar1 tArea; //texture diffuse
        ShaderUniformVar1 tSearch; //texture diffuse

        public SMAABlendingWeightCalculationShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vertexShader = new[]
            {
                "#define SMAA_MAX_SEARCH_STEPS 8",
                "#define SMAA_AREATEX_MAX_DISTANCE 16",
                "#define SMAA_AREATEX_PIXEL_SIZE (1.0 / vec2(160.0, 560.0))",

                "uniform sampler2D tDiffuse;",
                "uniform sampler2D tArea;",
                "uniform sampler2D tSearch;",
                "uniform vec2 resolution;",
                //

                "varying vec2 vUv;",
                "varying vec4 vOffset[ 3 ];",
                "varying vec2 vPixcoord;",

                "void SMAABlendingWeightCalculationVS( vec2 texcoord ) {",
                    "vPixcoord = texcoord / resolution;",

			        // We will use these offsets for the searches later on (see @PSEUDO_GATHER4):
			        "vOffset[ 0 ] = texcoord.xyxy + resolution.xyxy * vec4( -0.25, 0.125, 1.25, 0.125 );", // WebGL port note: Changed sign in Y and W components
			        "vOffset[ 1 ] = texcoord.xyxy + resolution.xyxy * vec4( -0.125, 0.25, -0.125, -1.25 );", // WebGL port note: Changed sign in Y and W components

			        // And these for the searches, they indicate the ends of the loops:
			        "vOffset[ 2 ] = vec4( vOffset[ 0 ].xz, vOffset[ 1 ].yw ) + vec4( -2.0, 2.0, -2.0, 2.0 ) * resolution.xxyy * float( SMAA_MAX_SEARCH_STEPS );",

                "}",

                "void main() {",

                    "vUv = uv;",

                    "SMAABlendingWeightCalculationVS( vUv );",

                    "gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",

                "}"
            }.JoinWithNewLine();
            //
            string fragmentShader = new[]
            {
                "#define SMAASampleLevelZeroOffset( tex, coord, offset ) texture2D( tex, coord + float( offset ) * resolution, 0.0 )",

                "uniform sampler2D tDiffuse;",
                "uniform sampler2D tArea;",
                "uniform sampler2D tSearch;",
                "uniform vec2 resolution;",

                "varying vec2 vUv;",
                "varying vec4 vOffset[3];",
                "varying vec2 vPixcoord;",

                "vec2 round( vec2 x ) {",
                    "return sign( x ) * floor( abs( x ) + 0.5 );",
                "}",

                "float SMAASearchLength( sampler2D searchTex, vec2 e, float bias, float scale ) {",
			        // Not required if searchTex accesses are set to point:
			        // float2 SEARCH_TEX_PIXEL_SIZE = 1.0 / float2(66.0, 33.0);
			        // e = float2(bias, 0.0) + 0.5 * SEARCH_TEX_PIXEL_SIZE +
			        //     e * float2(scale, 1.0) * float2(64.0, 32.0) * SEARCH_TEX_PIXEL_SIZE;
			        "e.r = bias + e.r * scale;",
                    "return 255.0 * texture2D( searchTex, e, 0.0 ).r;",
                "}",

                "float SMAASearchXLeft( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
			        /**
			        * @PSEUDO_GATHER4
			        * This texcoord has been offset by (-0.25, -0.125) in the vertex shader to
			        * sample between edge, thus fetching four edges in a row.
			        * Sampling with different offsets in each direction allows to disambiguate
			        * which edges are active from the four fetched ones.
			        */
			        "vec2 e = vec2( 0.0, 1.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord -= vec2( 2.0, 0.0 ) * resolution;",
                        "if ( ! ( texcoord.x > end && e.g > 0.8281 && e.r == 0.0 ) ) break;",
                    "}",

			        // We correct the previous (-0.25, -0.125) offset we applied:
			        "texcoord.x += 0.25 * resolution.x;",

			        // The searches are bias by 1, so adjust the coords accordingly:
			        "texcoord.x += resolution.x;",

			        // Disambiguate the length added by the last step:
			        "texcoord.x += 2.0 * resolution.x;", // Undo last step
			        "texcoord.x -= resolution.x * SMAASearchLength(searchTex, e, 0.0, 0.5);",

                    "return texcoord.x;",
                "}",

                "float SMAASearchXRight( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
                    "vec2 e = vec2( 0.0, 1.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord += vec2( 2.0, 0.0 ) * resolution;",
                        "if ( ! ( texcoord.x < end && e.g > 0.8281 && e.r == 0.0 ) ) break;",
                    "}",

                    "texcoord.x -= 0.25 * resolution.x;",
                    "texcoord.x -= resolution.x;",
                    "texcoord.x -= 2.0 * resolution.x;",
                    "texcoord.x += resolution.x * SMAASearchLength( searchTex, e, 0.5, 0.5 );",

                    "return texcoord.x;",
                "}",

                "float SMAASearchYUp( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
                    "vec2 e = vec2( 1.0, 0.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord += vec2( 0.0, 2.0 ) * resolution;", // WebGL port note: Changed sign
				        "if ( ! ( texcoord.y > end && e.r > 0.8281 && e.g == 0.0 ) ) break;",
                    "}",

                    "texcoord.y -= 0.25 * resolution.y;", // WebGL port note: Changed sign
			        "texcoord.y -= resolution.y;", // WebGL port note: Changed sign
			        "texcoord.y -= 2.0 * resolution.y;", // WebGL port note: Changed sign
			        "texcoord.y += resolution.y * SMAASearchLength( searchTex, e.gr, 0.0, 0.5 );", // WebGL port note: Changed sign

			        "return texcoord.y;",
                "}",

                "float SMAASearchYDown( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
                    "vec2 e = vec2( 1.0, 0.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord -= vec2( 0.0, 2.0 ) * resolution;", // WebGL port note: Changed sign
				        "if ( ! ( texcoord.y < end && e.r > 0.8281 && e.g == 0.0 ) ) break;",
                    "}",

                    "texcoord.y += 0.25 * resolution.y;", // WebGL port note: Changed sign
			        "texcoord.y += resolution.y;", // WebGL port note: Changed sign
			        "texcoord.y += 2.0 * resolution.y;", // WebGL port note: Changed sign
			        "texcoord.y -= resolution.y * SMAASearchLength( searchTex, e.gr, 0.5, 0.5 );", // WebGL port note: Changed sign

			        "return texcoord.y;",
                "}",

                "vec2 SMAAArea( sampler2D areaTex, vec2 dist, float e1, float e2, float offset ) {",
			        // Rounding prevents precision errors of bilinear filtering:
			        "vec2 texcoord = float( SMAA_AREATEX_MAX_DISTANCE ) * round( 4.0 * vec2( e1, e2 ) ) + dist;",

			        // We do a scale and bias for mapping to texel space:
			        "texcoord = SMAA_AREATEX_PIXEL_SIZE * texcoord + ( 0.5 * SMAA_AREATEX_PIXEL_SIZE );",

			        // Move to proper place, according to the subpixel offset:
			        "texcoord.y += SMAA_AREATEX_SUBTEX_SIZE * offset;",

                    "return texture2D( areaTex, texcoord, 0.0 ).rg;",
                "}",

                "vec4 SMAABlendingWeightCalculationPS( vec2 texcoord, vec2 pixcoord, vec4 offset[ 3 ], sampler2D edgesTex, sampler2D areaTex, sampler2D searchTex, ivec4 subsampleIndices ) {",
                    "vec4 weights = vec4( 0.0, 0.0, 0.0, 0.0 );",

                    "vec2 e = texture2D( edgesTex, texcoord ).rg;",

                    "if ( e.g > 0.0 ) {", // Edge at north
				        "vec2 d;",

				        // Find the distance to the left:
				        "vec2 coords;",
                        "coords.x = SMAASearchXLeft( edgesTex, searchTex, offset[ 0 ].xy, offset[ 2 ].x );",
                        "coords.y = offset[ 1 ].y;", // offset[1].y = texcoord.y - 0.25 * resolution.y (@CROSSING_OFFSET)
				        "d.x = coords.x;",

				        // Now fetch the left crossing edges, two at a time using bilinear
				        // filtering. Sampling at -0.25 (see @CROSSING_OFFSET) enables to
				        // discern what value each edge has:
				        "float e1 = texture2D( edgesTex, coords, 0.0 ).r;",

				        // Find the distance to the right:
				        "coords.x = SMAASearchXRight( edgesTex, searchTex, offset[ 0 ].zw, offset[ 2 ].y );",
                        "d.y = coords.x;",

				        // We want the distances to be in pixel units (doing this here allow to
				        // better interleave arithmetic and memory accesses):
				        "d = d / resolution.x - pixcoord.x;",

				        // SMAAArea below needs a sqrt, as the areas texture is compressed
				        // quadratically:
				        "vec2 sqrt_d = sqrt( abs( d ) );",

				        // Fetch the right crossing edges:
				        "coords.y -= 1.0 * resolution.y;", // WebGL port note: Added
				        "float e2 = SMAASampleLevelZeroOffset( edgesTex, coords, ivec2( 1, 0 ) ).r;",

				        // Ok, we know how this pattern looks like, now it is time for getting
				        // the actual area:
				        "weights.rg = SMAAArea( areaTex, sqrt_d, e1, e2, float( subsampleIndices.y ) );",
                    "}",

                    "if ( e.r > 0.0 ) {", // Edge at west
				        "vec2 d;",

				        // Find the distance to the top:
				        "vec2 coords;",

                        "coords.y = SMAASearchYUp( edgesTex, searchTex, offset[ 1 ].xy, offset[ 2 ].z );",
                        "coords.x = offset[ 0 ].x;", // offset[1].x = texcoord.x - 0.25 * resolution.x;
				        "d.x = coords.y;",

				        // Fetch the top crossing edges:
				        "float e1 = texture2D( edgesTex, coords, 0.0 ).g;",

				        // Find the distance to the bottom:
				        "coords.y = SMAASearchYDown( edgesTex, searchTex, offset[ 1 ].zw, offset[ 2 ].w );",
                        "d.y = coords.y;",

				        // We want the distances to be in pixel units:
				        "d = d / resolution.y - pixcoord.y;",

				        // SMAAArea below needs a sqrt, as the areas texture is compressed
				        // quadratically:
				        "vec2 sqrt_d = sqrt( abs( d ) );",

				        // Fetch the bottom crossing edges:
				        "coords.y -= 1.0 * resolution.y;", // WebGL port note: Added
				        "float e2 = SMAASampleLevelZeroOffset( edgesTex, coords, ivec2( 0, 1 ) ).g;",

				        // Get the area for this direction:
				        "weights.ba = SMAAArea( areaTex, sqrt_d, e1, e2, float( subsampleIndices.x ) );",
                    "}",

                    "return weights;",
                "}",

                "void main() {",

                    "gl_FragColor = SMAABlendingWeightCalculationPS( vUv, vPixcoord, vOffset, tDiffuse, tArea, tSearch, ivec4( 0.0 ) );",

                "}"
            }.JoinWithNewLine();

            BuildProgram(vertexShader, fragmentShader);

        }
        protected override void OnProgramBuilt()
        {
            u_resolution = shaderProgram.GetUniform2("resolution");
            tArea = shaderProgram.GetUniform1("tArea");
            tSearch = shaderProgram.GetUniform1("tSearch");

        }


        protected override void OnSetVarsBeforeRenderer()
        {
            u_resolution.SetValue(resolution_x, resolution_y);
        }


        //-----------------------------------------
#if DEBUG
        float _latestBmpW;
        float _latestBmpH;
        bool _latestBmpInverted;
#endif
        /// <summary>
        /// load glbmp before draw
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadAreaTexture(GLBitmap bmp)
        {
            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0                  
            tDiffuse.SetValue(0);
#if DEBUG
            this._latestBmpW = bmp.Width;
            this._latestBmpH = bmp.Height;
            this._latestBmpInverted = bmp.IsInvert;
#endif
        }
        /// <summary>
        /// load glbmp before draw
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadSearchTexture(GLBitmap bmp)
        {
            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0                  
            tDiffuse.SetValue(0);
#if DEBUG
            this._latestBmpW = bmp.Width;
            this._latestBmpH = bmp.Height;
            this._latestBmpInverted = bmp.IsInvert;
#endif
        }
    }

    /// <summary>
    /// SMAA Neighborhood shader
    /// </summary>
    class SMAANeighborhoodBlendingShader : SMAAShaderBase
    {
        ShaderUniformVar2 u_resolution;


        public SMAANeighborhoodBlendingShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vertexShader = new[] {
                "uniform vec2 resolution;",
                "varying vec2 vUv;",
                "varying vec4 vOffset[ 2 ];",
                "void SMAANeighborhoodBlendingVS( vec2 texcoord ) {",
                    "vOffset[ 0 ] = texcoord.xyxy + resolution.xyxy * vec4( -1.0, 0.0, 0.0, 1.0 );", // WebGL port note: Changed sign in W component
			        "vOffset[ 1 ] = texcoord.xyxy + resolution.xyxy * vec4( 1.0, 0.0, 0.0, -1.0 );", // WebGL port note: Changed sign in W component
		        "}",
                "void main() {",

                    "vUv = uv;",

                    "SMAANeighborhoodBlendingVS( vUv );",

                    "gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",

                "}"}.JoinWithNewLine();

            string fragmentShader = new[]
            {
                "uniform sampler2D tDiffuse;",
                "uniform sampler2D tColor;",
                "uniform vec2 resolution;",

                "varying vec2 vUv;",
                "varying vec4 vOffset[ 2 ];",

                "vec4 SMAANeighborhoodBlendingPS( vec2 texcoord, vec4 offset[ 2 ], sampler2D colorTex, sampler2D blendTex ) {",
			        // Fetch the blending weights for current pixel:
			        "vec4 a;",
                    "a.xz = texture2D( blendTex, texcoord ).xz;",
                    "a.y = texture2D( blendTex, offset[ 1 ].zw ).g;",
                    "a.w = texture2D( blendTex, offset[ 1 ].xy ).a;",

			        // Is there any blending weight with a value greater than 0.0?
			        "if ( dot(a, vec4( 1.0, 1.0, 1.0, 1.0 )) < 1e-5 ) {",
                        "return texture2D( colorTex, texcoord, 0.0 );",
                    "} else {",
				        // Up to 4 lines can be crossing a pixel (one through each edge). We
				        // favor blending by choosing the line with the maximum weight for each
				        // direction:
				        "vec2 offset;",
                        "offset.x = a.a > a.b ? a.a : -a.b;", // left vs. right
				        "offset.y = a.g > a.r ? -a.g : a.r;", // top vs. bottom // WebGL port note: Changed signs

				        // Then we go in the direction that has the maximum weight:
				        "if ( abs( offset.x ) > abs( offset.y )) {", // horizontal vs. vertical
					        "offset.y = 0.0;",
                        "} else {",
                            "offset.x = 0.0;",
                        "}",

				        // Fetch the opposite color and lerp by hand:
				        "vec4 C = texture2D( colorTex, texcoord, 0.0 );",
                        "texcoord += sign( offset ) * resolution;",
                        "vec4 Cop = texture2D( colorTex, texcoord, 0.0 );",
                        "float s = abs( offset.x ) > abs( offset.y ) ? abs( offset.x ) : abs( offset.y );",

				        // WebGL port note: Added gamma correction
				        "C.xyz = pow(C.xyz, vec3(2.2));",
                        "Cop.xyz = pow(Cop.xyz, vec3(2.2));",
                        "vec4 mixed = mix(C, Cop, s);",
                        "mixed.xyz = pow(mixed.xyz, vec3(1.0 / 2.2));",

                        "return mixed;",
                    "}",
                "}",

                "void main() {",

                    "gl_FragColor = SMAANeighborhoodBlendingPS( vUv, vOffset, tColor, tDiffuse );",

                "}"
            }.JoinWithNewLine();

            BuildProgram(vertexShader, fragmentShader);

        }
        protected override void OnProgramBuilt()
        {
            u_resolution = shaderProgram.GetUniform2("resolution");
        }


        protected override void OnSetVarsBeforeRenderer()
        {
            u_resolution.SetValue(resolution_x, resolution_y);
        }
    }

}

