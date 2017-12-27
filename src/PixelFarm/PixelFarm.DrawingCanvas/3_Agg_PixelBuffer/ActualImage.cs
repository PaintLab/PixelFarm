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

    public struct TempMemPtr
    {
        int _lenInBytes; //in bytes
        System.Runtime.InteropServices.GCHandle handle1;
        //public TempMemPtr(byte[] buffer)
        //{
        //    handle1 = System.Runtime.InteropServices.GCHandle.Alloc(buffer, System.Runtime.InteropServices.GCHandleType.Pinned);
        //    this._lenInBytes = buffer.Length;
        //}
        public TempMemPtr(int[] buffer) //in element count
        {
            handle1 = System.Runtime.InteropServices.GCHandle.Alloc(buffer, System.Runtime.InteropServices.GCHandleType.Pinned);
            this._lenInBytes = buffer.Length * 4;
        }

        public int LengthInBytes
        {
            get { return _lenInBytes; }
        }

        public IntPtr Ptr
        {
            get
            {
                return handle1.AddrOfPinnedObject();
            }
        }
        public void Release()
        {
            this.handle1.Free();
        }
        public unsafe byte* BytePtr
        {
            get { return (byte*)handle1.AddrOfPinnedObject(); }
        }

    }
    public sealed class ActualImage : PixelFarm.Drawing.Image
    {
        int width;
        int height;
        int stride;
        int bitDepth;
        PixelFormat pixelFormat;
        int[] pixelBuffer;

        public ActualImage(int width, int height, PixelFormat format)
        {
            //width and height must >0 
            this.width = width;
            this.height = height;
            int bytesPerPixel;
            this.stride = CalculateStride(width,
                this.pixelFormat = format,
                out bitDepth,
                out bytesPerPixel);
            //alloc mem

            this.pixelBuffer = new int[width * height];
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


        public static TempMemPtr GetBufferPtr(ActualImage img)
        {
            TempMemPtr tmp = new TempMemPtr(img.pixelBuffer);
            return tmp;
        }

        public static int[] GetBuffer(ActualImage img)
        {
            return img.pixelBuffer;
        }
       
        public static void ReplaceBuffer(ActualImage img, int[] pixelBuffer)
        {
            img.pixelBuffer = pixelBuffer;
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
                fixed (int* header = &img.pixelBuffer[0])
                {
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, (IntPtr)header, buffer.Length);
                }
            }
            return img;
        }
        //public static ActualImage CreateFromBuffer(int width, int height, PixelFormat format, byte[] buffer)
        //{
        //    if (format != PixelFormat.ARGB32 && format != PixelFormat.RGB24)
        //    {
        //        throw new NotSupportedException();
        //    }
        //    //
        //    var img = new ActualImage(width, height, format);
        //    unsafe
        //    {
        //        fixed (byte* header = &img.pixelBuffer[0])
        //        {
        //            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, (IntPtr)header, buffer.Length);
        //        }
        //    }
        //    return img;
        //}

        public override void RequestInternalBuffer(ref ImgBufferRequestArgs buffRequest)
        {
            if (pixelFormat != PixelFormat.ARGB32)
            {
                throw new NotSupportedException();
            }
            int[] newBuff = new int[this.pixelBuffer.Length];
            Buffer.BlockCopy(this.pixelBuffer, 0, newBuff, 0, newBuff.Length);
            buffRequest.OutputBuffer32 = newBuff;
        }


        public static int CalculateStride(int width, PixelFormat format)
        {
            int bitDepth, bytesPerPixel;
            return CalculateStride(width, format, out bitDepth, out bytesPerPixel);
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
        public static int[] CopyImgBuffer(ActualImage img)
        {

            int[] buff2 = new int[img.Width * img.Height];
            unsafe
            {
                //byte[] pixelBuffer = ActualImage.GetBuffer(img);
                TempMemPtr pixBuffer = ActualImage.GetBufferPtr(img);
                //fixed (byte* header = &pixelBuffer[0])
                byte* header = (byte*)pixBuffer.Ptr;
                {
                    System.Runtime.InteropServices.Marshal.Copy((IntPtr)header, buff2, 0, buff2.Length);//length in bytes
                }
                pixBuffer.Release();
            }

            return buff2;
        }

        //

        public static void SaveImgBufferToPngFile(byte[] imgBuffer, int stride, int width, int height, string filename)
        {
            if (s_saveToPngFileDel != null)
            {
                unsafe
                {
                    fixed (byte* head = &imgBuffer[0])
                    {
                        s_saveToPngFileDel((IntPtr)head, stride, width, height, filename);
                    }
                }

            }
        }
        public static void SaveImgBufferToPngFile(int[] imgBuffer, int stride, int width, int height, string filename)
        {
            if (s_saveToPngFileDel != null)
            {
                unsafe
                {
                    fixed (int* head = &imgBuffer[0])
                    {
                        s_saveToPngFileDel((IntPtr)head, stride, width, height, filename);
                    }
                }
            }
        }
        static SaveToPngFileDelegate s_saveToPngFileDel;
        public delegate void SaveToPngFileDelegate(IntPtr imgBuffer, int stride, int width, int height, string filename);

        public static bool HasDefaultSavePngToFileDelegate()
        {
            return s_saveToPngFileDel != null;
        }


        public static void InstallImageSaveToFileService(SaveToPngFileDelegate saveToPngFileDelegate)
        {
            s_saveToPngFileDel = saveToPngFileDelegate;
        }
#if DEBUG

        public void dbugSaveToPngFile(string filename)
        {
            SaveImgBufferToPngFile(this.pixelBuffer, this.stride, this.width, this.height, filename);
        }

#endif
    }


    public static class ActualImageExtensions
    {



        public static int[] CopyImgBuffer(ActualImage img, int width)
        {
            //calculate stride for the width

            int destStride = ActualImage.CalculateStride(width, PixelFormat.ARGB32);
            int h = img.Height;
            int newBmpW = destStride / 4;

            int[] buff2 = new int[newBmpW * img.Height];
            unsafe
            {

                TempMemPtr srcBufferPtr = ActualImage.GetBufferPtr(img);
                byte* srcBuffer = (byte*)srcBufferPtr.Ptr;
                int srcIndex = 0;
                int srcStride = img.Stride;
                fixed (int* destHead = &buff2[0])
                {
                    byte* destHead2 = (byte*)destHead;
                    for (int line = 0; line < h; ++line)
                    {
                        //System.Runtime.InteropServices.Marshal.Copy(srcBuffer, srcIndex, (IntPtr)destHead2, destStride);
                        NaitveMemMx.memcpy((byte*)destHead2, srcBuffer + srcIndex, destStride);
                        srcIndex += srcStride;
                        destHead2 += destStride;
                    }
                }
                srcBufferPtr.Release();
            }

            return buff2;
        }
    }
}