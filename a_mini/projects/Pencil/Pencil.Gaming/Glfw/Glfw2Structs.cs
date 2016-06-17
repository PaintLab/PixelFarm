#region License
// Copyright (c) 2013 Antonie Blom
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
#endregion

#if USE_GLFW2
using System;
using System.Runtime.InteropServices;

namespace Pencil.Gaming {
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct GlfwVidMode {
		[FieldOffsetAttribute(0)]
		public int
			Width;
		[FieldOffsetAttribute(4)]
		public int
			Height;
		[FieldOffsetAttribute(8)]
		public int
			RedBits;
		[FieldOffsetAttribute(12)]
		public int
			BlueBits;
		[FieldOffsetAttribute(16)]
		public int
			GreenBits;
	}

	// Can't specify size, don't know whether I'm using 32 or 64 bit pointers.
	[StructLayout(LayoutKind.Explicit)]
	public struct GlfwImage {
		[FieldOffsetAttribute(0)]
		public int
			Width;
		[FieldOffsetAttribute(4)]
		public int
			Height;
		[FieldOffsetAttribute(8)]
		public int
			Format;
		[FieldOffsetAttribute(12)]
		public int
			BytesPerPixel;
		[FieldOffsetAttribute(14)]
		public IntPtr
			Data;
	}
}

#endif