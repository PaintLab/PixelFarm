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
        internal override void BlendPixel32(int[] buffer, int arrayOffset, Color sourceColor)
        {
            unsafe
            {
                fixed (int* head = &buffer[arrayOffset])
                {
                    BlendPixel32Internal(head, sourceColor);
                }
            }
        }

        internal override unsafe void BlendPixel32(int* ptr, Color sc)
        {
            BlendPixel32Internal(ptr, sc);
        }

        internal override void BlendPixels(int[] destBuffer, int arrayElemOffset, Color[] sourceColors, int sourceColorsOffset, byte[] covers, int coversIndex, bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                int cover = covers[coversIndex];
                if (cover == 255)
                {

                    unsafe
                    {
                        fixed (int* dstHead = &destBuffer[arrayElemOffset])
                        {
                            int* dstBuffer = dstHead;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++]);
                                dstBuffer++;//move next
                                count--;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++]);
                                dstBuffer++;//move next
                                count--;
                                //---------
                                //2
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++]);
                                dstBuffer++;//move next
                                count--;
                            }

                        }
                    }
                }
                else
                {

                    unsafe
                    {
                        fixed (int* head = &destBuffer[arrayElemOffset])
                        {
                            int* header2 = (int*)(IntPtr)head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(header2, sourceColors[sourceColorsOffset++], cover);
                                header2++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                BlendPixel32Internal(header2, sourceColors[sourceColorsOffset++], cover);
                                header2++;//move next
                                count--;
                                //2.
                                BlendPixel32Internal(header2, sourceColors[sourceColorsOffset++], cover);
                                header2++;//move next
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
                        BlendPixel32(destBuffer, arrayElemOffset, sourceColors[sourceColorsOffset]);
                    }
                    else
                    {
                        BlendPixel32(destBuffer, arrayElemOffset, sourceColors[sourceColorsOffset].NewFromChangeCoverage(cover));
                    }
                    arrayElemOffset++;
                    ++sourceColorsOffset;
                }
                while (--count != 0);
            }
        }


        internal override void CopyPixel(int[] buffer, int arrayOffset, Color sourceColor)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr = &buffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl***  

                        //
                        int dest = *ptr;
                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);
                        int src_a = sourceColor.A;

                        *ptr = (((((sourceColor.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b; ;
                    }
                }
            }
        }

        internal override void CopyPixels(int[] buffer, int arrayOffset, Color sourceColor, int count)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr_byte = &buffer[arrayOffset])
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

                        int src_a = sourceColor.A;
                        int value = (((((sourceColor.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b;


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

        static unsafe void BlendPixel32Internal(int* ptr, Color sc, int coverageValue)
        {
            //calculate new alpha
            int src_a = (byte)((sc.alpha * coverageValue + 255) >> 8);
            //after apply the alpha
            unchecked
            {
                if (src_a == 255)
                {
                    int dest = *ptr;
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *ptr = (((((sc.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b;
                }
                else
                {
                    int dest = *ptr;

                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);



                    *ptr = (((((sc.R - a) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                            (r << 16) |
                            (g << 8) |
                            b;
                }
            }

        }
        static unsafe void BlendPixel32Internal(int* ptr, Color sc)
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
                int dest = *ptr;
                byte a = (byte)((dest >> 24) & 0xff);
                byte r = (byte)((dest >> 16) & 0xff);
                byte g = (byte)((dest >> 8) & 0xff);
                byte b = (byte)((dest) & 0xff);

                *ptr = (((((sc.R - a) * sc.alpha + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 24) |
                 (r << 16) |
                 (g << 8) |
                 b;
                //}
            }
        }
    }


    public class PixelBlenderRed : PixelBlenderBGRABase, IPixelBlender
    {
        internal override void BlendPixel32(int[] buffer, int arrayOffset, Color sourceColor)
        {
            unsafe
            {
                fixed (int* head = &buffer[arrayOffset])
                {
                    BlendPixel32Internal(head, sourceColor);
                }
            }
        }

        internal override unsafe void BlendPixel32(int* ptr, Color sc)
        {
            BlendPixel32Internal(ptr, sc);
        }

        internal override void BlendPixels(int[] destBuffer, int arrayElemOffset, Color[] sourceColors, int sourceColorsOffset, byte[] covers, int coversIndex, bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                int cover = covers[coversIndex];
                if (cover == 255)
                {

                    unsafe
                    {
                        fixed (int* dstHead = &destBuffer[arrayElemOffset])
                        {
                            int* dstBuffer = dstHead;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++]);
                                dstBuffer++;//move next
                                count--;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++]);
                                dstBuffer++;//move next
                                count--;
                                //---------
                                //2
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++]);
                                dstBuffer++;//move next
                                count--;
                            }

                        }
                    }
                }
                else
                {

                    unsafe
                    {
                        fixed (int* dstHead = &destBuffer[arrayElemOffset])
                        {
                            int* dstBuffer = dstHead;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++], cover);
                                dstBuffer++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++], cover);
                                dstBuffer++;//move next
                                count--;
                                //2.
                                BlendPixel32Internal(dstBuffer, sourceColors[sourceColorsOffset++], cover);
                                dstBuffer++;//move next
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
                        BlendPixel32(destBuffer, arrayElemOffset, sourceColors[sourceColorsOffset]);
                    }
                    else
                    {
                        BlendPixel32(destBuffer, arrayElemOffset, sourceColors[sourceColorsOffset].NewFromChangeCoverage(cover));
                    }
                    arrayElemOffset++;
                    ++sourceColorsOffset;
                }
                while (--count != 0);
            }
        }


        internal override void CopyPixel(int[] buffer, int arrayOffset, Color sourceColor)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr = &buffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl***  

                        //
                        int dest = *ptr;
                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);
                        int src_a = sourceColor.A;

                        *ptr = (a << 24) |
                            (sourceColor.R << 16) |
                            (g << 8) |
                            b; ;
                    }
                }
            }
        }

        internal override void CopyPixels(int[] buffer, int arrayOffset, Color sourceColor, int count)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr_byte = &buffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl***
                        int* ptr = ptr_byte;
                        int dest = *ptr;

                        //
                        byte a = (byte)((dest >> 24) & 0xff);
                        byte r = (byte)((dest >> 16) & 0xff);
                        byte g = (byte)((dest >> 8) & 0xff);
                        byte b = (byte)((dest) & 0xff);

                        int src_a = sourceColor.A;
                        int value = a << 24 |
                            (sourceColor.R << 16) |
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

        static unsafe void BlendPixel32Internal(int* ptr, Color sc, int coverageValue)
        {
            //calculate new alpha
            int src_a = (byte)((sc.alpha * coverageValue + 255) >> 8);
            //after apply the alpha
            unchecked
            {
                if (src_a == 255)
                {
                    int dest = *ptr;
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *ptr = ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                            (((((sc.R - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                            (g << 8) |
                            b; ;
                }
                else
                {
                    int dest = *ptr;

                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);



                    *ptr = ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                            (((((sc.R - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                            (g << 8) |
                            b; ;
                }
            }

        }
        static unsafe void BlendPixel32Internal(int* ptr, Color sc)
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
                int dest = *ptr;
                byte a = (byte)((dest >> 24) & 0xff);
                byte r = (byte)((dest >> 16) & 0xff);
                byte g = (byte)((dest >> 8) & 0xff);
                byte b = (byte)((dest) & 0xff);

                int src_a = sc.alpha;

                *ptr = ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                        (((((sc.R - r) * sc.A + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) & 0xff) << 16) |
                        (g << 8) |
                        b; ;
                //}
            }
        }
    } 
}
