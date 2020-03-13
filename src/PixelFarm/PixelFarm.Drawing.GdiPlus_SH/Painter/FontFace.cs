//MIT, 2014-present, WinterDev

using System;
using PixelFarm.CpuBlit;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace PixelFarm.Drawing.Fonts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GlyphMatrix
    {
        public short unit_per_em;
        public short ascender;
        public short descender;
        public short height;
        public int advanceX;
        public int advanceY;
        public int bboxXmin;
        public int bboxXmax;
        public int bboxYmin;
        public int bboxYmax;
        public int img_width;
        public int img_height;
        public int img_horiBearingX;
        public int img_horiBearingY;
        public int img_horiAdvance;
        public int img_vertBearingX;
        public int img_vertBearingY;
        public int img_vertAdvance;
        public int bitmap_left;
        public int bitmap_top;
    }
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

        public GlyphMatrix glyphMatrix;
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
        public abstract FontStyle FontStyle { get; }
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
            s_actualFonts.TryGetValue(r.FontKey, out ActualFont font);
            return font;
        }
        public static void SetCacheActualFont(RequestFont r, ActualFont a)
        {
            s_actualFonts[r.FontKey] = a;
            //throw new NotSupportedException();
            //RequestFont.SetCacheActualFont(r, a);
        }
    }
}