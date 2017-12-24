//MIT, 2014-2017, WinterDev    
using System.Collections.Generic;
using Typography.OpenFont;
using Typography.TextLayout;


namespace Typography.TextServices
{

    public struct BreakSpan
    {
        public int startAt;
        public ushort len;
        public short flags;
        public ScriptLang scLang;

    }

    public class TextServices
    {
        //user can do text shaping by their own
        //this class is optional
        //it provide cache for previous 'used/ wellknown' Word-glyphPlans for a specific font 
        // 

        GlyphPlanCacheForTypefaceAndScriptLang _currentShapingContext;
        Dictionary<TextShapingContextKey, GlyphPlanCacheForTypefaceAndScriptLang> _registerShapingContexts = new Dictionary<TextShapingContextKey, GlyphPlanCacheForTypefaceAndScriptLang>();
        GlyphLayout _glyphLayout;

        Typeface _currentTypeface;
        float _fontSizeInPts;
        ScriptLang _defaultScriptLang;
        TypefaceStore typefaceStore;
        ScriptLang scLang;

        //GlyphPlanList userGlyphPlanList;
        //List<UserCharToGlyphIndexMap> userCharToGlyphMapList;

        public TextServices()
        {

            typefaceStore = new TypefaceStore();
            typefaceStore.FontCollection = InstalledFontCollection.GetSharedFontCollection(null);

            _glyphLayout = new GlyphLayout();
        }
        public bool TrySettingScriptLangFromCurrentThreadCultureInfo()
        {
            //accessory...
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Typography.OpenFont.ScriptLang scLang = null;
            string langFullName;
            if (IcuData.TryGetFullLanguageNameFromLangCode(
                 currentCulture.TwoLetterISOLanguageName,
                 currentCulture.ThreeLetterISOLanguageName,
                 out langFullName))
            {
                scLang = Typography.OpenFont.ScriptLangs.GetRegisteredScriptLangFromLanguageName(langFullName);
                SetDefaultScriptLang(scLang);
                CurrentScriptLang = scLang;
                return true;
            }

            throw new System.NotSupportedException();
            return false;
        }
        public void SetDefaultScriptLang(ScriptLang scLang)
        {
            this.scLang = _defaultScriptLang = scLang;
        }

        public ScriptLang CurrentScriptLang
        {
            get { return scLang; }
            set { this.scLang = _glyphLayout.ScriptLang = value; }
        }

        public void SetCurrentFont(Typeface typeface, float fontSizeInPts)
        {
            //check if we have the cache-key or create a new one.
            var key = new TextShapingContextKey(typeface, _glyphLayout.ScriptLang);
            if (!_registerShapingContexts.TryGetValue(key, out _currentShapingContext))
            {
                //not found
                //the create the new one 
                var shapingContext = new GlyphPlanCacheForTypefaceAndScriptLang(typeface, _glyphLayout.ScriptLang);
                //shaping context setup ...
                _registerShapingContexts.Add(key, shapingContext);
                _currentShapingContext = shapingContext;
            }

            _currentTypeface = _glyphLayout.Typeface = typeface;
            _fontSizeInPts = fontSizeInPts;

            //_glyphLayout.FontSizeInPoints = _fontSizeInPts = fontSizeInPts;
        }
        public Typeface GetTypeface(string name, InstalledFontStyle installedFontStyle)
        {
            return typefaceStore.GetTypeface(name, installedFontStyle);
        }
        /// <summary>
        /// shaping input string with current font and current script         
        /// </summary>
        /// <param name="inputString"></param>
        public GlyphPlanSequence LayoutText(string inputString)
        {
            //output is glyph plan for this input string 
            //input string need to be splited into 'words'. 
            TextBuffer textBuffer = new TextBuffer(inputString.ToCharArray());
            return _currentShapingContext.Layout(_glyphLayout, textBuffer, 0, textBuffer.Len);
        }
        public GlyphPlanSequence LayoutText(TextBuffer buffer, int start, int len)
        {
            return _currentShapingContext.Layout(_glyphLayout, buffer, start, len);
        }


        internal void ClearAllRegisteredShapingContext()
        {
            _registerShapingContexts.Clear();
        }



        Typography.TextBreak.CustomBreaker _textBreaker;
        public IEnumerable<BreakSpan> BreakToLineSegments(char[] str, int startAt, int len)
        {
            //user must setup the CustomBreakerBuilder before use              
            if (_textBreaker == null)
            {
                _textBreaker = Typography.TextBreak.CustomBreakerBuilder.NewCustomBreaker();
            }
            int cur_startAt = startAt;
            _textBreaker.BreakWords(str, cur_startAt, len);
            foreach (TextBreak.BreakSpan sp in _textBreaker.GetBreakSpanIter())
            {
                //our service select a proper script lang info and add to the breakspan

                //at this point
                //we assume that 1 break span 
                //has 1 script lang, and we examine it
                //with sample char
                char sample = str[sp.startAt];

                ScriptLang selectedScriptLang;
                if (sample == ' ')
                {
                    //whitespace
                    selectedScriptLang = _defaultScriptLang;
                }
                else if (char.IsWhiteSpace(sample))
                {
                    //other whitespace
                    selectedScriptLang = _defaultScriptLang;
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
                        selectedScriptLang = scLang;
                    }
                    else
                    {
                        //not found
                        //use default
                        selectedScriptLang = _defaultScriptLang;
                    }
                }

                BreakSpan breakspan = new BreakSpan();
                breakspan.startAt = sp.startAt;
                breakspan.len = sp.len;
                breakspan.scLang = selectedScriptLang;
                yield return breakspan;
            }

        }

        GlyphPlanList _reusableGlyphPlanList = new GlyphPlanList();
        List<MeasuredStringBox> _reusableMeasureBoxList = new List<MeasuredStringBox>();
        public void MeasureString(char[] str, int startAt, int len, out int w, out int h)
        {
            //measure string


            if (str.Length < 1)
            {
                w = h = 0;
            }
            _reusableMeasureBoxList.Clear(); //reset 

            float pxscale = _currentTypeface.CalculateScaleToPixelFromPointSize(_fontSizeInPts);
            //NOET:at this moment, simple operation
            //may not be simple...  
            //-------------------
            //input string may contain more than 1 script lang
            //user can parse it by other parser
            //but in this code, we use our Typography' parser
            //-------------------
            //user must setup the CustomBreakerBuilder before use         

            int cur_startAt = startAt;
            float accumW = 0;
            float accumH = 0;

            foreach (BreakSpan breakSpan in BreakToLineSegments(str, startAt, len))
            {


                //measure string at specific px scale 
                _glyphLayout.Layout(str, breakSpan.startAt, breakSpan.len);
                //
                _reusableGlyphPlanList.Clear();
                GlyphLayoutExtensions.GenerateGlyphPlan(_glyphLayout.ResultUnscaledGlyphPositions, pxscale, _reusableGlyphPlanList);
                //measure string size

                var result = new MeasuredStringBox(
                    _reusableGlyphPlanList.AccumAdvanceX * pxscale,
                    _currentTypeface.Ascender * pxscale,
                    _currentTypeface.Descender * pxscale,
                    _currentTypeface.LineGap * pxscale,
                     Typography.OpenFont.Extensions.TypefaceExtensions.CalculateRecommendLineSpacing(_currentTypeface) * pxscale);
                //
                ConcatMeasureBox(ref accumW, ref accumH, ref result);

                //public static void MeasureString(
                //        this GlyphLayout glyphLayout,
                //        char[] textBuffer,
                //        int startAt,
                //        int len, out MeasuredStringBox strBox, float scale)
                //{
                //    throw new NotSupportedException();
                //    //GlyphPlanList outputGlyphPlans = glyphLayout._myGlyphPlans;
                //    //outputGlyphPlans.Clear();
                //    //glyphLayout.Layout(textBuffer, startAt, len, outputGlyphPlans);

                //    ////
                //    //int j = outputGlyphPlans.Count;
                //    //Typeface currentTypeface = glyphLayout.Typeface;
                //    //if (j == 0)
                //    //{


                //    //    strBox = new 
                //public static void MeasureString(
                //        this GlyphLayout glyphLayout,
                //        char[] textBuffer,
                //        int startAt,
                //        int len, out MeasuredStringBox strBox, float scale)
                //{
                //    throw new NotSupportedException();
                //    //GlyphPlanList outputGlyphPlans = glyphLayout._myGlyphPlans;
                //    //outputGlyphPlans.Clear();
                //    //glyphLayout.Layout(textBuffer, startAt, len, outputGlyphPlans);

                //    ////
                //    //int j = outputGlyphPlans.Count;
                //    //Typeface currentTypeface = glyphLayout.Typeface;
                //    //if (j == 0)
                //    //{


                //    //    strBox = new MeasuredStringBox(0,
                //    //        currentTypeface.Ascender * scale,
                //    //        currentTypeface.Descender * scale,
                //    //        currentTypeface.LineGap * scale,
                //    //        Typography.OpenFont.Extensions.TypefaceExtensions.CalculateRecommendLineSpacing(currentTypeface) * scale);

                //    //}
                //    //else
                //    //{
                //    //    //TEST, 
                //    //    //if you want to snap each glyph to grid (1px or 0.5px) by ROUNDING
                //    //    //we can do it here,this produces a predictable caret position result
                //    //    //

                //    //    int accumW = 0;
                //    //    for (int i = 0; i < j; ++i)
                //    //    {
                //    //        GlyphPlan glyphPlan = outputGlyphPlans[i];
                //    //        float scaleW = glyphPlan.AdvanceX * scale;
                //    //        //select proper integer version
                //    //        accumW += (int)Math.Round(scaleW);
                //    //    }

                //    //    strBox = new MeasuredStringBox(accumW,
                //    //            currentTypeface.Ascender * scale,
                //    //            currentTypeface.Descender * scale,
                //    //            currentTypeface.LineGap * scale,
                //    //            Typography.OpenFont.Extensions.TypefaceExtensions.CalculateRecommendLineSpacing(currentTypeface) * scale);
                //    //}
                //}(0,
                //    //        currentTypeface.Ascender * scale,
                //    //        currentTypeface.Descender * scale,
                //    //        currentTypeface.LineGap * scale,
                //    //        Typography.OpenFont.Extensions.TypefaceExtensions.CalculateRecommendLineSpacing(currentTypeface) * scale);

                //    //}
                //    //else
                //    //{
                //    //    //TEST, 
                //    //    //if you want to snap each glyph to grid (1px or 0.5px) by ROUNDING
                //    //    //we can do it here,this produces a predictable caret position result
                //    //    //

                //    //    int accumW = 0;
                //    //    for (int i = 0; i < j; ++i)
                //    //    {
                //    //        GlyphPlan glyphPlan = outputGlyphPlans[i];
                //    //        float scaleW = glyphPlan.AdvanceX * scale;
                //    //        //select proper integer version
                //    //        accumW += (int)Math.Round(scaleW);
                //    //    }

                //    //    strBox = new MeasuredStringBox(accumW,
                //    //            currentTypeface.Ascender * scale,
                //    //            currentTypeface.Descender * scale,
                //    //            currentTypeface.LineGap * scale,
                //    //            Typography.OpenFont.Extensions.TypefaceExtensions.CalculateRecommendLineSpacing(currentTypeface) * scale);
                //    //}
                //}  
            }

            w = (int)System.Math.Round(accumW);
            h = (int)System.Math.Round(accumH);
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


        struct TextShapingContextKey
        {

            readonly Typeface _typeface;
            readonly ScriptLang _scLang;

            public TextShapingContextKey(Typeface typeface, ScriptLang scLang)
            {
                this._typeface = typeface;
                this._scLang = scLang;
            }
#if DEBUG
            public override string ToString()
            {
                return _typeface + " " + _scLang;
            }
#endif
        }
    }


    class GlyphPlanSeqSet
    {
        //TODO: consider this value, make this a variable (static int)
        const int PREDEFINE_LEN = 10;

        /// <summary>
        /// common len 0-10?
        /// </summary>
        GlyphPlanSeqCollection[] _cacheSeqCollection1;
        //other len
        Dictionary<int, GlyphPlanSeqCollection> _cacheSeqCollection2; //lazy init
        public GlyphPlanSeqSet()
        {
            _cacheSeqCollection1 = new GlyphPlanSeqCollection[PREDEFINE_LEN];

            this.MaxCacheLen = 20;//stop caching, please managed this ...
                                  //TODO:
                                  //what is the proper number of cache word ?
                                  //init free dic
            for (int i = PREDEFINE_LEN - 1; i >= 0; --i)
            {
                _cacheSeqCollection1[i] = new GlyphPlanSeqCollection(i);
            }
        }
        public int MaxCacheLen
        {
            get;
            private set;
        }

        public GlyphPlanSeqCollection GetSeqCollectionOrCreateIfNotExist(int len)
        {
            if (len < PREDEFINE_LEN)
            {
                return _cacheSeqCollection1[len];
            }
            else
            {
                if (_cacheSeqCollection2 == null)
                {
                    _cacheSeqCollection2 = new Dictionary<int, GlyphPlanSeqCollection>();
                }
                GlyphPlanSeqCollection seqCol;
                if (!_cacheSeqCollection2.TryGetValue(len, out seqCol))
                {
                    //new one if not exist
                    seqCol = new GlyphPlanSeqCollection(len);
                    _cacheSeqCollection2.Add(len, seqCol);
                }
                return seqCol;
            }
        }
    }

    /// <summary>
    /// glyph-cache based on typeface and script-lang with specific gsub/gpos features
    /// </summary>
    class GlyphPlanCacheForTypefaceAndScriptLang
    {
        GlyphPlanBuffer _glyphPlanBuffer;
        Typeface _typeface;
        ScriptLang _scLang;
        GlyphPlanSeqSet _glyphPlanSeqSet;

        public GlyphPlanCacheForTypefaceAndScriptLang(Typeface typeface, ScriptLang scLang)
        {
            _typeface = typeface;
            _scLang = scLang;
            _glyphPlanBuffer = new GlyphPlanBuffer(new GlyphPlanList());
            _glyphPlanSeqSet = new GlyphPlanSeqSet();

        }
        GlyphPlanSequence CreateGlyphPlanSeq(GlyphLayout glyphLayout, TextBuffer buffer, int startAt, int len)
        {
            GlyphPlanList planList = GlyphPlanBuffer.UnsafeGetGlyphPlanList(_glyphPlanBuffer);
            int pre_count = planList.Count;
            glyphLayout.Typeface = _typeface;
            glyphLayout.ScriptLang = _scLang;
            glyphLayout.Layout(
                TextBuffer.UnsafeGetCharBuffer(buffer),
                startAt,
                len);

            //glyphLayout.ReadOutput(planList);

            int post_count = planList.Count;
            return new GlyphPlanSequence(_glyphPlanBuffer, pre_count, post_count - pre_count);
        }
        static int CalculateHash(TextBuffer buffer, int startAt, int len)
        {
            //reference,
            //https://stackoverflow.com/questions/2351087/what-is-the-best-32bit-hash-function-for-short-strings-tag-names
            return CRC32.CalculateCRC32(TextBuffer.UnsafeGetCharBuffer(buffer), startAt, len);
        }

        public GlyphPlanSequence Layout(GlyphLayout glyphLayout, TextBuffer buffer, int startAt, int len)
        {
            //this func get the raw char from buffer
            //and create glyph list 
            //check if we have the string cache in specific value 
            //---------
            if (len > _glyphPlanSeqSet.MaxCacheLen)
            {
                //layout string is too long to be cache
                //it need to split into small buffer

            }

            GlyphPlanSequence planSeq = GlyphPlanSequence.Empty;

            GlyphPlanSeqCollection seqCol = _glyphPlanSeqSet.GetSeqCollectionOrCreateIfNotExist(len);
            int hashValue = CalculateHash(buffer, startAt, len);
            if (!seqCol.TryGetCacheGlyphPlanSeq(hashValue, out planSeq))
            {
                ////not found then create glyph plan seq
                //bool useOutputScale = glyphLayout.UsePxScaleOnReadOutput;

                ////save 
                //some font may have 'special' glyph x,y at some font size(eg. for subpixel-rendering position)
                //but in general we store the new glyph plan seq with unscale glyph pos
                //glyphLayout.UsePxScaleOnReadOutput = false;
                planSeq = CreateGlyphPlanSeq(glyphLayout, buffer, startAt, len);
                //glyphLayout.UsePxScaleOnReadOutput = useOutputScale;//restore
                seqCol.Register(hashValue, planSeq);
            }
            //---
            //on unscale font=> we use original 



            return planSeq;
        }
    }

    class GlyphPlanSeqCollection
    {
        int _seqLen;
        /// <summary>
        /// dic of hash string value and the cache seq
        /// </summary>
        Dictionary<int, GlyphPlanSequence> _knownSeqs = new Dictionary<int, GlyphPlanSequence>();
        public GlyphPlanSeqCollection(int seqLen)
        {
            this._seqLen = seqLen;
        }
        public int SeqLen
        {
            get { return _seqLen; }
        }
        public void Register(int hashValue, GlyphPlanSequence seq)
        {
            _knownSeqs.Add(hashValue, seq);
        }
        public bool TryGetCacheGlyphPlanSeq(int hashValue, out GlyphPlanSequence seq)
        {
            return _knownSeqs.TryGetValue(hashValue, out seq);
        }
    }
}