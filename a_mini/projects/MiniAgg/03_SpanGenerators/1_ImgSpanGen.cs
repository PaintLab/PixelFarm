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
using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using PixelFarm.Agg.Image;

namespace PixelFarm.Agg
{
    public abstract class ImgSpanGen : ISpanGenerator
    {
        IImageBufferAccessor imageBufferAccessor;
        protected ISpanInterpolator m_interpolator;
        protected ImageFilterLookUpTable filterLookup;

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
            filterLookup = (filter);
            m_dx_dbl = (0.5);
            m_dy_dbl = (0.5);
            m_dx_int = ((int)img_subpix_const.SCALE / 2);
            m_dy_int = ((int)img_subpix_const.SCALE / 2);
        }

        public abstract void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len);

        public IImageBufferAccessor GetImageBufferAccessor() { return imageBufferAccessor; }
        public ImageFilterLookUpTable FilterLookup
        {
            get { return filterLookup; }
            set { this.filterLookup = value; }
        }

        public ISpanInterpolator Interpolator
        {
            get { return m_interpolator; }
            set { this.m_interpolator = value; }
        }


     
        public double Dx { get { return m_dx_dbl; } }
        public double Dy { get { return m_dy_dbl; } }
        public int DxInt { get { return m_dx_int; } }
        public int DyInt { get { return m_dy_int; } }


        public void SetFilterOffset(double dx, double dy)
        {
            m_dx_dbl = dx;
            m_dy_dbl = dy;
            m_dx_int = (int)AggBasics.iround(dx * (int)img_subpix_const.SCALE);
            m_dy_int = (int)AggBasics.iround(dy * (int)img_subpix_const.SCALE);
        }

        public void SetFilterOffset(double d) { SetFilterOffset(d, d); }



        public void Prepare() { }
    }

}