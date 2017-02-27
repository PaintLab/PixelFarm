//BSD, 2014-2017, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------

using System;
namespace PixelFarm.Drawing.Fonts
{
    public class GlyphImage
    {
        int[] pixelBuffer;
        public GlyphImage(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public RectangleF OriginalGlyphBounds
        {
            get;
            set;
        }
        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }
        public bool IsBigEndian
        {
            get;
            private set;
        }
        public int BorderXY
        {
            get;
            set;
        }
        public int[] GetImageBuffer()
        {
            return pixelBuffer;
        }
        public void SetImageBuffer(int[] pixelBuffer, bool isBigEndian)
        {
            this.pixelBuffer = pixelBuffer;
            this.IsBigEndian = isBigEndian;
        }
    }
}
namespace PixelFarm.Agg
{

    public sealed class ActualImage : PixelFarm.Drawing.Image
    {
        int width;
        int height;
        int stride;
        int bitDepth;
        PixelFormat pixelFormat;
        byte[] pixelBuffer;

        public ActualImage(int width, int height, PixelFormat format)
        {
            //width and height must >0 
            this.width = width;
            this.height = height;
            switch (this.pixelFormat = format)
            {
                case PixelFormat.ARGB32:
                    {
                        this.bitDepth = 32;
                        this.stride = width * (32 / 8);
                        this.pixelBuffer = new byte[stride * height];
                    }
                    break;
                case PixelFormat.GrayScale8:
                    {
                        this.bitDepth = 8; //bit per pixel
                        int bytesPerPixel = (bitDepth + 7) / 8;
                        this.stride = 4 * ((width * bytesPerPixel + 3) / 4);
                        this.pixelBuffer = new byte[stride * height];
                    }
                    break;
                case PixelFormat.RGB24:
                    {
                        this.bitDepth = 24; //bit per pixel
                        int bytesPerPixel = (bitDepth + 7) / 8;
                        this.stride = 4 * ((width * bytesPerPixel + 3) / 4);
                        this.pixelBuffer = new byte[stride * height];
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        private ActualImage() { }

        public override void Dispose()
        {

        }
        public override int Width
        {
            get { return this.width; }
        }
        public override int Height
        {
            get { return this.height; }
        }
        public override int ReferenceX
        {
            get { return 0; }
        }
        public override int ReferenceY
        {
            get { return 0; }
        }
        public RectInt Bounds
        {
            get { return new RectInt(0, 0, this.width, this.height); }
        }
        public override bool IsReferenceImage
        {
            get { return false; }
        }

        public PixelFormat PixelFormat { get { return this.pixelFormat; } }
        public int Stride { get { return this.stride; } }
        public int BitDepth { get { return this.bitDepth; } }


        public static byte[] GetBuffer(ActualImage img)
        {
            return img.pixelBuffer;
        }
        public static int[] GetBuffer2(ActualImage img)
        {

            int[] buff2 = new int[img.width * img.height];
            unsafe
            {
                fixed (byte* header = &img.pixelBuffer[0])
                {
                    System.Runtime.InteropServices.Marshal.Copy((IntPtr)header, buff2, 0, buff2.Length);
                }
            }

            return buff2;
        }
        public static ActualImage CreateFromBuffer(int width, int height, PixelFormat format, int[] buffer)
        {
            if (format != PixelFormat.ARGB32)
            {
                throw new NotSupportedException();
            }
            //
            var img = new ActualImage(width, height, format);
            unsafe
            {
                fixed (byte* header = &img.pixelBuffer[0])
                {
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, (IntPtr)header, buffer.Length);
                }
            }
            return img;
        }

    }
}