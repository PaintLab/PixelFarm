//MIT, 2014-2016, WinterDev

using System;
using PixelFarm.Agg.Fonts;

namespace PixelFarm.Drawing
{
    public abstract class Font : IDisposable
    {
        public abstract FontInfo FontInfo { get; }
        public abstract string Name { get; }
        public abstract int Height { get; }
        public abstract float EmSize { get; }
        public abstract FontStyle Style { get; }

        //TODO:platform specific font object,
        //TODO: review here
        public abstract object InnerFont { get; }
        public void Dispose()
        {
            OnDispose();
        }
#if DEBUG
        static int dbugTotalId = 0;
        public readonly int dbugId = dbugTotalId++;
        public Font()
        {
            //if (this.dbugId == 2)
            //{ 
            //}

        }
#endif
        protected abstract void OnDispose();
        public abstract FontGlyph GetGlyphByIndex(uint glyphIndex);
        public abstract FontGlyph GetGlyph(char c);
        public abstract FontFace FontFace { get; }
        public abstract void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs);
        public abstract int EmSizeInPixels { get; }

        public abstract int GetAdvanceForCharacter(char c);
        public abstract int GetAdvanceForCharacter(char c, char next_c);
        public abstract double AscentInPixels { get; }
        public abstract double DescentInPixels { get; }
        public abstract double XHeightInPixels { get; }
        public abstract double CapHeightInPixels { get; }

        ~Font()
        {
            Dispose();
        }

        public abstract bool IsAtlasFont { get; }
        //-------------------------------------------
    }


    /// <summary>
    /// glyph ABC structure
    /// </summary>
    public struct FontABC
    {
        //see https://msdn.microsoft.com/en-us/library/windows/desktop/dd162454(v=vs.85).aspx
        //The ABC structure contains the width of a character in a TrueType font.
        public int a;
        public uint b;
        public int c;
        public FontABC(int a, uint b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public int Sum
        {
            get
            {
                return a + (int)b + c;
            }
        }
    }

    public abstract class FontInfo
    {
        public float AscentPx { get; protected set; }
        public float DescentPx { get; protected set; }
        public float BaseLine { get; protected set; }
        public int LineHeight { get; protected set; }
        //--------------------------------------------------
        public int FontHeight { get; protected set; }
        public int FontSize { get; protected set; }
        public abstract IntPtr HFont { get; }
        public abstract int GetCharWidth(char c);
        public abstract FontABC GetCharABCWidth(char c);
        public abstract int GetStringWidth(char[] buffer);
        public abstract int GetStringWidth(char[] buffer, int length);
        public abstract Font ResolvedFont { get; }
        public object PlatformSpecificFont { get; set; }
    }
    public interface IFonts
    {
        FontInfo GetFontInfo(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f);
        Size MeasureString(char[] str, int startAt, int len, Font font);
        Size MeasureString(char[] str, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth);
        void Dispose();
    }


    public abstract class StringFormat
    {
        public abstract object InnerFormat { get; }
    }


}