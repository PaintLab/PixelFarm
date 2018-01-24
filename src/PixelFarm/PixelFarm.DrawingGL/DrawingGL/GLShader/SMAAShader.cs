//MIT, 2018,  https://github.com/iryoku/smaa/releases/tag/v2.8
/**
 * Copyright (C) 2013 Jorge Jimenez (jorge@iryoku.com)
 * Copyright (C) 2013 Jose I. Echevarria (joseignacioechevarria@gmail.com)
 * Copyright (C) 2013 Belen Masia (bmasia@unizar.es)
 * Copyright (C) 2013 Fernando Navarro (fernandn@microsoft.com)
 * Copyright (C) 2013 Diego Gutierrez (diegog@unizar.es)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to
 * do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software. As clarification, there
 * is no requirement that the copyright notice and permission be included in
 * binary distributions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

//some parts are from  WebGL port of Subpixel Morphological Antialiasing (SMAA) v2.8 
//MIT, 2018, mpk (http://polko.me/)
//from three.js

//MIT, 2018, WinterDev
//-----------------------------------------------------------------------------------


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

    abstract class SMAAShaderBase : ShaderBase
    {
        protected static readonly ushort[] indices = new ushort[] { 0, 1, 2, 3 };
        protected ShaderUniformVar1 tDiffuse; //texture diffuse
        protected ShaderUniformMatrix4 u_matrix;
        protected float resolution_x = 1 / 1024f;
        protected float resolution_y = 1 / 512f;
        protected float resolution_z = 1024;
        protected float resolution_w = 512;

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


            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            tDiffuse = shaderProgram.GetUniform1("tDiffuse");
            OnProgramBuilt();
            return true;
        }
        protected virtual void OnProgramBuilt()
        {
        }
        public void SetResolution(float rs_x, float rs_y)
        {
            this.resolution_x = 1f / rs_x;
            this.resolution_y = 1f / rs_y;
            this.resolution_z = rs_x;
            this.resolution_w = rs_y;
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
            //Edge Detection Vertex Shader 
            string vertexShader = new[] {

                "#define mad(a, b, c) (a * b + c)",
                "precision mediump float;", //**

                "attribute vec3 position;", //**
                "attribute vec2 uv;",//**
                "uniform mat4 u_mvpMatrix;",//**


                "uniform vec2 resolution;",
                "uniform sampler2D tDiffuse;",


                "varying vec2 vUv;",
                "varying vec4 vOffset[ 3 ];",

                "void SMAAEdgeDetectionVS( vec2 texcoord ) {",

                    "vOffset[ 0 ] = mad(resolution.xyxy, vec4( -1.0, 0.0, 0.0, -1.0 ),  texcoord.xyxy);",
                    "vOffset[ 1 ] = mad(resolution.xyxy, vec4( 1.0, 0.0, 0.0,   1.0 ),  texcoord.xyxy);",
                    "vOffset[ 2 ] = mad(resolution.xyxy, vec4( -2.0, 0.0, 0.0, -2.0 ),  texcoord.xyxy);",

                "}",

                "void main() {",

                    "vUv = uv;",

                    "SMAAEdgeDetectionVS( vUv );",

                     "gl_Position = u_mvpMatrix * vec4( position, 1.0 );",
                    
                    //"gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",

                "}"

            }.JoinWithNewLine();

            //Edge Detection Pixel Shaders (First Pass)
            //Color Edge Detection
            //IMPORTANT NOTICE: color edge detection requires gamma - corrected colors, and
            //thus 'colorTex' should be a non-sRGB texture.

            string fragmentShader = new[] {
                "#define SMAA_THRESHOLD 0.1",
                "#define SMAA_LOCAL_CONTRAST_ADAPTATION_FACTOR 2",
                "precision mediump float;", //**

                "uniform sampler2D tDiffuse;",

                "varying vec2 vUv;",
                "varying vec4 vOffset[ 3 ];",

                "vec2 SMAAColorEdgeDetectionPS( vec2 texcoord, vec4 offset[3], sampler2D colorTex ) {",
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
			        "if ( dot( edges, vec2( 1.0, 1.0 ) ) == 0.0 ){",
                        //"return vec4( 0.0,0.0, 0.0, 1.0 );", //for debug
                        "discard;",
                    "}",

			        // Calculate right and bottom deltas:
			        "vec3 Cright = texture2D( colorTex, offset[1].xy ).rgb;",
                    "t = abs( C - Cright );",
                    "delta.z = max( max( t.r, t.g ), t.b );",

                    "vec3 Cbottom  = texture2D( colorTex, offset[1].zw ).rgb;",
                    "t = abs( C - Cbottom );",
                    "delta.w = max( max( t.r, t.g ), t.b );",

			        // Calculate the maximum delta in the direct neighborhood:			         
                    "vec2 maxDelta= max(delta.xy, delta.zw);",

			        // Calculate left-left and top-top deltas:
			        "vec3 Cleftleft  = texture2D( colorTex, offset[2].xy ).rgb;",
                    "t = abs( C - Cleftleft );",
                    "delta.z = max( max( t.r, t.g ), t.b );",

                    "vec3 Ctoptop = texture2D( colorTex, offset[2].zw ).rgb;",
                    "t = abs( C - Ctoptop );",
                    "delta.w = max( max( t.r, t.g ), t.b );",

			        // Calculate the final maximum delta: 
                    "maxDelta = max(maxDelta.xy, delta.zw);",
                    "float finalDelta = max(maxDelta.x, maxDelta.y);",

			        // Local contrast adaptation in action: 
                    "edges.xy *= step(finalDelta, float(SMAA_LOCAL_CONTRAST_ADAPTATION_FACTOR) * delta.xy);",
                    "return edges;",
                "}",

                "void main() {",
                    "gl_FragColor =vec4(SMAAColorEdgeDetectionPS( vUv, vOffset, tDiffuse ),0.0,0.0);",
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



        public void Render(FrameBuffer bmp, float left, float top, float w, float h)
        {
            unsafe
            {
                //if (bmp.IsInvert)
                //{

                //    float* imgVertices = stackalloc float[5 * 4];
                //    {
                //        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)
                //        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                //        //imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                //        //---------------------
                //        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                //        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                //        //---------------------
                //        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                //        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                //        //---------------------
                //        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                //        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)
                //    }
                //    position.UnsafeLoadMixedV3f(imgVertices, 5);
                //    uv.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                //}
                //else
                //{
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
                //}
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.FrameBufferId);
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
        ShaderVtxAttrib3f position;
        ShaderUniformVar4 u_resolution;
        ShaderUniformVar1 tArea; //texture diffuse
        ShaderUniformVar1 tSearch; //texture diffuse 
        ShaderVtxAttrib2f uv;//uv texture coord



        public SMAABlendingWeightCalculationShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {


            //Blend Weight Calculation Vertex Shader
            string vertexShader = new[]
            {
                "#define mad(a, b, c) (a * b + c)",
                "#define SMAA_MAX_SEARCH_STEPS 8",
                "#define SMAA_AREATEX_MAX_DISTANCE 16",
                "#define SMAA_AREATEX_PIXEL_SIZE (1.0 / vec2(160.0, 560.0))",
                "#define SMAASampleLevelZeroOffset( tex, coord, offset ) texture2D( tex, coord+ float( offset ) * resolution.xy, 0.0 )",


                "precision mediump float;", //**

                "attribute vec3 position;", //**
                "attribute vec2 uv;",//**
                "uniform mat4 u_mvpMatrix;",//**
                
                
                "uniform sampler2D tDiffuse;",
                "uniform sampler2D tArea;",
                "uniform sampler2D tSearch;",
                "uniform vec4 resolution;",
                //

                "varying vec2 vUv;",
                "varying vec4 vOffset[ 3 ];",
                "varying vec2 vPixcoord;",

                "void SMAABlendingWeightCalculationVS( vec2 texcoord ) {",
                    "vPixcoord = texcoord * resolution.zw;", 

                    // We will use these offsets for the searches later on (see @PSEUDO_GATHER4):
                     "vOffset[0] = mad(resolution.xyxy, vec4(-0.25, -0.125,  1.25, -0.125), texcoord.xyxy);",
                     "vOffset[1] = mad(resolution.xyxy, vec4(-0.125, -0.25, -0.125, 1.25), texcoord.xyxy);", 

                     // And these for the searches, they indicate the ends of the loops:
                     "vOffset[2] = mad(resolution.xxyy,",
                            "vec4(-2.0, 2.0, -2.0, 2.0) * float(SMAA_MAX_SEARCH_STEPS),",
                            "vec4(vOffset[0].xz, vOffset[1].yw));",

                 "}",

                "void main() {",

                    "vUv = uv;",

                    "SMAABlendingWeightCalculationVS( vUv );",

                    //"gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",
                    "gl_Position = u_mvpMatrix * vec4( position, 1.0 );",

                "}"
            }.JoinWithNewLine();
            //
            string fragmentShader = new[]
            {
                "#define SMAA_CORNER_ROUNDING 25",

                "#define mad(a, b, c) (a * b + c)",
                "#define saturate(a) clamp(a, 0.0, 1.0)",
                "#define SMAA_MAX_SEARCH_STEPS 8",
                "#define SMAA_MAX_SEARCH_STEPS_DIAG 8",
                
                
              
      

                //-----------------------------------------------------------------------------
                // Non-Configurable Defines
                "#define SMAA_AREATEX_MAX_DISTANCE 16",
                "#define SMAA_AREATEX_MAX_DISTANCE_DIAG 20",
                "#define SMAA_AREATEX_PIXEL_SIZE (1.0 / vec2(160.0, 560.0))",
                "#define SMAA_AREATEX_SUBTEX_SIZE ( 1.0 / 7.0 )",
                "#define SMAA_AREATEX_SELECT(sample) sample.ra", // *** since we load the original textarea as LuminanceApha
                "#define SMAA_SEARCHTEX_SIZE vec2(66.0, 33.0)",
                "#define SMAA_SEARCHTEX_PACKED_SIZE vec2(64.0, 16.0)",
                "#define SMAA_CORNER_ROUNDING_NORM (float(SMAA_CORNER_ROUNDING) / 100.0)",
                //-----------------------------------------------------------------------------


                "#define SMAA_SEARCHTEX_SELECT(sample) sample.r", //since we load the original seach area as Luminance
                //
                @"#define SMAASampleLevelZero( tex, coord) texture2D( tex, coord, 0.0 )",
                " #define SMAASampleLevelZeroOffset( tex, coord, offset ) texture2D( tex, coord + float( offset ) * resolution.xy, 0.0 )",

                "precision mediump float;",

                "uniform sampler2D tDiffuse;",
                "uniform sampler2D tArea;",
                "uniform sampler2D tSearch;",
                "uniform vec4 resolution;",

                "varying vec2 vUv;",
                "varying vec4 vOffset[3];",
                "varying vec2 vPixcoord;",

                "vec2 round( vec2 x ) {",
                    "return sign( x ) * floor( abs( x ) + vec2(0.5,0.5) );",
                "}",
                "vec4 round( vec4 x ) {",
                    "return sign( x ) * floor( abs( x ) + vec4(0.5,0.5,0.5,0.5) );",
                "}",


                //Conditional move

                @"
                 void SMAAMovc(bvec2 cond, inout vec2 variable, vec2 value)
                 {
                     if (cond.x) variable.x = value.x;
                     if (cond.y) variable.y = value.y;
                 }",
    
 
           

                //-----------------------------------------------------------------------------
                // Diagonal Search Functions               
                // Allows to decode two binary values from a bilinear-filtered access. 
                //
                @"vec2 SMAADecodeDiagBilinearAccess(vec2 e) {
                    // Bilinear access for fetching 'e' have a 0.25 offset, and we are
                    // interested in the R and G edges:
                    //
                    // +---G---+-------+
                    // |   x o R   x   |
                    // +-------+-------+
                    //
                    // Then, if one of these edge is enabled:
                    //   Red:   (0.75 * X + 0.25 * 1) => 0.25 or 1.0
                    //   Green: (0.75 * 1 + 0.25 * X) => 0.75 or 1.0
                    //
                    // This function will unpack the values (mad + mul + round):
                    // wolframalpha.com: round(x * abs(5 * x - 5 * 0.75)) plot 0 to 1
                    e.r = e.r * abs(5.0 * e.r - 5.0 * 0.75);
                    return round(e);
                }",
                @"vec4 SMAADecodeDiagBilinearAccess(vec4 e) {
                   e.rb = e.rb * abs(5.0 * e.rb - 5.0 * 0.75);
                   return round(e);
                 }",
                //
                // These functions allows to perform diagonal pattern searches. 
                @"vec2 SMAASearchDiag1(sampler2D edgesTex, vec2 texcoord, vec2 dir, out vec2 e) {
                    vec4 coord = vec4(texcoord, -1.0, 1.0);
                    vec3 t = vec3(resolution.xy, 1.0);
                    while (coord.z < float(SMAA_MAX_SEARCH_STEPS_DIAG - 1) &&
                        coord.w > 0.9) {
                        coord.xyz = mad(t, vec3(dir, 1.0), coord.xyz);
                        e = SMAASampleLevelZero(edgesTex, coord.xy).rg;
                        coord.w = dot(e, vec2(0.5, 0.5));
                    }
                  return coord.zw;
                }",
                @"vec2 SMAASearchDiag2(sampler2D edgesTex, vec2 texcoord, vec2 dir, out vec2 e) {
                    vec4 coord = vec4(texcoord, -1.0, 1.0);
                    coord.x += 0.25 * resolution.x; // See @SearchDiag2Optimization
                    vec3 t = vec3(resolution.xy, 1.0);
                    while (coord.z < float(SMAA_MAX_SEARCH_STEPS_DIAG - 1) &&
                          coord.w > 0.9) {
                        
                        coord.xyz = mad(t, vec3(dir, 1.0), coord.xyz);

                        // @SearchDiag2Optimization
                        // Fetch both edges at once using bilinear filtering:
                        e = SMAASampleLevelZero(edgesTex, coord.xy).rg;
                        e = SMAADecodeDiagBilinearAccess(e);

                        // Non-optimized version:
                        // e.g = SMAASampleLevelZero(edgesTex, coord.xy).g;
                        // e.r = SMAASampleLevelZeroOffset(edgesTex, coord.xy, ivec2(1, 0)).r;
                        coord.w = dot(e, vec2(0.5, 0.5));
                    }
                    return coord.zw;
                }",


                //Similar to SMAAArea, this calculates the area corresponding to a certain
                //diagonal distance and crossing edges 'e'.
                @"vec2 SMAAAreaDiag(sampler2D areaTex, vec2 dist, vec2 e, float offset) {
                    vec2 texcoord = mad(vec2(SMAA_AREATEX_MAX_DISTANCE_DIAG, SMAA_AREATEX_MAX_DISTANCE_DIAG), e, dist);

                    // We do a scale and bias for mapping to texel space:
                    texcoord = mad(SMAA_AREATEX_PIXEL_SIZE, texcoord, 0.5 * SMAA_AREATEX_PIXEL_SIZE);

                    // Diagonal areas are on the second half of the texture:
                    texcoord.x += 0.5;

                    // Move to proper place, according to the subpixel offset:
                    texcoord.y += SMAA_AREATEX_SUBTEX_SIZE * offset;

                    // Do it!
                    return SMAA_AREATEX_SELECT(SMAASampleLevelZero(areaTex, texcoord));
                }",

                //This searches for diagonal patterns and returns the corresponding weights.

                @"vec2 SMAACalculateDiagWeights(sampler2D edgesTex, sampler2D areaTex, vec2 texcoord, vec2 e, vec4 subsampleIndices) {
                    vec2 weights = vec2(0.0, 0.0);

                    // Search for the line ends:
                    vec4 d;
                    vec2 end;
                    if (e.r > 0.0) {
                        d.xz = SMAASearchDiag1(edgesTex, texcoord, vec2(-1.0,  1.0), end);
                        d.x += float(end.y > 0.9);
                    } else
                        d.xz = vec2(0.0, 0.0);
                    d.yw = SMAASearchDiag1(edgesTex, texcoord, vec2(1.0, -1.0), end);

                    //SMAA_BRANCH
                    if (d.x + d.y > 2.0) { // d.x + d.y + 1 > 3
                        // Fetch the crossing edges:
                        vec4 coords = mad(vec4(-d.x + 0.25, d.x, d.y, -d.y - 0.25), resolution.xyxy, texcoord.xyxy);
                        vec4 c;
                        c.xy = SMAASampleLevelZeroOffset(edgesTex, coords.xy, ivec2(-1,  0)).rg;
                        c.zw = SMAASampleLevelZeroOffset(edgesTex, coords.zw, ivec2( 1,  0)).rg;
                        c.yxwz = SMAADecodeDiagBilinearAccess(c.xyzw);

                        // Non-optimized version:
                        // float4 coords = mad(float4(-d.x, d.x, d.y, -d.y), SMAA_RT_METRICS.xyxy, texcoord.xyxy);
                        // float4 c;
                        // c.x = SMAASampleLevelZeroOffset(edgesTex, coords.xy, int2(-1,  0)).g;
                        // c.y = SMAASampleLevelZeroOffset(edgesTex, coords.xy, int2( 0,  0)).r;
                        // c.z = SMAASampleLevelZeroOffset(edgesTex, coords.zw, int2( 1,  0)).g;
                        // c.w = SMAASampleLevelZeroOffset(edgesTex, coords.zw, int2( 1, -1)).r;

                        // Merge crossing edges at each side into a single value:
                        vec2 cc = mad(vec2(2.0, 2.0), c.xz, c.yw);

                        // Remove the crossing edge if we didn't found the end of the line:
                        SMAAMovc(bvec2(step(0.9, d.zw)), cc, vec2(0.0, 0.0));

                        // Fetch the areas for this line:
                        weights += SMAAAreaDiag(areaTex, d.xy, cc, subsampleIndices.z);
                    }

                    // Search for the line ends:
                    d.xz = SMAASearchDiag2(edgesTex, texcoord, vec2(-1.0, -1.0), end);
                    if (SMAASampleLevelZeroOffset(edgesTex, texcoord, ivec2(1, 0)).r > 0.0) {
                        d.yw = SMAASearchDiag2(edgesTex, texcoord, vec2(1.0, 1.0), end);
                        d.y += float(end.y > 0.9);
                    } else
                        d.yw = vec2(0.0, 0.0);

                    //SMAA_BRANCH
                    if (d.x + d.y > 2.0) { // d.x + d.y + 1 > 3
                        // Fetch the crossing edges:
                        vec4 coords = mad(vec4(-d.x, -d.x, d.y, d.y), resolution.xyxy, texcoord.xyxy);
                        vec4 c;
                        c.x  = SMAASampleLevelZeroOffset(edgesTex, coords.xy, ivec2(-1,  0)).g;
                        c.y  = SMAASampleLevelZeroOffset(edgesTex, coords.xy, ivec2( 0, -1)).r;
                        c.zw = SMAASampleLevelZeroOffset(edgesTex, coords.zw, ivec2( 1,  0)).gr;
                        vec2 cc = mad(vec2(2.0, 2.0), c.xz, c.yw);

                        // Remove the crossing edge if we didn't found the end of the line:
                        SMAAMovc(bvec2(step(0.9, d.zw)), cc, vec2(0.0, 0.0));

                        // Fetch the areas for this line:
                        weights += SMAAAreaDiag(areaTex, d.xy, cc, subsampleIndices.w).gr;
                    }

                    return weights;
                }",
                
            //-----------------------------------------------------------------------------
            // Horizontal/Vertical Search Functions
            /**
            * This allows to determine how much length should we add in the last step
            * of the searches. It takes the bilinearly interpolated edge (see 
            * @PSEUDO_GATHER4), and adds 0, 1 or 2, depending on which edges and
            * crossing edges are active.
            */
                "float SMAASearchLength( sampler2D searchTex, vec2 e, float offset ){",
                    // The texture is flipped vertically, with left and right cases taking half
                    // of the space horizontally:
                    "vec2 scale = SMAA_SEARCHTEX_SIZE * vec2(0.5, -1.0);",
                    "vec2 bias = SMAA_SEARCHTEX_SIZE * vec2(offset, 1.0);",
                    // Scale and bias to access texel centers:
                    "scale += vec2(-1.0, 1.0);",
                    "bias += vec2(0.5, -0.5);", 
                   // Convert from pixel coordinates to texcoords:
                   // (We use SMAA_SEARCHTEX_PACKED_SIZE because the texture is cropped)
                    "scale *= 1.0 / SMAA_SEARCHTEX_PACKED_SIZE;",
                    "bias *= 1.0 / SMAA_SEARCHTEX_PACKED_SIZE;",

                   // Lookup the search texture:
                   "return SMAA_SEARCHTEX_SELECT(SMAASampleLevelZero(searchTex, mad(scale, e, bias)));",
                "}",

               
                 // Horizontal/vertical search functions for the 2nd pass.
                 
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
                        "texcoord = mad(-vec2(2.0, 0.0), resolution.xy, texcoord);",
                        "if ( ! ( texcoord.x > end && e.g > 0.8281 && e.r == 0.0 ) ) break;",
                    "}",

                    //
                     "float offset = mad(-(255.0 / 127.0), SMAASearchLength(searchTex, e, 0.0), 3.25);",
                    " return mad(resolution.x, offset, texcoord.x);", 
                "}",

                "float SMAASearchXRight( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
                    "vec2 e = vec2( 0.0, 1.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord = mad(vec2(2.0, 0.0), resolution.xy, texcoord);",
                        "if ( ! ( texcoord.x < end && e.g > 0.8281 && e.r == 0.0 ) ) break;",
                    "}",
                    "float offset = mad(-(255.0 / 127.0), SMAASearchLength(searchTex, e, 0.5), 3.25);",
                    "return mad(-resolution.x, offset, texcoord.x);", 
                "}",

                "float SMAASearchYUp( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
                    "vec2 e = vec2( 1.0, 0.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord = mad(-vec2(0.0, 2.0), resolution.xy, texcoord);",
                        "if ( ! ( texcoord.y > end && e.r > 0.8281 && e.g == 0.0 ) ) break;",
                    "}",
                    "float offset = mad(-(255.0 / 127.0), SMAASearchLength(searchTex, e.gr, 0.0), 3.25);",
                    "return mad(resolution.y, offset, texcoord.y);", 
                "}",

                "float SMAASearchYDown( sampler2D edgesTex, sampler2D searchTex, vec2 texcoord, float end ) {",
                    "vec2 e = vec2( 1.0, 0.0 );",

                    "for ( int i = 0; i < SMAA_MAX_SEARCH_STEPS; i ++ ) {", // WebGL port note: Changed while to for
				        "e = texture2D( edgesTex, texcoord, 0.0 ).rg;",
                        "texcoord = mad(vec2(0.0, 2.0), resolution.xy, texcoord);",
                        "if ( ! ( texcoord.y < end && e.r > 0.8281 && e.g == 0.0 ) ) break;",
                    "}",

                    "float offset = mad(-(255.0 / 127.0), SMAASearchLength(searchTex, e.gr, 0.5), 3.25);",
                    "return mad(-resolution.y, offset, texcoord.y);", 
                "}", 
               //-------------------
               //Ok, we have the distance and both crossing edges. So, wSMAASampleLevelZerohat are the areas
               //at each side of current edge? 

                "vec2 SMAAArea( sampler2D areaTex, vec2 dist, float e1, float e2, float offset ) {",
			        // Rounding prevents precision errors of bilinear filtering:
			        //"vec2 texcoord = float( SMAA_AREATEX_MAX_DISTANCE ) * round( 4.0 * vec2( e1, e2 ) ) + dist;",
                    "vec2 texcoord = mad(vec2(SMAA_AREATEX_MAX_DISTANCE, SMAA_AREATEX_MAX_DISTANCE), round(4.0 * vec2(e1, e2)), dist);",

			        // We do a scale and bias for mapping to texel space:
			        "texcoord = SMAA_AREATEX_PIXEL_SIZE * texcoord + ( 0.5 * SMAA_AREATEX_PIXEL_SIZE );",

			        // Move to proper place, according to the subpixel offset:
			        "texcoord.y += SMAA_AREATEX_SUBTEX_SIZE * offset;",
                        
                    // Do it!
                    "return SMAA_AREATEX_SELECT(SMAASampleLevelZero(areaTex, texcoord));",
                "}",

                //-----------------------------------------------------------------------------
                // Corner Detection Functions
                //-----------------------------------------------------------------------------
                @"void  SMAADetectHorizontalCornerPattern(sampler2D edgesTex, inout vec2 weights, vec4 texcoord, vec2 d){
                
                    vec2 leftRight = step(d.xy, d.yx);    
                    vec2 rounding = (1.0 - SMAA_CORNER_ROUNDING_NORM) * leftRight;
                    rounding /= leftRight.x + leftRight.y; // Reduce blending for pixels in the center of a line.
                    vec2 factor = vec2(1.0, 1.0);
                    factor.x -= rounding.x * SMAASampleLevelZeroOffset(edgesTex, texcoord.xy, ivec2(0, 1)).r;
                    factor.x -= rounding.y * SMAASampleLevelZeroOffset(edgesTex, texcoord.zw, ivec2(1, 1)).r;
                    factor.y -= rounding.x * SMAASampleLevelZeroOffset(edgesTex, texcoord.xy, ivec2(0, -2)).r;
                    factor.y -= rounding.y * SMAASampleLevelZeroOffset(edgesTex, texcoord.zw, ivec2(1, -2)).r;
                    weights *= saturate(factor);       
                }",

                @"void  SMAADetectVerticalCornerPattern(sampler2D edgesTex, inout vec2 weights, vec4 texcoord, vec2 d){
                
                    vec2 leftRight = step(d.xy, d.yx);
                    vec2 rounding = (1.0 - SMAA_CORNER_ROUNDING_NORM) * leftRight;
                    
                    rounding /= leftRight.x + leftRight.y;

                    vec2 factor = vec2(1.0, 1.0);
                    factor.x -= rounding.x * SMAASampleLevelZeroOffset(edgesTex, texcoord.xy, ivec2(1, 0)).g;
                    factor.x -= rounding.y * SMAASampleLevelZeroOffset(edgesTex, texcoord.zw, ivec2(1, 1)).g;
                    factor.y -= rounding.x * SMAASampleLevelZeroOffset(edgesTex, texcoord.xy, ivec2(-2, 0)).g;
                    factor.y -= rounding.y * SMAASampleLevelZeroOffset(edgesTex, texcoord.zw, ivec2(-2, 1)).g;
                    weights *= saturate(factor); 
                }",
                  

              //-----------------------------------------------------------------------------
              // Blending Weight Calculation Pixel Shader (Second Pass)

              "vec4 SMAABlendingWeightCalculationPS( vec2 texcoord, vec2 pixcoord, vec4 offset[ 3 ], sampler2D edgesTex, sampler2D areaTex, sampler2D searchTex, vec4 subsampleIndices ) {",
                    // subsampleIndices => Just pass zero for SMAA 1x, see @SUBSAMPLE_INDICES.

                    "vec4 weights = vec4( 0.0, 0.0, 0.0, 0.0 );",

                    "vec2 e = texture2D( edgesTex, texcoord ).rg;",

                    //SMAA_BRANCH
                    "if ( e.g > 0.0 ) {", // Edge at north

                            //  #if !defined(SMAA_DISABLE_DIAG_DETECTION)
                            //        // Diagonals have both north and west edges, so searching for them in
                            //        // one of the boundaries is enough.
                           "weights.rg = SMAACalculateDiagWeights(edgesTex, areaTex, texcoord, e, subsampleIndices);",

                            //            // We give priority to diagonals, so if we find a diagonal we skip 
                            //            // horizontal/vertical processing.
                            //            SMAA_BRANCH
                            //        if (weights.r == -weights.g)
                            //            { // weights.r + weights.g == 0.0
                            //#endif
                            "if (weights.r == -weights.g) { // weights.r + weights.g == 0.0",

                            "vec2 d;",

				            // Find the distance to the left:
				            "vec3 coords;",
                            "coords.x = SMAASearchXLeft( edgesTex, searchTex, offset[ 0 ].xy, offset[ 2 ].x );",
                            "coords.y = offset[ 1 ].y;", // offset[1].y = texcoord.y - 0.25 * resolution.y (@CROSSING_OFFSET)
				            "d.x = coords.x;",

				            // Now fetch the left crossing edges, two at a time using bilinear
				            // filtering. Sampling at -0.25 (see @CROSSING_OFFSET) enables to
				            // discern what value each edge has:
				            "float e1 = texture2D( edgesTex, coords.xy, 0.0 ).r;",

				            // Find the distance to the right:
				            "coords.z = SMAASearchXRight( edgesTex, searchTex, offset[ 0 ].zw, offset[ 2 ].y );",
                            "d.y = coords.z;",

				            // We want the distances to be in pixel units (doing this here allow to
				            // better interleave arithmetic and memory accesses):
				        
                            "d = abs(round(mad(resolution.zz, d, -pixcoord.xx)));",

				            // SMAAArea below needs a sqrt, as the areas texture is compressed
				            // quadratically:
				            "vec2 sqrt_d = sqrt(d);",

				            // Fetch the right crossing edges: 
				            "float e2 = SMAASampleLevelZeroOffset( edgesTex, coords.zy, vec2( 1.0, 0.0 ) ).r;",

				            // Ok, we know how this pattern looks like, now it is time for getting
				            // the actual area:
				            "weights.rg = SMAAArea( areaTex, sqrt_d, e1, e2, float( subsampleIndices.y ) );",

                             // Fix corners:
                             "coords.y = texcoord.y;",
                             "SMAADetectHorizontalCornerPattern(edgesTex, weights.rg, coords.xyzy, d);",
                             "}",
                             "else{",
                                "e.r = 0.0; // Skip vertical processing.",
                             "}",
                        "}",

                        // SMAA_BRANCH
                        "if ( e.r > 0.0 ) {", // Edge at west
				            "vec2 d;",

				            // Find the distance to the top:
				            "vec3 coords;",

                            "coords.y = SMAASearchYUp( edgesTex, searchTex, offset[ 1 ].xy, offset[ 2 ].z );",
                            "coords.x = offset[ 0 ].x;", // offset[1].x = texcoord.x - 0.25 * resolution.x;
				            "d.x = coords.y;",

				            // Fetch the top crossing edges:
				            "float e1 = texture2D( edgesTex, coords.xy, 0.0 ).g;",

				            // Find the distance to the bottom:
				            "coords.z = SMAASearchYDown( edgesTex, searchTex, offset[ 1 ].zw, offset[ 2 ].w );",
                            "d.y = coords.z;",

				            // We want the distances to be in pixel units: 
                            "d = abs(round(mad(resolution.ww, d, -pixcoord.yy)));",

				            // SMAAArea below needs a sqrt, as the areas texture is compressed
				            // quadratically: 
                            "vec2 sqrt_d = sqrt(d);",

				            // Fetch the bottom crossing edges: 
				            "float e2 = SMAASampleLevelZeroOffset( edgesTex, coords.xz, vec2( 0.0, 1.0 ) ).g;",

				            // Get the area for this direction:
				            "weights.ba = SMAAArea( areaTex, sqrt_d, e1, e2, float( subsampleIndices.x ) );",
                         
                            // Fix corners:
                            "coords.x = texcoord.x;",
                            "SMAADetectVerticalCornerPattern( edgesTex, weights.ba, coords.xyxz, d);",

                        "}",

                    "return weights;",
                "}",

                "void main() {",
                    "gl_FragColor = SMAABlendingWeightCalculationPS( vUv, vPixcoord, vOffset, tDiffuse, tArea, tSearch, vec4( 0.0 ) );",

                "}"
            }.JoinWithNewLine();

            BuildProgram(vertexShader, fragmentShader);

        }
        protected override void OnProgramBuilt()
        {
            u_resolution = shaderProgram.GetUniform4("resolution");
            tArea = shaderProgram.GetUniform1("tArea");
            tSearch = shaderProgram.GetUniform1("tSearch");
            position = shaderProgram.GetAttrV3f("position");
            uv = shaderProgram.GetAttrV2f("uv");
        }


        protected override void OnSetVarsBeforeRenderer()
        {
            u_resolution.SetValue(resolution_x, resolution_y, resolution_z, resolution_w);
        }


        //-----------------------------------------
        //#if DEBUG
        //        float _latestBmpW;
        //        float _latestBmpH;
        //        bool _latestBmpInverted;
        //#endif
        /// <summary>
        /// load glbmp before draw
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadAreaTexture(InternalGLBitmapTexture bmp)
        {
            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());

            // Set the texture sampler to texture unit
            tArea.SetValue(1);
        }
        /// <summary>
        /// load glbmp before draw
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadSearchTexture(InternalGLBitmapTexture bmp)
        {
            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0                  
            tSearch.SetValue(2);
        }

        public void Render(FrameBuffer frmBuffer, float left, float top, float w, float h)
        {
            unsafe
            {
                //if (bmp.IsInvert)
                //{

                //    float* imgVertices = stackalloc float[5 * 4];
                //    {
                //        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)
                //        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                //        //imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                //        //---------------------
                //        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                //        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                //        //---------------------
                //        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                //        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                //        //---------------------
                //        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                //        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)
                //    }
                //    position.UnsafeLoadMixedV3f(imgVertices, 5);
                //    uv.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                //}
                //else
                //{
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
                //}
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, frmBuffer.FrameBufferId);
            // Set the texture sampler to texture unit to 0     

            tDiffuse.SetValue(0);
            OnSetVarsBeforeRenderer();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }

    }


    // Neighborhood Blending Pixel Shader (Third Pass)
    /// <summary>
    /// SMAA Neighborhood shader
    /// </summary>
    class SMAANeighborhoodBlendingShader : SMAAShaderBase
    {
        ShaderUniformVar2 u_resolution;
        ShaderVtxAttrib3f position;
        ShaderVtxAttrib2f uv;//uv texture coord
        ShaderUniformVar1 tColor;

        public SMAANeighborhoodBlendingShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //Neighborhood Blending Vertex Shader
            string vertexShader = new[] {
                "#define mad(a, b, c) (a * b + c)",
                "precision mediump float;", //**
                "attribute vec3 position;", //**
                "attribute vec2 uv;",//**
                "uniform mat4 u_mvpMatrix;",//**

                "uniform vec2 resolution;",
                "varying vec2 vUv;",
                "varying vec4 vOffset;",
                "void SMAANeighborhoodBlendingVS( vec2 texcoord ) {",
                     "vOffset = mad(resolution.xyxy, vec4( 1.0, 0.0, 0.0,  1.0), texcoord.xyxy);",
                "}",
                "void main() {",

                    "vUv = uv;",
                    "SMAANeighborhoodBlendingVS( vUv );", 
                    //"gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",
                     "gl_Position = u_mvpMatrix * vec4( position, 1.0 );",

                "}"}.JoinWithNewLine();

            //-----------------------------------------------------------------------------
            // Neighborhood Blending Pixel Shader (Third Pass)
            string fragmentShader = new[]
            {
                "#define mad(a, b, c) (a * b + c)",
                "#define SMAASampleLevelZero( tex, coord) texture2D( tex, coord, 0.0 )",
                "precision mediump float;", //**
                "uniform sampler2D tDiffuse;",
                "uniform sampler2D tColor;",
                "uniform vec2 resolution;",

                "varying vec2 vUv;",
                "varying vec4 vOffset;",

                @"
                 void SMAAMovc(bvec2 cond, inout vec2 variable, vec2 value)
                 {
                     if (cond.x) variable.x = value.x;
                     if (cond.y) variable.y = value.y;
                 }
                 void SMAAMovc(bvec4 cond, inout vec4 variable, vec4 value) {
                    SMAAMovc(cond.xy, variable.xy, value.xy);
                    SMAAMovc(cond.zw, variable.zw, value.zw);
                 }",

                "vec4 SMAANeighborhoodBlendingPS( vec2 texcoord, vec4 offset, sampler2D colorTex, sampler2D blendTex ) {",
			        // Fetch the blending weights for current pixel:
			        "vec4 a;",
                    "a.x = texture2D( blendTex, offset.xy ).a;",// Right
                    "a.y = texture2D( blendTex, offset.zw ).g;",// Top
                    "a.wz = texture2D( blendTex, texcoord ).xz;",// Bottom / Left

			        // Is there any blending weight with a value greater than 0.0?
                    //SMAA_BRANCH
			        "if ( dot(a, vec4( 1.0, 1.0, 1.0, 1.0 )) < 1e-5 ) {",
                        @" vec4 color = SMAASampleLevelZero(colorTex, texcoord);
                           return color;
                        ",
                    "} else {",
                        @"
                            bool h = max(a.x, a.z) > max(a.y, a.w); // max(horizontal) > max(vertical)

                            // Calculate the blending offsets:
                            vec4 blendingOffset = vec4(0.0, a.y, 0.0, a.w);
                            vec2 blendingWeight = a.yw;
                            SMAAMovc(bvec4(h, h, h, h), blendingOffset, vec4(a.x, 0.0, a.z, 0.0));
                            SMAAMovc(bvec2(h, h), blendingWeight, a.xz);
                            blendingWeight /= dot(blendingWeight, vec2(1.0, 1.0));

                            // Calculate the texture coordinates:
                            vec4 blendingCoord = mad(blendingOffset, vec4(resolution.xy, -resolution.xy), texcoord.xyxy);

                            // We exploit bilinear filtering to mix current pixel with the chosen
                            // neighbor:
                            vec4 color = blendingWeight.x * SMAASampleLevelZero(colorTex, blendingCoord.xy);
                            color += blendingWeight.y * SMAASampleLevelZero(colorTex, blendingCoord.zw);

                            //#if SMAA_REPROJECTION
                                //.....
                            //#endif

                            return color;
                        ",
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
            position = shaderProgram.GetAttrV3f("position");
            uv = shaderProgram.GetAttrV2f("uv");
            tColor = shaderProgram.GetUniform1("tColor");
        }


        protected override void OnSetVarsBeforeRenderer()
        {
            u_resolution.SetValue(resolution_x, resolution_y);
        }

        public void LoadColorTexure(FrameBuffer frmBuffer)
        {
            //load before use with RenderSubImage
            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, frmBuffer.FrameBufferId);

            // Set the texture sampler to texture unit
            tColor.SetValue(1);
        }
        public void Render(FrameBuffer frmBuffer, float left, float top, float w, float h)
        {
            unsafe
            {
                //if (bmp.IsInvert)
                //{

                //    float* imgVertices = stackalloc float[5 * 4];
                //    {
                //        imgVertices[0] = left; imgVertices[1] = top; imgVertices[2] = 0; //coord 0 (left,top)
                //        imgVertices[3] = 0; imgVertices[4] = 0; //texture coord 0 (left,bottom)
                //        //imgVertices[3] = srcLeft / orgBmpW; imgVertices[4] = srcBottom / orgBmpH; //texture coord 0  (left,bottom)

                //        //---------------------
                //        imgVertices[5] = left; imgVertices[6] = top - h; imgVertices[7] = 0; //coord 1 (left,bottom)
                //        imgVertices[8] = 0; imgVertices[9] = 1; //texture coord 1  (left,top)

                //        //---------------------
                //        imgVertices[10] = left + w; imgVertices[11] = top; imgVertices[12] = 0; //coord 2 (right,top)
                //        imgVertices[13] = 1; imgVertices[14] = 0; //texture coord 2  (right,bottom)

                //        //---------------------
                //        imgVertices[15] = left + w; imgVertices[16] = top - h; imgVertices[17] = 0; //coord 3 (right, bottom)
                //        imgVertices[18] = 1; imgVertices[19] = 1; //texture coord 3 (right,top)
                //    }
                //    position.UnsafeLoadMixedV3f(imgVertices, 5);
                //    uv.UnsafeLoadMixedV2f(imgVertices + 3, 5);
                //}
                //else
                //{
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
                //}
            }

            SetCurrent();
            CheckViewMatrix();
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, frmBuffer.FrameBufferId);
            // Set the texture sampler to texture unit to 0     

            tDiffuse.SetValue(0);
            OnSetVarsBeforeRenderer();
            GL.DrawElements(BeginMode.TriangleStrip, 4, DrawElementsType.UnsignedShort, indices);
        }
    }

}

