//MIT 2016, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    class InvertAlphaFragmentShader
    {
        MiniShaderProgram shaderProgram = new MiniShaderProgram();
        ShaderVtxAttrib4f a_position;
        ShaderVtxAttrib4f a_color;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar4 u_solidColor;
        ShaderUniformVar1 u_linewidth;
        MyMat4 orthoView;
        Drawing.Color _strokeColor;
        float _strokeWidth = 1;
        public InvertAlphaFragmentShader()
        {
            InitShader();
        }
        bool InitShader()
        {
            string vs = @"                   
            attribute vec4 a_position; 
            attribute vec4 a_color;            

            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;              
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

                if(u_useSolidColor !=0)
                {
                    v_color= u_solidColor;
                }
                else
                {
                    v_color = a_color;
                }
            }
            ";
            //fragment source
            //this is invert fragment shader *** 
            //so we 
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
                        gl_FragColor = vec4(v_color[0],v_color[1],v_color[2], 1.0 -(v_color[3] *(d0 * factor)));
                    }else if(d0> p1){                         
                        gl_FragColor= vec4(v_color[0],v_color[1],v_color[2],1.0-(v_color[3] *((1.0-d0)* factor)));
                    }
                    else{ 
                       gl_FragColor = vec4(0,0,0,0);
                    } 
                }
            ";
            //---------------------
            if (!shaderProgram.Build(vs, fs))
            {
                return false;
            }
            //-----------------------

            a_position = shaderProgram.GetAttrV4f("a_position");
            a_color = shaderProgram.GetAttrV4f("a_color");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
            u_linewidth = shaderProgram.GetUniform1("u_linewidth");
            return true;
        }
        public MyMat4 OrthoView
        {
            get { return orthoView; }
            set { orthoView = value; }
        }
        public float StrokeWidth
        {
            get { return _strokeWidth; }
            set
            {
                _strokeWidth = value;
            }
        }

        public Drawing.Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
            }
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            float rad1 = (float)Math.Atan2(
                   y2 - y1,  //dy
                   x2 - x1); //dx
            float[] vtxs = new float[] {
                x1, y1,0,rad1,
                x1, y1,1,rad1,
                x2, y2,0,rad1,
                //-------
                x2, y2,1,rad1
            };
            shaderProgram.UseProgram();
            u_matrix.SetData(orthoView.data);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(
                  _strokeColor.R / 255f,
                  _strokeColor.G / 255f,
                  _strokeColor.B / 255f,
                  _strokeColor.A / 255f);
            a_position.LoadPureV4f(vtxs);
            u_linewidth.SetValue(_strokeWidth);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);
        }

        public void DrawPolyline(float[] coords, int coordCount)
        {
            //from user input coords
            //expand it
            List<float> expandCoords = new List<float>();
            for (int i = 0; i < coordCount;)
            {
                CreateLineSegment(expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                i += 2;
            }

            shaderProgram.UseProgram();
            u_matrix.SetData(orthoView.data);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(
                  _strokeColor.R / 255f,
                  _strokeColor.G / 255f,
                  _strokeColor.B / 255f,
                  _strokeColor.A / 255f);
            a_position.LoadPureV4f(expandCoords.ToArray());
            u_linewidth.SetValue(_strokeWidth);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, coordCount * 2);
        }



        public void DrawPolygon(float[] coords, int coordCount)
        {
            //from user input coords
            //expand it
            List<float> expandCoords = new List<float>();
            int lim = coordCount - 2;
            for (int i = 0; i < lim;)
            {
                CreateLineSegment(expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                i += 2;
            }
            //close coord
            CreateLineSegment(expandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[0], coords[1]);
            //we need exact close the polygon
            CreateLineSegment(expandCoords, coords[0], coords[1], coords[0], coords[1]);
            shaderProgram.UseProgram();
            u_matrix.SetData(orthoView.data);
            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(
                  _strokeColor.R / 255f,
                  _strokeColor.G / 255f,
                  _strokeColor.B / 255f,
                  _strokeColor.A / 255f);
            a_position.LoadPureV4f(expandCoords.ToArray());
            u_linewidth.SetValue(_strokeWidth);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, (coordCount + 2) * 2);
        }

        static void CreateLineSegment(List<float> coords, float x1, float y1, float x2, float y2)
        {
            //create wiht no line join
            float dx = x2 - x1;
            float dy = y2 - y1;
            float rad1 = (float)Math.Atan2(
                   y2 - y1,  //dy
                   x2 - x1); //dx
            coords.Add(x1); coords.Add(y1); coords.Add(0); coords.Add(rad1);
            coords.Add(x1); coords.Add(y1); coords.Add(1); coords.Add(rad1);
            coords.Add(x2); coords.Add(y2); coords.Add(0); coords.Add(rad1);
            coords.Add(x2); coords.Add(y2); coords.Add(1); coords.Add(rad1);
        }
        public void DrawLine2(float x1, float y1, float x2, float y2)
        {
            //test only ***
            float dx = x2 - x1;
            float dy = y2 - y1;
            float rad1 = (float)Math.Atan2(
                   y2 - y1,  //dy
                   x2 - x1); //dx
            float[] vtxs = new float[] {
                x1, y1,0,rad1,
                x1, y1,1,rad1,
                x2, y2,0,rad1,
                //-------
                x2, y2,1,rad1
            };
            a_position.LoadPureV4f(vtxs);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);
        }
    }
}