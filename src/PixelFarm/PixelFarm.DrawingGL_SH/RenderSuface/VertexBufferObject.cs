﻿//MIT, 2014-present, WinterDev
//https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
//https://www.khronos.org/opengl/wiki/Generic_Vertex_Attribute_-_examples
//https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;
using Tesselate;

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
                if (vertextBuffer.Length == 0)
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
                    fixed (void* vertDataPtr = &vertextBuffer[0])
                    {
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