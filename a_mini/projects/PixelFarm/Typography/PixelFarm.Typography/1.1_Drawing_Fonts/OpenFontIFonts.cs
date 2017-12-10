//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts; 

using Typography.OpenFont;
using Typography.TextLayout;
using Typography.TextServices;

namespace LayoutFarm
{
    public class OpenFontIFonts : IFonts
    {

        IFontLoader _fontloader;
        TypefaceStore typefaceStore;
        GlyphLayout glyphLayout;
        List<GlyphPlan> userGlyphPlanList;
        List<UserCharToGlyphIndexMap> userCharToGlyphMapList;

        Dictionary<int, Typeface> _resolvedTypefaceCache = new Dictionary<int, Typeface>();


        readonly int _system_id;

        public OpenFontIFonts(IFontLoader fontloader)
        {
            _fontloader = fontloader;
            _system_id = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetNewCacheSystemId();

            typefaceStore = new TypefaceStore();
            typefaceStore.FontCollection = InstalledFontCollection.GetSharedFontCollection(null);
            glyphLayout = new GlyphLayout(); //create glyph layout with default value
            userGlyphPlanList = new List<GlyphPlan>();
            userCharToGlyphMapList = new List<UserCharToGlyphIndexMap>();
        }
        public void CalculateGlyphAdvancePos(char[] str, int startAt, int len, RequestFont font, int[] glyphXAdvances)
        {

            //layout  
            //from font
            //resolve for typeface
            userGlyphPlanList.Clear();
            userCharToGlyphMapList.Clear();
            // 
            Typeface typeface = typefaceStore.GetTypeface(font.Name, InstalledFontStyle.Normal);
            glyphLayout.Typeface = typeface;
            glyphLayout.GenerateGlyphPlans(str, startAt, len, userGlyphPlanList, userCharToGlyphMapList);

            //
            //
            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            int endBefore = startAt + len;

            for (int i = startAt; i < endBefore; ++i)
            {
                GlyphPlan glyphPlan = userGlyphPlanList[i];
                float tx = glyphPlan.ExactX;
                float ty = glyphPlan.ExactY;
                double actualAdvX = glyphPlan.AdvanceX;
                glyphXAdvances[i] = (int)Math.Round(actualAdvX);
            }
        }

        Typeface ResolveTypeface(RequestFont font)
        {
            //from user's request font
            //resolve to actual Typeface

            //get data from...
            //cache level-0 (attached inside the request font)
            Typeface typeface = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetActualFont<Typeface>(font, _system_id);
            if (typeface != null) return typeface;
            //
            //cache level-1 (stored in this Ifonts)
            if (!_resolvedTypefaceCache.TryGetValue(font.FontKey, out typeface))
            {

                //not found ask the typeface store to load that font
                typeface = typefaceStore.GetTypeface(font.Name, font.Style.ConvToInstalledFontStyle());
                if (typeface == null)
                    throw new NotSupportedException();
                //
                //cache here (level-1)
                _resolvedTypefaceCache.Add(font.FontKey, typeface);
            }
            //and cache into level-0
            PixelFarm.Drawing.Internal.RequestFontCacheAccess.SetActualFont(font, _system_id, typeface);
            return typeface;
        }
        public float MeasureWhitespace(RequestFont f)
        {
            throw new NotImplementedException();
        }
        public Size MeasureString(char[] str, int startAt, int len, RequestFont font)
        {
            Typeface typeface = ResolveTypeface(font);
            glyphLayout.Typeface = typeface;
            MeasuredStringBox result;
            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            glyphLayout.FontSizeInPoints = font.SizeInPoints;

            //measure string at specific px scale
            glyphLayout.MeasureString(str, startAt, len, out result, scale);
            return new Size((int)result.width, (int)result.CalculateLineHeight()); 
        } 

        public int MeasureBlankLineHeight(RequestFont font)
        {
            Typeface typeface = ResolveTypeface(font);
            return (int)(typeface.LineSpacing * typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints));
        }
        float IFonts.MeasureBlankLineHeight(RequestFont font)
        {
            Typeface typeface = ResolveTypeface(font);
            return typeface.LineSpacing * typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
        }
    }
}
