//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using Typography.Text;
using Typography.TextLayout;

namespace LayoutFarm.TextFlow
{
    public readonly struct SelectionRangeInfo
    {
        public readonly EditableVisualPointInfo start;
        public readonly EditableVisualPointInfo end;
        public SelectionRangeInfo(EditableVisualPointInfo start, EditableVisualPointInfo end)
        {
            this.start = start;
            this.end = end;
        }
    }


    /// <summary>
    /// visual line walker
    /// </summary>
    sealed class VisualTextLineWalker
    {

#if DEBUG
        static int dbugTotalId;
        int dbug_MyId;
        public debugActivityRecorder dbugTextManRecorder;
#endif


        TextLineBox _currentLine;

        //Run _run1_x;
        Run _currentTextRun;
        //{
        //    get => _run1_x;
        //    set
        //    {
        //        _run1_x = value;
        //    }

        //}

        //int _caretXPos_1;
        int _caretXPos;
        //{
        //    get => _caretXPos_1;
        //    set
        //    {
        //        if (value != 0)
        //        {

        //        }
        //        _caretXPos_1 = value;
        //    }
        //}
        /// <summary>
        /// character offset of this run, start from start line, this value is reset for every current run
        /// </summary>
        int _rCharOffset = 0;
        /// <summary>
        /// pixel offset of this run, start from the begin of this line, this value is reset for every current run
        /// </summary>
        int _rPixelOffset = 0;
        public VisualTextLineWalker()
        {

#if DEBUG
            this.dbug_MyId = dbugTotalId;
            dbugTotalId++;
#endif  
        }

        public Run GetCurrentTextRun() => CurrentLine.IsBlankLine ? null : CurrentTextRun;
        public void CollectAffectedRuns(int charIndex, int count, System.Collections.Generic.List<Run> outputs)
        {
            //find affected run
            System.Collections.Generic.LinkedListNode<Run> node = _currentLine.First;

            int total = 0;
            bool foundBegin = false;
            int end = charIndex + count;

            while (node != null)
            {
                Run v = node.Value;
                if (!foundBegin)
                {
                    if (charIndex >= total && charIndex < total + v.CharacterCount)
                    {
                        foundBegin = true;
                        outputs.Add(v);
                        if (end <= total + v.CharacterCount)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    //find end
                    outputs.Add(v);
                    if (end <= total + v.CharacterCount)
                    {
                        return;
                    }
                }

                total += v.CharacterCount;//next                
                node = node.Next;
            }
        }

#if DEBUG
        int _i_charIndex;
        int _caret_NewCharIndex
        {
            get => _i_charIndex;
            set
            {
                if ((value % 2) != 0)
                {

                }
                _i_charIndex = value;
            }
        }
#else
         int _caret_NewCharIndex;
#endif
        public void EnsureCurrentTextRun(int index)
        {
            Run run = CurrentTextRun;
            if (run != null && run.HasParent)
            {
                SetCurrentCharIndex(index);
            }
            else
            {
                if (_needUpdateCurrentRun)
                {
                    SetCurrentCharIndex2(index);
                    _needUpdateCurrentRun = false;
                }
                else
                {
                    SetCurrentCharIndexToBegin();
                    if (index != -1)
                    {
                        int limit = CurrentLine.CharCount();
                        if (index > limit)
                        {
                            index = limit;
                        }
                        SetCurrentCharIndex(index);
                    }
                }

            }
        }

        public void EnsureCurrentTextRun() => EnsureCurrentTextRun(NewCharIndex);


        public TextLineBox CurrentLine => _currentLine;
        //
        Run CurrentTextRun => _currentTextRun;

        bool _needUpdateCurrentRun;

        public void InvalidateCurrentRun()
        {
            _currentTextRun = null;
            _needUpdateCurrentRun = true;
        }


        readonly LayoutWordVisitor _wordVisitor = new LayoutWordVisitor();
        readonly LineSegmentList<LineSegment> _lineSegs = new LineSegmentList<LineSegment>();

        readonly TextBuilder<int> _utf32ArrList = new TextBuilder<int>();

        public void FindCurrentHitWord(out int startAt, out int len)
        {
            if (_currentTextRun == null)
            {
                startAt = 0;
                len = 0;
                return;
            }

            using (new TextUtf32RangeCopyPoolContext<VisualTextLineWalker>(out TextCopyBufferUtf32 output))
            {
                //copy to int32 arr list

                _currentLine.CopyLineContent(output);

                _lineSegs.Clear();
                _wordVisitor.SetLineSegmentList(_lineSegs);

                int content_len = output.Length;

                _utf32ArrList.Clear(content_len);

                output.CopyTo(_utf32ArrList);

                Typography.Text.GlobalTextService.TxtClient.BreakToLineSegments(
                    new Typography.Text.TextBufferSpan(TextBuilder<int>.UnsafeGetInternalArray(_utf32ArrList),
                    0, _utf32ArrList.Count),
                    _wordVisitor);

                if (_lineSegs.Count == 0)
                {
                    startAt = 0;
                    len = 0;
                    return;
                }

                int segcount = _lineSegs.Count;
                for (int i = 0; i < segcount; ++i)
                {
                    LineSegment seg = _lineSegs.GetLineSegment(i);
                    if (seg.StartAt + seg.Length >= _caret_NewCharIndex)
                    {
                        //stop at this segment
                        startAt = seg.StartAt;
                        len = seg.Length;
                        return;
                    }
                }

            }


            //?
            startAt = 0;
            len = 0;
        }

        bool MoveToPreviousTextRun()
        {
            //#if DEBUG
            //            if (_currentTextRun.IsLineBreak)
            //            {
            //                throw new NotSupportedException();
            //            }
            //#endif
            if (_currentTextRun.PrevRun != null)
            {
                _currentTextRun = _currentTextRun.PrevRun;
                _rCharOffset -= _currentTextRun.CharacterCount;
                _rPixelOffset -= _currentTextRun.Width;
                _caret_NewCharIndex = _rCharOffset + _currentTextRun.CharacterCount;
                _caretXPos = _rPixelOffset + _currentTextRun.Width;
                return true;
            }
            return false;
        }

        bool MoveToNextTextRun()
        {
            //#if DEBUG
            //            if (_currentTextRun.IsLineBreak)
            //            {
            //                throw new NotSupportedException();
            //            }
            //#endif


            Run nextTextRun = _currentTextRun.NextRun;
            if (nextTextRun != null)// && !nextTextRun.IsLineBreak)
            {
                _rCharOffset += _currentTextRun.CharacterCount;
                _rPixelOffset += _currentTextRun.Width;
                _currentTextRun = nextTextRun;
                _caret_NewCharIndex = _rCharOffset;
                _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(0);
                return true;
            }
            return false;
        }
        public Point CaretPos => new Point(_caretXPos, _currentLine.LineTop);
        public void LoadLine(TextLineBox linebox)
        {
            _rCharOffset = 0;
            _rPixelOffset = 0;

            _caret_NewCharIndex = 0;
            _caretXPos = 0;

            //move to specific line box
            if (linebox != null)
            {
                _currentLine = linebox;
                //if current line is a blank line
                //not first run => currentTextRun= null 
                _currentTextRun = linebox.FirstRun;
            }
            else
            {
                _currentLine = null;

                _currentTextRun = null;
            }

        }

        public int PrevChar
        {
            get
            {
                if (_currentTextRun != null)
                {

                    if (_caret_NewCharIndex == 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (_caret_NewCharIndex == _rCharOffset)
                    {
                        if (_currentTextRun.PrevRun != null)
                        {
                            return (_currentTextRun.PrevRun).GetChar(_currentTextRun.PrevRun.CharacterCount - 1);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return _currentTextRun.GetChar(_caret_NewCharIndex - _rCharOffset);
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        public int NextChar
        {
            get
            {
                if (_currentTextRun != null)
                {

                    if (_caret_NewCharIndex == 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (_caret_NewCharIndex == _rCharOffset + _currentTextRun.CharacterCount)
                    {
                        if (_currentTextRun.NextRun != null)
                        {
                            return (_currentTextRun.NextRun).GetChar(0);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return _currentTextRun.GetChar(_caret_NewCharIndex - _rCharOffset);
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
                    if (_caret_NewCharIndex == _rCharOffset + _currentTextRun.CharacterCount)
                    {
                        //-----
                        //this is on the last of current run
                        //if we have next run, just get run of next width
                        //-----
                        Run nextRun = _currentTextRun.NextRun;
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

                        return _currentTextRun.GetRunWidth(_caret_NewCharIndex - _rCharOffset + 1) -
                                    _currentTextRun.GetRunWidth(_caret_NewCharIndex - _rCharOffset);
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
            //if (_currentTextRun != null && !_currentTextRun.HasParent)
            //{
            //    throw new NotSupportedException();
            //}
            //if (_currentTextRun == null)
            //{
            //}
#endif
            EditableVisualPointInfo textPointInfo =
                new EditableVisualPointInfo(_currentLine, _caret_NewCharIndex);
            textPointInfo.SetAdditionVisualInfo(
                _rCharOffset, _caretXPos, _rPixelOffset);
            return textPointInfo;
        }


        public int CurrentChar
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
                    if (_currentTextRun.CharacterCount == _caret_NewCharIndex - _rCharOffset)
                    {
                        //end of this run
                        return '\0';
                    }

                    return _currentTextRun.GetChar(_caret_NewCharIndex - _rCharOffset);
                }
            }
        }
        //
        public int CurrentTextRunCharOffset => _rCharOffset;
        //
        public int CurrentTextRunPixelOffset => _rPixelOffset;
        //

        //
        /// <summary>
        /// try set caret x pos to nearest request value
        /// </summary>
        /// <param name="xpos"></param>
        public void TrySetCaretPos(int xpos)
        {

            //--------

            //_textFlowLayer.LatestHitRun = null;
            //--------
            if (_currentTextRun == null)
            {
                _caret_NewCharIndex = 0;
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
                        CharLocation foundLocation = Run.InnerGetCharacterFromPixelOffset(_currentTextRun, xpos - _rPixelOffset);
#if DEBUG
                        if ((foundLocation.RunCharIndex % 2) != 0)
                        {

                        }
#endif
                        _caretXPos = _rPixelOffset + foundLocation.pixelOffset;
                        _caret_NewCharIndex = _rCharOffset + foundLocation.RunCharIndex;

                        //for solid text run
                        //we can send some event to it
                        //_textFlowLayer.LatestHitRun = _currentTextRun;
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
                _caret_NewCharIndex = _rCharOffset + _currentTextRun.CharacterCount;
                return;
            }
            else if (pixDiff < 0)
            {
                do
                {
                    if (xpos >= _rPixelOffset)
                    {
                        CharLocation foundLocation = Run.InnerGetCharacterFromPixelOffset(_currentTextRun, xpos - _rPixelOffset);
                        _caretXPos = _rPixelOffset + foundLocation.pixelOffset;
                        _caret_NewCharIndex = _rCharOffset + foundLocation.RunCharIndex;


                        //for solid text run
                        //we can send some event to it

                        //_textFlowLayer.LatestHitRun = _currentTextRun;
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
                _caret_NewCharIndex = 0;
                return;
            }
        }
        //
        public int CaretXPos => _caretXPos;
        //
        public int NewCharIndex => _caret_NewCharIndex;

        public void SetCurrentCharIndexToEnd() => SetCurrentCharIndex(this.CharCount);

        public void SetCurrentCharIndexToBegin() => SetCurrentCharIndex(0);
        public void SetCurrentTextRunToLast()
        {
            _currentTextRun = _currentLine.LastRun;
        }
        public void SetCurrentCharIndex2(int newCharIndexPointTo)
        {

#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.WriteInfo("TextLineReader::CharIndex_set=" + newCharIndexPointTo);
                dbugTextManRecorder.BeginContext();
            }
#endif
            if (newCharIndexPointTo < 0 || newCharIndexPointTo > _currentLine.CharCount())
            {
                throw new NotSupportedException("index out of range");
            }

            _caret_NewCharIndex = 0;
            _caretXPos = 0;
            _rCharOffset = 0;
            _rPixelOffset = 0;
            _currentTextRun = _currentLine.FirstRun;


            if (newCharIndexPointTo > 0)
            {
                //always search from begin           

                do
                {
                    if (_rCharOffset + _currentTextRun.CharacterCount >= newCharIndexPointTo)
                    {
                        _caret_NewCharIndex = newCharIndexPointTo;
                        _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(_caret_NewCharIndex - _rCharOffset);
#if DEBUG
                        if (dbugTextManRecorder != null)
                        {
                            dbugTextManRecorder.EndContext();
                        }
#endif

                        return;
                    }
                    //
                } while (MoveToNextTextRun());
                _caret_NewCharIndex = _rCharOffset + _currentTextRun.CharacterCount;
                _caretXPos = _rPixelOffset + _currentTextRun.Width;
                return;
            }
#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.EndContext();
            }
#endif

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
            if (newCharIndexPointTo < 0 || newCharIndexPointTo > _currentLine.CharCount())
            {
                System.Diagnostics.Debugger.Break();
                return;
                // throw new NotSupportedException("index out of range");
            }


            if (newCharIndexPointTo == 0)
            {
                _caret_NewCharIndex = 0;
                _caretXPos = 0;
                _rCharOffset = 0;
                _rPixelOffset = 0;
                _currentTextRun = _currentLine.FirstRun;
            }
            else
            {
                int diff = newCharIndexPointTo - _caret_NewCharIndex;
                switch (diff)
                {
                    case 0:
                        return;
                    default:
                        {
                            if (diff > 0)
                            {
                                do
                                {
                                    if (_rCharOffset + _currentTextRun.CharacterCount >= newCharIndexPointTo)
                                    {
                                        _caret_NewCharIndex = newCharIndexPointTo;
                                        _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(_caret_NewCharIndex - _rCharOffset);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif

                                        return;
                                    }
                                    //
                                } while (MoveToNextTextRun());
                                _caret_NewCharIndex = _rCharOffset + _currentTextRun.CharacterCount;
                                _caretXPos = _rPixelOffset + _currentTextRun.Width;
                                return;
                            }
                            else
                            {
                                do
                                {
                                    if (_rCharOffset - 1 < newCharIndexPointTo)
                                    {
                                        _caret_NewCharIndex = newCharIndexPointTo;
                                        _caretXPos = _rPixelOffset + _currentTextRun.GetRunWidth(_caret_NewCharIndex - _rCharOffset);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif
                                        return;
                                    }
                                } while (MoveToPreviousTextRun());
                                _caret_NewCharIndex = 0;
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
        public bool IsOnEndOfLine => _caret_NewCharIndex == _currentLine.CharCount();

        public bool HasNextLine => _currentLine.Next != null;

        public bool HasPrevLine => _currentLine.Prev != null;

        public bool IsOnStartOfLine => _caret_NewCharIndex == 0;

        public int CharCount => _currentLine.CharCount();

        public int LineNumber => _currentLine.LineNumber;

        public Rectangle LineArea => _currentLine.ActualLineArea;
    }
}