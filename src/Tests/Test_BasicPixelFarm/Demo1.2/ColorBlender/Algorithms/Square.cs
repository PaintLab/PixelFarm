// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

namespace ColorBlender.Algorithms
{
    public class Square : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            var w = MathHelpers.HueToWheel(hsv.H);
            HSV z = new HSV
            {
                H = MathHelpers.WheelToHue((w + 90) % 360),
                S = hsv.S,
                V = hsv.V
            };
            outp.Colors[1] = new HSV(z);

            z.H = MathHelpers.WheelToHue((w + 180) % 360);
            z.S = hsv.S;
            z.V = hsv.V;
            outp.Colors[2] = new HSV(z);

            z.H = MathHelpers.WheelToHue((w + 270) % 360);
            z.S = hsv.S;
            z.V = hsv.V;
            outp.Colors[3] = new HSV(z);

            z.S = 0;
            outp.Colors[4] = new HSV(z);

            z.V = 100 - z.V;
            outp.Colors[5] = new HSV(z);

            return outp;
        }
    }
}
