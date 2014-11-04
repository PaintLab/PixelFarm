//MIT 2014, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
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
using System;
using PixelFarm.Agg.Gradients;

namespace PixelFarm.Agg
{    
  
    //==========================================================span_gradient
    public class SpanGenGradient : ISpanGenerator
    {
        const int GR_SUBPIX_SHIFT = 4;                              //-----gradient_subpixel_shift
        public const int GR_SUBPIX_SCALE = 1 << GR_SUBPIX_SHIFT;   //-----gradient_subpixel_scale
        const int GR_SUBPIX_MASK = GR_SUBPIX_SCALE - 1;    //-----gradient_subpixel_mask

        const int SUBPIX_SHIFT = 8;

        const int DOWN_SCALE_SHIFT = SUBPIX_SHIFT - GR_SUBPIX_SHIFT;

        ISpanInterpolator m_interpolator;
        IGradientValueCalculator m_grValueCalculator;
        IColorsProvider m_colorsProvider;

        int m_d1;
        int m_d2;
        //--------------------------------------------------------------------
        public SpanGenGradient(ISpanInterpolator inter,
                      IGradientValueCalculator gradient_function,
                      IColorsProvider m_colorsProvider,
                      double d1, double d2)
        {
            m_interpolator = inter;
            this.m_grValueCalculator = gradient_function;
            this.m_colorsProvider = m_colorsProvider;
            m_d1 = AggBasics.iround(d1 * GR_SUBPIX_SCALE);
            m_d2 = AggBasics.iround(d2 * GR_SUBPIX_SCALE);
        }

        //--------------------------------------------------------------------
        public ISpanInterpolator Interpolator
        {
            get { return this.m_interpolator; }
            set { this.m_interpolator = value; }
        }
        public IGradientValueCalculator GradientGenFunction
        {
            get { return this.m_grValueCalculator; }
            set { this.m_grValueCalculator = value; }
        }
        public IColorsProvider ColorsProvider
        {
            get { return this.m_colorsProvider; }
            set { this.m_colorsProvider = value; }
        }

        //public double d1
        //{
        //    get { return (double)(m_d1) / GR_SUBPIX_SCALE; }
        //    set { m_d1 = AggBasics.iround(value * GR_SUBPIX_SCALE); }
        //}
        //public double d2
        //{
        //    get { return (double)(m_d2) / GR_SUBPIX_SCALE; }
        //    set { m_d2 = AggBasics.iround(value * GR_SUBPIX_SCALE); }
        //}

        //--------------------------------------------------------------------
        public void Prepare() { }
        //--------------------------------------------------------------------
        public void GenerateColors(ColorRGBA[] outputColors, int startIndex, int x, int y, int len)
        {
            int dd = m_d2 - m_d1;
            if (dd < 1) dd = 1;

            m_interpolator.Begin(x + 0.5, y + 0.5, len);
            do
            {
                m_interpolator.GetCoord(out x, out y);

                int d = m_grValueCalculator.Calculate(x >> DOWN_SCALE_SHIFT,
                                                      y >> DOWN_SCALE_SHIFT,
                                                      m_d2);

                d = ((d - m_d1) * (int)m_colorsProvider.Size) / dd;
                if (d < 0) d = 0;
                if (d >= (int)m_colorsProvider.Size)
                {
                    d = m_colorsProvider.Size - 1;
                }

                outputColors[startIndex++] = m_colorsProvider[d];
                m_interpolator.Next();
            }
            while (--len != 0);
        }
    }

    //=====================================================gradient_linear_color
    public class LinearGradientColorsProvider : Gradients.IColorsProvider
    {
        ColorRGBA m_c1;
        ColorRGBA m_c2;
        int m_size;

        public LinearGradientColorsProvider(ColorRGBA c1, ColorRGBA c2)
            : this(c1, c2, 256)
        {
        }
        public LinearGradientColorsProvider(ColorRGBA c1, ColorRGBA c2, int size)
        {
            m_c1 = c1;
            m_c2 = c2;
            m_size = size;
        }
        public int Size { get { return m_size; } }
        public ColorRGBA this[int v]
        {
            get
            {
                return m_c1.CreateGradient(m_c2, (double)(v) / (double)(m_size - 1));
            }
        }
        public void SetColors(ColorRGBA c1, ColorRGBA c2)
        {
            SetColors(c1, c2, 256);
        }
        public void SetColors(ColorRGBA c1, ColorRGBA c2, int size)
        {
            m_c1 = c1;
            m_c2 = c2;
            m_size = size;
        }
    }

}