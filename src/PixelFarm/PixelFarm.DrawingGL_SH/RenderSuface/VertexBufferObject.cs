//MIT, 2014-present, WinterDev
//https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
//https://www.khronos.org/opengl/wiki/Generic_Vertex_Attribute_-_examples
//https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;


namespace PixelFarm.DrawingGL
{

    public class VertexBufferObject : IDisposable
    {
        int _vertexBufferId; // array buffer
        int _indexBufferId; // element buffer
        bool _hasData;


#if DEBUG
        static Dictionary<int, bool> s_dbugVboCount = new Dictionary<int, bool>();
#endif

        public VertexBufferObject()
        {
            //TODO: review how to create vbo object 
#if DEBUG


            dbugId = dbugTotalId++;

            if (s_dbugVboCount.ContainsKey(dbugId))
            {
                //??
            }
            else
            {
                s_dbugVboCount.Add(dbugId, true);
            }
            if (s_dbugVboCount.Count > 10)
            {

            }
            System.Diagnostics.Debug.WriteLine("vbo_dbugId=" + dbugId);
#endif
        }
#if DEBUG
        readonly int dbugId = 0;
        static int dbugTotalId = 0;
#endif

        /// <summary>
        /// we copy data from array to mem, we don't store the vertex array,or index array here
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="vertexBufferLen"></param>
        /// <param name="indexBuffer"></param>
        /// <param name="indexBufferLen"></param>
        public void CreateBuffers(float[] vertexBuffer, int vertexBufferLen, ushort[] indexBuffer, int indexBufferLen)
        {
            if (_hasData)
            {
                throw new NotSupportedException();
            }

            if (vertexBuffer != null)
            {
                if (vertexBuffer.Length == 0)
                {

#if DEBUG
                    //this can occur,
                    //eg. when no glyph data 
                    //
                    //System.Diagnostics.Debugger.Break();
                    //System.Diagnostics.Debug.WriteLine("create_buffers?");
#endif
                    return;
                }
                //1.
                GL.GenBuffers(1, out _vertexBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
                unsafe
                {
                    fixed (void* vertDataPtr = &vertexBuffer[0])
                    {
                        GL.BufferData(BufferTarget.ArrayBuffer,
                         new IntPtr(vertexBufferLen * 4), //size in byte
                         new IntPtr(vertDataPtr),
                         BufferUsage.StaticDraw);   //this version we use static draw
                    }
                }
                // IMPORTANT: Unbind from the buffer when we're done with it.
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

#if DEBUG
                System.Diagnostics.Debug.WriteLine("vbo_create=" + _vertexBufferId);
#endif
            }
            //----
            //2.
            if (indexBuffer != null)
            {
                GL.GenBuffers(1, out _indexBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);
                unsafe
                {
                    fixed (void* indexDataPtr = &indexBuffer[0])
                    {
                        GL.BufferData(BufferTarget.ElementArrayBuffer,
                            new IntPtr(indexBufferLen * 2),
                            new IntPtr(indexDataPtr),
                            BufferUsage.StaticDraw);   //this version we use static draw
                    }
                }
                // IMPORTANT: Unbind from the buffer when we're done with it.
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            _hasData = true;
        }
        /// <summary>
        /// set up vertex data, we don't store the vertex array,or index array here
        /// </summary>
        public void CreateBuffers(float[] vertexBuffer, ushort[] indexBuffer)
        {
            CreateBuffers(
                   vertexBuffer, (vertexBuffer != null) ? vertexBuffer.Length : 0,
                   indexBuffer, (indexBuffer != null) ? indexBuffer.Length : 0);
        }

        internal void CreateBuffers(CpuBlit.ArrayListSpan<float> vertexBuffer, CpuBlit.ArrayListSpan<ushort> indexBuffer)
        {
            if (_hasData)
            {
                throw new NotSupportedException();
            }

            CpuBlit.ArrayListSpan<float>.UnsafeGetInternalArr(vertexBuffer, out int v_beginAt, out int v_len, out float[] v_arr);
            CpuBlit.ArrayListSpan<ushort>.UnsafeGetInternalArr(indexBuffer, out int i_beginAt, out int i_len, out ushort[] i_arr);

            if (v_arr != null)
            {
                if (v_len == 0)
                {

#if DEBUG
                    //this can occur,
                    //eg. when no glyph data 
                    //
                    //System.Diagnostics.Debugger.Break();
                    //System.Diagnostics.Debug.WriteLine("create_buffers?");
#endif
                    return;
                }
                //1.
                GL.GenBuffers(1, out _vertexBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
                unsafe
                {
                    fixed (void* vertDataPtr = &v_arr[v_beginAt])
                    {
                        GL.BufferData(BufferTarget.ArrayBuffer,
                         new IntPtr(v_len * 4), //size in byte
                         new IntPtr(vertDataPtr),
                         BufferUsage.StaticDraw);   //this version we use static draw
                    }
                }
                // IMPORTANT: Unbind from the buffer when we're done with it.
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("vbo_create=" + _vertexBufferId);
#endif
            }
            //----
            //2.
            if (i_arr != null)
            {
                GL.GenBuffers(1, out _indexBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);
                unsafe
                {
                    fixed (void* indexDataPtr = &i_arr[i_beginAt])
                    {
                        GL.BufferData(BufferTarget.ElementArrayBuffer,
                            new IntPtr(i_len * 2),
                            new IntPtr(indexDataPtr),
                            BufferUsage.StaticDraw);   //this version we use static draw
                    }
                }
                // IMPORTANT: Unbind from the buffer when we're done with it.
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            _hasData = true;
        }


        public void Dispose()
        {
            Clear();
        }
        /// <summary>
        /// unload data from server-side mem
        /// </summary>
        public void Clear()
        {
            unsafe
            {

                if (_vertexBufferId > 0 || _indexBufferId > 0)
                {
#if DEBUG
                    if (!s_dbugVboCount.ContainsKey(dbugId))
                    {
                        //???
                    }
                    else
                    {
                        s_dbugVboCount.Remove(dbugId);
                    }
#endif

                    int* toDeleteBufferIndexArr = stackalloc int[] { _vertexBufferId, _indexBufferId };

                    GL.DeleteBuffers(2, toDeleteBufferIndexArr);
                    _vertexBufferId = _indexBufferId = 0;
                }


                _hasData = false;
            }
        }
        /// <summary>
        /// bind array buffer and element array buffer
        /// </summary>
        public void Bind()
        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
            if (_indexBufferId > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);
            }
        }
        /// <summary>
        /// unbine array buffer and element array buffer
        /// </summary>
        public void UnBind()
        {
            // IMPORTANT: Unbind from the buffer when we're done with it.

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        }
        //
        public bool HasData => _hasData;
        //
    }
}