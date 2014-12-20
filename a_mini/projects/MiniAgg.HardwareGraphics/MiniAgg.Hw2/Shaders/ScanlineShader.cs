//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using OpenTK.Graphics.ES20;

namespace LayoutFarm.DrawingGL
{
    /// <summary>
    /// Agg Scanline shader
    /// </summary>
    class ScanlineShader
    {
        bool isInited;
        MiniShaderProgram shaderProgram;
        ShaderVtxAttrib a_position;
        ShaderVtxAttrib a_color;
        ShaderVtxAttrib a_textureCoord;

        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar4 u_solidColor;
       

        public ScanlineShader()
        {
            shaderProgram = new MiniShaderProgram(); 
        }
        
        public MyMat4 ViewMatrix
        {
            get;
            set;
        }

        public void InitShader()
        {
            if (isInited) { return; }
            //----------------


            //vertex shader source
            string vs = @"        
            attribute vec2 a_position;
            attribute vec4 a_color; 
            attribute vec2 a_texcoord;
            
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;
            uniform int u_useSolidColor;            

            varying vec4 v_color;
            varying vec2 v_texcoord;
             
            void main()
            {
                
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                if(u_useSolidColor !=0)
                {
                    v_color= u_solidColor;                   
                    
                }
                else
                {
                    v_color = a_color;
                } 
                v_texcoord= a_texcoord;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color; 
                varying vec2 v_texcoord;                 
                void main()
                {       
                    gl_FragColor= v_color;
                }
            ";

            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            } 

            a_position = shaderProgram.GetVtxAttrib("a_position");
            a_color = shaderProgram.GetVtxAttrib("a_color");
            a_textureCoord = shaderProgram.GetVtxAttrib("a_texcoord"); 

            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor"); 
           
            isInited = true;
        }





    }

}