//MIT, 2014-present, WinterDev

using System;
namespace PixelFarm.DrawingGL
{
    public class LazyActualBitmapBufferProvider : LazyBitmapBufferProvider
    {
        PixelFarm.CpuBlit.ActualBitmap image;
        public LazyActualBitmapBufferProvider(PixelFarm.CpuBlit.ActualBitmap image)
        {
            this.image = image;
        }
        public override bool IsInvert
        {
            get { return false; }
        }
        public override IntPtr GetRawBufferHead()
        {
            return PixelFarm.CpuBlit.ActualBitmap.GetBufferPtr(image).Ptr;
        }
        public override void ReleaseBufferHead()
        {

        }
        public override int Width
        {
            get { return this.image.Width; }
        }
        public override int Height
        {
            get { return this.image.Height; }
        }

        //
        public bool MayNeedUpdate { get; set; }

    }
}