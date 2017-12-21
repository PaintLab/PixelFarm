// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

using System.Collections.Generic; 
using ColorBlender.Algorithms;

namespace ColorBlender
{
    public class ColorMatch
    {
        public IList<IAlgorithm> Algorithms { get; set; }
        public IAlgorithm CurrentAlgorithm { get; set; }
        public Blend CurrentBlend { get; set; }
        public RGB CurrentRGB { get; set; }
        public HSV CurrentHSV { get; set; }
        public RGB[] VariationsRGB { get; set; }
        public RGB[] VariationsHSV { get; set; }

        public ColorMatch()
        {
            Algorithms = new List<IAlgorithm>()
            {
                new Classic(),
                new ColorExplorer(),
                new SingleHue(),
                new Complementary(),
                new SplitComplementary(),
                new Analogue(),
                new Triadic(),
                new Square()
            };

            CurrentAlgorithm = Algorithms[0];
        }

        public ColorMatch(double h, double s, double v) : this()
        {
            VariationsRGB = new RGB[7];
            VariationsHSV = new RGB[9];

            CurrentHSV = new HSV(h, s, v);
            CurrentRGB = new RGB(CurrentHSV);

            Update();
        }

        public ColorMatch(HSV hsv) : this(hsv.H, hsv.S, hsv.V)
        {
        }

        private double AddLimit(double x, double d, double min, double max)
        {
            x = x + d;
            if (x < min) return min;
            if (x > max) return max;
            if ((x >= min) && (x <= max)) return x;

            return double.NaN;
        }

        private RGB HsvVariation(HSV hsv, double addsat, double addval)
        {
            var rgbobj = new RGB();
            var hsvobj = new HSV
            {
                H = hsv.H,
                S = hsv.S,
                V = hsv.V
            };

            hsvobj.S = AddLimit(hsvobj.S, addsat, 0, 99);
            hsvobj.V = AddLimit(hsvobj.V, addval, 0, 99);

            rgbobj = hsvobj.ToRGB();

            return rgbobj;
        }

        public void UpdateVariationsRGB()
        {
            double vv = 20;
            double vw = 10;

            VariationsRGB[0] = new RGB(AddLimit(CurrentRGB.R, -vw, 0, 255), AddLimit(CurrentRGB.G, vv, 0, 255), AddLimit(CurrentRGB.B, -vw, 0, 255));
            VariationsRGB[1] = new RGB(AddLimit(CurrentRGB.R, vw, 0, 255), AddLimit(CurrentRGB.G, vw, 0, 255), AddLimit(CurrentRGB.B, -vv, 0, 255));
            VariationsRGB[2] = new RGB(AddLimit(CurrentRGB.R, -vv, 0, 255), AddLimit(CurrentRGB.G, vw, 0, 255), AddLimit(CurrentRGB.B, vw, 0, 255));
            VariationsRGB[3] = new RGB(CurrentRGB.R, CurrentRGB.G, CurrentRGB.B);
            VariationsRGB[4] = new RGB(AddLimit(CurrentRGB.R, vv, 0, 255), AddLimit(CurrentRGB.G, -vw, 0, 255), AddLimit(CurrentRGB.B, -vw, 0, 255));
            VariationsRGB[5] = new RGB(AddLimit(CurrentRGB.R, -vw, 0, 255), AddLimit(CurrentRGB.G, -vw, 0, 255), AddLimit(CurrentRGB.B, vv, 0, 255));
            VariationsRGB[6] = new RGB(AddLimit(CurrentRGB.R, vw, 0, 255), AddLimit(CurrentRGB.G, -vv, 0, 255), AddLimit(CurrentRGB.B, vw, 0, 255));
        }

        public void UpdateVariationsHSV()
        {
            double vv = 10;

            VariationsHSV[0] = HsvVariation(CurrentHSV, -vv, vv);
            VariationsHSV[1] = HsvVariation(CurrentHSV, 0, vv);
            VariationsHSV[2] = HsvVariation(CurrentHSV, vv, vv);
            VariationsHSV[3] = HsvVariation(CurrentHSV, -vv, 0);
            VariationsHSV[4] = CurrentHSV.ToRGB();
            VariationsHSV[5] = HsvVariation(CurrentHSV, vv, 0);
            VariationsHSV[6] = HsvVariation(CurrentHSV, -vv, -vv);
            VariationsHSV[7] = HsvVariation(CurrentHSV, 0, -vv);
            VariationsHSV[8] = HsvVariation(CurrentHSV, vv, -vv);
        }

        public void Update()
        {
            CurrentBlend = CurrentAlgorithm.Match(CurrentHSV);
            UpdateVariationsRGB();
            UpdateVariationsHSV();
        }
    }
}
