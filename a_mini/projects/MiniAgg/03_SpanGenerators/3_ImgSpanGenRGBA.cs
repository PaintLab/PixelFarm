//2014 BSD,WinterDev   
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


using PixelFarm.VectorMath;
using PixelFarm.Agg.Lines;

using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using img_filter_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgFilterConst;


namespace PixelFarm.Agg.Image
{
    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
    class ImgSpanGenRGBA_NN_StepXBy1 : ImgSpanGen
    {
        const int BASE_SHITF = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHITF);
        const int BASE_MASK = BASE_SCALE - 1;

        ImageReaderWriterBase srcRW;
        public ImgSpanGenRGBA_NN_StepXBy1(IImageReaderWriter src,
             ISpanInterpolator spanInterpolator)
            : base(src, spanInterpolator)
        {
            srcRW = (ImageReaderWriterBase)ImgBuffAccessor.SourceImage;
            if (srcRW.BitDepth != 32)
            {
                throw new NotSupportedException("The source is expected to be 32 bit.");
            }
        }

        public override void GenerateColors(ColorRGBA[] outputColors, int startIndex, int x, int y, int len)
        {

            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + dx, y + dy, len);
            int x_hr;
            int y_hr;
            spanInterpolator.GetCoord(out x_hr, out y_hr);
            int x_lr = x_hr >>  img_subpix_const.SHIFT;
            int y_lr = y_hr >>  img_subpix_const.SHIFT;

            int bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);

            byte[] fg_ptr = srcRW.GetBuffer();

            unsafe
            {
                fixed (byte* pSource = fg_ptr)
                {
                    do
                    {
                        outputColors[startIndex++] = *(ColorRGBA*)&(pSource[bufferIndex]);
                        bufferIndex += 4;
                    } while (--len != 0);
                }
            }

        }
    }



    public class ImgSpanGenRGBA_BilinearClip : ImgSpanGen
    {


        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        ImageReaderWriterBase srcRW;
        ColorRGBA m_bgcolor;
        int bytesBetweenPixelInclusive;
        public ImgSpanGenRGBA_BilinearClip(IImageReaderWriter src,
            ColorRGBA back_color,
            ISpanInterpolator inter)
            : base(src, inter)
        {
            m_bgcolor = back_color;
            srcRW = (ImageReaderWriterBase)base.ImgBuffAccessor.SourceImage;
            bytesBetweenPixelInclusive = srcRW.BytesBetweenPixelsInclusive;

        }
        public ColorRGBA BackgroundColor
        {
            get { return this.m_bgcolor; }
            set { this.m_bgcolor = value; }
        }

        public override void GenerateColors(ColorRGBA[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = base.Interpolator;
            int bufferIndex;
            byte[] fg_ptr;
            if (spanInterpolator.GetType() == typeof(PixelFarm.Agg.Transform.SpanInterpolatorLinear)
                && ((PixelFarm.Agg.Transform.SpanInterpolatorLinear)spanInterpolator).Transformer.GetType() == typeof(PixelFarm.Agg.Transform.Affine)
            && ((PixelFarm.Agg.Transform.Affine)((PixelFarm.Agg.Transform.SpanInterpolatorLinear)spanInterpolator).Transformer).IsIdentity())
            {
                fg_ptr = srcRW.GetPixelPointerXY(x, y, out bufferIndex);
                //unsafe
                {
#if true
                    do
                    {
                        outputColors[startIndex].blue = (byte)fg_ptr[bufferIndex++];
                        outputColors[startIndex].green = (byte)fg_ptr[bufferIndex++];
                        outputColors[startIndex].red = (byte)fg_ptr[bufferIndex++];
                        outputColors[startIndex].alpha = (byte)fg_ptr[bufferIndex++];
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

                return;
            }

            spanInterpolator.Begin(x + base.dx, y + base.dy, len);

            int[] accumulatedColor = new int[4];

            int back_r = m_bgcolor.red;
            int back_g = m_bgcolor.green;
            int back_b = m_bgcolor.blue;
            int back_a = m_bgcolor.alpha;

            int maxx = (int)srcRW.Width - 1;
            int maxy = (int)srcRW.Height - 1;


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
                        accumulatedColor[0] =
                        accumulatedColor[1] =
                        accumulatedColor[2] =
                        accumulatedColor[3] = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

                        x_hr &= (int)img_subpix_const.MASK;
                        y_hr &= (int)img_subpix_const.MASK;

                        fg_ptr = srcRW.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                        weight = (((int)img_subpix_const.SCALE - x_hr) *
                                 ((int)img_subpix_const.SCALE - y_hr));
                        if (weight > BASE_MASK)
                        {
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderA];
                        }

                        weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                        if (weight > BASE_MASK)
                        {
                            bufferIndex += bytesBetweenPixelInclusive;
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderA];
                        }

                        weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                        if (weight > BASE_MASK)
                        {
                            ++y_lr;
                            fg_ptr = srcRW.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderA];
                        }
                        weight = (x_hr * y_hr);
                        if (weight > BASE_MASK)
                        {
                            bufferIndex += bytesBetweenPixelInclusive;
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderA];
                        }
                        accumulatedColor[0] >>= img_subpix_const.SHIFT * 2;
                        accumulatedColor[1] >>= img_subpix_const.SHIFT * 2;
                        accumulatedColor[2] >>= img_subpix_const.SHIFT * 2;
                        accumulatedColor[3] >>= img_subpix_const.SHIFT * 2;
                    }
                    else
                    {
                        if (x_lr < -1 || y_lr < -1 ||
                           x_lr > maxx || y_lr > maxy)
                        {
                            accumulatedColor[0] = back_r;
                            accumulatedColor[1] = back_g;
                            accumulatedColor[2] = back_b;
                            accumulatedColor[3] = back_a;
                        }
                        else
                        {
                            accumulatedColor[0] =
                            accumulatedColor[1] =
                            accumulatedColor[2] =
                            accumulatedColor[3] = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

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
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);
                            }

                            x_lr--;
                            y_lr++;

                            weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                            if (weight > BASE_MASK)
                            {
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);
                            }

                            x_lr++;

                            weight = (x_hr * y_hr);
                            if (weight > BASE_MASK)
                            {
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);
                            }

                            accumulatedColor[0] >>= img_subpix_const.SHIFT * 2;
                            accumulatedColor[1] >>= img_subpix_const.SHIFT * 2;
                            accumulatedColor[2] >>= img_subpix_const.SHIFT * 2;
                            accumulatedColor[3] >>= img_subpix_const.SHIFT * 2;
                        }
                    }

                    outputColors[startIndex].red = (byte)accumulatedColor[0];
                    outputColors[startIndex].green = (byte)accumulatedColor[1];
                    outputColors[startIndex].blue = (byte)accumulatedColor[2];
                    outputColors[startIndex].alpha = (byte)accumulatedColor[3];
                    ++startIndex;
                    spanInterpolator.Next();
                } while (--len != 0);
            }
        }

        void BlendInFilterPixel(int[] accumulatedColor, int back_r, int back_g, int back_b, int back_a,
            IImageReaderWriter SourceRenderingBuffer, int maxx, int maxy, int x_lr, int y_lr, int weight)
        {
            byte[] fg_ptr;
            unchecked
            {
                if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                {
                    int bufferIndex = SourceRenderingBuffer.GetBufferOffsetXY(x_lr, y_lr);
                    fg_ptr = SourceRenderingBuffer.GetBuffer();

                    accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                    accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                    accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                    accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderA];
                }
                else
                {
                    accumulatedColor[0] += back_r * weight;
                    accumulatedColor[1] += back_g * weight;
                    accumulatedColor[2] += back_b * weight;
                    accumulatedColor[3] += back_a * weight;
                }
            }
        }
    }





}


//#endif



