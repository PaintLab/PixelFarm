//MIT, 2016-2018, WinterDev



#if GL_ENABLE
using System;
using System.Collections.Generic;
//

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.Rendering;
using Typography.OpenFont.Extensions;


namespace PixelFarm.DrawingGL
{
    class MySimpleGLBitmapFontManager
    {
        GLBitmapCache<SimpleFontAtlas> _loadedGlyphs;
        Dictionary<int, SimpleFontAtlas> _createdAtlases = new Dictionary<int, SimpleFontAtlas>();

        LayoutFarm.OpenFontTextService textServices;

        ScriptLang[] _currentScriptLangs;
        TextureKind _textureKind;
        public MySimpleGLBitmapFontManager(TextureKind textureKind, LayoutFarm.OpenFontTextService textServices)
        {
            this.textServices = textServices;
            //glyph cahce for specific atlas
            _loadedGlyphs = new GLBitmapCache<SimpleFontAtlas>(atlas =>
            {
                //create new one
                Typography.Rendering.GlyphImage totalGlyphImg = atlas.TotalGlyph;
                //load to glbmp 
                GLBitmap found = new GLBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer(), false);
                found.IsInvert = false;
                return found;
            });

            _textureKind = textureKind;
        }

        public void SetCurrentScriptLangs(ScriptLang[] currentScriptLangs)
        {
            this._currentScriptLangs = currentScriptLangs;
        }


        /// <summary>
        /// get from cache or create a new one
        /// </summary>
        /// <param name="reqFont"></param>
        /// <returns></returns>
        public SimpleFontAtlas GetFontAtlas(RequestFont reqFont,
            out GLBitmap glBmp)
        {

            int fontKey = reqFont.FontKey;
            SimpleFontAtlas fontAtlas;
            if (!_createdAtlases.TryGetValue(fontKey, out fontAtlas))
            {
                Typeface resolvedTypeface = textServices.ResolveTypeface(reqFont);
                //if we don't have 
                //the create it 
                SimpleFontAtlasBuilder atlasBuilder = null;
                var textureGen = new GlyphTextureBitmapGenerator();
                textureGen.CreateTextureFontFromScriptLangs(
                    resolvedTypeface,
                    reqFont.SizeInPoints,
                   _textureKind,
                   _currentScriptLangs,
                    (glyphIndex, glyphImage, outputAtlasBuilder) =>
                    {
                        if (outputAtlasBuilder != null)
                        {
                            //finish
                            atlasBuilder = outputAtlasBuilder;
                        }
                    }
                );

                //
                GlyphImage totalGlyphsImg = atlasBuilder.BuildSingleImage();

                //totalGlyphsImg = Sharpen(totalGlyphsImg, 1); //test shapen primary image
                //-               
                //
                //create atlas
                fontAtlas = atlasBuilder.CreateSimpleFontAtlas();
                fontAtlas.TotalGlyph = totalGlyphsImg;
#if DEBUG
                //save glyph image for debug
                //PixelFarm.Agg.ActualImage.SaveImgBufferToPngFile(
                //    totalGlyphsImg.GetImageBuffer(),
                //    totalGlyphsImg.Width * 4,
                //    totalGlyphsImg.Width, totalGlyphsImg.Height,
                //    "d:\\WImageTest\\total_" + reqFont.Name + "_" + reqFont.SizeInPoints + ".png");
#endif 

                //cache the atlas
                _createdAtlases.Add(fontKey, fontAtlas);
                //
                //calculate some commonly used values
                fontAtlas.SetTextureScaleInfo(
                    resolvedTypeface.CalculateScaleToPixelFromPointSize(fontAtlas.OriginalFontSizePts),
                    resolvedTypeface.CalculateScaleToPixelFromPointSize(reqFont.SizeInPoints));
                //TODO: review here, use scaled or unscaled values
                fontAtlas.SetCommonFontMetricValues(
                    resolvedTypeface.Ascender,
                    resolvedTypeface.Descender,
                    resolvedTypeface.LineGap,
                    resolvedTypeface.CalculateRecommendLineSpacing());
            }

            glBmp = _loadedGlyphs.GetOrCreateNewOne(fontAtlas);
            return fontAtlas;
        }


#if DEBUG
        /// <summary>
        /// test only, shapen org image with Paint.net sharpen filter
        /// </summary>
        /// <param name="org"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        static GlyphImage Sharpen(GlyphImage org, int radius)
        {
            GlyphImage newImg = new GlyphImage(org.Width, org.Height);
            Agg.Imaging.ShapenFilterPdn sharpen1 = new Agg.Imaging.ShapenFilterPdn();
            int[] orgBuffer = org.GetImageBuffer();
            unsafe
            {
                fixed (int* orgHeader = &orgBuffer[0])
                {
                    int[] output = sharpen1.Sharpen(orgHeader, org.Width, org.Height, org.Width * 4, radius);
                    newImg.SetImageBuffer(output, org.IsBigEndian);
                }
            }

            return newImg;
        }
#endif
        public void Clear()
        {
            _loadedGlyphs.Clear();
        }
    }

}

#endif