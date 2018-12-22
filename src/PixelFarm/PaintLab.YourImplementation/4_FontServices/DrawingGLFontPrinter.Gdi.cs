//MIT, 2016-present, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.DrawingGL
{
#if GL_ENABLE
    /// <summary>
    /// this use win gdi only
    /// </summary>
    public class WinGdiFontPrinter : ITextPrinter, IDisposable
    {

        int _width;
        int _height;
        Win32.NativeWin32MemoryDC _memdc;
        IntPtr _hfont;
        int _bmpWidth = 200;
        int _bmpHeight = 50;
        GLPainterContext _pcx;
        Win32.Win32Font _defautInitFont;

        public WinGdiFontPrinter(GLPainterContext pcx, int w, int h)
        {
            _pcx = pcx;
            _width = w;
            _height = h;
            _bmpWidth = w;
            _bmpHeight = h;

            _memdc = new Win32.NativeWin32MemoryDC(_bmpWidth, _bmpHeight);
            //TODO: review here
            //use default font from current platform
            InitFont("tahoma", 14);
            _memdc.SetTextColor(0);
        }
        public bool StartDrawOnLeftTop { get; set; }
        public void ChangeFont(RequestFont font)
        {

        }
        public void ChangeFillColor(Color fillColor)
        {

        }
        public void ChangeStrokeColor(Color strokeColor)
        {

        }
        public void Dispose()
        {
            //TODO: review here             
            _defautInitFont.Dispose();
            _defautInitFont = null;

            _hfont = IntPtr.Zero;
            _memdc.Dispose();
        }


        void InitFont(string fontName, int emHeight)
        {
            Win32.Win32Font font = Win32.FontHelper.CreateWin32Font(fontName, emHeight, false, false);
            _memdc.SetFont(font.GetHFont());
            _defautInitFont = font;
        }


        public void DrawString(char[] textBuffer, int startAt, int len, double x, double y)
        {
            //TODO: review performance              
            _memdc.PatBlt(Win32.NativeWin32MemoryDC.PatBltColor.White, 0, 0, _bmpWidth, _bmpHeight);
            _memdc.TextOut(textBuffer);
            //memdc.BitBltTo(destHdc);
            // Win32.Win32Utils.BitBlt(hdc, 0, 0, bmpWidth, 50, memHdc, 0, 0, Win32.MyWin32.SRCCOPY);
            //---------------
            int stride = 4 * ((_bmpWidth * 32 + 31) / 32);

            //Bitmap newBmp = new Bitmap(bmpWidth, 50, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var bmpData = newBmp.LockBits(new Rectangle(0, 0, bmpWidth, 50), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] tmp1 = new byte[stride * 50];
            System.Runtime.InteropServices.Marshal.Copy(_memdc.PPVBits, tmp1, 0, tmp1.Length);
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


            _memdc.MeasureTextSize(textBuffer, out _bmpWidth, out _bmpHeight);
            var memBmp = new CpuBlit.MemBitmap(_bmpWidth, _bmpHeight);
#if DEBUG
            memBmp._dbugNote = "WinGdiFontPrinter.DrawString";
#endif
            //------------------------------------------------------
            //copy bmp from specific bmp area 
            //and convert to GLBmp   
            unsafe
            {
                using (CpuBlit.Imaging.TempMemPtr.FromBmp(memBmp, out byte* dest0))
                {
                    byte* header = (byte*)_memdc.PPVBits;
                    {
                        byte* dest = dest0;
                        byte* rowHead = header;
                        int rowLen = _bmpWidth * 4;
                        for (int h = 0; h < _bmpHeight; ++h)
                        {

                            header = rowHead;
                            for (int n = 0; n < rowLen;)
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
            }

            //------------------------------------------------------
            GLBitmap glBmp = new GLBitmap(new MemBitmapBinder(memBmp, false));
            _pcx.DrawImage(glBmp, (float)x, (float)y);
            glBmp.Dispose();
        }

        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            throw new NotImplementedException();
        }

        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] buffer, int startAt, int len)
        {
            throw new NotImplementedException();
        }
    }
#endif
}