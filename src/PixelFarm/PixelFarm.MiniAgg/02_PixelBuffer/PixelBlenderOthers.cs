//BSD, 2014-2018, WinterDev
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
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------

using System;
using PixelFarm.Drawing;
namespace PixelFarm.Agg.Imaging
{



    /// <summary>
    /// change destination alpha change with red color from source
    /// </summary>
    public class PixelBlenderGrey : PixelBlender32
    {
        internal override void BlendPixel(int[] dstBuffer, int arrayOffset, Color srcColor)
        {
            unsafe
            {
                fixed (int* head = &dstBuffer[arrayOffset])
                {
                    BlendPixel32Internal(head, srcColor);
                }
            }
        }

        internal override unsafe void BlendPixel32(int* dstPtr, Color srcColor)
        {
            BlendPixel32Internal(dstPtr, srcColor);
        }

        internal override void BlendPixels(int[] dstBuffer,
            int arrayElemOffset,
            Color[] srcColors,
            int srcColorOffset,
            byte[] covers,
            int coversIndex,
            bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                int cover = covers[coversIndex];
                if (cover == 255)
                {
                    unsafe
                    {
                        fixed (int* head = &dstBuffer[arrayElemOffset])
                        {
                            int* header2 = (int*)(IntPtr)head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(header2, srcColors[srcColorOffset++]);
                                header2++;//move next
                                count--;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                BlendPixel32Internal(header2, srcColors[srcColorOffset++]);
                                header2++;//move next
                                count--;
                                //---------
                                //2
                                BlendPixel32Internal(header2, srcColors[srcColorOffset++]);
                                header2++;//move next
                                count--;
                            }

                        }
                    }
                }
                else
                {

                    unsafe
                    {
                        fixed (int* head = &dstBuffer[arrayElemOffset])
                        {
                            int* header2 = (int*)(IntPtr)head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(header2, srcColors[srcColorOffset++], cover);
                                header2++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                BlendPixel32Internal(header2, srcColors[srcColorOffset++], cover);
                                header2++;//move next
                                count--;
                                //2.
                                BlendPixel32Internal(header2, srcColors[srcColorOffset++], cover);
                                header2++;//move next
                                count--;
                            }

                        }
                    }
                }
            }
            else
            {
                unsafe
                {
                    fixed (int* dstHead = &dstBuffer[arrayElemOffset])
                    {
                        int* dstBufferPtr = dstHead;
                        do
                        {
                            //cover may diff in each loop
                            int cover = covers[coversIndex++];
                            if (cover == 255)
                            {
                                BlendPixel32Internal(dstBufferPtr, srcColors[srcColorOffset]);
                            }
                            else
                            {
                                BlendPixel32Internal(dstBufferPtr, srcColors[srcColorOffset].NewFromChangeCoverage(cover));
                            }
                            dstBufferPtr++;
                            ++srcColorOffset;
                        }
                        while (--count != 0);
                    }
                }

            }
        }


        internal override void CopyPixel(int[] dstBuffer, int arrayOffset, Color srcColor)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr = &dstBuffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl*** 
                        byte y = (byte)(((srcColor.red * 77) + (srcColor.green * 151) + (srcColor.blue * 28)) >> 8);
                        srcColor = new Color(srcColor.alpha, y, y, y);

                        *ptr = srcColor.ToARGB();
                    }
                }
            }

        }

        internal override void CopyPixels(int[] dstBuffer, int arrayOffset, Color srcColor, int count)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr_byte = &dstBuffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl***

                        byte y = (byte)(((srcColor.red * 77) + (srcColor.green * 151) + (srcColor.blue * 28)) >> 8);
                        srcColor = new Color(srcColor.alpha, y, y, y);

                        int* ptr = ptr_byte;
                        int argb = srcColor.ToARGB();

                        //---------
                        if ((count % 2) != 0)
                        {
                            *ptr = argb;
                            ptr++; //move next
                            count--;
                        }

                        while (count > 0)
                        {
                            //-----------
                            //1.
                            *ptr = argb;
                            ptr++; //move next
                            count--;
                            //-----------
                            //2
                            *ptr = argb;
                            ptr++; //move next
                            count--;
                        }

                    }
                }
            }

        }

        static unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor, int coverageValue)
        {

            //calculate new alpha
            byte y = (byte)(((srcColor.red * 77) + (srcColor.green * 151) + (srcColor.blue * 28)) >> 8);
            srcColor = new Color(srcColor.alpha, y, y, y);

            int src_a = (byte)((srcColor.alpha * coverageValue + 255) >> 8);
            //after apply the alpha
            unchecked
            {
                if (src_a == 255)
                {
                    *dstPtr = srcColor.ToARGB(); //just copy
                }
                else
                {
                    int dest = *dstPtr;
                    //separate each component
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *dstPtr =
                     ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                     ((byte)(((srcColor.red - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 16) |
                     ((byte)(((srcColor.green - g) * src_a + (g << ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT) << 8) |
                     ((byte)(((srcColor.blue - b) * src_a + (b << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT));
                }
            }
        }
        static unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor)
        {
            unchecked
            {

                //calculate new grey-scale color
                byte y = (byte)(((srcColor.red * 77) + (srcColor.green * 151) + (srcColor.blue * 28)) >> 8);
                srcColor = new Color(srcColor.alpha, y, y, y);

                if (srcColor.alpha == 255)
                {
                    *dstPtr = srcColor.ToARGB(); //just copy
                }
                else
                {
                    int dest = *dstPtr;
                    //separate each component
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);

                    byte src_a = srcColor.alpha;

                    *dstPtr =
                     ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                     ((byte)(((srcColor.red - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 16) |
                     ((byte)(((srcColor.green - g) * src_a + (g << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 8) |
                     ((byte)(((srcColor.blue - b) * src_a + (b << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT));
                }

            }
        }
    }



    /// <summary>
    /// apply mask to srcColor before send it to dest bmp
    /// </summary>
    public class PixelBlenderWithMask : PixelBlender32
    {
        ActualImage _maskImg;

        public PixelBlenderWithMask()
        {
            //please note that size of mask must be the same size of the dest buffer
        }
        public void SetMaskImage(ActualImage maskImg)
        {
            _maskImg = maskImg;
        }
        Color NewColorFromMask(Color srcColor, int arrayOffset)
        {
            //then apply alpha value to the srcColor
            return srcColor.NewFromChangeCoverage((byte)((ActualImage.GetBuffer(_maskImg)[arrayOffset]) >> 16));
        }
        internal override void BlendPixel(int[] dstBuffer, int arrayOffset, Color srcColor)
        {
            unsafe
            {
                fixed (int* head = &dstBuffer[arrayOffset])
                {
                    BlendPixel32Internal(head, NewColorFromMask(srcColor, arrayOffset));
                }
            }
        }

        internal override unsafe void BlendPixel32(int* dstPtr, Color srcColor)
        {
            BlendPixel32Internal(dstPtr, srcColor);
        }

        internal override void BlendPixels(int[] dstBuffer,
            int arrayElemOffset,
            Color[] srcColors,
            int srcColorOffset,
            byte[] covers,
            int coversIndex,
            bool firstCoverForAll, int count)
        {

            if (firstCoverForAll)
            {
                int cover = covers[coversIndex];
                if (cover == 255)
                {

                    unsafe
                    {
                        fixed (int* head = &dstBuffer[arrayElemOffset])
                        {
                            int* header2 = head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset++], arrayElemOffset));
                                header2++;//move next
                                count--;
                                arrayElemOffset++;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                //BlendPixel32Internal(header2, srcColors[srcColorOffset++]);
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset++], arrayElemOffset));
                                header2++;//move next
                                count--;
                                arrayElemOffset++;
                                //---------
                                //2
                                //BlendPixel32Internal(header2, srcColors[srcColorOffset++]);
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset++], arrayElemOffset));
                                header2++;//move next
                                count--;
                                arrayElemOffset++;
                            }

                        }
                    }
                }
                else
                {
                    unsafe
                    {
                        fixed (int* head = &dstBuffer[arrayElemOffset])
                        {
                            int* header2 = head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                //BlendPixel32Internal(header2, srcColors[srcColorOffset++], cover);
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset++], arrayElemOffset));
                                arrayElemOffset++;
                                header2++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                //BlendPixel32Internal(header2, srcColors[srcColorOffset++], cover);
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset++], arrayElemOffset));
                                arrayElemOffset++;

                                header2++;//move next
                                count--;
                                //2.
                                //BlendPixel32Internal(header2, srcColors[srcColorOffset++], cover);
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset++], arrayElemOffset));
                                arrayElemOffset++;

                                header2++;//move next
                                count--;
                            }

                        }
                    }
                }
            }
            else
            {
                unsafe
                {
                    fixed (int* dstHead = &dstBuffer[arrayElemOffset])
                    {
                        int* dstBufferPtr = dstHead;
                        do
                        {
                            //cover may diff in each loop
                            int cover = covers[coversIndex++];
                            if (cover == 255)
                            {
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset], arrayElemOffset));
                            }
                            else
                            {
                                BlendPixel(dstBuffer, arrayElemOffset, NewColorFromMask(srcColors[srcColorOffset].NewFromChangeCoverage(cover), arrayElemOffset));
                            }


                            arrayElemOffset++;
                            dstBufferPtr++;
                            ++srcColorOffset;
                        }
                        while (--count != 0);
                    }
                }
            }
        }
        internal override void CopyPixel(int[] dstBuffer, int arrayOffset, Color srcColor)
        {


            unsafe
            {
                unchecked
                {
                    fixed (int* ptr = &dstBuffer[arrayOffset])
                    {
                        BlendPixel32(ptr, NewColorFromMask(srcColor, arrayOffset));
                    }
                }
            }

        }

        internal override void CopyPixels(int[] dstBuffer, int arrayOffset, Color srcColor, int count)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr_byte = &dstBuffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl***
                        int* ptr = ptr_byte;

                        Color newColor = NewColorFromMask(srcColor, arrayOffset);

                        //---------
                        if ((count % 2) != 0)
                        {
                            BlendPixel32(ptr, newColor);

                            arrayOffset++;//move next
                            ptr++; //move next
                            count--;
                        }

                        while (count > 0)
                        {
                            //-----------

                            newColor = NewColorFromMask(srcColor, arrayOffset);

                            //1.
                            BlendPixel32(ptr, newColor);
                            arrayOffset++;//move next
                            ptr++; //move next
                            count--;
                            //-----------

                            //2
                            newColor = NewColorFromMask(srcColor, arrayOffset);

                            //1.
                            BlendPixel32(ptr, newColor);
                            arrayOffset++;//move next
                            ptr++; //move next
                            count--;
                        }

                    }
                }
            }

        }

        static unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor, int coverageValue)
        {

            //calculate new alpha
            int src_a = (byte)((srcColor.alpha * coverageValue + 255) >> 8);
            //after apply the alpha
            unchecked
            {
                if (src_a == 255)
                {
                    *dstPtr = srcColor.ToARGB(); //just copy
                }
                else
                {
                    int dest = *dstPtr;
                    //separate each component
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *dstPtr =
                     ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                     ((byte)(((srcColor.red - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 16) |
                     ((byte)(((srcColor.green - g) * src_a + (g << ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT) << 8) |
                     ((byte)(((srcColor.blue - b) * src_a + (b << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT));
                }
            }
        }
        static unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor)
        {
            unchecked
            {

                if (srcColor.alpha == 255)
                {
                    *dstPtr = srcColor.ToARGB(); //just copy
                }
                else
                {
                    int dest = *dstPtr;
                    //separate each component
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);

                    byte src_a = srcColor.alpha;

                    *dstPtr =
                     ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                     ((byte)(((srcColor.red - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 16) |
                     ((byte)(((srcColor.green - g) * src_a + (g << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 8) |
                     ((byte)(((srcColor.blue - b) * src_a + (b << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT));
                }
            }
        }
    }

}
