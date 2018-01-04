//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    class TextLineWriter : TextLineReader
    {
        public TextLineWriter(EditableTextFlowLayer textLayer)
            : base(textLayer)
        {
        }
        public TextSpanStyle CurrentSpanStyle
        {
            get
            {
                return this.TextLayer.CurrentTextSpanStyle;
            }
        }
        public void Reload(IEnumerable<EditableRun> runs)
        {
            this.TextLayer.Reload(runs);
        }
        public void Clear()
        {
            this.MoveToLine(0);
            CurrentLine.Clear();
            EnsureCurrentTextRun();
        }
        public void EnsureCurrentTextRun(int index)
        {
            var run = CurrentTextRun;
            if (run == null || !run.HasParent)
            {

                SetCurrentCharIndex(-1);
                if (index != -1)
                {
                    int limit = CurrentLine.CharCount - 1;
                    if (index > limit)
                    {
                        index = limit;
                    }
                    SetCurrentCharIndex(index);
                }
            }
        }
        public void EnsureCurrentTextRun()
        {
            EnsureCurrentTextRun(ProperCharIndex);
        }
        public void RemoveSelectedTextRuns(VisualSelectionRange selectionRange)
        {
            int precutIndex = selectionRange.StartPoint.LineCharIndex;
            CurrentLine.Remove(selectionRange);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(precutIndex);
        }

        public void ReplaceCurrentLine(IEnumerable<EditableRun> textRuns)
        {
            int currentCharIndex = ProperCharIndex;
            CurrentLine.ReplaceAll(textRuns);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(currentCharIndex);
        }
        public void JoinWithNextLine()
        {
            EditableTextLine.InnerDoJoinWithNextLine(this.CurrentLine);
            EnsureCurrentTextRun();
        }
        char BackSpaceOneChar()
        {
            if (CurrentTextRun == null)
            {
                return '\0';
            }
            else
            {
                if (ProperCharIndex < 0)
                {
                    return '\0';
                }

                char toBeRemovedChar = CurrentChar;
                EditableRun removingTextRun = CurrentTextRun;
                int removeIndex = CurrentTextRunCharIndex;
                SetCurrentCharStepLeft();
                EditableRun.InnerRemove(removingTextRun, removeIndex, 1, false);
                if (removingTextRun.CharacterCount == 0)
                {
                    CurrentLine.Remove(removingTextRun);
                    EnsureCurrentTextRun();
                }
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();
                return toBeRemovedChar;
            }
        }
        public EditableRun GetCurrentTextRun()
        {
            if (CurrentLine.IsBlankLine)
            {
                return null;
            }
            else
            {
                return CurrentTextRun;
            }
        }

        public void AddCharacter(char c)
        {
            if (CurrentLine.IsBlankLine)
            {
                //1. new 
                EditableRun t = new EditableTextRun(this.Root,
                    c,
                    this.CurrentSpanStyle);
                var owner = this.FlowLayer.OwnerRenderElement;
                CurrentLine.AddLast(t);
                SetCurrentTextRun(t);
            }
            else
            {
                EditableRun cRun = CurrentTextRun;
                if (cRun != null)
                {
                    if (cRun.IsInsertable)
                    {
                        cRun.InsertAfter(CurrentTextRunCharIndex, c);
                    }
                    else
                    {
                        AddTextSpan(new EditableTextRun(this.Root, c, this.CurrentSpanStyle));
                        return;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            SetCurrentCharStepRight();
        }
        public void AddTextSpan(EditableRun textRun)
        {
            if (CurrentLine.IsBlankLine)
            {
                CurrentLine.AddLast(textRun);
                SetCurrentTextRun(textRun);
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();

                SetCurrentCharIndex(ProperCharIndex + textRun.CharacterCount);
            }
            else
            {
                if (CurrentTextRun != null)
                {
                    VisualPointInfo newPointInfo = CurrentLine.Split(GetCurrentPointInfo());
                    if (newPointInfo.IsOnTheBeginOfLine)
                    {
                        CurrentLine.AddBefore((EditableRun)newPointInfo.TextRun, textRun);
                    }
                    else
                    {
                        CurrentLine.AddAfter((EditableRun)newPointInfo.TextRun, textRun);
                    }
                    CurrentLine.TextLineReCalculateActualLineSize();
                    CurrentLine.RefreshInlineArrange();
                    EnsureCurrentTextRun(ProperCharIndex + textRun.CharacterCount);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        public void ReplaceAllLineContent(EditableRun[] runs)
        {
            int charIndex = ProperCharIndex;
            CurrentLine.Clear();
            int j = runs.Length;
            for (int i = 0; i < j; ++i)
            {
                CurrentLine.AddLast(runs[i]);
            }

            EnsureCurrentTextRun(charIndex);
        }

        public char DoBackspaceOneChar()
        {
            //simulate backspace keystroke
            return BackSpaceOneChar();
        }
        public char DoDeleteOneChar()
        {
            if (ProperCharIndex < CurrentLine.CharCount - 1)
            {
                //simulate backspace keystroke

                SetCurrentCharStepRight();
                return BackSpaceOneChar();
            }
            else
            {
                return '\0';
            }
        }

        public void SplitToNewLine()
        {

            EditableRun lineBreakRun = new EditableTextRun(this.Root, '\n', this.CurrentSpanStyle);
            EditableRun currentRun = CurrentTextRun;
            if (CurrentLine.IsBlankLine)
            {
                CurrentLine.AddLast(lineBreakRun);
            }
            else
            {
                if (ProperCharIndex == -1)
                {
                    CurrentLine.AddFirst(lineBreakRun);
                    SetCurrentTextRun(null);
                }
                else
                {
                    EditableRun rightSplitedPart = EditableRun.InnerRemove(currentRun,
                        CurrentTextRunCharIndex + 1, true);
                    if (rightSplitedPart != null)
                    {
                        CurrentLine.AddAfter(currentRun, rightSplitedPart);
                    }
                    CurrentLine.AddAfter(currentRun, lineBreakRun);
                    if (currentRun.CharacterCount == 0)
                    {
                        CurrentLine.Remove(currentRun);
                    }
                }
            }


            this.TextLayer.TopDownReCalculateContentSize();
            EnsureCurrentTextRun();
        }
        public EditableVisualPointInfo[] SplitSelectedText(VisualSelectionRange selectionRange)
        {
            EditableVisualPointInfo[] newPoints = CurrentLine.Split(selectionRange);
            EnsureCurrentTextRun();
            return newPoints;
        }
    }

    abstract class TextLineReader
    {
#if DEBUG
        static int dbugTotalId;
        int dbug_MyId;
        public debugActivityRecorder dbugTextManRecorder;
#endif

        EditableTextFlowLayer visualFlowLayer;
        EditableTextLine currentLine;
        int currentLineY = 0;
        EditableRun currentTextRun;

        int caretXPos = 0;
        /// <summary>
        /// character offset of this run, start from start line, this value is reset for every current run
        /// </summary>
        int rCharOffset = 0;
        /// <summary>
        /// pixel offset of this run, start from the begin of this line, this value is reset for every current run
        /// </summary>
        int rPixelOffset = 0;
        public TextLineReader(EditableTextFlowLayer flowlayer)
        {

#if DEBUG
            this.dbug_MyId = dbugTotalId;
            dbugTotalId++;
#endif

            this.visualFlowLayer = flowlayer;
            flowlayer.Reflow += new EventHandler(flowlayer_Reflow);
            currentLine = flowlayer.GetTextLine(0);
            if (currentLine.FirstRun != null)
            {
                currentTextRun = currentLine.FirstRun;
            }
        }

#if DEBUG
        int _i_charIndex;
        int charIndex
        {
            get { return _i_charIndex; }
            set
            {
                _i_charIndex = value;
            }
        }
#else
         int charIndex;
#endif

        protected RootGraphic Root
        {
            get { return this.visualFlowLayer.Root; }
        }
        public EditableTextFlowLayer FlowLayer
        {
            get
            {
                return this.visualFlowLayer;
            }
        }
        void flowlayer_Reflow(object sender, EventArgs e)
        {
            int prevCharIndex = charIndex;
            this.SetCurrentCharIndex(-1);
            this.SetCurrentCharIndex(prevCharIndex);
        }
        protected EditableTextLine CurrentLine
        {
            get
            {
                return currentLine;
            }
        }

        protected EditableRun CurrentTextRun
        {
            get
            {
                return currentTextRun;
            }
        }
        protected void SetCurrentTextRun(EditableRun r)
        {
            currentTextRun = r;
        }
        bool MoveToPreviousTextRun()
        {
#if DEBUG
            if (currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif
            if (currentTextRun.PrevTextRun != null)
            {
                currentTextRun = currentTextRun.PrevTextRun;
                rCharOffset -= currentTextRun.CharacterCount;
                rPixelOffset -= currentTextRun.Width;
                charIndex = rCharOffset + currentTextRun.CharacterCount - 1;
                caretXPos = rPixelOffset + currentTextRun.Width;
                return true;
            }
            return false;
        }

        bool MoveToNextTextRun()
        {
#if DEBUG
            if (currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif


            EditableRun nextTextRun = currentTextRun.NextTextRun;
            if (nextTextRun != null && !nextTextRun.IsLineBreak)
            {
                rCharOffset += currentTextRun.CharacterCount;
                rPixelOffset += currentTextRun.Width;
                currentTextRun = nextTextRun;
                charIndex = rCharOffset;
                caretXPos = rPixelOffset + currentTextRun.GetRunWidth(0);
                return true;
            }
            return false;
        }

        public void MoveToLine(int lineNumber)
        {
            currentLine = visualFlowLayer.GetTextLine(lineNumber);
            currentLineY = currentLine.Top;
            currentTextRun = (EditableRun)currentLine.FirstRun;
            rCharOffset = 0;
            rPixelOffset = 0;
            charIndex = -1;
            caretXPos = 0;
        }
        public void CopyContentToStrignBuilder(StringBuilder stBuilder)
        {
            visualFlowLayer.CopyContentToStringBuilder(stBuilder);
        }

        public char NextChar
        {
            get
            {
                if (currentTextRun != null)
                {

                    if (charIndex < 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (charIndex == rCharOffset + currentTextRun.CharacterCount - 1)
                    {
                        if (currentTextRun.NextTextRun != null)
                        {
                            return (currentTextRun.NextTextRun).GetChar(0);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return currentTextRun.GetChar(charIndex - rCharOffset + 1);
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        /// <summary>
        /// next char width in this line
        /// </summary>
        public int NextCharWidth
        {
            get
            {
                if (currentTextRun != null)
                {
                    if (charIndex < 0)
                    {
                        return 0;
                    }
                    if (charIndex == rCharOffset + currentTextRun.CharacterCount - 1)
                    {
                        //-----
                        //this is on the last of current run
                        //if we have next run, just get run of next width
                        //-----
                        EditableRun nextRun = currentTextRun.NextTextRun;
                        if (nextRun != null)
                        {
                            return nextRun.GetRunWidth(0);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return currentTextRun.GetRunWidth(charIndex - rCharOffset + 1) -
                                currentTextRun.GetRunWidth(charIndex - rCharOffset);
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
#if DEBUG
            if (currentTextRun != null && !currentTextRun.HasParent)
            {
                throw new NotSupportedException();
            }
            if (currentTextRun == null)
            {
            }
#endif


            EditableVisualPointInfo textPointInfo =
                new EditableVisualPointInfo(currentLine, charIndex);
            textPointInfo.SetAdditionVisualInfo(currentTextRun,
                rCharOffset, caretXPos, rPixelOffset);
            return textPointInfo;
        }
        public Point CaretPosition
        {
            get
            {
                return new Point(caretXPos, currentLineY);
            }
        }

        public char CurrentChar
        {
            get
            {
                if (charIndex == -1)
                {
                    return '\0';
                }
                else
                {
                    return currentTextRun.GetChar(charIndex - rCharOffset);
                }
            }
        }
        public int CurrentTextRunCharOffset
        {
            get
            {
                return rCharOffset;
            }
        }
        public int CurrentTextRunPixelOffset
        {
            get
            {
                return rPixelOffset;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return charIndex - rCharOffset;
            }
        }
        /// <summary>
        /// try set caret x pos to nearest request value
        /// </summary>
        /// <param name="value"></param>
        public void TrySetCaretXPos(int value)
        {
            if (currentTextRun == null)
            {
                charIndex = -1;
                caretXPos = 0;
                rCharOffset = 0;
                rPixelOffset = 0;
                return;
            }
            int pixDiff = value - caretXPos;
            if (pixDiff > 0)
            {
                do
                {
                    int thisTextRunPixelLength = currentTextRun.Width;
                    if (rPixelOffset + thisTextRunPixelLength > value)
                    {
                        EditableRunCharLocation foundLocation = EditableRun.InnerGetCharacterFromPixelOffset(currentTextRun, value - rPixelOffset);
                        if (foundLocation.charIndex == -1)
                        {
                            if (!(MoveToPreviousTextRun()))
                            {
                                charIndex = -1;
                                caretXPos = 0;
                            }
                        }
                        else
                        {
                            caretXPos = rPixelOffset + foundLocation.pixelOffset; charIndex = rCharOffset + foundLocation.charIndex;
                        }
                        return;
                    }
                } while (MoveToNextTextRun());
                caretXPos = rPixelOffset + currentTextRun.Width;
                charIndex = rCharOffset + currentTextRun.CharacterCount - 1;
                return;
            }
            else if (pixDiff < 0)
            {
                do
                {
                    if (value >= rPixelOffset)
                    {
                        EditableRunCharLocation foundLocation = EditableRun.InnerGetCharacterFromPixelOffset(currentTextRun, value - rPixelOffset);
                        if (foundLocation.charIndex == -1)
                        {
                            if (!MoveToPreviousTextRun())
                            {
                                charIndex = -1;
                                caretXPos = 0;
                            }
                        }
                        else
                        {
                            caretXPos = rPixelOffset + foundLocation.pixelOffset; charIndex = rCharOffset + foundLocation.charIndex;
                        }
                        return;
                    }
                } while (MoveToPreviousTextRun());//
                caretXPos = 0;
                charIndex = -1;
                return;
            }
        }
        public int CaretXPos
        {
            get
            {
                return caretXPos;
            }

        }

        public int ProperCharIndex
        {
            get { return charIndex; }
        }
        int CharIndex
        {
            get
            {
                return charIndex;
            }
        }
        public void SetCurrentCharStepRight()
        {
            SetCurrentCharIndex(CharIndex + 1);
        }
        public void SetCurrentCharStepLeft()
        {
            SetCurrentCharIndex(CharIndex - 1);
        }
        public void SetCurrentCharIndexToEnd()
        {
            SetCurrentCharIndex(this.CharCount - 1);
        }
        public void SetCurrentCharIndexToBegin()
        {
            SetCurrentCharIndex(-1);
        }
        public void SetCurrentCharIndex(int newCharIndexPointTo)
        {

#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.WriteInfo("TextLineReader::CharIndex_set=" + newCharIndexPointTo);
                dbugTextManRecorder.BeginContext();
            }
#endif
            if (newCharIndexPointTo < -1 || newCharIndexPointTo > currentLine.CharCount - 1)
            {
                throw new NotSupportedException("index out of range");
            }


            if (newCharIndexPointTo == -1)
            {
                charIndex = -1;
                caretXPos = 0;
                rCharOffset = 0;
                rPixelOffset = 0;
                currentTextRun = currentLine.FirstRun;
            }
            else
            {
                int diff = newCharIndexPointTo - charIndex;
                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }

                    default:
                        {
                            if (diff > 0)
                            {
                                do
                                {
                                    if (rCharOffset + currentTextRun.CharacterCount > newCharIndexPointTo)
                                    {
                                        charIndex = newCharIndexPointTo;
                                        caretXPos = rPixelOffset + currentTextRun.GetRunWidth(charIndex - rCharOffset + 1);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif

                                        return;
                                    }
                                } while (MoveToNextTextRun());
                                charIndex = rCharOffset + currentTextRun.CharacterCount;
                                caretXPos = rPixelOffset + currentTextRun.Width;
                                return;
                            }
                            else
                            {
                                do
                                {
                                    if (rCharOffset - 1 < newCharIndexPointTo)
                                    {
                                        charIndex = newCharIndexPointTo;
                                        caretXPos = rPixelOffset + currentTextRun.GetRunWidth(charIndex - rCharOffset + 1);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif
                                        return;
                                    }
                                } while (MoveToPreviousTextRun());
                                charIndex = -1;
                                caretXPos = 0;
                            }
                        }
                        break;
                }
            }
#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.EndContext();
            }
#endif

        }


        public bool IsOnEndOfLine
        {
            get
            {
                return (charIndex == currentLine.CharCount - 1);
            }
        }

        internal EditableTextLine GetTextLine(int lineId)
        {
            return TextLayer.GetTextLine(lineId);
        }

        internal EditableTextLine GetTextLineAtPos(int y)
        {
            return this.TextLayer.GetTextLineAtPos(y);
        }
        public int LineCount
        {
            get
            {
                return TextLayer.LineCount;
            }
        }

        public bool HasNextLine
        {
            get
            {
                return currentLine.Next != null;
            }
        }
        public bool HasPrevLine
        {
            get
            {
                return currentLine.Prev != null;
            }
        }
        public bool IsOnStartOfLine
        {
            get
            {
                return CharIndex == -1;
            }
        }
        public int CharCount
        {
            get
            {
                return currentLine.CharCount;
            }
        }
        public void CopyLineContent(StringBuilder stBuilder)
        {
            currentLine.CopyLineContent(stBuilder);
        }
        public void CopySelectedTextRuns(VisualSelectionRange selectionRange, List<EditableRun> output)
        {
            currentLine.Copy(selectionRange, output);
        }

        public int LineNumber
        {
            get
            {
                return currentLine.LineNumber;
            }
        }
        public void MoveToNextLine()
        {
            MoveToLine(currentLine.LineNumber + 1);
        }
        public void MoveToPrevLine()
        {
            MoveToLine(currentLine.LineNumber - 1);
        }
        public Rectangle LineArea
        {
            get
            {
                return currentLine.ActualLineArea;
            }
        }
        public Rectangle ParentLineArea
        {
            get
            {
                return currentLine.ParentLineArea;
            }
        }


        internal EditableTextFlowLayer TextLayer
        {
            get
            {
                return currentLine.OwnerFlowLayer;
            }
        }
    }
    //class BackGroundTextLineWriter : TextLineWriter
    //{
    //    public BackGroundTextLineWriter(EditableTextFlowLayer visualElementLayer)
    //        : base(visualElementLayer)
    //    {
    //    }
    //}
}