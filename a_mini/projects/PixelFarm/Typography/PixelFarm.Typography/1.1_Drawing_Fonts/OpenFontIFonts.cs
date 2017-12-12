//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.OpenFont.Extensions;

using Typography.TextLayout;
using Typography.TextServices;
using Typography.TextBreak;

namespace LayoutFarm
{
    public class OpenFontIFonts : IFonts
    {
        //TODO: this class should be a Typography Service 
        //plan: remove dependcy on IFonts here

        TypefaceStore typefaceStore;
        GlyphLayout glyphLayout;
        GlyphPlanList userGlyphPlanList;
        List<UserCharToGlyphIndexMap> userCharToGlyphMapList;

        Dictionary<int, Typeface> _resolvedTypefaceCache = new Dictionary<int, Typeface>();
        CustomBreaker _textBreaker;


        readonly int _system_id;
        Typography.OpenFont.ScriptLang _defaultScLang;

        public OpenFontIFonts()
        {
            // 
            _system_id = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetNewCacheSystemId();
            typefaceStore = new TypefaceStore();
            typefaceStore.FontCollection = InstalledFontCollection.GetSharedFontCollection(null);
            glyphLayout = new GlyphLayout(); //create glyph layout with default value
            userGlyphPlanList = new GlyphPlanList();
            userCharToGlyphMapList = new List<UserCharToGlyphIndexMap>();

            //script lang has a potentail effect on how the layout engine instance work.
            //
            //so try to set default script lang to the layout engine instance
            //from current system default value...
            //user can set this to other choices...
            //eg. directly specific the script lang 

            //System.Text.Encoding defaultEncoding = System.Text.Encoding.Default;
            _defaultScLang = glyphLayout.ScriptLang;

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
                _defaultScLang = scLang;
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

        List<MeasuredStringBox> _reusableMeasureBoxList = new List<MeasuredStringBox>();

        public Size MeasureString(char[] str, int startAt, int len, RequestFont font)
        {
            Typeface typeface = ResolveTypeface(font);

            if (str.Length < 1)
            {
                return new Size(0, typeface.CalculateRecommendLineSpacing());
            }

            _reusableMeasureBoxList.Clear(); //reset 

            glyphLayout.Typeface = typeface;
            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            glyphLayout.FontSizeInPoints = font.SizeInPoints;

            //NOET:at this moment, simple operation
            //may not be simple... 

            //-------------------
            //input string may contain more than 1 script lang
            //user can parse it by other parser
            //but in this code, we use our Typography' parser
            //-------------------
            //user must setup the CustomBreakerBuilder before use              
            if (_textBreaker == null)
            {
                _textBreaker = CustomBreakerBuilder.NewCustomBreaker();
            }


            int cur_startAt = startAt;
            _textBreaker.BreakWords(str, cur_startAt, len);

            float accumW = 0;
            float accumH = 0;



            foreach (BreakSpan breakSpan in _textBreaker.GetBreakSpanIter())
            {
                //at this point
                //we assume that 1 break span 
                //has 1 script lang, and we examine it
                //with sample char
                char sample = str[breakSpan.startAt];
                if (sample == ' ')
                {
                    //whitespace
                    glyphLayout.ScriptLang = _defaultScLang;
                }
                else if (char.IsWhiteSpace(sample))
                {
                    //other whitespace
                    glyphLayout.ScriptLang = _defaultScLang;
                }
                else
                {
                    //
                    Typography.OpenFont.ScriptLang scLang;
                    if (Typography.OpenFont.ScriptLangs.TryGetScriptLang(sample, out scLang))
                    {
                        //we should decide to use
                        //current typeface
                        //or ask for alternate typeface 
                        //if  the current type face is not support the request scriptLang
                        // 
                    }
                    else
                    {
                        //not found
                        //use default
                        scLang = _defaultScLang;
                    }
                    glyphLayout.ScriptLang = scLang;
                }

                MeasuredStringBox result;
                //measure string at specific px scale

                glyphLayout.MeasureString(str, breakSpan.startAt, breakSpan.len, out result, scale);
                ConcatMeasureBox(ref accumW, ref accumH, ref result);
            }
            return new Size((int)accumW, (int)accumH);
        }
        static void ConcatMeasureBox(ref float accumW, ref float accumH, ref MeasuredStringBox measureBox)
        {
            accumW += measureBox.width;
            float h = measureBox.CalculateLineHeight();
            if (h > accumH)
            {
                accumH = h;
            }
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
