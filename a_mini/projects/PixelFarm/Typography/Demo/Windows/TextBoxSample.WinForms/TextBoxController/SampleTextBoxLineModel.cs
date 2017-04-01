//MIT, 2014-2017, WinterDev

using System.Collections.Generic;
using Typography.TextLayout;
using Typography.Rendering;

namespace SampleWinForms.UI
{
   

    class Line
    {

        int _caretCharIndex = 0;//default
        internal List<char> _charBuffer = new List<char>();
        internal List<GlyphPlan> _glyphPlans = new List<GlyphPlan>();
        internal List<UserCharToGlyphIndexMap> _userCharToGlyphMap = new List<UserCharToGlyphIndexMap>();

        bool _contentChanged = true;

        /// <summary>
        /// add char at current pos
        /// </summary>
        /// <param name="c"></param>
        public void AddChar(char c)
        {
            //add char at cursor index
            int count = _charBuffer.Count;

            if (_caretCharIndex == count)
            {
                //at the end                
                _charBuffer.Add(c);
                _caretCharIndex++;
            }
            else if (_caretCharIndex < count)
            {
                _charBuffer.Insert(_caretCharIndex, c);
                _caretCharIndex++;
            }
            else
            {
                throw new System.NotSupportedException();
            }
            _contentChanged = true;
        }
        public void DoBackspace()
        {
            if (_caretCharIndex == 0)
            {
                return;
            }
            //
            int count = _charBuffer.Count;
            if (count == 0)
            {
                _caretCharIndex = 0;
                return;
            }

            //end
            _caretCharIndex--;
            _charBuffer.RemoveAt(_caretCharIndex);

            _contentChanged = true;
        }
        public void DoDelete()
        {
            //simulate by do right + backspace
            int count = _charBuffer.Count;
            if (_caretCharIndex == count)
            {
                //caret is on the end
                //just return
                return;
            }
            DoRight();
            DoBackspace();
        }
        public void DoLeft()
        {
            int count = _charBuffer.Count;
            if (count == 0)
            {
                _caretCharIndex = 0;
                return;
            }
            else if (_caretCharIndex > 0)
            {
                //this is on the end
                _caretCharIndex--;

                //check if the caret can rest on this glyph?
                if (_caretCharIndex > 0)
                {
                    //find its mapping to glyph index
                    UserCharToGlyphIndexMap userCharToGlyphMap = _userCharToGlyphMap[_caretCharIndex];
                    int mapToGlyphIndex = userCharToGlyphMap.glyphIndexListOffset_plus1;
                    //
                    if (mapToGlyphIndex == 0)
                    {
                        //no map 
                        DoLeft();   //recursive ***
                        return;
                    }
                    //-------------------------
                    //we -1 ***
                    GlyphPlan glyphPlan = _glyphPlans[userCharToGlyphMap.glyphIndexListOffset_plus1 - 1];
                    if (glyphPlan.advX <= 0)
                    {
                        //caret can't rest here
                        //so
                        DoLeft();   //recursive ***
                        return;
                    }
                    //---------------------
                    // 
                }
            }
            else
            {

            }

        }
        public void DoRight()
        {
            int count = _charBuffer.Count;
            if (count == 0)
            {
                return;
            }
            else if (_caretCharIndex < count)
            {
                //this is on the end
                _caretCharIndex++;

                //check if the caret can rest on this glyph?
                if (_caretCharIndex < count)
                {

                    //find its mapping to glyph index
                    UserCharToGlyphIndexMap userCharToGlyphMap = _userCharToGlyphMap[_caretCharIndex];
                    int mapToGlyphIndex = userCharToGlyphMap.glyphIndexListOffset_plus1;
                    //
                    if (mapToGlyphIndex == 0)
                    {
                        //no map 
                        DoRight();   //recursive ***
                        return;
                    }
                    //-------------------------
                    //we -1 ***
                    GlyphPlan glyphPlan = _glyphPlans[userCharToGlyphMap.glyphIndexListOffset_plus1 - 1];
                    if (glyphPlan.advX <= 0)
                    {
                        //caret can't rest here
                        //so
                        DoRight();   //recursive ***
                        return;
                    }
                }
            }
            else
            {

            }
        }
        public void DoHome()
        {
            _caretCharIndex = 0;
        }
        public void DoEnd()
        {
            _caretCharIndex = _charBuffer.Count;
        }
        public int CharCount
        {
            get { return 0; }
        }
        public bool ContentChanged { get { return _contentChanged; } set { _contentChanged = value; } }
        public int CaretCharIndex { get { return _caretCharIndex; } }
        public void SetCaretCharIndex(int newindex)
        {
            if (newindex >= 0 && newindex <= _charBuffer.Count)
            {
                _caretCharIndex = newindex;
            }
        }

        public void SetCharIndexFromPos(float x, float y, float toPxScale)
        {

            int count = _glyphPlans.Count;
            float accum_x = 0;
            for (int i = 0; i < count; ++i)
            {
                float thisGlyphW = _glyphPlans[i].advX * toPxScale;
                accum_x += thisGlyphW;
                if (accum_x > x)
                {
                    //TODO: review here 
                    //for some glyph that has been substitued 
                    //glyph may not match with actual user char in the _line    

                    float xoffset_on_glyph = (x - (accum_x - thisGlyphW));
                    if (xoffset_on_glyph >= (thisGlyphW / 2))
                    {
                        _caretCharIndex = i + 1;
                        //check if the caret can rest on this pos or not
                        UserCharToGlyphIndexMap map = _userCharToGlyphMap[_caretCharIndex];
                        if (map.glyphIndexListOffset_plus1 == 0)
                        {
                            //no map
                            //cant rest here
                            if (_caretCharIndex < count)
                            {
                                DoRight();
                            }
                        }
                        else
                        {
                            //has map
                            if (_caretCharIndex < count && _glyphPlans[map.glyphIndexListOffset_plus1 - 1].advX <= 0)
                            {
                                //recursive ***
                                DoRight(); //
                            }
                        }
                    }
                    else
                    {
                        _caretCharIndex = i;
                        //check if the caret can rest on this pos or not
                        UserCharToGlyphIndexMap map = _userCharToGlyphMap[_caretCharIndex];
                        if (map.glyphIndexListOffset_plus1 == 0)
                        {
                            //no map
                            //cant rest here
                            if (_caretCharIndex > 0)
                            {
                                //recursive ***
                                DoLeft();
                            }
                        }
                        else
                        {
                            //has map
                            if (_caretCharIndex < count && _glyphPlans[map.glyphIndexListOffset_plus1 - 1].advX <= 0)
                            {
                                //recursive ***
                                DoLeft();
                            }
                        }

                    }
                    //stop
                    break;
                }
            }
        }

        public UserCharToGlyphIndexMap GetCurrentCharToGlyphMap()
        {
            return _userCharToGlyphMap[_caretCharIndex];
        }
    }


    class VisualLine
    {

        Line _line;
        DevTextPrinterBase _printer;

        float toPxScale = 1;
        public void BindLine(Line line)
        {
            this._line = line;
        }
        public void BindPrinter(DevTextPrinterBase printer)
        {
            _printer = printer;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public void SetCharIndexFromPos(float x, float y)
        {
            _line.SetCharIndexFromPos(x, y, toPxScale);
        }

        public void Draw()
        {

            List<GlyphPlan> glyphPlans = _line._glyphPlans;
            List<UserCharToGlyphIndexMap> userCharToGlyphIndexMap = _line._userCharToGlyphMap;
            if (_line.ContentChanged)
            {
                //re-calculate 
                char[] textBuffer = _line._charBuffer.ToArray();
                glyphPlans.Clear();


                userCharToGlyphIndexMap.Clear();

                //read glyph plan and userCharToGlyphIndexMap                 
                _printer.GlyphLayoutMan.GenerateGlyphPlans(textBuffer, 0, textBuffer.Length, glyphPlans, userCharToGlyphIndexMap);

                toPxScale = _printer.Typeface.CalculateToPixelScaleFromPointSize(_printer.FontSizeInPoints);
                _line.ContentChanged = false;
            }

            if (glyphPlans.Count > 0)
            {

                _printer.DrawFromGlyphPlans(glyphPlans, X, Y);
                //draw caret 
                //not blink in this version
                int caret_index = _line.CaretCharIndex;
                //find caret pos based on glyph plan
                //TODO: check when do gsub (glyph number may not match with user char number)                 

                if (caret_index == 0)
                {
                    _printer.DrawCaret(X, this.Y);
                }
                else
                {
                    UserCharToGlyphIndexMap map = userCharToGlyphIndexMap[caret_index - 1];
                    GlyphPlan p = glyphPlans[map.glyphIndexListOffset_plus1 + map.len - 2];
                    _printer.DrawCaret(X + ((p.x + p.advX) * toPxScale), this.Y);
                }
            }
            else
            {

                _printer.DrawCaret(X, this.Y);
            }


        }
    }


    class TextRun
    {
        char[] _srcTextBuffer;
        int _startAt;
        int _len;

        GlyphPlanListCache _glyphPlanListCache;

        public TextRun(char[] srcTextBuffer, int startAt, int len)
        {
            this._srcTextBuffer = srcTextBuffer;
            this._startAt = startAt;
            this._len = len;
        }
        public void SetGlyphPlan(List<GlyphPlan> glyphPlans, int startAt, int len)
        {
            _glyphPlanListCache = new GlyphPlanListCache(glyphPlans, startAt, len);
        }
        struct GlyphPlanListCache
        {
            public readonly List<GlyphPlan> glyphPlans;
            public readonly int startAt;
            public readonly int len;
            public GlyphPlanListCache(List<GlyphPlan> glyphPlans, int startAt, int len)
            {
                this.glyphPlans = glyphPlans;
                this.startAt = startAt;
                this.len = len;
            }

        }
    }
}
