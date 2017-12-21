// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

namespace ColorBlender.Algorithms
{
    public class SplitComplementary : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            var w = MathHelpers.HueToWheel(hsv.H);
            HSV z = new HSV
            {
                H = hsv.H,
                S = hsv.S,
                V = hsv.V
            };

            z.H = MathHelpers.WheelToHue((w + 150) % 360);
            z.S = hsv.S;
            z.V = hsv.V;
            outp.Colors[1] = new HSV(z);

            z.H = MathHelpers.WheelToHue((w + 210) % 360);
            z.S = hsv.S;
            z.V = hsv.V;
            outp.Colors[2] = new HSV(z);

            z.S = 0;
            z.V = hsv.S;
            outp.Colors[3] = new HSV(z);

            z.S = 0;
            z.V = hsv.V;
            outp.Colors[4] = new HSV(z);

            z.S = 0;
            z.V = (100 - hsv.V);
            outp.Colors[5] = new HSV(z);

            return outp;
        }
    }
}
