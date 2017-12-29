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
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
#define USE_UNSAFE_CODE

using System;
using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
namespace PixelFarm.Agg.Imaging
{
    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
    public class ImgSpanGenRGBA_NN_StepXBy1 : ImgSpanGen
    {
        const int BASE_SHITF = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHITF);
        const int BASE_MASK = BASE_SCALE - 1;
        ImageReaderWriterBase srcRW;
        public ImgSpanGenRGBA_NN_StepXBy1(IImageReaderWriter src, ISpanInterpolator spanInterpolator)
            : base(spanInterpolator)
        {
            srcRW = (ImageReaderWriterBase)src;
            if (srcRW.BitDepth != 32)
            {
                throw new NotSupportedException("The source is expected to be 32 bit.");
            }
        }

        public sealed override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + dx, y + dy, len);
            int x_hr;
            int y_hr;
            spanInterpolator.GetCoord(out x_hr, out y_hr);
            int x_lr = x_hr >> img_subpix_const.SHIFT;
            int y_lr = y_hr >> img_subpix_const.SHIFT;
            int bufferIndex = srcRW.GetBufferOffsetXY32(x_lr, y_lr);

            unsafe
            {
                TempMemPtr srcBufferPtr = srcRW.GetBufferPtr();
                int* pSource = (int*)srcBufferPtr.Ptr + bufferIndex;
                {
                    //int* src_ptr = (int*)pSource;
                    do
                    {
                        int src_value = *pSource;
                        //separate each component
                        byte a = (byte)((src_value >> 24) & 0xff);
                        byte r = (byte)((src_value >> 16) & 0xff);
                        byte g = (byte)((src_value >> 8) & 0xff);
                        byte b = (byte)((src_value) & 0xff);

                        //TODO: review here, color from source buffer
                        //should be in 'pre-multiplied' format.
                        //so it should be converted to 'straight' color by call something like ..'FromPreMult()' 

                        outputColors[startIndex++] = Drawing.Color.FromArgb(a, r, g, b);

                        pSource++;//move next
                    } while (--len != 0);
                }
                srcBufferPtr.Release();
            }



            //version 1 , incorrect
            //ISpanInterpolator spanInterpolator = Interpolator;
            //spanInterpolator.Begin(x + dx, y + dy, len);
            //int x_hr;
            //int y_hr;
            //spanInterpolator.GetCoord(out x_hr, out y_hr);
            //int x_lr = x_hr >> img_subpix_const.SHIFT;
            //int y_lr = y_hr >> img_subpix_const.SHIFT;
            //int bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);
            //byte[] srcBuffer = srcRW.GetBuffer();
            //unsafe
            //{
            //    fixed (byte* pSource = srcBuffer)
            //    {
            //        do
            //        {
            //            outputColors[startIndex++] = *(Drawing.Color*)&(pSource[bufferIndex]);
            //            bufferIndex += 4;
            //        } while (--len != 0);
            //    }
            //}
        }
    }



    public class ImgSpanGenRGBA_BilinearClip : ImgSpanGen
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;
        ImageReaderWriterBase srcRW;
        Drawing.Color m_bgcolor;
        int bytesBetweenPixelInclusive;
        public ImgSpanGenRGBA_BilinearClip(IImageReaderWriter src,
            Drawing.Color back_color,
            ISpanInterpolator inter)
            : base(inter)
        {
            m_bgcolor = back_color;
            srcRW = (ImageReaderWriterBase)src;
            bytesBetweenPixelInclusive = srcRW.BytesBetweenPixelsInclusive;
        }
        bool _mode0 = false;
        public override void Prepare()
        {
            base.Prepare();

            ISpanInterpolator spanInterpolator = base.Interpolator;
            _mode0 = (spanInterpolator.GetType() == typeof(PixelFarm.Agg.Transform.SpanInterpolatorLinear)
                && ((PixelFarm.Agg.Transform.SpanInterpolatorLinear)spanInterpolator).Transformer.GetType() == typeof(PixelFarm.Agg.Transform.Affine)
                && ((PixelFarm.Agg.Transform.Affine)((PixelFarm.Agg.Transform.SpanInterpolatorLinear)spanInterpolator).Transformer).IsIdentity());
        }
        public Drawing.Color BackgroundColor
        {
            get { return this.m_bgcolor; }
            set { this.m_bgcolor = value; }
        }

        public sealed override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
        {

            unsafe
            {
                //TODO: review here
               
                int bufferIndex; 
              
                if (_mode0)
                { 
                  
                    TempMemPtr srcBufferPtr = srcRW.GetBufferPtr();
                    byte* srcBuffer = (byte*)srcBufferPtr.BytePtr;
                    bufferIndex = srcRW.GetByteBufferOffsetXY(x, y);
                    //unsafe
                    {
#if true
                        do
                        {
                            //TODO: review here, match component?
                            //ORDER IS IMPORTANT!
                            //TODO : use CO (color order instead)
                            byte b = (byte)srcBuffer[bufferIndex++];
                            byte g = (byte)srcBuffer[bufferIndex++];
                            byte r = (byte)srcBuffer[bufferIndex++];
                            byte a = (byte)srcBuffer[bufferIndex++];

                            outputColors[startIndex] = Drawing.Color.FromArgb(a, r, g, b);

                            ++startIndex;
                        } while (--len != 0);
#else
                        fixed (byte* pSource = &fg_ptr[bufferIndex])
                        {
                            int* pSourceInt = (int*)pSource;
                            fixed (RGBA_Bytes* pDest = &span[spanIndex])
                            {
                                int* pDestInt = (int*)pDest;
                                do
                                {
                                    *pDestInt++ = *pSourceInt++;
                                } while (--len != 0);
                            }
                        }
#endif
                    }
                    srcBufferPtr.Release();
                    return;
                }
                else
                {
                    ISpanInterpolator spanInterpolator = base.Interpolator;
                    TempMemPtr srcBufferPtr = srcRW.GetBufferPtr();
                    byte* srcBuffer = (byte*)srcBufferPtr.BytePtr;
                    bufferIndex = srcRW.GetByteBufferOffsetXY(x, y);


                    spanInterpolator.Begin(x + base.dx, y + base.dy, len);
                    int accColor0, accColor1, accColor2, accColor3;
                    int back_r = m_bgcolor.red;
                    int back_g = m_bgcolor.green;
                    int back_b = m_bgcolor.blue;
                    int back_a = m_bgcolor.alpha;
                    int maxx = srcRW.Width - 1;
                    int maxy = srcRW.Height - 1;
                    //srcBuffer = srcRW.GetBuffer();
                    unchecked
                    {
                        do
                        {
                            int x_hr;
                            int y_hr;
                            spanInterpolator.GetCoord(out x_hr, out y_hr);
                            x_hr -= base.dxInt;
                            y_hr -= base.dyInt;
                            int x_lr = x_hr >> img_subpix_const.SHIFT;
                            int y_lr = y_hr >> img_subpix_const.SHIFT;
                            int weight;
                            if (x_lr >= 0 && y_lr >= 0 &&
                               x_lr < maxx && y_lr < maxy)
                            {
                                accColor0 =
                                accColor1 =
                                accColor2 =
                                accColor3 = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;
                                x_hr &= (int)img_subpix_const.MASK;
                                y_hr &= (int)img_subpix_const.MASK;
                                bufferIndex = srcRW.GetByteBufferOffsetXY(x_lr, y_lr);
                                weight = (((int)img_subpix_const.SCALE - x_hr) *
                                         ((int)img_subpix_const.SCALE - y_hr));
                                if (weight > BASE_MASK)
                                {
                                    accColor0 += weight * srcBuffer[bufferIndex + CO.R];
                                    accColor1 += weight * srcBuffer[bufferIndex + CO.G];
                                    accColor2 += weight * srcBuffer[bufferIndex + CO.B];
                                    accColor3 += weight * srcBuffer[bufferIndex + CO.A];
                                }

                                weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                                if (weight > BASE_MASK)
                                {
                                    bufferIndex += bytesBetweenPixelInclusive;
                                    accColor0 += weight * srcBuffer[bufferIndex + CO.R];
                                    accColor1 += weight * srcBuffer[bufferIndex + CO.G];
                                    accColor2 += weight * srcBuffer[bufferIndex + CO.B];
                                    accColor3 += weight * srcBuffer[bufferIndex + CO.A];
                                }

                                weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                                if (weight > BASE_MASK)
                                {
                                    ++y_lr;
                                    bufferIndex = srcRW.GetByteBufferOffsetXY(x_lr, y_lr);
                                    accColor0 += weight * srcBuffer[bufferIndex + CO.R];
                                    accColor1 += weight * srcBuffer[bufferIndex + CO.G];
                                    accColor2 += weight * srcBuffer[bufferIndex + CO.B];
                                    accColor3 += weight * srcBuffer[bufferIndex + CO.A];
                                }
                                weight = (x_hr * y_hr);
                                if (weight > BASE_MASK)
                                {
                                    bufferIndex += bytesBetweenPixelInclusive;
                                    accColor0 += weight * srcBuffer[bufferIndex + CO.R];
                                    accColor1 += weight * srcBuffer[bufferIndex + CO.G];
                                    accColor2 += weight * srcBuffer[bufferIndex + CO.B];
                                    accColor3 += weight * srcBuffer[bufferIndex + CO.A];
                                }
                                accColor0 >>= img_subpix_const.SHIFT * 2;
                                accColor1 >>= img_subpix_const.SHIFT * 2;
                                accColor2 >>= img_subpix_const.SHIFT * 2;
                                accColor3 >>= img_subpix_const.SHIFT * 2;
                            }
                            else
                            {
                                if (x_lr < -1 || y_lr < -1 ||
                                   x_lr > maxx || y_lr > maxy)
                                {
                                    accColor0 = back_r;
                                    accColor1 = back_g;
                                    accColor2 = back_b;
                                    accColor3 = back_a;
                                }
                                else
                                {
                                    accColor0 =
                                    accColor1 =
                                    accColor2 =
                                    accColor3 = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;
                                    x_hr &= (int)img_subpix_const.MASK;
                                    y_hr &= (int)img_subpix_const.MASK;
                                    weight = (((int)img_subpix_const.SCALE - x_hr) *
                                             ((int)img_subpix_const.SCALE - y_hr));
                                    if (weight > BASE_MASK)
                                    {
                                    }

                                    x_lr++;
                                    weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                                    if (weight > BASE_MASK)
                                    {
                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {
                                            BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref accColor3,
                                                srcBuffer,
                                                srcRW.GetByteBufferOffsetXY(x_lr, y_lr),
                                                weight);
                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }
                                    }

                                    x_lr--;
                                    y_lr++;
                                    weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                                    if (weight > BASE_MASK)
                                    {
                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {
                                            BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref accColor3,
                                                srcBuffer,
                                                srcRW.GetByteBufferOffsetXY(x_lr, y_lr),
                                                weight);
                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }
                                    }

                                    x_lr++;
                                    weight = (x_hr * y_hr);
                                    if (weight > BASE_MASK)
                                    {
                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {
                                            BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref accColor3,
                                               srcBuffer,
                                               srcRW.GetByteBufferOffsetXY(x_lr, y_lr),
                                               weight);
                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }
                                    }

                                    accColor0 >>= img_subpix_const.SHIFT * 2;
                                    accColor1 >>= img_subpix_const.SHIFT * 2;
                                    accColor2 >>= img_subpix_const.SHIFT * 2;
                                    accColor3 >>= img_subpix_const.SHIFT * 2;
                                }
                            }

                            outputColors[startIndex] = PixelFarm.Drawing.Color.FromArgb(
                                (byte)accColor3,
                                (byte)accColor0,
                                (byte)accColor1,
                                (byte)accColor2
                                );

                            //outputColors[startIndex].red = (byte)accColor0;
                            //outputColors[startIndex].green = (byte)accColor1;
                            //outputColors[startIndex].blue = (byte)accColor2;
                            //outputColors[startIndex].alpha = (byte)accColor3;
                            ++startIndex;
                            spanInterpolator.Next();
                        } while (--len != 0);
                    }

                    srcBufferPtr.Release();
                }

            }
        }

        static unsafe void BlendInFilterPixel(ref int accColor0, ref int accColor1, ref int accColor2, ref int accColor3,
            byte* srcBuffer, int bufferIndex, int weight)
        {
            //accColor0 = back_r;
            //accColor1 = back_g;
            //accColor2 = back_b;
            //accColor3 = back_a;
            unchecked
            {
                accColor0 += weight * srcBuffer[bufferIndex + CO.R];
                accColor1 += weight * srcBuffer[bufferIndex + CO.G];
                accColor2 += weight * srcBuffer[bufferIndex + CO.B];
                accColor3 += weight * srcBuffer[bufferIndex + CO.A];
            }
        }
    }
}


//#endif



