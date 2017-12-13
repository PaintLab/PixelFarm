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


    public class OpenFontTextService : ITextService
    {
        //TODO: this class should be a Typography Service 
        //plan: remove dependcy on IFonts here

        TypefaceStore typefaceStore;
        GlyphLayout glyphLayout;
        GlyphPlanList userGlyphPlanList;
        List<UserCharToGlyphIndexMap> userCharToGlyphMapList;

        TextShapingService _shapingServices;
        Dictionary<int, Typeface> _resolvedTypefaceCache = new Dictionary<int, Typeface>();
        CustomBreaker _textBreaker;


        readonly int _system_id;
        Typography.OpenFont.ScriptLang _defaultScLang;

        public OpenFontTextService()
        {
            // 
            _system_id = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetNewCacheSystemId();
            typefaceStore = new TypefaceStore();
            typefaceStore.FontCollection = InstalledFontCollection.GetSharedFontCollection(null);
            glyphLayout = new GlyphLayout(); //create glyph layout with default value
            userGlyphPlanList = new GlyphPlanList();
            userCharToGlyphMapList = new List<UserCharToGlyphIndexMap>();
            //
            _shapingServices = new TextShapingService(null, glyphLayout);

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

        public void CalculateGlyphAdvancePos(char[] str, int startAt, int len, RequestFont font, int[] glyphXAdvances, out int outputTotalW, out int outputLineHeight)
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

            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            int endBefore = startAt + len;
            outputTotalW = 0;
            for (int i = startAt; i < endBefore; ++i)
            {
                GlyphPlan glyphPlan = userGlyphPlanList[i];
                float tx = glyphPlan.ExactX;
                float ty = glyphPlan.ExactY;
                double actualAdvX = glyphPlan.AdvanceX;

                //if you want to snap each glyph to grid ... => Round it 
                outputTotalW += glyphXAdvances[i] = (int)Math.Round(actualAdvX * scale);
            }
            outputLineHeight = (int)Math.Round(typeface.CalculateRecommendLineSpacing() * scale);
        }
        public void CalculateGlyphAdvancePos(ILineSegmentList lineSegs, RequestFont font, int[] glyphXAdvances, out int outputTotalW, out int lineHeight)
        {

            //layout  
            //from font
            //resolve for typeface

            // 
            Typeface typeface = typefaceStore.GetTypeface(font.Name, InstalledFontStyle.Normal);
            glyphLayout.Typeface = typeface;


            MyLineSegmentList mylineSegs = (MyLineSegmentList)lineSegs;
            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);

            outputTotalW = 0;
            char[] str = mylineSegs._str;
            TextBuffer textBuffer = new TextBuffer(str);

            int j = mylineSegs.Count;
            for (int i = 0; i < j; ++i)
            {
                userGlyphPlanList.Clear();
                userCharToGlyphMapList.Clear();

                //get each segment
                MyLineSegment lineSeg = mylineSegs.GetSegment(i);
                glyphLayout.ScriptLang = lineSeg.scriptLang;
                _shapingServices.SetCurrentFont(typeface, font.SizeInPoints, lineSeg.scriptLang);
                //
                //CACHING ...., reduce number of GSUB/GPOS
                //
                //we cache used line segment for a while
                //we ask for caching context for a specific typeface and font size 
                GlyphPlanSequence seq = _shapingServices.LayoutText(textBuffer, lineSeg.StartAt, lineSeg.Length); 
                GlyphPlanList planList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(seq);

                int seqLen = seq.len;
                int endAt = seq.startAt + seqLen;
                int pos = 0;
                for (int s = seq.startAt; s < endAt; ++s)
                {
                    GlyphPlan glyphPlan = planList[s];
                    float tx = glyphPlan.ExactX;
                    float ty = glyphPlan.ExactY;
                    double actualAdvX = glyphPlan.AdvanceX;
                    outputTotalW += glyphXAdvances[pos] = (int)Math.Round(actualAdvX * scale);
                    pos++;
                }

                //int glyphPlanCount = userCharToGlyphMapList.Count;
                //for (int m = 0; m < glyphPlanCount; ++m)
                //{
                //    GlyphPlan glyphPlan = userGlyphPlanList[m];
                //    float tx = glyphPlan.ExactX;
                //    float ty = glyphPlan.ExactY;
                //    double actualAdvX = glyphPlan.AdvanceX;
                //    outputTotalW += glyphXAdvances[m] = (int)Math.Round(actualAdvX * scale);
                //}
            }
            lineHeight = (int)Math.Round(typeface.CalculateRecommendLineSpacing() * scale);
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
            return new Size((int)Math.Round(accumW), (int)Math.Round(accumH));
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
        float ITextService.MeasureBlankLineHeight(RequestFont font)
        {
            LineSpacingChoice sel_linespcingChoice;
            Typeface typeface = ResolveTypeface(font);
            return (int)(typeface.CalculateRecommendLineSpacing(out sel_linespcingChoice) *
                typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints));
        }

        public bool SupportsWordBreak
        {
            get
            {
                return true;
            }
        }

        class MyLineSegment : ILineSegment
        {
            MyLineSegmentList owner;
            int startAt;
            int len;
            internal ScriptLang scriptLang;

            public MyLineSegment(MyLineSegmentList owner, int startAt, int len)
            {
                this.owner = owner;
                this.startAt = startAt;
                this.len = len;
            }
            public int Length
            {
                get { return len; }
            }
            public int StartAt
            {
                get { return startAt; }
            }
            public string GetText()
            {
                return owner.GetSegmentText(this.startAt, len);
            }
            public int GetHashKey()
            {
                return owner.GetHashKey(this.startAt, len);
            }
        }
        class MyLineSegmentList : ILineSegmentList
        {
            MyLineSegment[] _segments;
            internal char[] _str;
            int _startAt;
            int _len;
            public MyLineSegmentList(char[] str, int startAt, int len)
            {
                this._str = str;
                this._startAt = startAt;
                this._len = len;
            }
            public ILineSegment this[int index]
            {
                get { return _segments[index]; }
            }
            public int Count
            {
                get { return _segments.Length; }
            }
            public void SetResultLineSegments(MyLineSegment[] segments)
            {
                this._segments = segments;
            }
            public MyLineSegment GetSegment(int index)
            {
                return _segments[index];
            }
            public string GetSegmentText(int segmentOffset, int len)
            {
                //start at 
                return new string(_str, _startAt + segmentOffset, len);
            }
            public int GetHashKey(int segmentOffset, int len)
            {
                return Typography.TextServices.CRC32.CalculateCRC32(_str, _startAt + segmentOffset, len);
            }
        }
        List<MyLineSegment> _resuableLineSegments = new List<MyLineSegment>();

        public ILineSegmentList BreakToLineSegments(char[] str, int startAt, int len)
        {
            _resuableLineSegments.Clear();
            //user must setup the CustomBreakerBuilder before use              
            if (_textBreaker == null)
            {
                _textBreaker = CustomBreakerBuilder.NewCustomBreaker();
            }
            MyLineSegmentList lineSegs = new MyLineSegmentList(str, startAt, len);
            int cur_startAt = startAt;
            _textBreaker.BreakWords(str, cur_startAt, len);

            foreach (BreakSpan breakSpan in _textBreaker.GetBreakSpanIter())
            {
                MyLineSegment lineSeg = new MyLineSegment(lineSegs, breakSpan.startAt, breakSpan.len);
                //set segment kind/ script lang
                char sample = str[breakSpan.startAt];
                ScriptLang selectedScriptLang = null;
                if (sample == ' ')
                {
                    //whitespace
                    selectedScriptLang = _defaultScLang;
                }
                else if (char.IsWhiteSpace(sample))
                {
                    //other whitespace
                    selectedScriptLang = _defaultScLang;
                }
                else
                {

                    Typography.OpenFont.ScriptLang scLang;
                    if (!Typography.OpenFont.ScriptLangs.TryGetScriptLang(sample, out scLang))
                    {
                        //not found
                        //we should decide using current typeface 
                        //or asking for alternate typeface 
                        //if  the current type face is not support the request scriptLang
                        //  
                        //use default
                        scLang = _defaultScLang;
                    }
                    selectedScriptLang = scLang;
                }
                lineSeg.scriptLang = selectedScriptLang;
                _resuableLineSegments.Add(lineSeg);
            }

            lineSegs.SetResultLineSegments(_resuableLineSegments.ToArray());
            return lineSegs;
        }
        //-----------------------------------
        static OpenFontTextService()
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
