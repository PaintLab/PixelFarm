//MIT,2016, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{
    /// <summary>
    /// cache texture font
    /// </summary>
    public class TextureFontStore
    {
        Dictionary<Font, TextureFont> registerFonts = new Dictionary<Font, TextureFont>();
        public void RegisterFont(Font f, TextureFont textureFont)
        {
            registerFonts.Add(f, textureFont);
        }
        public TextureFont GetResolvedFont(Font f)
        {
            TextureFont found;
            registerFonts.TryGetValue(f, out found);
            return found;
        }
    }

    public class TextureFont : ActualFont
    {
        SimpleFontAtlas fontAtlas;
        string name;
        IDisposable glBmp;
        Font nativeFont;
        static NativeFontStore s_nativeFontStore = new NativeFontStore();

        internal TextureFont(string fontName, float fontSizeInPts, string fontfile, SimpleFontAtlas fontAtlas)
        {
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            nativeFont = new Font(fontName, fontSizeInPts);
            s_nativeFontStore.LoadFont(nativeFont, fontfile);
        }
        internal TextureFont(string fontName, float fontSizeInPts, SimpleFontAtlas fontAtlas)
        {
            //not support font 
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            nativeFont = new Font(fontName, fontSizeInPts);
            s_nativeFontStore.LoadFont(nativeFont);
        }
        public override float AscentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override float DescentInPixels
        {
            get
            {
                throw new NotImplementedException();
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
        public override float EmSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override float EmSizeInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontFace FontFace
        {
            get
            {
                throw new NotImplementedException();
            }
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
            //return nativeFont.ActualFont.GetGlyphByIndex(glyphIndex);
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
            //nativeFont.ActualFont.GetGlyphPos(buffer, start, len, properGlyphs);
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
        public static TextureFont CreateFont(string fontName, float fontSizeInPoints, string xmlFontInfo, string imgAtlas)
        {
            //for msdf font
            //1 font atlas may support mutliple font size 
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontInfo);
            //2. load glyph image
            using (Bitmap bmp = new Bitmap(imgAtlas))
            {
                var glyImage = new GlyphImage(bmp.Width, bmp.Height);
                var buffer = new int[bmp.Width * bmp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmp.GetNativeHImage(), buffer, 0, buffer.Length);
                glyImage.SetImageBuffer(buffer, true);
                fontAtlas.TotalGlyph = glyImage;
            }
            return new TextureFont(fontName, fontSizeInPoints, fontAtlas);
        }
    }
}