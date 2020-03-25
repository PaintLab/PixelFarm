//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using Mini;

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
                using (var platformBmp = CreatePlatformBitmap(memBitmap))
                {
                    System.Windows.Forms.Clipboard.SetImage(platformBmp);
                }
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
                using (var platformBmp = CreatePlatformBitmap(memBitmap))
                {
                    System.Windows.Forms.Clipboard.SetImage(platformBmp);
                }
            }
        }
        [DemoAction]
        public void Paste()
        {
            //paste img from clipboard
            if (System.Windows.Forms.Clipboard.ContainsImage())
            {
                //convert clipboard img to 
                System.Drawing.Bitmap bmp = System.Windows.Forms.Clipboard.GetImage() as System.Drawing.Bitmap;
                MemBitmap memBmp = new MemBitmap(bmp.Width, bmp.Height);
                PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(bmp, memBmp);

                //...
                using (Tools.BorrowAggPainter(_lionPng, out var painter))
                {
                    painter.DrawImage(memBmp);
                }

            }
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
    }
}