//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using Mini;
using LayoutFarm.UI;

namespace PixelFarm.CpuBlit.Sample_FloodFill
{
    //simple cut, copy , paste example (simplified version of flood fill demo)
    [Info(OrderCode = "09", AvailableOn = AvailableOn.Agg)]
    [Info(DemoCategory.Bitmap, "simple cut, copy, paste")]
    public class CutCopyPasteDemo : DemoBase
    {
        MemBitmap _lionPng;
        public CutCopyPasteDemo()
        {
            _lionPng = MemBitmap.LoadBitmap("../Data/lion1.png");
        }
        public override void Draw(Painter p)
        {
            p.DrawImage(_lionPng);
            base.Draw(p);
        }
        [DemoAction]
        public void Cut()
        {
            //1.copy
            using (MemBitmap memBitmap = _lionPng.CopyImgBuffer(20, 20, 100, 100))
            {
                Clipboard.SetImage(memBitmap);
            }
            //2. fill cut area
            using (Tools.BorrowAggPainter(_lionPng, out var painter))
            {
                var prevColor = painter.FillColor;
                painter.FillColor = Color.White;
                painter.FillRect(20.5, 20.5, 100, 100);
                painter.FillColor = prevColor;
            }
        }
        [DemoAction]
        public void Copy()
        {
            using (MemBitmap memBitmap = _lionPng.CopyImgBuffer(20, 20, 100, 100))
            {
                Clipboard.SetImage(memBitmap);
            }
        }
        [DemoAction]
        public void Paste()
        {
            //paste img from clipboard
            if (Clipboard.ContainsImage())
            {
                //convert clipboard img to 
                MemBitmap memBmp = Clipboard.GetImage() as MemBitmap;
                using (Tools.BorrowAggPainter(_lionPng, out var painter))
                {
                    painter.DrawImage(memBmp);
                }
            }
        }
    }
}