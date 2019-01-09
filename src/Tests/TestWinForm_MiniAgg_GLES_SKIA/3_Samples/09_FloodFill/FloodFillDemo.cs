//BSD, 2014-present, WinterDev
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
using PixelFarm.PathReconstruction;
using Mini;

namespace PixelFarm.CpuBlit.Sample_FloodFill
{
    public enum ToolMode
    {
        ColorBucket,
        MagicWand,
    }


    [Info(OrderCode = "09", AvailableOn = AvailableOn.Agg)]
    [Info(DemoCategory.Bitmap, "Demonstration of a flood filling algorithm.")]
    public class FloodFillDemo : DemoBase
    {

        public enum ImageOption
        {
            Default,
            Lion,
            Stars,
            TestGlyphs,
            Rect01,
            VShape,
        }
        ImageOption _imgOption;

        MemBitmap _bmpToFillOn;

        MemBitmap _lionPng;
        MemBitmap _defaultImg;
        MemBitmap _starsPng;
        MemBitmap _test_glyphs;
        MemBitmap _rect01;
        MemBitmap _v_shape;



        int _imgOffsetX = 20;
        int _imgOffsetY = 60;
        byte _tolerance = 0;

        VertexStore _testReconstructedVxs;
        ColorBucket _floodFill;
        MagicWand _magicWand;

        bool _doOutlineRecon;
        bool _doOutlineSimplifier;
        bool _onlyOutlineReconstruction;

        public FloodFillDemo()
        {
            //
            BackgroundColor = Color.White;

            _defaultImg = new MemBitmap(400, 300);

            AggPainter p = AggPainter.Create(_defaultImg);
            p.Clear(Color.White);
            p.FillColor = Color.Black;
            p.FillEllipse(20, 20, 30, 30);


            for (int i = 0; i < 20; i++)
            {
                p.StrokeColor = Color.Black;
                p.DrawEllipse(i * 10, i * 10, 20, 20);
            }

            //
            this.PixelSize = 32;
            this.Gamma = 1;
            _tolerance = 30;

            _floodFill = new ColorBucket(Color.Red, _tolerance);
            _magicWand = new MagicWand(_tolerance);

            //
            //_lionPng = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/lion1_v2_2.png");
            //_lionPng = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/lion1_v2_4_1.png");
            _lionPng = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/lion1.png");
            //_lionPng = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/glyph_a.png");
            _starsPng = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/stars.png");
            _test_glyphs = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/test_glyphs.png");
            _rect01 = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/rect01.png");
            //_v_shape = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/shape_v.png");
            _v_shape = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/shape_v3.png");
            _bmpToFillOn = _defaultImg;

            OutlineReconstruction = true;
            WithOutlineSimplifier = true;
        }

        [DemoConfig]
        public ToolMode ToolMode { get; set; }
        [DemoConfig]
        public bool OutlineReconstruction
        {
            get => _doOutlineRecon;
            set
            {
                _doOutlineRecon = value;
                this.InvalidateGraphics();
            }
        }
        [DemoConfig]
        public bool WithOutlineSimplifier
        {
            get => _doOutlineSimplifier;
            set
            {
                _doOutlineSimplifier = value;
                this.InvalidateGraphics();
            }
        }
        [DemoConfig]
        public bool OnlyOutlineReconstruction
        {
            get => _onlyOutlineReconstruction;
            set
            {
                _onlyOutlineReconstruction = value;
                this.InvalidateGraphics();
            }
        }
        [DemoConfig]
        public ImageOption SelectedImageOption
        {
            get => _imgOption;
            set
            {
                _imgOption = value;
                switch (value)
                {
                    default:
                        _bmpToFillOn = _defaultImg;
                        break;
                    case ImageOption.Stars:
                        _bmpToFillOn = _starsPng;
                        break;
                    case ImageOption.Lion:
                        _bmpToFillOn = _lionPng;
                        break;
                    case ImageOption.TestGlyphs:
                        _bmpToFillOn = _test_glyphs;
                        break;
                    case ImageOption.VShape:
                        _bmpToFillOn = _v_shape;
                        break;
                    case ImageOption.Rect01:
                        _bmpToFillOn = _rect01;
                        break;
                }
                this.InvalidateGraphics();
            }
        }
        //[DemoConfig(MinValue = 8, MaxValue = 100)]
        public int PixelSize
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 3)]
        public double Gamma
        {
            get;
            set;
        }
        [DemoConfig(MinValue = 0, MaxValue = 255)]
        public byte Tolerance
        {
            get => _tolerance;
            set
            {
                _tolerance = value;
                _floodFill.Update(_floodFill.FillColor, (byte)value);
                _magicWand.Tolerance = value;

                //
                InvalidateGraphics();
            }

        }
        public Color BackgroundColor
        {
            get;
            set;
        }

        public override void Draw(Painter p)
        {
            p.Clear(Color.Blue);

            p.DrawImage(_bmpToFillOn, _imgOffsetX, _imgOffsetY);

            p.FillColor = Color.Yellow;
            p.FillEllipse(20, 20, 30, 30);

            if (_testReconstructedVxs != null)
            {
                p.Draw(_testReconstructedVxs, Color.Blue);
            }
        }



        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            int x = mx - _imgOffsetX;
            int y = my - _imgOffsetY;

            SpanBasedRegion spanCollectionOutput = null;
            if (ToolMode == ToolMode.MagicWand)
            {
                spanCollectionOutput = new SpanBasedRegion();
                _magicWand.CollectRegion(_bmpToFillOn, x, y, spanCollectionOutput);
            }
            else
            {
                _floodFill.SkipActualFill = this.OnlyOutlineReconstruction;

                if (!OutlineReconstruction)
                {
                    //just fill only, no outline reconstruction
                    _floodFill.Fill(_bmpToFillOn, x, y);
                    _testReconstructedVxs = null;
                }
                else
                {
                    //for flood-fill => ConnectedHSpans is optional
                    spanCollectionOutput = new SpanBasedRegion();
                    _floodFill.Fill(_bmpToFillOn, x, y, spanCollectionOutput);
                }
            }

            //try tracing for vxs
            if (spanCollectionOutput != null)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                {
                    HSpansRegionOutline rawPath = new HSpansRegionOutline(); 
                    spanCollectionOutput.ReconstructOutline(rawPath);
                    //convert path to vxs
                    //or do optimize raw path/simplify line and curve before  gen vxs 
                    // test simplify the path  
                    if (WithOutlineSimplifier)
                    {
                        rawPath.Simplify();
                    }
                    rawPath.MakeVxs(v1);
                    var tx = VertexProcessing.Affine.NewTranslation(_imgOffsetX, _imgOffsetY);
                    _testReconstructedVxs = v1.CreateTrim(tx);
                }
            }
            //---
            this.InvalidateGraphics();
        }

    }
}
