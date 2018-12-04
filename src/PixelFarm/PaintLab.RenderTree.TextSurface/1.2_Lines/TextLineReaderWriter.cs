//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{
    class TextLineWriter : TextLineReader
    {
        public TextLineWriter(EditableTextFlowLayer textLayer)
            : base(textLayer)
        {
        }
        //
        public TextSpanStyle CurrentSpanStyle => TextLayer.CurrentTextSpanStyle;
        //
        public void Reload(IEnumerable<EditableRun> runs)
        {
            this.TextLayer.Reload(runs);
        }
        public void Clear()
        {
            //clear all
            this.MoveToLine(0);
            ClearCurrentLine();
            //CurrentLine.Clear();
            EnsureCurrentTextRun();
        }
        public void EnsureCurrentTextRun(int index)
        {
            var run = CurrentTextRun;
            if (run == null || !run.HasParent)
            {

                SetCurrentCharIndexToBegin();
                if (index != -1)
                {
                    int limit = CurrentLine.CharCount;
                    if (index > limit)
                    {
                        index = limit;
                    }
                    SetCurrentCharIndex(index);
                }
            }
        }
        public void EnsureCurrentTextRun()
        {
            EnsureCurrentTextRun(CharIndex);
        }
        public void RemoveSelectedTextRuns(VisualSelectionRange selectionRange)
        {
            int precutIndex = selectionRange.StartPoint.LineCharIndex;
            CurrentLine.Remove(selectionRange);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(precutIndex);
        }

        public void ClearCurrentLine()
        {
            int currentCharIndex = CharIndex;
            CurrentLine.ReplaceAll(null);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(currentCharIndex);
        }
        public void ReplaceCurrentLine(IEnumerable<EditableRun> textRuns)
        {
            int currentCharIndex = CharIndex;
            CurrentLine.ReplaceAll(textRuns);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(currentCharIndex);
        }
        public void JoinWithNextLine()
        {
            EditableTextLine.InnerDoJoinWithNextLine(this.CurrentLine);
            EnsureCurrentTextRun();
        }
        char BackSpaceOneChar()
        {
            if (CurrentTextRun == null)
            {
                return '\0';
            }
            else
            {
                if (CharIndex == 0)
                {
                    return '\0';
                }

                //move back 1 char and do delete 
                EditableRun removingTextRun = CurrentTextRun;
                int removeIndex = CurrentTextRunCharIndex;
                SetCurrentCharStepLeft();
                char toBeRemovedChar = CurrentChar;

                EditableRun.InnerRemove(removingTextRun, removeIndex, 1, false);
                if (removingTextRun.CharacterCount == 0)
                {
                    CurrentLine.Remove(removingTextRun);
                    EnsureCurrentTextRun();
                }
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();
                return toBeRemovedChar;
            }
        }
        public EditableRun GetCurrentTextRun()
        {
            if (CurrentLine.IsBlankLine)
            {
                return null;
            }
            else
            {
                return CurrentTextRun;
            }
        }

        public bool CanAcceptThisChar(char c)
        {
            //TODO: review here, enable this feature or not
            //some char can't be a start char on blank line
            if (CurrentLine.IsBlankLine &&
                !InternalTextLayerController.CanCaretStopOnThisChar(c))
            {
                return false;
            }
            return true;
        }
        public void AddCharacter(char c)
        {
            if (CurrentLine.IsBlankLine)
            {
                //TODO: review here, enable this feature or not
                //some char can't be a start char on blank line

                if (!InternalTextLayerController.CanCaretStopOnThisChar(c))
                {
                    return;
                }
                //

                //1. new 
                EditableRun t = new EditableTextRun(this.Root,
                    c,
                    this.CurrentSpanStyle);
                var owner = this.FlowLayer.OwnerRenderElement;
                CurrentLine.AddLast(t);
                SetCurrentTextRun(t);
            }
            else
            {
                EditableRun cRun = CurrentTextRun;
                if (cRun != null)
                {
                    if (cRun.IsInsertable)
                    {
                        cRun.InsertAfter(CurrentTextRunCharIndex, c);
                    }
                    else
                    {
                        AddTextSpan(new EditableTextRun(this.Root, c, this.CurrentSpanStyle));
                        return;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            SetCurrentCharStepRight();
        }
        public void AddTextSpan(EditableRun textRun)
        {
            if (CurrentLine.IsBlankLine)
            {
                CurrentLine.AddLast(textRun);
                SetCurrentTextRun(textRun);
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();

                SetCurrentCharIndex(CharIndex + textRun.CharacterCount);
            }
            else
            {
                if (CurrentTextRun != null)
                {
                    VisualPointInfo newPointInfo = CurrentLine.Split(GetCurrentPointInfo());
                    if (newPointInfo.IsOnTheBeginOfLine)
                    {
                        CurrentLine.AddBefore((EditableRun)newPointInfo.TextRun, textRun);
                    }
                    else
                    {
                        CurrentLine.AddAfter((EditableRun)newPointInfo.TextRun, textRun);
                    }
                    CurrentLine.TextLineReCalculateActualLineSize();
                    CurrentLine.RefreshInlineArrange();
                    EnsureCurrentTextRun(CharIndex + textRun.CharacterCount);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        public void ReplaceAllLineContent(EditableRun[] runs)
        {
            int charIndex = CharIndex;
            CurrentLine.Clear();
            int j = runs.Length;
            for (int i = 0; i < j; ++i)
            {
                CurrentLine.AddLast(runs[i]);
            }

            EnsureCurrentTextRun(charIndex);
        }

        public char DoBackspaceOneChar()
        {
            //simulate backspace keystroke
            return BackSpaceOneChar();
        }
        public char DoDeleteOneChar()
        {
            if (CharIndex < CurrentLine.CharCount)
            {
                //simulate backspace keystroke

                SetCurrentCharStepRight();
                return BackSpaceOneChar();
            }
            else
            {
                return '\0';
            }
        }

        public void SplitToNewLine()
        {

            EditableRun lineBreakRun = new EditableTextRun(this.Root, '\n', this.CurrentSpanStyle);
            EditableRun currentRun = CurrentTextRun;
            if (CurrentLine.IsBlankLine)
            {
                CurrentLine.AddLast(lineBreakRun);
            }
            else
            {
                if (CharIndex == -1)
                {
                    CurrentLine.AddFirst(lineBreakRun);
                    SetCurrentTextRun(null);
                }
                else
                {
                    EditableRun rightSplitedPart = EditableRun.InnerRemove(currentRun,
                        CurrentTextRunCharIndex + 1, true);
                    if (rightSplitedPart != null)
                    {
                        CurrentLine.AddAfter(currentRun, rightSplitedPart);
                    }
                    CurrentLine.AddAfter(currentRun, lineBreakRun);
                    if (currentRun.CharacterCount == 0)
                    {
                        CurrentLine.Remove(currentRun);
                    }
                }
            }


            this.TextLayer.TopDownReCalculateContentSize();
            EnsureCurrentTextRun();
        }
        public EditableVisualPointInfo[] SplitSelectedText(VisualSelectionRange selectionRange)
        {
            EditableVisualPointInfo[] newPoints = CurrentLine.Split(selectionRange);
            EnsureCurrentTextRun();
            return newPoints;
        }
    }

    abstract class TextLineReader
    {
#if DEBUG
        static int dbugTotalId;
        int dbug_MyId;
        public debugActivityRecorder dbugTextManRecorder;
#endif

        EditableTextFlowLayer _textFlowLayer;
        EditableTextLine _currentLine;
        int _currentLineY = 0;
        EditableRun _currentTextRun;

        int _caretXPos = 0;
        /// <summary>
        /// character offset of this run, start from start line, this value is reset for every current run
        /// </summary>
        int _rCharOffset = 0;
        /// <summary>
        /// pixel offset of this run, start from the begin of this line, this value is reset for every current run
        /// </summary>
        int _rPixelOffset = 0;
        public TextLineReader(EditableTextFlowLayer flowlayer)
        {

#if DEBUG
            this.dbug_MyId = dbugTotalId;
            dbugTotalId++;
#endif

            _textFlowLayer = flowlayer;
            flowlayer.Reflow += new EventHandler(flowlayer_Reflow);
            _currentLine = flowlayer.GetTextLine(0);
            if (_currentLine.FirstRun != null)
            {
                _currentTextRun = _currentLine.FirstRun;
            }
        }

#if DEBUG
        int _i_charIndex;
        int caret_char_index
        {
            get { return _i_charIndex; }
            set
            {
                _i_charIndex = value;
            }
        }
#else
         int caret_char_index;
#endif

        void flowlayer_Reflow(object sender, EventArgs e)
        {
            int prevCharIndex = caret_char_index;
            this.SetCurrentCharIndexToBegin();
            this.SetCurrentCharIndex(prevCharIndex);
        }
        //
        protected RootGraphic Root => _textFlowLayer.Root;
        //
        public EditableTextFlowLayer FlowLayer => _textFlowLayer;
        //
        protected EditableTextLine CurrentLine => _currentLine;
        //
        protected EditableRun CurrentTextRun => _currentTextRun;
        //
        protected void SetCurrentTextRun(EditableRun r)
        {
            _currentTextRun = r;
        }
        public void FindCurrentHitWord(out int startAt, out int len)
        {
            if (_currentTextRun == null)
            {
                startAt = 0;
                len = 0;
                return;
            }

            //
            //we read entire line 
            //and send to line parser to parse a word
            StringBuilder stbuilder = new StringBuilder();
            _currentLine.CopyLineContent(stbuilder);
            string lineContent = stbuilder.ToString();
            //find char at

            TextBufferSpan textBufferSpan = new TextBufferSpan(lineContent.ToCharArray());
            ILineSegmentList segmentList = this.Root.TextServices.BreakToLineSegments(ref textBufferSpan);
            if (segmentList != null)
            {
                int segcount = segmentList.Count;
                for (int i = 0; i < segcount; ++i)
                {
                    ILineSegment seg = segmentList[i];
                    if (seg.StartAt + seg.Length >= caret_char_index)
                    {
                        //stop at this segment
                        startAt = seg.StartAt;
                        len = seg.Length;
                        return;
                    }
                }
            }
            else
            {
                //TODO: review here
                //this is a bug!!!
            }
            //?
            startAt = 0;
            len = 0;

        }
        bool MoveToPreviousTextRun()
        {
#if DEBUG
            if (_currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif
            if (_currentTextRun.PrevTextRun != null)
            {
                _currentTextRun = _currentTextRun.PrevTextRun;
                _rCharOffset -= _currentTextRun.CharacterCount;
                _rPixelOffset -= _currentTextRun.Width;
                caret_char_index = _rCharOffset + _currentTextRun.CharacterCount;
                _caretXPos = _rPixelOffset + _currentTextRun.Width;
                return true;
            }
            return false;
        }

        bool MoveToNextTextRun()
        {
#if DEBUG
            if (_currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif


            EditableRun nextTextRun = _currentTextRun.NextTextRun;
            if (nextTextRun != null && !nextTextRun.IsLineBreak)
            {
                _rCharOffset += _currentTextRun.CharacterCount;
                _rPixelOffset += _currentTextRun.Width;
                _currentTextRun = nextTextRun;
                caret_char_index = _rCharOffset;
                _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(0);
                return true;
            }
            return false;
        }

        public void MoveToLine(int lineNumber)
        {
            _currentLine = _textFlowLayer.GetTextLine(lineNumber);
            _currentLineY = _currentLine.Top;

            //if current line is a blank line
            //not first run => currentTextRun= null 
            _currentTextRun = (EditableRun)_currentLine.FirstRun;

            _rCharOffset = 0;
            _rPixelOffset = 0;

            caret_char_index = 0;
            _caretXPos = 0;
        }
        public void CopyContentToStrignBuilder(StringBuilder stBuilder)
        {
            _textFlowLayer.CopyContentToStringBuilder(stBuilder);
        }
        public char PrevChar
        {
            get
            {
                if (_currentTextRun != null)
                {

                    if (caret_char_index == 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (caret_char_index == _rCharOffset)
                    {
                        if (_currentTextRun.PrevTextRun != null)
                        {
                            return (_currentTextRun.PrevTextRun).GetChar(_currentTextRun.PrevTextRun.CharacterCount - 1);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return _currentTextRun.GetChar(caret_char_index - _rCharOffset);
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        public char NextChar
        {
            get
            {
                if (_currentTextRun != null)
                {

                    if (caret_char_index == 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (caret_char_index == _rCharOffset + _currentTextRun.CharacterCount)
                    {
                        if (_currentTextRun.NextTextRun != null)
                        {
                            return (_currentTextRun.NextTextRun).GetChar(0);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return _currentTextRun.GetChar(caret_char_index - _rCharOffset);
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        /// <summary>
        /// next char width in this line
        /// </summary>
        public int NextCharWidth
        {
            get
            {
                if (_currentTextRun != null)
                {
                    if (CharCount == 0)
                    {
                        //no text in this line
                        return 0;
                    }
                    if (caret_char_index == _rCharOffset + _currentTextRun.CharacterCount)
                    {
                        //-----
                        //this is on the last of current run
                        //if we have next run, just get run of next width
                        //-----
                        EditableRun nextRun = _currentTextRun.NextTextRun;
                        if (nextRun != null)
                        {
                            return nextRun.GetRunWidth(0);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        //in some line

                        return _currentTextRun.GetRunWidth(caret_char_index - _rCharOffset + 1) -
                                    _currentTextRun.GetRunWidth(caret_char_index - _rCharOffset);
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
#if DEBUG
            if (_currentTextRun != null && !_currentTextRun.HasParent)
            {
                throw new NotSupportedException();
            }
            if (_currentTextRun == null)
            {
            }
#endif      
            EditableVisualPointInfo textPointInfo =
                new EditableVisualPointInfo(_currentLine, caret_char_index);
            textPointInfo.SetAdditionVisualInfo(_currentTextRun,
                _rCharOffset, _caretXPos, _rPixelOffset);
            return textPointInfo;
        }
        public Point CaretPosition => new Point(_caretXPos, _currentLineY);

        public char CurrentChar
        {
            get
            {
                //1. blank line
                if (_currentTextRun == null)
                {
                    return '\0';
                }
                else
                {
                    //2.  
                    if (_currentTextRun.CharacterCount == caret_char_index - _rCharOffset)
                    {
                        //end of this run
                        return '\0';
                    }

                    return _currentTextRun.GetChar(caret_char_index - _rCharOffset);
                }
            }
        }
        //
        public int CurrentTextRunCharOffset => _rCharOffset;
        //
        public int CurrentTextRunPixelOffset => _rPixelOffset;
        //
        public int CurrentTextRunCharIndex => caret_char_index - _rCharOffset - 1;
        //
        /// <summary>
        /// try set caret x pos to nearest request value
        /// </summary>
        /// <param name="xpos"></param>
        public void TrySetCaretPos(int xpos, int ypos)
        {

            //--------
            _textFlowLayer.NotifyHitOnSolidTextRun(null);
            //--------
            if (_currentTextRun == null)
            {
                caret_char_index = 0;
                _caretXPos = 0;
                _rCharOffset = 0;
                _rPixelOffset = 0;
                return;
            }
            int pixDiff = xpos - _caretXPos;
            if (pixDiff > 0)
            {
                do
                {
                    int thisTextRunPixelLength = _currentTextRun.Width;
                    if (_rPixelOffset + thisTextRunPixelLength > xpos)
                    {
                        EditableRunCharLocation foundLocation = EditableRun.InnerGetCharacterFromPixelOffset(_currentTextRun, xpos - _rPixelOffset);
                        _caretXPos = _rPixelOffset + foundLocation.pixelOffset;
                        caret_char_index = _rCharOffset + foundLocation.RunCharIndex;

                        //for solid text run
                        //we can send some event to it
                        SolidTextRun solidTextRun = _currentTextRun as SolidTextRun;
                        if (solidTextRun != null)
                        {
                            _textFlowLayer.NotifyHitOnSolidTextRun(solidTextRun);
                        }

                        //if (foundLocation.charIndex == -1)
                        //{
                        //    if (!(MoveToPreviousTextRun()))
                        //    {
                        //        caretXPos = 0;
                        //        caret_char_index = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    caretXPos = rPixelOffset + foundLocation.pixelOffset;
                        //    caret_char_index = rCharOffset + foundLocation.charIndex;
                        //}
                        return;
                    }
                } while (MoveToNextTextRun());
                //to the last
                _caretXPos = _rPixelOffset + _currentTextRun.Width;
                caret_char_index = _rCharOffset + _currentTextRun.CharacterCount;
                return;
            }
            else if (pixDiff < 0)
            {
                do
                {
                    if (xpos >= _rPixelOffset)
                    {
                        EditableRunCharLocation foundLocation = EditableRun.InnerGetCharacterFromPixelOffset(_currentTextRun, xpos - _rPixelOffset);
                        _caretXPos = _rPixelOffset + foundLocation.pixelOffset;
                        caret_char_index = _rCharOffset + foundLocation.RunCharIndex;


                        //for solid text run
                        //we can send some event to it
                        SolidTextRun solidTextRun = _currentTextRun as SolidTextRun;
                        if (solidTextRun != null)
                        {
                            _textFlowLayer.NotifyHitOnSolidTextRun(solidTextRun);
                        }


                        //if (foundLocation.charIndex == -1)
                        //{
                        //    if (!MoveToPreviousTextRun())
                        //    {
                        //        caret_char_index = 0;
                        //        caretXPos = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    caretXPos = rPixelOffset + foundLocation.pixelOffset;
                        //    caret_char_index = rCharOffset + foundLocation.RunCharIndex;
                        //}
                        return;
                    }
                } while (MoveToPreviousTextRun());//
                _caretXPos = 0;
                caret_char_index = 0;
                return;
            }
        }
        //
        public int CaretXPos => _caretXPos;
        //
        public int CharIndex => caret_char_index;
        //
        int InternalCharIndex => caret_char_index;
        //
        public void SetCurrentCharStepRight()
        {
            SetCurrentCharIndex(InternalCharIndex + 1);
        }
        public void SetCurrentCharStepLeft()
        {
            SetCurrentCharIndex(InternalCharIndex - 1);
        }
        public void SetCurrentCharIndexToEnd()
        {
            SetCurrentCharIndex(this.CharCount);
        }
        public void SetCurrentCharIndexToBegin()
        {
            SetCurrentCharIndex(0);
        }
        public void SetCurrentCharIndex(int newCharIndexPointTo)
        {

#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.WriteInfo("TextLineReader::CharIndex_set=" + newCharIndexPointTo);
                dbugTextManRecorder.BeginContext();
            }
#endif
            if (newCharIndexPointTo < 0 || newCharIndexPointTo > _currentLine.CharCount)
            {
                throw new NotSupportedException("index out of range");
            }


            if (newCharIndexPointTo == 0)
            {
                caret_char_index = 0;
                _caretXPos = 0;
                _rCharOffset = 0;
                _rPixelOffset = 0;
                _currentTextRun = _currentLine.FirstRun;
            }
            else
            {
                int diff = newCharIndexPointTo - caret_char_index;
                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }

                    default:
                        {
                            if (diff > 0)
                            {
                                do
                                {
                                    if (_rCharOffset + _currentTextRun.CharacterCount >= newCharIndexPointTo)
                                    {
                                        caret_char_index = newCharIndexPointTo;
                                        _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(caret_char_index - _rCharOffset);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif

                                        return;
                                    }
                                } while (MoveToNextTextRun());
                                caret_char_index = _rCharOffset + _currentTextRun.CharacterCount;
                                _caretXPos = _rPixelOffset + _currentTextRun.Width;
                                return;
                            }
                            else
                            {
                                do
                                {
                                    if (_rCharOffset - 1 < newCharIndexPointTo)
                                    {
                                        caret_char_index = newCharIndexPointTo;
                                        _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(caret_char_index - _rCharOffset);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif
                                        return;
                                    }
                                } while (MoveToPreviousTextRun());
                                caret_char_index = 0;
                                _caretXPos = 0;
                            }
                        }
                        break;
                }
            }
#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.EndContext();
            }
#endif

        }

        //
        public bool IsOnEndOfLine => caret_char_index == _currentLine.CharCount;
        //
        internal EditableTextLine GetTextLine(int lineId)
        {
            return TextLayer.GetTextLine(lineId);
        }
        //
        internal EditableTextLine GetTextLineAtPos(int y)
        {
            return this.TextLayer.GetTextLineAtPos(y);
        }
        //
        public int LineCount => TextLayer.LineCount;
        //
        public bool HasNextLine => _currentLine.Next != null;
        //
        public bool HasPrevLine => _currentLine.Prev != null;
        //
        public bool IsOnStartOfLine => InternalCharIndex == 0;
        //
        public int CharCount => _currentLine.CharCount;
        //
        public void CopyLineContent(StringBuilder stBuilder)
        {
            _currentLine.CopyLineContent(stBuilder);
        }
        //
        public void CopySelectedTextRuns(VisualSelectionRange selectionRange, List<EditableRun> output)
        {
            _currentLine.Copy(selectionRange, output);
        }
        //
        public int LineNumber => _currentLine.LineNumber;
        //
        public void MoveToNextLine()
        {
            MoveToLine(_currentLine.LineNumber + 1);
        }
        public void MoveToPrevLine()
        {
            MoveToLine(_currentLine.LineNumber - 1);
        }
        //
        public Rectangle LineArea => _currentLine.ActualLineArea;
        //
        public Rectangle ParentLineArea => _currentLine.ParentLineArea;
        //
        internal EditableTextFlowLayer TextLayer => _currentLine.OwnerFlowLayer;
    }
}