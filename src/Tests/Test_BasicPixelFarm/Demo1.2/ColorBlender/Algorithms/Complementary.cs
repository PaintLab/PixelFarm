// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System;

namespace ColorBlender.Algorithms
{
    public class Complementary : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            HSV z = new HSV
            {
                H = hsv.H,
                S = (hsv.S > 50) ? (hsv.S * 0.5) : (hsv.S * 2),
                V = (hsv.V < 50) ? (Math.Min(hsv.V * 1.5, 100)) : (hsv.V / 1.5)
            };
            outp.Colors[1] = new HSV(z);

            var w = MathHelpers.HueToWheel(hsv.H);
            z.H = MathHelpers.WheelToHue((w + 180) % 360);
            z.S = hsv.S;
            z.V = hsv.V;
            outp.Colors[2] = new HSV(z);

            z.S = (z.S > 50) ? (z.S * 0.5) : (z.S * 2);
            z.V = (z.V < 50) ? (Math.Min(z.V * 1.5, 100)) : (z.V / 1.5);
            outp.Colors[3] = new HSV(z);

            z = new HSV
            {
                S = 0,
                H = 0,
                V = hsv.V
            };
            outp.Colors[4] = new HSV(z);

            z.V = 100 - hsv.V;
            outp.Colors[5] = new HSV(z);

            return outp;
        }
    }
}
