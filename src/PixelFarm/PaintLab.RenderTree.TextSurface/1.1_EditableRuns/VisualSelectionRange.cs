//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public struct VisualSelectionRangeSnapShot
    {
        public readonly int startLineNum;
        public readonly int startColumnNum;
        public readonly int endLineNum;
        public readonly int endColumnNum;
        public VisualSelectionRangeSnapShot(int startLineNum, int startColumnNum, int endLineNum, int endColumnNum)
        {
            this.startLineNum = startLineNum;
            this.startColumnNum = startColumnNum;
            this.endLineNum = endLineNum;
            this.endColumnNum = endColumnNum;
        }
        public bool IsEmpty()
        {
            return startLineNum == 0 && startColumnNum == 0
                && endLineNum == 0 && endColumnNum == 0;
        }
        public static readonly VisualSelectionRangeSnapShot Empty = new VisualSelectionRangeSnapShot();
    }

    class VisualSelectionRange
    {
        EditableVisualPointInfo startPoint = null;
        EditableVisualPointInfo endPoint = null;
        public VisualSelectionRange(
            EditableVisualPointInfo startPoint,
            EditableVisualPointInfo endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.BackgroundColor = Color.LightGray;
        }
        public EditableVisualPointInfo StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                startPoint = value;
            }
        }
        public EditableVisualPointInfo EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                if (startPoint != null)
                {
                    endPoint = value;
                }
                else
                {
                    endPoint = null;
                }
            }
        }

        public Color BackgroundColor { get; set; }

        public bool IsOnTheSameLine
        {
            get
            {
                return startPoint.LineId == endPoint.LineId;
            }
        }


        public void SwapIfUnOrder()
        {
            if (IsOnTheSameLine)
            {
                if (startPoint.LineCharIndex > endPoint.LineCharIndex)
                {
                    EditableVisualPointInfo tmpPoint = startPoint;
                    startPoint = endPoint;
                    endPoint = tmpPoint;
                }
            }
            else
            {
                if (startPoint.LineId > endPoint.LineId)
                {
                    EditableVisualPointInfo tmp = startPoint;
                    startPoint = endPoint;
                    endPoint = tmp;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if (startPoint != null && endPoint != null)
                {
                    if ((startPoint.TextRun != null && !startPoint.TextRun.HasParent) ||
                        (endPoint.TextRun != null && !endPoint.TextRun.HasParent))
                    {
                        throw new NotSupportedException("text range err");
                    }
                    if ((startPoint.LineCharIndex == endPoint.LineCharIndex) &&
                    (startPoint.LineId == endPoint.LineId))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public EditableVisualPointInfo TopEnd
        {
            get
            {
                switch (startPoint.LineId.CompareTo(endPoint.LineId))
                {
                    case -1:
                        return startPoint;
                    case 0:
                        if (startPoint.X <= endPoint.X)
                        {
                            return startPoint;
                        }
                        else
                        {
                            return endPoint;
                        }
                }
                return endPoint;
            }
        }
        public EditableVisualPointInfo BottomEnd
        {
            get
            {
                switch (startPoint.LineId.CompareTo(endPoint.LineId))
                {
                    case -1:
                        return endPoint;
                    case 0:
                        if (endPoint.X > startPoint.X)
                        {
                            return endPoint;
                        }
                        else
                        {
                            return startPoint;
                        }
                }
                return startPoint;
            }
        }
        public void Draw(DrawBoard destPage, Rectangle updateArea)
        {
            if (IsOnTheSameLine)
            {
                VisualPointInfo topEndPoint = TopEnd;
                VisualPointInfo bottomEndPoint = BottomEnd;
                int linetop = topEndPoint.LineTop;
                destPage.FillRectangle(BackgroundColor, topEndPoint.X, linetop,
                    bottomEndPoint.X - topEndPoint.X, topEndPoint.ActualLineHeight);
            }
            else
            {
                EditableVisualPointInfo topEndPoint = TopEnd;
                int lineYPos = topEndPoint.LineTop;
                destPage.FillRectangle(BackgroundColor, topEndPoint.X, lineYPos,
                    topEndPoint.CurrentWidth - topEndPoint.X,
                    topEndPoint.ActualLineHeight);
                int topLineId = topEndPoint.LineId;
                int bottomLineId = BottomEnd.LineId;
                if (bottomLineId - topLineId > 1)
                {
                    EditableTextLine adjacentStartLine = topEndPoint.EditableLine.Next;
                    while (adjacentStartLine != BottomEnd.Line)
                    {
                        destPage.FillRectangle(BackgroundColor, 0,
                            adjacentStartLine.LineTop,
                            adjacentStartLine.LineWidth,
                            adjacentStartLine.ActualLineHeight);
                        adjacentStartLine = adjacentStartLine.Next;
                    }
                    EditableTextLine adjacentStopLine = BottomEnd.Line.Prev;
                }
                VisualPointInfo bottomEndPoint = BottomEnd;
                lineYPos = bottomEndPoint.LineTop;
                destPage.FillRectangle(BackgroundColor, 0, lineYPos, bottomEndPoint.X,
                     bottomEndPoint.ActualLineHeight);
            }
        }
        public void UpdateSelectionRange()
        {
            if (startPoint.TextRun != null && !startPoint.TextRun.HasParent)
            {
                EditableTextLine startLine = startPoint.EditableLine;
                startPoint = startLine.GetTextPointInfoFromCharIndex(startPoint.LineCharIndex);
            }
            if (endPoint.TextRun != null && !endPoint.TextRun.HasParent)
            {
                EditableTextLine stopLine = endPoint.EditableLine;
                endPoint = stopLine.GetTextPointInfoFromCharIndex(endPoint.LineCharIndex);
            }
        }

        public IEnumerable<EditableRun> GetPrintableTextRunIter()
        {
            EditableRun startRun = null;
            if (startPoint.TextRun == null)
            {
                EditableTextLine line = startPoint.EditableLine;
                startRun = line.FirstRun;
            }
            else
            {
                startRun = startPoint.TextRun.NextTextRun;
            }

            EditableTextFlowLayer layer = startRun.OwnerEditableLine.editableFlowLayer;
            foreach (EditableRun t in layer.GetDrawingIter(startRun, endPoint.TextRun))
            {
                if (!t.IsLineBreak)
                {
                    yield return t;
                }
            }
        }
        public VisualSelectionRangeSnapShot GetSelectionRangeSnapshot()
        {
            return new VisualSelectionRangeSnapShot(
                startPoint.LineNumber,
                startPoint.LineCharIndex,
                endPoint.LineNumber,
                endPoint.LineCharIndex);
        }


#if DEBUG
        public override string ToString()
        {
            StringBuilder stBuilder = new StringBuilder();
            if (this.IsValid)
            {
                stBuilder.Append("sel");
            }
            else
            {
                stBuilder.Append("!sel");
            }
            stBuilder.Append(startPoint.ToString());
            stBuilder.Append(',');
            stBuilder.Append(endPoint.ToString());
            return stBuilder.ToString();
        }
#endif
    }



    class VisualMarkerSelectionRange
    {

        struct MarkerLocation
        {
            public int lineNum;
            public float x_offset;
            public EditableTextLine line;

        }

        EditableTextFlowLayer textLayer;
        VisualSelectionRangeSnapShot selectionRangeSnapshot;
        MarkerLocation _startLocation;
        MarkerLocation _stopLocation;

        public VisualMarkerSelectionRange(VisualSelectionRangeSnapShot selectionRangeSnapshot)
        {
            this.selectionRangeSnapshot = selectionRangeSnapshot;
            BackgroundColor = Color.FromArgb(80, Color.Yellow); //test
        }
        public Color BackgroundColor { get; set; }
        public bool IsOnTheSameLine
        {
            get
            {
                return selectionRangeSnapshot.startLineNum == selectionRangeSnapshot.endLineNum;
            }
        }

        public void BindToTextLayer(EditableTextFlowLayer textLayer)
        {
            this.textLayer = textLayer;
            //check is on the sameline,
            //or multiple lines
            //
            if (IsOnTheSameLine)
            {
                EditableTextLine line = textLayer.GetTextLine(selectionRangeSnapshot.startLineNum);
                //at this line
                //find start and end point
                int startColNum = selectionRangeSnapshot.startColumnNum;
                int endColNum = selectionRangeSnapshot.endColumnNum;
                int lineHeight = line.ActualLineHeight;

                _startLocation = new MarkerLocation() { lineNum = line.LineNumber, x_offset = line.GetXOffsetAtCharIndex(startColNum), line = line };
                _stopLocation = new MarkerLocation() { lineNum = line.LineNumber, x_offset = line.GetXOffsetAtCharIndex(endColNum), line = line };
            }
            else
            {
                EditableTextLine startLine = textLayer.GetTextLine(selectionRangeSnapshot.startLineNum);
                _startLocation = new MarkerLocation()
                {
                    lineNum = startLine.LineNumber,
                    x_offset = startLine.GetXOffsetAtCharIndex(selectionRangeSnapshot.startColumnNum),
                    line = startLine
                };
                //
                EditableTextLine endLine = textLayer.GetTextLine(selectionRangeSnapshot.endLineNum);
                _stopLocation = new MarkerLocation()
                {
                    lineNum = endLine.LineNumber,
                    x_offset = endLine.GetXOffsetAtCharIndex(selectionRangeSnapshot.endColumnNum),
                    line = endLine
                };
                //

            }
        }
        public void Draw(DrawBoard destPage, Rectangle updateArea)
        {
            if (IsOnTheSameLine)
            {
                EditableTextLine line = _startLocation.line;
                if (line.OwnerFlowLayer == null)
                {
                    //this marker should be remove or not
                    return;
                } 
                destPage.FillRectangle(BackgroundColor, _startLocation.x_offset, line.LineTop,
                    _stopLocation.x_offset - _startLocation.x_offset, line.ActualLineHeight);
            }
            else
            {
                //multiple line
                EditableTextLine startLine = _startLocation.line;
                EditableTextLine endLine = _stopLocation.line;

                if (startLine.OwnerFlowLayer == null || endLine.OwnerFlowLayer == null)
                {
                    //this marker should be remove or not
                    return;
                }

                int lineYPos = startLine.LineTop;
                destPage.FillRectangle(BackgroundColor, _startLocation.x_offset, lineYPos,
                    startLine.ActualLineWidth - _startLocation.x_offset,
                    startLine.ActualLineHeight);

                int topLineId = startLine.LineNumber;
                int bottomLineId = endLine.LineNumber;
                if (bottomLineId - topLineId > 1)
                {
                    EditableTextLine adjacentStartLine = startLine.Next;
                    while (adjacentStartLine != endLine)
                    {
                        destPage.FillRectangle(BackgroundColor, 0,
                            adjacentStartLine.LineTop,
                            adjacentStartLine.LineWidth,
                            adjacentStartLine.ActualLineHeight);
                        adjacentStartLine = adjacentStartLine.Next;
                    }
                }

                destPage.FillRectangle(BackgroundColor, 0, endLine.LineTop,
                     _stopLocation.x_offset,
                     endLine.ActualLineHeight);
            }

        }
        public static VisualMarkerSelectionRange CreateFromSelectionRange(VisualSelectionRangeSnapShot selectionRangeSnapshot)
        {
            return new VisualMarkerSelectionRange(selectionRangeSnapshot);
        }
    }
}

