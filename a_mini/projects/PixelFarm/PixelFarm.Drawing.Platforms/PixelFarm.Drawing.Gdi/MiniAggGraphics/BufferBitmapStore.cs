//2016 MIT, WinterDev

using System;
using System.Drawing;
namespace PixelFarm.Drawing.WinGdi
{
    class BufferBitmapStore
    {
        public BufferBitmapStore(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}