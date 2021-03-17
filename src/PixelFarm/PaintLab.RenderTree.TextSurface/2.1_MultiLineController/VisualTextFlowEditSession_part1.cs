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
            if (_selectionRange != null)
            {
                SetSelectionToTextModel();
                using (new TextUtf32RangeCopyPoolContext<VisualTextFlowEditSession>(out TextCopyBufferUtf32 u32buff))
                {
                    _pte.CopySelection(u32buff);
                    u32buff.CopyTo(output);
                }
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
            _hx.IncrementHxStepNumber();
            _updateJustCurrentLine = true;
            RemoveSelectedText();

            int pre_lineNumber = _lineWalker.LineNumber;
            int pre_charIndex = _lineWalker.NewCharIndex;

            UpdateCaretPos();

            _hx.AddChar(c);
            _pte.AddChar(c);//text model


            //then update visual presentation of current line
            //TODO: text-span visual formatting ...
            UpdateCurrentLinePresentation(pre_lineNumber, _lineWalker.NewCharIndex);
        }
        /// <summary>
        /// update caret position from visual layer
        /// </summary>
        void UpdateCaretPos()
        {
            if (_pte.NewCharIndex != _lineWalker.NewCharIndex)
            {
                _pte.NewCharIndex = _lineWalker.NewCharIndex;
            }
            _hx.SetCurrentPos(_lineWalker.LineNumber, _lineWalker.NewCharIndex);
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

        }

        bool RemoveSelectedText()
        {
            if (_selectionRange == null)
            {
                return false;
            }
            else if (!_selectionRange.IsValid)
            {
                CancelSelect();//clear
                return false;
            }

            //
            _selectionRange.Normalize();
            SelectionRangeSnapShot sel = _selectionRange.GetSelectionRangeSnapshot();
            VisualPointInfo startPoint = _selectionRange.StartPoint;
            CurrentLineNumber = startPoint.LineId;
            _lineWalker.SetCurrentCharIndex(startPoint.LineCharIndex);

            SetSelectionToTextModel();


            int startPointLindId = startPoint.LineId;
            int startPointCharIndex = startPoint.LineCharIndex;

            UpdateCaretPos();

            _hx.DoDelete();
            _pte.DoDelete(); //delete text

            if (_selectionRange.IsOnTheSameLine)
            {
                //2. update presentation
                UpdateCurrentLinePresentation(0, -1);
                _updateJustCurrentLine = true;
            }
            else
            {
                //2. update presentation

                int ss = sel.startLineNum;
                int ee = sel.endLineNum;
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

                UpdateLinePresentation2(_textLayer.GetTextLine(ss), 0, -1);

                _updateJustCurrentLine = false;

                MoveToLine(startPointLindId);

                _lineWalker.SetCurrentCharIndex(startPointCharIndex);

            }

            //after 
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
            _hx.IncrementHxStepNumber();
            RemoveSelectedText();

            UpdateCaretPos();
            _hx.SplitIntoNewLine();

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
            _hx.IncrementHxStepNumber();
            if (RemoveSelectedText())
            {
                //TODO: review here
                NotifyContentSizeChanged();
                return;
            }

            //no selection range-> noting to delete 
            _updateJustCurrentLine = true;
            //edit session may delete more than 1 char** (since some char can' exist alone)***
            //or it may involve more than 1 line

            if (!_pte.IsOnTheEnd())
            {
                //current line
                _pte.NewCharIndex = _lineWalker.NewCharIndex;
                int numCharAffected = _pte.PreviewSingleCharDelete();
                int before = _pte.NewCharIndex; //before

                if (numCharAffected == 0)
                {
                    //we are on the end of this line
                    //should not occure because we check this in the upper step
                }
                else if (numCharAffected == 1)
                {
                    //single char delete
                    UpdateCaretPos();

                    _pte.TempCopyBuffer.Clear();//reset
                    _pte.TempCopyBuffer.Append(_pte.GetCharacter(before));

                    _hx.DoDelete();//record delete  
                    //single delete may afffect more than 1 char 
                    _pte.DoDelete();
                    //update presentation
                    UpdateCurrentLinePresentation(before, 1);
                }
                else
                {
                    //more than 1 char, just do-
                    UpdateCaretPos();
                    _pte.TempCopyBuffer.Clear();//reset
                    for (int i = 0; i < numCharAffected; ++i)
                    {
                        //tobe deleted char
                        _pte.TempCopyBuffer.Append(_pte.GetCharacter(before + i));
                    }

                    int s0 = _pte.CurrentLineNumber;
                    int s1 = before;
                    int e0 = s0;
                    int e1 = before + numCharAffected;

                    _pte.SetSelection(s0, s1, e0, e1);
                    _hx.SetSelection(s0, s1, e0, e1);


                    _hx.DoDelete();
                    _pte.DoDelete(); //delete text
                    UpdateCurrentLinePresentation(before, numCharAffected);
                }
            }
            else
            {
                //we are on the end of this line            
                if (CurrentLineNumber < _textLayer.LineCount - 1)
                {
                    //has lower line

                    int before = _pte.NewCharIndex; //before
                    UpdateCaretPos();
                    _hx.DoDelete();
                    _pte.DoDelete();

                    int after = _pte.NewCharIndex;
                    //update presentation 
                    if (CurrentLineNumber < LineCount - 1)
                    {
                        //not the last line
                        //remove lower
                        TextLineBox curLine = _textLayer.GetTextLine(CurrentLineNumber);
                        _textLayer.Remove(CurrentLineNumber + 1);

                        UpdateCurrentLinePresentation(before, 1);
                        NotifyContentSizeChanged();
                        _textLayer.ClientLineBubbleupInvalidateArea(
                            new PixelFarm.Drawing.Rectangle(0, curLine.Top, _textLayer.OwnerWidth, _textLayer.OwnerHeight - curLine.Top));
                    }
                    else
                    {
                        UpdateCurrentLinePresentation(before, 1);

                        NotifyContentSizeChanged();
                    }
                }
            }
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
            _hx.IncrementHxStepNumber();
            if (RemoveSelectedText())
            {
                //TODO: review here
                NotifyContentSizeChanged();
                return true;
            }

            _updateJustCurrentLine = true;

            if (_lineWalker.NewCharIndex == 0)
            {

                //at the begein of line
                if (CurrentLineNumber > 0)
                {
                    _pte.NewCharIndex = _lineWalker.NewCharIndex;
                    int before1 = _pte.NewCharIndex; //before
                    _pte.GetCharacter(before1);

                    UpdateCaretPos();

                    _hx.DoBackspace();
                    _pte.DoBackspace();

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
                _pte.NewCharIndex = _lineWalker.NewCharIndex;
                int c = _pte.GetCharacter(_pte.NewCharIndex - 1); //tobe deleted char
                _pte.TempCopyBuffer.Clear();//reset
                _pte.TempCopyBuffer.Append(c);//copy to temp buffer, and shared it between hx-collection and session listener
                //
                int before1 = _pte.NewCharIndex; //before

                UpdateCaretPos();

                _hx.DoBackspace();
                _pte.DoBackspace();

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
            _tmpBuffer.Append(doc);
            AddText(_tmpBuffer);
        }

        public void AddText(TextCopyBuffer input)
        {
            _hx.IncrementHxStepNumber();
            RemoveSelectedText();
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = true;
            if (input.Length > 0)
            {
                int prev_lineNo = _pte.CurrentLineNumber;
                //add multiline text to the model
                UpdateCaretPos();

                _hx.AddText(input);

                _pte.NewCharIndex = _lineWalker.NewCharIndex;
                _pte.AddText(input);
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

                int end_LineCharIndex = _pte.NewCharIndex;
                CurrentLineNumber = _pte.CurrentLineNumber;
                _pte.NewCharIndex = end_LineCharIndex;
                TryMoveCaretTo(end_LineCharIndex, true);
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
            int currentCharIndex = _lineWalker.NewCharIndex;
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