//MIT, 2014-2017, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;

namespace PixelFarm.Drawing.Fonts
{
    class SvgFont
    {
        SvgFontFace fontface;
        int emSizeInPoints;
        int emSizeInPixels;
        float currentEmScalling;
        Dictionary<char, FontGlyph> cachedGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> cachedGlyphsByIndex = new Dictionary<uint, FontGlyph>();
        Affine scaleTx;
        string fontName;
        FontStyle fontStyle;
        PixelFarm.Agg.VertexSource.CurveFlattener curveFlattner = new PixelFarm.Agg.VertexSource.CurveFlattener();
        public SvgFont(SvgFontFace fontface, string fontName, FontStyle fontStyle, int emSizeInPoints)
        {
            this.fontface = fontface;
            this.emSizeInPoints = emSizeInPoints;
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            //------------------------------------
            emSizeInPixels = (int)RequestFont.ConvEmSizeInPointsToPixels(emSizeInPoints);
            currentEmScalling = (float)emSizeInPixels / (float)fontface.UnitsPerEm;
            scaleTx = Affine.NewMatix(AffinePlan.Scale(currentEmScalling));
        }
        public string FontName
        {
            get { throw new NotImplementedException(); }
        }
        public FontStyle FontStyle
        {
            get { throw new NotImplementedException(); }
        }
        public SvgFontFace FontFace
        {
            get { return fontface; }
        }
        public FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            FontGlyph glyph;
            //temp 
            if (!cachedGlyphsByIndex.TryGetValue(glyphIndex, out glyph))
            {
                //create font glyph for this font size
                FontGlyph originalGlyph = fontface.GetGlyphByIndex((int)glyphIndex);

                VertexStore characterGlyph = new VertexStore();
                scaleTx.TransformToVxs(originalGlyph.originalVxs, characterGlyph);

                glyph = new FontGlyph();
                glyph.originalVxs = characterGlyph;
                //then flatten it
                glyph.flattenVxs = curveFlattner.MakeVxs(characterGlyph, new VertexStore());
                glyph.horiz_adv_x = originalGlyph.horiz_adv_x;
                cachedGlyphsByIndex.Add(glyphIndex, glyph);
            }
            return glyph;
        }
        public FontGlyph GetGlyph(char c)
        {
            FontGlyph glyph;
            if (!cachedGlyphs.TryGetValue(c, out glyph))
            {
                //create font glyph for this font size
                var originalGlyph = fontface.GetGlyphForCharacter(c);
                VertexStore characterGlyph = new VertexStore();
                scaleTx.TransformToVxs(originalGlyph.originalVxs, characterGlyph);
                glyph = new FontGlyph();
                glyph.horiz_adv_x = originalGlyph.horiz_adv_x;
                glyph.originalVxs = characterGlyph;
                //then flatten it

                glyph.flattenVxs = curveFlattner.MakeVxs(characterGlyph, new VertexStore());

                cachedGlyphs.Add(c, glyph);
            }
            return glyph;
        }
        //public void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        //{
        //    //find proper position for each glyph
        //    int j = buffer.Length;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        FontGlyph f = this.GetGlyph(buffer[i]);
        //        properGlyphs[i].x_advance = f.horiz_adv_x >> 6; //64
        //        properGlyphs[i].codepoint = (uint)buffer[i];
        //    }
        //}
        protected void OnDispose()
        {
        }

        public float SizeInPoints
        {
            get
            {
                return emSizeInPoints;
            }
        }
        public float SizeInPixels
        {
            get { return emSizeInPixels; }
        }
        public float GetAdvanceForCharacter(char c)
        {
            return this.GetGlyph(c).horiz_adv_x >> 6;//64
        }
        public float GetAdvanceForCharacter(char c, char next_c)
        {
            //TODO: review here 
            //this should check kerning info 
            return this.GetGlyph(c).horiz_adv_x >> 6;//64
        }
        public float AscentInPixels
        {
            get
            {
                return fontface.Ascent * currentEmScalling;
            }
        }
        public float DescentInPixels
        {
            get
            {
                return fontface.Descent * currentEmScalling;
            }
        }
    }
}
