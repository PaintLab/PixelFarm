// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System;

namespace ColorBlender.Algorithms
{
    public class ColorExplorer : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            HSV z = new HSV
            {
                H = hsv.H,
                S = Math.Round(hsv.S * 0.3),
                V = Math.Min(Math.Round(hsv.V * 1.3), 100)
            };
            outp.Colors[1] = new HSV(z);

            z = new HSV
            {
                H = (hsv.H + 300) % 360,
                S = hsv.S,
                V = hsv.V
            };
            outp.Colors[3] = new HSV(z);

            z.S = Math.Min(Math.Round(z.S * 1.2), 100);
            z.V = Math.Min(Math.Round(z.V * 0.5), 100);
            outp.Colors[2] = new HSV(z);

            z.S = 0;
            z.V = (hsv.V + 50) % 100;
            outp.Colors[4] = new HSV(z);

            z.V = (z.V + 50) % 100;
            outp.Colors[5] = new HSV(z);

            return outp;
        }
    }
}
