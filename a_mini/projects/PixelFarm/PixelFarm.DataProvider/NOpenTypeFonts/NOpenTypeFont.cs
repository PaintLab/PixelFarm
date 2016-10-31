//MIT, 2014-2016, WinterDev 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NOpenType;
using System.IO;
namespace PixelFarm.Drawing.Fonts
{
    class NOpenTypeFontFace : FontFace
    {
        Typeface ntypeface;
        string name, path;
        public NOpenTypeFontFace(Typeface ntypeface, string fontName, string fontPath)
        {
            this.ntypeface = ntypeface;
            this.name = fontName;
            this.path = fontPath;
        }
        public override string Name
        {
            get { return name; }
        }
        public override string FontPath
        {
            get { return path; }
        }
        protected override void OnDispose() { }
        public override ActualFont GetFontAtPointsSize(float pointSize)
        {
            NOpenTypeActualFont actualFont = new NOpenTypeActualFont(this, pointSize, FontStyle.Regular);
            return actualFont;
        }
        public Typeface Typeface { get { return this.ntypeface; } }
    }
    class NOpenTypeActualFont : ActualFont
    {
        NOpenTypeFontFace ownerFace;
        float sizeInPoints;
        FontStyle style;
        Typeface typeFace;
        float scale;
        public NOpenTypeActualFont(NOpenTypeFontFace ownerFace, float sizeInPoints, FontStyle style)
        {
            this.ownerFace = ownerFace;
            this.sizeInPoints = sizeInPoints;
            this.style = style;
            this.typeFace = ownerFace.Typeface;
            //calculate scale *** 
            scale = typeFace.CalculateScale(sizeInPoints);
        }
        public override float SizeInPoints
        {
            get { return this.sizeInPoints; }
        }
        public override float SizeInPixels
        {
            //font height 
            get { return sizeInPoints * scale; }
        }
        public override float AscentInPixels
        {
            get { return typeFace.Ascender * scale; }
        }
        public override float DescentInPixels
        {
            get { return typeFace.Descender * scale; }
        }
        public override FontFace FontFace
        {
            get { return ownerFace; }
        }
        public override string FontName
        {
            get { return typeFace.Name; }
        }
        public override FontStyle FontStyle
        {
            get { return style; }
        }

        public override float GetAdvanceForCharacter(char c)
        {
            return typeFace.GetAdvanceWidth(c);
        }
        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            //TODO: review kerning here 
            //and do scaleing here
            return typeFace.GetAdvanceWidth(c);
        }
        public override FontGlyph GetGlyph(char c)
        {
            return GetGlyphByIndex((uint)typeFace.LookupIndex(c));
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            Glyph glyph = typeFace.GetGlyphByIndex((int)glyphIndex);
            //-------------------------------------------------

            FontGlyph fontGlyph = new FontGlyph();
            fontGlyph.horiz_adv_x = typeFace.GetAdvanceWidthFromGlyphIndex((int)glyphIndex);
            return fontGlyph;
        }
        protected override void OnDispose()
        {
        }
    }


}