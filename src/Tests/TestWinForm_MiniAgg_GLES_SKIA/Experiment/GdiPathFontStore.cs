////MIT, 2014-present, WinterDev  
//using System.Collections.Generic;
//namespace PixelFarm.Drawing.Fonts
//{
//    class GdiPathFontStore
//    {
//        Dictionary<string, GdiPathFontFace> fontFaces = new Dictionary<string, GdiPathFontFace>();
//        Dictionary<FontKey, GdiPathFont> registerFonts = new Dictionary<FontKey, GdiPathFont>();

//        internal GdiPathFont LoadFont(string fontName, float fontPointSize)
//        {
//            //load font from specific file 
//            GdiPathFontFace fontFace;
//            if (!fontFaces.TryGetValue(fontName, out fontFace))
//            {
//                //create new font face               
//                fontFace = new GdiPathFontFace(fontName);
//                fontFaces.Add(fontName, fontFace);
//            }
//            if (fontFace == null)
//            {
//                return null;
//            }

//            GdiPathFont gdiPathFont = fontFace.GetFontAtSpecificSize((int)fontPointSize);
//            FontKey f = new FontKey(fontName, fontPointSize, FontStyle.Regular);
//            registerFonts.Add(f, gdiPathFont);
//            return gdiPathFont;
//        }
//        internal GdiPathFont GetResolvedFont(RequestFont f)
//        {
//            GdiPathFont found;
//            registerFonts.TryGetValue(f.FontKey, out found);
//            return found;
//        }

//    }
//}