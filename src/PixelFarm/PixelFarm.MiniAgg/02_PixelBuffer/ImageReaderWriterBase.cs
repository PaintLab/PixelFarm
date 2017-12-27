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
using PixelFarm.Agg.Imaging;
namespace PixelFarm.Agg
{
    /// <summary>
    /// base class for access(read/write) pixel buffer
    /// </summary>
    public abstract class ImageReaderWriterBase : IImageReaderWriter
    {
        const int BASE_MASK = 255;
        //--------------------------------------------
        //look up table , for random access to specific point of image buffer
        //we pre-calculate the offset of each row and column
        int[] yTableArray;
        int[] xTableArray;
        //--------------------------------------------
        //byte[] m_ByteBuffer;
        int[] raw_buffer32;
        //--------------------------------------------
        // Pointer to first pixel depending on strideInBytes and image position         
        protected int int32ArrayStartPixelAt;
        int width;  // in pixels
        int height; // in pixels
        int strideInBytes; // Number of bytes per row,  Can be < 0
        int m_DistanceInBytesBetweenPixelsInclusive;
        int bitDepth;

        ///// <summary>
        ///// blender for destination image buffer, in this version we support 32 bits ARGB and 8 bits gray-scale
        ///// </summary>
        //IPixelBlender recieveBlender;//blender to the target surface
        PixelBlenderBGRA _recvBlender32;
        //-------------------------------------------- 
        public byte[] GetBuffer()
        {
            throw new NotSupportedException();
        }
        public int[] GetBuffer32()
        {
            return this.raw_buffer32;
        }

        public TempMemPtr GetBufferPtr()
        {
            return new TempMemPtr(raw_buffer32);
        }

        protected void SetBufferToNull()
        {
            this.raw_buffer32 = null;
        }
        protected void SetBuffer(int[] byteBuffer)
        {
            raw_buffer32 = byteBuffer;
        }


        public abstract void ReplaceBuffer(int[] newbuffer);

        /// <summary>
        /// attach image buffer and its information to the reader
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bitsPerPixel"></param>
        /// <param name="imgbuffer"></param>
        /// <param name="recieveBlender"></param>
        protected void Attach(int width, int height, int bitsPerPixel, int[] imgbuffer, IPixelBlender recieveBlender)
        {


            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("You must have a width and height > than 0.");
            }
            if (bitsPerPixel != 32 && bitsPerPixel != 24 && bitsPerPixel != 8)
            {
                throw new Exception("Unsupported bits per pixel.");
            }
            //
            //
            int bytesPerPixel = (bitsPerPixel + 7) / 8;
            int stride = 4 * ((width * bytesPerPixel + 3) / 4);

#if DEBUG
            if (bytesPerPixel == 0)
            {

            }
#endif            
            //
            SetDimmensionAndFormat(width, height, stride, bitsPerPixel, bitsPerPixel / 8);
            SetUpLookupTables();
            //
            SetRecieveBlender(recieveBlender);
            //
            //this.m_ByteBuffer = imgbuffer;
            this.raw_buffer32 = imgbuffer;
        }


        protected void SetDimmensionAndFormat(int width, int height,
           int strideInBytes,
           int bitDepth,
           int distanceInBytesBetweenPixelsInclusive)
        {
            this.width = width;
            this.height = height;
            this.strideInBytes = strideInBytes;
            this.bitDepth = bitDepth;

            if (distanceInBytesBetweenPixelsInclusive > 4)
            {
                throw new System.Exception("It looks like you are passing bits per pixel rather than distance in bytes.");
            }
            if (distanceInBytesBetweenPixelsInclusive < (bitDepth / 8))
            {
                throw new Exception("You do not have enough room between pixels to support your bit depth.");
            }
            m_DistanceInBytesBetweenPixelsInclusive = distanceInBytesBetweenPixelsInclusive;
            if (strideInBytes < distanceInBytesBetweenPixelsInclusive * width)
            {
                throw new Exception("You do not have enough strideInBytes to hold the width and pixel distance you have described.");
            }
        }

        void CopyFromNoClipping(IImageReaderWriter sourceImage, RectInt clippedSourceImageRect, int destXOffset, int destYOffset)
        {
            if (BytesBetweenPixelsInclusive != BitDepth / 8
                || sourceImage.BytesBetweenPixelsInclusive != sourceImage.BitDepth / 8)
            {
                throw new Exception("WIP we only support packed pixel formats at this time.");
            }

            if (BitDepth == sourceImage.BitDepth)
            {
                int lengthInBytes = clippedSourceImageRect.Width * BytesBetweenPixelsInclusive;
                int sourceOffset = sourceImage.GetBufferOffsetXY(clippedSourceImageRect.Left, clippedSourceImageRect.Bottom);
                byte[] sourceBuffer = sourceImage.GetBuffer();
                byte[] destBuffer = GetBuffer();
                int destOffset = GetBufferOffsetXY(clippedSourceImageRect.Left + destXOffset, clippedSourceImageRect.Bottom + destYOffset);
                for (int i = 0; i < clippedSourceImageRect.Height; i++)
                {
                    AggMemMx.memmove(destBuffer, destOffset, sourceBuffer, sourceOffset, lengthInBytes);
                    sourceOffset += sourceImage.Stride;
                    destOffset += Stride;
                }
            }
            else
            {
                bool haveConversion = true;
                switch (sourceImage.BitDepth)
                {
                    case 24:
                        switch (BitDepth)
                        {
                            case 32:
                                {
                                    //TODO: review here, this may not correct
                                    int numPixelsToCopy = clippedSourceImageRect.Width;
                                    for (int i = clippedSourceImageRect.Bottom; i < clippedSourceImageRect.Top; i++)
                                    {
                                        int sourceOffset = sourceImage.GetBufferOffsetXY(clippedSourceImageRect.Left, clippedSourceImageRect.Bottom + i);

                                        //byte[] sourceBuffer = sourceImage.GetBuffer();
                                        //byte[] destBuffer = GetBuffer();

                                        TempMemPtr srcMemPtr = sourceImage.GetBufferPtr();
                                        TempMemPtr destBufferPtr = this.GetBufferPtr();

                                        int destOffset = GetBufferOffsetXY(
                                            clippedSourceImageRect.Left + destXOffset,
                                            clippedSourceImageRect.Bottom + i + destYOffset);
                                        unsafe
                                        {
                                            byte* destBuffer = (byte*)destBufferPtr.Ptr;
                                            byte* sourceBuffer = (byte*)srcMemPtr.Ptr;
                                            for (int x = 0; x < numPixelsToCopy; x++)
                                            {
                                                destBuffer[destOffset++] = sourceBuffer[sourceOffset++];
                                                destBuffer[destOffset++] = sourceBuffer[sourceOffset++];
                                                destBuffer[destOffset++] = sourceBuffer[sourceOffset++];
                                                destBuffer[destOffset++] = 255;
                                            }
                                        }


                                        srcMemPtr.Release();
                                        destBufferPtr.Release();
                                    }
                                }
                                break;
                            default:
                                haveConversion = false;
                                break;
                        }
                        break;
                    default:
                        haveConversion = false;
                        break;
                }

                if (!haveConversion)
                {
                    throw new NotImplementedException("You need to write the " + sourceImage.BitDepth.ToString() + " to " + BitDepth.ToString() + " conversion");
                }
            }
        }

        public void CopyFrom(IImageReaderWriter sourceImage, RectInt sourceImageRect, int destXOffset, int destYOffset)
        {
            RectInt sourceImageBounds = sourceImage.GetBounds();
            RectInt clippedSourceImageRect = new RectInt();
            if (clippedSourceImageRect.IntersectRectangles(sourceImageRect, sourceImageBounds))
            {
                RectInt destImageRect = clippedSourceImageRect;
                destImageRect.Offset(destXOffset, destYOffset);
                RectInt destImageBounds = GetBounds();
                RectInt clippedDestImageRect = new RectInt();
                if (clippedDestImageRect.IntersectRectangles(destImageRect, destImageBounds))
                {
                    // we need to make sure the source is also clipped to the dest. So, we'll copy this back to source and offset it.
                    clippedSourceImageRect = clippedDestImageRect;
                    clippedSourceImageRect.Offset(-destXOffset, -destYOffset);
                    CopyFromNoClipping(sourceImage, clippedSourceImageRect, destXOffset, destYOffset);
                }
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int Stride { get { return strideInBytes; } }

        public int BytesBetweenPixelsInclusive
        {
            get { return m_DistanceInBytesBetweenPixelsInclusive; }
        }
        public int BitDepth
        {
            get { return bitDepth; }
        }

        public RectInt GetBounds()
        {
            return new RectInt(0, 0, this.width, this.height);
        }

        /// <summary>
        /// get blender of destination image buffer
        /// </summary>
        /// <returns></returns>
        public IPixelBlender GetRecieveBlender()
        {
            return _recvBlender32;
        }
        /// <summary>
        /// set pixel blender for destination image buffer
        /// </summary>
        /// <param name="value"></param>
        public void SetRecieveBlender(IPixelBlender value)
        {
            if (BitDepth != 0 && value != null && value.NumPixelBits != BitDepth)
            {
                throw new NotSupportedException("The blender has to support the bit depth of this image.");
            }
            _recvBlender32 = (PixelBlenderBGRA)value;
        }

        protected void SetUpLookupTables()
        {
            yTableArray = new int[height];
            xTableArray = new int[width];
            unsafe
            {
                fixed (int* first = &yTableArray[0])
                {
                    //go last
                    int* cur = first + height - 1;
                    for (int i = height - 1; i >= 0;)
                    {
                        //--------------------
                        //*cur = i * strideInBytes;
                        *cur = i * width;
                        --i;
                        cur--;
                        //--------------------
                    }
                }
                fixed (int* first = &xTableArray[0])
                {
                    //go last
                    int* cur = first + width - 1;
                    //even
                    for (int i = width - 1; i >= 0;)
                    {
                        //--------------------
                        //*cur = i * m_DistanceInBytesBetweenPixelsInclusive;
                        *cur = i * 1;
                        --i;
                        cur--;
                        //--------------------
                    }
                }
            }


            if (yTableArray.Length != height
              || xTableArray.Length != width)
            {
                // LBB, don't fix this if you don't understand what it's trying to do.
                throw new Exception("The yTable and xTable should be allocated correctly at this point. Figure out what happend.");
            }

        }







        public static void CopySubBufferToInt32Array(ImageReaderWriterBase buff, int mx, int my, int w, int h, int[] buffer)
        {
            //TODO: review here, 
            //check pixel format for an image buffer before use
            //if mBuffer is not 32 bits ARGB => this may not correct

            int i = 0;
            int[] mBuffer = buff.raw_buffer32;
            for (int y = my; y < h; ++y)
            {
                //int xbufferOffset = buff.GetBufferOffsetXY(0, y);
                int xbuffOffset32 = buff.GetBufferOffsetXY32(0, y);

                for (int x = mx; x < w; ++x)
                {
                    //A R G B

                    int val = mBuffer[xbuffOffset32];

                    byte r = (byte)((val >> 16) & 0xff);// mBuffer[xbufferOffset + 2];
                    byte g = (byte)((val >> 8) & 0xff);// mBuffer[xbufferOffset + 1];
                    byte b = (byte)((val >> 0) & 0xff);// mBuffer[xbufferOffset];


                    //xbufferOffset += 4;
                    xbuffOffset32++;
                    //
                    buffer[i] = b | (g << 8) | (r << 16);
                    i++;
                }
            }
            //int i = 0;
            //byte[] mBuffer = buff.m_ByteBuffer;
            //for (int y = my; y < h; ++y)
            //{
            //    int xbufferOffset = buff.GetBufferOffsetXY(0, y);
            //    for (int x = mx; x < w; ++x)
            //    {
            //        //A R G B
            //        byte r = mBuffer[xbufferOffset + 2];
            //        byte g = mBuffer[xbufferOffset + 1];
            //        byte b = mBuffer[xbufferOffset];
            //        xbufferOffset += 4;
            //        //
            //        buffer[i] = b | (g << 8) | (r << 16);
            //        i++;
            //    }
            //}
        }
        public Color GetPixel(int x, int y)
        {
            return _recvBlender32.PixelToColorRGBA(raw_buffer32, GetBufferOffsetXY(x, y) / 4);
        }
        public int GetBufferOffsetXY(int x, int y)
        {
            return int32ArrayStartPixelAt + yTableArray[y] + xTableArray[x];
        }
        public int GetBufferOffsetXY32(int x, int y)
        {
            return (int32ArrayStartPixelAt + yTableArray[y] + xTableArray[x])  ;
        }
        public void SetPixel(int x, int y, Color color)
        {
            _recvBlender32.CopyPixel(raw_buffer32, GetBufferOffsetXY32(x, y), color);
        }

        public void CopyHL(int x, int y, int len, Color sourceColor)
        {
            _recvBlender32.CopyPixels(this.raw_buffer32, GetBufferOffsetXY32(x, y), sourceColor, len);
        }

        public void CopyVL(int x, int y, int len, Color sourceColor)
        {
            throw new NotImplementedException();
#if false
            int scanWidth = StrideInBytes();
            byte* pDestBuffer = GetPixelPointerXY(x, y);
            do
            {
                m_Blender.CopyPixel(pDestBuffer, sourceColor);
                pDestBuffer = &pDestBuffer[scanWidth];
            }
            while (--len != 0);
#endif
        }
        public void BlendHL(int x1, int y, int x2, Color sourceColor, byte cover)
        {
            if (sourceColor.A == 0) { return; }
            //-------------------------------------------------

            int len = x2 - x1 + 1;


            int[] buffer = this.GetBuffer32();
            int bufferOffset = GetBufferOffsetXY32(x1, y);
            int alpha = (((int)(sourceColor.A) * (cover + 1)) >> 8);
            if (alpha == BASE_MASK)
            {
                //full
                _recvBlender32.CopyPixels(buffer, bufferOffset, sourceColor, len);
            }
            else
            {
                Color c2 = Color.FromArgb(alpha, sourceColor);
                do
                {
                    //copy pixel-by-pixel
                    _recvBlender32.BlendPixel32(buffer, bufferOffset, c2);
                    //bufferOffset += m_DistanceInBytesBetweenPixelsInclusive;
                    bufferOffset++;
                }
                while (--len != 0);
            }


            //byte[] buffer = GetBuffer();
            //int bufferOffset = GetBufferOffsetXY(x1, y);
            //int alpha = (((int)(sourceColor.A) * (cover + 1)) >> 8);
            //if (alpha == BASE_MASK)
            //{
            //    //full
            //    _recvBlender32.CopyPixels(buffer, bufferOffset, sourceColor, len);
            //}
            //else
            //{
            //    Color c2 = Color.FromArgb(alpha, sourceColor);
            //    do
            //    {
            //        //copy pixel-by-pixel
            //        _recvBlender32.BlendPixel(buffer, bufferOffset, c2);
            //        bufferOffset += m_DistanceInBytesBetweenPixelsInclusive;
            //    }
            //    while (--len != 0);
            //}


        }

        public void BlendVL(int x, int y1, int y2, Color sourceColor, byte cover)
        {
            throw new NotImplementedException();
#if false
            int ScanWidth = StrideInBytes();
            if (sourceColor.m_A != 0)
            {
                unsafe
                {
                    int len = y2 - y1 + 1;
                    byte* p = GetPixelPointerXY(x, y1);
                    sourceColor.m_A = (byte)(((int)(sourceColor.m_A) * (cover + 1)) >> 8);
                    if (sourceColor.m_A == base_mask)
                    {
                        byte cr = sourceColor.m_R;
                        byte cg = sourceColor.m_G;
                        byte cb = sourceColor.m_B;
                        do
                        {
                            m_Blender.CopyPixel(p, sourceColor);
                            p = &p[ScanWidth];
                        }
                        while (--len != 0);
                    }
                    else
                    {
                        if (cover == 255)
                        {
                            do
                            {
                                m_Blender.BlendPixel(p, sourceColor);
                                p = &p[ScanWidth];
                            }
                            while (--len != 0);
                        }
                        else
                        {
                            do
                            {
                                m_Blender.BlendPixel(p, sourceColor);
                                p = &p[ScanWidth];
                            }
                            while (--len != 0);
                        }
                    }
                }
            }
#endif
        }



        public void BlendSolidHSpan(int x, int y, int len, Color sourceColor, byte[] covers, int coversIndex)
        {
            int colorAlpha = sourceColor.alpha;
            if (colorAlpha != 0)
            {
                //byte[] buffer = GetBuffer();
                int[] buffer = this.GetInt32Buffer();
                int bufferOffset32 = GetBufferOffsetXY32(x, y);
                do
                {
                    int alpha = ((colorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                    if (alpha == BASE_MASK)
                    {
                        _recvBlender32.CopyPixel(buffer, bufferOffset32, sourceColor);
                    }
                    else
                    {
                        _recvBlender32.BlendPixel32(buffer, bufferOffset32, Color.FromArgb(alpha, sourceColor));
                    }
                    //bufferOffset += m_DistanceInBytesBetweenPixelsInclusive;
                    bufferOffset32++;
                    coversIndex++;
                }
                while (--len != 0);

                //bufferPtr.Release();
            }
        }

        public void BlendSolidVSpan(int x, int y, int len, Color sourceColor, byte[] covers, int coversIndex)
        {
            if (sourceColor.A != 0)
            {
                int scanWidthBytes = Stride;
                unchecked
                {
                    //int bufferOffset = GetBufferOffsetXY(x, y);
                    int bufferOffset32 = GetBufferOffsetXY(x, y) / 4;
                    //do
                    //{
                    //    //byte oldAlpha = sourceColor.A;
                    //    ////TODO:review here, sourceColor mat not changed
                    //    //sourceColor.alpha = (byte)(((int)(sourceColor.A) * ((int)(covers[coversIndex++]) + 1)) >> 8); 

                    //    //TODO: review here again
                    //    Color newcolor = sourceColor.NewFromChangeCoverage(covers[coversIndex++]);
                    //    if (newcolor.alpha == BASE_MASK)
                    //    {
                    //        _recvBlender32.CopyPixel(m_ByteBuffer, bufferOffset, newcolor);
                    //    }
                    //    else
                    //    {
                    //        _recvBlender32.BlendPixel(m_ByteBuffer, bufferOffset, newcolor);
                    //    }
                    //    bufferOffset += scanWidthBytes;
                    //    //sourceColor.alpha = oldAlpha;
                    //}
                    //while (--len != 0); 
                    //
                    int actualW = scanWidthBytes / 4;
                    do
                    {
                        //byte oldAlpha = sourceColor.A;
                        ////TODO:review here, sourceColor mat not changed
                        //sourceColor.alpha = (byte)(((int)(sourceColor.A) * ((int)(covers[coversIndex++]) + 1)) >> 8); 

                        //TODO: review here again
                        Color newcolor = sourceColor.NewFromChangeCoverage(covers[coversIndex++]);
                        if (newcolor.alpha == BASE_MASK)
                        {
                            _recvBlender32.CopyPixel(raw_buffer32, bufferOffset32, newcolor);
                        }
                        else
                        {

                            _recvBlender32.BlendPixel32(raw_buffer32, bufferOffset32, newcolor);
                        }
                        bufferOffset32 += scanWidthBytes;
                        //sourceColor.alpha = oldAlpha;
                    }
                    while (--len != 0);
                }
            }
        }

        public void CopyColorHSpan(int x, int y, int len, Color[] colors, int colorsIndex)
        {
            int bufferOffset32 = GetBufferOffsetXY(x, y) / 4;
            do
            {
                _recvBlender32.CopyPixel(raw_buffer32, bufferOffset32, colors[colorsIndex]);
                ++colorsIndex;
                //bufferOffset += m_DistanceInBytesBetweenPixelsInclusive;
                bufferOffset32++;
            }
            while (--len != 0);
        }

        public void CopyColorVSpan(int x, int y, int len, Color[] colors, int colorsIndex)
        {
            //int bufferOffset = GetBufferOffsetXY(x, y);
            //do
            //{
            //    _recvBlender32.CopyPixel(m_ByteBuffer, bufferOffset, colors[colorsIndex]);
            //    ++colorsIndex;
            //    bufferOffset += strideInBytes;
            //}
            //while (--len != 0);
            //
            int bufferOffset32 = GetBufferOffsetXY(x, y) / 4;
            int actualW = strideInBytes / 4;
            do
            {
                _recvBlender32.CopyPixel(raw_buffer32, bufferOffset32, colors[colorsIndex]);
                ++colorsIndex;
                bufferOffset32 += actualW;
            }
            while (--len != 0);
        }

        public void BlendColorHSpan(int x, int y, int len, Color[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll)
        {
            //int bufferOffset = GetBufferOffsetXY(x, y);
            int bufferOffset32 = GetBufferOffsetXY(x, y) / 4;
            //_recvBlender32.BlendPixels(m_ByteBuffer, bufferOffset, colors, colorsIndex, covers, coversIndex, firstCoverForAll, len);
            _recvBlender32.BlendPixels(raw_buffer32, bufferOffset32, colors, colorsIndex, covers, coversIndex, firstCoverForAll, len);
        }

        public void BlendColorVSpan(int x, int y, int len, Color[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll)
        {
            //int bufferOffset = GetBufferOffsetXY(x, y);
            int bufferOffset32 = GetBufferOffsetXY(x, y) / 4;
            int scanWidthBytes = System.Math.Abs(Stride);
            if (!firstCoverForAll)
            {
                unsafe
                {
                    fixed (int* head = &raw_buffer32[bufferOffset32])
                    {
                        int* hptr = (int*)head;
                        int actualWidth = scanWidthBytes / 4;
                        do
                        {
                            CopyOrBlend32_BasedOnAlphaAndCover(_recvBlender32, hptr, bufferOffset32, colors[colorsIndex], covers[coversIndex++]);

                            //bufferOffset += actualWidth;
                            bufferOffset32++;
                            ++colorsIndex;
                        }
                        while (--len != 0);
                    }
                }

            }
            else
            {
                if (covers[coversIndex] == 255)
                {
                    unsafe
                    {
                        fixed (int* destH = &raw_buffer32[0])
                        {
                            int* destBuffer = (int*)destH;
                            int actualWidth = scanWidthBytes / 4;

                            do
                            {
                                PixelBlenderBGRA.Blend32PixelInternal(destBuffer, colors[colorsIndex]);
                                //CopyOrBlend32_BasedOnAlpha(_recvBlender32, m_ByteBuffer, bufferOffset, colors[colorsIndex]);
                                //bufferOffset += scanWidthBytes;
                                ++colorsIndex;
                                destBuffer += actualWidth;
                            }
                            while (--len != 0);
                        }
                    }
                }
                else
                {
                    unsafe
                    {
                        fixed (int* byteptr = &raw_buffer32[0])
                        {
                            int* head = (int*)byteptr;
                            int actualWidth = scanWidthBytes / 4;
                            do
                            {
                                CopyOrBlend32_BasedOnAlphaAndCover(_recvBlender32, head, bufferOffset32, colors[colorsIndex], covers[coversIndex]);
                                // bufferOffset += actualWidth;
                                ++colorsIndex;
                            }
                            while (--len != 0);
                        }
                    }

                }
            }
        }
        public RectInt GetBoundingRect()
        {
            return new RectInt(0, 0, Width, Height);
        }

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugGetNewDebugId();
        static int dbugGetNewDebugId()
        {
            return dbugTotalId++;
        }
#endif

        //        static unsafe void CopyOrBlend32_BasedOnAlpha(PixelBlenderBGRA recieveBlender,
        //            int* destBuffer,
        //            int arrayOffset,
        //            Color sourceColor)
        //        {
        //            //if (sourceColor.m_A != 0)
        //            {
        //#if false // we blend regardless of the alpha so that we can get Light Opacity working (used this way we have addative and faster blending in one blender) LBB
        //                if (sourceColor.m_A == base_mask)
        //                {
        //                    Blender.CopyPixel(pDestBuffer, sourceColor);
        //                }
        //                else
        //#endif
        //                {
        //                    PixelBlenderBGRA.Blend32PixelInternal(destBuffer + arrayOffset, sourceColor);
        //                }
        //            }
        //        }


        //        static void CopyOrBlend_BasedOnAlpha(IPixelBlender recieveBlender,
        //        byte[] destBuffer,
        //        int bufferOffset,
        //        Color sourceColor)
        //        {
        //            //if (sourceColor.m_A != 0)
        //            {
        //#if false // we blend regardless of the alpha so that we can get Light Opacity working (used this way we have addative and faster blending in one blender) LBB
        //                if (sourceColor.m_A == base_mask)
        //                {
        //                    Blender.CopyPixel(pDestBuffer, sourceColor);
        //                }
        //                else
        //#endif
        //                {
        //                    recieveBlender.BlendPixel(destBuffer, bufferOffset, sourceColor);
        //                }
        //            }
        //        }

        //        static unsafe void CopyOrBlend32_BasedOnAlphaAndCover(IPixelBlender recieveBlender, int[] destBuffer, int arrayElemOffset, Color sourceColor, int cover)
        //        {
        //            if (cover == 255)
        //            {
        //                //CopyOrBlend_BasedOnAlpha(recieveBlender, destBuffer, bufferOffset, sourceColor);

        //                fixed (int* dest = &destBuffer[arrayElemOffset])
        //                {
        //                    recieveBlender.BlendPixel(destBuffer, bufferOffset, sourceColor);
        //                }

        //            }
        //            else
        //            {
        //                //if (sourceColor.m_A != 0)
        //                {

        //#if false // we blend regardless of the alpha so that we can get Light Opacity working (used this way we have addative and faster blending in one blender) LBB
        //                    if (sourceColor.m_A == base_mask)
        //                    {
        //                        Blender.CopyPixel(pDestBuffer, sourceColor);
        //                    }
        //                    else
        //#endif
        //                    {
        //                        recieveBlender.BlendPixel(destBuffer, bufferOffset, sourceColor.NewFromChangeCoverage(cover));
        //                    }
        //                }
        //            }
        //        }
        static unsafe void CopyOrBlend32_BasedOnAlphaAndCover(PixelBlenderBGRA recieveBlender, int* destBuffer, int destArrayOffset, Color sourceColor, int cover)
        {
            if (cover == 255)
            {
                PixelBlenderBGRA.Blend32PixelInternal(destBuffer + destArrayOffset, sourceColor);
                //CopyOrBlend32_BasedOnAlpha(recieveBlender, destBuffer, destArrayOffset, sourceColor);
            }
            else
            {
                //if (sourceColor.m_A != 0)
                {

#if false // we blend regardless of the alpha so that we can get Light Opacity working (used this way we have addative and faster blending in one blender) LBB
                    if (sourceColor.m_A == base_mask)
                    {
                        Blender.CopyPixel(pDestBuffer, sourceColor);
                    }
                    else
#endif


                    PixelBlenderBGRA.Blend32PixelInternal(destBuffer + destArrayOffset, sourceColor.NewFromChangeCoverage(cover));
                    //recieveBlender.BlendPixel(destBuffer, destArrayOffset, sourceColor.NewFromChangeCoverage(cover));


                }
            }
        }
        static unsafe void CopyOrBlend32_BasedOnAlphaAndCover(PixelBlenderBGRA recieveBlender, int[] destBuffer, int destArrayOffset, Color sourceColor, int cover)
        {
            if (cover == 255)
            {
                fixed (int* destAdd = &destBuffer[destArrayOffset])
                {
                    PixelBlenderBGRA.Blend32PixelInternal(destAdd, sourceColor);
                    //CopyOrBlend32_BasedOnAlpha(recieveBlender, destBuffer, destArrayOffset, sourceColor);
                }

            }
            else
            {
                //if (sourceColor.m_A != 0)
                {

#if false // we blend regardless of the alpha so that we can get Light Opacity working (used this way we have addative and faster blending in one blender) LBB
                    if (sourceColor.m_A == base_mask)
                    {
                        Blender.CopyPixel(pDestBuffer, sourceColor);
                    }
                    else
#endif

                    fixed (int* destAdd = &destBuffer[destArrayOffset])
                    {
                        PixelBlenderBGRA.Blend32PixelInternal(destAdd, sourceColor.NewFromChangeCoverage(cover));
                        //recieveBlender.BlendPixel(destBuffer, destArrayOffset, sourceColor.NewFromChangeCoverage(cover));

                    }
                }
            }
        }

        public int[] GetInt32Buffer()
        {
            return this.raw_buffer32;
            //throw new NotImplementedException();
        }


        //public void apply_gamma_inv(GammaLookUpTable g)
        //{
        //    throw new System.NotImplementedException();
        //    //for_each_pixel(apply_gamma_inv_rgba<color_type, order_type, GammaLut>(g));
        //}

        //public bool IsPixelVisible(int x, int y)
        //{
        //    ColorRGBA pixelValue = GetRecieveBlender().PixelToColorRGBA_Bytes(m_ByteBuffer, GetBufferOffsetXY(x, y));
        //    return (pixelValue.Alpha0To255 != 0 || pixelValue.Red0To255 != 0 || pixelValue.Green0To255 != 0 || pixelValue.Blue0To255 != 0);
        //}


        //public override int GetHashCode()
        //{
        //    // This might be hard to make fast and usefull.
        //    return m_ByteBuffer.GetHashCode() ^ bufferOffset.GetHashCode() ^ bufferFirstPixel.GetHashCode();
        //}
        //public byte[] GetPixelPointerY(int y, out int bufferOffset)
        //{
        //    bufferOffset = bufferFirstPixel + yTableArray[y];
        //    return m_ByteBuffer;
        //}
    }




    public class MyImageReaderWriter : ImageReaderWriterBase
    {
        ActualImage actualImage;
        IPixelBlender pixelBlenderRGBA;
        PixelBlenderGray pixelBlenderGray;

        public MyImageReaderWriter()
        {
        }
        public override void ReplaceBuffer(int[] newbuffer)
        {
            ActualImage.ReplaceBuffer(actualImage, newbuffer);
        }
        /// <summary>
        /// load image to the reader/writer
        /// </summary>
        /// <param name="actualImage"></param>
        public void ReloadImage(ActualImage actualImage)
        {

            if (this.actualImage == actualImage)
            {
                return;
            }

            //in this version we support actual images
            //in 2 formats : 
            //1. 32 bits ARGB
            //2. 8  bits gray scale
            //

            this.actualImage = actualImage;
            switch (actualImage.PixelFormat)
            {
                case PixelFormat.ARGB32:
                    {
                        Attach(actualImage.Width,
                           actualImage.Height,
                           actualImage.BitDepth,
                           ActualImage.GetBuffer(actualImage),
                           pixelBlenderRGBA ?? (pixelBlenderRGBA = new PixelBlenderBGRA()));
                    }
                    break;
                case PixelFormat.GrayScale8:
                    {
                        Attach(actualImage.Width,
                          actualImage.Height,
                          actualImage.BitDepth,
                          ActualImage.GetBuffer(actualImage),
                          pixelBlenderGray ?? (pixelBlenderGray = new PixelBlenderGray(1)));
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
    }
}