//MIT, 2014-present, WinterDev

using System;
namespace PixelFarm.Drawing
{
   
   

    public class LazyMemBitmapBufferProvider : LazyBitmapBufferProvider
    {
        PixelFarm.CpuBlit.MemBitmap _memBmp;
        public LazyMemBitmapBufferProvider(PixelFarm.CpuBlit.MemBitmap memBmp)
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
        public override int ImageWidth
        {
            get { return this._memBmp.Width; }
        }
        public override int ImageHeight
        {
            get { return this._memBmp.Height; }
        }
    }
}