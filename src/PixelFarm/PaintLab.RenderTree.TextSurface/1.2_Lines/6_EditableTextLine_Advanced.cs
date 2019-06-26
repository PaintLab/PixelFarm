//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{
    public enum RunKind : byte
    {
        Text,
        Image,
        Solid
    }

    public class CopyRun
    {
        public RunKind RunKind { get; set; }
        public char[] RawContent { get; set; }
        public TextSpanStyle SpanStyle { get; set; }
        public CopyRun() { }
        public CopyRun(string rawContent)
        {
            RawContent = rawContent.ToCharArray();
        }
        public CopyRun(char[] rawContent)
        {
            RawContent = rawContent;
        }
        public int CharacterCount
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Image:
                    case RunKind.Solid: return 1;
                    case RunKind.Text:
                        return RawContent.Length;
                    default: throw new NotSupportedException();
                }
            }
        }
        public void CopyContentToStringBuilder(StringBuilder stbuilder)
        {
            throw new NotSupportedException();
            //if (IsLineBreak)
            //{
            //    stBuilder.Append("\r\n");
            //}
            //else
            //{
            //    stBuilder.Append(_mybuffer);
            //}
        }
    }



    public class TextRangeCopy
    {
        public class TextLine
        {
            LinkedList<CopyRun> _runs = new LinkedList<CopyRun>();
            public TextLine()
            {

            }
            public IEnumerable<CopyRun> GetRunIter()
            {
                var node = _runs.First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;//**
                }

            }
            public int RunCount => _runs.Count;
            public void Append(CopyRun run) => _runs.AddLast(run);
            public void CopyContentToStringBuilder(StringBuilder stbuilder)
            {
                foreach (CopyRun run in _runs)
                {
                    stbuilder.Append(run.RawContent);
                }
            }
        }

        TextLine _currentLine;
        LinkedList<TextLine> _lines;
        public TextRangeCopy()
        {
            _currentLine = new TextLine();//create default blank lines
        }
        public bool HasSomeRuns
        {
            get
            {
                if (_lines == null)
                {
                    return _currentLine.RunCount > 0;
                }
                else
                {
                    //has more than 1 line (at least we have a line break)
                    return true;
                }
            }
        }
        public IEnumerable<TextLine> GetTextLineIter()
        {
            if (_lines == null)
            {
                yield return _currentLine;
            }
            else
            {
                var node = _lines.First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;//***
                }
            }
        }
        public void AppendNewLine()
        {
            if (_lines == null)
            {
                _lines = new LinkedList<TextLine>();
                //add current line ot the collection
                _lines.AddLast(_currentLine);
            }
            //new line
            _currentLine = new TextLine();
            _lines.AddLast(_currentLine);
        }
        public void AddRun(CopyRun copyRun)
        {
            _currentLine.Append(copyRun);
        }
        public void Clear()
        {
            if (_lines != null)
            {
                _lines.Clear();
                _lines = null;
            }
            _currentLine = new TextLine();
        }
        public void CopyContentToStringBuilder(StringBuilder stbuilder)
        {
            if (!HasSomeRuns) return;
            //

            if (_lines == null)
            {
                _currentLine.CopyContentToStringBuilder(stbuilder);
            }
            else
            {
                bool passFirstLine = false;
                foreach (TextLine line in _lines)
                {
                    if (passFirstLine)
                    {
                        stbuilder.AppendLine();
                    }
                    line.CopyContentToStringBuilder(stbuilder);
                    passFirstLine = true;
                }
            }

        }
    }


    partial class EditableTextLine
    {
        public static void InnerDoJoinWithNextLine(EditableTextLine line)
        {
            line.JoinWithNextLine();
        }
        void JoinWithNextLine()
        {
            if (!IsLastLine)
            {
                EditableTextLine lowerLine = EditableFlowLayer.GetTextLine(_currentLineNumber + 1);
                this.LocalSuspendLineReArrange();
                int cx = 0;
                EditableRun lastTextRun = (EditableRun)this.LastRun;
                if (lastTextRun != null)
                {
                    cx = lastTextRun.Right;
                }

                foreach (EditableRun r in lowerLine._runs)
                {
                    this.AddLast(r);
                    EditableRun.DirectSetLocation(r, cx, 0);
                    cx += r.Width;
                }
                this.LocalResumeLineReArrange();
                this.EndWithLineBreak = lowerLine.EndWithLineBreak;
                EditableFlowLayer.Remove(lowerLine._currentLineNumber);
            }
        }

        public void Copy(TextRangeCopy output)
        {
            LinkedListNode<EditableRun> curNode = this.First;
            while (curNode != null)
            {
                output.AddRun(curNode.Value.CreateCopy());
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
                        output.AddRun(elem);
                    }
                }
                else
                {
                    EditableTextLine startLine = null;
                    EditableTextLine stopLine = null;
                    if (startPoint.LineId == _currentLineNumber)
                    {
                        startLine = this;
                    }
                    else
                    {
                        startLine = EditableFlowLayer.GetTextLine(startPoint.LineId);
                    }
                    if (endPoint.LineId == _currentLineNumber)
                    {
                        stopLine = this;
                    }
                    else
                    {
                        stopLine = EditableFlowLayer.GetTextLine(endPoint.LineId);
                    }
                    if (startLine == stopLine)
                    {
                        CopyRun rightPart = startPoint.Run.Copy(startPoint.RunLocalSelectedIndex);
                        if (rightPart != null)
                        {
                            output.AddRun(rightPart);
                        }
                        if (startPoint.Run.NextRun != endPoint.Run)
                        {
                            foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                                startPoint.Run.NextRun,
                                endPoint.Run.PrevRun))
                            {
                                output.AddRun(t.CreateCopy());
                            }
                        }

                        CopyRun leftPart = endPoint.Run.LeftCopy(endPoint.RunLocalSelectedIndex);
                        if (leftPart != null)
                        {
                            output.AddRun(leftPart);
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
                            EditableTextLine line = EditableFlowLayer.GetTextLine(i);
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
                EditableTextLine startLine = null;
                EditableTextLine stopLine = null;
                if (startPoint.LineId == _currentLineNumber)
                {
                    startLine = this;
                }
                else
                {
                    startLine = EditableFlowLayer.GetTextLine(startPoint.LineId);
                }

                if (endPoint.LineId == _currentLineNumber)
                {
                    stopLine = this;
                }
                else
                {
                    stopLine = EditableFlowLayer.GetTextLine(endPoint.LineId);
                }


                if (startLine == stopLine)
                {
                    if (startPoint.LineCharIndex == -1)
                    {
                        foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                            startPoint.Run,
                            endPoint.Run.PrevRun))
                        {
                            output.AddRun(t.CreateCopy());
                        }
                        CopyRun postCutTextRun = endPoint.Run.Copy(endPoint.RunLocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddRun(postCutTextRun);
                        }
                    }
                    else
                    {
                        CopyRun rightPart = startPoint.Run.Copy(startPoint.RunLocalSelectedIndex + 1);
                        if (rightPart != null)
                        {
                            output.AddRun(rightPart);
                        }

                        foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                            startPoint.Run.NextRun,
                            endPoint.Run.PrevRun))
                        {
                            output.AddRun(t.CreateCopy());
                        }

                        CopyRun leftPart = endPoint.Run.LeftCopy(startPoint.RunLocalSelectedIndex);
                        if (leftPart != null)
                        {
                            output.AddRun(leftPart);
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
                        EditableTextLine line = EditableFlowLayer.GetTextLine(i);
                        line.Copy(output);
                    }
                    stopLine.LeftCopy(endPoint, output);
                }
            }
        }
        internal TextSpanStyle CurrentTextSpanStyle
        {
            get
            {
                return this.OwnerFlowLayer.CurrentTextSpanStyle;
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
                    EditableRun removedRun = startPoint.Run;
                    EditableRun.InnerRemove(removedRun,
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
                            EditableTextLine line = EditableFlowLayer.GetTextLine(startPoint.LineId);
                            line.Remove(removedRun);
                        }
                    }
                }
                else
                {
                    EditableVisualPointInfo newStartPoint = null;
                    EditableVisualPointInfo newStopPoint = null;
                    EditableTextLine startLine = null;
                    EditableTextLine stopLine = null;
                    if (startPoint.LineId == _currentLineNumber)
                    {
                        startLine = this;
                    }
                    else
                    {
                        startLine = EditableFlowLayer.GetTextLine(startPoint.LineId);
                    }
                    newStartPoint = startLine.Split(startPoint);
                    if (endPoint.LineId == _currentLineNumber)
                    {
                        stopLine = this;
                    }
                    else
                    {
                        stopLine = EditableFlowLayer.GetTextLine(endPoint.LineId);
                    }

                    newStopPoint = stopLine.Split(endPoint);
                    if (startLine == stopLine)
                    {
                        if (newStartPoint.Run != null)
                        {
                            LinkedList<EditableRun> tobeRemoveRuns = new LinkedList<EditableRun>();
                            if (newStartPoint.LineCharIndex == 0)
                            {
                                foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                                     newStartPoint.Run,
                                     newStopPoint.Run))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            else
                            {
                                foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                                     newStartPoint.Run.NextRun,
                                     newStopPoint.Run))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            startLine.LocalSuspendLineReArrange();
                            foreach (EditableRun t in tobeRemoveRuns)
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
                            EditableTextLine line = EditableFlowLayer.GetTextLine(i);
                            line.Clear();
                            line.JoinWithNextLine();
                        }
                        if (newStartPoint.LineCharIndex == 0)
                        {
                            startLine.RemoveRight(newStartPoint.Run);
                        }
                        else
                        {
                            EditableRun nextRun = (newStartPoint.Run).NextRun;
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
                EditableTextLine startLine = null;
                EditableTextLine stopLine = null;
                if (startPoint.LineId == _currentLineNumber)
                {
                    startLine = this;
                }
                else
                {
                    startLine = EditableFlowLayer.GetTextLine(startPoint.LineId);
                }
                newStartPoint = startLine.Split(startPoint);
                if (endPoint.LineId == _currentLineNumber)
                {
                    stopLine = this;
                }
                else
                {
                    stopLine = EditableFlowLayer.GetTextLine(endPoint.LineId);
                }
                newStopPoint = stopLine.Split(endPoint);
                if (startLine == stopLine)
                {
                    if (newStartPoint.Run != null)
                    {
                        LinkedList<EditableRun> tobeRemoveRuns = new LinkedList<EditableRun>();
                        if (newStartPoint.LineCharIndex == -1)
                        {
                            foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                                 newStartPoint.Run,
                                 newStopPoint.Run))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        else
                        {
                            foreach (EditableRun t in EditableFlowLayer.TextRunForward(
                                newStartPoint.Run.NextRun,
                                newStopPoint.Run))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        foreach (EditableRun t in tobeRemoveRuns)
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
                        EditableTextLine line = EditableFlowLayer.GetTextLine(i);
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
                            EditableRun nextRun = newStartPoint.Run.NextRun;
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
            var txServices = Root.TextServices;
            Size size;
            char[] mybuffer = copyRun.RawContent;
            if (txServices.SupportsWordBreak)
            {
                var textBufferSpan = new TextBufferSpan(mybuffer);
                var outputUserCharAdvances = new int[mybuffer.Length];
                ILineSegmentList lineSegs = txServices.BreakToLineSegments(ref textBufferSpan);

                txServices.CalculateUserCharGlyphAdvancePos(ref textBufferSpan, lineSegs,
                    this.CurrentTextSpanStyle.ReqFont,
                    outputUserCharAdvances, out int outputTotalW, out int outputLineHeight);

                size = new Size(outputTotalW, outputLineHeight);
            }
            else
            {

                //_content_unparsed = false; 
                var outputUserCharAdvances = new int[mybuffer.Length];
                var textBufferSpan = new TextBufferSpan(mybuffer);

                txServices.CalculateUserCharGlyphAdvancePos(ref textBufferSpan,
                    this.CurrentTextSpanStyle.ReqFont,
                    outputUserCharAdvances, out int outputTotalW, out int outputLineHeight);

                size = new Size(outputTotalW, outputLineHeight);
            }
            return size;
        }
        internal SelectionRangeInfo Split(VisualSelectionRange selectionRange)
        {
            selectionRange.SwapIfUnOrder();

            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;

            if (startPoint.Run == endPoint.Run)
            {
                EditableRun toBeCutTextRun = startPoint.Run;

                CopyRun leftPart = toBeCutTextRun.LeftCopy(startPoint.RunLocalSelectedIndex);
                CopyRun middlePart = toBeCutTextRun.Copy(startPoint.RunLocalSelectedIndex, endPoint.LineCharIndex - startPoint.LineCharIndex);
                CopyRun rightPart = toBeCutTextRun.Copy(endPoint.RunLocalSelectedIndex);

                EditableVisualPointInfo newStartRangePointInfo = null;
                EditableVisualPointInfo newEndRangePointInfo = null;
                EditableTextLine line = this;

                if (startPoint.LineId != _currentLineNumber)
                {
                    line = EditableFlowLayer.GetTextLine(startPoint.LineId);
                }

                line.LocalSuspendLineReArrange();

                if (leftPart != null)
                {
                    EditableRun leftRun = line.AddBefore(toBeCutTextRun, leftPart);
                    newStartRangePointInfo = CreateTextPointInfo(
                        startPoint.LineId, startPoint.LineCharIndex, startPoint.X,
                        leftRun,
                        startPoint.TextRunCharOffset, startPoint.TextRunPixelOffset);
                }
                else
                {
                    //no left part, 
                    //so we connect to prev text run

                    EditableRun prevTxtRun = startPoint.Run.PrevRun;
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
                    EditableRun rightRun = line.AddAfter(toBeCutTextRun, rightPart);
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
                    EditableRun nextTxtRun = endPoint.Run.NextRun;
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
                EditableTextLine workingLine = this;
                if (startPoint.LineId != _currentLineNumber)
                {
                    workingLine = EditableFlowLayer.GetTextLine(startPoint.LineId);
                }
                EditableVisualPointInfo newStartPoint = workingLine.Split(startPoint);
                workingLine = this;
                if (endPoint.LineId != _currentLineNumber)
                {
                    workingLine = EditableFlowLayer.GetTextLine(endPoint.LineId);
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

            EditableRun tobeCutRun = pointInfo.Run;
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
                EditableRun leftRun = AddBefore(tobeCutRun, leftPart);
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


                EditableRun infoTextRun = null;
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
            EditableRun tobeCutRun = pointInfo.Run;
            if (tobeCutRun == null)
            {
                return;
            }
            CopyRun rightPart = tobeCutRun.Copy(pointInfo.RunLocalSelectedIndex);
            if (rightPart != null)
            {
                output.AddRun(rightPart);
            }
            foreach (EditableRun t in GetVisualElementForward(tobeCutRun.NextRun, this.LastRun))
            {
                output.AddRun(t.CreateCopy());
            }
        }

        void LeftCopy(VisualPointInfo pointInfo, TextRangeCopy output)
        {
            if (pointInfo.LineId != _currentLineNumber)
            {
                throw new NotSupportedException();
            }
            EditableRun tobeCutRun = pointInfo.Run;
            if (tobeCutRun == null)
            {
                return;
            }

            foreach (EditableRun t in _runs)
            {
                if (t != tobeCutRun)
                {
                    output.AddRun(t.CreateCopy());
                }
                else
                {
                    break;
                }
            }
            CopyRun leftPart = tobeCutRun.LeftCopy(pointInfo.RunLocalSelectedIndex);
            if (leftPart != null)
            {
                output.AddRun(leftPart);
            }
        }

        EditableVisualPointInfo CreateTextPointInfo(
            int lineId, int lineCharIndex, int caretPixelX,
            EditableRun onRun,
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
            EditableRun lastestTextRun = null;
            foreach (EditableRun t in _runs)
            {
                lastestTextRun = t;
                int thisTextRunWidth = t.Width;
                if (accTextRunWidth + thisTextRunWidth > caretX)
                {
                    EditableRunCharLocation localPointInfo = t.GetCharacterFromPixelOffset(caretX - thisTextRunWidth);
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
            EditableRun lastestRun = null;
            EditableVisualPointInfo textPointInfo = null;
            foreach (EditableRun r in _runs)
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