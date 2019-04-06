//MIT, 2014-present, WinterDev
//https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
//https://www.khronos.org/opengl/wiki/Generic_Vertex_Attribute_-_examples
//https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

using System;
using OpenTK.Graphics.ES20;

namespace PixelFarm.DrawingGL
{

    public class VertexBufferObject : IDisposable
    {
        int _vertexBufferId; // array buffer
        int _indexBufferId; // element buffer
        bool _hasData;

        public VertexBufferObject()
        {
            //TODO: review how to create vbo object 
#if DEBUG
            dbugId = dbugTotalId++;
            System.Diagnostics.Debug.WriteLine("vbo_dbugId=" + dbugId);
#endif
        }
#if DEBUG
        readonly int dbugId = 0;
        static int dbugTotalId = 0;
#endif

        //public VertexBufferObject CreateClone()
        //{
        //    VertexBufferObject newclone = new VertexBufferObject();
        //    newclone.CreateBuffers(_vertextBuffer, _indexBuffer);
        //    return newclone;
        //}
        //float[] _vertextBuffer;
        //ushort[] _indexBuffer;
        /// <summary>
        /// set up vertex data, we don't store the vertex array,or index array here
        /// </summary>
        public void CreateBuffers(float[] vertextBuffer, ushort[] indexBuffer)
        {   
            if (_hasData)
            {
                throw new NotSupportedException();
            }
            //_vertextBuffer = vertextBuffer;
            //_indexBuffer = indexBuffer;
            //
            if (vertextBuffer != null)
            {
                //1.
                GL.GenBuffers(1, out _vertexBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
                unsafe
                {
                    fixed (void* vertDataPtr = &vertextBuffer[0])
                    {
                        //GL.BufferData(BufferTarget.ArrayBuffer,
                        //    new IntPtr(vertextBuffer.Length * 4), //size in byte
                        //    new IntPtr(vertDataPtr),
                        //    (BufferUsageHint)BufferUsage.StaticDraw);   //this version we use static draw

                        GL.BufferData(BufferTarget.ArrayBuffer,
                         new IntPtr(vertextBuffer.Length * 4), //size in byte
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
                            new IntPtr(indexBuffer.Length * 2),
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
                    int* toDeleteBufferIndexArr = stackalloc int[2];
                    toDeleteBufferIndexArr[0] = _vertexBufferId;
                    toDeleteBufferIndexArr[1] = _indexBufferId;
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