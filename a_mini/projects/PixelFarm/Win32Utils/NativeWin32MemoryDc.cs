//BSD, 2014-2016, WinterDev  

using System;
using System.Runtime.InteropServices;

namespace Win32
{

    class NativeWin32MemoryDc : IDisposable
    {
        int _width;
        int _height;
        IntPtr memHdc;
        IntPtr dib;
        IntPtr ppvBits;
        bool isDisposed;
        public NativeWin32MemoryDc(int w, int h, bool invertImage = false)
        {
            this._width = w;
            this._height = h;

            memHdc = MyWin32.CreateMemoryHdc(
                IntPtr.Zero,
                w,
                invertImage ? -h : h, //***
                out dib,
                out ppvBits);
        }
        public IntPtr DC
        {
            get { return this.memHdc; }
        }
        public IntPtr PPVBits
        {
            get { return this.ppvBits; }
        }
        public void SetTextColor(int win32Color)
        {
            Win32.MyWin32.SetTextColor(memHdc, win32Color);
        }
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            MyWin32.ReleaseMemoryHdc(memHdc, dib);
            dib = IntPtr.Zero;
            memHdc = IntPtr.Zero;
            isDisposed = true;
        }
        public void PatBlt(PatBltColor color)
        {
            MyWin32.PatBlt(memHdc, 0, 0, _width, _height, (int)color);
        }
        public void SetBackTransparent(bool value)
        {
            //public const int _SetBkMode_TRANSPARENT = 1;
            //public const int _SetBkMode_OPAQUE = 2;
            MyWin32.SetBkMode(memHdc, value ? 1 : 2);
        }
        public enum PatBltColor
        {
            Black = MyWin32.BLACKNESS,
            White = MyWin32.WHITENESS
        }
        public IntPtr SetFont(IntPtr hFont)
        {
            return MyWin32.SelectObject(memHdc, hFont);
        }
        /// <summary>
        /// set solid text color
        /// </summary>
        /// <param name="r">0-255</param>
        /// <param name="g">0-255</param>
        /// <param name="b">0-255</param>
        public void SetSolidTextColor(byte r, byte g, byte b)
        {   
            //convert to win32 colorv
            MyWin32.SetTextColor(memHdc, (b & 0xFF) << 16 | (g & 0xFF) << 8 | r);
        }

    }

}