//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.TextLayout;
using Typography.TextServices;
using Typography.OpenFont.Extensions;


namespace LayoutFarm
{
    public class OpenFontIFonts : IFonts
    {


        TypefaceStore typefaceStore;
        GlyphLayout glyphLayout;
        List<GlyphPlan> userGlyphPlanList;
        List<UserCharToGlyphIndexMap> userCharToGlyphMapList;

        Dictionary<int, Typeface> _resolvedTypefaceCache = new Dictionary<int, Typeface>();


        readonly int _system_id;

        public OpenFontIFonts()
        {
            // 
            _system_id = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetNewCacheSystemId();
            typefaceStore = new TypefaceStore();
            typefaceStore.FontCollection = InstalledFontCollection.GetSharedFontCollection(null);
            glyphLayout = new GlyphLayout(); //create glyph layout with default value
            userGlyphPlanList = new List<GlyphPlan>();
            userCharToGlyphMapList = new List<UserCharToGlyphIndexMap>();



            //script lang has a potentail effect on how the layout engine instance work.
            //
            //so try to set default script lang to the layout engine instance
            //from current system default value...
            //user can set this to other choices...
            //eg. directly specific the script lang 

            //System.Text.Encoding defaultEncoding = System.Text.Encoding.Default;
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Typography.OpenFont.ScriptLang scLang = null;
            string langFullName;
            if (IcuData.TryGetFullLanguageNameFromLangCode(
                 currentCulture.TwoLetterISOLanguageName,
                 currentCulture.ThreeLetterISOLanguageName,
                 out langFullName))
            {
                scLang = Typography.OpenFont.ScriptLangs.GetRegisteredScriptLangFromLanguageName(langFullName);
            }
            if (scLang != null)
            {
                //set script lang to the engine
                glyphLayout.ScriptLang = scLang;
            }


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
            //input string may contain more than 1 script lang,
            //user can parse it by other parser
            //but in this code, we use our Typography' parser


            Typeface typeface = ResolveTypeface(font);
            glyphLayout.Typeface = typeface;
            MeasuredStringBox result;
            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            glyphLayout.FontSizeInPoints = font.SizeInPoints;

            //measure string at specific px scale
            glyphLayout.MeasureString(str, startAt, len, out result, scale);
            return new Size((int)result.width, (int)Math.Round(result.CalculateLineHeight()));
        }

        public int MeasureBlankLineHeight(RequestFont font)
        {
            LineSpacingChoice sel_linespcingChoice;
            Typeface typeface = ResolveTypeface(font);
            return (int)(typeface.CalculateRecommendLineSpacing(out sel_linespcingChoice) *
                typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints));
        }
        float IFonts.MeasureBlankLineHeight(RequestFont font)
        {
            LineSpacingChoice sel_linespcingChoice;
            Typeface typeface = ResolveTypeface(font);
            return (int)(typeface.CalculateRecommendLineSpacing(out sel_linespcingChoice) *
                typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints));
        }



        //-----------------------------------
        static OpenFontIFonts()
        {

            CurrentEnv.CurrentOSName = (IsOnMac()) ?
                         CurrentOSName.Mac :
                         CurrentOSName.Windows;
        }
        static bool _s_evaluatedOS;
        static bool _s_onMac;
        static bool IsOnMac()
        {

            if (_s_evaluatedOS) return _s_onMac;
            // 
            _s_evaluatedOS = true;
#if NETCORE
                return _s_onMac=  System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                  System.Runtime.InteropServices.OSPlatform.OSX);                    
#else

            return _s_onMac = (System.Environment.OSVersion.Platform == System.PlatformID.MacOSX);
#endif
        }

    }
}
