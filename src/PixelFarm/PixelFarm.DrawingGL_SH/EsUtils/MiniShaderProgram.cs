//MIT, 2014-present, WinterDev

//ES30 and ES20

using System;
namespace OpenTK.Graphics.ES30
{

    public struct ShaderVtxAttrib2f
    {
        readonly int _location;
        public ShaderVtxAttrib2f(int location)
        {
            _location = location;
        }
        public unsafe void UnsafeLoadMixedV2f(float* vertexH, int totalFieldCount)
        {
            GL.VertexAttribPointer(_location, 2, VertexAttribPointerType.Float, false, totalFieldCount * sizeof(float), (IntPtr)vertexH);
            GL.EnableVertexAttribArray(_location);
        }

        /// <summary>
        /// load pure vector2f, from start array
        /// </summary>
        /// <param name="vertices"></param>
        public void LoadPureV2f(float[] vertices)
        {
            //bind 
            GL.VertexAttribPointer(_location,
                2, //float2
                VertexAttribPointerType.Float,
                false,
                2 * sizeof(float), //total size
                vertices);
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load pure vector2f, from start array
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="totalFieldCount"></param>
        /// <param name="startOffset"></param>
        public unsafe void UnsafeLoadPureV2f(float* vertices)
        {
            //bind 
            GL.VertexAttribPointer(_location,
                2, //float2
                VertexAttribPointerType.Float,
                false,
                 2 * sizeof(float),
                (IntPtr)(vertices));
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                2,
                VertexAttribPointerType.Float,
                false,
                2 * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);

        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int totalFieldCount, int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                2,
                VertexAttribPointerType.Float,
                false,
                totalFieldCount * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
    }
    public struct ShaderVtxAttrib3f
    {
        readonly int _location;
        public ShaderVtxAttrib3f(int location)
        {
            _location = location;
        }
        public unsafe void UnsafeLoadMixedV3f(float* vertexH, int totalFieldCount)
        {
            GL.VertexAttribPointer(_location, 3, VertexAttribPointerType.Float, false, totalFieldCount * sizeof(float), (IntPtr)vertexH);
            GL.EnableVertexAttribArray(_location);
        }
        public void LoadPureV3f(float[] vertices)
        {
            //bind 
            GL.VertexAttribPointer(_location,
                3, //float3
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float), //total size
                vertices);
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int totalFieldCount, int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                3,
                VertexAttribPointerType.Float,
                false,
                totalFieldCount * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                3,
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
    }
    public struct ShaderVtxAttrib4f
    {
        readonly int _location;
        public ShaderVtxAttrib4f(int location)
        {
            _location = location;
        }
        /// <summary>
        ///  load pure vector4f, from start array
        /// </summary>
        /// <param name="vertices"></param>
        public void LoadPureV4f(float[] vertices)
        {
            unsafe
            {
                fixed (float* h = &vertices[0])
                {
                    LoadPureV4fUnsafe(h);
                }
            }
        }
        public unsafe void LoadPureV4fUnsafe(float* vertices)
        {
            GL.VertexAttribPointer(_location,
                       4, //float4
                       VertexAttribPointerType.Float,
                       false,
                       4 * sizeof(float), //total size
                       (IntPtr)vertices);
            GL.EnableVertexAttribArray(_location);
        }

        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int offset = 0)
        {
            GL.VertexAttribPointer(_location,
                4,
                VertexAttribPointerType.Float,
                false,
                4 * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
    }


    public struct ShaderUniformMatrix4
    {
        readonly int _location;
        public ShaderUniformMatrix4(int location)
        {
            _location = location;
        }
        public void SetData(int count, bool transpose, float[] mat)
        {
            GL.UniformMatrix4(_location, count, transpose, mat);
        }
        public void SetData(float[] mat)
        {
            GL.UniformMatrix4(_location, 1, false, mat);
        }
    }
    public struct ShaderUniformMatrix3
    {
        readonly int location;
        public ShaderUniformMatrix3(int location)
        {
            this.location = location;
        }
        public void SetData(int count, bool transpose, float[] mat)
        {
            GL.UniformMatrix3(this.location, count, transpose, mat);
        }
        public void SetData(float[] mat)
        {
            GL.UniformMatrix3(this.location, 1, false, mat);
        }
    }
    public struct ShaderUniformVar1
    {
        readonly int _location;
        public ShaderUniformVar1(int location)
        {
            _location = location;
        }
        public void SetValue(float value)
        {
            GL.Uniform1(_location, value);
        }
        public void SetValue(int value)
        {
            GL.Uniform1(_location, value);
        }
        public void SetValue(bool value)
        {
            GL.Uniform1(_location, value ? 1 : 0);
        }
    }
    public struct ShaderUniformVar2
    {
        readonly int _location;
        public ShaderUniformVar2(int location)
        {
            _location = location;
        }
        public void SetValue(int a, int b)
        {
            GL.Uniform2(_location, a, b);
        }
        public void SetValue(float a, float b)
        {
            GL.Uniform2(_location, a, b);
        }
        public void SetValue(byte a, byte b)
        {
            GL.Uniform2(_location, a, b);
        }
    }
    public struct ShaderUniformVar3
    {
        readonly int _location;
        public ShaderUniformVar3(int location)
        {
            _location = location;
        }
        public void SetValue(int a, int b, int c)
        {
            GL.Uniform3(_location, a, b, c);
        }
        public void SetValue(float a, float b, float c)
        {
            GL.Uniform3(_location, a, b, c);
        }
        public void SetValue(byte a, byte b, byte c)
        {
            GL.Uniform3(_location, a, b, c);
        }
    }
    public struct ShaderUniformVar4
    {
        readonly int _location;
        public ShaderUniformVar4(int location)
        {
            _location = location;
        }
        public void SetValue(int a, int b, int c, int d)
        {
            GL.Uniform4(_location, a, b, c, d);
        }
        public void SetValue(float a, float b, float c, float d)
        {
            GL.Uniform4(_location, a, b, c, d);
        }
        public void SetValue(byte a, byte b, byte c, byte d)
        {
            GL.Uniform4(_location, a, b, c, d);
        }
    }


    public class MiniShaderProgram
    {
        int _program_id;
        public MiniShaderProgram()
        {
        }

        public int ProgramId => _program_id;


        public ShaderVtxAttrib2f GetAttrV2f(string attrName) => new ShaderVtxAttrib2f(GL.GetAttribLocation(_program_id, attrName));

        public ShaderVtxAttrib3f GetAttrV3f(string attrName) => new ShaderVtxAttrib3f(GL.GetAttribLocation(_program_id, attrName));

        public ShaderVtxAttrib4f GetAttrV4f(string attrName) => new ShaderVtxAttrib4f(GL.GetAttribLocation(_program_id, attrName));

        public ShaderUniformVar1 GetUniform1(string uniformVarName) => new ShaderUniformVar1(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformVar2 GetUniform2(string uniformVarName) => new ShaderUniformVar2(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformVar3 GetUniform3(string uniformVarName) => new ShaderUniformVar3(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformVar4 GetUniform4(string uniformVarName) => new ShaderUniformVar4(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformMatrix4 GetUniformMat4(string uniformVarName) => new ShaderUniformMatrix4(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformMatrix3 GetUniformMat3(string uniformVarName) => new ShaderUniformMatrix3(GL.GetUniformLocation(_program_id, uniformVarName));

        public bool Build(string vs, string fs)
        {
            try
            {
                _program_id = OpenTK.Graphics.ES20.EsUtils.CompileProgram(vs, fs);
            }
            catch (Exception ex)
            {
            }
            return _program_id != 0;
        }

        public void UseProgram()
        {
            GL.UseProgram(_program_id);
        }
        public void DeleteProgram()
        {
            GL.DeleteProgram(_program_id);
            _program_id = 0;
        }

        //ref: https://github.com/Microsoft/angle/wiki/Caching-compiled-program-binaries 

        //from https://github.com/Microsoft/angle 
        //file gl2ext.h

        //# ifndef GL_OES_get_program_binary
        //#define GL_OES_get_program_binary 1
        //#define GL_PROGRAM_BINARY_LENGTH_OES      0x8741
        //#define GL_NUM_PROGRAM_BINARY_FORMATS_OES 0x87FE
        //#define GL_PROGRAM_BINARY_FORMATS_OES     0x87FF

        const int GL_PROGRAM_BINARY_LENGTH_OES = 0x8741;

        //this is an optional feature
        public bool SaveCompiledShader(System.IO.BinaryWriter w)
        {
            //1. First we find out the length of the program binary.
            GL.GetProgram(_program_id, (ProgramParameter)GL_PROGRAM_BINARY_LENGTH_OES, out int prog_bin_len);

            if (prog_bin_len == 0) return false; //?

            //2. Then we create a buffer of the correct length.
            byte[] binaryData = new byte[prog_bin_len];

            //3. Then we retrieve the program binary.
            int writtenLen = 0;
            All binFormat = 0;

            unsafe
            {

                fixed (byte* binDataPtr = &binaryData[0])
                {
                    GL.GetProgramBinary(_program_id, prog_bin_len, &writtenLen, &binFormat, (System.IntPtr)binDataPtr);
                }
            }
            //using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
            //using (System.IO.BinaryWriter w = new System.IO.BinaryWriter(fs))
            //{
            //1. format (int)
            //2. len of actual programs binary (int)
            //3. program binary
            w.Write((int)binFormat);
            w.Write((int)prog_bin_len);
            w.Write(binaryData);
            w.Flush();
            //}
            return true;
        }
        public bool LoadCompiledShader(System.IO.BinaryReader reader)
        {
            All binFormat = 0;
            int prog_bin_len = 0;
            byte[] compiled_binary = null;
            //using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            //using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
            //{
            //1. format (int)
            //2. len of actual programs binary (int)
            //3. program binary
            binFormat = (All)reader.ReadInt32();
            prog_bin_len = reader.ReadInt32();
            compiled_binary = reader.ReadBytes(prog_bin_len);
            //}

            unsafe
            {
                fixed (byte* compiled_binary_ptr = &compiled_binary[0])
                {

                    _program_id = GL.CreateProgram();

                    // update the program's data. 



                    //GL.Oes.ProgramBinary(_program_id, binFormat, (System.IntPtr)compiled_binary_ptr, prog_bin_len);
                    //// Check the link status, which indicates whether glProgramBinaryOES() succeeded.
                    //GL.GetProgram(_program_id, GetProgramParameterName.LinkStatus, out int linkStatus);


                    GL.ProgramBinary(_program_id, binFormat, (System.IntPtr)compiled_binary_ptr, prog_bin_len);
                    GL.GetProgram(_program_id, ProgramParameter.LinkStatus, out int linkStatus);

                    if (linkStatus != 0)
                    {
                        //success
                        return true;
                    }
                    else
                    {
                        GL.DeleteProgram(_program_id);//?
                        //# ifdef _DEBUG
                        // Code to help debug programs failing to load.
                        // GLint infoLogLength;
                        // glGetProgramiv(program, GL_INFO_LOG_LENGTH, &infoLogLength);

                        // if (infoLogLength > 0)
                        // {
                        //   std::vector<GLchar> infoLog(infoLogLength);
                        //   glGetProgramInfoLog(program, infoLog.size(), NULL, &infoLog[0]);
                        //   OutputDebugStringA(&infoLog[0]);
                        //}
                        //#endif // _DEBUG
                        return false;
                    }
                }
            }
        }


    }
}

/// ////////////////////////////////////////// 
//TODO: plan remove ES20

namespace OpenTK.Graphics.ES20
{

    public struct ShaderVtxAttrib2f
    {
        readonly int _location;
        public ShaderVtxAttrib2f(int location)
        {
            _location = location;
        }
        public unsafe void UnsafeLoadMixedV2f(float* vertexH, int totalFieldCount)
        {
            GL.VertexAttribPointer(_location, 2, VertexAttribPointerType.Float, false, totalFieldCount * sizeof(float), (IntPtr)vertexH);
            GL.EnableVertexAttribArray(_location);
        }

        /// <summary>
        /// load pure vector2f, from start array
        /// </summary>
        /// <param name="vertices"></param>
        public void LoadPureV2f(float[] vertices)
        {
            //bind 
            GL.VertexAttribPointer(_location,
                2, //float2
                VertexAttribPointerType.Float,
                false,
                2 * sizeof(float), //total size
                vertices);
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load pure vector2f, from start array
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="totalFieldCount"></param>
        /// <param name="startOffset"></param>
        public unsafe void UnsafeLoadPureV2f(float* vertices)
        {
            //bind 
            GL.VertexAttribPointer(_location,
                2, //float2
                VertexAttribPointerType.Float,
                false,
                 2 * sizeof(float),
                (IntPtr)(vertices));
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                2,
                VertexAttribPointerType.Float,
                false,
                2 * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);

        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int totalFieldCount, int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                2,
                VertexAttribPointerType.Float,
                false,
                totalFieldCount * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
    }
    public struct ShaderVtxAttrib3f
    {
        readonly int _location;
        public ShaderVtxAttrib3f(int location)
        {
            _location = location;
        }
        public unsafe void UnsafeLoadMixedV3f(float* vertexH, int totalFieldCount)
        {
            GL.VertexAttribPointer(_location, 3, VertexAttribPointerType.Float,
                false, totalFieldCount * sizeof(float), (IntPtr)vertexH);
            GL.EnableVertexAttribArray(_location);
        }
        public void LoadPureV3f(float[] vertices)
        {
            //bind 
            GL.VertexAttribPointer(_location,
                3, //float3
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float), //total size
                vertices);
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int totalFieldCount, int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                3,
                VertexAttribPointerType.Float,
                false,
                totalFieldCount * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int offset = 0)
        {

            GL.VertexAttribPointer(_location,
                3,
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
    }
    public struct ShaderVtxAttrib4f
    {
        readonly int _location;
        public ShaderVtxAttrib4f(int location)
        {
            _location = location;
        }
        /// <summary>
        ///  load pure vector4f, from start array
        /// </summary>
        /// <param name="vertices"></param>
        public void LoadPureV4f(float[] vertices)
        {
            unsafe
            {
                fixed (float* h = &vertices[0])
                {
                    LoadPureV4fUnsafe(h);
                }
            }
        }
        public unsafe void LoadPureV4fUnsafe(float* vertices)
        {
            GL.VertexAttribPointer(_location,
                       4, //float4
                       VertexAttribPointerType.Float,
                       false,
                       4 * sizeof(float), //total size
                       (IntPtr)vertices);
            GL.EnableVertexAttribArray(_location);
        }

        /// <summary>
        /// load latest server side data
        /// </summary>
        public void LoadLatest(int offset = 0)
        {
            GL.VertexAttribPointer(_location,
                4,
                VertexAttribPointerType.Float,
                false,
                4 * sizeof(float),
                offset);
            GL.EnableVertexAttribArray(_location);
        }
    }


    public struct ShaderUniformMatrix4
    {
        readonly int _location;
        public ShaderUniformMatrix4(int location)
        {
            _location = location;
        }
        public void SetData(int count, bool transpose, float[] mat)
        {
            GL.UniformMatrix4(_location, count, transpose, mat);
        }
        public void SetData(float[] mat)
        {
            GL.UniformMatrix4(_location, 1, false, mat);
        }
    }
    public struct ShaderUniformMatrix3
    {
        readonly int _location;
        public ShaderUniformMatrix3(int location)
        {
            _location = location;
        }
        public void SetData(int count, bool transpose, float[] mat)
        {
            GL.UniformMatrix3(_location, count, transpose, mat);
        }
        public void SetData(float[] mat)
        {
            GL.UniformMatrix3(_location, 1, false, mat);
        }
    }
    public struct ShaderUniformVar1
    {
        readonly int _location;
        public ShaderUniformVar1(int location)
        {
            _location = location;
        }
        public void SetValue(float value)
        {
            GL.Uniform1(_location, value);
        }
        public void SetValue(int value)
        {
            GL.Uniform1(_location, value);
        }
        public void SetValue(bool value)
        {
            GL.Uniform1(_location, value ? 1 : 0);
        }
    }
    public struct ShaderUniformVar2
    {
        readonly int _location;
        public ShaderUniformVar2(int location)
        {
            _location = location;
        }
        public void SetValue(int a, int b)
        {
            GL.Uniform2(_location, a, b);
        }
        public void SetValue(float a, float b)
        {
            GL.Uniform2(_location, a, b);
        }
        public void SetValue(byte a, byte b)
        {
            GL.Uniform2(_location, a, b);
        }
    }
    public struct ShaderUniformVar3
    {
        readonly int _location;
        public ShaderUniformVar3(int location)
        {
            _location = location;
        }
        public void SetValue(int a, int b, int c)
        {
            GL.Uniform3(_location, a, b, c);
        }
        public void SetValue(float a, float b, float c)
        {
            GL.Uniform3(_location, a, b, c);
        }
        public void SetValue(byte a, byte b, byte c)
        {
            GL.Uniform3(_location, a, b, c);
        }
    }
    public struct ShaderUniformVar4
    {
        readonly int _location;
        public ShaderUniformVar4(int location)
        {
            _location = location;
        }
        public void SetValue(int a, int b, int c, int d)
        {
            GL.Uniform4(_location, a, b, c, d);
        }
        public void SetValue(float a, float b, float c, float d)
        {
            GL.Uniform4(_location, a, b, c, d);
        }
        public void SetValue(byte a, byte b, byte c, byte d)
        {
            GL.Uniform4(_location, a, b, c, d);
        }
    }


    public class MiniShaderProgram
    {
        int _program_id;
        public MiniShaderProgram()
        {
        }

        //public int ProgramId => _program_id;


        public ShaderVtxAttrib2f GetAttrV2f(string attrName) => new ShaderVtxAttrib2f(GL.GetAttribLocation(_program_id, attrName));

        public ShaderVtxAttrib3f GetAttrV3f(string attrName) => new ShaderVtxAttrib3f(GL.GetAttribLocation(_program_id, attrName));

        public ShaderVtxAttrib4f GetAttrV4f(string attrName) => new ShaderVtxAttrib4f(GL.GetAttribLocation(_program_id, attrName));

        public ShaderUniformVar1 GetUniform1(string uniformVarName) => new ShaderUniformVar1(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformVar2 GetUniform2(string uniformVarName) => new ShaderUniformVar2(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformVar3 GetUniform3(string uniformVarName) => new ShaderUniformVar3(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformVar4 GetUniform4(string uniformVarName) => new ShaderUniformVar4(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformMatrix4 GetUniformMat4(string uniformVarName) => new ShaderUniformMatrix4(GL.GetUniformLocation(_program_id, uniformVarName));

        public ShaderUniformMatrix3 GetUniformMat3(string uniformVarName) => new ShaderUniformMatrix3(GL.GetUniformLocation(_program_id, uniformVarName));

        public bool Build(string vs, string fs)
        {
            try
            {
                _program_id = OpenTK.Graphics.ES20.EsUtils.CompileProgram(vs, fs);
            }
            catch (Exception ex)
            {
            }
            return _program_id != 0;
        }

        public void UseProgram()
        {
            GL.UseProgram(_program_id);
        }
        public void DeleteProgram()
        {
            GL.DeleteProgram(_program_id);
            _program_id = 0;
        }

        //ref: https://github.com/Microsoft/angle/wiki/Caching-compiled-program-binaries 

        //from https://github.com/Microsoft/angle 
        //file gl2ext.h

        //# ifndef GL_OES_get_program_binary
        //#define GL_OES_get_program_binary 1
        //#define GL_PROGRAM_BINARY_LENGTH_OES      0x8741
        //#define GL_NUM_PROGRAM_BINARY_FORMATS_OES 0x87FE
        //#define GL_PROGRAM_BINARY_FORMATS_OES     0x87FF

        const int GL_PROGRAM_BINARY_LENGTH_OES = 0x8741;

        //this is an optional feature
        public bool SaveCompiledShader(System.IO.BinaryWriter w)
        {
            //1. First we find out the length of the program binary.
            //GL.GetProgram(_program_id, (GetProgramParameterName)GL_PROGRAM_BINARY_LENGTH_OES, out int prog_bin_len);
            GL.GetProgram(_program_id, (ProgramParameter)GL_PROGRAM_BINARY_LENGTH_OES, out int prog_bin_len);

            if (prog_bin_len == 0) return false; //?

            //2. Then we create a buffer of the correct length.
            byte[] binaryData = new byte[prog_bin_len];

            //3. Then we retrieve the program binary.
            int writtenLen = 0;
            All binFormat = 0;

            unsafe
            {
                fixed (byte* binDataPtr = &binaryData[0])
                {
#if !IOS_OPENTK
                    GL.Oes.GetProgramBinary(_program_id, prog_bin_len, &writtenLen, &binFormat, (System.IntPtr)binDataPtr);
#endif
                }
            }
            //using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
            //using (System.IO.BinaryWriter w = new System.IO.BinaryWriter(fs))
            //{
            //1. format (int)
            //2. len of actual programs binary (int)
            //3. program binary
            w.Write((int)binFormat);
            w.Write((int)prog_bin_len);
            w.Write(binaryData);
            w.Flush();
            //}
            return true;
        }
        public bool LoadCompiledShader(System.IO.BinaryReader reader)
        {
#if IOS_OPENTK
            return false;
#endif
            All binFormat = 0;
            int prog_bin_len = 0;
            byte[] compiled_binary = null;
            //using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            //using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
            //{
            //1. format (int)
            //2. len of actual programs binary (int)
            //3. program binary
            binFormat = (All)reader.ReadInt32();
            prog_bin_len = reader.ReadInt32();
            compiled_binary = reader.ReadBytes(prog_bin_len);
            //}

            unsafe
            {
                fixed (byte* compiled_binary_ptr = &compiled_binary[0])
                {

                    _program_id = OpenTK.Graphics.ES20.GL.CreateProgram();

#if !IOS_OPENTK
                    // update the program's data. 
                    GL.Oes.ProgramBinary(_program_id, binFormat, (System.IntPtr)compiled_binary_ptr, prog_bin_len);
                    // Check the link status, which indicates whether glProgramBinaryOES() succeeded.
                    GL.GetProgram(_program_id, GetProgramParameterName.LinkStatus, out int linkStatus);
#else
                    int linkStatus = 1;
#endif

                    if (linkStatus != 0)
                    {
                        //success
                        return true;
                    }
                    else
                    {
                        GL.DeleteProgram(_program_id);//?
                        //# ifdef _DEBUG
                        // Code to help debug programs failing to load.
                        // GLint infoLogLength;
                        // glGetProgramiv(program, GL_INFO_LOG_LENGTH, &infoLogLength);

                        // if (infoLogLength > 0)
                        // {
                        //   std::vector<GLchar> infoLog(infoLogLength);
                        //   glGetProgramInfoLog(program, infoLog.size(), NULL, &infoLog[0]);
                        //   OutputDebugStringA(&infoLog[0]);
                        //}
                        //#endif // _DEBUG
                        return false;
                    }
                }
            }
        }


    }
}

//////////////////////////////////////////////