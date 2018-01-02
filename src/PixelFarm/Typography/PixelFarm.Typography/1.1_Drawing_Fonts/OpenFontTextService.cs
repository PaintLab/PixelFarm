//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.OpenFont.Extensions;

using Typography.TextLayout;
using Typography.TextServices;

namespace LayoutFarm
{


    public class OpenFontTextService : ITextService
    {
        /// <summary>
        /// instance of Typography lib's text service
        /// </summary>
        TextServices _typographyTxtServices;
        Dictionary<int, Typeface> _resolvedTypefaceCache = new Dictionary<int, Typeface>();
        readonly int _system_id;
        public OpenFontTextService()
        {
            // 
            _system_id = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetNewCacheSystemId();

            //set up typography text service
            _typographyTxtServices = new TextServices();
            //create typography service
            //you can implement this service on your own
            //just see the code inside the service 
            //script lang has a potentail effect on how the layout engine instance work.
            //
            //so try to set default script lang to the layout engine instance
            //from current system default value...
            //user can set this to other choices...
            //eg. directly specific the script lang  

            _typographyTxtServices.TrySettingScriptLangFromCurrentThreadCultureInfo();
            // ... or specific the scriptlang manully, eg. ...
            //_shapingServices.SetDefaultScriptLang(scLang);
            //_shapingServices.SetCurrentScriptLang(scLang);
            //--------------- 
        }
        public ScriptLang CurrentScriptLang
        {
            get { return _typographyTxtServices.CurrentScriptLang; }
        }

        public void CalculateGlyphAdvancePos(ref TextBufferSpan textBufferSpan, RequestFont font, int[] outputGlyphAdvances, out int outputTotalW, out int outputLineHeight)
        {
            CalculateGlyphAdvancePos(ref textBufferSpan, this.BreakToLineSegments(ref textBufferSpan), font, outputGlyphAdvances, out outputTotalW, out outputLineHeight);
        }

        ReusableTextBuffer _reusableTextBuffer = new ReusableTextBuffer();

        public void CalculateGlyphAdvancePos(ref TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs, RequestFont font,
            int[] outputGlyphAdvances, out int outputTotalW, out int lineHeight)
        {

            //layout  
            //from font
            //resolve for typeface
            //  
            Typeface typeface = ResolveTypeface(font);
            _typographyTxtServices.SetCurrentFont(typeface, font.SizeInPoints);


            MyLineSegmentList mylineSegs = (MyLineSegmentList)lineSegs;
            float scale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);
            outputTotalW = 0;
            int j = mylineSegs.Count;
            int pos = 0; //start at 0

            _reusableTextBuffer.SetRawCharBuffer(textBufferSpan.GetRawCharBuffer());

            for (int i = 0; i < j; ++i)
            {

                //userGlyphPlanList.Clear();
                //userCharToGlyphMapList.Clear(); 
                //get each segment
                MyLineSegment lineSeg = mylineSegs.GetSegment(i);
                //each line seg may has different script lang
                _typographyTxtServices.CurrentScriptLang = lineSeg.scriptLang;
                //
                //CACHING ...., reduce number of GSUB/GPOS
                //
                //we cache used line segment for a while
                //we ask for caching context for a specific typeface and font size   
                GlyphPlanSequence seq = _typographyTxtServices.GetUnscaledGlyphPlanSequence(_reusableTextBuffer,
                     lineSeg.StartAt,
                     lineSeg.Length);

                GlyphPlanList planList = GlyphPlanSequence.UnsafeGetInteralGlyphPlanList(seq);
                //IMPORTANT
                //num of glyph may more or less than original user input char buffer
                //

                int seqLen = seq.len;
                int endAt = seq.startAt + seqLen;
                int seq_startAt = seq.startAt;

                for (int s = seq_startAt; s < endAt; ++s)
                {
                    GlyphPlan glyphPlan = planList[s];
                    float tx = glyphPlan.ExactX;
                    float ty = glyphPlan.ExactY;
                    double actualAdvX = glyphPlan.AdvanceX;

                    outputTotalW += 
                        outputGlyphAdvances[pos + glyphPlan.input_cp_offset] += 
                        (int)Math.Round(actualAdvX * scale);
                }
                pos += seqLen;
            }

            //

            lineHeight = (int)Math.Round(typeface.CalculateRecommendLineSpacing() * scale);

            _reusableTextBuffer.SetRawCharBuffer(null);
        }

        public Typeface ResolveTypeface(RequestFont font)
        {
            //from user's request font
            //resolve to actual Typeface
            //get data from... 
            //cache level-1 (attached inside the request font)
            Typeface typeface = PixelFarm.Drawing.Internal.RequestFontCacheAccess.GetActualFont<Typeface>(font, _system_id);
            if (typeface != null) return typeface;
            //
            //cache level-2 (stored in this Ifonts)
            if (!_resolvedTypefaceCache.TryGetValue(font.FontKey, out typeface))
            {
                //not found ask the typeface store to load that font
                //....
                typeface = _typographyTxtServices.GetTypeface(font.Name, font.Style.ConvToInstalledFontStyle());
                if (typeface == null)
                    throw new NotSupportedException();
                //
                //cache here (level-1)
                _resolvedTypefaceCache.Add(font.FontKey, typeface);
            }
            //and cache into level-0

            float pxSize = Typeface.ConvPointsToPixels(font.SizeInPoints);

            float pxscale = typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints);

            float recommedLineSpacing = typeface.CalculateRecommendLineSpacing() * pxscale;
            float descentInPx = typeface.Descender * pxscale;
            float ascentInPx = typeface.Ascender * pxscale;
            float lineGapInPx = typeface.LineGap * pxscale;

            PixelFarm.Drawing.Internal.RequestFontCacheAccess.SetActualFont(font, _system_id, typeface);
            PixelFarm.Drawing.Internal.RequestFontCacheAccess.SetGeneralFontMetricInfo(font,
                pxSize,
                ascentInPx,
                descentInPx,
                lineGapInPx,
                recommedLineSpacing);

            return typeface;
        }
        public float MeasureWhitespace(RequestFont f)
        {
            throw new NotImplementedException();
        }



        public GlyphPlanSequence CreateGlyphPlanSeq(ref TextBufferSpan textBufferSpan, RequestFont font)
        {

            Typeface typeface = ResolveTypeface(font);
            _typographyTxtServices.SetCurrentFont(typeface, font.SizeInPoints);

            _reusableTextBuffer.SetRawCharBuffer(textBufferSpan.GetRawCharBuffer());

            return _typographyTxtServices.GetUnscaledGlyphPlanSequence(_reusableTextBuffer, textBufferSpan.start, textBufferSpan.len);
        }
        public Size MeasureString(ref TextBufferSpan textBufferSpan, RequestFont font)
        {
            Typeface typeface = ResolveTypeface(font);
            _typographyTxtServices.SetCurrentFont(typeface, font.SizeInPoints);
            int w, h;

            //
            _typographyTxtServices.MeasureString(textBufferSpan.GetRawCharBuffer(), textBufferSpan.start, textBufferSpan.len, out w, out h);
            return new Size(w, h);
        }
        public void MeasureString(ref TextBufferSpan textBufferSpan, RequestFont font, int limitWidth, out int charFit, out int charFitWidth)
        {
            Typeface typeface = ResolveTypeface(font);
            _typographyTxtServices.SetCurrentFont(typeface, font.SizeInPoints);

            charFit = 0;
            _typographyTxtServices.MeasureString(textBufferSpan.GetRawCharBuffer(), textBufferSpan.start, textBufferSpan.len, limitWidth, out charFit, out charFitWidth);

        }
        float ITextService.MeasureBlankLineHeight(RequestFont font)
        {
            LineSpacingChoice sel_linespcingChoice;
            Typeface typeface = ResolveTypeface(font);
            return (int)(Math.Round(typeface.CalculateRecommendLineSpacing(out sel_linespcingChoice) *
                                    typeface.CalculateScaleToPixelFromPointSize(font.SizeInPoints)));
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
            //public string GetText()
            //{
            //    return owner.GetSegmentText(this.startAt, len);
            //}
            //public int GetHashKey()
            //{
            //    return owner.GetHashKey(this.startAt, len);
            //}
        }
        class MyLineSegmentList : ILineSegmentList
        {
            MyLineSegment[] _segments;

            int _startAt;
            int _len;
            public MyLineSegmentList(int startAt, int len)
            {
                //this._str = str;
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

            public int GetHashKey(char[] orgString, int segmentOffset, int len)
            {
                return Typography.TextServices.CRC32.CalculateCRC32(orgString, _startAt + segmentOffset, len);
            }
        }
        List<MyLineSegment> _resuableLineSegments = new List<MyLineSegment>();

        public ILineSegmentList BreakToLineSegments(ref TextBufferSpan textBufferSpan)
        {
            _resuableLineSegments.Clear();

            //a text buffer span is separated into multiple line segment list

            char[] str = textBufferSpan.GetRawCharBuffer();

            MyLineSegmentList lineSegs = new MyLineSegmentList(textBufferSpan.start, textBufferSpan.len);
            int cur_startAt = textBufferSpan.start;
            foreach (Typography.TextServices.BreakSpan breakSpan in _typographyTxtServices.BreakToLineSegments(str, textBufferSpan.start, textBufferSpan.len))
            {
                MyLineSegment lineSeg = new MyLineSegment(lineSegs, breakSpan.startAt, breakSpan.len);
                lineSeg.scriptLang = breakSpan.scLang;
                _resuableLineSegments.Add(lineSeg);
            }

            //TODO: review here, 
            //check if we need to create new array everytime?
            lineSegs.SetResultLineSegments(_resuableLineSegments.ToArray());
            _resuableLineSegments.Clear();
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
