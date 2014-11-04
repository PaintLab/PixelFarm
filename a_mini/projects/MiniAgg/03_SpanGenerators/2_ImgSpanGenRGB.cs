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

        public ImgSpanGenRGB_NNStepXby1(IImageBufferAccessor sourceAccessor, ISpanInterpolator spanInterpolator)
            : base(sourceAccessor, spanInterpolator, null)
        {
        }

        public override void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageReaderWriterBase SourceRenderingBuffer = (ImageReaderWriterBase)ImgBuffAccessor.SourceImage;
            if (SourceRenderingBuffer.BitDepth != 24)
            {
                throw new NotSupportedException("The source is expected to be 32 bit.");
            }
            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + Dx, y + Dy, len);
            int x_hr;
            int y_hr;
            spanInterpolator.GetCoord(out x_hr, out y_hr);
            int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
            int y_lr = y_hr >> (int)img_subpix_const.SHIFT;
            int bufferIndex;
            bufferIndex = SourceRenderingBuffer.GetBufferOffsetXY(x_lr, y_lr);

            byte[] fg_ptr = SourceRenderingBuffer.GetBuffer();
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
                span[spanIndex++] = color;
            } while (--len != 0);
#endif
        }
    }

    //===============================================span_image_filter_rgb_nn
    class ImgSpanGenRGB_NN : ImgSpanGen
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        //--------------------------------------------------------------------
        public ImgSpanGenRGB_NN(IImageBufferAccessor src, ISpanInterpolator inter)
            : base(src, inter, null)
        {
        }

        public override void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageReaderWriterBase SourceRenderingBuffer = (ImageReaderWriterBase)ImgBuffAccessor.SourceImage;
            if (SourceRenderingBuffer.BitDepth != 24)
            {
                throw new NotSupportedException("The source is expected to be 32 bit.");
            }
            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + Dx, y + Dy, len);

            byte[] fg_ptr = SourceRenderingBuffer.GetBuffer();
            do
            {
                int x_hr;
                int y_hr;
                spanInterpolator.GetCoord(out x_hr, out y_hr);
                int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
                int y_lr = y_hr >> (int)img_subpix_const.SHIFT;
                int bufferIndex;
                bufferIndex = SourceRenderingBuffer.GetBufferOffsetXY(x_lr, y_lr);
                ColorRGBA color;
                color.blue = fg_ptr[bufferIndex++];
                color.green = fg_ptr[bufferIndex++];
                color.red = fg_ptr[bufferIndex++];
                color.alpha = 255;
                span[spanIndex] = color;
                spanIndex++;
                spanInterpolator.Next();
            } while (--len != 0);
        }
    };

    //==========================================span_image_filter_rgb_bilinear
    class ImgSpanGenRGB_Bilinear : ImgSpanGen
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        //--------------------------------------------------------------------
        public ImgSpanGenRGB_Bilinear(IImageBufferAccessor src,
                                            ISpanInterpolator inter)
            : base(src, inter, null)
        {
            if (src.SourceImage.BytesBetweenPixelsInclusive != 3)
            {
                throw new System.NotSupportedException("span_image_filter_rgb must have a 24 bit DestImage");
            }
        }

        public override void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            base.Interpolator.Begin(x + base.Dx, y + base.Dy, len);

            ImageReaderWriterBase srcImg = (ImageReaderWriterBase)base.ImgBuffAccessor.SourceImage;
            ISpanInterpolator spanInterpolator = base.Interpolator;
            int bufferIndex = 0;
            byte[] fg_ptr = srcImg.GetBuffer();
            unchecked
            {
                do
                {
                    int tempR;
                    int tempG;
                    int tempB;

                    int x_hr;
                    int y_hr;

                    spanInterpolator.GetCoord(out x_hr, out y_hr);

                    x_hr -= base.DxInt;
                    y_hr -= base.DyInt;

                    int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
                    int y_lr = y_hr >> (int)img_subpix_const.SHIFT;
                    int weight;

                    tempR =
                    tempG =
                    tempB = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

                    x_hr &= (int)img_subpix_const.MASK;
                    y_hr &= (int)img_subpix_const.MASK;

                    bufferIndex = srcImg.GetBufferOffsetXY(x_lr, y_lr);

                    weight = (((int)img_subpix_const.SCALE - x_hr) *
                             ((int)img_subpix_const.SCALE - y_hr));
                    tempR += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                    bufferIndex += 3;

                    weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                    tempR += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                    y_lr++;
                    bufferIndex = srcImg.GetBufferOffsetXY(x_lr, y_lr);

                    weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                    tempR += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];
                    bufferIndex += 3;

                    weight = (x_hr * y_hr);
                    tempR += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                    tempR >>= (int)img_subpix_const.SHIFT * 2;
                    tempG >>= (int)img_subpix_const.SHIFT * 2;
                    tempB >>= (int)img_subpix_const.SHIFT * 2;

                    ColorRGBA color;
                    color.red = (byte)tempR;
                    color.green = (byte)tempG;
                    color.blue = (byte)tempB;
                    color.alpha = 255;
                    span[spanIndex] = color;
                    spanIndex++;
                    spanInterpolator.Next();

                } while (--len != 0);
            }
        }

        private void BlendInFilterPixel(int[] fg, ref int src_alpha, int back_r, int back_g, int back_b, int back_a, ImageReaderWriterBase SourceRenderingBuffer, int maxx, int maxy, int x_lr, int y_lr, int weight)
        {
            throw new NotImplementedException(); /*
            int[] fg_ptr;
            int bufferIndex;
            unchecked
            {
                if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                {
                    fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                    fg[0] += (weight * (fg_ptr[bufferIndex] & (int)RGBA_Bytes.m_R) >> (int)RGBA_Bytes.Shift.R);
                    fg[1] += (weight * (fg_ptr[bufferIndex] & (int)RGBA_Bytes.m_G) >> (int)RGBA_Bytes.Shift.G);
                    fg[2] += (weight * (fg_ptr[bufferIndex] & (int)RGBA_Bytes.m_G) >> (int)RGBA_Bytes.Shift.B);
                    src_alpha += weight * base_mask;
                }
                else
                {
                    fg[0] += (weight * back_r);
                    fg[1] += (weight * back_g);
                    fg[2] += (weight * back_b);
                    src_alpha += back_a * weight;
                }
            }
                                                      */
        }
    }

    //=====================================span_image_filter_rgb_bilinear_clip
    class ImgSpanGenRGB_BilinearClip : ImgSpanGen
    {
        ColorRGBA m_OutsideSourceColor;

        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        //--------------------------------------------------------------------
        public ImgSpanGenRGB_BilinearClip(IImageBufferAccessor src,
                                            ColorRGBA back_color,
                                            ISpanInterpolator inter)
            : base(src, inter, null)
        {
            m_OutsideSourceColor = back_color;
        }
        public ColorRGBA background_color() { return m_OutsideSourceColor; }
        public void background_color(ColorRGBA v) { m_OutsideSourceColor = v; }

        public override void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            base.Interpolator.Begin(x + base.Dx, y + base.Dy, len);

            int[] accumulatedColor = new int[3];
            int sourceAlpha;

            int back_r = m_OutsideSourceColor.red;
            int back_g = m_OutsideSourceColor.green;
            int back_b = m_OutsideSourceColor.blue;
            int back_a = m_OutsideSourceColor.alpha;

            int bufferIndex;
            byte[] fg_ptr;

            ImageReaderWriterBase SourceRenderingBuffer = (ImageReaderWriterBase)base.ImgBuffAccessor.SourceImage;
            int maxx = (int)SourceRenderingBuffer.Width - 1;
            int maxy = (int)SourceRenderingBuffer.Height - 1;
            ISpanInterpolator spanInterpolator = base.Interpolator;

            unchecked
            {
                do
                {
                    int x_hr;
                    int y_hr;

                    spanInterpolator.GetCoord(out x_hr, out y_hr);

                    x_hr -= base.DxInt;
                    y_hr -= base.DyInt;

                    int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
                    int y_lr = y_hr >> (int)img_subpix_const.SHIFT;
                    int weight;

                    if (x_lr >= 0 && y_lr >= 0 &&
                       x_lr < maxx && y_lr < maxy)
                    {
                        accumulatedColor[0] =
                        accumulatedColor[1] =
                        accumulatedColor[2] = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

                        x_hr &= (int)img_subpix_const.MASK;
                        y_hr &= (int)img_subpix_const.MASK;

                        fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                        weight = (((int)img_subpix_const.SCALE - x_hr) *
                                 ((int)img_subpix_const.SCALE - y_hr));
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        bufferIndex += 3;
                        weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        y_lr++;
                        fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                        weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        bufferIndex += 3;
                        weight = (x_hr * y_hr);
                        accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        accumulatedColor[0] >>= (int)img_subpix_const.SHIFT * 2;
                        accumulatedColor[1] >>= (int)img_subpix_const.SHIFT * 2;
                        accumulatedColor[2] >>= (int)img_subpix_const.SHIFT * 2;

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
                            accumulatedColor[2] = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;
                            sourceAlpha = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

                            x_hr &= (int)img_subpix_const.MASK;
                            y_hr &= (int)img_subpix_const.MASK;

                            weight = (((int)img_subpix_const.SCALE - x_hr) *
                                     ((int)img_subpix_const.SCALE - y_hr));
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);

                            x_lr++;

                            weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);

                            x_lr--;
                            y_lr++;

                            weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);

                            x_lr++;

                            weight = (x_hr * y_hr);
                            BlendInFilterPixel(accumulatedColor, ref sourceAlpha, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);

                            accumulatedColor[0] >>= (int)img_subpix_const.SHIFT * 2;
                            accumulatedColor[1] >>= (int)img_subpix_const.SHIFT * 2;
                            accumulatedColor[2] >>= (int)img_subpix_const.SHIFT * 2;
                            sourceAlpha >>= (int)img_subpix_const.SHIFT * 2;
                        }
                    }

                    span[spanIndex].red = (byte)accumulatedColor[0];
                    span[spanIndex].green = (byte)accumulatedColor[1];
                    span[spanIndex].blue = (byte)accumulatedColor[2];
                    span[spanIndex].alpha = (byte)sourceAlpha;
                    spanIndex++;
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

    //===================================================span_image_filter_rgb
    class ImgSpanGenRGB : ImgSpanGen
    {
        const int BASE_MASK = 255;
        //--------------------------------------------------------------------
        public ImgSpanGenRGB(IImageBufferAccessor src, ISpanInterpolator inter, ImageFilterLookUpTable filter)
            : base(src, inter, filter)
        {
            if (src.SourceImage.BytesBetweenPixelsInclusive != 3)
            {
                throw new System.NotSupportedException("span_image_filter_rgb must have a 24 bit DestImage");
            }
        }

        public override void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            base.Interpolator.Begin(x + base.Dx, y + base.Dy, len);

            int f_r, f_g, f_b;

            byte[] fg_ptr;

            int diameter = filterLookup.Diameter;
            int start = filterLookup.Start;
            int[] weight_array = filterLookup.WeightArray;

            int x_count;
            int weight_y;

            ISpanInterpolator spanInterpolator = base.Interpolator;

            do
            {
                spanInterpolator.GetCoord(out x, out y);

                x -= base.DxInt;
                y -= base.DyInt;

                int x_hr = x;
                int y_hr = y;

                int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
                int y_lr = y_hr >> (int)img_subpix_const.SHIFT;

                f_b = f_g = f_r = (int)img_filter_const.SCALE / 2;

                int x_fract = x_hr & (int)img_subpix_const.MASK;
                int y_count = diameter;

                y_hr = (int)img_subpix_const.MASK - (y_hr & (int)img_subpix_const.MASK);

                int bufferIndex;
                fg_ptr = ImgBuffAccessor.GetSpan(x_lr + start, y_lr + start, diameter, out bufferIndex);
                for (; ; )
                {
                    x_count = (int)diameter;
                    weight_y = weight_array[y_hr];
                    x_hr = (int)img_subpix_const.MASK - x_fract;
                    for (; ; )
                    {
                        int weight = (weight_y * weight_array[x_hr] +
                                     (int)img_filter_const.SCALE / 2) >>
                                     (int)img_filter_const.SHIFT;

                        f_b += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderR];
                        f_g += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderG];
                        f_r += weight * fg_ptr[bufferIndex + ImageReaderWriterBase.OrderB];

                        if (--x_count == 0) break;
                        x_hr += (int)img_subpix_const.SCALE;
                        ImgBuffAccessor.NextX(out bufferIndex);
                    }

                    if (--y_count == 0) break;
                    y_hr += (int)img_subpix_const.SCALE;
                    fg_ptr = ImgBuffAccessor.NextY(out bufferIndex);
                }

                f_b >>= (int)img_filter_const.SHIFT;
                f_g >>= (int)img_filter_const.SHIFT;
                f_r >>= (int)img_filter_const.SHIFT;

                unchecked
                {
                    if ((uint)f_b > BASE_MASK)
                    {
                        if (f_b < 0) f_b = 0;
                        if (f_b > BASE_MASK) f_b = (int)BASE_MASK;
                    }

                    if ((uint)f_g > BASE_MASK)
                    {
                        if (f_g < 0) f_g = 0;
                        if (f_g > BASE_MASK) f_g = (int)BASE_MASK;
                    }

                    if ((uint)f_r > BASE_MASK)
                    {
                        if (f_r < 0) f_r = 0;
                        if (f_r > BASE_MASK) f_r = (int)BASE_MASK;
                    }
                }

                span[spanIndex].alpha = (byte)BASE_MASK;
                span[spanIndex].red = (byte)f_b;
                span[spanIndex].green = (byte)f_g;
                span[spanIndex].blue = (byte)f_r;

                spanIndex++;
                spanInterpolator.Next();

            } while (--len != 0);
        }
    };

    //===============================================span_image_filter_rgb_2x2
    class ImgSpanGenRGB_2x2 : ImgSpanGen
    {
        const int BASE_MASK = 255;

        //--------------------------------------------------------------------
        public ImgSpanGenRGB_2x2(IImageBufferAccessor src, ISpanInterpolator inter, ImageFilterLookUpTable filter)
            : base(src, inter, filter)
        {
        }

        public override void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            throw new NotImplementedException(); /*
            ISpanInterpolator spanInterpolator = base.interpolator;
            spanInterpolator.begin(x + base.filter_dx_dbl(), y + base.filter_dy_dbl(), len);

            int[] fg = new int[3];

            int[] fg_ptr;
            int bufferIndex;
            int[] weight_array = filter().weight_array();
            int weightArrayIndex = ((filter().diameter() / 2 - 1) << (int)img_subpix_const.image_subpixel_shift);

            do
            {
                int x_hr;
                int y_hr;

                spanInterpolator.coordinates(out x_hr, out y_hr);

                x_hr -= filter_dx_int();
                y_hr -= filter_dy_int();

                int x_lr = x_hr >> (int)img_subpix_const.image_subpixel_shift;
                int y_lr = y_hr >> (int)img_subpix_const.image_subpixel_shift;

                int weight;
                fg[0] = fg[1] = fg[2] = (int)img_filter_const.image_filter_scale / 2;

                x_hr &= (int)img_subpix_const.image_subpixel_mask;
                y_hr &= (int)img_subpix_const.image_subpixel_mask;

                fg_ptr = source().span(x_lr, y_lr, 2, out bufferIndex);
                weight = ((weight_array[x_hr + (int)img_subpix_const.image_subpixel_scale] *
                          weight_array[y_hr + (int)img_subpix_const.image_subpixel_scale] +
                          (int)img_filter_const.image_filter_scale / 2) >>
                          (int)img_filter_const.image_filter_shift);
                fg[0] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderR];
                fg[1] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderG];
                fg[2] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderB];

                fg_ptr = source().next_x(out bufferIndex);
                weight = ((weight_array[x_hr] *
                          weight_array[y_hr + (int)img_subpix_const.image_subpixel_scale] +
                          (int)img_filter_const.image_filter_scale / 2) >>
                          (int)img_filter_const.image_filter_shift);
                fg[0] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderR];
                fg[1] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderG];
                fg[2] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderB];

                fg_ptr = source().next_y(out bufferIndex);
                weight = ((weight_array[x_hr + (int)img_subpix_const.image_subpixel_scale] *
                          weight_array[y_hr] +
                          (int)img_filter_const.image_filter_scale / 2) >>
                          (int)img_filter_const.image_filter_shift);
                fg[0] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderR];
                fg[1] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderG];
                fg[2] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderB];

                fg_ptr = source().next_x(out bufferIndex);
                weight = ((weight_array[x_hr] *
                          weight_array[y_hr] +
                          (int)img_filter_const.image_filter_scale / 2) >>
                          (int)img_filter_const.image_filter_shift);
                fg[0] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderR];
                fg[1] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderG];
                fg[2] += weight * fg_ptr[bufferIndex + ImageBuffer.OrderB];

                fg[0] >>= (int)img_filter_const.image_filter_shift;
                fg[1] >>= (int)img_filter_const.image_filter_shift;
                fg[2] >>= (int)img_filter_const.image_filter_shift;

                if (fg[0] > base_mask) fg[0] = (int)base_mask;
                if (fg[1] > base_mask) fg[1] = (int)base_mask;
                if (fg[2] > base_mask) fg[2] = (int)base_mask;

                span[spanIndex].m_ARGBData = base_mask << (int)RGBA_Bytes.Shift.A | fg[0] << (int)RGBA_Bytes.Shift.R | fg[1] << (int)RGBA_Bytes.Shift.G | fg[2] << (int)RGBA_Bytes.Shift.B;

                spanIndex++;
                spanInterpolator.Next();

            } while (--len != 0);
                                                      */
        }
    }



   
}
