// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System;

namespace ColorBlender
{
    public class RGB
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        public RGB()
        {
        }

        public RGB(double r, double g, double b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public RGB(RGB rg)
        {
            this.R = rg.R;
            this.G = rg.G;
            this.B = rg.B;
        }

        public RGB(HSV hs)
        {
            RGB rg = hs.ToRGB();
            this.R = rg.R;
            this.G = rg.G;
            this.B = rg.B;
        }

        public HSV ToHSV()
        {
            HSV hs = new HSV();
            RGB rg = new RGB(this.R, this.G, this.B);

            var m = rg.R;
            if (rg.G < m) { m = rg.G; }
            if (rg.B < m) { m = rg.B; }
            var v = rg.R;
            if (rg.G > v) { v = rg.G; }
            if (rg.B > v) { v = rg.B; }
            var value = 100 * v / 255;
            var delta = v - m;
            if (v == 0.0) { hs.S = 0; } else { hs.S = 100 * delta / v; }

            if (hs.S == 0) { hs.H = 0; }
            else
            {
                if (rg.R == v) { hs.H = 60.0 * (rg.G - rg.B) / delta; }
                else if (rg.G == v) { hs.H = 120.0 + 60.0 * (rg.B - rg.R) / delta; }
                else if (rg.B == v) { hs.H = 240.0 + 60.0 * (rg.R - rg.G) / delta; }
                if (hs.H < 0.0) { hs.H = hs.H + 360.0; }
            }

            hs.H = Math.Round(hs.H);
            hs.S = Math.Round(hs.S);
            hs.V = Math.Round(value);

            return hs;
        }
    }
}
