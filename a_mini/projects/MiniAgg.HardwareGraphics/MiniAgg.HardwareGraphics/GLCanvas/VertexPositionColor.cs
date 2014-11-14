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
    struct VertexC4ubV3f
    {
       
        public uint color; 
        public Vector3 Position;

        public static int SizeInBytes = 16;
        
        public VertexC4ubV3f(uint color, int x, int y)
        {
            this.Position = new Vector3(x, y, 0);
            //this.R = 0;
            //this.G = 0;
            //this.B = 0;
            //this.A = 0;
            this.color = color;
        }
    }
   
    /// <summary>
    /// vertex buffer object
    /// </summary>
    public struct Vbo { public int VboID, EboID, NumElements; }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex3dPositionColor
    {
        public Vector3 Position;
        public uint Color;
        public Vertex3dPositionColor(float x, float y, float z, Color color)
        {
            Position = new Vector3(x, y, z);
            Color = ToRgba(color);
        }
        static uint ToRgba(Color color)
        {
            return (uint)color.A << 24 | (uint)color.B << 16 | (uint)color.G << 8 | (uint)color.R;
        }
    }
}