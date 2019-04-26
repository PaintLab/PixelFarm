//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.TextEditing.Commands;

namespace LayoutFarm.TextEditing
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

        internal InternalTextLayerController(EditableTextFlowLayer textLayer)
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
        //
        internal List<VisualMarkerSelectionRange> VisualMarkers => _visualMarkers;
        //
        internal int VisualMarkerCount => _visualMarkers.Count;
        internal DocumentCommandListener DocCmdListener
        {
            get => _commandHistoryList.Listener;
            set => _commandHistoryList.Listener = value;
        }

        //
        public bool EnableUndoHistoryRecording
        {
            get => _enableUndoHistoryRecording;
            set => _enableUndoHistoryRecording = value;
        }
        // 
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
                VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
                passRemoveSelectedText = true;
            }

            if (passRemoveSelectedText && c == ' ')
            {
            }
            else
            {
                if (!_textLineWriter.CanAcceptThisChar(c))
                {
                    return;
                }
                //
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

        public EditableRun CurrentTextRun => _textLineWriter.GetCurrentTextRun();

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
            //NotifyContentSizeChanged();
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
            if (selRange == null) return;
            //

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

        public void DoTabOverSelectedRange()
        {
            //eg. user press 'Tab' key over selected range
            VisualSelectionRange selRange = SelectionRange;
            if (selRange == null) return;
            //

            EditableVisualPointInfo startPoint = selRange.StartPoint;
            EditableVisualPointInfo endPoint = selRange.EndPoint;
            //
            if (!selRange.IsOnTheSameLine)
            {
                EditableTextLine line = startPoint.Line;
                EditableTextLine end_line = endPoint.Line;

                while (line.LineNumber <= end_line.LineNumber)
                {
                    var whitespace = new EditableTextRun(_textLineWriter.RootGfx, "    ", _textLineWriter.CurrentSpanStyle);

                    line.AddFirst(whitespace);
                    line.TextLineReCalculateActualLineSize();
                    line.RefreshInlineArrange();


                    line = line.Next;//move to next line
                }

                return;//finish here
            }

        }
        public void SplitCurrentLineIntoNewLine()
        {
            VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
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
                _updateJustCurrentLine = _selectionRange.IsOnTheSameLine;
                CancelSelect();
                //?
                //CharIndex++;
                //CharIndex--;
            }
        }
        public void DoFormatSelection(TextSpanStyle textStyle, FontStyle toggleFontStyle)
        {
            //int startLineNum = _textLineWriter.LineNumber;
            //int startCharIndex = _textLineWriter.CharIndex;
            SplitSelectedText();
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                foreach (EditableRun r in selRange.GetPrintableTextRunIter())
                {
                    TextSpanStyle existingStyle = r.SpanStyle;
                    switch (toggleFontStyle)
                    {
                        case FontStyle.Bold:
                            if ((existingStyle.ReqFont.Style & FontStyle.Bold) != 0)
                            {
                                //change to normal
                                RequestFont existingFont = existingStyle.ReqFont;
                                RequestFont newReqFont = new RequestFont(
                                    existingFont.Name, existingFont.SizeInPoints,
                                    existingStyle.ReqFont.Style & ~FontStyle.Bold); //clear bold

                                TextSpanStyle textStyle2 = new TextSpanStyle();
                                textStyle2.ReqFont = newReqFont;
                                textStyle2.ContentHAlign = textStyle.ContentHAlign;
                                textStyle2.FontColor = textStyle.FontColor;
                                r.SetStyle(textStyle2);
                                continue;//go next***
                            }
                            break;
                        case FontStyle.Italic:
                            if ((existingStyle.ReqFont.Style & FontStyle.Italic) != 0)
                            {
                                //change to normal
                                RequestFont existingFont = existingStyle.ReqFont;
                                RequestFont newReqFont = new RequestFont(
                                    existingFont.Name, existingFont.SizeInPoints,
                                    existingStyle.ReqFont.Style & ~FontStyle.Italic); //clear italic

                                TextSpanStyle textStyle2 = new TextSpanStyle();
                                textStyle2.ReqFont = newReqFont;
                                textStyle2.ContentHAlign = textStyle.ContentHAlign;
                                textStyle2.FontColor = textStyle.FontColor;
                                r.SetStyle(textStyle2);
                                continue;//go next***
                            }
                            break;
                    }
                    r.SetStyle(textStyle);
                }
                _updateJustCurrentLine = _selectionRange.IsOnTheSameLine;
                CancelSelect();
                //?
                //CharIndex++;
                //CharIndex--;
            }
        }

        public void AddMarkerSpan(VisualMarkerSelectionRange markerRange)
        {
            markerRange.BindToTextLayer(_textLayer);
            _visualMarkers.Add(markerRange);
        }

        /// <summary>
        /// clear all marker
        /// </summary>
        public void ClearMarkers() => _visualMarkers.Clear();


        public void RemoveMarkers(VisualMarkerSelectionRange marker)
        {
            _visualMarkers.Remove(marker);
        }
        //
        public int CurrentLineCharCount => _textLineWriter.CharCount;
        //
        public int LineCount => _textLineWriter.LineCount;
        //
        public int CurrentLineCharIndex => _textLineWriter.CharIndex;
        //
        public int CurrentTextRunCharIndex => _textLineWriter.CurrentTextRunCharIndex;
        //
        public int CurrentLineNumber
        {
            get => _textLineWriter.LineNumber;
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

        public VisualSelectionRange SelectionRange => _selectionRange;

        public void UpdateSelectionRange()
        {
            _selectionRange?.UpdateSelectionRange();
        }

        public EditableVisualPointInfo GetCurrentPointInfo() => _textLineWriter.GetCurrentPointInfo();

        /// <summary>
        /// find underlying word at current caret pos
        /// </summary>
        public void FindUnderlyingWord(out int startAt, out int len)
        {
            _textLineWriter.FindCurrentHitWord(out startAt, out len);
        }

        public void TryMoveCaretTo(int charIndex, bool backward = false)
        {
            if (_textLineWriter.CharIndex < 1 && charIndex < 0)
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
                if (_textLineWriter.CharIndex >= lineLength && charIndex > lineLength)
                {
                    if (_textLineWriter.HasNextLine)
                    {
                        _textLineWriter.MoveToNextLine();
                    }
                }
                else
                {
                    _textLineWriter.SetCurrentCharIndex(charIndex);
                    //check if we can stop at this char or not
                    if (backward)
                    {
                        //move caret backward
                        char prevChar = _textLineWriter.PrevChar;
                        int tmp_index = charIndex;
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
                        int tmp_index = charIndex + 1;
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
        //
        public int CharIndex => _textLineWriter.CharIndex;
        //
        public bool IsOnEndOfLine => _textLineWriter.IsOnEndOfLine;
        public bool IsOnStartOfLine => _textLineWriter.IsOnStartOfLine;
        public int CurrentCaretHeight
        {
            get
            {
                EditableRun currentRun = this.CurrentTextRun;
                return (currentRun != null) ? currentRun.Height : 17; //TODO: review this...
            }
        }
        public Point CaretPos
        {
            get => _textLineWriter.CaretPosition;
            set
            {
                if (_textLineWriter.LineCount > 0)
                {
                    EditableTextLine line = _textLineWriter.GetTextLineAtPos(value.Y);
                    int calculatedLineId = 0;
                    int lineTop = 0;
                    if (line != null)
                    {
                        calculatedLineId = line.LineNumber;
                        lineTop = line.Top;
                    }
                    this.CurrentLineNumber = calculatedLineId;
                    _textLineWriter.TrySetCaretPos(value.X, value.Y - lineTop);
                }
            }
        }
        //
        public int GetNextCharacterWidth() => _textLineWriter.NextCharWidth;
        //
        public void SetCaretPos(int x, int y)
        {
            if (_textLineWriter.LineCount > 0)
            {
                EditableTextLine line = _textLineWriter.GetTextLineAtPos(y);
                int lineNo = 0;
                int lineTop = 0;
                if (line != null)
                {
                    lineNo = line.LineNumber;
                    lineTop = line.Top;
                }

                this.CurrentLineNumber = lineNo;
                _textLineWriter.TrySetCaretPos(x, y - lineTop);
            }
        }
        public Rectangle CurrentLineArea => _textLineWriter.LineArea;
        public Rectangle CurrentParentLineArea => _textLineWriter.ParentLineArea;

        public bool IsOnFirstLine => !_textLineWriter.HasPrevLine;

        void JoinWithNextLine()
        {
            _textLineWriter.JoinWithNextLine();
            //
            NotifyContentSizeChanged();
        }
        public void UndoLastAction() => _commandHistoryList.UndoLastAction();

        public void ReverseLastUndoAction() => _commandHistoryList.ReverseLastUndoAction();
    }
}