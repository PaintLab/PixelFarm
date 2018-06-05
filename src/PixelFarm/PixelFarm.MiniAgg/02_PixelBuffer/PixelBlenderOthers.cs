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
    public class PixelBlenderAlpha : PixelBlenderBGRABase, IPixelBlender
    {
        internal override void BlendPixel32(int[] dstBuffer, int arrayOffset, Color srcColor)
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
                        fixed (int* dstHead = &dstBuffer[arrayElemOffset])
                        {
                            int* dstPtr = dstHead;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++]);
                                dstPtr++;//move next
                                count--;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++]);
                                dstPtr++;//move next
                                count--;
                                //---------
                                //2
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++]);
                                dstPtr++;//move next
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
                            int* dstPtr = head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++], cover);
                                dstPtr++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++], cover);
                                dstPtr++;//move next
                                count--;
                                //2.
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++], cover);
                                dstPtr++;//move next
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
                        int* dstPtr = dstHead;
                        do
                        {
                            //cover may diff in each loop
                            int cover = covers[coversIndex++];
                            if (cover == 255)
                            {
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset]);
                            }
                            else
                            {
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset].NewFromChangeCoverage(cover));
                            }
                            arrayElemOffset++;
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

                        //
                        int dest = *ptr;
                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);
                        int src_a = srcColor.A;

                        *ptr = (((((srcColor.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b; ;
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

                        //
                        int dest = *ptr;

                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);


                        int argb = (((((srcColor.R - a) * srcColor.A + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b;


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
            int src_a = (byte)((srcColor.alpha * coverageValue + 255) >> 8);
            //after apply the alpha
            unchecked
            {
                if (src_a == 255)
                {
                    int dest = *dstPtr;
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *dstPtr = (((((srcColor.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b;
                }
                else
                {
                    int dest = *dstPtr;

                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);



                    *dstPtr = (((((srcColor.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b;
                }
            }

        }
        static unsafe void BlendPixel32Internal(int* dstPtr, Color sc)
        {
            unchecked
            {
                //if (sc.alpha == 255)
                //{
                //    int src_a = 255;
                //    int dest = *ptr;

                //    byte a = (byte)((dest >> 24) & 0xff);
                //    byte r = (byte)((dest >> 16) & 0xff);
                //    byte g = (byte)((dest >> 8) & 0xff);
                //    byte b = (byte)((dest) & 0xff);

                //    int value = ((byte)(255) << 24) |
                //                ((byte)((((0 - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                //                 (g << 8) |
                //                  b;

                //    *ptr = value;
                //}
                //else
                //{
                int dest = *dstPtr;
                byte a = (byte)((dest >> 24) & 0xff);
                byte r = (byte)((dest >> 16) & 0xff);
                byte g = (byte)((dest >> 8) & 0xff);
                byte b = (byte)((dest) & 0xff);

                *dstPtr = (((((sc.R - a) * sc.alpha + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                 (r << 16) |
                 (g << 8) |
                 b;
                //}
            }
        }
    }


    public class PixelBlenderRed : PixelBlenderBGRABase, IPixelBlender
    {
        internal override void BlendPixel32(int[] dstBuffer, int arrayOffset, Color srcColor)
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
            bool firstCoverForAll, 
            int count)
        {
            if (firstCoverForAll)
            {
                int cover = covers[coversIndex];
                if (cover == 255)
                {

                    unsafe
                    {
                        fixed (int* dstHead = &dstBuffer[arrayElemOffset])
                        {
                            int* dstPtr = dstHead;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++]);
                                dstPtr++;//move next
                                count--;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++]);
                                dstPtr++;//move next
                                count--;
                                //---------
                                //2
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++]);
                                dstPtr++;//move next
                                count--;
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
                            int* dstPtr = dstHead;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++], cover);
                                dstPtr++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++], cover);
                                dstPtr++;//move next
                                count--;
                                //2.
                                BlendPixel32Internal(dstPtr, srcColors[srcColorOffset++], cover);
                                dstPtr++;//move next
                                count--;
                            }

                        }
                    }
                }
            }
            else
            {
                do
                {
                    //cover may diff in each loop
                    int cover = covers[coversIndex++];
                    if (cover == 255)
                    {
                        BlendPixel32(dstBuffer, arrayElemOffset, srcColors[srcColorOffset]);
                    }
                    else
                    {
                        BlendPixel32(dstBuffer, arrayElemOffset, srcColors[srcColorOffset].NewFromChangeCoverage(cover));
                    }
                    arrayElemOffset++;
                    ++srcColorOffset;
                }
                while (--count != 0);
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

                        //
                        int dest = *ptr;
                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);
                        int src_a = srcColor.A;

                        *ptr = (a << 24) |
                            (srcColor.R << 16) |
                            (g << 8) |
                            b; ;
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
                        int dest = *ptr;

                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);

                        int src_a = srcColor.A;
                        int value = a << 24 |
                            (srcColor.R << 16) |
                            (g << 8) |
                            b; ;


                        //---------
                        if ((count % 2) != 0)
                        {
                            *ptr = value;
                            ptr++; //move next
                            count--;
                        }

                        while (count > 0)
                        {
                            //-----------
                            //1.
                            *ptr = value;
                            ptr++; //move next
                            count--;
                            //-----------
                            //2
                            *ptr = value;
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
                    int dest = *dstPtr;
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *dstPtr = ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                            (((((srcColor.R - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                            (g << 8) |
                            b; ;
                }
                else
                {
                    int dest = *dstPtr;

                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);



                    *dstPtr = ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                            (((((srcColor.R - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                            (g << 8) |
                            b; ;
                }
            }

        }
        static unsafe void BlendPixel32Internal(int* dstPtr, Color srcColor)
        {
            unchecked
            {
                //if (sc.alpha == 255)
                //{
                //    int src_a = 255;
                //    int dest = *ptr;

                //    byte a = (byte)((dest >> 24) & 0xff);
                //    byte r = (byte)((dest >> 16) & 0xff);
                //    byte g = (byte)((dest >> 8) & 0xff);
                //    byte b = (byte)((dest) & 0xff);

                //    int value = ((byte)(255) << 24) |
                //                ((byte)((((0 - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                //                 (g << 8) |
                //                  b;

                //    *ptr = value;
                //}
                //else
                //{
                int dest = *dstPtr;
                byte a = (byte)((dest >> 24) & 0xff);
                byte r = (byte)((dest >> 16) & 0xff);
                byte g = (byte)((dest >> 8) & 0xff);
                byte b = (byte)((dest) & 0xff);

                int src_a = srcColor.alpha;

                *dstPtr = ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                        (((((srcColor.R - r) * srcColor.A + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                        (g << 8) |
                        b; ;
                //}
            }
        }
    }
}
