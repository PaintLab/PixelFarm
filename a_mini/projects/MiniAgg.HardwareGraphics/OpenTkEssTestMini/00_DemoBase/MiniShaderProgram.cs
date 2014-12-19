//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;

using OpenTK.Graphics.ES20;


namespace Mini
{
    public struct ShaderAttribute
    {
        internal readonly int location;
        public ShaderAttribute(int location)
        {
            this.location = location;
        }
        /// <summary>
        /// load and enable
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="fieldCount"></param>
        /// <param name="startOffset"></param>
        public void LoadV3f(float[] vertices, int fieldCount, int startOffset)
        {
            BindV3f(vertices, fieldCount, startOffset);
            Enable();
        }
        /// <summary>
        /// load and enable
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="fieldCount"></param>
        /// <param name="startOffset"></param>
        public void LoadV2f(float[] vertices, int fieldCount, int startOffset)
        {
            BindV2f(vertices, fieldCount, startOffset);
            Enable();
        }
        public void BindV3f(float[] vertices, int fieldCount, int startOffset)
        {
            if (startOffset == 0)
            {
                GL.VertexAttribPointer(location,
                    3, //float3
                    VertexAttribPointerType.Float,
                    false,
                    fieldCount * sizeof(float), //total size
                    vertices);
            }
            else
            {
                unsafe
                {
                    fixed (float* h = &vertices[0])
                    {
                        GL.VertexAttribPointer(location,
                            3, //float3
                            VertexAttribPointerType.Float,
                            false,
                            fieldCount * sizeof(float), //total size
                            (IntPtr)(h + startOffset));
                    }
                }
            }
        }
        public void BindV2f(float[] vertices, int fieldCount, int startOffset)
        {
            if (startOffset == 0)
            {
                GL.VertexAttribPointer(location,
                    2, //float2
                    VertexAttribPointerType.Float,
                    false,
                    fieldCount * sizeof(float), //total size
                    vertices);
            }
            else
            {
                unsafe
                {
                    fixed (float* h = &vertices[0])
                    {
                        GL.VertexAttribPointer(location,
                            2, //float3
                            VertexAttribPointerType.Float,
                            false,
                            fieldCount * sizeof(float), //total size
                            (IntPtr)(h + startOffset));
                    }
                }
            }
        }
        public void Enable()
        {
            GL.EnableVertexAttribArray(this.location);
        }
    }


    public struct ShaderUniformVar
    {
        internal readonly int location;
        public ShaderUniformVar(int location)
        {
            this.location = location;
        }
        public void Enable()
        {

        }
    }
    public class MiniShaderProgram
    {
        int mProgram;

        string vs;
        string fs;
        public void LoadVertexShaderSource(string vs)
        {
            this.vs = vs;
        }
        public void LoadFragmentShaderSource(string fs)
        {
            this.fs = fs;
        }
        public void DeleteMe()
        {
            GL.DeleteProgram(mProgram);
            this.mProgram = 0;
        }
        public bool Build()
        {

            mProgram = OpenTkEssTest.ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                return false;
            }
            return true;
        }
        public bool Build(string vs, string fs)
        {
            LoadVertexShaderSource(vs);
            LoadFragmentShaderSource(fs);
            mProgram = OpenTkEssTest.ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                return false;
            }
            return true;
        }
        public ShaderAttribute GetAttribVar(string attrName)
        {
            return new ShaderAttribute(GL.GetAttribLocation(mProgram, attrName));
        }
        public ShaderUniformVar GetUniform(string uniformVarName)
        {
            return new ShaderUniformVar(GL.GetUniformLocation(this.mProgram, uniformVarName));
        }
        public void UseProgram()
        {
            GL.UseProgram(mProgram);
        }
    }
}