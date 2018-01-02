//MIT, 2016-2018, WinterDev
//we use concept from https://www.mapbox.com/blog/drawing-antialiased-lines/
using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class SmoothLineShader : ShaderBase
    {
        ShaderVtxAttrib4f a_position;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar4 u_solidColor;
        ShaderUniformVar1 u_linewidth;
        public SmoothLineShader(ShaderSharedResource shareRes)
            : base(shareRes)
        {
            string vs = @"                   
            attribute vec4 a_position;  

            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
                
            uniform float u_linewidth;

            varying vec4 v_color; 
            varying float v_distance;
            varying float p0;
            
            void main()
            {   
                
                float rad = a_position[3];
                v_distance= a_position[2];

                float n_x = sin(rad); 
                float n_y = cos(rad);  

                vec4 delta;
                if(v_distance <1.0){                                         
                    delta = vec4(-n_x * u_linewidth,n_y * u_linewidth,0,0);                       
                }else{                      
                    delta = vec4(n_x * u_linewidth,-n_y * u_linewidth,0,0);
                }
    
                if(u_linewidth <= 0.5){
                    p0 = 0.5;      
                }else if(u_linewidth <=1.0){
                    p0 = 0.45;  
                }else if(u_linewidth>1.0 && u_linewidth<3.0){                    
                    p0 = 0.25;  
                }else{
                    p0= 0.1;
                }
                
                vec4 pos = vec4(a_position[0],a_position[1],0,1) + delta;                 
                gl_Position = u_mvpMatrix* pos;                
                v_color= u_solidColor;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color;  
                varying float v_distance;
                varying float p0;                
                void main()
                {
                    float d0= v_distance; 
                    float p1= 1.0-p0;
                    float factor= 1.0 /p0;
            
                    if(d0 < p0){                        
                        gl_FragColor =vec4(v_color[0],v_color[1],v_color[2], v_color[3] *(d0 * factor));
                    }else if(d0> p1){                         
                        gl_FragColor =vec4(v_color[0],v_color[1],v_color[2], v_color[3] *((1.0-d0)* factor));
                    }
                    else{ 
                        gl_FragColor =v_color; 
                    } 
                }
            ";
            //---------------------
            if (!shaderProgram.Build(vs, fs))
            {
                return;
            }
            //-----------------------
            a_position = shaderProgram.GetAttrV4f("a_position");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
            u_linewidth = shaderProgram.GetUniform1("u_linewidth");
        }
        int orthoviewVersion = -1;
        void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _shareRes.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_shareRes.OrthoView.data);
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
            u_linewidth.SetValue(_shareRes._strokeWidth / 2f);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);
        }
        public void DrawTriangleStrips(MultiPartTessResult multipartTessResult)
        {
            throw new NotSupportedException();
        }
        public void DrawTriangleStrips(float[] coords, int ncount)
        {
            SetCurrent();
            CheckViewMatrix();

            _shareRes.AssignStrokeColorToVar(u_solidColor);
            u_linewidth.SetValue(_shareRes._strokeWidth / 2f);
            //
            a_position.LoadPureV4f(coords);
            //because original stroke width is the width of both side of
            //the line, but u_linewidth is the half of the strokeWidth            
            GL.DrawArrays(BeginMode.TriangleStrip, 0, ncount);
        }

        public void DrawTriangleStrips(MultiPartTessResult multipartTessResult, int index, PixelFarm.Drawing.Color color)
        {

            ////note (A):
            ////from https://www.khronos.org/registry/OpenGL-Refpages/es2.0/xhtml/glVertexAttribPointer.xml
            ////... If a non-zero named buffer object is bound to the GL_ARRAY_BUFFER target (see glBindBuffer)
            ////while a generic vertex attribute array is specified,
            ////pointer is treated as **a byte offset** into the buffer object's data store.  

            SetCurrent();
            CheckViewMatrix();
            //--------------------
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);

            _shareRes.AssignStrokeColorToVar(u_solidColor);
            //because original stroke width is the width of both side of
            //the line, but u_linewidth is the half of the strokeWidth
            u_linewidth.SetValue(_shareRes._strokeWidth / 2f);
            //--------------------
            VertexBufferObject borderVBO = multipartTessResult.GetBorderVBO();
            borderVBO.Bind();
            BorderPart p = multipartTessResult.GetBorderPartRange(index);
            //get part range from border part
            int borderSetIndex = p.beginAtBorderSetIndex;
            for (int i = 0; i < p.count; ++i)
            {
                PartRange borderset = multipartTessResult.GetSmoothBorderPartRange(borderSetIndex + i);
                a_position.LoadLatest(borderset.beginVertexAt * 4);
                GL.DrawArrays(BeginMode.TriangleStrip, 0, borderset.elemCount);
            }
            borderVBO.UnBind(); //unbind
        }
#if DEBUG
        public void dbugDrawTriangleStrips(MultiPartTessResult multipartTessResult)
        {
            ////backup
            //System.Collections.Generic.List<SmoothBorderSet> borderSets = multipartTessResult.GetAllSmoothBorderSet();
            //int j = borderSets.Count;
            //for (int i = 0; i < j; ++i)
            //{
            //    SmoothBorderSet borderSet = borderSets[i];
            //    DrawTriangleStrips(
            //      borderSet.smoothBorderArr,
            //      borderSet.vertexStripCount);
            //}
        }
#endif
    }
}