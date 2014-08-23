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
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using System.Collections;

namespace MatterHackers.Agg
{
    // Supported byte orders for RGB and RGBA pixel formats
    //=======================================================================
    //struct order_rgb { enum rgb_e { R = 0, G = 1, B = 2, rgb_tag }; };       //----order_rgb
    //struct order_bgr { enum bgr_e { B = 0, G = 1, R = 2, rgb_tag }; };       //----order_bgr
    //struct order_rgba { enum rgba_e { R = 0, G = 1, B = 2, A = 3, rgba_tag }; }; //----order_rgba
    //struct order_argb { enum argb_e { A = 0, R = 1, G = 2, B = 3, rgba_tag }; }; //----order_argb
    //struct order_abgr { enum abgr_e { A = 0, B = 1, G = 2, R = 3, rgba_tag }; }; //----order_abgr
    //struct order_bgra { enum bgra_e { B = 0, G = 1, R = 2, A = 3, rgba_tag }; }; //----order_bgra

    public struct ColorRGBAf : IColor
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        public float red;
        public float green;
        public float blue;
        public float alpha;

        public int Red0To255
        {
            get { return (int)AggBasics.uround(red * (float)BASE_MASK); }

        }
        public int Green0To255
        {
            get { return (int)AggBasics.uround(green * (float)BASE_MASK); }

        }
        public int Blue0To255
        {
            get { return (int)AggBasics.uround(blue * (float)BASE_MASK); }

        }
        public int Alpha0To255
        {
            get { return (int)AggBasics.uround(alpha * (float)BASE_MASK); }
        }

        public float Red0To1 { get { return red; } set { red = value; } }
        public float Green0To1 { get { return green; } set { green = value; } }
        public float Blue0To1 { get { return blue; } set { blue = value; } }
        public float Alpha0To1 { get { return alpha; } set { alpha = value; } }

        #region Defined Colors
        public static readonly ColorRGBAf White = new ColorRGBAf(1, 1, 1, 1);
        public static readonly ColorRGBAf Black = new ColorRGBAf(0, 0, 0, 1);
        public static readonly ColorRGBAf Red = new ColorRGBAf(1, 0, 0, 1);
        public static readonly ColorRGBAf Green = new ColorRGBAf(0, 1, 0, 1);
        public static readonly ColorRGBAf Blue = new ColorRGBAf(0, 0, 1, 1);
        public static readonly ColorRGBAf Cyan = new ColorRGBAf(0, 1, 1, 1);
        public static readonly ColorRGBAf Magenta = new ColorRGBAf(1, 0, 1, 1);
        public static readonly ColorRGBAf Yellow = new ColorRGBAf(1, 1, 0, 1);
        #endregion // Defined Colors

        #region Constructors
        public ColorRGBAf(double r_, double g_, double b_)
            : this(r_, g_, b_, 1.0)
        {
        }

        public ColorRGBAf(double r_, double g_, double b_, double a_)
        {
            red = (float)r_;
            green = (float)g_;
            blue = (float)b_;
            alpha = (float)a_;
        }

        public ColorRGBAf(float r_, float g_, float b_)
            : this(r_, g_, b_, 1.0f)
        {
        }

        public ColorRGBAf(float r_, float g_, float b_, float a_)
        {
            red = r_;
            green = g_;
            blue = b_;
            alpha = a_;
        }

        public ColorRGBAf(ColorRGBAf c)
            : this(c, c.alpha)
        {
        }

        public ColorRGBAf(ColorRGBAf c, float a_)
        {
            red = c.red;
            green = c.green;
            blue = c.blue;
            alpha = a_;
        }

        public ColorRGBAf(float wavelen)
            : this(wavelen, 1.0f)
        {

        }

        public ColorRGBAf(float wavelen, float gamma)
        {
            this = from_wavelength(wavelen, gamma);
        }

        public ColorRGBAf(ColorRGBA color)
        {
            red = color.Red0To1;
            green = color.Green0To1;
            blue = color.Blue0To1;
            alpha = color.Alpha0To1;
        }
        #endregion Constructors

        #region HSL
        // Given H,S,L,A in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static ColorRGBAf FromHSL(double hue0To1, double saturation0To1, double lightness0To1, double alpha = 1)
        {
            double v;
            double r, g, b;
            if (alpha > 1.0)
            {
                alpha = 1.0;
            }

            r = lightness0To1;   // default to gray
            g = lightness0To1;
            b = lightness0To1;
            v = lightness0To1 + saturation0To1 - lightness0To1 * saturation0To1;
            if (lightness0To1 <= 0.5)
            {
                v = lightness0To1 * (1.0 + saturation0To1);
            }

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = lightness0To1 + lightness0To1 - v;
                sv = (v - m) / v;
                hue0To1 *= 6.0;
                sextant = (int)hue0To1;
                fract = hue0To1 - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                    case 6:
                        goto case 0;
                }
            }

            return new ColorRGBAf(r, g, b, alpha);
        }

        public void GetHSL(out double hue0To1, out double saturation0To1, out double lightness0To1)
        {
            double maxRGB = Math.Max(red, Math.Max(green, blue));
            double minRGB = Math.Min(red, Math.Min(green, blue));
            double deltaMaxToMin = maxRGB - minRGB;
            double r2, g2, b2;

            hue0To1 = 0; // default to black
            saturation0To1 = 0;
            lightness0To1 = 0;
            lightness0To1 = (minRGB + maxRGB) / 2.0;
            if (lightness0To1 <= 0.0)
            {
                return;
            }
            saturation0To1 = deltaMaxToMin;
            if (saturation0To1 > 0.0)
            {
                saturation0To1 /= (lightness0To1 <= 0.5) ? (maxRGB + minRGB) : (2.0 - maxRGB - minRGB);
            }
            else
            {
                return;
            }
            r2 = (maxRGB - red) / deltaMaxToMin;
            g2 = (maxRGB - green) / deltaMaxToMin;
            b2 = (maxRGB - blue) / deltaMaxToMin;
            if (red == maxRGB)
            {
                if (green == minRGB)
                {
                    hue0To1 = 5.0 + b2;
                }
                else
                {
                    hue0To1 = 1.0 - g2;
                }
            }
            else if (green == maxRGB)
            {
                if (blue == minRGB)
                {
                    hue0To1 = 1.0 + r2;
                }
                else
                {
                    hue0To1 = 3.0 - b2;
                }
            }
            else
            {
                if (red == minRGB)
                {
                    hue0To1 = 3.0 + g2;
                }
                else
                {
                    hue0To1 = 5.0 - r2;
                }
            }
            hue0To1 /= 6.0;
        }

        public static ColorRGBAf AdjustSaturation(ColorRGBAf original, double saturationMultiplier)
        {
            double hue0To1;
            double saturation0To1;
            double lightness0To1;
            original.GetHSL(out hue0To1, out saturation0To1, out lightness0To1);
            saturation0To1 *= saturationMultiplier;

            return FromHSL(hue0To1, saturation0To1, lightness0To1);
        }

        public static ColorRGBAf AdjustLightness(ColorRGBAf original, double lightnessMultiplier)
        {
            double hue0To1;
            double saturation0To1;
            double lightness0To1;
            original.GetHSL(out hue0To1, out saturation0To1, out lightness0To1);
            lightness0To1 *= lightnessMultiplier;

            return FromHSL(hue0To1, saturation0To1, lightness0To1);
        }

        #endregion // HSL

        public static bool operator ==(ColorRGBAf a, ColorRGBAf b)
        {
            if (a.red == b.red && a.green == b.green && a.blue == b.blue && a.alpha == b.alpha)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(ColorRGBAf a, ColorRGBAf b)
        {
            if (a.red != b.red || a.green != b.green || a.blue != b.blue || a.alpha != b.alpha)
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ColorRGBAf))
            {
                return this == (ColorRGBAf)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new { blue, green, red, alpha }.GetHashCode();
        }

        public ColorRGBA GetAsRGBA_Bytes()
        {
            return ColorRGBA.Make(Red0To255, Green0To255, Blue0To255, Alpha0To255);
        }

        public ColorRGBAf GetAsRGBA_Floats()
        {
            return this;
        }

        static public ColorRGBAf operator +(ColorRGBAf A, ColorRGBAf B)
        {
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red + B.red;
            temp.green = A.green + B.green;
            temp.blue = A.blue + B.blue;
            temp.alpha = A.alpha + B.alpha;
            return temp;
        }

        static public ColorRGBAf operator -(ColorRGBAf A, ColorRGBAf B)
        {
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red - B.red;
            temp.green = A.green - B.green;
            temp.blue = A.blue - B.blue;
            temp.alpha = A.alpha - B.alpha;
            return temp;
        }

        static public ColorRGBAf operator *(ColorRGBAf A, ColorRGBAf B)
        {
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red * B.red;
            temp.green = A.green * B.green;
            temp.blue = A.blue * B.blue;
            temp.alpha = A.alpha * B.alpha;
            return temp;
        }

        static public ColorRGBAf operator /(ColorRGBAf A, ColorRGBAf B)
        {
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red / B.red;
            temp.green = A.green / B.green;
            temp.blue = A.blue / B.blue;
            temp.alpha = A.alpha / B.alpha;
            return temp;
        }

        static public ColorRGBAf operator /(ColorRGBAf A, float B)
        {
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red / B;
            temp.green = A.green / B;
            temp.blue = A.blue / B;
            temp.alpha = A.alpha / B;
            return temp;
        }

        static public ColorRGBAf operator /(ColorRGBAf A, double doubleB)
        {
            float B = (float)doubleB;
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red / B;
            temp.green = A.green / B;
            temp.blue = A.blue / B;
            temp.alpha = A.alpha / B;
            return temp;
        }

        static public ColorRGBAf operator *(ColorRGBAf A, float B)
        {
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red * B;
            temp.green = A.green * B;
            temp.blue = A.blue * B;
            temp.alpha = A.alpha * B;
            return temp;
        }

        static public ColorRGBAf operator *(ColorRGBAf A, double doubleB)
        {
            float B = (float)doubleB;
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red * B;
            temp.green = A.green * B;
            temp.blue = A.blue * B;
            temp.alpha = A.alpha * B;
            return temp;
        }

        public void clear()
        {
            red = green = blue = alpha = 0;
        }

        public ColorRGBAf transparent()
        {
            alpha = 0.0f;
            return this;
        }

        public ColorRGBAf opacity(float a_)
        {
            if (a_ < 0.0) a_ = 0.0f;
            if (a_ > 1.0) a_ = 1.0f;
            alpha = a_;
            return this;
        }

        public float opacity()
        {
            return alpha;
        }

        public ColorRGBAf premultiply()
        {
            red *= alpha;
            green *= alpha;
            blue *= alpha;
            return this;
        }

        public ColorRGBAf premultiply(float a_)
        {
            if (alpha <= 0.0 || a_ <= 0.0)
            {
                red = green = blue = alpha = 0.0f;
                return this;
            }
            a_ /= alpha;
            red *= a_;
            green *= a_;
            blue *= a_;
            alpha = a_;
            return this;
        }

        public ColorRGBAf demultiply()
        {
            if (alpha == 0)
            {
                red = green = blue = 0;
                return this;
            }
            float a_ = 1.0f / alpha;
            red *= a_;
            green *= a_;
            blue *= a_;
            return this;
        }

        public ColorRGBA gradient(ColorRGBA c_8, double k)
        {
            ColorRGBAf c = c_8.GetAsRGBA_Floats();
            ColorRGBAf ret;
            ret.red = (float)(red + (c.red - red) * k);
            ret.green = (float)(green + (c.green - green) * k);
            ret.blue = (float)(blue + (c.blue - blue) * k);
            ret.alpha = (float)(alpha + (c.alpha - alpha) * k);
            return ret.GetAsRGBA_Bytes();
        }


        public static IColor no_color() { return (IColor)new ColorRGBAf(0, 0, 0, 0); }

        public static ColorRGBAf from_wavelength(float wl)
        {
            return from_wavelength(wl, 1.0f);
        }

        public static ColorRGBAf from_wavelength(float wl, float gamma)
        {
            ColorRGBAf t = new ColorRGBAf(0.0f, 0.0f, 0.0f);

            if (wl >= 380.0 && wl <= 440.0)
            {
                t.red = (float)(-1.0 * (wl - 440.0) / (440.0 - 380.0));
                t.blue = 1.0f;
            }
            else if (wl >= 440.0 && wl <= 490.0)
            {
                t.green = (float)((wl - 440.0) / (490.0 - 440.0));
                t.blue = 1.0f;
            }
            else if (wl >= 490.0 && wl <= 510.0)
            {
                t.green = 1.0f;
                t.blue = (float)(-1.0 * (wl - 510.0) / (510.0 - 490.0));
            }
            else if (wl >= 510.0 && wl <= 580.0)
            {
                t.red = (float)((wl - 510.0) / (580.0 - 510.0));
                t.green = 1.0f;
            }
            else if (wl >= 580.0 && wl <= 645.0)
            {
                t.red = 1.0f;
                t.green = (float)(-1.0 * (wl - 645.0) / (645.0 - 580.0));
            }
            else if (wl >= 645.0 && wl <= 780.0)
            {
                t.red = 1.0f;
            }

            float s = 1.0f;
            if (wl > 700.0) s = (float)(0.3 + 0.7 * (780.0 - wl) / (780.0 - 700.0));
            else if (wl < 420.0) s = (float)(0.3 + 0.7 * (wl - 380.0) / (420.0 - 380.0));

            t.red = (float)Math.Pow(t.red * s, gamma);
            t.green = (float)Math.Pow(t.green * s, gamma);
            t.blue = (float)Math.Pow(t.blue * s, gamma);

            return t;
        }

        public static ColorRGBAf rgba_pre(double r, double g, double b)
        {
            return rgba_pre((float)r, (float)g, (float)b, 1.0f);
        }

        public static ColorRGBAf rgba_pre(float r, float g, float b)
        {
            return rgba_pre(r, g, b, 1.0f);
        }

        public static ColorRGBAf rgba_pre(float r, float g, float b, float a)
        {
            return new ColorRGBAf(r, g, b, a).premultiply();
        }

        public static ColorRGBAf rgba_pre(double r, double g, double b, double a)
        {
            return new ColorRGBAf((float)r, (float)g, (float)b, (float)a).premultiply();
        }

        public static ColorRGBAf rgba_pre(ColorRGBAf c)
        {
            return new ColorRGBAf(c).premultiply();
        }

        public static ColorRGBAf rgba_pre(ColorRGBAf c, float a)
        {
            return new ColorRGBAf(c, a).premultiply();
        }

        public static ColorRGBAf GetTweenColor(ColorRGBAf Color1, ColorRGBAf Color2, double RatioOf2)
        {
            if (RatioOf2 <= 0)
            {
                return new ColorRGBAf(Color1);
            }

            if (RatioOf2 >= 1.0)
            {
                return new ColorRGBAf(Color2);
            }

            // figure out how much of each color we should be.
            double RatioOf1 = 1.0 - RatioOf2;
            return new ColorRGBAf(
                Color1.red * RatioOf1 + Color2.red * RatioOf2,
                Color1.green * RatioOf1 + Color2.green * RatioOf2,
                Color1.blue * RatioOf1 + Color2.blue * RatioOf2);
        }

        public ColorRGBAf Blend(ColorRGBAf other, double weight)
        {
            ColorRGBAf result = new ColorRGBAf(this);
            result = this * (1 - weight) + other * weight;
            return result;
        }

        public double SumOfDistances(ColorRGBAf other)
        {
            double dist = Math.Abs(red - other.red) + Math.Abs(green - other.green) + Math.Abs(blue - other.blue);
            return dist;
        }


        static void Clamp0To1(ref float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
        }

        public void Clamp0To1()
        {
            Clamp0To1(ref red);
            Clamp0To1(ref green);
            Clamp0To1(ref blue);
            Clamp0To1(ref alpha);
        }
    }

    public struct ColorRGBA : IColor
    {   
        //--------------
        //BGRA *** 
        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;


        public const int COVER_SHIFT = 8;
        public const int COVER_SIZE = 1 << COVER_SHIFT;  //----cover_size 
        public const int COVER_MASK = COVER_SIZE - 1;    //----cover_mask 
        //public const int cover_none  = 0,                 //----cover_none 
        //public const int cover_full  = cover_mask         //----cover_full 

        public const int BASE_SHIFT = 8;
        public const int BASE_SCALE = (1 << BASE_SHIFT);
        public const int BASE_MASK = (BASE_SCALE - 1);




        public static readonly ColorRGBA White = new ColorRGBA(255, 255, 255, 255);
        public static readonly ColorRGBA LightGray = new ColorRGBA(225, 225, 225, 255);
        public static readonly ColorRGBA Gray = new ColorRGBA(125, 125, 125, 235);
        public static readonly ColorRGBA DarkGray = new ColorRGBA(85, 85, 85, 255);
        public static readonly ColorRGBA Black = new ColorRGBA(0, 0, 0, 255);
        public static readonly ColorRGBA Red = new ColorRGBA(255, 0, 0, 255);
        public static readonly ColorRGBA Orange = new ColorRGBA(255, 127, 0, 255);
        public static readonly ColorRGBA Pink = new ColorRGBA(255, 192, 203, 255);
        public static readonly ColorRGBA Green = new ColorRGBA(0, 255, 0, 255);
        public static readonly ColorRGBA Blue = new ColorRGBA(0, 0, 255, 255);
        public static readonly ColorRGBA Indigo = new ColorRGBA(75, 0, 130, 255);
        public static readonly ColorRGBA Violet = new ColorRGBA(143, 0, 255, 255);
        public static readonly ColorRGBA Cyan = new ColorRGBA(0, 255, 255, 255);
        public static readonly ColorRGBA Magenta = new ColorRGBA(255, 0, 255, 255);
        public static readonly ColorRGBA Yellow = new ColorRGBA(255, 255, 0, 255);
        public static readonly ColorRGBA YellowGreen = new ColorRGBA(154, 205, 50, 255);

        public int Red0To255
        {
            get { return (int)red; }
            set { red = (byte)value; }
        }
        public int Green0To255
        {
            get { return (int)green; }
            set { green = (byte)value; }
        }
        public int Blue0To255
        {
            get { return (int)blue; }
            set { blue = (byte)value; }
        }
        public int Alpha0To255
        {
            get { return (int)alpha; }
            set { alpha = (byte)value; }
        }

        public float Red0To1
        {
            get { return red / 255.0f; }
            //set { red = (byte)Math.Max(0, Math.Min((int)(value * 255), 255)); }
        }
        public float Green0To1
        {
            get { return green / 255.0f; }
            //set { green = (byte)Math.Max(0, Math.Min((int)(value * 255), 255)); }
        }
        public float Blue0To1
        {
            get { return blue / 255.0f; }
            //set { blue = (byte)Math.Max(0, Math.Min((int)(value * 255), 255)); }
        }
        public float Alpha0To1
        {
            get { return alpha / 255.0f; }
            //set { alpha = (byte)Math.Max(0, Math.Min((int)(value * 255), 255)); }
        }



        public ColorRGBA(byte r_, byte g_, byte b_)
        {
            red = r_;
            green = g_;
            blue = b_;
            alpha = (byte)Math.Min(Math.Max(BASE_MASK, 0), 255);
        }
        public ColorRGBA(byte r_, byte g_, byte b_, byte a_)
        {

            red = r_;
            green = g_;
            blue = b_;
            alpha = a_;
        }
        public static ColorRGBA Make(double r_, double g_, double b_, double a_)
        {
            return new ColorRGBA(
               ((byte)AggBasics.uround(r_ * (double)BASE_MASK)),
               ((byte)AggBasics.uround(g_ * (double)BASE_MASK)),
               ((byte)AggBasics.uround(b_ * (double)BASE_MASK)),
               ((byte)AggBasics.uround(a_ * (double)BASE_MASK)));

        }
        public static ColorRGBA Make(double r_, double g_, double b_)
        {
            return new ColorRGBA(
               ((byte)AggBasics.uround(r_ * (double)BASE_MASK)),
               ((byte)AggBasics.uround(g_ * (double)BASE_MASK)),
               ((byte)AggBasics.uround(b_ * (double)BASE_MASK)),
               ((byte)AggBasics.uround(BASE_MASK)));
        }
        public static ColorRGBA Make(int r_, int g_, int b_, int a_)
        {
            return new ColorRGBA(
               (byte)Math.Min(Math.Max(r_, 0), 255),
               (byte)Math.Min(Math.Max(g_, 0), 255),
               (byte)Math.Min(Math.Max(b_, 0), 255),
               (byte)Math.Min(Math.Max(a_, 0), 255));
        }

        public ColorRGBA(ColorRGBA c)
            : this(c, c.alpha)
        {
        }

        public ColorRGBA(ColorRGBA c, int a_)
        {
            red = (byte)c.red;
            green = (byte)c.green;
            blue = (byte)c.blue;
            alpha = (byte)a_;
        }

        public ColorRGBA(uint fourByteColor)
        {
            red = (byte)((fourByteColor >> 16) & 0xFF);
            green = (byte)((fourByteColor >> 8) & 0xFF);
            blue = (byte)((fourByteColor >> 0) & 0xFF);
            alpha = (byte)((fourByteColor >> 24) & 0xFF);
        }

        public ColorRGBA(ColorRGBAf c)
        {
            red = ((byte)AggBasics.uround(c.red * (double)BASE_MASK));
            green = ((byte)AggBasics.uround(c.green * (double)BASE_MASK));
            blue = ((byte)AggBasics.uround(c.blue * (double)BASE_MASK));
            alpha = ((byte)AggBasics.uround(c.alpha * (double)BASE_MASK));
        }

        public static bool operator ==(ColorRGBA a, ColorRGBA b)
        {
            //if a.red== bred then
            //a.red ^ b.red =0 
            return ((a.red ^ b.red) ^ (a.green ^ b.green) ^ (b.blue ^ a.alpha) ^ (a.alpha ^ b.alpha)) == 0;

            //if (a.red == b.red && a.green == b.green && a.blue == b.blue && a.alpha == b.alpha)
            //{
            //    return true;
            //} 
            //return false;
        }

        public static bool operator !=(ColorRGBA a, ColorRGBA b)
        {
            //if a.red !=  b.red then
            //a.red ^ b.red  =1
            return ((a.red ^ b.red) ^ (a.green ^ b.green) ^ (b.blue ^ a.alpha) ^ (a.alpha ^ b.alpha)) != 0;

            //if (a.red != b.red || a.green != b.green || a.blue != b.blue || a.alpha != b.alpha)
            //{
            //    return true;
            //}

            //return false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ColorRGBA))
            {
                return this == (ColorRGBA)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new { blue, green, red, alpha }.GetHashCode();
        }

        public ColorRGBAf GetAsRGBA_Floats()
        {
            return new ColorRGBAf((float)red / (float)BASE_MASK,
                (float)green / (float)BASE_MASK,
                (float)blue / (float)BASE_MASK,
                (float)alpha / (float)BASE_MASK);
        }

        public ColorRGBA GetAsRGBA_Bytes()
        {
            return this;
        }

        //public string GetAsHTMLString()
        //{
        //    string html = string.Format("#{0:X2}{1:X2}{2:X2}", red, green, blue);
        //    return html;
        //}

        void clear()
        {
            red = green = blue = alpha = 0;
        }

        public ColorRGBA gradient(ColorRGBA c, double k)
        {
            ColorRGBA ret = new ColorRGBA();
            int ik = AggBasics.uround(k * BASE_SCALE);
            ret.Red0To255 = (byte)((int)(Red0To255) + ((((int)(c.Red0To255) - Red0To255) * ik) >> BASE_SHIFT));
            ret.Green0To255 = (byte)((int)(Green0To255) + ((((int)(c.Green0To255) - Green0To255) * ik) >> BASE_SHIFT));
            ret.Blue0To255 = (byte)((int)(Blue0To255) + ((((int)(c.Blue0To255) - Blue0To255) * ik) >> BASE_SHIFT));
            ret.Alpha0To255 = (byte)((int)(Alpha0To255) + ((((int)(c.Alpha0To255) - Alpha0To255) * ik) >> BASE_SHIFT));
            return ret;
        }

        static public ColorRGBA operator +(ColorRGBA A, ColorRGBA B)
        {
            ColorRGBA temp = new ColorRGBA();
            temp.red = (byte)((A.red + B.red) > 255 ? 255 : (A.red + B.red));
            temp.green = (byte)((A.green + B.green) > 255 ? 255 : (A.green + B.green));
            temp.blue = (byte)((A.blue + B.blue) > 255 ? 255 : (A.blue + B.blue));
            temp.alpha = (byte)((A.alpha + B.alpha) > 255 ? 255 : (A.alpha + B.alpha));


            return temp;
        }

        static public ColorRGBA operator -(ColorRGBA A, ColorRGBA B)
        {
            ColorRGBA temp = new ColorRGBA();
            temp.red = (byte)((A.red - B.red) < 0 ? 0 : (A.red - B.red));
            temp.green = (byte)((A.green - B.green) < 0 ? 0 : (A.green - B.green));
            temp.blue = (byte)((A.blue - B.blue) < 0 ? 0 : (A.blue - B.blue));
            temp.alpha = 255;// (byte)((A.m_A - B.m_A) < 0 ? 0 : (A.m_A - B.m_A));
            return temp;
        }

        static public ColorRGBA operator *(ColorRGBA A, double doubleB)
        {
            float B = (float)doubleB;
            ColorRGBAf temp = new ColorRGBAf();
            temp.red = A.red / 255.0f * B;
            temp.green = A.green / 255.0f * B;
            temp.blue = A.blue / 255.0f * B;
            temp.alpha = A.alpha / 255.0f * B;
            return new ColorRGBA(temp);
        }

        public void add(ColorRGBA c, int cover)
        {
            int cr, cg, cb, ca;
            if (cover == COVER_MASK)
            {
                if (c.Alpha0To255 == BASE_MASK)
                {
                    this = c;
                }
                else
                {
                    cr = Red0To255 + c.Red0To255; Red0To255 = (cr > (int)(BASE_MASK)) ? (int)(BASE_MASK) : cr;
                    cg = Green0To255 + c.Green0To255; Green0To255 = (cg > (int)(BASE_MASK)) ? (int)(BASE_MASK) : cg;
                    cb = Blue0To255 + c.Blue0To255; Blue0To255 = (cb > (int)(BASE_MASK)) ? (int)(BASE_MASK) : cb;
                    ca = Alpha0To255 + c.Alpha0To255; Alpha0To255 = (ca > (int)(BASE_MASK)) ? (int)(BASE_MASK) : ca;
                }
            }
            else
            {
                cr = Red0To255 + ((c.Red0To255 * cover + COVER_MASK / 2) >> COVER_SHIFT);
                cg = Green0To255 + ((c.Green0To255 * cover + COVER_MASK / 2) >> COVER_SHIFT);
                cb = Blue0To255 + ((c.Blue0To255 * cover + COVER_MASK / 2) >> COVER_SHIFT);
                ca = Alpha0To255 + ((c.Alpha0To255 * cover + COVER_MASK / 2) >> COVER_SHIFT);
                Red0To255 = (cr > (int)(BASE_MASK)) ? (int)(BASE_MASK) : cr;
                Green0To255 = (cg > (int)(BASE_MASK)) ? (int)(BASE_MASK) : cg;
                Blue0To255 = (cb > (int)(BASE_MASK)) ? (int)(BASE_MASK) : cb;
                Alpha0To255 = (ca > (int)(BASE_MASK)) ? (int)(BASE_MASK) : ca;
            }
        }

        public void apply_gamma_dir(GammaLookUpTable gamma)
        {
            Red0To255 = gamma.dir((byte)Red0To255);
            Green0To255 = gamma.dir((byte)Green0To255);
            Blue0To255 = gamma.dir((byte)Blue0To255);
        }
         
        //-------------------------------------------------------------rgb8_packed
        static public ColorRGBA rgb8_packed(int v)
        {
            return new ColorRGBA((byte)((v >> 16) & 0xFF), (byte)((v >> 8) & 0xFF), ((byte)(v & 0xFF)));
        }

        public ColorRGBA Blend(ColorRGBA other, double weight)
        {
            ColorRGBA result = new ColorRGBA(this);
            result = this * (1 - weight) + other * weight;
            return result;
        }
#if DEBUG
        public override string ToString()
        {
            return "r:" + this.red + ",g:" + this.green + ",b:" + this.blue + ",a:" + this.alpha;
        }
#endif

    }
}