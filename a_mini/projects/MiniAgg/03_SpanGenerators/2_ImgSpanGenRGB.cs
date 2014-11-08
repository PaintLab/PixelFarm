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
using System;

using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using img_filter_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgFilterConst;

namespace PixelFarm.Agg.Image
{
    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
    class ImgSpanGenRGB_NNStepXby1 : ImgSpanGen
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        ImageReaderWriterBase srcRW;
        public ImgSpanGenRGB_NNStepXby1(IImageReaderWriter src, ISpanInterpolator spanInterpolator)
            : base(src, spanInterpolator)
        {
            this.srcRW = (ImageReaderWriterBase)ImgBuffAccessor.SourceImage;
            if (srcRW.BitDepth != 24)
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
            int x_lr = x_hr >> img_subpix_const.SHIFT;
            int y_lr = y_hr >> img_subpix_const.SHIFT;
            int bufferIndex;
            bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);

            byte[] fg_ptr = srcRW.GetBuffer();
#if USE_UNSAFE_CODE
            unsafe
            {
                fixed (byte* pSource = fg_ptr)
                {
                    do
                    {
                        span[spanIndex++] = *(RGBA_Bytes*)&(pSource[bufferIndex]);
                        bufferIndex += 4;
                    } while (--len != 0);
                }
            }
#else
            ColorRGBA color = ColorRGBA.White;
            do
            {
                color.blue = fg_ptr[bufferIndex++];
                color.green = fg_ptr[bufferIndex++];
                color.red = fg_ptr[bufferIndex++];
                outputColors[startIndex++] = color;
            } while (--len != 0);
#endif
        }
    }




    //=====================================span_image_filter_rgb_bilinear_clip
    class ImgSpanGenRGB_BilinearClip : ImgSpanGen
    {
        ColorRGBA m_bgcolor;

        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        ImageReaderWriterBase srcRW;
        //--------------------------------------------------------------------
        public ImgSpanGenRGB_BilinearClip(IImageReaderWriter src,
                                          ColorRGBA back_color,
                                          ISpanInterpolator inter)
            : base(src, inter)
        {
            m_bgcolor = back_color;
            srcRW = (ImageReaderWriterBase)base.ImgBuffAccessor.SourceImage;
        }

        public ColorRGBA BackgroundColor
        {
            get { return this.m_bgcolor; }
            set { this.m_bgcolor = value; }
        }
        public override void GenerateColors(ColorRGBA[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = base.Interpolator;
            spanInterpolator.Begin(x + base.dx, y + base.dy, len);

            int[] accumulatedColor = new int[3];
            int sourceAlpha;
            int back_r = m_bgcolor.red;
            int back_g = m_bgcolor.green;
            int back_b = m_bgcolor.blue;
            int back_a = m_bgcolor.alpha;

            int bufferIndex;
            byte[] fg_ptr;


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
                        accumulatedColor[2] = img_subpix_const.SCALE * img_subpix_const.SCALE / 2;

                        x_hr &= img_subpix_const.MASK;
                        y_hr &= img_subpix_const.MASK;

                        fg_ptr = srcRW.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                        weight = ((img_subpix_const.SCALE - x_hr) *
                                 (img_subpix_const.SCALE - y_hr));
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        bufferIndex += 3;
                        weight = (x_hr * (img_subpix_const.SCALE - y_hr));
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        y_lr++;
                        fg_ptr = srcRW.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                        weight = ((img_subpix_const.SCALE - x_hr) * y_hr);
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        bufferIndex += 3;
                        weight = (x_hr * y_hr);
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        accumulatedColor[0] >>= img_subpix_const.SHIFT * 2;
                        accumulatedColor[1] >>= img_subpix_const.SHIFT * 2;
                        accumulatedColor[2] >>= img_subpix_const.SHIFT * 2;

                        sourceAlpha = BASE_MASK;
                    }
                    else
                    {
                        if (x_lr < -1 || y_lr < -1 ||
                           x_lr > maxx || y_lr > maxy)
                        {
                            accumulatedColor[0] = back_r;
                            accumulatedColor[1] = back_g;
                            accumulatedColor[2] = back_b;
                            sourceAlpha = back_a;
                        }
                        else
                        {
                            accumulatedColor[0] =
                            accumulatedColor[1] =
                            accumulatedColor[2] = img_subpix_const.SCALE * img_subpix_const.SCALE / 2;
                            sourceAlpha = img_subpix_const.SCALE * img_subpix_const.SCALE / 2;

                            x_hr &= img_subpix_const.MASK;
                            y_hr &= img_subpix_const.MASK;

                            weight = ((img_subpix_const.SCALE - x_hr) *
                                     (img_subpix_const.SCALE - y_hr));
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);

                            x_lr++;

                            weight = (x_hr * (img_subpix_const.SCALE - y_hr));
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);

                            x_lr--;
                            y_lr++;

                            weight = ((img_subpix_const.SCALE - x_hr) * y_hr);
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);

                            x_lr++;

                            weight = (x_hr * y_hr);
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, srcRW, maxx, maxy, x_lr, y_lr, weight);

                            accumulatedColor[0] >>= img_subpix_const.SHIFT * 2;
                            accumulatedColor[1] >>= img_subpix_const.SHIFT * 2;
                            accumulatedColor[2] >>= img_subpix_const.SHIFT * 2;
                            sourceAlpha >>= img_subpix_const.SHIFT * 2;
                        }
                    }

                    outputColors[startIndex].red = (byte)accumulatedColor[0];
                    outputColors[startIndex].green = (byte)accumulatedColor[1];
                    outputColors[startIndex].blue = (byte)accumulatedColor[2];
                    outputColors[startIndex].alpha = (byte)sourceAlpha;
                    startIndex++;
                    spanInterpolator.Next();
                } while (--len != 0);
            }
        }

        private void BlendInFilterPixel(int[] accumulatedColor, ref int sourceAlpha, int back_r, int back_g, int back_b, int back_a, ImageReaderWriterBase SourceRenderingBuffer, int maxx, int maxy, int x_lr, int y_lr, int weight)
        {
            byte[] fg_ptr;
            unchecked
            {
                if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                {
                    int bufferIndex;
                    fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                    accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                    accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                    accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                    sourceAlpha += weight * BASE_MASK;
                }
                else
                {
                    accumulatedColor[0] += back_r * weight;
                    accumulatedColor[1] += back_g * weight;
                    accumulatedColor[2] += back_b * weight;
                    sourceAlpha += back_a * weight;
                }
            }
        }
    };




}
