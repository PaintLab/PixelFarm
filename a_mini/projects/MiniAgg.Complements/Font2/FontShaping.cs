//MIT 2014,WinterDev 
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using System.IO;

namespace PixelFarm.Font2
{
    public static class FontShaping
    {

        public static void GetGlyphPos(Font font, char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {

            unsafe
            {


                fixed (ProperGlyph* propGlyphH = &properGlyphs[0])
                fixed (char* head = &buffer[0])
                {
                    NativeMyFontsLib.MyFtShaping(
                        font.FontFace.HBFont,                         
                        head,
                        buffer.Length,
                        propGlyphH);
                }
            }
        }

    }
}