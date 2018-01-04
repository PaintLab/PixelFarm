//MIT, 2016-2018, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;

using Typography.Rendering;

namespace PixelFarm.Drawing.Fonts
{
    public enum TextureKind
    {
        Msdf,
        StencilGreyScale,
        StencilLcdEffect,
        Bitmap,
    }
    public class SimpleFontAtlas
    {
        GlyphImage totalGlyphImage;
        Dictionary<int, TextureFontGlyphData> codePointLocations = new Dictionary<int, TextureFontGlyphData>();

        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// original font size in point unit
        /// </summary>
        public float OriginalFontSizePts { get; set; }
        public TextureKind TextureKind { get; set; }
        public void AddGlyph(int codePoint, TextureFontGlyphData glyphData)
        {
            codePointLocations.Add(codePoint, glyphData);
        }

        public GlyphImage TotalGlyph
        {
            get { return totalGlyphImage; }
            set { totalGlyphImage = value; }
        }
        public bool TryGetGlyphDataByCodePoint(int codepoint, out TextureFontGlyphData glyphdata)
        {
            if (!codePointLocations.TryGetValue(codepoint, out glyphdata))
            {
                glyphdata = null;
                return false;
            }
            return true;
        }


        //-----
        //pre-calculate values 
        public float SourceTextureScale { get; private set; }
        public float TargetTextureScale { get; private set; }
        public float FinalTextureScale { get; private set; }
        //TODO: review here, or we should use scaled
        //UNSCALED version
        public int OriginalRecommendLineSpacing { get; private set; }
        public int OriginalAscending { get; private set; }
        public int OriginalDescending { get; private set; }
        public int OriginalLineGap { get; private set; }
       

        public void SetTextureScaleInfo(float sourceTextureScale, float targetTextureScale)
        {
            this.SourceTextureScale = sourceTextureScale;
            this.TargetTextureScale = targetTextureScale;
            this.FinalTextureScale = targetTextureScale / sourceTextureScale;
        }

        public void SetCommonFontMetricValues(int ascending, int descending, int linegap, int recommendLineSpacing)
        {
            //TODO: review here, or we should use scaled
            //UNSCALED version


            this.OriginalAscending = ascending;
            this.OriginalDescending = descending;
            this.OriginalLineGap = linegap;
            
            this.OriginalRecommendLineSpacing = recommendLineSpacing;

        }

    }

}