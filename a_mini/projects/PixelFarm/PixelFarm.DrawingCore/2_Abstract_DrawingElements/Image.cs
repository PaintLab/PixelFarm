//MIT, 2014-2017, WinterDev

using System;
namespace PixelFarm.Drawing
{
    public abstract class Image : System.IDisposable
    {
        public abstract void Dispose();
        public abstract int Width { get; }
        public abstract int Height { get; }


        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
        }
        public abstract bool IsReferenceImage { get; }
        public abstract int ReferenceX { get; }
        public abstract int ReferenceY { get; }

        public abstract byte[] CopyInternalBitmapBuffer();
        //--------
        WeakReference innerImage;
        public static object GetCacheInnerImage(Image img)
        {
            if (img.innerImage != null && img.innerImage.IsAlive)
            {
                return img.innerImage.Target;
            }
            return null;
        }
        public static void ClearCache(Image img)
        {
            if (img != null)
            {
                img.innerImage = null;
            }
        }
        public static void SetCacheInnerImage(Image img, object o)
        {
            img.innerImage = new WeakReference(o);
        }
    }
}