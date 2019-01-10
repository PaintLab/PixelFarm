//MIT, 2014-present, WinterDev
//MatterHackers 
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------



using PixelFarm.Drawing;

namespace PixelFarm.PathReconstruction
{
    public abstract class PixelEvaluator
    {
        public abstract bool CheckPixel(int pixelValue32);
        public abstract void SetStartColor(Color startColor);
    }

    public class ExactMatch : PixelEvaluator
    {
        int _startColorInt32;
        public ExactMatch()
        {

        }
        public override void SetStartColor(Color startColor)
        {
            _startColorInt32 =
             (startColor.red << PixelFarm.CpuBlit.PixelProcessing.CO.R_SHIFT) |
             (startColor.green << PixelFarm.CpuBlit.PixelProcessing.CO.G_SHIFT) |
             (startColor.blue << PixelFarm.CpuBlit.PixelProcessing.CO.B_SHIFT);
        }
        public override bool CheckPixel(int pixelValue32)
        {
            //ARGB
            return _startColorInt32 == pixelValue32;
        }
    }

    public class ToleranceMatch : PixelEvaluator
    {
        byte _tolerance0To255;
        //** only RGB?
        byte _red_min, _red_max;
        byte _green_min, _green_max;
        byte _blue_min, _blue_max;
        public ToleranceMatch(byte initTolerance)
        {
            _tolerance0To255 = initTolerance;
        }
        public byte Tolerance
        {
            get => _tolerance0To255;
            set => _tolerance0To255 = value;
        }
        static byte Clamp0_255(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return (byte)value;
        }
        public override void SetStartColor(Color startColor)
        {
            _red_min = Clamp0_255(startColor.R - _tolerance0To255);
            _red_max = Clamp0_255(startColor.R + _tolerance0To255);
            //
            _green_min = Clamp0_255(startColor.G - _tolerance0To255);
            _green_max = Clamp0_255(startColor.G + _tolerance0To255);
            //
            _blue_min = Clamp0_255(startColor.B - _tolerance0To255);
            _blue_max = Clamp0_255(startColor.B + _tolerance0To255);
        }
        public override bool CheckPixel(int pixelValue32)
        {
            int r = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.R_SHIFT) & 0xff;
            int g = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.G_SHIFT) & 0xff;
            int b = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.B_SHIFT) & 0xff;
            //range test
            return (r >= _red_min) && (r <= _red_max) &&
                   (g >= _green_min) && (g <= _green_max) &&
                   (b >= _blue_min) && (b <= _blue_max);
        }
    }


}