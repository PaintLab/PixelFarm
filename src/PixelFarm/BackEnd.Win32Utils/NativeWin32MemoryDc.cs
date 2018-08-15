//BSD, 2014-present, WinterDev  

using System;

namespace Win32
{

    public class NativeWin32MemoryDc : IDisposable
    {
        int _width;
        int _height;

        IntPtr memHdc;
        IntPtr dib;
        IntPtr ppvBits;
        IntPtr hRgn = IntPtr.Zero;

        bool isDisposed;
        bool _invertedImage;

        public NativeWin32MemoryDc(int w, int h, bool invertImage = false)
        {
            this._width = w;
            this._height = h;

            _invertedImage = invertImage;

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

            if (hRgn != IntPtr.Zero)
            {
                MyWin32.DeleteObject(hRgn);
                hRgn = IntPtr.Zero;
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
        public void PatBlt(PatBltColor color, int x, int y, int w, int h)
        {
            MyWin32.PatBlt(memHdc, x, y, w, h, (int)color);
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

        public void SetClipRect(int x, int y, int w, int h)
        {
            if (hRgn == IntPtr.Zero)
            {
                //create
                hRgn = MyWin32.CreateRectRgn(0, 0, w, h);
            }
            MyWin32.SetRectRgn(hRgn,
            x,
            y,
            x + w,
            y + h);
            MyWin32.SelectObject(memHdc, hRgn);
        }
        public void ClearClipRect()
        {
            MyWin32.SelectClipRgn(memHdc, IntPtr.Zero);
        }
        //
        public void TextOut(char[] textBuffer)
        {
            NativeTextWin32.TextOut(this.memHdc, 0, 0, textBuffer, textBuffer.Length);
        }
        public void TextOut(char[] textBuffer, int x, int y)
        {
            NativeTextWin32.TextOut(this.memHdc, x, y, textBuffer, textBuffer.Length);
        }
        public unsafe void CopyPixelBitsToOutput(byte* outputBuffer)
        {
            Win32.MyWin32.memcpy((byte*)outputBuffer, (byte*)this.PPVBits, _width * 4 * _height);
        }
        public unsafe void CopyPixelBitsToOutput(byte* outputBuffer, int copyLen)
        {
            Win32.MyWin32.memcpy((byte*)outputBuffer, (byte*)this.PPVBits, copyLen);
        }

        public unsafe void BlendBltBitFrom(
            byte* srcH, int srcStrideInBytes,
            int srcX,
            int srcY,
            int srcWidth,
            int srcHeight,
            int destX, int destY)
        {
            //no alpha

            Rectangle rect = Rectangle.Intersect(
                        new Rectangle(destX, destY, srcWidth, srcHeight), //src rect
                        new Rectangle(0, 0, this._width, this._height));//dest rectt

            if (rect.W <= 0 || rect.H <= 0)
            {
                return;
            }
            //----------------------------------------------------------------------------------------------

            //copy line by line 
            int destStrideInBytes = _width * 4; //**4 => 32 bits pixel (4 bytes per 1 pixel)

            byte* destHead = (byte*)this.PPVBits;
            int destXOffset = destX * 4;//to bytes
            int srcXOffset = srcX * 4;
            //
            int srcRowLenInBytes = rect.W * 4;
            int src_intersect_height = rect.H;            //


            if (_invertedImage)
            {
                destHead += (destStrideInBytes) * (_height - 1); //*** 
                for (int h = srcY; h < src_intersect_height; ++h)
                {
                    //
                    Win32.MyWin32.memcpy(destHead + destXOffset, srcH + srcXOffset, srcRowLenInBytes);

                    srcH += srcStrideInBytes;
                    destHead -= destStrideInBytes; //***
                }
            }
            else
            {
                for (int h = srcY; h < src_intersect_height; ++h)
                {
                    Win32.MyWin32.memcpy(destHead + destXOffset, srcH + srcXOffset, srcRowLenInBytes);
                    srcH += srcStrideInBytes;
                    destHead += destStrideInBytes;
                }
            }
        }
        public unsafe void BltBitFrom(byte* srcH, int srcStrideInBytes,
            int srcX,
            int srcY,
            int srcWidth,
            int srcHeight,
            int destX, int destY)
        {
            //no alpha

            Rectangle rect = Rectangle.Intersect(
                        new Rectangle(destX, destY, srcWidth, srcHeight), //src rect
                        new Rectangle(0, 0, this._width, this._height));//dest rectt

            if (rect.W <= 0 || rect.H <= 0)
            {
                return;
            }
            //----------------------------------------------------------------------------------------------

            //copy line by line 
            int destStrideInBytes = _width * 4; //**4 => 32 bits pixel (4 bytes per 1 pixel)

            byte* destHead = (byte*)this.PPVBits;
            int destXOffset = destX * 4;//to bytes
            int srcXOffset = srcX * 4;
            //
            int srcRowLenInBytes = rect.W * 4;
            int src_intersect_height = rect.H;            //


            if (_invertedImage)
            {
                destHead += (destStrideInBytes) * (_height - 1); //*** 
                for (int h = srcY; h < src_intersect_height; ++h)
                {
                    Win32.MyWin32.memcpy(destHead + destXOffset, srcH + srcXOffset, srcRowLenInBytes);
                    srcH += srcStrideInBytes;
                    destHead -= destStrideInBytes; //***
                }
            }
            else
            {
                for (int h = srcY; h < src_intersect_height; ++h)
                {
                    Win32.MyWin32.memcpy(destHead + destXOffset, srcH + srcXOffset, srcRowLenInBytes);
                    srcH += srcStrideInBytes;
                    destHead += destStrideInBytes;
                }
            }
        }
        public void MeasureTextSize(char[] textBuffer, out int width, out int height)
        {
            Size win32Size;
            unsafe
            {
                fixed (char* bufferHead = &textBuffer[0])
                {
                    Win32.NativeTextWin32.GetTextExtentPoint32Char(this.memHdc,
                        bufferHead, textBuffer.Length, out win32Size);
                }
            }
            width = win32Size.W;
            height = win32Size.H;
        }
        public void BitBltTo(IntPtr destHdc)
        {
            Win32.MyWin32.BitBlt(destHdc, 0, 0, _width, _height, this.memHdc, 0, 0, MyWin32.SRCCOPY);
        }
    }

    public class Win32Font : IDisposable
    {
        IntPtr hfont;
        public Win32Font(IntPtr hfont)
        {
            this.hfont = hfont;
        }
        public IntPtr GetHFont()
        {
            return this.hfont;
        }
        public void Dispose()
        {
            Win32.MyWin32.DeleteObject(hfont);
        }
    }

    public static class FontHelper
    {
        public static Win32Font CreateWin32Font(string fontName, float emHeight, bool bold, bool italic, float pixels_per_inch = 96)
        {
            //see: MSDN, LOGFONT structure
            //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
            MyWin32.LOGFONT logFont = new MyWin32.LOGFONT();
            MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = -(int)MyWin32.ConvEmSizeInPointsToPixels(emHeight, pixels_per_inch);//minus **
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default

            //
            MyWin32.LOGFONT_FontWeight weight =
                bold ?
                MyWin32.LOGFONT_FontWeight.FW_BOLD :
                MyWin32.LOGFONT_FontWeight.FW_REGULAR;
            logFont.lfWeight = (int)weight;
            //
            logFont.lfItalic = (byte)(italic ? 1 : 0);
            return new Win32Font(MyWin32.CreateFontIndirect(ref logFont));
        }
    }
}