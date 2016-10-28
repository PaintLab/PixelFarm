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
        public NOpenTypeFontFace(Typeface ntypeface)
        {
            this.ntypeface = ntypeface;
        }
        protected override void OnDispose() { }
        public override ActualFont GetFontAtPointsSize(float pointSize)
        {
            NOpenTypeActualFont actualFont = new NOpenTypeActualFont(ntypeface, pointSize);

            return actualFont;
        }
    }
    class NOpenTypeActualFont : ActualFont
    {
        Typeface typeFace;
        float sizeInPoints;
        public NOpenTypeActualFont(Typeface typeFace, float sizeInPoints)
        {
            this.typeFace = typeFace;
            this.sizeInPoints = sizeInPoints;
        }
        public override float SizeInPoints
        {
            get { return this.sizeInPoints; }
        }
        public override float AscentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override float DescentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override FontFace FontFace
        {
            get { throw new NotImplementedException(); }
        }
        public override string FontName
        {
            get { throw new NotImplementedException(); }
        }
        public override FontStyle FontStyle
        {
            get { throw new NotImplementedException(); }
        }

        public override float SizeInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }
        public override float GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }
        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }
        public override FontGlyph GetGlyph(char c)
        {
            throw new NotImplementedException();
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }
        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
         
    }


}