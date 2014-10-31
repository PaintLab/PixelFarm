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
using image_subpixel_scale_e = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using PixelFarm.Agg.Image;

namespace PixelFarm.Agg
{
    public abstract class ImgSpanGen : ISpanGenerator
    {
        IImageBufferAccessor imageBufferAccessor;
        protected ISpanInterpolator m_interpolator;
        protected ImageFilterLookUpTable m_filter;
        double m_dx_dbl;
        double m_dy_dbl;
        int m_dx_int;
        int m_dy_int;

        public ImgSpanGen() { }
        public ImgSpanGen(IImageBufferAccessor src,
            ISpanInterpolator interpolator)
            : this(src, interpolator, null)
        {

        }

        public ImgSpanGen(IImageBufferAccessor src,
            ISpanInterpolator interpolator, ImageFilterLookUpTable filter)
        {
            imageBufferAccessor = src;
            m_interpolator = interpolator;
            m_filter = (filter);
            m_dx_dbl = (0.5);
            m_dy_dbl = (0.5);
            m_dx_int = ((int)image_subpixel_scale_e.SCALE / 2);
            m_dy_int = ((int)image_subpixel_scale_e.SCALE / 2);
        }
        public void attach(IImageBufferAccessor v) { imageBufferAccessor = v; }

        public abstract void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len);

        public IImageBufferAccessor GetImageBufferAccessor() { return imageBufferAccessor; }
        public ImageFilterLookUpTable filter() { return m_filter; }
        public int filter_dx_int() { return (int)m_dx_int; }
        public int filter_dy_int() { return (int)m_dy_int; }
        public double filter_dx_dbl() { return m_dx_dbl; }
        public double filter_dy_dbl() { return m_dy_dbl; }

        public void interpolator(ISpanInterpolator v) { m_interpolator = v; }
        public void filter(ImageFilterLookUpTable v) { m_filter = v; }
        public void filter_offset(double dx, double dy)
        {
            m_dx_dbl = dx;
            m_dy_dbl = dy;
            m_dx_int = (int)AggBasics.iround(dx * (int)image_subpixel_scale_e.SCALE);
            m_dy_int = (int)AggBasics.iround(dy * (int)image_subpixel_scale_e.SCALE);
        }
        public void filter_offset(double d) { filter_offset(d, d); }

        public ISpanInterpolator interpolator() { return m_interpolator; }

        public void Prepare() { }
    }

}