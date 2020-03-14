//MIT, 2016-present, WinterDev
using System;

using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.BitmapAtlas
{
       
    public class BitmapAtlasItem
    {
        int[] _pixelBuffer;
        public BitmapAtlasItem(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }

        public RectangleF OriginalGlyphBounds { get; set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public bool IsBigEndian { get; private set; }

        public int BorderXY { get; set; }

        public int[] GetImageBuffer() => _pixelBuffer;
        //
        public void SetImageBuffer(int[] pixelBuffer, bool isBigEndian)
        {
            _pixelBuffer = pixelBuffer;
            this.IsBigEndian = isBigEndian;
        }
        public void SetImageBuffer(MemBitmap memBmp)
        {
            _pixelBuffer = PixelFarm.CpuBlit.MemBitmap.CopyImgBuffer(memBmp);
        }
        /// <summary>
        /// texture offset X from original glyph
        /// </summary>
        public short TextureOffsetX { get; set; }
        /// <summary>
        /// texture offset Y from original glyph 
        /// </summary>
        public short TextureOffsetY { get; set; }
 
    }

    class RelocationAtlasItem
    {
        public readonly ushort glyphIndex;
        internal readonly BitmapAtlasItem img;
        public Rectangle area;
        public RelocationAtlasItem(ushort glyphIndex, BitmapAtlasItem img)
        {
            this.glyphIndex = glyphIndex;
            this.img = img;
        }
#if DEBUG
        public override string ToString()
        {
            return glyphIndex.ToString();
        }
#endif
    }

    public class TextureGlyphMapData
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float TextureXOffset { get; set; }
        public float TextureYOffset { get; set; }

        public void GetRect(out int x, out int y, out int w, out int h)
        {
            x = Left;
            y = Top;
            w = Width;
            h = Height;
        }
#if DEBUG
        public override string ToString()
        {
            return "(" + Left + "," + Top + "," + Width + "," + Height + ")";
        }
#endif
    }

}