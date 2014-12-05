//----------------------------------- 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using PixelFarm.Agg;
namespace PixelFarm.Font2
{

    public abstract class Font : IDisposable
    {

        protected abstract void OnDispose();
        public abstract FontGlyph GetGlyphByIndex(uint glyphIndex);
        public abstract FontGlyph GetGlyph(char c);
        public abstract FontFace FontFace { get; }
        public abstract void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs);

        public void Dispose()
        {
            OnDispose();
        }
        
        ~Font()
        {
            Dispose();
        }
    }

   
}