//Apache2, 2014-present, WinterDev
using System;
using PixelFarm.Drawing;

 

namespace LayoutFarm
{

    public sealed class GdiPlusBitmap : Image
    {
        int _width;
        int _height;
        System.Drawing.Bitmap _innerBmp;

        public GdiPlusBitmap(int w, int h, System.Drawing.Bitmap innerImage)
        {
            this._width = w;
            this._height = h;
            this._innerBmp = innerImage;
            SetCacheInnerImage(this, innerImage);
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
            //COPY***
            var bmpData = _innerBmp.LockBits(new System.Drawing.Rectangle(0, 0, _width, _height),
             System.Drawing.Imaging.ImageLockMode.ReadOnly,
             System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            int size = bmpData.Width * bmpData.Height;
            int[] newBuff = new int[size];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newBuff, 0, size);
            _innerBmp.UnlockBits(bmpData);

            buffRequest.OutputBuffer32 = newBuff;
        }

        public override int Width => _width;

        public override int Height => _height;

        public override bool IsReferenceImage => false;

        public override int ReferenceX => 0;

        public override int ReferenceY => 0;
    }
     
}