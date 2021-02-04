//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using LayoutFarm.TextEditing.Commands;
using PixelFarm.Drawing;
using Typography.Text;

namespace LayoutFarm.TextEditing
{
    public class TextFlowSelectSession : ITextFlowSelectSession
    {
        internal TextFlowEditWalker _lineEditor;
#if DEBUG
        internal debugActivityRecorder _dbugActivityRecorder;
        internal bool dbugEnableTextManRecorder = false;
#endif

        protected VisualSelectionRange _selectionRange;//primary visual selection
        internal TextFlowLayer _textLayer;
        public event EventHandler SelectionCanceled;

        internal TextFlowSelectSession(TextFlowLayer textLayer)
        {
            //this controller control the editaible-textflow-layer
            _textLayer = textLayer;
            //write to textflow-layer with text-line-writer (handle the writing line)
            _lineEditor = new TextFlowEditWalker(textLayer);
        }

        public Run CurrentTextRun => _lineEditor.GetCurrentTextRun();
        public void CopyAllToPlainText(TextCopyBuffer output)
        {
            _lineEditor.CopyContentToStrignBuilder(output);
        }
        public void CopySelectedTextToPlainText(TextCopyBuffer output)
        {
            //TODO: review here!
            throw new NotSupportedException();
        }
        public void CopySelectedTextToPlainText(StringBuilder stBuilder)
        {
            if (_selectionRange != null)
            {
                _selectionRange.Normalize();
                if (_selectionRange.IsOnTheSameLine)
                {
                    var copyRuns = new TextCopyBuffer();
                    _lineEditor.CopySelectedTextRuns(_selectionRange, copyRuns);
                    copyRuns.CopyTo(stBuilder);

                }
                else
                {
                    VisualPointInfo startPoint = _selectionRange.StartPoint;
                    CurrentLineNumber = startPoint.LineId;
                    _lineEditor.SetCurrentCharIndex(startPoint.LineCharIndex);
                    var copyRuns = new TextCopyBuffer();
                    _lineEditor.CopySelectedTextRuns(_selectionRange, copyRuns);
                    copyRuns.CopyTo(stBuilder);
                }
            }
        }
        public void CopyCurrentLine(TextCopyBuffer output)
        {
            _lineEditor.CopyLineContent(output);
        }
        public void CopyLine(int lineNum, TextCopyBuffer output)
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
        public void SelectAll()
        {
            CurrentLineNumber = 0;
            SetCaretPos(0, 0);
            StartSelect();
            CurrentLineNumber = LineCount - 1;
            _lineEditor.SetCurrentCharIndexToEnd();
            EndSelect();
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

                Rectangle before = _selectionRange.GetSelectionUpdateArea();
                _selectionRange.EndPoint = GetCurrentPointInfo();
                Rectangle after = _selectionRange.GetSelectionUpdateArea();
                _textLayer.ClientLineBubbleupInvalidateArea(Rectangle.Union(before, after));
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
            if (_selectionRange != null)
            {
                //invalidate graphic area?

                _textLayer.ClientLineBubbleupInvalidateArea(_selectionRange.GetSelectionUpdateArea());
            }
            _selectionRange = null;
            SelectionCanceled?.Invoke(this, EventArgs.Empty);
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
                    TextLineBox textLine = _lineEditor.GetTextLineAtPos(y);
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
                        int prevChar = _lineEditor.PrevChar;
                        int tmp_index = charIndex;
#if DEBUG
                        //UnicodeCategory unicodeCat = char.GetUnicodeCategory(prevChar);
                        //bool is_highSurrogate = char.IsHighSurrogate(prevChar);
                        //bool is_lowSurrogate = char.IsLowSurrogate(prevChar);
#endif


                        while (prevChar != '\0' && tmp_index > 0 && !CanCaretStopOnThisChar(prevChar))
                        {
                            _lineEditor.SetCurrentCharStepLeft();
                            prevChar = _lineEditor.PrevChar;
                            tmp_index--;
                        }

                        //if (char.IsLowSurrogate(_lineEditor.CurrentChar))
                        //{
                        //    _lineEditor.SetCurrentCharStepLeft();
                        //}
                    }
                    else
                    {
                        int nextChar = _lineEditor.NextChar;

                        //#if DEBUG
                        //                        UnicodeCategory unicodeCat = char.GetUnicodeCategory(nextChar);
                        //                        bool is_highSurrogate = char.IsHighSurrogate(nextChar);
                        //                        bool is_lowSurrogate = char.IsLowSurrogate(nextChar);
                        //#endif

                        int lineCharCount = _lineEditor.CharCount;
                        int tmp_index = charIndex + 1;
                        while (nextChar != '\0' && tmp_index <= lineCharCount && !CanCaretStopOnThisChar(nextChar))
                        {
                            _lineEditor.SetCurrentCharStepRight();
                            nextChar = _lineEditor.NextChar;
                            tmp_index++;
                        }

                        //if (char.IsLowSurrogate(_lineEditor.CurrentChar))
                        //{
                        //    _lineEditor.SetCurrentCharStepRight();
                        //} 
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

        public void ReplaceCurrentLine(string newlineContent)
        {
            //temp fix
            throw new NotSupportedException();
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
                    TextLineBox line = _lineEditor.GetTextLineAtPos(value.Y);
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
                TextLineBox line = _lineEditor.GetTextLineAtPos(y);
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
        internal static bool CanCaretStopOnThisChar(int c)
        {
            char upper = (char)(c >> 16);
            char lower = (char)c;

            if (char.IsHighSurrogate((char)upper))
            {
                return false;
            }

            UnicodeCategory unicodeCatg = char.GetUnicodeCategory(lower);
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
                        return s_CaretCanStopOnThisChar(lower);
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


            //ref: https://docs.microsoft.com/en-us/dotnet/api/system.globalization.unicodecategory?view=netcore-3.1
            //ClosePunctuation 	21 	
            //Closing character of one of the paired punctuation marks, such as parentheses, square brackets, and braces. 
            //Signified by the Unicode designation "Pe" (punctuation, close). The value is 21.

            //ConnectorPunctuation 	18 	
            //Connector punctuation character that connects two characters. 
            //Signified by the Unicode designation "Pc" (punctuation, connector). The value is 18.

            //Control 	14 	
            //Control code character, with a Unicode value of U+007F or in the range U+0000 through U+001F or U+0080 through U+009F.
            //Signified by the Unicode designation "Cc" (other, control). The value is 14.

            //CurrencySymbol 	26 	
            //Currency symbol character. 
            //Signified by the Unicode designation "Sc" (symbol, currency). The value is 26.

            //DashPunctuation 	19 	
            //Dash or hyphen character. 
            //Signified by the Unicode designation "Pd" (punctuation, dash). The value is 19.

            //DecimalDigitNumber 	8 	
            //Decimal digit character, that is, a character in the range 0 through 9.
            //Signified by the Unicode designation "Nd" (number, decimal digit). The value is 8.

            //EnclosingMark 	7 	
            //Enclosing mark character, which is a nonspacing combining character that surrounds all previous characters up to and including a base character. 
            //Signified by the Unicode designation "Me" (mark, enclosing). The value is 7.

            //FinalQuotePunctuation 	23 	
            //Closing or final quotation mark character. 
            //Signified by the Unicode designation "Pf" (punctuation, final quote). The value is 23.

            //Format 	15 	
            //Format character that affects the layout of text or the operation of text processes, but is not normally rendered.
            //Signified by the Unicode designation "Cf" (other, format). The value is 15.

            //InitialQuotePunctuation 	22 	
            //Opening or initial quotation mark character. 
            //Signified by the Unicode designation "Pi" (punctuation, initial quote). The value is 22.

            //LetterNumber 	9 	
            //Number represented by a letter, instead of a decimal digit, for example, the Roman numeral for five, which is "V". 
            //The indicator is signified by the Unicode designation "Nl" (number, letter). The value is 9.

            //LineSeparator 	12 	
            //Character that is used to separate lines of text.
            //Signified by the Unicode designation "Zl" (separator, line). The value is 12.

            //LowercaseLetter 	1 	
            //Lowercase letter. 
            //Signified by the Unicode designation "Ll" (letter, lowercase). The value is 1.

            //MathSymbol 	25 	
            //Mathematical symbol character, such as "+" or "= ". 
            //Signified by the Unicode designation "Sm" (symbol, math). The value is 25.

            //ModifierLetter 	3 	
            //Modifier letter character, which is free-standing spacing character that indicates modifications of a preceding letter. 
            //Signified by the Unicode designation "Lm" (letter, modifier). The value is 3.

            //ModifierSymbol 	27 	
            //Modifier symbol character, which indicates modifications of surrounding characters. 
            //For example, the fraction slash indicates that the number to the left is the numerator and the number to the right is the denominator. 
            //The indicator is signified by the Unicode designation "Sk" (symbol, modifier). The value is 27.

            //NonSpacingMark 	5 	
            //Nonspacing character that indicates modifications of a base character. 
            //Signified by the Unicode designation "Mn" (mark, nonspacing). The value is 5.

            //OpenPunctuation 	20 	
            //Opening character of one of the paired punctuation marks, such as parentheses, square brackets, and braces.
            //Signified by the Unicode designation "Ps" (punctuation, open). The value is 20.

            //OtherLetter 	4 	
            //Letter that is not an uppercase letter, a lowercase letter, a titlecase letter, or a modifier letter. 
            //Signified by the Unicode designation "Lo" (letter, other). The value is 4.

            //OtherNotAssigned 	29 	
            //Character that is not assigned to any Unicode category. 
            //Signified by the Unicode designation "Cn" (other, not assigned). The value is 29.

            //OtherNumber 	10 	
            //Number that is neither a decimal digit nor a letter number, for example, the fraction 1/2. The indicator is signified by the Unicode designation "No" (number, other). The value is 10.

            //OtherPunctuation 	24 	
            //Punctuation character that is not a connector, a dash, open punctuation, close punctuation, an initial quote, or a final quote. 
            //Signified by the Unicode designation "Po" (punctuation, other). The value is 24.

            //OtherSymbol 	28 	
            //Symbol character that is not a mathematical symbol, a currency symbol or a modifier symbol. 
            //Signified by the Unicode designation "So" (symbol, other). The value is 28.

            //ParagraphSeparator 	13 	
            //Character used to separate paragraphs. 
            //Signified by the Unicode designation "Zp" (separator, paragraph). The value is 13.

            //PrivateUse 	17 	
            //Private-use character, with a Unicode value in the range U+E000 through U+F8FF. 
            //Signified by the Unicode designation "Co" (other, private use). The value is 17.

            //SpaceSeparator 	11 	
            //Space character, which has no glyph but is not a control or format character. 
            //Signified by the Unicode designation "Zs" (separator, space). The value is 11.

            //SpacingCombiningMark 	6 	
            //Spacing character that indicates modifications of a base character and affects the width of the glyph for that base character. 
            //Signified by the Unicode designation "Mc" (mark, spacing combining). The value is 6.

            //Surrogate 	16 	
            //High surrogate or a low surrogate character.
            //Surrogate code values are in the range U+D800 through U+DFFF. Signified by the Unicode designation "Cs" (other, surrogate). The value is 16.

            //TitlecaseLetter 	2 	
            //Titlecase letter.
            //Signified by the Unicode designation "Lt" (letter, titlecase). The value is 2.

            //UppercaseLetter 	0 	
            //Uppercase letter. 
            //Signified by the Unicode designation "Lu" (letter, uppercase). The value is 0.
        }

    }
}