//MIT, 2014-2017, WinterDev 

using System.IO;
using Typography.Rendering;

namespace PixelFarm.Drawing.Fonts
{
    public static class TextureFontLoader
    {
        public static FontFace LoadFont(string fontfile,
            ScriptLang scriptLang,
            WriteDirection writeDirection = WriteDirection.LTR)
        {

            //1. read font info
            ManagedFontFace openFont = (ManagedFontFace)OpenFontLoader.LoadFont(fontfile, scriptLang, writeDirection);

            //2. build texture font on the fly! OR load from prebuilt file

            string xmlFontFileInfo = "";
            GlyphImage glyphImg = null;

            MySimpleFontAtlasBuilder atlasBuilder = new Typography.Rendering.MySimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontFileInfo);
            glyphImg = atlasBuilder.BuildSingleImage(); //we can create a new glyph or load from prebuilt file
            fontAtlas.TotalGlyph = glyphImg;

            var textureFontFace = new TextureFontFace(openFont, fontAtlas);
            return textureFontFace;
        }
    }


}