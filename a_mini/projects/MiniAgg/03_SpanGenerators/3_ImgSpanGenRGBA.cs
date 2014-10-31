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

        public ImgSpanGenRGBA_NN_StepXBy1(IImageBufferAccessor sourceAccessor, ISpanInterpolator spanInterpolator)
            : base(sourceAccessor, spanInterpolator, null)
        {
        }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageBase SourceRenderingBuffer = (ImageBase)GetImageBufferAccessor().SourceImage;
            if (SourceRenderingBuffer.BitDepth != 32)
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
                        span[spanIndex++] = *(ColorRGBA*)&(pSource[bufferIndex]);
                        bufferIndex += 4;
                    } while (--len != 0);
                }
            }
#else
            RGBA_Bytes color = new RGBA_Bytes();
            do
            {
                color.blue = fg_ptr[bufferIndex++];
                color.green = fg_ptr[bufferIndex++];
                color.red = fg_ptr[bufferIndex++];
                color.alpha = fg_ptr[bufferIndex++];
                span[spanIndex++] = color;
            } while (--len != 0);
#endif
        }
    }


    //==============================================span_image_filter_rgba_nn
    class ImgSpanGenRGBA_NN : ImgSpanGen
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        public ImgSpanGenRGBA_NN(IImageBufferAccessor sourceAccessor, ISpanInterpolator spanInterpolator)
            : base(sourceAccessor, spanInterpolator, null)
        {
        }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageBase SourceRenderingBuffer = (ImageBase)GetImageBufferAccessor().SourceImage;
            if (SourceRenderingBuffer.BitDepth != 32)
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
                color.alpha = fg_ptr[bufferIndex++];
                span[spanIndex] = color;
                spanIndex++;
                spanInterpolator.Next();
            } while (--len != 0);
        }
    }

    class ImgSpanGenRGBA_Bilinear : ImgSpanGen
    {
        const int base_shift = 8;
        const int base_scale = (int)(1 << base_shift);
        const int base_mask = base_scale - 1;

        public ImgSpanGenRGBA_Bilinear(IImageBufferAccessor src, ISpanInterpolator inter)
            : base(src, inter, null)
        {
        }

#if false
            public void generate(out RGBA_Bytes destPixel, int x, int y)
            {
                base.interpolator.begin(x + base.filter_dx_dbl, y + base.filter_dy_dbl, 1);

                int* fg = stackalloc int[4];

                byte* fg_ptr;

                IImage imageSource = base.source().DestImage;
                int maxx = (int)imageSource.Width() - 1;
                int maxy = (int)imageSource.Height() - 1;
                ISpanInterpolator spanInterpolator = base.interpolator;

                unchecked
                {
                    int x_hr;
                    int y_hr;

                    spanInterpolator.coordinates(out x_hr, out y_hr);

                    x_hr -= base.filter_dx_int;
                    y_hr -= base.filter_dy_int;

                    int x_lr = x_hr >> (int)img_subpix_const.image_subpixel_shift;
                    int y_lr = y_hr >> (int)img_subpix_const.image_subpixel_shift;

                    int weight;

                    fg[0] = fg[1] = fg[2] = fg[3] = (int)img_subpix_const.image_subpixel_scale * (int)img_subpix_const.image_subpixel_scale / 2;

                    x_hr &= (int)img_subpix_const.image_subpixel_mask;
                    y_hr &= (int)img_subpix_const.image_subpixel_mask;

                    fg_ptr = imageSource.GetPixelPointerY(y_lr) + (x_lr * 4);

                    weight = (int)(((int)img_subpix_const.image_subpixel_scale - x_hr) *
                             ((int)img_subpix_const.image_subpixel_scale - y_hr));
                    fg[0] += weight * fg_ptr[0];
                    fg[1] += weight * fg_ptr[1];
                    fg[2] += weight * fg_ptr[2];
                    fg[3] += weight * fg_ptr[3];

                    weight = (int)(x_hr * ((int)img_subpix_const.image_subpixel_scale - y_hr));
                    fg[0] += weight * fg_ptr[4];
                    fg[1] += weight * fg_ptr[5];
                    fg[2] += weight * fg_ptr[6];
                    fg[3] += weight * fg_ptr[7];

                    ++y_lr;
                    fg_ptr = imageSource.GetPixelPointerY(y_lr) + (x_lr * 4);

                    weight = (int)(((int)img_subpix_const.image_subpixel_scale - x_hr) * y_hr);
                    fg[0] += weight * fg_ptr[0];
                    fg[1] += weight * fg_ptr[1];
                    fg[2] += weight * fg_ptr[2];
                    fg[3] += weight * fg_ptr[3];

                    weight = (int)(x_hr * y_hr);
                    fg[0] += weight * fg_ptr[4];
                    fg[1] += weight * fg_ptr[5];
                    fg[2] += weight * fg_ptr[6];
                    fg[3] += weight * fg_ptr[7];

                    fg[0] >>= (int)img_subpix_const.image_subpixel_shift * 2;
                    fg[1] >>= (int)img_subpix_const.image_subpixel_shift * 2;
                    fg[2] >>= (int)img_subpix_const.image_subpixel_shift * 2;
                    fg[3] >>= (int)img_subpix_const.image_subpixel_shift * 2;

                    destPixel.m_R = (byte)fg[OrderR];
                    destPixel.m_G = (byte)fg[OrderG];
                    destPixel.m_B = (byte)fg[ImageBuffer.OrderB];
                    destPixel.m_A = (byte)fg[OrderA];
                }
            }
#endif

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            base.Interpolator.Begin(x + base.Dx, y + base.Dy, len);

            ImageBase srcImg = (ImageBase)base.GetImageBufferAccessor().SourceImage;
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
                    int tempA;

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
                    tempB =
                    tempA = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

                    x_hr &= (int)img_subpix_const.MASK;
                    y_hr &= (int)img_subpix_const.MASK;

                    bufferIndex = srcImg.GetBufferOffsetXY(x_lr, y_lr);

                    weight = (((int)img_subpix_const.SCALE - x_hr) *
                             ((int)img_subpix_const.SCALE - y_hr));
                    tempR += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                    tempA += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
                    bufferIndex += 4;

                    weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                    tempR += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                    tempA += weight * fg_ptr[bufferIndex + ImageBase.OrderA];

                    y_lr++;
                    bufferIndex = srcImg.GetBufferOffsetXY(x_lr, y_lr);

                    weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                    tempR += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                    tempA += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
                    bufferIndex += 4;

                    weight = (x_hr * y_hr);
                    tempR += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                    tempG += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                    tempB += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                    tempA += weight * fg_ptr[bufferIndex + ImageBase.OrderA];

                    tempR >>= (int)img_subpix_const.SHIFT * 2;
                    tempG >>= (int)img_subpix_const.SHIFT * 2;
                    tempB >>= (int)img_subpix_const.SHIFT * 2;
                    tempA >>= (int)img_subpix_const.SHIFT * 2;

                    ColorRGBA color;
                    color.red = (byte)tempR;
                    color.green = (byte)tempG;
                    color.blue = (byte)tempB;
                    color.alpha = (byte)255;// tempA;
                    span[spanIndex] = color;
                    spanIndex++;
                    spanInterpolator.Next();

                } while (--len != 0);
            }
        }
    }


    public class ImgSpanGenRGBA_BilinearClip : ImgSpanGen
    {
        ColorRGBA m_OutsideSourceColor;

        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        public ImgSpanGenRGBA_BilinearClip(IImageBufferAccessor src,
            ColorRGBA back_color, ISpanInterpolator inter)
            : base(src, inter, null)
        {
            m_OutsideSourceColor = back_color;
        }

        public ColorRGBA background_color() { return m_OutsideSourceColor; }
        public void background_color(ColorRGBA v) { m_OutsideSourceColor = v; }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageBase SourceRenderingBuffer = (ImageBase)base.GetImageBufferAccessor().SourceImage;
            int bufferIndex;
            byte[] fg_ptr;

            if (base.m_interpolator.GetType() == typeof(PixelFarm.Agg.Lines.InterpolatorLinear)
                && ((PixelFarm.Agg.Lines.InterpolatorLinear)base.m_interpolator).GetTransformer().GetType() == typeof(PixelFarm.Agg.Transform.Affine)
            && ((PixelFarm.Agg.Transform.Affine)((PixelFarm.Agg.Lines.InterpolatorLinear)base.m_interpolator).GetTransformer()).IsIdentity())
            {
                fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x, y, out bufferIndex);
                //unsafe
                {
#if true
                    do
                    {
                        span[spanIndex].blue = (byte)fg_ptr[bufferIndex++];
                        span[spanIndex].green = (byte)fg_ptr[bufferIndex++];
                        span[spanIndex].red = (byte)fg_ptr[bufferIndex++];
                        span[spanIndex].alpha = (byte)fg_ptr[bufferIndex++];
                        ++spanIndex;
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

            base.Interpolator.Begin(x + base.Dx, y + base.Dy, len);

            int[] accumulatedColor = new int[4];

            int back_r = m_OutsideSourceColor.red;
            int back_g = m_OutsideSourceColor.green;
            int back_b = m_OutsideSourceColor.blue;
            int back_a = m_OutsideSourceColor.alpha;

            int distanceBetweenPixelsInclusive = base.GetImageBufferAccessor().SourceImage.BytesBetweenPixelsInclusive;
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
                        accumulatedColor[2] =
                        accumulatedColor[3] = (int)img_subpix_const.SCALE * (int)img_subpix_const.SCALE / 2;

                        x_hr &= (int)img_subpix_const.MASK;
                        y_hr &= (int)img_subpix_const.MASK;

                        fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);

                        weight = (((int)img_subpix_const.SCALE - x_hr) *
                                 ((int)img_subpix_const.SCALE - y_hr));
                        if (weight > BASE_MASK)
                        {
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
                        }

                        weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                        if (weight > BASE_MASK)
                        {
                            bufferIndex += distanceBetweenPixelsInclusive;
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
                        }

                        weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                        if (weight > BASE_MASK)
                        {
                            ++y_lr;
                            fg_ptr = SourceRenderingBuffer.GetPixelPointerXY(x_lr, y_lr, out bufferIndex);
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
                        }
                        weight = (x_hr * y_hr);
                        if (weight > BASE_MASK)
                        {
                            bufferIndex += distanceBetweenPixelsInclusive;
                            accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                            accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                            accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                            accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
                        }
                        accumulatedColor[0] >>= (int)img_subpix_const.SHIFT * 2;
                        accumulatedColor[1] >>= (int)img_subpix_const.SHIFT * 2;
                        accumulatedColor[2] >>= (int)img_subpix_const.SHIFT * 2;
                        accumulatedColor[3] >>= (int)img_subpix_const.SHIFT * 2;
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
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);
                            }

                            x_lr++;

                            weight = (x_hr * ((int)img_subpix_const.SCALE - y_hr));
                            if (weight > BASE_MASK)
                            {
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);
                            }

                            x_lr--;
                            y_lr++;

                            weight = (((int)img_subpix_const.SCALE - x_hr) * y_hr);
                            if (weight > BASE_MASK)
                            {
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);
                            }

                            x_lr++;

                            weight = (x_hr * y_hr);
                            if (weight > BASE_MASK)
                            {
                                BlendInFilterPixel(accumulatedColor, back_r, back_g, back_b, back_a, SourceRenderingBuffer, maxx, maxy, x_lr, y_lr, weight);
                            }

                            accumulatedColor[0] >>= (int)img_subpix_const.SHIFT * 2;
                            accumulatedColor[1] >>= (int)img_subpix_const.SHIFT * 2;
                            accumulatedColor[2] >>= (int)img_subpix_const.SHIFT * 2;
                            accumulatedColor[3] >>= (int)img_subpix_const.SHIFT * 2;
                        }
                    }

                    span[spanIndex].red = (byte)accumulatedColor[0];
                    span[spanIndex].green = (byte)accumulatedColor[1];
                    span[spanIndex].blue = (byte)accumulatedColor[2];
                    span[spanIndex].alpha = (byte)accumulatedColor[3];
                    ++spanIndex;
                    spanInterpolator.Next();
                } while (--len != 0);
            }
        }

        private void BlendInFilterPixel(int[] accumulatedColor, int back_r, int back_g, int back_b, int back_a, IImage SourceRenderingBuffer, int maxx, int maxy, int x_lr, int y_lr, int weight)
        {
            byte[] fg_ptr;
            unchecked
            {
                if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                {
                    int bufferIndex = SourceRenderingBuffer.GetBufferOffsetXY(x_lr, y_lr);
                    fg_ptr = SourceRenderingBuffer.GetBuffer();

                    accumulatedColor[0] += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                    accumulatedColor[1] += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                    accumulatedColor[2] += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                    accumulatedColor[3] += weight * fg_ptr[bufferIndex + ImageBase.OrderA];
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


    public class ImgSpanGenRGBA : ImgSpanGen
    {
        const int BASE_MASK = 255;
        //--------------------------------------------------------------------
        public ImgSpanGenRGBA(IImageBufferAccessor src, ISpanInterpolator inter, ImageFilterLookUpTable filter)
            : base(src, inter, filter)
        {
            if (src.SourceImage.BytesBetweenPixelsInclusive != 4)
            {
                throw new System.NotSupportedException("span_image_filter_rgba must have a 32 bit DestImage");
            }
        }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            base.Interpolator.Begin(x + base.Dx, y + base.Dy, len);

            int f_r, f_g, f_b, f_a;

            byte[] fg_ptr;

            int diameter = filterLookup.Diameter;
            int start = filterLookup.Start;
            int[] weight_array = filterLookup.WeightArray;

            int x_count;
            int weight_y;

            ISpanInterpolator spanInterpolator = base.Interpolator;
            IImageBufferAccessor sourceAccessor = GetImageBufferAccessor();

            do
            {
                spanInterpolator.GetCoord(out x, out y);

                x -= base.DxInt;
                y -= base.DyInt;

                int x_hr = x;
                int y_hr = y;

                int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
                int y_lr = y_hr >> (int)img_subpix_const.SHIFT;

                f_b = f_g = f_r = f_a = (int)img_filter_const.SCALE / 2;

                int x_fract = x_hr & (int)img_subpix_const.MASK;
                int y_count = diameter;

                y_hr = (int)img_subpix_const.MASK - (y_hr & (int)img_subpix_const.MASK);

                int bufferIndex;
                fg_ptr = sourceAccessor.span(x_lr + start, y_lr + start, diameter, out bufferIndex);
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

                        f_b += weight * fg_ptr[bufferIndex + ImageBase.OrderR];
                        f_g += weight * fg_ptr[bufferIndex + ImageBase.OrderG];
                        f_r += weight * fg_ptr[bufferIndex + ImageBase.OrderB];
                        f_a += weight * fg_ptr[bufferIndex + ImageBase.OrderA];

                        if (--x_count == 0) break;
                        x_hr += (int)img_subpix_const.SCALE;
                        sourceAccessor.next_x(out bufferIndex);
                    }

                    if (--y_count == 0) break;
                    y_hr += (int)img_subpix_const.SCALE;
                    fg_ptr = sourceAccessor.next_y(out bufferIndex);
                }

                f_b >>= (int)img_filter_const.SHIFT;
                f_g >>= (int)img_filter_const.SHIFT;
                f_r >>= (int)img_filter_const.SHIFT;
                f_a >>= (int)img_filter_const.SHIFT;

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

                    if ((uint)f_a > BASE_MASK)
                    {
                        if (f_a < 0) f_a = 0;
                        if (f_a > BASE_MASK) f_a = (int)BASE_MASK;
                    }
                }

                span[spanIndex].red = (byte)f_b;
                span[spanIndex].green = (byte)f_g;
                span[spanIndex].blue = (byte)f_r;
                span[spanIndex].alpha = (byte)f_a;

                spanIndex++;
                spanInterpolator.Next();

            } while (--len != 0);
        }
    }



}


//#endif



