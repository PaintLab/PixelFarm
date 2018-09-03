//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{

    partial class InternalTextLayerController
    {
        VisualSelectionRange _selectionRange;
        internal bool _updateJustCurrentLine = true;
        bool _enableUndoHistoryRecording = true;
        DocumentCommandCollection _commandHistoryList;
        TextLineWriter _textLineWriter;
        List<VisualMarkerSelectionRange> _visualMarkers = new List<VisualMarkerSelectionRange>();
        EditableTextFlowLayer _textLayer;

#if DEBUG
        debugActivityRecorder _dbugActivityRecorder;
        internal bool dbugEnableTextManRecorder = false;
#endif

        public InternalTextLayerController(EditableTextFlowLayer textLayer)
        {
            //this controller control the editaible-textflow-layer
            _textLayer = textLayer;
            //write to textflow-layer with text-line-writer (handle the writing line)
            _textLineWriter = new TextLineWriter(textLayer);
            //and record editing hx, support undo-redo
            _commandHistoryList = new DocumentCommandCollection(this);

#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder = new debugActivityRecorder();
                _textLineWriter.dbugTextManRecorder = _dbugActivityRecorder;
                throw new NotSupportedException();
                _dbugActivityRecorder.Start(null);
            }
#endif
        }

        internal List<VisualMarkerSelectionRange> VisualMarkers
        {
            get { return _visualMarkers; }
        }

        internal int VisualMarkerCount
        {
            get
            {
                return _visualMarkers.Count;
            }
        }

        public bool EnableUndoHistoryRecording
        {
            get
            {
                return _enableUndoHistoryRecording;
            }
            set
            {
                _enableUndoHistoryRecording = value;
            }
        }


        public void AddCharToCurrentLine(char c)
        {
            _updateJustCurrentLine = true;
            bool passRemoveSelectedText = false;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::AddCharToCurrentLine " + c);
                _dbugActivityRecorder.BeginContext();
            }
#endif
            if (SelectionRange != null)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo(SelectionRange);
                }
#endif
                RemoveSelectedText();
                passRemoveSelectedText = true;
            }
            if (passRemoveSelectedText && c == ' ')
            {
            }
            else
            {
                _commandHistoryList.AddDocAction(
                  new DocActionCharTyping(c, _textLineWriter.LineNumber, _textLineWriter.CharIndex));
            }

            _textLineWriter.AddCharacter(c);
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
        }
        public EditableRun CurrentTextRun
        {
            get
            {
                return _textLineWriter.GetCurrentTextRun();
            }
        }

        VisualSelectionRangeSnapShot RemoveSelectedText()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::RemoveSelectedText");
                _dbugActivityRecorder.BeginContext();
            }
#endif

            if (_selectionRange == null)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo("NO_SEL_RANGE");
                    _dbugActivityRecorder.EndContext();
                }
#endif
                return VisualSelectionRangeSnapShot.Empty;
            }
            else if (!_selectionRange.IsValid)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo("!RANGE_ON_SAME_POINT");
                }
#endif
                CancelSelect();
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.EndContext();
                }
#endif
                return VisualSelectionRangeSnapShot.Empty;
            }
            _selectionRange.SwapIfUnOrder();
            VisualSelectionRangeSnapShot selSnapshot = _selectionRange.GetSelectionRangeSnapshot();
            VisualPointInfo startPoint = _selectionRange.StartPoint;
            CurrentLineNumber = startPoint.LineId;
            int preCutIndex = startPoint.LineCharIndex;
            _textLineWriter.SetCurrentCharIndex(startPoint.LineCharIndex);
            if (_selectionRange.IsOnTheSameLine)
            {
                List<EditableRun> tobeDeleteTextRuns = new List<EditableRun>();
                _textLineWriter.CopySelectedTextRuns(_selectionRange, tobeDeleteTextRuns);
                if (tobeDeleteTextRuns != null && tobeDeleteTextRuns.Count > 0)
                {

                    _commandHistoryList.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRuns,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));
                    _textLineWriter.RemoveSelectedTextRuns(_selectionRange);
                    _updateJustCurrentLine = true;
                }
            }
            else
            {
                int startPointLindId = startPoint.LineId;
                int startPointCharIndex = startPoint.LineCharIndex;
                List<EditableRun> tobeDeleteTextRuns = new List<EditableRun>();
                _textLineWriter.CopySelectedTextRuns(_selectionRange, tobeDeleteTextRuns);
                if (tobeDeleteTextRuns != null && tobeDeleteTextRuns.Count > 0)
                {
                    _commandHistoryList.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRuns,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));
                    _textLineWriter.RemoveSelectedTextRuns(_selectionRange);
                    _updateJustCurrentLine = false;
                    _textLineWriter.MoveToLine(startPointLindId);
                    _textLineWriter.SetCurrentCharIndex(startPointCharIndex);
                }
            }
            CancelSelect();
            NotifyContentSizeChanged();
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
            return selSnapshot;
        }
        void NotifyContentSizeChanged()
        {
            _textLayer.NotifyContentSizeChanged();
        }
        void SplitSelectedText()
        {
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                EditableVisualPointInfo[] newPoints = _textLineWriter.SplitSelectedText(selRange);
                if (newPoints != null)
                {
                    selRange.StartPoint = newPoints[0];
                    selRange.EndPoint = newPoints[1];
                    return;
                }
                else
                {
                    _selectionRange = null;
                }
            }
        }
        public void SplitCurrentLineIntoNewLine()
        {
            RemoveSelectedText();
            _commandHistoryList.AddDocAction(
                 new DocActionSplitToNewLine(_textLineWriter.LineNumber, _textLineWriter.CharIndex));
            _textLineWriter.SplitToNewLine();
            CurrentLineNumber++;
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public TextSpanStyle GetFirstTextStyleInSelectedRange()
        {
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                if (_selectionRange.StartPoint.TextRun != null)
                {
                    return _selectionRange.StartPoint.TextRun.SpanStyle;
                }
                else
                {
                    return TextSpanStyle.Empty;
                }
            }
            else
            {
                return TextSpanStyle.Empty;
            }
        }
        public void DoFormatSelection(TextSpanStyle textStyle)
        {
            //int startLineNum = _textLineWriter.LineNumber;
            //int startCharIndex = _textLineWriter.CharIndex;
            SplitSelectedText();
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                foreach (EditableRun r in selRange.GetPrintableTextRunIter())
                {
                    r.SetStyle(textStyle);
                }

                this._updateJustCurrentLine = _selectionRange.IsOnTheSameLine;
                CancelSelect();
                //?
                //CharIndex++;
                //CharIndex--;
            }
        }

        public void AddMarkerSpan(VisualSelectionRangeSnapShot selectoinRangeSnapshot)
        {


        }


        public int CurrentLineCharCount
        {
            get
            {
                return _textLineWriter.CharCount;
            }
        }

        public int LineCount
        {
            get
            {
                return _textLineWriter.LineCount;
            }
        }
        public int CurrentLineCharIndex
        {
            get
            {
                return _textLineWriter.CharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return _textLineWriter.CurrentTextRunCharIndex;
            }
        }
        public int CurrentLineNumber
        {
            get
            {
                return _textLineWriter.LineNumber;
            }
            set
            {
                int diff = value - _textLineWriter.LineNumber;
                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }
                    case 1:
                        {
                            if (_textLineWriter.HasNextLine)
                            {
                                _textLineWriter.MoveToNextLine();
                                DoHome();
                            }
                        }
                        break;
                    case -1:
                        {
                            if (_textLineWriter.HasPrevLine)
                            {
                                _textLineWriter.MoveToPrevLine();
                                DoEnd();
                            }
                        }
                        break;
                    default:
                        {
                            if (diff > 1)
                            {
                                _textLineWriter.MoveToLine(value);
                            }
                            else
                            {
                                if (value < -1)
                                {
                                    _textLineWriter.MoveToLine(value);
                                }
                                else
                                {
                                    _textLineWriter.MoveToLine(value);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public VisualSelectionRange SelectionRange
        {
            get
            {
                return _selectionRange;
            }
        }
        public void UpdateSelectionRange()
        {
            if (_selectionRange != null)
            {
                _selectionRange.UpdateSelectionRange();
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
            return _textLineWriter.GetCurrentPointInfo();
        }

        /// <summary>
        /// find underlying word at current caret pos
        /// </summary>
        public void FindUnderlyingWord(out int startAt, out int len)
        {
            _textLineWriter.FindCurrentHitWord(out startAt, out len);
        }

        public void TryMoveCaretTo(int value, bool backward = false)
        {
            if (_textLineWriter.CharIndex < 1 && value < 0)
            {
                if (backward)
                {
                    if (_textLineWriter.HasPrevLine)
                    {
                        _textLineWriter.MoveToPrevLine();
                        DoEnd();
                    }
                }
            }
            else
            {
                int lineLength = _textLineWriter.CharCount;
                if (_textLineWriter.CharIndex >= lineLength && value > lineLength)
                {
                    if (_textLineWriter.HasNextLine)
                    {
                        _textLineWriter.MoveToNextLine();
                    }
                }
                else
                {
                    _textLineWriter.SetCurrentCharIndex(value);
                    //check if we can stop at this char or not
                    if (backward)
                    {
                        //move caret backward
                        char prevChar = _textLineWriter.PrevChar;
                        int tmp_index = value;
                        while ((prevChar != '\0' && !CanCaretStopOnThisChar(prevChar)) && tmp_index > 0)
                        {
                            _textLineWriter.SetCurrentCharStepLeft();
                            prevChar = _textLineWriter.PrevChar;
                            tmp_index--;
                        }
                    }
                    else
                    {
                        char nextChar = _textLineWriter.NextChar;
                        int lineCharCount = _textLineWriter.CharCount;
                        int tmp_index = value + 1;
                        while ((nextChar != '\0' && !CanCaretStopOnThisChar(nextChar)) && tmp_index < lineCharCount)
                        {
                            _textLineWriter.SetCurrentCharStepRight();
                            nextChar = _textLineWriter.NextChar;
                            tmp_index++;
                        }
                    }

                }
            }
        }
        public void TryMoveCaretForward()
        {
            //move caret forward 1 key stroke
            TryMoveCaretTo(_textLineWriter.CharIndex + 1);
        }
        public void TryMoveCaretBackward()
        {
            TryMoveCaretTo(_textLineWriter.CharIndex - 1, true);
        }
        public int CharIndex
        {
            get
            {
                return _textLineWriter.CharIndex;
            }
        }
        public bool IsOnEndOfLine
        {
            get { return _textLineWriter.IsOnEndOfLine; }
        }
        public bool IsOnStartOfLine
        {
            get
            {
                return _textLineWriter.IsOnStartOfLine;
            }
        }
        public int CurrentCaretHeight
        {
            get
            {
                EditableRun currentRun = this.CurrentTextRun;
                return (currentRun != null) ? currentRun.Height : 14;
            }
        }
        public Point CaretPos
        {
            get
            {
                return this._textLineWriter.CaretPosition;
            }
            set
            {
                if (_textLineWriter.LineCount > 0)
                {
                    EditableTextLine line = _textLineWriter.GetTextLineAtPos(value.Y);
                    int calculatedLineId = 0;
                    if (line != null)
                    {
                        calculatedLineId = line.LineNumber;
                    }
                    this.CurrentLineNumber = calculatedLineId;
                    this._textLineWriter.TrySetCaretXPos(value.X);
                }
            }
        }
        public int GetNextCharacterWidth()
        {
            return _textLineWriter.NextCharWidth;
        }
        public void SetCaretPos(int x, int y)
        {
            int j = _textLineWriter.LineCount;
            if (j > 0)
            {
                EditableTextLine line = _textLineWriter.GetTextLineAtPos(y);
                int calculatedLineId = 0;
                if (line != null)
                {
                    calculatedLineId = line.LineNumber;
                }
                this.CurrentLineNumber = calculatedLineId;
                this._textLineWriter.TrySetCaretXPos(x);
            }
        }
        public Rectangle CurrentLineArea
        {
            get
            {
                return _textLineWriter.LineArea;
            }
        }
        public Rectangle CurrentParentLineArea
        {
            get
            {
                return _textLineWriter.ParentLineArea;
            }
        }
        public bool IsOnFirstLine
        {
            get
            {
                return !_textLineWriter.HasPrevLine;
            }
        }

        void JoinWithNextLine()
        {
            _textLineWriter.JoinWithNextLine();
            //
            NotifyContentSizeChanged();
        }
        public void UndoLastAction()
        {
            _commandHistoryList.UndoLastAction();
        }
        public void ReverseLastUndoAction()
        {
            _commandHistoryList.ReverseLastUndoAction();
        }
    }
}