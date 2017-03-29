//MIT, 2016-2017, WinterDev
using System.Collections.Generic;
using Typography.OpenFont;

namespace Typography.Rendering
{

    //see also: PixelFarm's  class HintedVxsGlyphCollection 

    public class GlyphMeshCollection<T>
    {
        //hint glyph collection        
        //per typeface
        Dictionary<ushort, T> _currentGlyphDic = null;
        Dictionary<HintedVxsConvtextKey, Dictionary<ushort, T>> _hintedGlyphs = new Dictionary<HintedVxsConvtextKey, Dictionary<ushort, T>>();
        public void SetCacheInfo(Typeface typeface, float sizeInPts, HintTechnique hintTech)
        {
            //check if we have create the context for this request parameters?
            var key = new HintedVxsConvtextKey() { hintTech = hintTech, sizeInPts = sizeInPts, typeface = typeface };
            if (!_hintedGlyphs.TryGetValue(key, out _currentGlyphDic))
            {
                //if not found 
                //create new
                _currentGlyphDic = new Dictionary<ushort, T>();
                _hintedGlyphs.Add(key, _currentGlyphDic);
            }
        }
        public bool TryGetCacheGlyph(ushort glyphIndex, out T vxs)
        {
            return _currentGlyphDic.TryGetValue(glyphIndex, out vxs);
        }
        public void RegisterCachedGlyph(ushort glyphIndex, T vxs)
        {
            _currentGlyphDic[glyphIndex] = vxs;
        }
        struct HintedVxsConvtextKey
        {
            public HintTechnique hintTech;
            public Typeface typeface;
            public float sizeInPts;
        }
    }

}