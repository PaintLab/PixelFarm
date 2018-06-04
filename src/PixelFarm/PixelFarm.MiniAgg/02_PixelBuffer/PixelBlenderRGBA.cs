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
#define USE_BLENDER

using PixelFarm.Drawing;
using System;

namespace PixelFarm.Agg.Imaging
{
    /// <summary>
    /// look up table helper for clamp value from 9 bits to 8 bits
    /// </summary>
    static class ClampFrom9To8Bits
    {
        internal static readonly byte[] _ = new byte[1 << 9];
        static ClampFrom9To8Bits()
        {
            //this is a clamp table
            //9 bits to 8 bits
            //if we don't use this clamp table
            for (int i = _.Length - 1; i >= 0; --i)
            {
                _[i] = (byte)Math.Min(i, 255);
            }
        }
    }



    public interface IPixelBlender
    {
        /// <summary>
        /// info 
        /// </summary>
        int NumPixelBits { get; }

        //Color PixelToColorRGBA(byte[] buffer, int bufferOffset); 
        ///// <summary>
        ///// copy source color and set to dest buffer
        ///// </summary>
        ///// <param name="buffer"></param>
        ///// <param name="bufferOffset"></param>
        ///// <param name="sourceColor"></param>
        ///// <param name="count"></param>
        //void CopyPixels(byte[] buffer, int bufferOffset, Color sourceColor, int count);
        ///// <summary>
        ///// copy source color and set to dest buffer
        ///// </summary>
        ///// <param name="buffer"></param>
        ///// <param name="bufferOffset"></param>
        ///// <param name="sourceColor"></param>
        //void CopyPixel(byte[] buffer, int bufferOffset, Color sourceColor);
        ///// <summary>
        ///// blend source color to dest buffer with specific blend function of this PixelBlender
        ///// </summary>
        ///// <param name="buffer"></param>
        ///// <param name="bufferOffset"></param>
        ///// <param name="sourceColor"></param>
        //void BlendPixel(byte[] buffer, int bufferOffset, Color sourceColor);

        ///// <summary>
        ///// blend source color to dest buffer with specific blend function of this PixelBlender
        ///// </summary>
        ///// <param name="buffer"></param>
        ///// <param name="bufferOffset"></param>
        ///// <param name="sourceColors"></param>
        ///// <param name="sourceColorsOffset"></param>
        ///// <param name="sourceCovers"></param>
        ///// <param name="sourceCoversOffset"></param>
        ///// <param name="firstCoverForAll"></param>
        ///// <param name="count"></param>
        //void BlendPixels(byte[] buffer, nt bufferOffset, Color[] sourceColors, int sourceColorsOffset, byte[] sourceCovers, int sourceCoversOffset, bool firstCoverForAll, int count);
    }

    public sealed class PixelBlenderBGRA : IPixelBlender
    {
        //from https://microsoft.github.io/Win2D/html/PremultipliedAlpha.htm
        //1. Straight alpha
        //result = (source.RGB* source.A) + (dest.RGB* (1 - source.A))
        //---
        //2. Premultiplied alpha
        //result = source.RGB + (dest.RGB * (1 - source.A))
        //---
        //3. Converting between alpha formats
        //3.1 from straight to premult
        //premultiplied.R = (byte) (straight.R* straight.A / 255);
        //premultiplied.G = (byte) (straight.G* straight.A / 255);
        //premultiplied.B = (byte) (straight.B* straight.A / 255);
        //premultiplied.A = straight.A;
        //3.2 from premult to strait
        //straight.R = premultiplied.R  * ((1/straight.A) * 255);
        //straight.G = premultiplied.G  * ((1/straight.A) * 255);
        //straight.B = premultiplied.B  * ((1/straight.A) * 255);
        //straight.A = premultiplied.A;



        bool _enableGamma;
        float _gammaValue;
        public PixelBlenderBGRA() { }
        public bool EnableGamma
        {
            get { return _enableGamma; }
            set
            {

                if (value != _enableGamma)
                {

                }
                this._enableGamma = value;
            }
        }
        public float GammaValue
        {
            get { return _gammaValue; }
            set
            {
                _gammaValue = value;
                //TODO: 
                //get new gamma table
            }
        }

        public int NumPixelBits { get { return 32; } }
        const byte BASE_MASK = 255;
        /// <summary>
        /// blend source color to target buffer
        /// </summary>
        /// <param name="dstBuffer"></param>
        /// <param name="arrayOffset"></param>
        /// <param name="srcColor"></param>
        public void BlendPixel32(int[] dstBuffer, int arrayOffset, Color srcColor)
        {
            unsafe
            {
                fixed (int* head = &dstBuffer[arrayOffset])
                {
                    Blend32PixelInternal(head, srcColor);
                }
            }
        }


        /// <summary>
        /// blend src color to target buffer
        /// </summary>
        /// <param name="dstBufferPtr"></param>
        /// <param name="srcColor"></param>
        /// <param name="coverageValue"></param>
        static unsafe void Blend32PixelInternal(int* dstBufferPtr, Color srcColor, byte coverageValue)
        {
            //calculate new alpha
            int src_a = (byte)((srcColor.alpha * coverageValue + 255) >> 8);
            //after apply the alpha
            unchecked
            {
                if (src_a == 255)
                {
                    *dstBufferPtr = srcColor.ToARGB(); //just copy
                }
                else
                {
                    int dest = *dstBufferPtr;
                    //separate each component
                    byte a = (byte)((dest >> 24) & 0xff);
                    byte r = (byte)((dest >> 16) & 0xff);
                    byte g = (byte)((dest >> 8) & 0xff);
                    byte b = (byte)((dest) & 0xff);


                    *dstBufferPtr =
                     ((byte)((src_a + a) - ((src_a * a + BASE_MASK) >> ColorEx.BASE_SHIFT)) << 24) |
                     ((byte)(((srcColor.red - r) * src_a + (r << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT) << 16) |
                     ((byte)(((srcColor.green - g) * src_a + (g << ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT) << 8) |
                     ((byte)(((srcColor.blue - b) * src_a + (b << ColorEx.BASE_SHIFT)) >> ColorEx.BASE_SHIFT));
                }
            }
        }



        internal static unsafe void Blend32PixelInternal(int* dstPtr, Color srcColor)
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



        public void BlendPixels(
            int[] dstBuffer, int arrayElemOffset,
            Color[] srcColors, int srcColorsOffset,
            byte[] covers, int coversIndex, bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                byte cover = covers[coversIndex];
                if (cover == 255)
                {
                    //version 1
                    //do
                    //{
                    //    BlendPixel(destBuffer, bufferOffset, sourceColors[sourceColorsOffset++]);
                    //    bufferOffset += 4;
                    //}
                    //while (--count != 0);

                    //version 2
                    //unsafe
                    //{
                    //    fixed (byte* head = &destBuffer[bufferOffset])
                    //    {
                    //        int* header2 = (int*)(IntPtr)head;
                    //        do
                    //        {
                    //            Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++]);
                    //            header2++;//move next
                    //        }
                    //        while (--count != 0);
                    //    }
                    //}
                    //------------------------------
                    //version 3: similar to version 2, but have a plan
                    unsafe
                    {
                        fixed (int* head = &dstBuffer[arrayElemOffset])
                        {
                            int* header2 = (int*)(IntPtr)head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                Blend32PixelInternal(header2, srcColors[srcColorsOffset++]);
                                header2++;//move next
                                count--;
                            }

                            //now count is even number
                            while (count > 0)
                            {
                                //now count is even number
                                //---------
                                //1
                                Blend32PixelInternal(header2, srcColors[srcColorsOffset++]);
                                header2++;//move next
                                count--;
                                //---------
                                //2
                                Blend32PixelInternal(header2, srcColors[srcColorsOffset++]);
                                header2++;//move next
                                count--;
                            }

                        }
                    }
                }
                else
                {
                    ////version 1
                    //do
                    //{
                    //    BlendPixel(destBuffer, bufferOffset, sourceColors[sourceColorsOffset].NewFromChangeCoverage(cover));
                    //    bufferOffset += 4;
                    //    ++sourceColorsOffset;
                    //}
                    //while (--count != 0);

                    ////version 2 
                    //unsafe
                    //{
                    //    fixed (byte* head = &destBuffer[bufferOffset])
                    //    {
                    //        int* header2 = (int*)(IntPtr)head;
                    //        do
                    //        {

                    //            //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                    //            Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++], cover);
                    //            header2++;//move next
                    //        }
                    //        while (--count != 0);
                    //    }
                    //}
                    //------------------------------
                    //version 3: similar to version 2, but have a plan
                    unsafe
                    {
                        fixed (int* head = &dstBuffer[arrayElemOffset])
                        {
                            int* header2 = (int*)(IntPtr)head;

                            if (count % 2 != 0)
                            {
                                //odd
                                //
                                Blend32PixelInternal(header2, srcColors[srcColorsOffset++], cover);
                                header2++;//move next
                                count--;
                            }
                            while (count > 0)
                            {
                                //Blend32PixelInternal(header2, sourceColors[sourceColorsOffset++].NewFromChangeCoverage(cover));
                                //1.
                                Blend32PixelInternal(header2, srcColors[srcColorsOffset++], cover);
                                header2++;//move next
                                count--;
                                //2.
                                Blend32PixelInternal(header2, srcColors[srcColorsOffset++], cover);
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
                        BlendPixel32(dstBuffer, arrayElemOffset, srcColors[srcColorsOffset]);
                    }
                    else
                    {
                        BlendPixel32(dstBuffer, arrayElemOffset, srcColors[srcColorsOffset].NewFromChangeCoverage(cover));
                    }
                    arrayElemOffset++;
                    ++srcColorsOffset;
                }
                while (--count != 0);
            }
        }

        public void CopyPixels(int[] dstBuffer, int arrayOffset, Color srcColor, int count)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr_byte = &dstBuffer[arrayOffset])
                    {
                        //TODO: consider use memcpy() impl***
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


        public void CopyPixel(int[] dstBuffer, int arrayOffset, Color srcColor)
        {
            unsafe
            {
                unchecked
                {
                    fixed (int* ptr = &dstBuffer[arrayOffset])
                    {
                        //TODO: review 
                        *ptr = srcColor.ToARGB();
                    }
                }
            }
        }




        public Color PixelToColorRGBA(int[] buffer, int bufferOffset32)
        {
            //TODO: review here ...             
            //check if the buffer is pre-multiplied color?
            //if yes=> this is not correct, 
            //we must convert the pixel from pre-multiplied color 
            //to the 'straight alpha color'

            int value = buffer[bufferOffset32];
            return new Color(
               (byte)((value >> (CO.A * 8)) & 0xff),
               (byte)((value >> (CO.R * 8)) & 0xff),
               (byte)((value >> (CO.G * 8)) & 0xff),
               (byte)((value >> (CO.B * 8)) & 0xff));

            //        buffer[bufferOffset + CO.A],
            //        buffer[bufferOffset + CO.R],
            //        buffer[bufferOffset + CO.G],
            //        buffer[bufferOffset + CO.B]
            //        );
            //}

        }
    }

}

