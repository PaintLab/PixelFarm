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
using PixelFarm.Agg.Image;

using image_subpixel_scale_e = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using image_filter_scale_e = PixelFarm.Agg.ImageFilterLookUpTable.ImgFilterConst;

namespace PixelFarm.Agg.Image
{

    //=================================================span_image_resample_rgb
    class FilterRGBImageSpanGen : FilterImageSpanGenerator
    {
        const int BASE_MASK = 255;
        const int DOWNSCALE_SHIFT = (int)ImageFilterLookUpTable.ImgFilterConst.SHIFT;

        //--------------------------------------------------------------------
        public FilterRGBImageSpanGen(IImageBufferAccessor src,
                            ISpanInterpolator inter,
                            ImageFilterLookUpTable filter) :
            base(src, inter, filter)
        {
            if (src.SourceImage.GetRecieveBlender().NumPixelBits != 24)
            {
                throw new System.FormatException("You have to use a rgb blender with span_image_resample_rgb");
            }
        }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = base.interpolator();
            spanInterpolator.Begin(x + base.filter_dx_dbl(), y + base.filter_dy_dbl(), len);

            int[] fg = new int[3];

            byte[] fg_ptr;
            int[] weightArray = filter().WeightArray;
            int diameter = (int)base.filter().Diameter;
            int filter_scale = diameter << (int)image_subpixel_scale_e.SHIFT;

            int[] weight_array = weightArray;

            do
            {
                int rx;
                int ry;
                int rx_inv = (int)image_subpixel_scale_e.SCALE;
                int ry_inv = (int)image_subpixel_scale_e.SCALE;
                spanInterpolator.GetCoord(out x, out y);
                spanInterpolator.GetLocalScale(out rx, out ry);
                base.AdjustScale(ref rx, ref ry);

                rx_inv = (int)image_subpixel_scale_e.SCALE * (int)image_subpixel_scale_e.SCALE / rx;
                ry_inv = (int)image_subpixel_scale_e.SCALE * (int)image_subpixel_scale_e.SCALE / ry;

                int radius_x = (diameter * rx) >> 1;
                int radius_y = (diameter * ry) >> 1;
                int len_x_lr =
                    (diameter * rx + (int)image_subpixel_scale_e.MASK) >>
                        (int)(int)image_subpixel_scale_e.SHIFT;

                x += base.filter_dx_int() - radius_x;
                y += base.filter_dy_int() - radius_y;

                fg[0] = fg[1] = fg[2] = (int)image_filter_scale_e.SCALE / 2;

                int y_lr = y >> (int)(int)image_subpixel_scale_e.SHIFT;
                int y_hr = (((int)image_subpixel_scale_e.MASK - (y & (int)image_subpixel_scale_e.MASK)) *
                               ry_inv) >> (int)(int)image_subpixel_scale_e.SHIFT;
                int total_weight = 0;
                int x_lr = x >> (int)(int)image_subpixel_scale_e.SHIFT;
                int x_hr = (((int)image_subpixel_scale_e.MASK - (x & (int)image_subpixel_scale_e.MASK)) *
                               rx_inv) >> (int)(int)image_subpixel_scale_e.SHIFT;
                int x_hr2 = x_hr;
                int sourceIndex;
                fg_ptr = base.GetImageBufferAccessor().span(x_lr, y_lr, len_x_lr, out sourceIndex);

                for (; ; )
                {
                    int weight_y = weight_array[y_hr];
                    x_hr = x_hr2;
                    for (; ; )
                    {
                        int weight = (weight_y * weight_array[x_hr] +
                                     (int)image_filter_scale_e.SCALE / 2) >>
                                     DOWNSCALE_SHIFT;
                        fg[0] += fg_ptr[sourceIndex + ImageBase.OrderR] * weight;
                        fg[1] += fg_ptr[sourceIndex + ImageBase.OrderG] * weight;
                        fg[2] += fg_ptr[sourceIndex + ImageBase.OrderB] * weight;
                        total_weight += weight;
                        x_hr += rx_inv;
                        if (x_hr >= filter_scale) break;
                        fg_ptr = base.GetImageBufferAccessor().next_x(out sourceIndex);
                    }
                    y_hr += ry_inv;
                    if (y_hr >= filter_scale)
                    {
                        break;
                    }

                    fg_ptr = base.GetImageBufferAccessor().next_y(out sourceIndex);
                }

                fg[0] /= total_weight;
                fg[1] /= total_weight;
                fg[2] /= total_weight;

                if (fg[0] < 0) fg[0] = 0;
                if (fg[1] < 0) fg[1] = 0;
                if (fg[2] < 0) fg[2] = 0;

                if (fg[0] > BASE_MASK) fg[0] = BASE_MASK;
                if (fg[1] > BASE_MASK) fg[1] = BASE_MASK;
                if (fg[2] > BASE_MASK) fg[2] = BASE_MASK;

                span[spanIndex].alpha = BASE_MASK;
                span[spanIndex].red = (byte)fg[0];
                span[spanIndex].green = (byte)fg[1];
                span[spanIndex].blue = (byte)fg[2];

                spanIndex++;
                interpolator().Next();
            } while (--len != 0);
        }
    }
}