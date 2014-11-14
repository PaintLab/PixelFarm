#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK;

namespace PixelFarm.Agg
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexC4XYZ3I
    {
        public uint color;
        public int x;
        public int y;
        int z;
        public VertexC4XYZ3I(uint color, int x, int y)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            z = 0;

        }
        //--------------------------------------------
        public const int SizeInBytes = 16;
        public const int CoordOffset = sizeof(uint);
        public override string ToString()
        {
            return x + "," + y;
        }
    }
    /// <summary>
    /// vertex buffer object
    /// </summary>
    public struct Vbo
    {
        public int VboID;
        public void Dispose()
        {
            OpenTK.Graphics.OpenGL.GL.DeleteBuffers(1, ref this.VboID);
        }
    }


}