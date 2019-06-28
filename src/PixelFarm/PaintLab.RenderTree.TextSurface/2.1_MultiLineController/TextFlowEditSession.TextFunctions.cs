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
            _lineEditor.ReplaceCurrentLine(runs);
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
                        _lineEditor.AddCharacter(c);
                        _commandHistoryList.AddDocAction(
                            new DocActionCharTyping(c, _lineEditor.LineNumber, _lineEditor.CharIndex));
                    }
                }
            }
        }
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
            int startLineNum = _lineEditor.LineNumber;
            int startCharIndex = _lineEditor.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;

            if (copyRange.HasSomeRuns)
            {
                bool hasFirstLine = false;
                foreach (string line in copyRange.GetLineIter())
                {
                    if (hasFirstLine)
                    {
                        _lineEditor.SplitToNewLine();
                        CurrentLineNumber++;
                    }
                    _lineEditor.AddTextSpan(line);
                    hasFirstLine = true;
                }
            }

            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(copyRange, startLineNum, startCharIndex,
                    _lineEditor.LineNumber, _lineEditor.CharIndex));
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
            int startLineNum = _lineEditor.LineNumber;
            int startCharIndex = _lineEditor.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;
            _lineEditor.AddTextSpan(textbuffer);

            CopyRun copyRun = new CopyRun(textbuffer);
            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(copyRun, startLineNum, startCharIndex,
                    _lineEditor.LineNumber, _lineEditor.CharIndex));
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public void AddTextRunToCurrentLine(Run run)
        {
            _updateJustCurrentLine = true;
            VisualSelectionRangeSnapShot removedRange = RemoveSelectedText();
            int startLineNum = _lineEditor.LineNumber;
            int startCharIndex = _lineEditor.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;
            _lineEditor.AddTextSpan(run);


            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(run.CreateCopy(), startLineNum, startCharIndex,
                    _lineEditor.LineNumber, _lineEditor.CharIndex));
            _updateJustCurrentLine = false;
            //
            NotifyContentSizeChanged();
        }
        public void CopyAllToPlainText(StringBuilder output)
        {
            _lineEditor.CopyContentToStrignBuilder(output);
        }
        public void Clear()
        {
            //1.
            CancelSelect();
            _textLayer.Clear();
            _lineEditor.Clear();
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
                    _lineEditor.CopySelectedTextRuns(_selectionRange, copyRuns);
                    copyRuns.CopyContentToStringBuilder(stBuilder);

                }
                else
                {
                    VisualPointInfo startPoint = _selectionRange.StartPoint;
                    CurrentLineNumber = startPoint.LineId;
                    _lineEditor.SetCurrentCharIndex(startPoint.LineCharIndex);
                    var copyRuns = new TextRangeCopy();
                    _lineEditor.CopySelectedTextRuns(_selectionRange, copyRuns);
                    copyRuns.CopyContentToStringBuilder(stBuilder);
                }
            }
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            _lineEditor.CopyLineContent(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            if (_lineEditor.LineNumber == lineNum)
            {
                //on the sameline
                _lineEditor.CopyLineContent(output);
            }
            else
            {
                int cur_line = _lineEditor.LineNumber;
                _lineEditor.MoveToLine(lineNum);
                _lineEditor.CopyLineContent(output);
                _lineEditor.MoveToLine(cur_line);
            }
            //backGroundTextLineWriter.MoveToLine(lineNum);
            //backGroundTextLineWriter.CopyLineContent(output);
        }

        public void StartSelect()
        {
            if (_lineEditor != null)
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
            if (_lineEditor != null)
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
                int j = _lineEditor.LineCount;
                if (j > 0)
                {
                    TextLine textLine = _lineEditor.GetTextLineAtPos(y);
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