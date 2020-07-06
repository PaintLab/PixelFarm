//MIT, 2016-present, WinterDev
//we use concept from https://www.mapbox.com/blog/drawing-antialiased-lines/
//
using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class SmoothLineShader : ColorFillShaderBase
    {
        ShaderVtxAttrib4f a_position;

        ShaderUniformVar4 u_solidColor;
        ShaderUniformVar1 u_linewidth;
        ShaderUniformVar1 u_p0;

        public SmoothLineShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            //NOTE: during development, 
            //new shader source may not recompile if you don't clear cache or disable cache feature
            //like...
            //EnableProgramBinaryCache = false;

            if (!LoadCompiledShader())
            {
                //we may store this outside the exe ?

                //vertex shader source

                string vs = @"        
                    precision mediump float;
                    attribute vec4 a_position;  
                    uniform vec2 u_ortho_offset; 
                     
                    uniform mat4 u_mvpMatrix; 
                    uniform float u_linewidth;                  
                    varying float v_distance; 
                    varying vec2 v_dir; 
                    void main()
                    {                   
                        float rad = a_position[3];
                        v_distance= a_position[2]; 
                        vec2 delta;
                        if(v_distance <1.0){                                         
                            delta = vec2(-sin(rad) * u_linewidth,cos(rad) * u_linewidth) + u_ortho_offset;                       
                            v_dir = vec2(0.80,0.0);
                        }else{                      
                            delta = vec2(sin(rad) * u_linewidth,-cos(rad) * u_linewidth) + u_ortho_offset;
                            v_dir = vec2(0.0,0.80);  
                        } 
                        gl_Position = u_mvpMatrix*  vec4(a_position[0] +delta[0],a_position[1]+delta[1],0,1);
                    }
                ";



                //version3
                string fs = @"
                    precision mediump float;
                    uniform vec4 u_solidColor;
                    uniform float p0;
                    varying float v_distance;
                    varying vec2 v_dir;                      
                    void main()
                    {   
                         gl_FragColor =vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], 
                                            u_solidColor[3] *((v_distance* (v_dir[0])+ (1.0-v_distance)* (v_dir[1]))  * (1.0/p0)) * 0.55);  
                    }
                ";

                ////old version 2
                //string fs = @"
                //    precision mediump float;
                //    uniform vec4 u_solidColor;
                //    uniform float p0;
                //    varying float v_distance;                    
                //    void main()
                //    {      
                //        if(v_distance < p0){                        
                //            gl_FragColor =vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], u_solidColor[3] *(v_distance * (1.0/p0)) * 0.55);
                //        }else{           
                //            gl_FragColor =vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], u_solidColor[3] *((1.0-v_distance) * (1.0/p0)) * 0.55);
                //        } 
                //    }
                //";
                ////old version 1
                //string fs = @"
                //    precision mediump float;
                //    uniform vec4 u_solidColor;
                //    uniform float p0;
                //    varying float v_distance;                    
                //    void main()
                //    {       

                //        if(v_distance < p0){                        
                //            gl_FragColor =vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], u_solidColor[3] *(v_distance * (1.0/p0)) * 0.55);
                //        }else if(v_distance >= (1.0-p0)){           
                //            gl_FragColor =vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], u_solidColor[3] *((1.0-v_distance) * (1.0/p0)) * 0.55);
                //        }else{
                //            gl_FragColor = u_solidColor;
                //        }
                //    }
                //";


                //---------------------
                if (!_shaderProgram.Build(vs, fs))
                {
                    return;
                }
                //
                SaveCompiledShader();
            }


            //-----------------------
            a_position = _shaderProgram.GetAttrV4f("a_position");
            u_ortho_offset = _shaderProgram.GetUniform2("u_ortho_offset");
            u_matrix = _shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = _shaderProgram.GetUniform4("u_solidColor");
            u_linewidth = _shaderProgram.GetUniform1("u_linewidth");
            u_p0 = _shaderProgram.GetUniform1("p0");
        }

        static float GetCutPoint(float half_w)
        {
            if (half_w <= 0.5)
            {
                return 0.5f;
            }
            else if (half_w <= 1.0)
            {
                return 0.475f;
            }
            else if (half_w > 1.0 && half_w < 3.0)
            {
                return 0.25f;
            }
            else
            {
                return 0.1f;
            }

        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            //float dx = x2 - x1;
            //float dy = y2 - y1; 
            SetCurrent();
            CheckViewMatrix();
            //--------------------
            _shareRes.AssignStrokeColorToVar(u_solidColor);
            unsafe
            {
                float rad1 = (float)Math.Atan2(
                  y2 - y1,  //dy
                  x2 - x1); //dx
                //float[] vtxs = new float[] {
                //    x1, y1,0,rad1,
                //    x1, y1,1,rad1,
                //    x2, y2,0,rad1,
                //    //-------
                //    x2, y2,1,rad1
                //}; 
                //-------------------- 
                float* vtxs = stackalloc float[4 * 4];
                vtxs[0] = x1; vtxs[1] = y1; vtxs[2] = 0; vtxs[3] = rad1;
                vtxs[4] = x1; vtxs[5] = y1; vtxs[6] = 1; vtxs[7] = rad1;
                vtxs[8] = x2; vtxs[9] = y2; vtxs[10] = 0; vtxs[11] = rad1;
                vtxs[12] = x2; vtxs[13] = y2; vtxs[14] = 1; vtxs[15] = rad1;
                a_position.LoadPureV4fUnsafe(vtxs);
            }

            //because original stroke width is the width of both side of
            //the line, but u_linewidth is the half of the strokeWidth
            float half_w = _shareRes._strokeWidth / 2f;
            u_linewidth.SetValue(half_w);
            //u_p0.SetValue((1 / GetCutPoint(half_w)) * 0.55f);
            u_p0.SetValue(GetCutPoint(half_w));
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);
        }
        public void DrawTriangleStrips(float[] coords, int ncount)
        {
            SetCurrent();
            CheckViewMatrix();

            _shareRes.AssignStrokeColorToVar(u_solidColor);
            float half_w = 1.5f / 2f;
            u_linewidth.SetValue(half_w);
            u_p0.SetValue(GetCutPoint(half_w));
            //u_p0.SetValue((1 / GetCutPoint(half_w)) * 0.55f);
            //
            a_position.LoadPureV4f(coords);
            //because original stroke width is the width of both side of
            //the line, but u_linewidth is the half of the strokeWidth            
            GL.DrawArrays(BeginMode.TriangleStrip, 0, ncount);
        }
        public void DrawTriangleStrips(int startAt, int ncount)
        {
            SetCurrent();
            CheckViewMatrix();
            _shareRes.AssignStrokeColorToVar(u_solidColor);
            float half_w = 1.5f / 2f;
            u_linewidth.SetValue(half_w);
            u_p0.SetValue(GetCutPoint(half_w));
            //u_p0.SetValue((1 / GetCutPoint(half_w)) * 0.55f);
            //
            a_position.LoadLatest();
            //because original stroke width is the width of both side of
            //the line, but u_linewidth is the half of the strokeWidth            
            GL.DrawArrays(BeginMode.TriangleStrip, startAt, ncount);
        }
    }

    class InvertAlphaLineSmoothShader : ColorFillShaderBase
    {
        //for stencil buffer ***
        readonly ShaderVtxAttrib4f a_position;
        readonly ShaderUniformVar1 u_linewidth;
        readonly ShaderUniformVar1 u_p0;

        float _cutPoint;
        bool _loadCutPoint;
        float _latestDrawW;

        public InvertAlphaLineSmoothShader(ShaderSharedResource shareRes)
             : base(shareRes)
        {
            //NOTE: during development, 
            //new shader source may not recompile if you don't clear cache or disable cache feature
            //like...
            //EnableProgramBinaryCache = false;

            if (!LoadCompiledShader())
            {
                //vertex shader source
                string vs = @"                   
                attribute vec4 a_position;     
                uniform mat4 u_mvpMatrix; 
                uniform float u_linewidth;
                uniform vec2 u_ortho_offset;  
                varying float v_distance; 
                void main()
                {                   
                    float rad = a_position[3];
                    v_distance= a_position[2]; 

                    vec2 delta;
                    if(v_distance <1.0){                                         
                        delta = vec2(-sin(rad) * u_linewidth,cos(rad) * u_linewidth) + u_ortho_offset;   
                    }else{                      
                        delta = vec2(sin(rad) * u_linewidth,-cos(rad) * u_linewidth)+ u_ortho_offset;  
                    } 
                    gl_Position = u_mvpMatrix*  vec4(a_position[0] +delta[0],a_position[1]+delta[1],0,1); 
                }
                ";
                //fragment source
                //this is invert fragment shader *** 


                //p0= cutpoint of inside,outside
                //1/p0 = factor
                string fs = @"
                    precision mediump float;      
                    uniform float p0; 
                    varying float v_distance; 
                    void main()
                    {  
                        if(v_distance < p0){                        
                            gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0-((v_distance * (1.0 /p0))));
                        }else if(v_distance> (1.0-p0)){                         
                            gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0-(((1.0-v_distance)* (1.0 /p0))));
                        }
                        else{ 
                            discard;                      
                        } 
                    }
                ";


                //string fs = @"
                //    precision mediump float;                    
                //    uniform vec4 u_solidColor; 
                //    uniform float p0;
                //    varying vec2 v_dir;
                //    varying float v_distance;

                //    void main()
                //    {     
                //        float factor= 1.0 /p0;            
                //        if(v_distance < p0){                        
                //            gl_FragColor = vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], 1.0-(u_solidColor[3] *(v_distance * factor)));
                //        }else if(v_distance> (1.0-p0)){                         
                //            gl_FragColor = vec4(u_solidColor[0],u_solidColor[1],u_solidColor[2], 1.0-(u_solidColor[3] *((1.0-v_distance)* factor)));
                //        }
                //        else{ 
                //            discard;                      
                //        } 
                //    }
                //";

                //---------------------
                if (!_shaderProgram.Build(vs, fs))
                {
                    return;
                }

                //-----------------------
                SaveCompiledShader();
            }


            a_position = _shaderProgram.GetAttrV4f("a_position");
            u_matrix = _shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_ortho_offset = _shaderProgram.GetUniform2("u_ortho_offset");
            u_linewidth = _shaderProgram.GetUniform1("u_linewidth");
            u_p0 = _shaderProgram.GetUniform1("p0");
            _cutPoint = SetCutPoint(0.5f); //this are fixed for inverted alpha smooth line shader

        }



        public void DrawTriangleStrips(float[] coords, int ncount)
        {
            SetCurrent();
            CheckViewMatrix();
            //----------------------------------- 
            if (!_loadCutPoint) //just for reduce draw call
            {
                u_p0.SetValue(_cutPoint);
                _loadCutPoint = true;
            }

            if (_latestDrawW != _shareRes._strokeWidth)//just for reduce draw call
            {

                u_linewidth.SetValue(_latestDrawW = _shareRes._strokeWidth);
            }


            a_position.LoadPureV4f(coords);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, ncount);
        }

        static float SetCutPoint(float value)
        {
            if (value <= 0.5)
            {
                return 0.5f;
            }
            else if (value <= 1.0)
            {
                return 0.45f;
            }
            else if (value > 1.0 && value < 3.0)
            {
                return 0.25f;
            }
            else
            {
                return 0.1f;
            }
        }
    }

}