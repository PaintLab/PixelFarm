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

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using CO = PixelFarm.CpuBlit.PixelProcessing.CO;

namespace PixelFarm.PathReconstruction
{
    /// <summary>
    /// solid color bucket tool
    /// </summary>
    public class ColorBucket
    {
        byte _tolerance;
        Color _fillColor;

        PixelEvaluatorBitmap32 _fillEval;
        FloodFillRunner _floodRunner = new FloodFillRunner();


        class FillWithExactMatch : ExactMatch
        {
            int _fillColorInt32;
            public FillWithExactMatch(int fillColorInt32)
            {
                _fillColorInt32 = fillColorInt32;
            }
            protected override unsafe bool CheckPixel(int* pixelAddr)
            {
                int value = *pixelAddr;
                if (base.CheckPixel(pixelAddr))
                {
                    *pixelAddr = _fillColorInt32;
                    return true;
                }
                return false;
            }
        }
        class FillWithTolerance : ToleranceMatch
        {
            int _fillColorInt32;
            public FillWithTolerance(int fillColorInt32, byte tolerance) : base(tolerance)
            {
                _fillColorInt32 = fillColorInt32;
            }
            protected override unsafe bool CheckPixel(int* pixelAddr)
            {
                if (base.CheckPixel(pixelAddr))
                {
                    *pixelAddr = _fillColorInt32;
                    return true;
                }
                return false;
            }
        }

        public ColorBucket(Color fillColor)
          : this(fillColor, 0)
        {
        }
        public ColorBucket(Color fillColor, byte tolerance)
        {
            Update(fillColor, tolerance);
        }
        public Color FillColor => _fillColor;
        public byte Tolerance => _tolerance;

        public void Update(Color fillColor, byte tolerance)
        {
            _tolerance = tolerance;
            _fillColor = fillColor;

            int fillColorInt32 =
                (_fillColor.red << CO.R_SHIFT) |
                (_fillColor.green << CO.G_SHIFT) |
                (_fillColor.blue << CO.B_SHIFT) |
                (_fillColor.alpha << CO.A_SHIFT);

            if (tolerance > 0)
            {
                _fillEval = new FillWithTolerance(fillColorInt32, tolerance);
            }
            else
            {
                _fillEval = new FillWithExactMatch(fillColorInt32);
            }
        }


        /// <summary>
        /// fill target bmp, start at (x,y), 
        /// </summary>
        /// <param name="bmpTarget"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="output"></param>
        public void Fill(MemBitmap bmp, int x, int y, ReconstructedRegionData output = null)
        {
            if (x < bmp.Width && y < bmp.Height)
            {
                _fillEval.SetSourceBitmap(bmp);
                output.HSpans = _floodRunner.InternalFill(_fillEval, x, y, output != null);
                _fillEval.ReleaseSourceBitmap();
            }
        }
    }

    public class MagicWand
    {
        byte _tolerance;
        PixelEvaluatorBitmap32 _pixelEvalutor;
        FloodFillRunner _floodRunner = new FloodFillRunner();

        public MagicWand(byte tolerance)
        {
            //no actual fill  
            Tolerance = tolerance;
        }
        public byte Tolerance
        {
            get => _tolerance;
            set
            {
                _tolerance = value;
                //set new pixel evaluator 
                if (value > 0)
                {
                    _pixelEvalutor = new ToleranceMatch(value);
                }
                else
                {
                    _pixelEvalutor = new ExactMatch();
                }
            }
        }
        /// <summary>
        /// collect hspans into output region data
        /// </summary>
        /// <param name="bmpTarget"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="output"></param>
        public void CollectRegion(MemBitmap bmp, int x, int y, ReconstructedRegionData output)
        {
            if (x < bmp.Width && y < bmp.Height)
            {
                _pixelEvalutor.SetSourceBitmap(bmp);
                output.HSpans = _floodRunner.InternalFill(_pixelEvalutor, x, y, output != null);
                _pixelEvalutor.ReleaseSourceBitmap();
            }
        }
    }

}