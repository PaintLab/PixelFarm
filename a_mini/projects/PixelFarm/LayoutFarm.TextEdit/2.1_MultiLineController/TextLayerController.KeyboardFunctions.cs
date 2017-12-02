//Apache2, 2014-2017, WinterDev

using System;
using System.Globalization;

namespace LayoutFarm.Text
{
    partial class InternalTextLayerController
    {
        public VisualSelectionRangeSnapShot DoDelete()
        {
            //recursive
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::DoDelete");
                dbugTextManRecorder.BeginContext();
            }
#endif

            VisualSelectionRangeSnapShot removedRange = this.RemoveSelectedText();
            if (removedRange.IsEmpty())
            {
                updateJustCurrentLine = true;
                char deletedChar = textLineWriter.DoDeleteOneChar();
                //some language

                if (deletedChar == '\0')
                {
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
                        var unicodeCatg = char.GetUnicodeCategory(nextChar);
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
                                return DoDelete();
                            default:
                                break;
                        }
                        
                        
                    }
                }
            }
#if DEBUG
            if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
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
                dbugTextManRecorder.WriteInfo("TxLMan::DoBackSpace");
                dbugTextManRecorder.BeginContext();
            }
#endif

            VisualSelectionRangeSnapShot removeSelRange = this.RemoveSelectedText();
            if (!removeSelRange.IsEmpty())
            {
                CancelSelect();
#if DEBUG
                if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
#endif
                return true;
            }
            else
            {
                updateJustCurrentLine = true;
                char deletedChar = textLineWriter.DoBackspaceOneChar();
                if (deletedChar == '\0')
                {
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
                    if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
#endif
                    return false;
                }
                else
                {
                    commandHistory.AddDocAction(
                            new DocActionDeleteChar(
                                deletedChar, textLineWriter.LineNumber, textLineWriter.CharIndex));
#if DEBUG
                    if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
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
                dbugTextManRecorder.WriteInfo("TxLMan::DoEnd");
                dbugTextManRecorder.BeginContext();
            }
#endif
            textLineWriter.CharIndex = textLineWriter.CharCount - 1;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.EndContext();
            }
#endif
        }
        public void DoHome()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::DoHome");
                dbugTextManRecorder.BeginContext();
            }
#endif
            textLineWriter.CharIndex = -1;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.EndContext();
            }
#endif
        }
    }
}