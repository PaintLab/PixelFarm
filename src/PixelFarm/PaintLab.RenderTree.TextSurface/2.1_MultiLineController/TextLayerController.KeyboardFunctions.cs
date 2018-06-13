//Apache2, 2014-present, WinterDev

using System;
using System.Globalization;

namespace LayoutFarm.Text
{
    partial class InternalTextLayerController
    {

        static bool CanCaretStopOnThisChar(char c)
        {
            var unicodeCatg = char.GetUnicodeCategory(c);
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
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.DecimalDigitNumber:
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
                updateJustCurrentLine = true;
                char deletedChar = textLineWriter.DoDeleteOneChar();

                if (deletedChar == '\0')
                {
                    //end of this line
                    commandHistory.AddDocAction(
                        new DocActionJoinWithNextLine(
                            textLineWriter.LineNumber, textLineWriter.CharIndex));
                    JoinWithNextLine();
                    updateJustCurrentLine = false;
                }
                else
                {
                    commandHistory.AddDocAction(
                        new DocActionDeleteChar(
                            deletedChar, textLineWriter.LineNumber, textLineWriter.CharIndex));
                    char nextChar = textLineWriter.NextChar;

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
#if DEBUG
                if (dbugEnableTextManRecorder) _dbugActivityRecorder.EndContext();
#endif
                return true;
            }
            else
            {
                updateJustCurrentLine = true;
                char deletedChar = textLineWriter.DoBackspaceOneChar();
                if (deletedChar == '\0')
                {
                    //end of current line


                    if (!IsOnFirstLine)
                    {
                        CurrentLineNumber--;
                        DoEnd();
                        commandHistory.AddDocAction(
                            new DocActionJoinWithNextLine(
                                textLineWriter.LineNumber, textLineWriter.CharIndex));
                        JoinWithNextLine();
                    }
#if DEBUG
                    if (dbugEnableTextManRecorder) _dbugActivityRecorder.EndContext();
#endif
                    return false;
                }
                else
                {
                    commandHistory.AddDocAction(
                            new DocActionDeleteChar(
                                deletedChar, textLineWriter.LineNumber, textLineWriter.CharIndex));
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
            textLineWriter.SetCurrentCharIndexToEnd();
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

            textLineWriter.SetCurrentCharIndexToBegin();
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
        }
    }
}