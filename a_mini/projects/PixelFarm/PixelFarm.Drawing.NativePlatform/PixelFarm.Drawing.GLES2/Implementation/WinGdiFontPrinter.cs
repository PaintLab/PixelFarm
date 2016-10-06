//MIT, 2016, WinterDev
using System;
namespace PixelFarm.DrawingGL
{
    /// <summary>
    /// this use win gdi only
    /// </summary>
    class WinGdiFontPrinter : IDisposable
    {

        int _width;
        int _height;

        IntPtr memHdc;
        IntPtr dib;
        IntPtr ppvBits;
        IntPtr hfont;
        int bmpWidth = 200;
        int bmpHeight = 50;
        public WinGdiFontPrinter(int w, int h)
        {
            _width = w;
            _height = h;
            bmpWidth = w;
            bmpHeight = h;
            memHdc = Win32.Win32Utils.CreateMemoryHdc(IntPtr.Zero, bmpWidth, bmpHeight, out dib, out ppvBits);
            InitFont("tahoma", 14);
            Win32.MyWin32.SetTextColor(memHdc, 0);
        }
        public void Dispose()
        {
            //TODO: review here 
            Win32.Win32Utils.DeleteObject(dib);
            Win32.Win32Utils.DeleteObject(hfont);
            Win32.Win32Utils.DeleteDC(memHdc);
            dib = IntPtr.Zero;
            hfont = IntPtr.Zero;
            memHdc = IntPtr.Zero;

        }
        void InitFont(string fontName, int emHeight)
        {
            Win32.MyWin32.LOGFONT logFont = new Win32.MyWin32.LOGFONT();
            Win32.MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = emHeight;
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default
            hfont = Win32.MyWin32.CreateFontIndirect(ref logFont);
            Win32.MyWin32.SelectObject(memHdc, hfont);
        }

        public void DrawString(CanvasGL2d canvas, string text, float x, float y)
        {
            char[] textBuffer = text.ToCharArray();
            Win32.MyWin32.PatBlt(memHdc, 0, 0, bmpWidth, bmpHeight, Win32.MyWin32.WHITENESS);
            Win32.NativeTextWin32.TextOut(memHdc, 0, 0, textBuffer, textBuffer.Length);
            // Win32.Win32Utils.BitBlt(hdc, 0, 0, bmpWidth, 50, memHdc, 0, 0, Win32.MyWin32.SRCCOPY);
            //---------------
            int stride = 4 * ((bmpWidth * 32 + 31) / 32);

            //Bitmap newBmp = new Bitmap(bmpWidth, 50, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var bmpData = newBmp.LockBits(new Rectangle(0, 0, bmpWidth, 50), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] tmp1 = new byte[stride * 50];
            System.Runtime.InteropServices.Marshal.Copy(ppvBits, tmp1, 0, tmp1.Length);
            //---------------
            int pos = 3;
            for (int r = 0; r < 50; ++r)
            {
                for (int c = 0; c < stride; ++c)
                {
                    tmp1[pos] = 255;
                    pos += 4;
                    c += 4;
                }
            }

            Win32.NativeTextWin32.WIN32SIZE win32Size;
            unsafe
            {
                fixed (char* bufferHead = &textBuffer[0])
                {
                    Win32.NativeTextWin32.GetTextExtentPoint32Char(memHdc, bufferHead, textBuffer.Length, out win32Size);
                }
            }
            bmpWidth = win32Size.Width;
            bmpHeight = win32Size.Height;

            var actualImg = new Agg.ActualImage(bmpWidth, bmpHeight, Agg.Image.PixelFormat.ARGB32);
            //------------------------------------------------------
            //copy bmp from specific bmp area 
            //and convert to GLBmp   
            byte[] buffer = actualImg.GetBuffer();
            unsafe
            {
                byte* header = (byte*)ppvBits;
                fixed (byte* dest0 = &buffer[0])
                {
                    byte* dest = dest0;
                    byte* rowHead = header;
                    int rowLen = bmpWidth * 4;
                    for (int h = 0; h < bmpHeight; ++h)
                    {

                        header = rowHead;
                        for (int n = 0; n < rowLen; )
                        {
                            //move next
                            *(dest + 0) = *(header + 0);
                            *(dest + 1) = *(header + 1);
                            *(dest + 2) = *(header + 2);
                            //*(dest + 3) = *(header + 3);
                            *(dest + 3) = 255;
                            header += 4;
                            dest += 4;
                            n += 4;
                        }
                        //finish one row
                        rowHead += stride;
                    }
                }
            }

            //------------------------------------------------------
            GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, false);
            canvas.DrawImage(glBmp, (float)x, (float)y);
            glBmp.Dispose();

        }
    }
}