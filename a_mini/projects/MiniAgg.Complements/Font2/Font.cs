//----------------------------------- 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using PixelFarm.Agg;
namespace PixelFarm.Font2
{
    public class Font : IDisposable
    {
        FontFace ownerFace;
        float fontSizeInPoint;
        int fontSizeInPixelUnit;
        /// <summary>
        /// glyph
        /// </summary>
        Dictionary<char, FontGlyph> dicGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> dicGlyphs2 = new Dictionary<uint, FontGlyph>();


        internal Font(FontFace ownerFace, int pixelSize)
        {
            //store unmanage font file information
            this.ownerFace = ownerFace; 
            this.fontSizeInPixelUnit = pixelSize;
        }

        public void Dispose()
        {
            //TODO: clear resource here 
        }
        public float SizeInPoint
        {
            get { return this.fontSizeInPoint; }
        }
        public FontGlyph GetGlyph(char c)
        {
            FontGlyph found;
            if (!dicGlyphs.TryGetValue(c, out found))
            {

                found = ownerFace.ReloadGlyphFromChar(c, fontSizeInPixelUnit);
                this.dicGlyphs.Add(c, found);
            }
            return found;
        }
        public FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            FontGlyph found;
            if (!dicGlyphs2.TryGetValue(glyphIndex, out found))
            {
                found = ownerFace.ReloadGlyphFromIndex(glyphIndex, fontSizeInPixelUnit);
                this.dicGlyphs2.Add(glyphIndex, found);
            }
            return found;
        }

        /// <summary>
        /// owner font face
        /// </summary>
        public FontFace FontFace
        {
            get { return this.ownerFace; }
        }
    }
}