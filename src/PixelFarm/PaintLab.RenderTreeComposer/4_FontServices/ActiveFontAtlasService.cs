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
            public TextureKind textureKind;
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
            TextureFontCreationParams creationParams,
            out SimpleFontAtlas fontAtlas)
        {
            //1. resolve for actual typeface
            Typeface typeface = fontService.ResolveTypeface(font);
            //2. 

            //check if we have created this font
            var key = new FontTextureKey();
            key.fontName = font.Name;
            key.textureKind = creationParams.textureKind;
            key.sizeInPoint = font.SizeInPoints;
            key.fontStyle = font.Style;
            //------------------------
            TextureAtlasCache found;
            FontFace fontface = null;
            if (!s_cachedFontAtlas.TryGetValue(key, out found))
            {
                //if not, then create the new one 

                //optimize here
                //TODO: review 

                if (font.SizeInPoints >= 4 && font.SizeInPoints <= 14)
                {
                    //creationParams.hintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction;
                    //creationParams.hintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly;
                    //creationParams.hintTechnique = Typography.Contours.HintTechnique.CustomAutoFit;

                }
                //
                fontface = TextureFontLoader.LoadFont(typeface, creationParams, out fontAtlas);


                //cache it 
                var textureAtlasCache = new TextureAtlasCache();
                textureAtlasCache.fontFace = fontface;
                textureAtlasCache.atlas = fontAtlas;
                s_cachedFontAtlas.Add(key, textureAtlasCache);
                return fontface.GetFontAtPointSize(font.SizeInPoints);
            }
            fontAtlas = found.atlas;
            fontface = found.fontFace;
            return fontface.GetFontAtPointSize(font.SizeInPoints);
        }
       
    }


}