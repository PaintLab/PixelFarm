//MIT, 2014-present, WinterDev

using System;
using System.Runtime.InteropServices;
namespace PixelFarm.DrawingGL
{
    public class LazyAggBitmapBufferProvider : LazyBitmapBufferProvider
    {
        PixelFarm.CpuBlit.ActualBitmap image;
        GCHandle handle;
        public LazyAggBitmapBufferProvider(PixelFarm.CpuBlit.ActualBitmap image)
        {
            this.image = image;
        }
        public override bool IsInvert
        {
            get { return false; }
        }
        public override IntPtr GetRawBufferHead()
        {
            int[] buffer = PixelFarm.CpuBlit.ActualBitmap.GetBuffer(image);
            this.handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            return this.handle.AddrOfPinnedObject();
        }
        public override void ReleaseBufferHead()
        {
            this.handle.Free();
        }
        public override int Width
        {
            get { return this.image.Width; }
        }
        public override int Height
        {
            get { return this.image.Height; }
        }
    }
}