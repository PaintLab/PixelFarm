//MIT, 2016, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Drawing.Text
{
    public abstract class TextShapingService
    {

        protected abstract void GetGlyphPosImpl(ActualFont actualFont, char[] buffer, int startAt, int len, ProperGlyph[] properGlyphs);

        public static void GetGlyphPos(ActualFont actualFont, char[] buffer, int startAt, int len, ProperGlyph[] properGlyphs)
        {
            defaultSharpingService.GetGlyphPosImpl(actualFont,buffer, startAt, len, properGlyphs);
        }
        static TextShapingService defaultSharpingService;
        public void SetAsCurrentImplementation()
        {
            defaultSharpingService = this;
        }
    }


}