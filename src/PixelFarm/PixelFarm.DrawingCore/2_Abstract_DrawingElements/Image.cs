//MIT, 2014-present, WinterDev

using System;
namespace PixelFarm.Drawing
{

    public abstract class Image : IDisposable
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


        object _innerImg;
        public static object GetCacheInnerImage(Image img)
        {    
            return img._innerImg;
        }
        public static void ClearCache(Image img)
        {
            if (img != null)
            {
                img._innerImg = null;
            }
        }
        public static void SetCacheInnerImage(Image img, object o)
        {
            img._innerImg = o;
        }

    }

}