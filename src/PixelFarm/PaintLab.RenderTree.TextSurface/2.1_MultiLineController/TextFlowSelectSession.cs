//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using System.Globalization;

using PixelFarm.Drawing;
using LayoutFarm.TextEditing.Commands;
using System.Text;

namespace LayoutFarm.TextEditing
{
    public class TextFlowSelectSession : ITextFlowSelectSession
    {
        internal TextLineEditor _lineEditor;
#if DEBUG
        internal debugActivityRecorder _dbugActivityRecorder;
        internal bool dbugEnableTextManRecorder = false;
#endif

        protected VisualSelectionRange _selectionRange;//primary visual selection
        internal TextFlowLayer _textLayer;

        internal TextFlowSelectSession(TextFlowLayer textLayer)
        {
            //this controller control the editaible-textflow-layer
            _textLayer = textLayer;
            //write to textflow-layer with text-line-writer (handle the writing line)
            _lineEditor = new TextLineEditor(textLayer);
        }
        protected void NotifyContentSizeChanged()
        {
            _textLayer.NotifyContentSizeChanged();
        }
        public Run CurrentTextRun => _lineEditor.GetCurrentTextRun();
        public void CopyAllToPlainText(StringBuilder output)
        {
            _lineEditor.CopyContentToStrignBuilder(output);
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
        public void DoEnd()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::DoEnd");
                _dbugActivityRecorder.BeginContext();
            }
#endif
            _lineEditor.SetCurrentCharIndexToEnd();
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
        }
        public void DoHome()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::DoHome");
                _dbugActivityRecorder.BeginContext();
            }
#endif

            _lineEditor.SetCurrentCharIndexToBegin();
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
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


        static Func<char, bool> s_CaretCanStopOnThisChar;

        public static void SetCaretCanStopOnThisChar(Func<char, bool> caretCanStopOnThisCharDel)
        {
            s_CaretCanStopOnThisChar = caretCanStopOnThisCharDel;
        }
        internal static bool CanCaretStopOnThisChar(char c)
        {
            UnicodeCategory unicodeCatg = char.GetUnicodeCategory(c);
            switch (unicodeCatg)
            {
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.Control:
                    break;
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    break;
                case UnicodeCategory.OtherLetter:

                    if (s_CaretCanStopOnThisChar != null)
                    {
                        return s_CaretCanStopOnThisChar(c);
                    }
                    break;
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                    //recursive
                    return false;
                default:
                    break;
            }
            return true;
        }

    }
}