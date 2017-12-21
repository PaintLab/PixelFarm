// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System;

namespace ColorBlender
{
    internal static class MathHelpers
    {
        public static double RC(double x, double m)
        {
            if (x > m) { return m; }
            if (x < 0) { return 0; } else { return x; }
        }

        public static double HueToWheel(double h)
        {
            if (h <= 120)
            {
                return (Math.Round(h * 1.5));
            }
            else
            {
                return (Math.Round(180 + (h - 120) * 0.75));
            }
        }

        public static double WheelToHue(double w)
        {
            if (w <= 180)
            {
                return (Math.Round(w / 1.5));
            }
            else
            {
                return (Math.Round(120 + (w - 180) / 0.75));
            }
        }
    }
}
