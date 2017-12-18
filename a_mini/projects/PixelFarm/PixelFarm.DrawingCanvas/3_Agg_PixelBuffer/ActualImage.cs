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
using PixelFarm.Drawing;

namespace PixelFarm.Agg
{
    /// <summary>
    /// agg buffer's pixel format
    /// </summary>
    public enum PixelFormat
    {
        ARGB32,
        RGB24,
        GrayScale8,
    }

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
        public bool IsBigEndian { get; set; }



        public static byte[] GetBuffer(ActualImage img)
        {
            return img.pixelBuffer;
        }
        public static int[] CopyImgBuffer(ActualImage img)
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
        public static ActualImage CreateFromBuffer(int width, int height, PixelFormat format, byte[] buffer)
        {
            if (format != PixelFormat.ARGB32 && format != PixelFormat.RGB24)
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

        public override void RequestInternalBuffer(ref ImgBufferRequestArgs buffRequest)
        {
            if (pixelFormat != PixelFormat.ARGB32)
            {
                throw new NotSupportedException();
            }
            byte[] newBuff = new byte[this.pixelBuffer.Length];
            Buffer.BlockCopy(this.pixelBuffer, 0, newBuff, 0, newBuff.Length);
            buffRequest.OutputBuffer = newBuff;
        }



        public static int CalculateStride(int width, PixelFormat format, out int bitDepth, out int bytesPerPixel)
        {
            //stride calcuation helper

            switch (format)
            {
                case PixelFormat.ARGB32:
                    {
                        bitDepth = 32;
                        bytesPerPixel = (bitDepth + 7) / 8;
                        return width * (32 / 8);
                    }
                case PixelFormat.GrayScale8:
                    {
                        bitDepth = 8; //bit per pixel
                        bytesPerPixel = (bitDepth + 7) / 8;
                        return 4 * ((width * bytesPerPixel + 3) / 4);
                    }
                case PixelFormat.RGB24:
                    {
                        bitDepth = 24; //bit per pixel
                        bytesPerPixel = (bitDepth + 7) / 8;
                        return 4 * ((width * bytesPerPixel + 3) / 4);
                    }
                default:
                    throw new NotSupportedException();
            }
        }



        //class MyBitmapData : PixelFarm.Drawing.BitmapData, IDisposable
        //{
        //    System.Runtime.InteropServices.GCHandle _gcHandle;
        //    byte[] pixelBuffer;
        //    unsafe byte* arrayHeader;
        //    public MyBitmapData(byte[] pixelBuffer)
        //    {
        //        this.pixelBuffer = pixelBuffer;
        //        _gcHandle = System.Runtime.InteropServices.GCHandle.Alloc(this.pixelBuffer);
        //        unsafe
        //        {
        //            fixed (byte* h = &pixelBuffer[0])
        //            {
        //                this.arrayHeader = h;
        //            }
        //        }
        //    }
        //    public void Dispose()
        //    {
        //        _gcHandle.Free();
        //        pixelBuffer = null;
        //    }
        //    public override IntPtr Scan0
        //    {
        //        get
        //        {
        //            return _gcHandle.AddrOfPinnedObject();
        //        }
        //    }
        //}

    }
}