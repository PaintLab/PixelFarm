//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using Win32;
namespace PixelFarm.Drawing.WinGdi
{
    /// <summary>
    /// for this platform font management
    /// </summary>
    class GdiPlusPlatformFontMx
    {
        //gdiplus platform can handle following font
        //1. gdiplus font
        //2. gdi font
        //3. vector font
        //4. opentype font

        GdiPlusIFonts gdiPlusIFonts = new GdiPlusIFonts();
        public GdiPlusPlatformFontMx()
        {
        }
    
        public WinGdiPlusFont ResolveForWinGdiPlusFont(RequestFont r)
        {
            WinGdiPlusFont winGdiPlusFont = r.ActualFont as WinGdiPlusFont;
            if (winGdiPlusFont != null)
            {
                return winGdiPlusFont;
            }
            //check if 
            throw new NotSupportedException();
        }

        //---------
        static GdiPlusPlatformFontMx s_gdiPlusFontMx = new GdiPlusPlatformFontMx();
        public static GdiPlusPlatformFontMx Default { get { return s_gdiPlusFontMx; } }
    }

    class GdiPlusIFonts : IFonts
    {
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(2, 2);
        NativeWin32MemoryDc win32MemDc;
        WinGdiPlusFontStore fontStore = new WinGdiPlusFontStore();

        //=====================================
        //static 
        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];

        public GdiPlusIFonts()
        {
            win32MemDc = new NativeWin32MemoryDc(2, 2);
        }
        public float MeasureWhitespace(RequestFont f)
        {
            return fontStore.MeasureWhitespace(this, f);
        }
        void SetFont(RequestFont font)
        {
            WinGdiPlusFont winFont = fontStore.ResolveFont(font);
            Win32Utils.SelectObject(win32MemDc.DC, winFont.ToHfont());
        }
        public PixelFarm.Drawing.Fonts.ActualFont ResolveActualFont(RequestFont f)
        {
            return fontStore.ResolveFont(f);
        }
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font)
        {
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    _characterRanges[0] = new System.Drawing.CharacterRange(0, len);
            //    _stringFormat.SetMeasurableCharacterRanges(_characterRanges);
            //    System.Drawing.Font font2 = (System.Drawing.Font)font.InnerFont;

            //    var size = gx.MeasureCharacterRanges(
            //        new string(buff, startAt, len),
            //        font2,
            //        System.Drawing.RectangleF.Empty,
            //        _stringFormat)[0].GetBounds(gx).Size;
            //    return new PixelFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            //}
            //else
            //{
            SetFont(font);
            PixelFarm.Drawing.Size size = new Size();
            if (buff.Length > 0)
            {
                unsafe
                {
                    fixed (char* startAddr = &buff[0])
                    {
                        NativeTextWin32.UnsafeGetTextExtentPoint32(win32MemDc.DC, startAddr + startAt, len, ref size);
                    }
                }
            }

            return size;
            //}
        }
        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.<br/>
        /// Restrict the width of the string and get the number of characters able to fit in the restriction and
        /// the width those characters take.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <param name="maxWidth">the max width to render the string in</param>
        /// <param name="charFit">the number of characters that will fit under <see cref="maxWidth"/> restriction</param>
        /// <param name="charFitWidth"></param>
        /// <returns>the size of the string</returns>
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth)
        {
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            //}
            //else
            //{
            SetFont(font);
            if (buff.Length == 0)
            {
                charFit = 0;
                charFitWidth = 0;
                return Size.Empty;
            }
            var size = new PixelFarm.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    NativeTextWin32.UnsafeGetTextExtentExPoint(
                        win32MemDc.DC, startAddr + startAt, len,
                        (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                }
            }
            charFit = _charFit[0];
            charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
            return size;
            //}
        }
        //==============================================


        public void Dispose()
        {
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }

            win32MemDc.Dispose();
            win32MemDc = null;
        }
    }

}