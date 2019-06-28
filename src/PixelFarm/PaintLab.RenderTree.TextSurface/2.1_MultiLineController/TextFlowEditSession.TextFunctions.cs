//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.TextEditing.Commands;
namespace LayoutFarm.TextEditing
{

    partial class TextFlowEditSession
    {
        public void ReplaceCurrentLineTextRun(IEnumerable<Run> runs)
        {
            _lineWalker.ReplaceCurrentLine(runs);
        }

        public void ReplaceLocalContent(int nBackSpace, string content)
        {
            if (content != null)
            {
                for (int i = 0; i < nBackSpace; i++)
                {
                    DoBackspace();
                }
                //------------------
                int j = content.Length;
                if (j > 0)
                {
                    for (int i = 0; i < j; i++)
                    {
                        char c = content[i];
                        _lineWalker.AddCharacter(c);
                        _commandHistoryList.AddDocAction(
                            new DocActionCharTyping(c, _lineWalker.LineNumber, _lineWalker.CharIndex));
                    }
                }
            }
        }
        //public void AddUnformattedStringToCurrentLine(string str)
        //{
        //    //this should be a text-service work ***
        //    //TODO: use specific text model to format this document
        //    using (System.IO.StringReader reader = new System.IO.StringReader(str))
        //    {
        //        string line = reader.ReadLine();
        //        var runs = new List<TextRun>();
        //        var copyRange = new TextRangeCopy();

        //        int lineCount = 0;
        //        while (line != null)
        //        {
        //            if (lineCount > 0)
        //            {
        //                copyRange.AppendNewLine();
        //                //runs.Add(new EditableTextRun(root, '\n', initTextSpanStyle));
        //            }

        //            if (line.Length > 0)
        //            {
        //                copyRange.AddRun(new CopyRun(line));
        //                //runs.Add(new EditableTextRun(root, line, initTextSpanStyle));
        //            }

        //            //
        //            line = reader.ReadLine();
        //            lineCount++;
        //        }

        //        AddTextRunsToCurrentLine(copyRange);
        //    }
        //}

        public void AddTextToCurrentLine(PlainTextDocument doc)
        {
            int lineCount = 0;
            foreach (PlainTextLine line in doc.GetLineIter())
            {
                if (lineCount > 0)
                {
                    SplitCurrentLineIntoNewLine();
                }
                AddTextLine(line);
                lineCount++;
            }
        }
        public void AddTextRunsToCurrentLine(TextRangeCopy copyRange)
        {
            VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
            int startLineNum = _lineWalker.LineNumber;
            int startCharIndex = _lineWalker.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;

            if (copyRange.HasSomeRuns)
            {
                bool hasFirstLine = false;
                foreach (TextRangeCopy.TextLine line in copyRange.GetTextLineIter())
                {
                    if (hasFirstLine)
                    {
                        _lineWalker.SplitToNewLine();
                        CurrentLineNumber++;
                    }

                    foreach (CopyRun run in line.GetRunIter())
                    {
                        _lineWalker.AddTextSpan(run.RawContent);
                    }
                    hasFirstLine = true;
                }
            }



            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(copyRange, startLineNum, startCharIndex,
                    _lineWalker.LineNumber, _lineWalker.CharIndex));
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public void AddTextRunToCurrentLine(CopyRun copyRun)
        {
            AddTextRunToCurrentLine(copyRun.RawContent);
        }
        public void AddTextRunToCurrentLine(char[] textbuffer)
        {
            _updateJustCurrentLine = true;
            VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
            int startLineNum = _lineWalker.LineNumber;
            int startCharIndex = _lineWalker.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;
            _lineWalker.AddTextSpan(textbuffer);

            CopyRun copyRun = new CopyRun(textbuffer);
            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(copyRun, startLineNum, startCharIndex,
                    _lineWalker.LineNumber, _lineWalker.CharIndex));
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public void AddTextRunToCurrentLine(Run run)
        {
            _updateJustCurrentLine = true;
            VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
            int startLineNum = _lineWalker.LineNumber;
            int startCharIndex = _lineWalker.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;
            _lineWalker.AddTextSpan(run);


            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(run.CreateCopy(), startLineNum, startCharIndex,
                    _lineWalker.LineNumber, _lineWalker.CharIndex));
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public void CopyAllToPlainText(StringBuilder output)
        {
            _lineWalker.CopyContentToStrignBuilder(output);
        }
        public void Clear()
        {
            //1.
            CancelSelect();
            _textLayer.Clear();
            _lineWalker.Clear();
            //
            NotifyContentSizeChanged();
        }
        public void CopySelectedTextToPlainText(StringBuilder stBuilder)
        {
            if (_selectionRange != null)
            {
                _selectionRange.SwapIfUnOrder();
                if (_selectionRange.IsOnTheSameLine)
                {
                    var copyRuns = new TextRangeCopy();
                    _lineWalker.CopySelectedTextRuns(_selectionRange, copyRuns);
                    copyRuns.CopyContentToStringBuilder(stBuilder);

                }
                else
                {
                    VisualPointInfo startPoint = _selectionRange.StartPoint;
                    CurrentLineNumber = startPoint.LineId;
                    _lineWalker.SetCurrentCharIndex(startPoint.LineCharIndex);
                    var copyRuns = new TextRangeCopy();
                    _lineWalker.CopySelectedTextRuns(_selectionRange, copyRuns);
                    copyRuns.CopyContentToStringBuilder(stBuilder);
                }
            }
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            _lineWalker.CopyLineContent(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            if (_lineWalker.LineNumber == lineNum)
            {
                //on the sameline
                _lineWalker.CopyLineContent(output);
            }
            else
            {
                int cur_line = _lineWalker.LineNumber;
                _lineWalker.MoveToLine(lineNum);
                _lineWalker.CopyLineContent(output);
                _lineWalker.MoveToLine(cur_line);
            }
            //backGroundTextLineWriter.MoveToLine(lineNum);
            //backGroundTextLineWriter.CopyLineContent(output);
        }

        public void StartSelect()
        {
            if (_lineWalker != null)
            {
                _selectionRange = new VisualSelectionRange(_textLayer, GetCurrentPointInfo(), GetCurrentPointInfo());
            }
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::StartSelect");
            }
#endif
        }
        public void EndSelect()
        {
            if (_lineWalker != null)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo("TxLMan::EndSelect");
                }
#endif
                _selectionRange.EndPoint = GetCurrentPointInfo();
            }
        }

        public void CancelSelect()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::CancelSelect");
            }
#endif
            _selectionRange = null;
        }

        public void StartSelectIfNoSelection()
        {
            if (_selectionRange == null)
            {
                this.StartSelect();
            }
        }
        public void EndSelectIfNoSelection()
        {
            if (_selectionRange == null)
            {
                this.StartSelect();
            }
            this.EndSelect();
        }

        public VisualPointInfo FindTextRunOnPosition(int x, int y)
        {
            if (y < 0)
            {
                return null;
            }
            else
            {
                int j = _lineWalker.LineCount;
                if (j > 0)
                {
                    TextLine textLine = _lineWalker.GetTextLineAtPos(y);
                    if (textLine != null)
                    {
                        return textLine.GetTextPointInfoFromCaretPoint(x);
                    }
                }
                return null;
            }
        }
    }
}