//MIT, 2016-present, WinterDev 

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class RectFillShader : ColorFillShaderBase
    {
        ShaderVtxAttrib2f a_position;
        ShaderVtxAttrib4f a_color; 

        public RectFillShader(ShaderSharedResource shareRes)
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
                    attribute vec2 a_position;     
                    attribute vec4 a_color;
                    uniform mat4 u_mvpMatrix; 
                    uniform vec2 u_ortho_offset;
                    varying vec4 v_color;
 
                    void main()
                    {
                        gl_Position = u_mvpMatrix* vec4(a_position+ u_ortho_offset,0,1); 
                        v_color= a_color;
                    }
                    ";
                //fragment source
                string fs = @"
                        precision mediump float;
                        varying vec4 v_color; 
                        void main()
                        {
                            gl_FragColor = v_color;
                        }
                    ";
                if (!_shaderProgram.Build(vs, fs))
                {
                    throw new NotSupportedException();
                }

                SaveCompiledShader();
            }


            a_position = _shaderProgram.GetAttrV2f("a_position");
            u_orthov_offset = _shaderProgram.GetUniform2("u_ortho_offset");
            a_color = _shaderProgram.GetAttrV4f("a_color");
            u_matrix = _shaderProgram.GetUniformMat4("u_mvpMatrix");
        } 
        public void Render(float[] v2fArray, float[] colors)
        {
            SetCurrent();
            CheckViewMatrix();
            //----------------------------------------------------
            a_position.LoadPureV2f(v2fArray);
            a_color.LoadPureV4f(colors);
            GL.DrawArrays(BeginMode.Triangles, 0, v2fArray.Length / 2);
        }
    }


    class RadialGradientFillShader : ColorFillShaderBase
    {

        ShaderVtxAttrib2f a_position; 
        ShaderUniformMatrix3 u_invertedTxMatrix;

        ShaderUniformVar3 u_center; //center x,y and radius
        ShaderUniformVar1 s_texture; //lookup 
        

        public RadialGradientFillShader(ShaderSharedResource shareRes)
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
                    attribute vec2 a_position;
                    uniform mat4 u_mvpMatrix;                     
                    uniform vec2 u_ortho_offset;   
                    void main()
                    {
                        gl_Position = u_mvpMatrix* vec4(u_ortho_offset+ a_position,0,1);  
                    }";


                //fragment source
                //use clamp
                string fs = @"
                        precision mediump float; 
                        uniform vec3 u_center; 
                        uniform sampler2D s_texture;
                        uniform mat3 u_invertedTxMatrix;

                        void main()
                        {
                            vec4 pos=gl_FragCoord;                            
                            vec3 new_pos =  u_invertedTxMatrix* vec3(pos.x,pos.y,1.0); 
                            float r_distance= sqrt((new_pos.x-u_center.x)* (new_pos.x-u_center.x) + (new_pos.y -u_center.y)*(new_pos.y-u_center.y))/(u_center.z);                            
                            gl_FragColor= texture2D(s_texture,vec2(clamp(r_distance,0.0,0.9),0.0));
                        }
                    ";

                //fragment source
                //old version
                //string fs = @"
                //        precision mediump float; 
                //        uniform vec3 u_center; 
                //        uniform sampler2D s_texture;
                //        uniform mat3 u_invertedTxMatrix;

                //        void main()
                //        {
                //            vec4 pos=gl_FragCoord;                            
                //            vec3 new_pos =  u_invertedTxMatrix* vec3(pos.x,pos.y,1.0);                            

                //            //float r_distance= sqrt((pos.x-u_center.x)* (pos.x-u_center.x) + (pos.y -u_center.y)*(pos.y-u_center.y))/u_center.z;
                //            float r_distance= sqrt((new_pos.x-u_center.x)* (new_pos.x-u_center.x) + (new_pos.y -u_center.y)*(new_pos.y-u_center.y))/(u_center.z);

                //            if(r_distance >=0.9){
                //               gl_FragColor= texture2D(s_texture,vec2(0.9,0.0));
                //            }else{
                //               gl_FragColor= texture2D(s_texture,vec2(r_distance,0.0));
                //            }
                //        }
                //    ";


                //old version
                //string fs = @"
                //        precision mediump float; 
                //        uniform vec3 u_center; 
                //        uniform sampler2D s_texture;
                //        uniform mat3 u_invertedTxMatrix;

                //        void main()
                //        {
                //            vec4 pos=gl_FragCoord;                            
                //            vec3 new_pos =  u_invertedTxMatrix* vec3(pos.x,pos.y,1.0); 

                //            float r_distance= sqrt((new_pos.x-u_center.x)* (new_pos.x-u_center.x) + (new_pos.y -u_center.y)*(new_pos.y-u_center.y))/(u_center.z);

                //            if(r_distance >=0.9){
                //               gl_FragColor= texture2D(s_texture,vec2(0.9,0.0));
                //            }else{
                //               gl_FragColor= texture2D(s_texture,vec2(r_distance,0.0));
                //            }
                //        }
                //    ";

                //in fragment shader if we not 'clamp' r_distance
                //  if(r_distance > 1.0) r_distance =1
                //then the pattern will be repeat. ***

                if (!_shaderProgram.Build(vs, fs))
                {
                    throw new NotSupportedException();
                }

                SaveCompiledShader();
            }
            a_position = _shaderProgram.GetAttrV2f("a_position");
            u_matrix = _shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_orthov_offset = _shaderProgram.GetUniform2("u_ortho_offset");

            u_center = _shaderProgram.GetUniform3("u_center");
            s_texture = _shaderProgram.GetUniform1("s_texture");
            u_invertedTxMatrix = _shaderProgram.GetUniformMat3("u_invertedTxMatrix");
        } 
        public void Render(float[] v2fArray, float cx, float cy, float r, PixelFarm.CpuBlit.VertexProcessing.Affine invertedAffineTx, GLBitmap lookupBmp)
        {
            SetCurrent();
            CheckViewMatrix();
            //----------------------------------------------------
            a_position.LoadPureV2f(v2fArray);
            u_center.SetValue(cx, cy, r);
            UploadGradientLookupTable(lookupBmp);

            if (invertedAffineTx != null)
            {
                float[] mat3x3 = invertedAffineTx.Get3x3MatrixElements();
                u_invertedTxMatrix.SetData(mat3x3);
            }
            else
            {
                //identity mat
                u_invertedTxMatrix.SetData(mat3x3Identity);
            }
            GL.DrawArrays(BeginMode.Triangles, 0, v2fArray.Length / 2);
        }
        static readonly float[] mat3x3Identity = new float[]
        {
            1,0,0,
            0,1,0,
            0,0,1
        };
        void UploadGradientLookupTable(GLBitmap bmp)
        {
            //load before use with RenderSubImage 
            //-------------------------------------------------------------------------------------
            // Bind the texture...
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
            // Set the texture sampler to texture unit to 0     
            s_texture.SetValue(0);
        }
    }
}
