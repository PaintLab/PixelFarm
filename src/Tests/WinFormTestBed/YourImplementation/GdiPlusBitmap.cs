//Apache2, 2014-present, WinterDev
using System;
using PixelFarm.Drawing;

namespace LayoutFarm
{

    public sealed class GdiPlusBitmap : Image
    {
        int width;
        int height;
        System.Drawing.Bitmap _innerBmp;
        public GdiPlusBitmap(int w, int h, System.Drawing.Bitmap innerImage)
        {
            this.width = w;
            this.height = h;
            this._innerBmp = innerImage;
            SetCacheInnerImage(this, innerImage);
        }
        public override Image CreateAnother(float scaleW, float scaleH)
        {
            int bmpW = _innerBmp.Width;
            int bmpH = _innerBmp.Height;
            System.Drawing.Bitmap newclone = new System.Drawing.Bitmap(_innerBmp, (int)(bmpW * scaleW), (int)(bmpH * scaleH));
            return new GdiPlusBitmap((int)(width * scaleW), (int)(height * scaleH), newclone);
        }
        public override int Width
        {
            get { return this.width; }
        }
        public override int Height
        {
            get { return this.height; }
        }
        public override void Dispose()
        {
            ClearCache(this);
            if (_innerBmp != null)
            {
                _innerBmp.Dispose();
                _innerBmp = null;
            }
        }
        public override void RequestInternalBuffer(ref ImgBufferRequestArgs buffRequest)
        {
            var bmpData = _innerBmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
             System.Drawing.Imaging.ImageLockMode.ReadOnly,
             System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            int size = bmpData.Width * bmpData.Height;
            int[] newBuff = new int[size];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newBuff, 0, size);
            _innerBmp.UnlockBits(bmpData);

            buffRequest.OutputBuffer32 = newBuff;
        }

        public override bool IsReferenceImage
        {
            get { return false; }
        }
        public override int ReferenceX
        {
            get { return 0; }
        }
        public override int ReferenceY
        {
            get { return 0; }
        }

    }

}