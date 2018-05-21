using System;
using Typography.Contours;
namespace Typography.Rendering
{
    public class GlyphImage
    {
        int[] pixelBuffer;
        public GlyphImage(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public RectangleF OriginalGlyphBounds
        {
            get;
            set;
        }
        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }
        public bool IsBigEndian
        {
            get;
            private set;
        }
        public int BorderXY
        {
            get;
            set;
        }
        public int[] GetImageBuffer()
        {
            return pixelBuffer;
        }
        public void SetImageBuffer(int[] pixelBuffer, bool isBigEndian)
        {
            this.pixelBuffer = pixelBuffer;
            this.IsBigEndian = isBigEndian;
        }
        /// <summary>
        /// texture offset X from original glyph
        /// </summary>
        public double TextureOffsetX { get; set; }

        double _textureOffsetY;
        /// <summary>
        /// texture offset Y from original glyph 
        /// </summary>
        public double TextureOffsetY
        {
            get { return _textureOffsetY; }
            set
            {
                _textureOffsetY = value;
            }
        }
    }

    public class CacheGlyph
    {
        public int borderX;
        public int borderY;
        internal GlyphImage img;
        public Rectangle area;
        public char character; //TODO: this should be code point(int32)
        public int glyphIndex;

    }

    public class TextureFontGlyphData
    {
        public float BorderX { get; set; }
        public float BorderY { get; set; }
        public float AdvanceX { get; set; }
        public float AdvanceY { get; set; }
        public float BBoxXMin { get; set; }
        public float BBoxXMax { get; set; }
        public float BBoxYMin { get; set; }
        public float BBoxYMax { get; set; }
        public float ImgWidth { get; set; }
        public float ImgHeight { get; set; }
        //-----
        public float HAdvance { get; set; }
        public float HBearingX { get; set; }
        public float HBearingY { get; set; }
        //-----
        public float VAdvance { get; set; }
        public float VBearingX { get; set; }
        public float VBearingY { get; set; }
        //---
        public double TextureXOffset { get; set; }
        public double TextureYOffset { get; set; }

        public Rectangle Rect
        {
            get;
            set;
        }

    }

}