//MIT,2016, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{


    class TextureFont : ActualFont
    {
        SimpleFontAtlas fontAtlas;
        string name;
        IDisposable glBmp;

        NativeFont nativeFont;
        static NativeFontStore s_nativeFontStore = new NativeFontStore();

        internal TextureFont(string fontName, float fontSizeInPts, string fontfile, SimpleFontAtlas fontAtlas)
        {
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            var font = new RequestFont(fontName, fontSizeInPts);
            nativeFont = (NativeFont)s_nativeFontStore.LoadFont(fontName, fontSizeInPts);
        }

        internal TextureFont(string fontName, float fontSizeInPts, SimpleFontAtlas fontAtlas)
        {
            //not support font 
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            //var font = new RequestFont(fontName, fontSizeInPts);
            nativeFont = (NativeFont)s_nativeFontStore.LoadFont(fontName, fontSizeInPts);
            ////var fontKey = new FontKey(fontName, fontSizeInPts, FontStyle.Regular);
            //nativeFont = (NativeFont)s_nativeFontStore.GetResolvedNativeFont(fontName, fontSizeInPts);
        }
        public override string FontName
        {
            get { return name; }
        }
        public override FontStyle FontStyle
        {
            get { return Drawing.FontStyle.Regular; }
        }
        public override float AscentInPixels
        {
            get
            {
                return nativeFont.AscentInPixels;
            }
        }
        public override float DescentInPixels
        {
            get
            {
                return nativeFont.DescentInPixels;
            }
        }
        public IDisposable GLBmp
        {
            get { return glBmp; }
            set { glBmp = value; }
        }
        public SimpleFontAtlas FontAtlas
        {
            get { return fontAtlas; }
        }
        public override float SizeInPoints
        {
            get
            {
                return nativeFont.SizeInPoints;
            }
        }

        public override float SizeInPixels
        {
            get
            {
                return nativeFont.SizeInPixels;
            }
        }

        public override FontFace FontFace
        {
            get
            {
                return nativeFont.FontFace;
            }
        }
        public override float GetAdvanceForCharacter(char c)
        {
            return nativeFont.GetAdvanceForCharacter(c);
        }

        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            return nativeFont.GetAdvanceForCharacter(c, next_c);
        }

        public override FontGlyph GetGlyph(char c)
        {
            return nativeFont.GetGlyph(c);
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            return nativeFont.GetGlyphByIndex(glyphIndex);
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            nativeFont.GetGlyphPos(buffer, start, len, properGlyphs);
        }

        protected override void OnDispose()
        {
            if (glBmp != null)
            {
                glBmp.Dispose();
                glBmp = null;
            }
        }
        //----------------------------------------------------
        /// <summary>
        /// this method always create new TextureFont, 
        /// user should do caching by themself 
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="xmlFontInfo"></param>
        /// <param name="imgAtlas"></param>
        /// <returns></returns>
        public static TextureFont CreateFont(string fontName, float fontSizeInPoints, string xmlFontInfo, GlyphImage glyphImg)
        {
            //for msdf font
            //1 font atlas may support mutliple font size 
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontInfo);
            fontAtlas.TotalGlyph = glyphImg;


            return new TextureFont(fontName, fontSizeInPoints, fontAtlas);
        }
    }
}