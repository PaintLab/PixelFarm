//Apache2, 2014-present, WinterDev
using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{

    public abstract class DemoBase
    {
        public void StartDemo(SampleViewport viewport)
        {
            OnStartDemo(viewport);
        }
        protected virtual void OnStartDemo(SampleViewport viewport)
        {
        }
        public virtual string Desciption
        {
            get { return ""; }
        }


        public static Image LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            GdiPlusBitmap bmp = new GdiPlusBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }

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


    public class DemoNoteAttribute : Attribute
    {
        public DemoNoteAttribute(string msg)
        {
            this.Message = msg;
        }
        public string Message { get; set; }
    }
    public class DemoInfo
    {
        public readonly Type DemoType;
        public readonly string DemoNote;
        public int demoBaseTypeKind; // 0,1

        public DemoInfo(Type demoType, string demoNote)
        {
            this.DemoType = demoType;
            this.DemoNote = demoNote;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(DemoNote))
            {
                return this.DemoType.Name;
            }
            else
            {
                return this.DemoNote + " : " + this.DemoType.Name;
            }
        }
    }

}