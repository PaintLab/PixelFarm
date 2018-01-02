//Apache2, 2014-2018, WinterDev
using System;
using PixelFarm.Drawing;
using PaintLab;
namespace LayoutFarm
{

    public abstract class DemoBase2
    {
        public void StartDemo(IViewport viewport)
        {
            OnStartDemo(viewport);
        }
        protected virtual void OnStartDemo(IViewport viewport)
        {
        }
        public virtual string Desciption
        {
            get { return ""; }
        }
    }
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
    }

    sealed class DemoBitmap : Image
    {
        int width;
        int height;
        System.Drawing.Bitmap innerImage;
        public DemoBitmap(int w, int h, System.Drawing.Bitmap innerImage)
        {
            this.width = w;
            this.height = h;
            this.innerImage = innerImage;
            SetCacheInnerImage(this, innerImage);
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
        }
        public override void RequestInternalBuffer(ref ImgBufferRequestArgs buffRequest)
        {
            var bmpData = innerImage.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
             System.Drawing.Imaging.ImageLockMode.ReadOnly,
             System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            int size = bmpData.Width * bmpData.Height;
            int[] newBuff = new int[size];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newBuff, 0, size);
            innerImage.UnlockBits(bmpData);

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
    class DemoInfo
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