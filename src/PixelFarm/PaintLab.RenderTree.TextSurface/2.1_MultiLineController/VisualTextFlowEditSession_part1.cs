//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.Text;

namespace LayoutFarm.TextFlow
{

    public partial class VisualTextFlowEditSession : ITextFlowEditSession
    {

        PlainTextEditSession _pte; //text-model edit (for plain  text)
        HistoryCollector _hx;//history collector
        TextFlowEditSessionListener _sessionListener; //other edit session listener
        TextFlowLayer _textLayer;//visual multi textlinebox layer
        VisualTextLineWalker _lineWalker;

        internal bool _updateJustCurrentLine = true;
        TextMarkerLayer _textMarkerLayer;

        internal VisualTextFlowEditSession(TextFlowLayer textLayer)
        {
            _textLayer = textLayer;

            _pte = new PlainTextEditSession();
            _pte.LoadPlainText(textLayer._plainText);
            //
            _hx = new HistoryCollector(this, _pte); //record editing hx, support undo-redo


            EnableUndoHistoryRecording = true;

            // 
            //create presentation
            int count = _pte.LineCount;
            for (int i = 0; i < count; ++i)
            {
                //create a visual presentation of text line///
                TextLineBox linebox = new TextLineBox(textLayer);

                //add visual line box
                textLayer.AppendNewLine(linebox);
            }

            _lineWalker = new VisualTextLineWalker();
            _lineWalker.LoadLine(textLayer.GetTextLine(0));
        }
        internal PlainTextEditSession GetPlainTextEditSession() => _pte;


        public Run LatestHitRun => _lineWalker.GetCurrentTextRun();
        public void CopyAllToPlainText(TextCopyBuffer output)
        {
            if (output is TextCopyBufferUtf32 u32buff)
            {
                _pte.CopyAll(u32buff);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public void CopySelectedTextToPlainText(System.Text.StringBuilder output)
        {
            //1. copy from model or from presentation?
            SetSelectionToTextModel();
            using (new TextUtf32RangeCopyPoolContext<VisualTextFlowEditSession>(out TextCopyBufferUtf32 u32buff))
            {
                _pte.CopySelection(u32buff);
                u32buff.CopyTo(output);
            }
        }
        public void CopySelectedTextToPlainText(TextCopyBuffer output)
        {
            SetSelectionToTextModel();
            if (output is TextCopyBufferUtf32 u32buff)
            {
                _pte.CopySelection(u32buff);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        protected void NotifyContentSizeChanged()
        {
            _textLayer.NotifyContentSizeChanged();
        }
        public TextFlowEditSessionListener EditSessionListener
        {
            get => _sessionListener;
            set => _sessionListener = value;
        }

        internal void SetMarkerLayer(TextMarkerLayer textMarkerLayer)
        {
            _textMarkerLayer = textMarkerLayer;
        }

        internal bool UndoMode { get; set; }
        //
        public bool EnableUndoHistoryRecording
        {
            get => _hx.EnableUndoHistoryRecording;
            set => _hx.EnableUndoHistoryRecording = value;
        }
        // 
        public void AddChar(int c)
        {
            _updateJustCurrentLine = true;
            bool passRemoveSelectedText = false;
            if (SelectionRange != null)
            {
                RemoveSelectedText();
                passRemoveSelectedText = true;
            }


            if (passRemoveSelectedText && c == ' ')
            {

            }
            else if (!_pte.CanAcceptThisChar(c))
            {
                return;
            }

            int pre_lineNumber = _lineWalker.LineNumber;
            int pre_charIndex = _lineWalker.CharIndex;

            UpdateHxLinePos();

            _hx.AddChar(c);


            _pte.AddChar(c);//text model
            _sessionListener?.AddChar(c);

            //then update visual presentation of current line
            //TODO: text-span visual formatting ...
            UpdateCurrentLinePresentation(pre_lineNumber, _lineWalker.CharIndex);
        }
        void UpdateHxLinePos()
        {
            _hx.SetCurrentPos(_lineWalker.LineNumber, _lineWalker.CharIndex);
            _sessionListener?.SetCurrentPos(_lineWalker.LineNumber, _lineWalker.CharIndex);
        }

        void SetSelectionToTextModel()
        {
            _selectionRange.Normalize();
            int s0 = _selectionRange.StartPoint.LineNumber;
            int s1 = _selectionRange.StartPoint.LineCharIndex;
            int e0 = _selectionRange.EndPoint.LineNumber;
            int e1 = _selectionRange.EndPoint.LineCharIndex;

            _pte.SetSelection(s0, s1, e0, e1);
            _hx.SetSelection(s0, s1, e0, e1);

            _sessionListener?.SetSelection(s0, s1, e0, e1);
        }

        bool RemoveSelectedText()
        {

            if (_selectionRange == null)
            {
                return false;
            }
            else if (!_selectionRange.IsValid)
            {
                CancelSelect();
                return false;
            }

            _selectionRange.Normalize();

            SelectionRangeSnapShot selSnapshot = _selectionRange.GetSelectionRangeSnapshot();
            VisualPointInfo startPoint = _selectionRange.StartPoint;
            CurrentLineNumber = startPoint.LineId;
            _lineWalker.SetCurrentCharIndex(startPoint.LineCharIndex);

            SetSelectionToTextModel();

            var deletedText = new TextCopyBufferUtf32();
            _pte.CopySelection(deletedText);

            if (deletedText.Length > 0)
            {
                int startPointLindId = startPoint.LineId;
                int startPointCharIndex = startPoint.LineCharIndex;

                UpdateHxLinePos();

                _sessionListener?.DoDelete();
                _hx.DoDelete();
                _pte.DoDelete();

                if (_selectionRange.IsOnTheSameLine)
                {
                    //2. update presentation
                    UpdateCurrentLinePresentation(0, -1);
                    _updateJustCurrentLine = true;
                }
                else
                {
                    //2. update presentation

                    int ss = selSnapshot.startLineNum;
                    int ee = selSnapshot.endLineNum;
                    int diff = ee - ss;

                    if (diff > 1)
                    {
                        for (int mm = 0; mm < diff; ++mm)
                        {
                            //remove difftimes
                            _textLayer.Remove(ss + 1);
                        }

                    }
                    else
                    {
                        _textLayer.Remove(ss + 1);
                    }
                    //UpdateLinePresentation2(_textLayer.GetTextLine(ss + 1), 0, -1);
                    UpdateLinePresentation2(_textLayer.GetTextLine(ss), 0, -1);

                    _updateJustCurrentLine = false;

                    MoveToLine(startPointLindId);

                    _lineWalker.SetCurrentCharIndex(startPointCharIndex);

                    _sessionListener?.SetCurrentPos(startPointLindId, startPointCharIndex);
                }
            }

            CancelSelect();
            return true;
        }

        static readonly char[] s_tabspaces = "    ".ToCharArray();

        public void DoTabOverSelectedRange()
        {
            //eg. user press 'Tab' key over selected range
            VisualSelectionRange selRange = SelectionRange;
            if (selRange == null) return;
            //

            //EditableVisualPointInfo startPoint = selRange.StartPoint;
            //EditableVisualPointInfo endPoint = selRange.EndPoint;
            ////
            //if (!selRange.IsOnTheSameLine)
            //{
            //    //TextLineBox line = startPoint.Line;
            //    //TextLineBox end_line = endPoint.Line; 
            //    //RunStyle runstyle = _textLayer.DefaultRunStyle;// _editWalker.CurrentSpanStyle;

            //    //while (line.LineNumber <= end_line.LineNumber)
            //    //{
            //    //    //TODO, review here...

            //    //    TextRun whitespace = null;// line.CreateTextRun(s_tabspaces);
            //    //    line.AddFirst(whitespace);
            //    //    line.TextLineReCalculateActualLineSize();
            //    //    line.RefreshInlineArrange();
            //    //    line = line.Next;//move to next line
            //    //}
            //    throw new NotSupportedException();
            //    return;//finish here
            //}

        }
        public void SplitIntoNewLine()
        {
            RemoveSelectedText();

            var cmd = new DocActionSplitToNewLine(_lineWalker.LineNumber, _lineWalker.CharIndex);
            //_commandHistoryList.AddDocAction(cmd);
            //_sessionListener?.AddDocAction(cmd);

            UpdateHxLinePos();
            _hx.SplitIntoNewLine();
            _sessionListener?.SplitIntoNewLine();
            //1. update model
            _pte.SplitIntoNewLine();
            //2. update presentation 

            _textLayer.InsertNewLine(_lineWalker.LineNumber + 1);//add new line

            UpdateLinePresentation2(_textLayer.GetTextLine(CurrentLineNumber), 0, -1); //entire line
            CurrentLineNumber++;//move to next
            _updateJustCurrentLine = false;
            UpdateCurrentLinePresentation(0, -1);
            //
            NotifyContentSizeChanged();
        }
        //public void AddTextLine(PlainTextLine textline)
        //{
        //    throw new NotSupportedException();

        //    ////TODO: replace 1 tab with 4 blank spaces? 
        //    //_updateJustCurrentLine = true;
        //    //SelectionRangeSnapShot removedRange = RemoveSelectedText();
        //    //int startLineNum = _lineWalker.LineNumber;
        //    //int startCharIndex = _lineWalker.CharIndex;
        //    //bool isRecordingHx = EnableUndoHistoryRecording;
        //    //EnableUndoHistoryRecording = false;

        //    ////--------------------- 

        //    ////copy content from prev line
        //    ////temp fix!
        //    ////TODO: review here
        //    ////char[] textbuffer = textline.ToString().ToCharArray();
        //    ////_editWalker.AddTextSpan(textbuffer);

        //    //string text_str = textline.ToString();
        //    //_tme.AddText(text_str); //add text to

        //    //EnableUndoHistoryRecording = isRecordingHx;

        //    //var cmd = new DocActionInsertText(new TextCopyBufferUtf16(text_str.ToCharArray()), startLineNum, startCharIndex,
        //    //        _lineWalker.LineNumber, _lineWalker.CharIndex);
        //    ////_commandHistoryList.AddDocAction(cmd);
        //    //_sessionListener?.AddDocAction(cmd);

        //    //_updateJustCurrentLine = false;
        //    ////
        //    //NotifyContentSizeChanged();
        //}

        public void AddMarkerSpan(VisualMarkerSelectionRange markerRange)
        {
            markerRange.BindToTextLayer(_textLayer);
            _textMarkerLayer.AddMarker(markerRange);
        }

        /// <summary>
        /// clear all marker
        /// </summary>
        public void ClearMarkers() => _textMarkerLayer?.Clear();
        public void RemoveMarkers(VisualMarkerSelectionRange marker)
        {
            _textMarkerLayer?.Remove(marker);
        }
        public void UndoLastAction()
        {
            EnableUndoHistoryRecording = false;
            UndoMode = true;

            _hx.UndoLastAction(this);

            EnableUndoHistoryRecording = true;
            UndoMode = false;
        }

        public void ReverseLastUndoAction()
        {
            //update model and presentation
            //_editSession.EnableUndoHistoryRecording = false;
            //_editSession.UndoMode = true;
            EnableUndoHistoryRecording = false;
            UndoMode = true;

            _hx.ReverseLastUndoAction();

            EnableUndoHistoryRecording = true;
            UndoMode = false;
        }

#if DEBUG
        int dbug_BackSpaceCount = 0;
#endif
        public void DoDelete()
        {
            //recursive

#if DEBUG
            //if (dbugEnableTextManRecorder)
            //{
            //    _dbugActivityRecorder.WriteInfo("TxLMan::DoDelete");
            //    _dbugActivityRecorder.BeginContext();
            //}
#endif

            //have selected text or not
            //if yes=> delete the selected text
            //else  delete             

            if (!this.RemoveSelectedText())
            {

                _updateJustCurrentLine = true;
                //edit session may delete more than 1 char** (since some char can' exist alone)***
                //or it may involve more than 1 line 
                if (!_pte.IsOnTheEnd())
                {
                    int before = _pte.NewCharIndex; //before

                    UpdateHxLinePos();
                    _hx.DoDelete();
                    _sessionListener?.DoDelete();
                    _pte.ResetAffectedCharCount();
                    _pte.DoDelete();

                    int affectedCount = _pte.AffectedCharCount;
                    int after = _pte.NewCharIndex;

                    //update presentation
                    UpdateCurrentLinePresentation(before, affectedCount);
                    //_walker.SetCurrentCharIndex2(_editSession.NewCharIndex);
                }
                else
                {
                    //we are on the end of this line                      

                    int before = _pte.NewCharIndex; //before

                    UpdateHxLinePos();
                    _sessionListener?.DoDelete();
                    _hx.DoDelete();

                    _pte.ResetAffectedCharCount();
                    _pte.DoDelete();

                    int affectedCount = _pte.AffectedCharCount;
                    int after = _pte.NewCharIndex;

                    //update presentation 
                    if (CurrentLineNumber < LineCount - 1)
                    {
                        //not the last line
                        //remove lower
                        _textLayer.Remove(CurrentLineNumber + 1);
                    }

                    UpdateCurrentLinePresentation(before, affectedCount);
                    //next line will merge in
                    //update presentation
                    //
                    NotifyContentSizeChanged();
                }

                return;
            }
            NotifyContentSizeChanged();
        }



        TextCopyBufferUtf32 _copyBuffer = new TextCopyBufferUtf32();
        List<Run> _collectedRuns = new List<Run>();

        void UpdateLinePresentation2(TextLineBox linebox, int startAt, int affectedCharCount)
        {
            linebox.Clear();
            _copyBuffer.Clear();

            _pte.CopyLine(linebox.LineNumber, _copyBuffer);

            TextRun run = new TextRun(_textLayer.DefaultRunStyle, _textLayer.CharSource.NewSegment(_copyBuffer));
            linebox.AddLast(run);
            linebox.TextLineReCalculateActualLineSize();
            linebox.RefreshInlineArrange();

        }
        void UpdateCurrentLinePresentation(int startAt, int affectedCharCount)
        {
            //check if we need to create entire line or not
            //check if we involve a single run or multiple run


            //the formatting is up to user
            //eg.
            //1) replace entire line
            //2) edit somepart of the current line

            bool clear_line_and_create = false;
            if (clear_line_and_create)
            {
                _copyBuffer.Clear();
                _pte.ReadCurrentLine(_copyBuffer); //copy current line to temp buffer
                                                   //create text runs
                                                   //users can parse and format the line content by themself
                UpdateLinePresentation2(_lineWalker.CurrentLine, startAt, affectedCharCount);
                _lineWalker.SetCurrentCharIndex2(_pte.NewCharIndex);

            }
            else
            {

                _collectedRuns.Clear(); //collect consecutive runs
                _lineWalker.CollectAffectedRuns(startAt, affectedCharCount, _collectedRuns);
                _copyBuffer.Clear();

                if (_collectedRuns.Count == 1)
                {
                    //affect single run

                    _pte.ReadCurrentLine(_copyBuffer); //copy current line to temp buffer, create text runs

                    TextRun rr = (TextRun)_lineWalker.GetCurrentTextRun();
                    rr.SetNewContent(_textLayer.CharSource.NewSegment(_copyBuffer));
                    rr.UpdateRunWidth();

                    TextLineBox linebox = _lineWalker.CurrentLine;
                    linebox.InvalidateCharCount();
                    int newcount = linebox.CharCount();
                    linebox.TextLineReCalculateActualLineSize();
                    linebox.RefreshInlineArrange();

                    _lineWalker.SetCurrentCharIndex2(_pte.NewCharIndex);
                }
                else if (_collectedRuns.Count > 1)
                {
                    throw new NotSupportedException();
                }
                else
                {
                    _pte.ReadCurrentLine(_copyBuffer); //copy current line to temp buffer
                                                       //create text runs
                    TextLineBox linebox = _lineWalker.CurrentLine;
                    linebox.Clear();
                    TextRun run = new TextRun(_textLayer.DefaultRunStyle, _textLayer.CharSource.NewSegment(_copyBuffer));
                    linebox.AddLast(run);
                    linebox.TextLineReCalculateActualLineSize();
                    linebox.RefreshInlineArrange();
                    _lineWalker.SetCurrentCharIndex2(_pte.NewCharIndex);
                }

            }
        }
        public bool DoBackspace()
        {
            if (RemoveSelectedText())
            {
                CancelSelect();
                NotifyContentSizeChanged();
                return true;
            }

            _updateJustCurrentLine = true;

            int before1 = _pte.NewCharIndex; //before
            UpdateHxLinePos();
            _sessionListener?.DoBackspace();

            _hx.DoBackspace();

            _pte.DoBackspace();
            //how check hx result

            if (_pte.DeletedCharCount == 0)
            {
                //no char delete
                if (CurrentLineNumber > 0)
                {
                    //move to current line
                    MoveToPrevLine();
                    DoEnd();
                    //remove lower line
                    UpdateLinePresentation2(_textLayer.GetTextLine(CurrentLineNumber), 0, -1);
                    _textLayer.Remove(CurrentLineNumber + 1);
                    NotifyContentSizeChanged();
                    return true;
                }
                // throw new NotSupportedException();
                return false;
            }
            else
            {
                //update presentation
                UpdateCurrentLinePresentation(_pte.NewCharIndex, _pte.NewCharIndex - before1 + 1);
                NotifyContentSizeChanged();
                return true;
            }
        }

        public void CopyCurrentLine(TextCopyBuffer output)
        {
            if (output is TextCopyBufferUtf32 u32Output)
            {
                _pte.CopyCurrentLine(u32Output);
            }
            else
            {
                //TODO:
                throw new NotSupportedException();
            }

        }
        public void CopyLine(int lineNum, TextCopyBuffer output)
        {
            _pte.CopyLine(lineNum, output);
        }

        public void ReplaceLocalContent(int nBackSpace, string content)
        {
            //delete and replace
            //1.
            int start = _pte.NewCharIndex;
            _pte.NewCharIndex -= nBackSpace;
            _pte.SetSelection(_pte.CurrentLineNumber, start - nBackSpace, _pte.CurrentLineNumber, start);
            _pte.DoDelete();//
            //and insert
            _pte.AddText(content);
            //then update visual presentation  
            UpdateCurrentLinePresentation(0, -1);
            //int dd = _pte.NewCharIndex;
            //int dd3 = _lineWalker.CharIndex;



            //if (content != null)
            //{
            //    //replace 
            //    //record hx as once***
            //    for (int i = 0; i < nBackSpace; i++)
            //    {

            //    }
            //    //------------------
            //    int j = content.Length;
            //    if (j > 0)
            //    {
            //        //TODO: ! replace with text***

            //        //for (int i = 0; i < j; i++)
            //        //{
            //        //    char c = content[i];
            //        //    _editWalker.AddCharacter(c);

            //        //    //TODO:... review this again!

            //        //    var cmd = new DocActionCharTyping(c, _editWalker.LineNumber, _editWalker.CharIndex);
            //        //    _commandHistoryList.AddDocAction(cmd);
            //        //    _sessionListener?.AddDocAction(cmd);
            //        //}
            //    }
            //}
        }


        readonly TextCopyBufferUtf32 _tmpBuffer = new TextCopyBufferUtf32();//TODO: use pool
        public void AddText(string doc)
        {
            //helper
            _tmpBuffer.Clear();
            _tmpBuffer.AppendData(doc);
            AddText(_tmpBuffer);
        }

        public void AddText(TextCopyBuffer output)
        {
            RemoveSelectedText();

            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;

            if (output.Length > 0)
            {
                int prev_lineNo = _pte.CurrentLineNumber;
                //add multiline text to the model
                UpdateHxLinePos();
                _hx.AddText(output);
                _sessionListener?.AddText(output);
                _pte.NewCharIndex = _lineWalker.CharIndex;
                _pte.AddText(output);
                //

                int cur_lineNo = _pte.CurrentLineNumber;

                //then update
                //update visual presentation of all lines

                for (int i = prev_lineNo; i <= cur_lineNo; ++i)
                {
                    TextLineBox linebox = _textLayer.GetTextLine(i);
                    if (linebox != null)
                    {
                        UpdateLinePresentation2(linebox, 0, -1);
                    }
                    else
                    {
                        //not found this line

                        if (i == _textLayer.LineCount)
                        {
                            linebox = new TextLineBox(_textLayer);
                            _textLayer.AppendNewLine(linebox);
                            UpdateLinePresentation2(linebox, 0, -1);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }

                CurrentLineNumber = _pte.CurrentLineNumber;
                TryMoveCaretTo(_pte.NewCharIndex, true);
            }

            EnableUndoHistoryRecording = isRecordingHx;

            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }


        internal void Clear()
        {
            //1.
            CancelSelect();
            _textLayer.Clear();

            //clear all-------

            MoveToLine(0);
            //clear current text 
            int currentCharIndex = _lineWalker.CharIndex;
            _lineWalker.CurrentLine.Clear();
            _lineWalker.CurrentLine.TextLineReCalculateActualLineSize();
            _lineWalker.CurrentLine.RefreshInlineArrange();
            _lineWalker.EnsureCurrentTextRun(currentCharIndex);

            _lineWalker.EnsureCurrentTextRun();
            //-------
            //
            NotifyContentSizeChanged();
        }

    }
}