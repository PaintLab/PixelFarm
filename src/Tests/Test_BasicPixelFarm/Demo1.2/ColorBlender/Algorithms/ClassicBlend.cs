// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

namespace ColorBlender.Algorithms
{
    public class Classic : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            HSV y = new HSV();
            HSV yx = new HSV();

            y.S = hsv.S;
            y.H = hsv.H;
            if (hsv.V > 70) { y.V = hsv.V - 30; } else { y.V = hsv.V + 30; };
            outp.Colors[1] = new HSV(y);

            if ((hsv.H >= 0) && (hsv.H < 30))
            {
                yx.H = y.H = hsv.H + 30; yx.S = y.S = hsv.S; y.V = hsv.V;
                if (hsv.V > 70) { yx.V = hsv.V - 30; } else { yx.V = hsv.V + 30; }
            }

            if ((hsv.H >= 30) && (hsv.H < 60))
            {
                yx.H = y.H = hsv.H + 150;
                y.S = MathHelpers.RC(hsv.S - 30, 100);
                y.V = MathHelpers.RC(hsv.V - 20, 100);
                yx.S = MathHelpers.RC(hsv.S - 50, 100);
                yx.V = MathHelpers.RC(hsv.V + 20, 100);
            }

            if ((hsv.H >= 60) && (hsv.H < 180))
            {
                yx.H = y.H = hsv.H - 40;
                y.S = yx.S = hsv.S;
                y.V = hsv.V; if (hsv.V > 70) { yx.V = hsv.V - 30; } else { yx.V = hsv.V + 30; }
            }

            if ((hsv.H >= 180) && (hsv.H < 220))
            {
                yx.H = hsv.H - 170;
                y.H = hsv.H - 160;
                yx.S = y.S = hsv.S;
                y.V = hsv.V;
                if (hsv.V > 70) { yx.V = hsv.V - 30; } else { yx.V = hsv.V + 30; }

            }
            if ((hsv.H >= 220) && (hsv.H < 300))
            {
                yx.H = y.H = hsv.H;
                yx.S = y.S = MathHelpers.RC(hsv.S - 40, 100);
                y.V = hsv.V;
                if (hsv.V > 70) { yx.V = hsv.V - 30; } else { yx.V = hsv.V + 30; }
            }
            if (hsv.H >= 300)
            {
                if (hsv.S > 50) { y.S = yx.S = hsv.S - 40; } else { y.S = yx.S = hsv.S + 40; }
                yx.H = y.H = (hsv.H + 20) % 360;
                y.V = hsv.V;
                if (hsv.V > 70) { yx.V = hsv.V - 30; } else { yx.V = hsv.V + 30; }
            }

            outp.Colors[2] = new HSV(y);
            outp.Colors[3] = new HSV(yx);

            y.H = 0;
            y.S = 0;
            y.V = 100 - hsv.V;
            outp.Colors[4] = new HSV(y);

            y.H = 0;
            y.S = 0;
            y.V = hsv.V;
            outp.Colors[5] = new HSV(y);

            return outp;
        }
    }
}
