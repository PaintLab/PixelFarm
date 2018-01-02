//MIT, 2014-2018, WinterDev
//https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
//https://www.khronos.org/opengl/wiki/Generic_Vertex_Attribute_-_examples
//https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

using System;
using OpenTK.Graphics.ES20;

namespace PixelFarm.DrawingGL
{
    public class VertexBufferObject : IDisposable
    {


        int _vertextBufferIndex; // array buffer
        int _indexBufferIndex; // element buffer
        bool _hasData;

        public VertexBufferObject()
        {
            //TODO: review how to create vbo object

#if DEBUG
            dbugId = dbugTotoalId++;
            Console.WriteLine(dbugId);
#endif
        }
#if DEBUG
        readonly int dbugId = 0;
        static int dbugTotoalId = 0;
#endif
        /// <summary>
        /// set up vertex data, we don't store the vertex array,or index array here
        /// </summary>
        public void CreateBuffers(float[] _vertextBuffer, ushort[] _indexBuffer, PartRange[] multipartIndex)
        {

            if (_hasData)
            {
                throw new NotSupportedException();
            }

            unsafe
            {

                //
                if (_vertextBuffer != null)
                {
                    //1.
                    GL.GenBuffers(1, out _vertextBufferIndex);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertextBufferIndex);
                    fixed (void* vertDataPtr = &_vertextBuffer[0])
                    {
                        GL.BufferData(BufferTarget.ArrayBuffer,
                            new IntPtr(_vertextBuffer.Length * 4), //size in byte
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
        /// bind array buffer and element array buffer
        /// </summary>
        public void Bind()
        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertextBufferIndex);
            if (_indexBufferIndex > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferIndex);
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
        public bool HasData
        {
            get { return _hasData; }
        }
    }

}