//MIT, 2016-present, WinterDev

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.WinGdi
{
    class BufferBitmapStore
    {
        Stack<System.Drawing.Bitmap> _bmpStack = new Stack<System.Drawing.Bitmap>();
        public BufferBitmapStore(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public System.Drawing.Bitmap GetFreeBmp()
        {
            if (_bmpStack.Count > 0)
            {
                return _bmpStack.Pop();
            }
            else
            {
                return new System.Drawing.Bitmap(Width, Height);
            }
        }
        public void RelaseBmp(System.Drawing.Bitmap bmp)
        {
            _bmpStack.Push(bmp);
        }
    }
}