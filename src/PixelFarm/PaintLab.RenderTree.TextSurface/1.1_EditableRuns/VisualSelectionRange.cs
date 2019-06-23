//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.TextEditing
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

    public class VisualSelectionRange
    {
        EditableVisualPointInfo _startPoint = null;
        EditableVisualPointInfo _endPoint = null;
        internal VisualSelectionRange(
            EditableVisualPointInfo startPoint,
            EditableVisualPointInfo endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            this.BackgroundColor = Color.LightGray;
        }
        internal EditableVisualPointInfo StartPoint
        {
            get => _startPoint;
            set => _startPoint = value;
        }
        internal EditableVisualPointInfo EndPoint
        {
            get => _endPoint;
            set
            {
                if (_startPoint != null)
                {
                    _endPoint = value;
                }
                else
                {
                    _endPoint = null;
                }
            }
        }
        //
        public Color BackgroundColor { get; set; }
        //
        public bool IsOnTheSameLine => _startPoint.LineId == _endPoint.LineId;
        //

        public void SwapIfUnOrder()
        {
            if (IsOnTheSameLine)
            {
                if (_startPoint.LineCharIndex > _endPoint.LineCharIndex)
                {
                    EditableVisualPointInfo tmpPoint = _startPoint;
                    _startPoint = _endPoint;
                    _endPoint = tmpPoint;
                }
            }
            else
            {
                if (_startPoint.LineId > _endPoint.LineId)
                {
                    EditableVisualPointInfo tmp = _startPoint;
                    _startPoint = _endPoint;
                    _endPoint = tmp;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if (_startPoint != null && _endPoint != null)
                {
                    if ((_startPoint.TextRun != null && !_startPoint.TextRun.HasParent) ||
                        (_endPoint.TextRun != null && !_endPoint.TextRun.HasParent))
                    {
                        throw new NotSupportedException("text range err");
                    }
                    if ((_startPoint.LineCharIndex == _endPoint.LineCharIndex) &&
                    (_startPoint.LineId == _endPoint.LineId))
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


        internal EditableVisualPointInfo TopEnd
        {
            get
            {
                switch (_startPoint.LineId.CompareTo(_endPoint.LineId))
                {
                    case -1:
                        return _startPoint;
                    case 0:
                        if (_startPoint.X <= _endPoint.X)
                        {
                            return _startPoint;
                        }
                        else
                        {
                            return _endPoint;
                        }
                }
                return _endPoint;
            }
        }
        internal EditableVisualPointInfo BottomEnd
        {
            get
            {
                switch (_startPoint.LineId.CompareTo(_endPoint.LineId))
                {
                    case -1:
                        return _endPoint;
                    case 0:
                        if (_endPoint.X > _startPoint.X)
                        {
                            return _endPoint;
                        }
                        else
                        {
                            return _startPoint;
                        }
                }
                return _startPoint;
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
            if (_startPoint.TextRun != null && !_startPoint.TextRun.HasParent)
            {
                EditableTextLine startLine = _startPoint.EditableLine;
                _startPoint = startLine.GetTextPointInfoFromCharIndex(_startPoint.LineCharIndex);
            }
            if (_endPoint.TextRun != null && !_endPoint.TextRun.HasParent)
            {
                EditableTextLine stopLine = _endPoint.EditableLine;
                _endPoint = stopLine.GetTextPointInfoFromCharIndex(_endPoint.LineCharIndex);
            }
        }

        public IEnumerable<EditableRun> GetPrintableTextRunIter()
        {
            EditableRun startRun = null;
            if (_startPoint.TextRun == null)
            {
                EditableTextLine line = _startPoint.EditableLine;
                startRun = line.FirstRun;
            }
            else
            {
                startRun = _startPoint.TextRun.NextTextRun;
            }

            EditableTextFlowLayer layer = startRun.OwnerEditableLine.EditableFlowLayer;
            foreach (EditableRun t in layer.GetDrawingIter(startRun, _endPoint.TextRun))
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
                _startPoint.LineNumber,
                _startPoint.LineCharIndex,
                _endPoint.LineNumber,
                _endPoint.LineCharIndex);
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
            stBuilder.Append(_startPoint.ToString());
            stBuilder.Append(',');
            stBuilder.Append(_endPoint.ToString());
            return stBuilder.ToString();
        }
#endif
    }



    public class VisualMarkerSelectionRange
    {

        struct MarkerLocation
        {
            public int lineNum;
            public float x_offset;
            public EditableTextLine line;

        }

        EditableTextFlowLayer _textLayer;
        VisualSelectionRangeSnapShot _selectionRangeSnapshot;
        MarkerLocation _startLocation;
        MarkerLocation _stopLocation;


        public VisualMarkerSelectionRange(VisualSelectionRangeSnapShot selectionRangeSnapshot)
        {
            _selectionRangeSnapshot = selectionRangeSnapshot;
            BackgroundColor = Color.FromArgb(80, Color.Yellow);//test, default
        }
        //
        public Color BackgroundColor { get; set; }
        //
        public bool IsOnTheSameLine => _selectionRangeSnapshot.startLineNum == _selectionRangeSnapshot.endLineNum;


        internal void BindToTextLayer(EditableTextFlowLayer textLayer)
        {
            _textLayer = textLayer;
            //check is on the sameline,
            //or multiple lines
            //
            if (IsOnTheSameLine)
            {
                EditableTextLine line = textLayer.GetTextLine(_selectionRangeSnapshot.startLineNum);
                //at this line
                //find start and end point
                int startColNum = _selectionRangeSnapshot.startColumnNum;
                int endColNum = _selectionRangeSnapshot.endColumnNum;
                int lineHeight = line.ActualLineHeight;

                _startLocation = new MarkerLocation() { lineNum = line.LineNumber, x_offset = line.GetXOffsetAtCharIndex(startColNum), line = line };
                _stopLocation = new MarkerLocation() { lineNum = line.LineNumber, x_offset = line.GetXOffsetAtCharIndex(endColNum), line = line };
            }
            else
            {
                EditableTextLine startLine = textLayer.GetTextLine(_selectionRangeSnapshot.startLineNum);
                _startLocation = new MarkerLocation()
                {
                    lineNum = startLine.LineNumber,
                    x_offset = startLine.GetXOffsetAtCharIndex(_selectionRangeSnapshot.startColumnNum),
                    line = startLine
                };
                //
                EditableTextLine endLine = textLayer.GetTextLine(_selectionRangeSnapshot.endLineNum);
                _stopLocation = new MarkerLocation()
                {
                    lineNum = endLine.LineNumber,
                    x_offset = endLine.GetXOffsetAtCharIndex(_selectionRangeSnapshot.endColumnNum),
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



    }
}

