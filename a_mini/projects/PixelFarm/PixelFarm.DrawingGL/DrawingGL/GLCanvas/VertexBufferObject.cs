//MIT, 2014-2017, WinterDev
//https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
//https://www.khronos.org/opengl/wiki/Generic_Vertex_Attribute_-_examples
//https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

using System;
using OpenTK.Graphics.ES20;

namespace PixelFarm.DrawingGL
{

    class VertexBufferObject : IDisposable
    {
        //TODO: consider implement generic version of this*** 
        int _vertextBufferIndex; //index to 'server-side' memory
        int _indexBufferIndex; //index to 'server-side' memory 
        bool _hasData;
        public VertexBufferObject()
        {
            //TODO: review how to create vbo object
        }
        /// <summary>
        /// unload data from server-side mem
        /// </summary>
        public void Clear()
        {
            unsafe
            {
                if (_vertextBufferIndex > 0 || _indexBufferIndex > 0)
                {
                    int* toDeleteBufferIndexArr = stackalloc int[2];
                    toDeleteBufferIndexArr[0] = _vertextBufferIndex;
                    toDeleteBufferIndexArr[1] = _indexBufferIndex;
                    GL.DeleteBuffers(_vertextBufferIndex, toDeleteBufferIndexArr);
                    _vertextBufferIndex = _indexBufferIndex = 0;
                }
                _hasData = false;
            }
        }
        /// <summary>
        /// set up vertex data, we don't store the vertex array,or index array here
        /// </summary>
        public void SetupVertexData(float[] _vertextBuffer, ushort[] _indexBuffer)
        {

            if (_hasData)
            {
                throw new NotSupportedException();
            }

            unsafe
            {
                if (_vertextBuffer != null)
                {
                    //1.
                    GL.GenBuffers(1, out _vertextBufferIndex);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertextBufferIndex);
                    fixed (void* vertDataPtr = &_vertextBuffer[0])
                    {
                        GL.BufferData(BufferTarget.ArrayBuffer,
                            new IntPtr(_vertextBuffer.Length * 4),
                            new IntPtr(vertDataPtr),
                            BufferUsage.StaticDraw);   //this version we use static draw
                    }
                    // IMPORTANT: Unbind from the buffer when we're done with it.
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }
                //----
                //2.
                if (_indexBuffer != null)
                {
                    GL.GenBuffers(1, out _indexBufferIndex);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferIndex);
                    fixed (void* indexDataPtr = &_indexBuffer[0])
                    {
                        GL.BufferData(BufferTarget.ElementArrayBuffer,
                            new IntPtr(_indexBuffer.Length * 4),
                            new IntPtr(indexDataPtr),
                            BufferUsage.StaticDraw);   //this version we use static draw
                    }
                    // IMPORTANT: Unbind from the buffer when we're done with it.
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                }
            }
            _hasData = true;
        }
        public void Dispose()
        {
            Clear();
        }
        public int VertextBufferIndex { get { return _vertextBufferIndex; } }
        public int IndexBufferIndex { get { return _indexBufferIndex; } }
    }
}