//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.Text
{
    partial class InternalTextLayerController
    {
        public void ReplaceCurrentLineTextRun(IEnumerable<EditableRun> textruns)
        {
            _textLineWriter.ReplaceCurrentLine(textruns);
        }
        public void ReplaceLine(int lineNum, IEnumerable<EditableRun> textruns)
        {
            if (_textLineWriter.LineNumber == lineNum)
            {
                //on the sameline
                _textLineWriter.ReplaceCurrentLine(textruns);
            }
            else
            {
                int cur_line = _textLineWriter.LineNumber;
                _textLineWriter.MoveToLine(lineNum);
                _textLineWriter.ReplaceCurrentLine(textruns);
                _textLineWriter.MoveToLine(cur_line);
            }
            //if (textLineWriter.LineNumber == backGroundTextLineWriter.LineNumber)
            //{
            //    int prevIndex = textLineWriter.CharIndex;
            //    textLineWriter.ReplaceCurrentLine(textruns);
            //}
            //else
            //{
            //    backGroundTextLineWriter.MoveToLine(lineNum);
            //    backGroundTextLineWriter.ReplaceCurrentLine(textruns);
            //}
        }
        public void LoadTextRun(IEnumerable<EditableRun> runs)
        {
            this.CancelSelect();
            _textLineWriter.Clear();
            _textLineWriter.Reload(runs);
            _updateJustCurrentLine = false;
            _textLineWriter.MoveToLine(0);
        }

        public void AddRuns(IEnumerable<EditableRun> textSpans)
        {
            foreach (var span in textSpans)
            {
                _textLineWriter.AddTextSpan(span);
            }
        }
        public void ReplaceCurrentTextRunContent(int nBackSpace, EditableRun newTextRun)
        {
            if (newTextRun != null)
            {
                EnableUndoHistoryRecording = false;

                for (int i = 0; i < nBackSpace; i++)
                {
                    DoBackspace();
                }

                EnableUndoHistoryRecording = true;
                int startLineNum = _textLineWriter.LineNumber;
                int startCharIndex = _textLineWriter.CharIndex;
                _textLineWriter.AddTextSpan(newTextRun);
                _textLineWriter.EnsureCurrentTextRun();

                _commandHistoryList.AddDocAction(
                    new DocActionInsertRuns(
                        new EditableRun[] { newTextRun }, startLineNum, startCharIndex,
                        _textLineWriter.LineNumber, _textLineWriter.CharIndex));
            }
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
                        _textLineWriter.AddCharacter(content[i]);
                    }
                }
            }
        }
        public void AddUnformattedStringToCurrentLine(string str, TextSpanStyle initTextSpanStyle)
        {
            //this should be a text-service work ***
            using (System.IO.StringReader reader = new System.IO.StringReader(str))
            {
                string line = reader.ReadLine();
                List<EditableTextRun> runs = new List<EditableTextRun>();
                RootGraphic root = _visualTextSurface.Root;
                int lineCount = 0;
                while (line != null)
                {
                    if (lineCount > 0)
                    {
                        runs.Add(new EditableTextRun(root, '\n', initTextSpanStyle));
                    }

                    runs.Add(new EditableTextRun(root, line, initTextSpanStyle));
                    line = reader.ReadLine();
                    lineCount++;
                }

                AddTextRunsToCurrentLine(runs.ToArray());
            }

        }
        public void AddTextRunsToCurrentLine(IEnumerable<EditableRun> textRuns)
        {
            RemoveSelectedText();
            int startLineNum = _textLineWriter.LineNumber;
            int startCharIndex = _textLineWriter.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;
            foreach (EditableRun t in textRuns)
            {
                if (t.IsLineBreak)
                {
                    _textLineWriter.SplitToNewLine();
                    CurrentLineNumber++;
                }
                else
                {
                    _textLineWriter.AddTextSpan(t);
                }
            }
            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(textRuns, startLineNum, startCharIndex,
                    _textLineWriter.LineNumber, _textLineWriter.CharIndex));
            _updateJustCurrentLine = false;
            TextEditRenderBox.NotifyTextContentSizeChanged(_visualTextSurface);
        }
        public void AddTextRunToCurrentLine(EditableRun t)
        {
            _updateJustCurrentLine = true;
            RemoveSelectedText();
            int startLineNum = _textLineWriter.LineNumber;
            int startCharIndex = _textLineWriter.CharIndex;
            bool isRecordingHx = EnableUndoHistoryRecording;
            EnableUndoHistoryRecording = false;
            if (t.IsLineBreak)
            {
                _textLineWriter.SplitToNewLine();
                CurrentLineNumber++;
            }
            else
            {
                _textLineWriter.AddTextSpan(t);
            }

            EnableUndoHistoryRecording = isRecordingHx;
            _commandHistoryList.AddDocAction(
                new DocActionInsertRuns(t, startLineNum, startCharIndex,
                    _textLineWriter.LineNumber, _textLineWriter.CharIndex));
            _updateJustCurrentLine = false;
            TextEditRenderBox.NotifyTextContentSizeChanged(_visualTextSurface);
        }
        public void CopyAllToPlainText(StringBuilder output)
        {
            _textLineWriter.CopyContentToStrignBuilder(output);
        }
        public void Clear()
        {
            CancelSelect();
            _textLineWriter.Clear();
            TextEditRenderBox.NotifyTextContentSizeChanged(_visualTextSurface);
        }


        public void CopySelectedTextToPlainText(StringBuilder stBuilder)
        {
            if (_selectionRange == null)
            {
            }
            else
            {
                _selectionRange.SwapIfUnOrder();
                if (_selectionRange.IsOnTheSameLine)
                {
                    List<EditableRun> copyRuns = new List<EditableRun>();
                    _textLineWriter.CopySelectedTextRuns(_selectionRange, copyRuns);
                    foreach (EditableRun t in copyRuns)
                    {
                        t.CopyContentToStringBuilder(stBuilder);
                    }
                }
                else
                {
                    VisualPointInfo startPoint = _selectionRange.StartPoint;
                    CurrentLineNumber = startPoint.LineId;
                    _textLineWriter.SetCurrentCharIndex(startPoint.LineCharIndex);
                    List<EditableRun> copyRuns = new List<EditableRun>();
                    _textLineWriter.CopySelectedTextRuns(_selectionRange, copyRuns);
                    foreach (EditableRun t in copyRuns)
                    {
                        t.CopyContentToStringBuilder(stBuilder);
                    }
                }
            }
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            _textLineWriter.CopyLineContent(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            if (_textLineWriter.LineNumber == lineNum)
            {
                //on the sameline
                _textLineWriter.CopyLineContent(output);
            }
            else
            {
                int cur_line = _textLineWriter.LineNumber;
                _textLineWriter.MoveToLine(lineNum);
                _textLineWriter.CopyLineContent(output);
                _textLineWriter.MoveToLine(cur_line);
            }
            //backGroundTextLineWriter.MoveToLine(lineNum);
            //backGroundTextLineWriter.CopyLineContent(output);
        }

        public void StartSelect()
        {
            if (_textLineWriter != null)
            {
                _selectionRange = new VisualSelectionRange(GetCurrentPointInfo(), GetCurrentPointInfo());
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
            if (_textLineWriter != null)
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
            if (this._selectionRange == null)
            {
                this.StartSelect();
            }
        }
        public void EndSelectIfNoSelection()
        {
            if (this._selectionRange == null)
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
                int j = _textLineWriter.LineCount;
                if (j > 0)
                {
                    EditableTextLine textLine = _textLineWriter.GetTextLineAtPos(y);
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