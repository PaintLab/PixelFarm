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

    public enum TodoWithMagicWandProduct
    {
        Nothing,
        CreateMaskBitmapAndFill,
        CreateBitmapRgnAndFill,
        CreateVxsRgnAndFill,
        CreateVxsRgnAndDraw,
        CreateVxsRgnAndDeleteSelectedArea,
        CreateVxsRgnAndCopyToClipboard
    }
    [DemoConfigGroup]
    public class MagicWandConfigGroup
    {
        [DemoConfig]
        public TodoWithMagicWandProduct TodoWithMagicWandProduct { get; set; }

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

        VertexStore _reconstructedRgn;
        ColorBucket _floodFill;
        MagicWand _magicWand;

        bool _doOutlineRecon;
        bool _doOutlineSimplifier;
        bool _onlyOutlineReconstruction;

        ReconstructedRegionData _tmpMagicWandRgnData;

        MagicWandConfigGroup _magicWandConfigs;


        public FloodFillDemo()
        {
            //
            _magicWandConfigs = new MagicWandConfigGroup();

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
            //_lionPng = MemBitmap.LoadBitmap("../Data/lion1_v2_2.png");
            //_lionPng =  MemBitmap.LoadBitmap("../Data/lion1_v2_4_1.png");
            _lionPng = MemBitmap.LoadBitmap("../Data/lion1.png");
            //_lionPng =  MemBitmap.LoadBitmap("../Data/lion_1_v3_2.png");
            //_lionPng = MemBitmap.LoadBitmap("../Data/glyph_a.png");
            _starsPng = MemBitmap.LoadBitmap("../Data/stars.png");
            _test_glyphs = MemBitmap.LoadBitmap("../Data/test_glyphs.png");
            _rect01 = MemBitmap.LoadBitmap("../Data/rect01.png");
            //_v_shape =  MemBitmap.LoadBitmap("../Data/shape_v.png");
            //_v_shape = MemBitmap.LoadBitmap("../Data/shape_v3.png");
            _bmpToFillOn = _defaultImg;

            OutlineReconstruction = true;
            WithOutlineSimplifier = true;
        }


        [DemoAction]
        public void DoAutoFill()
        {
            //test auto fill at 20,20
            RunAutoFill(20, 20);
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
        public int PixelSize { get; set; }
        [DemoConfig(MaxValue = 3)]
        public double Gamma { get; set; }
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
        public Color BackgroundColor { get; set; }


        [DemoConfig]
        public MagicWandConfigGroup MagicWandConfig => _magicWandConfigs;

        public override void Draw(Painter p)
        {
            p.Clear(Color.Blue);

            p.DrawImage(_bmpToFillOn, _imgOffsetX, _imgOffsetY);

            p.FillColor = Color.Yellow;
            p.FillEllipse(20, 20, 30, 30);

            if (_reconstructedRgn != null)
            {
                p.Draw(_reconstructedRgn, Color.Blue);
            }

            switch (_magicWandConfigs.TodoWithMagicWandProduct)
            {
                case TodoWithMagicWandProduct.CreateMaskBitmapAndFill:
                    if (_tmpMaskBitmap != null)
                    {
                        p.DrawImage(_tmpMaskBitmap, _imgOffsetX + _rgnBounds.X, _imgOffsetY + _rgnBounds.Y);
                    }
                    break;
                case TodoWithMagicWandProduct.CreateBitmapRgnAndFill:
                case TodoWithMagicWandProduct.CreateVxsRgnAndFill:
                    if (_tmpRgn != null)
                    {
                        p.Fill(_tmpRgn);
                    }
                    break;
                case TodoWithMagicWandProduct.CreateVxsRgnAndDraw:
                    if (_tmpRgn != null)
                    {
                        p.Draw(_tmpRgn);
                    }
                    break;
                case TodoWithMagicWandProduct.CreateVxsRgnAndCopyToClipboard:
                case TodoWithMagicWandProduct.CreateVxsRgnAndDeleteSelectedArea:
                    if (_tmpRgn != null)
                    {
                        //fill the rgn with some other color
                        //or bg color
                        p.Fill(_tmpRgn, Color.White);
                    }
                    break;

            }
        }

        static void PutBitmapToClipboardPreserveAlpha(System.Drawing.Bitmap bmp)
        {
            //save to png
            string tmpfilename = System.Windows.Forms.Application.CommonAppDataPath + "\\clipboard_tmp.png";

            using (System.IO.FileStream fs = new System.IO.FileStream(tmpfilename, System.IO.FileMode.Create))
            {
                bmp.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            }
            var fileList = new System.Collections.Specialized.StringCollection();
            fileList.Add(tmpfilename);
            System.Windows.Forms.Clipboard.SetFileDropList(fileList);

        }
        static System.Drawing.Bitmap CreatePlatformBitmap(MemBitmap memBmp)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
                memBmp.Width,
                memBmp.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var srcPtr = MemBitmap.GetBufferPtr(memBmp);

            var bmpdata = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, memBmp.Width, memBmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            {
                MemMx.memcpy(
                    (byte*)bmpdata.Scan0,
                    (byte*)srcPtr.Ptr,
                     srcPtr.LengthInBytes);
            }
            bmp.UnlockBits(bmpdata);
            return bmp;
        }
        //-------------------------
        //for wanding-tool test
        MemBitmap _tmpMaskBitmap;
        Region _tmpRgn;
        Drawing.Rectangle _rgnBounds;
        //-------------------------


        void RunAutoFill(int x, int y)
        {
            //1. 
            ReconstructedRegionData rgnData = new ReconstructedRegionData();

            if (!OutlineReconstruction)
            {
                //just fill only, no outline reconstruction
                //_floodFill.Fill(_bmpToFillOn, 0, 0);

                _floodFill.AutoFill(_bmpToFillOn, 0, 0, _bmpToFillOn.Width, _bmpToFillOn.Height, null);
                _reconstructedRgn = null;
            }
            else
            {
                try
                {
                    //_floodFill.Fill(_bmpToFillOn, 0, 0);

                    //for flood-fill => ConnectedHSpans is optional
                    //rgnData = new ReconstructedRegionData();
                    List<ReconstructedRegionData> rgnList = new List<ReconstructedRegionData>();
                    _floodFill.AutoFill(_bmpToFillOn, 0, 0, _bmpToFillOn.Width, _bmpToFillOn.Height, rgnList);
                    //_floodFill.AutoFill(_bmpToFillOn, 0, 0, 100, 100, rgnList);
                    //_floodFill.Fill(_bmpToFillOn, x, y, rgnData);
                }
                catch (Exception ex)
                {

                }
            }


            //try tracing for vxs
            if (rgnData != null)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                {
                    RawOutline rawOutline = new RawOutline();
                    rgnData.ReconstructOutline(rawOutline);

                    //convert path to vxs
                    //or do optimize raw path/simplify line and curve before  gen vxs 
                    // test simplify the path  
                    if (WithOutlineSimplifier)
                    {
                        rawOutline.Simplify();
                    }

                    rawOutline.MakeVxs(v1);
                    var tx = VertexProcessing.Affine.NewTranslation(_imgOffsetX, _imgOffsetY);
                    _reconstructedRgn = v1.CreateTrim(tx);
                }
            }
        }

        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            int x = mx - _imgOffsetX;
            int y = my - _imgOffsetY;

            _tmpMagicWandRgnData = null;
            if (_tmpMaskBitmap != null)
            {
                _tmpMaskBitmap.Dispose();
                _tmpMaskBitmap = null;
            }

            if (_tmpRgn != null)
            {
                _tmpRgn.Dispose();
                _tmpRgn = null;
            }

            ReconstructedRegionData rgnData = new ReconstructedRegionData();

            if (ToolMode == ToolMode.MagicWand)
            {

                _magicWand.CollectRegion(_bmpToFillOn, x, y, rgnData);
                //
                _tmpMagicWandRgnData = rgnData;



                if (rgnData != null)
                {
                    using (VxsTemp.Borrow(out VertexStore v1))
                    {
                        RawOutline rawOutline = new RawOutline();
                        rgnData.ReconstructOutline(rawOutline);

                        //convert path to vxs
                        //or do optimize raw path/simplify line and curve before  gen vxs 
                        // test simplify the path  
                        if (WithOutlineSimplifier)
                        {
                            rawOutline.Simplify();
                        }

                        rawOutline.MakeVxs(v1);
                        var tx = VertexProcessing.Affine.NewTranslation(_imgOffsetX, _imgOffsetY);
                        _reconstructedRgn = v1.CreateTrim(tx);
                    }
                }

                //...
                //example...
                //from the reconstructed rgn data
                //we can trace the outline (see below)
                //or create a CpuBlitRegion  
                switch (_magicWandConfigs.TodoWithMagicWandProduct)
                {
                    case TodoWithMagicWandProduct.CreateMaskBitmapAndFill:
                        {
                            _tmpMaskBitmap = rgnData.CreateMaskBitmap();
                            _rgnBounds = rgnData.GetBounds();
                        }
                        break;
                    case TodoWithMagicWandProduct.CreateBitmapRgnAndFill:
                        {
                            _tmpRgn = new BitmapBasedRegion(rgnData);
                        }
                        break;
                    case TodoWithMagicWandProduct.CreateVxsRgnAndFill:
                    case TodoWithMagicWandProduct.CreateVxsRgnAndDraw:
                    case TodoWithMagicWandProduct.CreateVxsRgnAndDeleteSelectedArea:
                        {
                            _tmpRgn = new VxsRegion(_reconstructedRgn);
                        }
                        break;
                    case TodoWithMagicWandProduct.CreateVxsRgnAndCopyToClipboard:
                        {
                            _tmpRgn = new VxsRegion(_reconstructedRgn);
                            //we may have some choices
                            //1). create a new dst bitmap -> set mask with the rgn -> and draw img from src to the bitmap
                            // or 
                            //2). if we have exact ReconstructedRegionData then
                            //   create a new dst bitmap-> direct copy src to dst look up with ReconstructedRegionData                        
                            //
                            //this version, we use 1)


                            Rectangle bounds = _tmpRgn.GetRectBounds();
                            using (MemBitmap bmp = new MemBitmap(bounds.Width, bounds.Height))
                            using (Tools.BorrowAggPainter(bmp, out AggPainter painter))
                            {
                                painter.Clear(Color.Transparent); //clear bg color

                                float prevX = painter.OriginX;
                                float prevY = painter.OriginY;
                                painter.SetOrigin(-bounds.Left, -bounds.Top);
                                painter.SetClipRgn(_reconstructedRgn);
                                painter.EnableBuiltInMaskComposite = true;

                                //painter.Fill(_tmpRgn, Color.Blue);
                                painter.DrawImage(_bmpToFillOn, 0, 0);
                                painter.SetOrigin(prevX, prevY);

                                //copy to clipboard
                                //convert to platform specific bitmap data
                                painter.EnableBuiltInMaskComposite = false;
                                using (var platformBmp = CreatePlatformBitmap(bmp))
                                {
                                    //PutBitmapToClipboardPreserveAlpha(platformBmp);
                                    System.Windows.Forms.Clipboard.SetImage(platformBmp);
                                }
                            }

                        }
                        break;
                }
            }
            else
            {

                if (!OutlineReconstruction)
                {
                    //just fill only, no outline reconstruction
                    _floodFill.Fill(_bmpToFillOn, x, y);
                    _reconstructedRgn = null;
                }
                else
                {
                    //for flood-fill => ConnectedHSpans is optional
                    rgnData = new ReconstructedRegionData();
                    _floodFill.Fill(_bmpToFillOn, x, y, rgnData);
                }


                //try tracing for vxs
                if (rgnData != null)
                {
                    using (VxsTemp.Borrow(out VertexStore v1))
                    {
                        RawOutline rawOutline = new RawOutline();
                        rgnData.ReconstructOutline(rawOutline);

                        //convert path to vxs
                        //or do optimize raw path/simplify line and curve before  gen vxs 
                        // test simplify the path  
                        if (WithOutlineSimplifier)
                        {
                            rawOutline.Simplify();
                        }

                        rawOutline.MakeVxs(v1);
                        var tx = VertexProcessing.Affine.NewTranslation(_imgOffsetX, _imgOffsetY);
                        _reconstructedRgn = v1.CreateTrim(tx);
                    }
                }
            }


            //---
            this.InvalidateGraphics();
        }

    }
}
