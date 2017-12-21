// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System;

namespace ColorBlender.Algorithms
{
    public class Analogue : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            var w = MathHelpers.HueToWheel(hsv.H);
            HSV z = new HSV
            {
                H = MathHelpers.WheelToHue((w + 30) % 360),
                S = hsv.S,
                V = hsv.V
            };
            outp.Colors[1] = new HSV(z);

            z = new HSV
            {
                H = MathHelpers.WheelToHue((w + 60) % 360),
                S = hsv.S,
                V = hsv.V
            };
            outp.Colors[2] = new HSV(z);

            z = new HSV
            {
                S = 0,
                H = 0,
                V = 100 - hsv.V
            };
            outp.Colors[3] = new HSV(z);

            z.V = Math.Round(hsv.V * 1.3) % 100;
            outp.Colors[4] = new HSV(z);

            z.V = Math.Round(hsv.V / 1.3) % 100;
            outp.Colors[5] = new HSV(z);

            return outp;
        }
    }
}
