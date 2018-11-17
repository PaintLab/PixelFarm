//MIT, 2014-present, WinterDev

using System;
namespace PixelFarm.DrawingGL
{
    public class LazyActualBitmapBufferProvider : LazyBitmapBufferProvider
    {
        PixelFarm.CpuBlit.MemBitmap image;
        public LazyActualBitmapBufferProvider(PixelFarm.CpuBlit.MemBitmap image)
        {
            this.image = image;
        }
        public override bool IsYFlipped
        {
            get { return false; }
        }
        public override IntPtr GetRawBufferHead()
        {
            return PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(image).Ptr;
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