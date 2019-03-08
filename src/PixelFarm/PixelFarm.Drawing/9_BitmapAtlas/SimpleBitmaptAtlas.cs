//MIT, 2016-present, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;


namespace PixelFarm.Drawing.BitmapAtlas
{
    public enum TextureKind : byte
    {
        StencilLcdEffect, //default
        StencilGreyScale,
        Msdf,
        Bitmap
    }
    public class SimpleBitmaptAtlas
    {
        AtlasItemImage _totalGlyphImage;
        Dictionary<ushort, BitmapMapData> _glyphLocations = new Dictionary<ushort, BitmapMapData>();

        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// original font size in point unit
        /// </summary>
        public float OriginalFontSizePts { get; set; }
        public TextureKind TextureKind { get; set; }
        public string FontFilename { get; set; }

        public void AddGlyph(ushort glyphIndex, BitmapMapData glyphData)
        {
            _glyphLocations.Add(glyphIndex, glyphData);
        }
        public AtlasItemImage TotalGlyph
        {
            get => _totalGlyphImage;
            set => _totalGlyphImage = value;
        }
        public bool TryGetGlyphMapData(ushort glyphIndex, out BitmapMapData glyphdata)
        {
            if (!_glyphLocations.TryGetValue(glyphIndex, out glyphdata))
            {
                glyphdata = null;
                return false;
            }
            return true;
        }
    }

}