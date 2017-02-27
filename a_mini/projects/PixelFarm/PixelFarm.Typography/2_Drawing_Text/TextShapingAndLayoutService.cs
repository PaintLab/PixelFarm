﻿//MIT, 2016-2017, WinterDev
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;
using Typography.TextLayout;
namespace PixelFarm.Drawing.Text
{
    public abstract class TextShapingService
    {

        protected abstract void GetGlyphPosImpl(ActualFont actualFont, char[] buffer, int startAt, int len, List<GlyphPlan> properGlyphs);
        public static void GetGlyphPos(ActualFont actualFont, char[] buffer, int startAt, int len, List<GlyphPlan> properGlyphs)
        {
            defaultSharpingService.GetGlyphPosImpl(actualFont, buffer, startAt, len, properGlyphs);
        }
        static TextShapingService defaultSharpingService;
        public void SetAsCurrentImplementation()
        {
            defaultSharpingService = this;
        }
    }

    public abstract class TextLayoutService
    {
        static TextLayoutService defaultTextLayoutServices;
        public void SetAsCurrentImplementation()
        {
            defaultTextLayoutServices = this;
        }

        public abstract Size MeasureStringImpl(char[] buff, int startAt, int len, RequestFont font);

        public abstract Size MeasureStringImpl(char[] buff, int startAt, int len,
            RequestFont font, float maxWidth,
            out int charFit, out int charFitWidth);
    }
}