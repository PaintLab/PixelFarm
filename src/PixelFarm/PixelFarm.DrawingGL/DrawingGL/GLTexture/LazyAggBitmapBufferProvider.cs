//MIT, 2014-present, WinterDev

using System;
namespace PixelFarm.DrawingGL
{
    public class LazyActualBitmapBufferProvider : LazyBitmapBufferProvider
    {
        PixelFarm.CpuBlit.MemBitmap _memBmp;
        public LazyActualBitmapBufferProvider(PixelFarm.CpuBlit.MemBitmap memBmp)
        {
            this._memBmp = memBmp;
        }
        public override bool IsYFlipped
        {
            get { return false; }
        }
        public override IntPtr GetRawBufferHead()
        {
            return PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(_memBmp).Ptr;
        }
        public override void ReleaseBufferHead()
        {

        }
        public override int Width
        {
            get { return this._memBmp.Width; }
        }
        public override int Height
        {
            get { return this._memBmp.Height; }
        }
         

    }
}