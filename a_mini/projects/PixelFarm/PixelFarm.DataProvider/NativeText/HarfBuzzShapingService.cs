//MIT, 2014-2016, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Drawing.Text
{

    public sealed class HarfBuzzShapingService : TextShapingService
    {
        protected override void GetGlyphPosImpl(ActualFont actualFont, char[] buffer, int startAt, int len, Fonts.ProperGlyph[] properGlyphs)
        {
            NativeFont nativeFont = actualFont as NativeFont;
            if (nativeFont == null)
            {
                //not native font
            }
            else
            {
                unsafe
                {
                    fixed (ProperGlyph* propGlyphH = &properGlyphs[0])
                    fixed (char* head = &buffer[0])
                    {
                        //we use font shaping engine here
                        NativeMyFontsLib.MyFtShaping(
                           nativeFont.NativeFontFace.HBFont,
                            head,
                            buffer.Length,
                            propGlyphH);
                    }
                }
            }
        }
    }
}