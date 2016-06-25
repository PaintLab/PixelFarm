//----------------------------------- 
using System;
using PixelFarm.Drawing;

namespace PixelFarm.Agg
{
    public class AggFont : Drawing.Font
    {
        internal PixelFarm.Agg.Fonts.Font font;
        public AggFont(PixelFarm.Agg.Fonts.Font font)
        {
            this.font = font;
        }
        public override FontInfo FontInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object InnerFont
        {
            get
            {
                return font;
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override float Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontStyle Style
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override IntPtr ToHfont()
        {
            throw new NotImplementedException();
        }
         
    }
}
namespace PixelFarm.Agg.Fonts
{
    public abstract class Font : IDisposable
    {
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


        public void Dispose()
        {
            OnDispose();
        }

        ~Font()
        {
            Dispose();
        }

        public abstract bool IsAtlasFont { get; }
    }


}