//MIT, 2016, WinterDev

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Skia
{
    class BufferBitmapStore
    {
        Stack<MySkBmp> bmpStack = new Stack<MySkBmp>();
        public BufferBitmapStore(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public MySkBmp GetFreeBmp()
        {
            if (bmpStack.Count > 0)
            {
                return bmpStack.Pop();
            }
            else
            {
                return new MySkBmp(Width, Height);
            }
        }
        public void RelaseBmp(MySkBmp bmp)
        {
            bmpStack.Push(bmp);
        }
    }
}