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

namespace PixelFarm.Agg
{
    public interface IGradientGen
    {
        int Calculate(int x, int y, int d);
    }

    public interface IColorFunction
    {
        int Size { get; }
        ColorRGBA this[int v] { get; }
    }

    //==========================================================span_gradient
    public class SpanGenGradient : ISpanGenerator
    {
        const int GR_SUBPIX_SHIFT = 4;                              //-----gradient_subpixel_shift
        public const int GR_SUBPIX_SCALE = 1 << GR_SUBPIX_SHIFT;   //-----gradient_subpixel_scale
        const int GR_SUBPIX_MASK = GR_SUBPIX_SCALE - 1;    //-----gradient_subpixel_mask

        const int SUBPIX_SHIFT = 8;

        const int DOWN_SCALE_SHIFT = SUBPIX_SHIFT - GR_SUBPIX_SHIFT;

        ISpanInterpolator m_interpolator;
        IGradientGen m_gradient_function;
        IColorFunction m_color_function;
        int m_d1;
        int m_d2;

        //--------------------------------------------------------------------
        public SpanGenGradient() { } 
        //--------------------------------------------------------------------
        public SpanGenGradient(ISpanInterpolator inter,
                      IGradientGen gradient_function,
                      IColorFunction color_function,
                      double d1, double d2)
        {
            m_interpolator = inter;
            m_gradient_function = gradient_function;
            m_color_function = color_function;
            m_d1 = (AggBasics.iround(d1 * GR_SUBPIX_SCALE));
            m_d2 = (AggBasics.iround(d2 * GR_SUBPIX_SCALE));

        }

        //--------------------------------------------------------------------
        public ISpanInterpolator Interpolator
        {
            get { return this.m_interpolator; }
            set { this.m_interpolator = value; }
        }
        public IGradientGen GradientGenFunction
        {
            get { return this.m_gradient_function; }
            set { this.m_gradient_function = value; }
        }
        public IColorFunction ColorFunction
        {
            get { return this.m_color_function; }
            set { this.m_color_function = value; }
        }
        public double d1
        {
            get { return (double)(m_d1) / GR_SUBPIX_SCALE; }
            set { m_d1 = AggBasics.iround(value * GR_SUBPIX_SCALE); }
        }
        public double d2
        {
            get { return (double)(m_d2) / GR_SUBPIX_SCALE; }
            set { m_d2 = AggBasics.iround(value * GR_SUBPIX_SCALE); }
        }

        //--------------------------------------------------------------------
        public void Prepare() { }
        //--------------------------------------------------------------------
        public void GenerateColors(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            int dd = m_d2 - m_d1;
            if (dd < 1) dd = 1;
            m_interpolator.Begin(x + 0.5, y + 0.5, len);
            do
            {
                m_interpolator.GetCoord(out x, out y);
                int d = m_gradient_function.Calculate(x >> DOWN_SCALE_SHIFT,
                                                       y >> DOWN_SCALE_SHIFT, m_d2);
                d = ((d - m_d1) * (int)m_color_function.Size) / dd;
                if (d < 0) d = 0;
                if (d >= (int)m_color_function.Size)
                {
                    d = m_color_function.Size - 1;
                }

                span[spanIndex++] = m_color_function[d];
                m_interpolator.Next();
            }
            while (--len != 0);
        }
    }

    //=====================================================gradient_linear_color
    public struct LinearGradientColorFunction : IColorFunction
    {
        ColorRGBA m_c1;
        ColorRGBA m_c2;
        int m_size;

        public LinearGradientColorFunction(ColorRGBA c1, ColorRGBA c2)
            : this(c1, c2, 256)
        {
        }
        public LinearGradientColorFunction(ColorRGBA c1, ColorRGBA c2, int size)
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

    //==========================================================gradient_circle
    public class GradientGenCircle : IGradientGen
    {
        // Actually the same as radial. Just for compatibility
        public int Calculate(int x, int y, int d)
        {

            return (int)(AggMath.fast_sqrt((int)(x * x + y * y)));
        }
    }


    //==========================================================gradient_radial
    public class GradientGenRadial : IGradientGen
    {
        public int Calculate(int x, int y, int d)
        {
            return (int)(System.Math.Sqrt(x * x + y * y));
        }
    }

    //========================================================gradient_radial_d
    public class GradientGenRadialD : IGradientGen
    {
        public int Calculate(int x, int y, int d)
        {
            return (int)AggBasics.uround(System.Math.Sqrt((double)(x) * (double)(x) + (double)(y) * (double)(y)));
        }
    }

    //====================================================gradient_radial_focus
    public class GradientGenRadialFocus : IGradientGen
    {
        int m_r;
        int m_fx;
        int m_fy;
        double m_r2;
        double m_fx2;
        double m_fy2;
        double m_mul;

        //---------------------------------------------------------------------
        public GradientGenRadialFocus()
        {
            m_r = (100 * SpanGenGradient.GR_SUBPIX_SCALE);
            m_fx = (0);
            m_fy = (0);
            UpdateValues();
        }

        //---------------------------------------------------------------------
        public GradientGenRadialFocus(double r, double fx, double fy)
        {
            m_r = (AggBasics.iround(r * SpanGenGradient.GR_SUBPIX_SCALE));
            m_fx = (AggBasics.iround(fx * SpanGenGradient.GR_SUBPIX_SCALE));
            m_fy = (AggBasics.iround(fy * SpanGenGradient.GR_SUBPIX_SCALE));
            UpdateValues();
        }

        //---------------------------------------------------------------------
        public void Init(double r, double fx, double fy)
        {
            m_r = AggBasics.iround(r * SpanGenGradient.GR_SUBPIX_SCALE);
            m_fx = AggBasics.iround(fx * SpanGenGradient.GR_SUBPIX_SCALE);
            m_fy = AggBasics.iround(fy * SpanGenGradient.GR_SUBPIX_SCALE);
            UpdateValues();
        }

        //---------------------------------------------------------------------
        public double Radius { get { return (double)(m_r) / SpanGenGradient.GR_SUBPIX_SCALE; } }
        public double FocusX { get { return (double)(m_fx) / SpanGenGradient.GR_SUBPIX_SCALE; } }
        public double FocusY { get { return (double)(m_fy) / SpanGenGradient.GR_SUBPIX_SCALE; } }

        //---------------------------------------------------------------------
        public int Calculate(int x, int y, int d)
        {
            double dx = x - m_fx;
            double dy = y - m_fy;
            double d2 = dx * m_fy - dy * m_fx;
            double d3 = m_r2 * (dx * dx + dy * dy) - d2 * d2;
            return AggBasics.iround((dx * m_fx + dy * m_fy + System.Math.Sqrt(System.Math.Abs(d3))) * m_mul);

        }

        //---------------------------------------------------------------------
        private void UpdateValues()
        {
            // Calculate the invariant values. In case the focal center
            // lies exactly on the gradient circle the divisor degenerates
            // into zero. In this case we just move the focal center by
            // one subpixel unit possibly in the direction to the origin (0,0)
            // and calculate the values again.
            //-------------------------
            m_r2 = (double)(m_r) * (double)(m_r);
            m_fx2 = (double)(m_fx) * (double)(m_fx);
            m_fy2 = (double)(m_fy) * (double)(m_fy);
            double d = (m_r2 - (m_fx2 + m_fy2));
            if (d == 0)
            {
                if (m_fx != 0)
                {
                    if (m_fx < 0) ++m_fx; else --m_fx;
                }

                if (m_fy != 0)
                {
                    if (m_fy < 0) ++m_fy; else --m_fy;
                }

                m_fx2 = (double)(m_fx) * (double)(m_fx);
                m_fy2 = (double)(m_fy) * (double)(m_fy);
                d = (m_r2 - (m_fx2 + m_fy2));
            }
            m_mul = m_r / d;
        }
    }


    //==============================================================gradient_x
    public class GradientGenX : IGradientGen
    {
        public int Calculate(int x, int y, int d) { return x; }
    }


    //==============================================================gradient_y
    public class GradientGenY : IGradientGen
    {
        public int Calculate(int x, int y, int d) { return y; }
    }

    //========================================================gradient_diamond
    public class GradientGenDiamond : IGradientGen
    {
        public int Calculate(int x, int y, int d)
        {
            int ax = System.Math.Abs(x);
            int ay = System.Math.Abs(y);
            return ax > ay ? ax : ay;
        }
    }

    //=============================================================gradient_xy
    public class GradientGenXY : IGradientGen
    {
        public int Calculate(int x, int y, int d)
        {
            return System.Math.Abs(x * y) / d;
        }
    }

    //========================================================gradient_sqrt_xy
    public class GradientGenSquareXY : IGradientGen
    {
        public int Calculate(int x, int y, int d)
        {
            //return (int)System.Math.Sqrt((int)(System.Math.Abs(x) * System.Math.Abs(y)));
            return (int)AggMath.fast_sqrt((int)(System.Math.Abs(x * y)));
        }
    }

    //==========================================================gradient_conic
    public class GradientConic : IGradientGen
    {
        public int Calculate(int x, int y, int d)
        {
            return (int)AggBasics.uround(System.Math.Abs(System.Math.Atan2((double)(y), (double)(x))) * (double)(d) / System.Math.PI);
        }
    }

    //=================================================gradient_repeat_adaptor
    public class GradientRepeatAdaptor : IGradientGen
    {
        IGradientGen m_gradient;

        public GradientRepeatAdaptor(IGradientGen gradient)
        {
            m_gradient = gradient;
        }


        public int Calculate(int x, int y, int d)
        {
            int ret = m_gradient.Calculate(x, y, d) % d;
            if (ret < 0) ret += d;
            return ret;
        }
    }

    //================================================gradient_reflect_adaptor
    public class GradientGenReflectAdaptor : IGradientGen
    {
        IGradientGen m_gradient;

        public GradientGenReflectAdaptor(IGradientGen gradient)
        {
            m_gradient = gradient;
        }

        public int Calculate(int x, int y, int d)
        {
            int d2 = d << 1;
            int ret = m_gradient.Calculate(x, y, d) % d2;
            if (ret < 0) ret += d2;
            if (ret >= d) ret = d2 - ret;
            return ret;
        }
    }

    public class GradientGenClampAdapter : IGradientGen
    {
        IGradientGen m_gradient;

        public GradientGenClampAdapter(IGradientGen gradient)
        {
            m_gradient = gradient;
        }

        public int Calculate(int x, int y, int d)
        {
            int ret = m_gradient.Calculate(x, y, d);
            if (ret < 0) ret = 0;
            if (ret > d) ret = d;
            return ret;
        }
    }
}