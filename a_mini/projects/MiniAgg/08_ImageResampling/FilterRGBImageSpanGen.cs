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

using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using img_filter_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgFilterConst;

namespace PixelFarm.Agg.Image
{

    //=================================================span_image_resample_rgb
    class FilterRGBImageSpanGen : FilterImageSpanGenerator
    {
        const int BASE_MASK = 255;
        const int DOWNSCALE_SHIFT = (int)ImageFilterLookUpTable.ImgFilterConst.SHIFT;

        //--------------------------------------------------------------------
        public FilterRGBImageSpanGen(IImageReaderWriter src,
                            ISpanInterpolator inter,
                            ImageFilterLookUpTable filter) :
            base(src, inter, filter)
        {
            if (src.GetRecieveBlender().NumPixelBits != 24)
            {
                throw new System.FormatException("You have to use a rgb blender with span_image_resample_rgb");
            }
        }

        public override void GenerateColors(ColorRGBA[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = base.Interpolator;
            spanInterpolator.Begin(x + base.dx, y + base.dy, len);


            int fg0, fg1, fg2;

            byte[] fg_ptr;
            int diameter = base.FilterLookup.Diameter;
            int filter_scale = diameter << img_subpix_const.SHIFT;
            int[] weight_array = FilterLookup.WeightArray;




            do
            {
                int rx;
                int ry;
                int rx_inv = img_subpix_const.SCALE;
                int ry_inv = img_subpix_const.SCALE;

                spanInterpolator.GetCoord(out x, out y);
                spanInterpolator.GetLocalScale(out rx, out ry);

                AdjustScale(ref rx, ref ry);


                rx_inv = img_subpix_const.SCALE * img_subpix_const.SCALE / rx;
                ry_inv = img_subpix_const.SCALE * img_subpix_const.SCALE / ry;

                int radius_x = (diameter * rx) >> 1;
                int radius_y = (diameter * ry) >> 1;
                int len_x_lr =
                    (diameter * rx + img_subpix_const.MASK) >>
                        img_subpix_const.SHIFT;

                x += base.dxInt - radius_x;
                y += base.dyInt - radius_y;

                fg0 = fg1 = fg2 = img_filter_const.SCALE / 2;

                int y_lr = y >> img_subpix_const.SHIFT;
                int y_hr = ((img_subpix_const.MASK - (y & img_subpix_const.MASK)) *
                               ry_inv) >> img_subpix_const.SHIFT;
                int total_weight = 0;
                int x_lr = x >> img_subpix_const.SHIFT;
                int x_hr = ((img_subpix_const.MASK - (x & img_subpix_const.MASK)) *
                               rx_inv) >> img_subpix_const.SHIFT;
                int x_hr2 = x_hr;
                int sourceIndex;
                fg_ptr = BaseGetSpan(x_lr, y_lr, len_x_lr, out sourceIndex);

                for (; ; )
                {
                    int weight_y = weight_array[y_hr];
                    x_hr = x_hr2;
                    for (; ; )
                    {
                        int weight = (weight_y * weight_array[x_hr] +
                                     (int)img_filter_const.SCALE / 2) >>
                                     DOWNSCALE_SHIFT;
                        fg0 += fg_ptr[sourceIndex + ImageReaderWriterBase.R] * weight;
                        fg1 += fg_ptr[sourceIndex + ImageReaderWriterBase.G] * weight;
                        fg2 += fg_ptr[sourceIndex + ImageReaderWriterBase.B] * weight;
                        total_weight += weight;
                        x_hr += rx_inv;
                        if (x_hr >= filter_scale) break;
                        fg_ptr = BaseNextX(out sourceIndex);
                    }
                    y_hr += ry_inv;
                    if (y_hr >= filter_scale)
                    {
                        break;
                    }

                    fg_ptr = BaseNextY(out sourceIndex);
                }

                fg0 /= total_weight;
                fg1 /= total_weight;
                fg2 /= total_weight;

                //clamps..
                if (fg0 < 0) { fg0 = 0; }
                else if (fg0 > BASE_MASK) { fg0 = BASE_MASK; }

                if (fg1 < 0) { fg1 = 0; }
                else if (fg1 > BASE_MASK) { fg1 = BASE_MASK; }

                if (fg2 < 0) { fg2 = 0; }
                else if (fg2 > BASE_MASK) { fg2 = BASE_MASK; }


                outputColors[startIndex].alpha = BASE_MASK;
                outputColors[startIndex].red = (byte)fg0;
                outputColors[startIndex].green = (byte)fg1;
                outputColors[startIndex].blue = (byte)fg2;

                startIndex++;
                spanInterpolator.Next();
            } while (--len != 0);
        }
    }
}