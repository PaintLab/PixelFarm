//MIT 2014, WinterDev
using System.Text;
using System;
using System.Runtime.InteropServices;
using LayoutFarm.Drawing;

namespace LayoutFarm.DrawingGL
{
    public class LazyAggBitmapBufferProvider : LazyBitmapBufferProvider
    {

        Image image;
        GCHandle handle;
        public LazyAggBitmapBufferProvider(Image image)
        {
            this.image = image;
        }
        public override bool IsInvert
        {
            get { return false; }
        }
        public override IntPtr GetRawBufferHead()
        {
            if (image is PixelFarm.Agg.ActualImage)
            {
                var img = image as PixelFarm.Agg.ActualImage;
                byte[] buffer = img.GetBuffer();
                this.handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                return this.handle.AddrOfPinnedObject();
            }
            else
            {
                return IntPtr.Zero;
            }

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