//BSD, 2011-2019, Gregor Aisch, Chroma.js, https://github.com/gka/chroma.js/blob/master/LICENSE

using PaintLab.Colourful;
using PaintLab.Colourful.Conversion;
using System;
using PixelFarm.Drawing;
namespace PaintLab.ChromaJs
{
    public class Chroma
    {
        RGBColor _rgb;
        Color _argb;
        double _alpha = 1;
        ColourfulConverter _converter = new ColourfulConverter();

        public Chroma(Color color)
        {
            _argb = color;
            _rgb = new RGBColor((double)color.R / 255, (double)color.G / 255, (double)color.B / 255);
            _alpha = color.A / 255;
        }

        public Color Darken(double amount = 1)
        {
            LabColor lab = this.Lab();
            LabColor lab2 = new LabColor(lab.L - (LabConsts.Kn * amount), lab.a, lab.b);
            RGBColor rgb = _converter.ToRGB(lab2);
            return Alpha(rgb, Alpha(), true);
        }

        public Color Brighten(double amount = 1)
        {
            return Darken(-amount);
        }

        public Color Saturate(double amount = 1)
        {
            LChabColor lch = _converter.ToLChab(_rgb);
            double C = lch.C + (LabConsts.Kn * amount);
            if (C < 0)
            {
                C = 0;
            }
            LChabColor lch2 = new LChabColor(lch.L, C, lch.h);
            RGBColor rgb = _converter.ToRGB(lch2);
            return Alpha(rgb, Alpha(), true);
        }

        public Color Desaturate(double amount = 1)
        {
            return Saturate(-amount);
        }

        #region Luminance
        const double EPS = 1e-7;
        const int MAX_ITER = 20;
        public Color Luminance(double lum)
        {
            if (lum == 0)
            {
                // return pure black
                return Color.Black;
            }
            if (lum == 1)
            {
                // return pure white
                return Color.White;
            }
            double cur_lum = this.Luminance();
            int maxIter = MAX_ITER;
            Color test(Chroma low, Chroma high)
            {
                Chroma mid = low.Interpolate(low, high, 0.5);
                double lm = mid.Luminance();
                if (Math.Abs(lum - lm) < EPS || (maxIter--) > 0)
                {
                    return ToArgb(mid._rgb, (byte)(Alpha() * 255));
                }
                return lm > lum ? test(low, mid) : test(mid, high);
            }
            Color rgb = cur_lum > lum ? test(new Chroma(Color.Black), this) : test(this, new Chroma(Color.White));
            return rgb;
        }

        public double Luminance()
        {
            double temp = Rgb2Luminance(_argb.R, _argb.G, _argb.B);
            return temp;
        }

        private double Rgb2Luminance(double r, double g, double b)
        {
            // relative luminance
            // see http://www.w3.org/TR/2008/REC-WCAG20-20081211/#relativeluminancedef
            r = Luminance_X(r);
            g = Luminance_X(g);
            b = Luminance_X(b);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private double Luminance_X(double x)
        {
            x /= 255;
            return x <= 0.03928 ? x / 12.92 : Math.Pow((x + 0.055) / 1.055, 2.4);
        }

        private Chroma Interpolate(Chroma color1, Chroma color2, double f)
        {
            RGBColor rgb1 = color1._rgb;
            RGBColor rgb2 = color2._rgb;
            RGBColor result = new RGBColor(
                rgb1.R + f * (rgb2.R - rgb1.R),
                rgb1.G + f * (rgb2.G - rgb1.G),
                rgb1.B + f * (rgb2.B - rgb1.B)
                );
            return new Chroma(ToArgb(result, 255));
        }
        #endregion

        public LabColor Lab()
        {
            return _converter.ToLab(_rgb);
        }

        public double Alpha()
        {
            return _alpha;
        }

        public Color Alpha(RGBColor rgb, double alpha, bool mutate = false)
        {
            byte a = (byte)(Alpha() * 255);
            if (mutate)
            {
                a = (byte)(alpha * 255);
            }
            Color color = ToArgb(rgb, a);
            return color;
        }

        private static Color ToArgb(RGBColor rgb, byte alpha)
        {
            return Color.FromArgb(alpha, (byte)(rgb.R * 255), (byte)(rgb.G * 255), (byte)(rgb.B * 255));
        }
    }

    public static class LabConsts
    {
        // Corresponds roughly to RGB brighter/darker
        public const float Kn = 18;

        //D65 standard referent
        public const float Xn = 0.950470f;
        public const float Yn = 1;
        public const float Zn = 1.088830f;

        public const float t0 = 4f / 29;
        public const float t1 = 6f / 29;
        public const float t2 = 3 * t1 * t1;
        public const float t3 = t1 * t1 * t1;

    }
}