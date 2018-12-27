//BSD, 2014-present, WinterDev 

using System;
using System.Collections.Generic;
using Typography.OpenFont;
using Typography.FontManagement;
namespace PixelFarm.Drawing.Fonts
{
    //cross-platform font mx***

    static class GLES2PlatformFontMx
    {


        internal static ScriptLang s_defaultScriptLang = ScriptLangs.Latin;
        static IInstalledTypefaceProvider s_installedTypefaceProvider;
        static Dictionary<string, LateTextureFontInfo> s_textureBitmapInfos = new Dictionary<string, LateTextureFontInfo>();

        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider fontLoader)
        {
            s_installedTypefaceProvider = fontLoader;
        }

        public static InstalledTypeface GetInstalledFont(string fontName, Typography.FontManagement.TypefaceStyle style)
        {
            return s_installedTypefaceProvider.GetInstalledTypeface(fontName, style);
        }

        public static void AddTextureFontInfo(string fontname, string fontMapFile, string textureBitmapFile)
        {
            //add info for texture font
            s_textureBitmapInfos[fontname] = new LateTextureFontInfo(fontname, fontMapFile, textureBitmapFile);
        }

        class LateTextureFontInfo
        {
            public LateTextureFontInfo(string fontName, string fontMapFile, string textureBitmapFile)
            {
                this.FontName = fontName;
                this.FontMapFile = fontMapFile;
                this.TextureBitmapFile = textureBitmapFile;
            }
            public string FontName { get; set; }
            public string FontMapFile { get; set; }
            public string TextureBitmapFile { get; set; }
            //  public TextureFontFace Fontface { get; set; }
        }
    }

}