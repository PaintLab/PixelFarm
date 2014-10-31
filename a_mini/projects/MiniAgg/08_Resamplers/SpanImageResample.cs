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
using image_subpixel_scale_e = PixelFarm.Agg.ImageFilterLookUpTable.image_subpixel_scale_e;

namespace PixelFarm.Agg
{
     
    public abstract class SpanImageFilter : ISpanGenerator
    {
        private IImageBufferAccessor imageBufferAccessor;
        protected ISpanInterpolator m_interpolator;
        protected ImageFilterLookUpTable m_filter;
        private double m_dx_dbl;
        private double m_dy_dbl;
        private int m_dx_int;
        private int m_dy_int;

        public SpanImageFilter() { }
        public SpanImageFilter(IImageBufferAccessor src,
            ISpanInterpolator interpolator)
            : this(src, interpolator, null)
        {

        }

        public SpanImageFilter(IImageBufferAccessor src,
            ISpanInterpolator interpolator, ImageFilterLookUpTable filter)
        {
            imageBufferAccessor = src;
            m_interpolator = interpolator;
            m_filter = (filter);
            m_dx_dbl = (0.5);
            m_dy_dbl = (0.5);
            m_dx_int = ((int)image_subpixel_scale_e.image_subpixel_scale / 2);
            m_dy_int = ((int)image_subpixel_scale_e.image_subpixel_scale / 2);
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
            m_dx_int = (int)AggBasics.iround(dx * (int)image_subpixel_scale_e.image_subpixel_scale);
            m_dy_int = (int)AggBasics.iround(dy * (int)image_subpixel_scale_e.image_subpixel_scale);
        }
        public void filter_offset(double d) { filter_offset(d, d); }

        public ISpanInterpolator interpolator() { return m_interpolator; }

        public void Prepare() { }
    }

    

    //=====================================================span_image_resample
    public abstract class SpanImageResample
        : SpanImageFilter
    {
        public SpanImageResample(IImageBufferAccessor src,
                            ISpanInterpolator inter,
                            ImageFilterLookUpTable filter)
            : base(src, inter, filter)
        {
            m_scale_limit = (20);
            m_blur_x = ((int)image_subpixel_scale_e.image_subpixel_scale);
            m_blur_y = ((int)image_subpixel_scale_e.image_subpixel_scale);
        }

        //public abstract void prepare();
        //public abstract unsafe void generate(rgba8* span, int x, int y, int len);

        //--------------------------------------------------------------------
        int scale_limit() { return m_scale_limit; }
        void scale_limit(int v) { m_scale_limit = v; }

        //--------------------------------------------------------------------
        double blur_x() { return (double)(m_blur_x) / (double)((int)image_subpixel_scale_e.image_subpixel_scale); }
        double blur_y() { return (double)(m_blur_y) / (double)((int)image_subpixel_scale_e.image_subpixel_scale); }
        void blur_x(double v) { m_blur_x = (int)AggBasics.uround(v * (double)((int)image_subpixel_scale_e.image_subpixel_scale)); }
        void blur_y(double v) { m_blur_y = (int)AggBasics.uround(v * (double)((int)image_subpixel_scale_e.image_subpixel_scale)); }
        public void blur(double v)
        {
            m_blur_x = m_blur_y = (int)AggBasics.uround(v * (double)((int)image_subpixel_scale_e.image_subpixel_scale));
        }

        protected void adjust_scale(ref int rx, ref int ry)
        {
            if (rx < (int)image_subpixel_scale_e.image_subpixel_scale) rx = (int)image_subpixel_scale_e.image_subpixel_scale;
            if (ry < (int)image_subpixel_scale_e.image_subpixel_scale) ry = (int)image_subpixel_scale_e.image_subpixel_scale;
            if (rx > (int)image_subpixel_scale_e.image_subpixel_scale * m_scale_limit)
            {
                rx = (int)image_subpixel_scale_e.image_subpixel_scale * m_scale_limit;
            }
            if (ry > (int)image_subpixel_scale_e.image_subpixel_scale * m_scale_limit)
            {
                ry = (int)image_subpixel_scale_e.image_subpixel_scale * m_scale_limit;
            }
            rx = (rx * m_blur_x) >> (int)image_subpixel_scale_e.image_subpixel_shift;
            ry = (ry * m_blur_y) >> (int)image_subpixel_scale_e.image_subpixel_shift;
            if (rx < (int)image_subpixel_scale_e.image_subpixel_scale) rx = (int)image_subpixel_scale_e.image_subpixel_scale;
            if (ry < (int)image_subpixel_scale_e.image_subpixel_scale) ry = (int)image_subpixel_scale_e.image_subpixel_scale;
        }

        int m_scale_limit;
        int m_blur_x;
        int m_blur_y;
    }
}
