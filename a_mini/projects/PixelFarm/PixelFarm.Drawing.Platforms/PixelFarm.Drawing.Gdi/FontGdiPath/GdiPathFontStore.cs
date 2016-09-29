//MIT, 2014-2016, WinterDev  
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{
    public static class GdiPathFontStore
    {
        static Dictionary<string, GdiPathFontFace> fontFaces = new Dictionary<string, GdiPathFontFace>();
        public static Font LoadFont(string fontName, float fontPointSize)
        {
            //load font from specific file 
            GdiPathFontFace fontFace;
            if (!fontFaces.TryGetValue(fontName, out fontFace))
            {
                //create new font face               
                fontFace = new GdiPathFontFace(fontName);
                fontFaces.Add(fontName, fontFace);
            }
            if (fontFace == null)
            {
                return null;
            }

            Font font = new Drawing.Font(fontName, fontPointSize);
            GdiPathFont gdiPathFont = fontFace.GetFontAtSpecificSize((int)fontPointSize);
            font.SetOutlineFont(gdiPathFont);
            return font;
        }


        //---------------------------------------------------
        //helper function
        public static int ConvertFromPointUnitToPixelUnit(float point)
        {
            //from FreeType Documenetation
            //pixel_size = (pointsize * (resolution/72);
            return (int)(point * 96 / 72);
        }
    }
}