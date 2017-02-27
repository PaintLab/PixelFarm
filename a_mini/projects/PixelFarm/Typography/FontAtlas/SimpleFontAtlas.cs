//MIT, 2016-2017, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using Typography.OpenFont;

namespace Typography.Rendering
{


    public class SimpleFontAtlas
    {

        Dictionary<int, TextureFontGlyphData> codePointLocations = new Dictionary<int, TextureFontGlyphData>();

        public int Width { get; set; }
        public int Height { get; set; }

        public void AddGlyph(int codePoint, TextureFontGlyphData glyphData)
        {
            codePointLocations.Add(codePoint, glyphData);
        }
        public bool GetRectByCodePoint(int codepoint, out TextureFontGlyphData glyphdata)
        {
            if (!codePointLocations.TryGetValue(codepoint, out glyphdata))
            {
                glyphdata = null;
                return false;
            }
            return true;
        }
    }

    

}