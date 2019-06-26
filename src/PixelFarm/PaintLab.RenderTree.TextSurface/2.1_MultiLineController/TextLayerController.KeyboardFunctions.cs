//Apache2, 2014-present, WinterDev

using System;
using System.Globalization;
using LayoutFarm.TextEditing.Commands;
namespace LayoutFarm.TextEditing
{
    partial class InternalTextLayerController
    {
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
        public VisualSelectionRangeSnapShot DoDelete()
        {
            //recursive
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::DoDelete");
                _dbugActivityRecorder.BeginContext();
            }
#endif

            VisualSelectionRangeSnapShot removedRange = this.RemoveSelectedText();
            if (removedRange.IsEmpty())
            {
                _updateJustCurrentLine = true;

                char deletedChar = _textLineWriter.DoDeleteOneChar();
                if (deletedChar == '\0')
                {
                    //end of this line
                    _commandHistoryList.AddDocAction(
                        new DocActionJoinWithNextLine(
                            _textLineWriter.LineNumber, _textLineWriter.CharIndex));

                    JoinWithNextLine();

                    _updateJustCurrentLine = false;
                }
                else
                {
                    _commandHistoryList.AddDocAction(
                        new DocActionDeleteChar(
                            deletedChar, _textLineWriter.LineNumber, _textLineWriter.CharIndex));

                    char nextChar = _textLineWriter.NextChar;

                    if (nextChar != '\0')
                    {
                        if (!CanCaretStopOnThisChar(nextChar))
                        {
                            //TODO: review return range here again
                            return DoDelete();
                        }
                    }
                }
            }
#if DEBUG
            if (dbugEnableTextManRecorder) _dbugActivityRecorder.EndContext();
#endif
            NotifyContentSizeChanged();
            return removedRange;
        }
#if DEBUG
        int dbug_BackSpaceCount = 0;
#endif
        public bool DoBackspace()
        {
#if DEBUG

            if (dbugEnableTextManRecorder)
            {
                dbug_BackSpaceCount++;
                _dbugActivityRecorder.WriteInfo("TxLMan::DoBackSpace");
                _dbugActivityRecorder.BeginContext();
            }
#endif

            VisualSelectionRangeSnapShot removeSelRange = this.RemoveSelectedText();
            if (!removeSelRange.IsEmpty())
            {
                CancelSelect();
                NotifyContentSizeChanged();
#if DEBUG
                if (dbugEnableTextManRecorder) _dbugActivityRecorder.EndContext();
#endif
                return true;
            }
            else
            {
                _updateJustCurrentLine = true;

                char deletedChar = _textLineWriter.DoBackspaceOneChar();
                if (deletedChar == '\0')
                {
                    //end of current line 
                    if (!IsOnFirstLine)
                    {
                        CurrentLineNumber--;
                        DoEnd();
                        _commandHistoryList.AddDocAction(
                            new DocActionJoinWithNextLine(
                                _textLineWriter.LineNumber, _textLineWriter.CharIndex));
                        JoinWithNextLine();
                    }
                    NotifyContentSizeChanged();
#if DEBUG
                    if (dbugEnableTextManRecorder) _dbugActivityRecorder.EndContext();
#endif
                    return false;
                }
                else
                {
                    _commandHistoryList.AddDocAction(
                            new DocActionDeleteChar(
                                deletedChar, _textLineWriter.LineNumber, _textLineWriter.CharIndex));
                    NotifyContentSizeChanged();
#if DEBUG
                    if (dbugEnableTextManRecorder) _dbugActivityRecorder.EndContext();
#endif
                    return true;
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
            _textLineWriter.SetCurrentCharIndexToEnd();
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

            _textLineWriter.SetCurrentCharIndexToBegin();
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
        }
    }
}