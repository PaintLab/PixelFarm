//MIT, 2014-present, WinterDev

using System;
using PixelFarm.CpuBlit;
using System.Collections.Generic;
namespace PixelFarm.Drawing
{

    public abstract class FontFace : IDisposable
    {
        public bool HasKerning { get; set; }
        protected abstract void OnDispose();
        public void Dispose()
        {
            OnDispose();
        }
        public abstract ActualFont GetFontAtPointSize(float pointSize);
        public abstract string Name { get; }
        public abstract string FontPath { get; }
        public abstract float GetScale(float pointSize);
        public abstract int AscentInDzUnit { get; }
        public abstract int DescentInDzUnit { get; }
        public abstract int LineGapInDzUnit { get; }
        public abstract object GetInternalTypeface();
        public abstract int RecommendedLineHeight { get; }
    }
    /// <summary>
    /// provide information about a glyph
    /// </summary>
    public class FontGlyph
    {
        //metrics
        public int horiz_adv_x;
        public string glyphName;
        public int unicode;
        /// <summary>
        /// code point/glyph index?
        /// </summary>
        public int codePoint;


        /// <summary>
        /// 32 bpp image for render
        /// </summary>
        public MemBitmap glyphImage32;
        //----------------------------
        /// <summary>
        /// original glyph outline
        /// </summary>
        public VertexStore originalVxs;
        /// <summary>
        /// flaten version of original glyph outline
        /// </summary>
        public VertexStore flattenVxs;
        //----------------------------
    }

    [Flags]
    public enum OldFontStyle : byte
    {
        Regular = 0,
        Bold = 1,
        Italic = 1 << 1,
        Underline = 1 << 2,
        Strikeout = 1 << 3,
        Others = 1 << 4
    }
    /// <summary>
    /// specific fontface + size + style
    /// </summary>
    public abstract class ActualFont : IDisposable
    {

        public abstract float SizeInPoints { get; }
        public abstract float SizeInPixels { get; }
        public void Dispose()
        {
            OnDispose();
        }
#if DEBUG
        static int dbugTotalId = 0;
        public readonly int dbugId = dbugTotalId++;
        public ActualFont()
        {

        }
#endif
        protected abstract void OnDispose();
        //---------------------
        public abstract FontGlyph GetGlyphByIndex(ushort glyphIndex);
        public abstract FontGlyph GetGlyph(char c);
        public abstract FontFace FontFace { get; }
        public abstract OldFontStyle FontStyle { get; }
        public abstract string FontName { get; }


        public abstract float AscentInPixels { get; }
        public abstract float DescentInPixels { get; }
        public abstract float LineGapInPixels { get; }
        public abstract float RecommendedLineSpacingInPixels { get; }
        ~ActualFont()
        {
            Dispose();
        }
        //---------------------

        protected static ActualFont GetCacheActualFont(RequestFont r)
        {
            //throw new NotSupportedException();
            //return RequestFont.GetCacheActualFont(r);
            return CacheFont.GetCacheActualFont(r);
        }
        protected static void SetCacheActualFont(RequestFont r, ActualFont a)
        {
            CacheFont.SetCacheActualFont(r, a);
            //throw new NotSupportedException();
            //RequestFont.SetCacheActualFont(r, a);
        }
    }

    static class CacheFont
    {
        static Dictionary<int, ActualFont> s_actualFonts = new Dictionary<int, ActualFont>();
        public static ActualFont GetCacheActualFont(RequestFont r)
        {
            s_actualFonts.TryGetValue(r.GetReqKey(), out ActualFont font);
            return font;
        }
        public static void SetCacheActualFont(RequestFont r, ActualFont a)
        {
            s_actualFonts[r.GetReqKey()] = a;
        }
    }
}