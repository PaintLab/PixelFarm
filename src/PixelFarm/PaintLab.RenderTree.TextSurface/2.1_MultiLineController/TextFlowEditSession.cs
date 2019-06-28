//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.TextEditing.Commands;

namespace LayoutFarm.TextEditing
{

    public static class PlainTextDocumentHelper
    {
        public static PlainTextDocument CreatePlainTextDocument(string orgText)
        {
            PlainTextDocument doc = new PlainTextDocument();
            using (System.IO.StringReader reader = new System.IO.StringReader(orgText))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    //...
                    doc.AppendLine(line);
                    line = reader.ReadLine();
                }
            }
            return doc;
        }
        public static PlainTextDocument CreatePlainTextDocument(IEnumerable<string> lines)
        {
            PlainTextDocument doc = new PlainTextDocument();
            foreach (string line in lines)
            {
                doc.AppendLine(line);
            }
            return doc;
        }
    }

    public partial class TextFlowEditSession : ITextFlowEditSession
    {
        VisualSelectionRange _selectionRange;//primary visual selection
        internal bool _updateJustCurrentLine = true;
        bool _enableUndoHistoryRecording = true;

        TextFlowLayer _textLayer;
        TextLineEditor _lineEditor;

        DocumentCommandCollection _commandHistoryList;
        List<VisualMarkerSelectionRange> _visualMarkers;

#if DEBUG
        debugActivityRecorder _dbugActivityRecorder;
        internal bool dbugEnableTextManRecorder = false;
#endif

        internal TextFlowEditSession(TextFlowLayer textLayer)
        {
            //this controller control the editaible-textflow-layer
            _textLayer = textLayer;
            //write to textflow-layer with text-line-writer (handle the writing line)
            _lineEditor = new TextLineEditor(textLayer);

            //and record editing hx, support undo-redo
            _commandHistoryList = new DocumentCommandCollection(this);

#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder = new debugActivityRecorder();
                _lineEditor.dbugTextManRecorder = _dbugActivityRecorder;
                throw new NotSupportedException();
                _dbugActivityRecorder.Start(null);
            }
#endif

        }

        //
        internal List<VisualMarkerSelectionRange> VisualMarkers => _visualMarkers;
        //
        internal int VisualMarkerCount => (_visualMarkers == null) ? 0 : _visualMarkers.Count;

        public DocumentCommandListener DocCmdListener
        {
            get => _commandHistoryList.Listener;
            set => _commandHistoryList.Listener = value;
        }
        internal bool UndoMode { get; set; }
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
                if (!_lineEditor.CanAcceptThisChar(c))
                {
                    return;
                }
                //
                _commandHistoryList.AddDocAction(
                  new DocActionCharTyping(c, _lineEditor.LineNumber, _lineEditor.CharIndex));
            }

            _lineEditor.AddCharacter(c);
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
        }

        public Run CurrentTextRun => _lineEditor.GetCurrentTextRun();

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
            _lineEditor.SetCurrentCharIndex(startPoint.LineCharIndex);
            if (_selectionRange.IsOnTheSameLine)
            {
                var tobeDeleteTextRuns = new TextRangeCopy();
                _lineEditor.CopySelectedTextRuns(_selectionRange, tobeDeleteTextRuns);

                if (tobeDeleteTextRuns.HasSomeRuns)
                {
                    _commandHistoryList.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRuns,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));
                    _lineEditor.RemoveSelectedTextRuns(_selectionRange);
                    _updateJustCurrentLine = true;
                }
            }
            else
            {
                int startPointLindId = startPoint.LineId;
                int startPointCharIndex = startPoint.LineCharIndex;
                var tobeDeleteTextRuns = new TextRangeCopy();
                _lineEditor.CopySelectedTextRuns(_selectionRange, tobeDeleteTextRuns);
                if (tobeDeleteTextRuns != null && tobeDeleteTextRuns.HasSomeRuns)
                {
                    _commandHistoryList.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRuns,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));
                    _lineEditor.RemoveSelectedTextRuns(_selectionRange);
                    _updateJustCurrentLine = false;
                    _lineEditor.MoveToLine(startPointLindId);
                    _lineEditor.SetCurrentCharIndex(startPointCharIndex);
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

        public Run LatestHitRun => _textLayer.LatestHitRun;
        void SplitSelectedText()
        {

            if (_selectionRange == null) return;
            //
            SelectionRangeInfo selRangeInfo = _lineEditor.SplitSelectedText(_selectionRange);
            //add startPointInfo and EndPoint info to current selection range
            _selectionRange.StartPoint = selRangeInfo.start;
            _selectionRange.EndPoint = selRangeInfo.end;
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
                TextLine line = startPoint.Line;
                TextLine end_line = endPoint.Line;

                RunStyle runstyle = _lineEditor.CurrentSpanStyle;

                while (line.LineNumber <= end_line.LineNumber)
                {
                    //TODO, review here...
                    var whitespace = new TextRun(runstyle, "    ".ToCharArray());
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
                 new DocActionSplitToNewLine(_lineEditor.LineNumber, _lineEditor.CharIndex));
            _lineEditor.SplitToNewLine();
            CurrentLineNumber++;
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public void AddTextLine(PlainTextLine textline)
        {
            _updateJustCurrentLine = true;
            VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
            int startLineNum = _lineEditor.LineNumber;
            int startCharIndex = _lineEditor.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;

            //---------------------
            //TODO: review here again, use pool
            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            textline.CopyText(stbuilder);
            char[] textbuffer = stbuilder.ToString().ToCharArray();
            _lineEditor.AddTextSpan(textbuffer);
            //---------------------


            CopyRun copyRun = new CopyRun(textbuffer);
            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(copyRun, startLineNum, startCharIndex,
                    _lineEditor.LineNumber, _lineEditor.CharIndex));
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public TextSpanStyle GetFirstTextStyleInSelectedRange()
        {
            //TODO: review here again
            throw new NotSupportedException();
            //VisualSelectionRange selRange = SelectionRange;
            //if (selRange != null)
            //{
            //    if (_selectionRange.StartPoint.Run != null)
            //    {
            //        return _selectionRange.StartPoint.Run.SpanStyle;
            //    }
            //    else
            //    {
            //        return TextSpanStyle.Empty;
            //    }
            //}
            //else
            //{
            //    return TextSpanStyle.Empty;
            //}
        }
        public void DoFormatSelection(TextSpanStyle textStyle)
        {
            //int startLineNum = _textLineWriter.LineNumber;
            //int startCharIndex = _textLineWriter.CharIndex;
            SplitSelectedText();
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                RunStyle runstyle = new RunStyle(_textLayer.TextServices)
                {
                    ReqFont = textStyle.ReqFont,
                    FontColor = textStyle.FontColor,
                    ContentHAlign = textStyle.ContentHAlign
                };

                foreach (Run r in selRange.GetPrintableTextRunIter())
                {
                    r.SetStyle(runstyle);
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
            ////int startLineNum = _textLineWriter.LineNumber;
            ////int startCharIndex = _textLineWriter.CharIndex;
            //SplitSelectedText();
            //VisualSelectionRange selRange = SelectionRange;
            //if (selRange != null)
            //{
            //    foreach (EditableRun r in selRange.GetPrintableTextRunIter())
            //    {
            //        RunStyle existingStyle = r.SpanStyle;
            //        switch (toggleFontStyle)
            //        {
            //            case FontStyle.Bold:
            //                if ((existingStyle.ReqFont.Style & FontStyle.Bold) != 0)
            //                {
            //                    //change to normal
            //                    RequestFont existingFont = existingStyle.ReqFont;
            //                    RequestFont newReqFont = new RequestFont(
            //                        existingFont.Name, existingFont.SizeInPoints,
            //                        existingStyle.ReqFont.Style & ~FontStyle.Bold); //clear bold

            //                    RunStyle textStyle2 = new RunStyle();
            //                    textStyle2.ReqFont = newReqFont;
            //                    textStyle2.ContentHAlign = textStyle.ContentHAlign;
            //                    textStyle2.FontColor = textStyle.FontColor;
            //                    r.SetStyle(textStyle2);
            //                    continue;//go next***
            //                }
            //                break;
            //            case FontStyle.Italic:
            //                if ((existingStyle.ReqFont.Style & FontStyle.Italic) != 0)
            //                {
            //                    //change to normal
            //                    RequestFont existingFont = existingStyle.ReqFont;
            //                    RequestFont newReqFont = new RequestFont(
            //                        existingFont.Name, existingFont.SizeInPoints,
            //                        existingStyle.ReqFont.Style & ~FontStyle.Italic); //clear italic

            //                    TextSpanStyle textStyle2 = new TextSpanStyle();
            //                    textStyle2.ReqFont = newReqFont;
            //                    textStyle2.ContentHAlign = textStyle.ContentHAlign;
            //                    textStyle2.FontColor = textStyle.FontColor;
            //                    r.SetStyle(textStyle2);
            //                    continue;//go next***
            //                }
            //                break;
            //        }
            //        r.SetStyle(textStyle);
            //    }
            //    _updateJustCurrentLine = _selectionRange.IsOnTheSameLine;
            //    CancelSelect();
            //    //?
            //    //CharIndex++;
            //    //CharIndex--;
            //}
        }

        public void AddMarkerSpan(VisualMarkerSelectionRange markerRange)
        {
            markerRange.BindToTextLayer(_textLayer);
            if (_visualMarkers == null)
            {
                _visualMarkers = new List<VisualMarkerSelectionRange>();
            }
            _visualMarkers.Add(markerRange);
        }

        /// <summary>
        /// clear all marker
        /// </summary>
        public void ClearMarkers() => _visualMarkers?.Clear();
        public void RemoveMarkers(VisualMarkerSelectionRange marker)
        {
            _visualMarkers?.Remove(marker);
        }

        //
        public int CurrentLineCharCount => _lineEditor.CharCount;
        //
        public int LineCount => _lineEditor.LineCount;
        //
        public int CurrentLineCharIndex => _lineEditor.CharIndex;
        //
        public int CurrentTextRunCharIndex => _lineEditor.CurrentTextRunCharIndex;
        //
        public int CurrentLineNumber
        {
            get => _lineEditor.LineNumber;
            set
            {
                int diff = value - _lineEditor.LineNumber;
                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }
                    case 1:
                        {
                            if (_lineEditor.HasNextLine)
                            {
                                _lineEditor.MoveToNextLine();
                                DoHome();
                            }
                        }
                        break;
                    case -1:
                        {
                            if (_lineEditor.HasPrevLine)
                            {
                                _lineEditor.MoveToPrevLine();
                                DoEnd();
                            }
                        }
                        break;
                    default:
                        {
                            if (diff > 1)
                            {
                                _lineEditor.MoveToLine(value);
                            }
                            else
                            {
                                if (value < -1)
                                {
                                    _lineEditor.MoveToLine(value);
                                }
                                else
                                {
                                    _lineEditor.MoveToLine(value);
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

        public EditableVisualPointInfo GetCurrentPointInfo() => _lineEditor.GetCurrentPointInfo();

        /// <summary>
        /// find underlying word at current caret pos
        /// </summary>
        public void FindUnderlyingWord(out int startAt, out int len)
        {
            _lineEditor.FindCurrentHitWord(out startAt, out len);
        }

        public void TryMoveCaretTo(int charIndex, bool backward = false)
        {
            if (_lineEditor.CharIndex < 1 && charIndex < 0)
            {
                if (backward)
                {
                    if (_lineEditor.HasPrevLine)
                    {
                        _lineEditor.MoveToPrevLine();
                        DoEnd();
                    }
                }
            }
            else
            {
                int lineLength = _lineEditor.CharCount;
                if (_lineEditor.CharIndex >= lineLength && charIndex > lineLength)
                {
                    if (_lineEditor.HasNextLine)
                    {
                        _lineEditor.MoveToNextLine();
                    }
                }
                else
                {
                    _lineEditor.SetCurrentCharIndex(charIndex);
                    //check if we can stop at this char or not
                    if (backward)
                    {
                        //move caret backward
                        char prevChar = _lineEditor.PrevChar;
                        int tmp_index = charIndex;
                        while ((prevChar != '\0' && !CanCaretStopOnThisChar(prevChar)) && tmp_index > 0)
                        {
                            _lineEditor.SetCurrentCharStepLeft();
                            prevChar = _lineEditor.PrevChar;
                            tmp_index--;
                        }
                    }
                    else
                    {
                        char nextChar = _lineEditor.NextChar;
                        int lineCharCount = _lineEditor.CharCount;
                        int tmp_index = charIndex + 1;
                        while ((nextChar != '\0' && !CanCaretStopOnThisChar(nextChar)) && tmp_index < lineCharCount)
                        {
                            _lineEditor.SetCurrentCharStepRight();
                            nextChar = _lineEditor.NextChar;
                            tmp_index++;
                        }
                    }

                }
            }
        }
        public void TryMoveCaretForward()
        {
            //move caret forward 1 key stroke
            TryMoveCaretTo(_lineEditor.CharIndex + 1);
        }
        public void TryMoveCaretBackward()
        {
            TryMoveCaretTo(_lineEditor.CharIndex - 1, true);
        }
        //
        public int CharIndex => _lineEditor.CharIndex;
        //
        public bool IsOnEndOfLine => _lineEditor.IsOnEndOfLine;
        public bool IsOnStartOfLine => _lineEditor.IsOnStartOfLine;
        public int CurrentCaretHeight
        {
            get
            {
                Run currentRun = this.CurrentTextRun;
                return (currentRun != null) ? currentRun.Height : 17; //TODO: review this...
            }
        }
        public Point CaretPos
        {
            get => _lineEditor.CaretPosition;
            set
            {
                if (_lineEditor.LineCount > 0)
                {
                    TextLine line = _lineEditor.GetTextLineAtPos(value.Y);
                    int calculatedLineId = 0;
                    int lineTop = 0;
                    if (line != null)
                    {
                        calculatedLineId = line.LineNumber;
                        lineTop = line.Top;
                    }
                    this.CurrentLineNumber = calculatedLineId;
                    _lineEditor.TrySetCaretPos(value.X, value.Y - lineTop);
                }
            }
        }
        //
        public int GetNextCharacterWidth() => _lineEditor.NextCharWidth;
        //
        public void SetCaretPos(int x, int y)
        {
            if (_lineEditor.LineCount > 0)
            {
                TextLine line = _lineEditor.GetTextLineAtPos(y);
                int lineNo = 0;
                int lineTop = 0;
                if (line != null)
                {
                    lineNo = line.LineNumber;
                    lineTop = line.Top;
                }

                this.CurrentLineNumber = lineNo;
                _lineEditor.TrySetCaretPos(x, y - lineTop);
            }
        }
        public Rectangle CurrentLineArea => _lineEditor.LineArea;


        public bool IsOnFirstLine => !_lineEditor.HasPrevLine;

        void JoinWithNextLine()
        {
            _lineEditor.JoinWithNextLine();
            //
            NotifyContentSizeChanged();
        }
        public void UndoLastAction() => _commandHistoryList.UndoLastAction();

        public void ReverseLastUndoAction() => _commandHistoryList.ReverseLastUndoAction();
    }
}