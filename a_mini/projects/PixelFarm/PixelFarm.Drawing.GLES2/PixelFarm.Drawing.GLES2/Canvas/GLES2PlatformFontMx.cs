//BSD, 2014-2016, WinterDev 

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using PixelFarm.DrawingGL;

namespace PixelFarm.Drawing.Fonts
{
    //cross-platform font mx***
    class GLES2PlatformFontMx
    {
        //gdiplus platform can handle following font       
        //1. gdi font
        //2. vector font
        //3. opentype font
        //4. texture font

        //public WinGdiPlusFont ResolveForWinGdiPlusFont(RequestFont r)
        //{
        //    WinGdiPlusFont winGdiPlusFont = r.ActualFont as WinGdiPlusFont;
        //    if (winGdiPlusFont != null)
        //    {
        //        return winGdiPlusFont;
        //    }
        //    //check if 
        //    throw new NotSupportedException();
        //}

        ////---------
        //static GdiPlusPlatformFontMx s_gdiPlusFontMx = new GdiPlusPlatformFontMx();
        //public static GdiPlusPlatformFontMx Default { get { return s_gdiPlusFontMx; } }

        public ActualFont ResolveForGdiFont(RequestFont font)
        {
            return null;
        }
        public ActualFont ResolveForTextureFont(RequestFont font)
        {
            return null;
        }
        static GLES2PlatformFontMx s_fontMx = new GLES2PlatformFontMx();
        public static GLES2PlatformFontMx Default { get { return s_fontMx; } }
    }

    /// <summary>
    /// store native font here
    /// </summary>
    class NativeFontStore
    {
        Dictionary<InstalledFont, FontFace> fonts = new Dictionary<InstalledFont, FontFace>();
        Dictionary<FontKey, ActualFont> registerFonts = new Dictionary<FontKey, ActualFont>();
        //--------------------------------------------------
        InstalledFontCollection installFonts;
        string defaultLang = "en";
        HBDirection defaultHbDirection = HBDirection.HB_DIRECTION_LTR;
        int defaultScriptCode = 0;
        public NativeFontStore()
        {

        }
        public void LoadFonts(IInstalledFontProvider provider)
        {
            installFonts = new InstalledFontCollection();
            installFonts.LoadInstalledFont(provider.GetInstalledFontIter());
        }
        public ActualFont LoadFont(string fontName, float fontSizeInPoints)
        {
            //find install font from fontname
            InstalledFont found = installFonts.LoadFont(fontName, InstalledFontStyle.Regular);
            if (found == null)
            {
                return null;
            }

            FontFace fontFace;
            if (!fonts.TryGetValue(found, out fontFace))
            {
                fontFace = FreeTypeFontLoader.LoadFont(found, defaultLang, defaultHbDirection, defaultScriptCode);
                if (fontFace == null)
                {
                    throw new NotSupportedException();
                }
                fonts.Add(found, fontFace);//register
            }
            //-----------
            //create font at specific size from this fontface
            FontKey fontKey = new FontKey(fontName, fontSizeInPoints, FontStyle.Regular);
            ActualFont createdFont;
            if (!registerFonts.TryGetValue(fontKey, out createdFont))
            {
                createdFont = fontFace.GetFontAtPointsSize(fontSizeInPoints);
            }
            //-----------
            return createdFont;
        }

        public ActualFont GetResolvedNativeFont(RequestFont reqFont)
        {
            ActualFont found;
            registerFonts.TryGetValue(reqFont.FontKey, out found);
            return found;
        }
    }
}