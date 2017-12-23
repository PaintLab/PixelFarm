//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
//

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using Typography.OpenFont;
using Typography.TextServices;

namespace PixelFarm.DrawingGL
{

    static class ActiveFontAtlasService
    {
        struct FontTextureKey
        {
            public float sizeInPoint;
            public string fontName;
            public FontStyle fontStyle;
            public string scriptLang;
        }

        struct TextureAtlasCache
        {
            public FontFace fontFace;
            public SimpleFontAtlas atlas;
        }

        static Dictionary<FontTextureKey, TextureAtlasCache> s_cachedFontAtlas = new Dictionary<FontTextureKey, TextureAtlasCache>();

        //TODO: review here again

        public static ActualFont GetTextureFontAtlasOrCreateNew(
            LayoutFarm.OpenFontTextService fontService,
            RequestFont font,
            out SimpleFontAtlas fontAtlas)
        {
            //1. resolve for actual typeface
            Typeface typeface = fontService.ResolveTypeface(font);
            //2. 

            //check if we have created this font
            var key = new FontTextureKey();
            key.fontName = font.Name;
            //key.scriptLang = scLang.shortname;
            key.sizeInPoint = font.SizeInPoints;
            key.fontStyle = font.Style;
            //------------------------
            TextureAtlasCache found;
            FontFace ff = null;
            if (!s_cachedFontAtlas.TryGetValue(key, out found))
            {
                //if not, then create the new one 

                //ptimize here
                //TODO: review
                TextureFontCreationParams creationParams = new TextureFontCreationParams();
                creationParams.originalFontSizeInPoint = font.SizeInPoints;
                //creationParams.scriptLang = scLang;
                //creationParams.writeDirection = WriteDirection.LTR;//default 
                //TODO: review here, langBits can be created with scriptLang ?
                creationParams.scriptLangs = new ScriptLang[]
                {
                    Typography.OpenFont.ScriptLangs.Latin,
                    Typography.OpenFont.ScriptLangs.Thai //eg. Thai, for test with complex script, you can change to your own
                };
                //
                creationParams.textureKind = PixelFarm.Drawing.Fonts.TextureKind.StencilGreyScale;
                if (font.SizeInPoints >= 4 && font.SizeInPoints <= 14)
                {
                    //creationParams.hintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction;
                    //creationParams.hintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly;
                    //creationParams.hintTechnique = Typography.Contours.HintTechnique.CustomAutoFit;

                }
                //
                ff = TextureFontLoader.LoadFont(typeface, creationParams, out fontAtlas);


                //cache it 
                var textureAtlasCache = new TextureAtlasCache();
                textureAtlasCache.fontFace = ff;
                textureAtlasCache.atlas = fontAtlas;
                s_cachedFontAtlas.Add(key, textureAtlasCache);
                return ff.GetFontAtPointSize(font.SizeInPoints);
            }
            fontAtlas = found.atlas;
            ff = found.fontFace;
            return ff.GetFontAtPointSize(font.SizeInPoints);
        }

    }


}