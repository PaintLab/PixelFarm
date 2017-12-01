// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)


namespace ColorBlender.Algorithms
{
    public class SingleHue : IAlgorithm
    {
        public Blend Match(HSV hsv)
        {
            Blend outp = new Blend();
            outp.Colors[0] = new HSV(hsv);

            HSV z = new HSV
            {
                H = hsv.H,
                S = hsv.S,
                V = hsv.V + ((hsv.V < 50) ? 20 : -20)
            };
            outp.Colors[1] = new HSV(z);

            z.S = hsv.S;
            z.V = hsv.V + ((hsv.V < 50) ? 40 : -40);
            outp.Colors[2] = new HSV(z);

            z.S = hsv.S + ((hsv.S < 50) ? 20 : -20);
            z.V = hsv.V;
            outp.Colors[3] = new HSV(z);

            z.S = hsv.S + ((hsv.S < 50) ? 40 : -40);
            z.V = hsv.V;
            outp.Colors[4] = new HSV(z);

            z.S = hsv.S + ((hsv.S < 50) ? 40 : -40);
            z.V = hsv.V + ((hsv.V < 50) ? 40 : -40);
            outp.Colors[5] = new HSV(z);

            return outp;
        }
    }
}
