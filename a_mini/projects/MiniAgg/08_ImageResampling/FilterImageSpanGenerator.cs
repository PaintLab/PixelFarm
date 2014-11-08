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
// Image transformations with filtering. Span generator base class
//
//----------------------------------------------------------------------------
using System;
using img_subpix_scale = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;

namespace PixelFarm.Agg.Image
{
    //=====================================================span_image_resample
    public abstract class FilterImageSpanGenerator : ImgSpanGen
    {
        ImageFilterLookUpTable filterLookup;
        public FilterImageSpanGenerator(IImageReaderWriter src,
                            ISpanInterpolator inter,
                            ImageFilterLookUpTable filterLookup)
            : base(src, inter)
        {
            m_scale_limit = 20;
            m_blur_x = ((int)img_subpix_scale.SCALE);
            m_blur_y = ((int)img_subpix_scale.SCALE);
            this.filterLookup = filterLookup;
        }
        protected ImageFilterLookUpTable FilterLookup
        {
            get { return filterLookup; }
        }
        //public abstract void prepare();
        //public abstract unsafe void generate(rgba8* span, int x, int y, int len);

        ////--------------------------------------------------------------------
        ////int scale_limit() { return m_scale_limit; }
        ////void scale_limit(int v) { m_scale_limit = v; }
        //int ScaleLimit
        //{
        //    get;
        //    set;
        //}
        ////--------------------------------------------------------------------
        //double blur_x() { return (double)(m_blur_x) / (double)((int)img_subpix_const.image_subpixel_scale); }
        //double blur_y() { return (double)(m_blur_y) / (double)((int)img_subpix_const.image_subpixel_scale); }
        //void blur_x(double v) { m_blur_x = (int)AggBasics.uround(v * (double)((int)img_subpix_const.image_subpixel_scale)); }
        //void blur_y(double v) { m_blur_y = (int)AggBasics.uround(v * (double)((int)img_subpix_const.image_subpixel_scale)); }

        //public void blur(double v)
        //{
        //    m_blur_x = m_blur_y = (int)AggBasics.uround(v * (double)((int)img_subpix_const.image_subpixel_scale));
        //}



        protected void AdjustScale(ref int rx, ref int ry)
        {
            if (rx <  img_subpix_scale.SCALE) rx =  img_subpix_scale.SCALE;
            if (ry <  img_subpix_scale.SCALE) ry =  img_subpix_scale.SCALE;
            if (rx >  img_subpix_scale.SCALE * m_scale_limit)
            {
                rx =  img_subpix_scale.SCALE * m_scale_limit;
            }
            if (ry >  img_subpix_scale.SCALE * m_scale_limit)
            {
                ry =  img_subpix_scale.SCALE * m_scale_limit;
            }
            rx = (rx * m_blur_x) >>  img_subpix_scale.SHIFT;
            ry = (ry * m_blur_y) >> img_subpix_scale.SHIFT;
            if (rx <  img_subpix_scale.SCALE) rx =  img_subpix_scale.SCALE;
            if (ry <  img_subpix_scale.SCALE) ry =  img_subpix_scale.SCALE;
        }

        int m_scale_limit;
        int m_blur_x;
        int m_blur_y;
    }
}
