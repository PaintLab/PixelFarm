//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{


    partial class TextLineBox
    {
        public static void InnerDoJoinWithNextLine(TextLineBox line)
        {
            line.JoinWithNextLine();
        }
        void JoinWithNextLine()
        {
            if (!IsLastLine)
            {
                TextLineBox lowerLine = _textFlowLayer.GetTextLine(_currentLineNumber + 1);
                this.LocalSuspendLineReArrange();
                int cx = 0;
                Run lastTextRun = (Run)this.LastRun;
                if (lastTextRun != null)
                {
                    cx = lastTextRun.Right;
                }

                foreach (Run r in lowerLine._runs)
                {
                    this.AddLast(r);
                    Run.DirectSetLocation(r, cx, 0);
                    cx += r.Width;
                }
                this.LocalResumeLineReArrange();
                this.EndWithLineBreak = lowerLine.EndWithLineBreak;
                _textFlowLayer.Remove(lowerLine._currentLineNumber);
            }
        }

        public void Copy(TextRangeCopy output)
        {
            LinkedListNode<Run> curNode = this.First;
            while (curNode != null)
            {
                output.AppendRun(curNode.Value);
                curNode = curNode.Next;
            }
        }
        public void Copy(VisualSelectionRange selectionRange, TextRangeCopy output)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.Run != null)
            {
                if (startPoint.Run == endPoint.Run)
                {
                    CopyRun elem =
                      startPoint.Run.Copy(
                        startPoint.RunLocalSelectedIndex,
                        endPoint.LineCharIndex - startPoint.LineCharIndex);
                    if (elem != null)
                    {
                        output.AppendRun(elem);
                    }
                }
                else
                {
                    TextLineBox startLine = null;
                    TextLineBox stopLine = null;
                    if (startPoint.LineId == _currentLineNumber)
                    {
                        startLine = this;
                    }
                    else
                    {
                        startLine = _textFlowLayer.GetTextLine(startPoint.LineId);
                    }
                    if (endPoint.LineId == _currentLineNumber)
                    {
                        stopLine = this;
                    }
                    else
                    {
                        stopLine = _textFlowLayer.GetTextLine(endPoint.LineId);
                    }
                    if (startLine == stopLine)
                    {
                        CopyRun rightPart = startPoint.Run.Copy(startPoint.RunLocalSelectedIndex);
                        if (rightPart != null)
                        {
                            output.AppendRun(rightPart);
                        }
                        if (startPoint.Run.NextRun != endPoint.Run)
                        {
                            foreach (Run run in _textFlowLayer.TextRunForward(
                                startPoint.Run.NextRun,
                                endPoint.Run.PrevRun))
                            {
                                output.AppendRun(run);
                            }
                        }

                        CopyRun leftPart = endPoint.Run.LeftCopy(endPoint.RunLocalSelectedIndex);
                        if (leftPart != null)
                        {
                            output.AppendRun(leftPart);
                        }
                    }
                    else
                    {
                        int startLineId = startPoint.LineId;
                        int stopLineId = endPoint.LineId;
                        startLine.RightCopy(startPoint, output);
                        for (int i = startLineId + 1; i < stopLineId; i++)
                        {
                            //begine new line
                            output.AppendNewLine();
                            TextLineBox line = _textFlowLayer.GetTextLine(i);
                            line.Copy(output);
                        }
                        if (endPoint.LineCharIndex > -1)
                        {
                            output.AppendNewLine();
                            stopLine.LeftCopy(endPoint, output);
                        }
                    }
                }
            }
            else
            {
                TextLineBox startLine = null;
                TextLineBox stopLine = null;
                if (startPoint.LineId == _currentLineNumber)
                {
                    startLine = this;
                }
                else
                {
                    startLine = _textFlowLayer.GetTextLine(startPoint.LineId);
                }

                if (endPoint.LineId == _currentLineNumber)
                {
                    stopLine = this;
                }
                else
                {
                    stopLine = _textFlowLayer.GetTextLine(endPoint.LineId);
                }


                if (startLine == stopLine)
                {
                    if (startPoint.LineCharIndex == -1)
                    {
                        foreach (Run t in _textFlowLayer.TextRunForward(
                            startPoint.Run,
                            endPoint.Run.PrevRun))
                        {
                            output.AppendRun(t);
                        }
                        CopyRun postCutTextRun = endPoint.Run.Copy(endPoint.RunLocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AppendRun(postCutTextRun);
                        }
                    }
                    else
                    {
                        CopyRun rightPart = startPoint.Run.Copy(startPoint.RunLocalSelectedIndex + 1);
                        if (rightPart != null)
                        {
                            output.AppendRun(rightPart);
                        }

                        foreach (Run t in _textFlowLayer.TextRunForward(
                            startPoint.Run.NextRun,
                            endPoint.Run.PrevRun))
                        {
                            output.AppendRun(t.CreateCopy());
                        }

                        CopyRun leftPart = endPoint.Run.LeftCopy(startPoint.RunLocalSelectedIndex);
                        if (leftPart != null)
                        {
                            output.AppendRun(leftPart);
                        }
                    }
                }
                else
                {
                    int startLineId = startPoint.LineId;
                    int stopLineId = endPoint.LineId;
                    startLine.RightCopy(startPoint, output);
                    for (int i = startLineId + 1; i < stopLineId; i++)
                    {
                        output.AppendNewLine();
                        TextLineBox line = _textFlowLayer.GetTextLine(i);
                        line.Copy(output);
                    }
                    stopLine.LeftCopy(endPoint, output);
                }
            }
        }



        internal void Remove(VisualSelectionRange selectionRange)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.Run != null)
            {
                if (startPoint.Run == endPoint.Run)
                {
                    Run removedRun = startPoint.Run;
                    Run.InnerRemove(removedRun,
                                    startPoint.RunLocalSelectedIndex,
                                    endPoint.LineCharIndex - startPoint.LineCharIndex, false);
                    if (removedRun.CharacterCount == 0)
                    {
                        if (startPoint.LineId == _currentLineNumber)
                        {
                            this.Remove(removedRun);
                        }
                        else
                        {
                            TextLineBox line = _textFlowLayer.GetTextLine(startPoint.LineId);
                            line.Remove(removedRun);
                        }
                    }
                }
                else
                {
                    EditableVisualPointInfo newStartPoint = null;
                    EditableVisualPointInfo newStopPoint = null;
                    TextLineBox startLine = null;
                    TextLineBox stopLine = null;
                    if (startPoint.LineId == _currentLineNumber)
                    {
                        startLine = this;
                    }
                    else
                    {
                        startLine = _textFlowLayer.GetTextLine(startPoint.LineId);
                    }
                    newStartPoint = startLine.Split(startPoint);
                    if (endPoint.LineId == _currentLineNumber)
                    {
                        stopLine = this;
                    }
                    else
                    {
                        stopLine = _textFlowLayer.GetTextLine(endPoint.LineId);
                    }

                    newStopPoint = stopLine.Split(endPoint);
                    if (startLine == stopLine)
                    {
                        if (newStartPoint.Run != null)
                        {
                            LinkedList<Run> tobeRemoveRuns = new LinkedList<Run>();
                            if (newStartPoint.LineCharIndex == 0)
                            {
                                foreach (Run t in _textFlowLayer.TextRunForward(
                                     newStartPoint.Run,
                                     newStopPoint.Run))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            else
                            {
                                foreach (Run t in _textFlowLayer.TextRunForward(
                                     newStartPoint.Run.NextRun,
                                     newStopPoint.Run))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            startLine.LocalSuspendLineReArrange();
                            foreach (Run t in tobeRemoveRuns)
                            {
                                startLine.Remove(t);
                            }
                            startLine.LocalResumeLineReArrange();
                        }
                        else
                        {
                            //this may be the blank line
                            startLine.Clear();
#if DEBUG
                            //TODO: review here again
                            //System.Diagnostics.Debug.WriteLine("EditableTextLine_adv1");
#endif
                        }
                    }
                    else
                    {
                        int startLineId = newStartPoint.LineId;
                        int stopLineId = newStopPoint.LineId;
                        if (newStopPoint.LineCharIndex > 0)
                        {
                            stopLine.RemoveLeft(newStopPoint.Run);
                        }
                        for (int i = stopLineId - 1; i > startLineId; i--)
                        {
                            TextLineBox line = _textFlowLayer.GetTextLine(i);
                            line.Clear();
                            line.JoinWithNextLine();
                        }
                        if (newStartPoint.LineCharIndex == 0)
                        {
                            startLine.RemoveRight(newStartPoint.Run);
                        }
                        else
                        {
                            Run nextRun = (newStartPoint.Run).NextRun;
                            if (nextRun != null)
                            {
                                startLine.RemoveRight(nextRun);
                            }
                        }
                        startLine.JoinWithNextLine();
                    }
                }
            }
            else
            {
                VisualPointInfo newStartPoint = null;
                VisualPointInfo newStopPoint = null;
                TextLineBox startLine = null;
                TextLineBox stopLine = null;
                if (startPoint.LineId == _currentLineNumber)
                {
                    startLine = this;
                }
                else
                {
                    startLine = _textFlowLayer.GetTextLine(startPoint.LineId);
                }
                newStartPoint = startLine.Split(startPoint);
                if (endPoint.LineId == _currentLineNumber)
                {
                    stopLine = this;
                }
                else
                {
                    stopLine = _textFlowLayer.GetTextLine(endPoint.LineId);
                }
                newStopPoint = stopLine.Split(endPoint);
                if (startLine == stopLine)
                {
                    if (newStartPoint.Run != null)
                    {
                        LinkedList<Run> tobeRemoveRuns = new LinkedList<Run>();
                        if (newStartPoint.LineCharIndex == -1)
                        {
                            foreach (Run t in _textFlowLayer.TextRunForward(
                                 newStartPoint.Run,
                                 newStopPoint.Run))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        else
                        {
                            foreach (Run t in _textFlowLayer.TextRunForward(
                                newStartPoint.Run.NextRun,
                                newStopPoint.Run))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        foreach (Run t in tobeRemoveRuns)
                        {
                            startLine.Remove(t);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    int startLineId = newStartPoint.LineId;
                    int stopLineId = newStopPoint.LineId;
                    if (newStopPoint.LineCharIndex > -1)
                    {
                        stopLine.RemoveLeft(newStopPoint.Run);
                    }
                    for (int i = stopLineId - 1; i > startLineId; i--)
                    {
                        TextLineBox line = _textFlowLayer.GetTextLine(i);
                        line.Clear();
                        line.JoinWithNextLine();
                    }
                    if (newStartPoint.LineCharIndex == -1)
                    {
                        //TODO: review here again
                        //at this point newStartPoint.TextRun should always null
                        if (newStartPoint.Run != null)
                        {
                            startLine.RemoveRight(newStartPoint.Run);
                        }
                    }
                    else
                    {
                        //at this point newStartPoint.TextRun should always null
                        //TODO newStartPoint.TextRun == null???
                        if (newStartPoint.Run != null)
                        {
                            Run nextRun = newStartPoint.Run.NextRun;
                            if (nextRun != null)
                            {
                                startLine.RemoveRight(nextRun);
                            }
                        }

                    }
                    startLine.JoinWithNextLine();
                }
            }
        }
        Size MeasureCopyRunLength(CopyRun copyRun)
        {

            ITextService txServices = TextService;

            char[] mybuffer = copyRun.RawContent;
            if (txServices.SupportsWordBreak)
            {
                var textBufferSpan = new TextBufferSpan(mybuffer);

                ILineSegmentList lineSegs = txServices.BreakToLineSegments(textBufferSpan);

                var result = new TextSpanMeasureResult();
                result.outputXAdvances = new int[mybuffer.Length];

                txServices.CalculateUserCharGlyphAdvancePos(textBufferSpan, lineSegs,
                    DefaultRunStyle.ReqFont,
                    ref result);

                return new Size(result.outputTotalW, result.lineHeight);
            }
            else
            {


                var textBufferSpan = new TextBufferSpan(mybuffer);

                var result = new TextSpanMeasureResult();
                result.outputXAdvances = new int[mybuffer.Length];

                txServices.CalculateUserCharGlyphAdvancePos(textBufferSpan,
                    DefaultRunStyle.ReqFont,
                    ref result);

                return new Size(result.outputTotalW, result.lineHeight);
            }

        }
        internal SelectionRangeInfo Split(VisualSelectionRange selectionRange)
        {
            selectionRange.SwapIfUnOrder();

            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;

            if (startPoint.Run == endPoint.Run)
            {
                Run toBeCutTextRun = startPoint.Run;

                CopyRun leftPart = toBeCutTextRun.LeftCopy(startPoint.RunLocalSelectedIndex);
                CopyRun middlePart = toBeCutTextRun.Copy(startPoint.RunLocalSelectedIndex, endPoint.LineCharIndex - startPoint.LineCharIndex);
                CopyRun rightPart = toBeCutTextRun.Copy(endPoint.RunLocalSelectedIndex);

                EditableVisualPointInfo newStartRangePointInfo = null;
                EditableVisualPointInfo newEndRangePointInfo = null;
                TextLineBox line = this;

                if (startPoint.LineId != _currentLineNumber)
                {
                    line = _textFlowLayer.GetTextLine(startPoint.LineId);
                }

                line.LocalSuspendLineReArrange();

                if (leftPart != null)
                {
                    Run leftRun = line.AddBefore(toBeCutTextRun, leftPart);
                    newStartRangePointInfo = CreateTextPointInfo(
                        startPoint.LineId, startPoint.LineCharIndex, startPoint.X,
                        leftRun,
                        startPoint.TextRunCharOffset, startPoint.TextRunPixelOffset);
                }
                else
                {
                    //no left part, 
                    //so we connect to prev text run

                    Run prevTxtRun = startPoint.Run.PrevRun;
                    if (prevTxtRun != null)
                    {
                        newStartRangePointInfo = CreateTextPointInfo(
                            startPoint.LineId, startPoint.LineCharIndex, startPoint.X,
                            prevTxtRun,
                            startPoint.TextRunCharOffset - leftPart.CharacterCount,
                            startPoint.TextRunPixelOffset - prevTxtRun.Width);
                    }
                    else
                    {
                        //no prev run, we are at the begining of the line
                        newStartRangePointInfo = CreateTextPointInfo(
                            startPoint.LineId,
                            startPoint.LineCharIndex,
                            0,
                            null,
                            0, 0);
                    }
                }

                if (rightPart != null)
                {
                    Run rightRun = line.AddAfter(toBeCutTextRun, rightPart);
                    newEndRangePointInfo =
                        CreateTextPointInfo(
                            endPoint.LineId,
                            endPoint.LineCharIndex,
                            endPoint.X,
                             null,///??
                            startPoint.TextRunCharOffset + middlePart.CharacterCount,
                            startPoint.TextRunPixelOffset + MeasureCopyRunLength(middlePart).Width);
                }
                else
                {
                    Run nextTxtRun = endPoint.Run.NextRun;
                    if (nextTxtRun != null)
                    {
                        newEndRangePointInfo = CreateTextPointInfo(
                            endPoint.LineId,
                            endPoint.LineCharIndex,
                            endPoint.X,
                              null,///??
                            endPoint.TextRunPixelOffset + endPoint.Run.CharacterCount,
                            endPoint.TextRunPixelOffset + endPoint.Run.Width);
                    }
                    else
                    {
                        newEndRangePointInfo = CreateTextPointInfo(
                            endPoint.LineId,
                            endPoint.LineCharIndex,
                            endPoint.X,
                            null,
                            endPoint.TextRunCharOffset,
                            endPoint.TextRunPixelOffset);
                    }
                }

                if (middlePart != null)
                {
                    line.AddAfter(toBeCutTextRun, middlePart);
                }
                else
                {
                    throw new NotSupportedException();
                }
                line.Remove(toBeCutTextRun);
                line.LocalResumeLineReArrange();
                return new SelectionRangeInfo(newStartRangePointInfo, newEndRangePointInfo);
            }
            else
            {
                TextLineBox workingLine = this;
                if (startPoint.LineId != _currentLineNumber)
                {
                    workingLine = _textFlowLayer.GetTextLine(startPoint.LineId);
                }
                EditableVisualPointInfo newStartPoint = workingLine.Split(startPoint);
                workingLine = this;
                if (endPoint.LineId != _currentLineNumber)
                {
                    workingLine = _textFlowLayer.GetTextLine(endPoint.LineId);
                }

                EditableVisualPointInfo newEndPoint = workingLine.Split(endPoint);
                return new SelectionRangeInfo(newStartPoint, newEndPoint);
            }
        }

        internal EditableVisualPointInfo Split(EditableVisualPointInfo pointInfo)
        {
            if (pointInfo.LineId != _currentLineNumber)
            {
                throw new NotSupportedException();
            }

            Run tobeCutRun = pointInfo.Run;
            if (tobeCutRun == null)
            {
                return CreateTextPointInfo(
                       pointInfo.LineId,
                       pointInfo.LineCharIndex,
                       pointInfo.X,
                       null,
                       pointInfo.TextRunCharOffset,
                       pointInfo.TextRunPixelOffset);
            }

            this.LocalSuspendLineReArrange();
            EditableVisualPointInfo result = null;

            CopyRun leftPart = tobeCutRun.LeftCopy(pointInfo.RunLocalSelectedIndex);
            CopyRun rightPart = tobeCutRun.Copy(pointInfo.RunLocalSelectedIndex);

            if (leftPart != null)
            {
                Run leftRun = AddBefore(tobeCutRun, leftPart);
                if (rightPart != null)
                {
                    this.AddAfter(tobeCutRun, rightPart);
                }

                result = CreateTextPointInfo(
                    pointInfo.LineId,
                    pointInfo.LineCharIndex,
                    pointInfo.X,
                    leftRun,
                    pointInfo.TextRunCharOffset,
                    pointInfo.TextRunPixelOffset);
            }
            else
            {
                if (rightPart != null)
                {
                    this.AddAfter(tobeCutRun, rightPart);
                }


                Run infoTextRun = null;
                if (IsSingleLine)
                {
                    if (tobeCutRun.PrevRun != null)
                    {
                        infoTextRun = tobeCutRun.PrevRun;
                    }
                    else
                    {
                        infoTextRun = tobeCutRun.NextRun;
                    }
                }
                else
                {
                    if (IsFirstLine)
                    {
                        if (tobeCutRun.PrevRun != null)
                        {
                            infoTextRun = tobeCutRun.PrevRun;
                        }
                        else
                        {
                            if (tobeCutRun.NextRun == null)
                            {
                                infoTextRun = null;
                            }
                            else
                            {
                                infoTextRun = tobeCutRun.NextRun;
                            }
                        }
                    }
                    else if (IsLastLine)
                    {
                        if (tobeCutRun.PrevRun != null)
                        {
                            infoTextRun = tobeCutRun.PrevRun;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (tobeCutRun.NextRun != null)
                        {
                            infoTextRun = tobeCutRun.NextRun;
                        }
                        else
                        {
                            infoTextRun = null;
                        }
                    }
                }
                result = CreateTextPointInfo(
                    pointInfo.LineId,
                    pointInfo.LineCharIndex,
                    pointInfo.X,
                    infoTextRun,
                    pointInfo.TextRunCharOffset,
                    pointInfo.TextRunPixelOffset);
            }

            this.Remove(tobeCutRun);
            this.LocalResumeLineReArrange();
            return result;
        }
        void RightCopy(VisualPointInfo pointInfo, TextRangeCopy output)
        {
            if (pointInfo.LineId != _currentLineNumber)
            {
                throw new NotSupportedException();
            }
            Run tobeCutRun = pointInfo.Run;
            if (tobeCutRun == null)
            {
                return;
            }
            CopyRun rightPart = tobeCutRun.Copy(pointInfo.RunLocalSelectedIndex);
            if (rightPart != null)
            {
                output.AppendRun(rightPart);
            }
            foreach (Run run in GetRunIterForward(tobeCutRun.NextRun, this.LastRun))
            {
                output.AppendRun(run);
            }
        }

        void LeftCopy(VisualPointInfo pointInfo, TextRangeCopy output)
        {
            if (pointInfo.LineId != _currentLineNumber)
            {
                throw new NotSupportedException();
            }
            Run tobeCutRun = pointInfo.Run;
            if (tobeCutRun == null)
            {
                return;
            }

            foreach (Run run in _runs)
            {
                if (run != tobeCutRun)
                {
                    output.AppendRun(run);
                }
                else
                {
                    break;
                }
            }
            CopyRun leftPart = tobeCutRun.LeftCopy(pointInfo.RunLocalSelectedIndex);
            if (leftPart != null)
            {
                output.AppendRun(leftPart);
            }
        }

        EditableVisualPointInfo CreateTextPointInfo(
            int lineId, int lineCharIndex, int caretPixelX,
            Run onRun,
            int textRunCharOffset, int textRunPixelOffset)
        {
            EditableVisualPointInfo textPointInfo = new EditableVisualPointInfo(this, lineCharIndex, onRun);
            textPointInfo.SetAdditionVisualInfo(textRunCharOffset, caretPixelX, textRunPixelOffset);
            return textPointInfo;
        }

        public VisualPointInfo GetTextPointInfoFromCaretPoint(int caretX)
        {
            int accTextRunWidth = 0;
            int accTextRunCharCount = 0;
            Run lastestTextRun = null;
            foreach (Run t in _runs)
            {
                lastestTextRun = t;
                int thisTextRunWidth = t.Width;
                if (accTextRunWidth + thisTextRunWidth > caretX)
                {
                    CharLocation localPointInfo = t.GetCharacterFromPixelOffset(caretX - thisTextRunWidth);
                    var pointInfo = new EditableVisualPointInfo(this, accTextRunCharCount + localPointInfo.RunCharIndex, t);
                    pointInfo.SetAdditionVisualInfo(accTextRunCharCount, caretX, accTextRunWidth);
                    return pointInfo;
                }
                else
                {
                    accTextRunWidth += thisTextRunWidth;
                    accTextRunCharCount += t.CharacterCount;
                }
            }
            if (lastestTextRun != null)
            {
                return null;
            }
            else
            {

                EditableVisualPointInfo pInfo = new EditableVisualPointInfo(this, -1, null);
                pInfo.SetAdditionVisualInfo(accTextRunCharCount, caretX, accTextRunWidth);
                return pInfo;
            }
        }


        public EditableVisualPointInfo GetTextPointInfoFromCharIndex(int charIndex)
        {
            int limit = CharCount - 1;
            if (charIndex > limit)
            {
                charIndex = limit;
            }


            int rCharOffset = 0;
            int rPixelOffset = 0;
            Run lastestRun = null;
            EditableVisualPointInfo textPointInfo = null;
            foreach (Run r in _runs)
            {
                lastestRun = r;
                int thisCharCount = lastestRun.CharacterCount;
                if (thisCharCount + rCharOffset > charIndex)
                {
                    int localCharOffset = charIndex - rCharOffset;
                    int pixelOffset = lastestRun.GetRunWidth(localCharOffset);
                    textPointInfo = new EditableVisualPointInfo(this, charIndex, lastestRun);
                    textPointInfo.SetAdditionVisualInfo(localCharOffset, rPixelOffset + pixelOffset, rPixelOffset);
                    return textPointInfo;
                }
                else
                {
                    rCharOffset += thisCharCount;
                    rPixelOffset += r.Width;
                }
            }


            textPointInfo = new EditableVisualPointInfo(this, charIndex, lastestRun);
            textPointInfo.SetAdditionVisualInfo(rCharOffset - lastestRun.CharacterCount, rPixelOffset, rPixelOffset - lastestRun.Width);
            return textPointInfo;
        }

    }
}