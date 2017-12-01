// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System;

namespace ColorBlender
{
    public class HSV
    {
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }

        public HSV()
        {
        }

        public HSV(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        public HSV(HSV hs)
        {
            this.H = hs.H;
            this.S = hs.S;
            this.V = hs.V;
        }

        public HSV(RGB rg)
        {
            HSV hs = rg.ToHSV();
            this.H = hs.H;
            this.S = hs.S;
            this.V = hs.V;
        }

        public RGB ToRGB()
        {
            RGB rg = new RGB();
            HSV hsx = new HSV(this.H, this.S, this.V);

            if (hsx.S == 0)
            {
                rg.R = rg.G = rg.B = Math.Round(hsx.V * 2.55); return (rg);
            }

            hsx.S = hsx.S / 100;
            hsx.V = hsx.V / 100;
            hsx.H /= 60;

            var i = Math.Floor(hsx.H);
            var f = hsx.H - i;
            var p = hsx.V * (1 - hsx.S);
            var q = hsx.V * (1 - hsx.S * f);
            var t = hsx.V * (1 - hsx.S * (1 - f));

            switch ((int)i)
            {
                case 0: rg.R = hsx.V; rg.G = t; rg.B = p; break;
                case 1: rg.R = q; rg.G = hsx.V; rg.B = p; break;
                case 2: rg.R = p; rg.G = hsx.V; rg.B = t; break;
                case 3: rg.R = p; rg.G = q; rg.B = hsx.V; break;
                case 4: rg.R = t; rg.G = p; rg.B = hsx.V; break;
                default: rg.R = hsx.V; rg.G = p; rg.B = q; break;
            }

            rg.R = Math.Round(rg.R * 255);
            rg.G = Math.Round(rg.G * 255);
            rg.B = Math.Round(rg.B * 255);

            return rg;
        }
    }
}
