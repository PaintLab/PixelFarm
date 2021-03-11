//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.TextFlow
{

    public sealed class VisualSelectionRange
    {
        EditableVisualPointInfo _startPoint = null;
        EditableVisualPointInfo _endPoint = null;
        TextFlowLayer _layer;
        internal VisualSelectionRange(
            TextFlowLayer layer,
            EditableVisualPointInfo startPoint,
            EditableVisualPointInfo endPoint)
        {
            _layer = layer;
            _startPoint = startPoint;
            _endPoint = endPoint;
            this.BackgroundColor = Color.FromArgb(150, Color.Yellow);
        }
        public Rectangle GetSelectionUpdateArea()
        {
            return Rectangle.FromLTRB(0,
                    TopEnd.LineTop - TopEnd.Line.OverlappedTop,
                    _layer.OwnerWidth,
                    BottomEnd.Line.LineBottom + BottomEnd.Line.OverlappedBottom);
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
                    //???
                    _endPoint = null;
                }
            }
        }
        //
        public Color BackgroundColor { get; set; }
        public Color FontColor { get; set; } = Color.Black; //color for selection font
        //
        public bool IsOnTheSameLine => _startPoint.LineId == _endPoint.LineId;
        //
        public void Normalize()
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

        internal enum ClipRectKind : byte
        {
            No,
            SameLine,
            StartLine,
            InBetween,
            EndLine,
        }

        internal ClipRectKind GetLineClip(int lineNo, out int clipLeft, out int clipWidth)
        {
            if (IsOnTheSameLine)
            {
                if (lineNo == _startPoint.LineId)
                {
                    clipLeft = TopEnd.X;
                    clipWidth = BottomEnd.X - clipLeft;

                    return ClipRectKind.SameLine;
                }
                else
                {
                    clipLeft = clipWidth = 0;
                    return ClipRectKind.No;
                }
            }
            else
            {
                EditableVisualPointInfo top_point = TopEnd;
                EditableVisualPointInfo bottom_point = BottomEnd;

                if (lineNo == top_point.LineId)
                {
                    clipLeft = top_point.X;
                    clipWidth = top_point.CurrentWidth;
                    return ClipRectKind.StartLine;
                }
                else if (lineNo == bottom_point.LineId)
                {
                    clipLeft = 0;
                    clipWidth = bottom_point.X;
                    return ClipRectKind.EndLine;
                }
                else if (lineNo > top_point.LineId && lineNo < bottom_point.LineId)
                {
                    clipLeft = clipWidth = 0;
                    return ClipRectKind.InBetween;
                }
                else
                {
                    clipLeft = clipWidth = 0;
                    return ClipRectKind.No;
                }
            }
        }
        public void Draw(DrawBoard destPage, UpdateArea updateArea)
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
                    TextLineBox adjacentStartLine = topEndPoint.Line.Next;
                    while (adjacentStartLine != BottomEnd.Line)
                    {
                        destPage.FillRectangle(BackgroundColor, 0,
                            adjacentStartLine.LineTop,
                            adjacentStartLine.LineWidth,
                            adjacentStartLine.ActualLineHeight);
                        adjacentStartLine = adjacentStartLine.Next;
                    }

#if DEBUG
                    TextLineBox adjacentStopLine = BottomEnd.Line.Prev;
#endif
                }
                VisualPointInfo bottomEndPoint = BottomEnd;
                lineYPos = bottomEndPoint.LineTop;
                destPage.FillRectangle(BackgroundColor, 0, lineYPos, bottomEndPoint.X,
                     bottomEndPoint.ActualLineHeight);
            }
        }
        public void UpdateSelectionRange()
        {
            if (!IsValid) { return; }

            _startPoint = _startPoint.Line.GetTextPointInfoFromCharIndex(_startPoint.LineCharIndex);
            _endPoint = _endPoint.Line.GetTextPointInfoFromCharIndex(_endPoint.LineCharIndex);
        }

        public SelectionRangeSnapShot GetSelectionRangeSnapshot()
        {
            return new SelectionRangeSnapShot(
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
            public readonly int lineNum;
            public readonly float x_offset;
            public readonly TextLineBox line;
            public MarkerLocation(int lineNum, float x_offset, TextLineBox line)
            {
                this.lineNum = lineNum;
                this.x_offset = x_offset;
                this.line = line;
            }
        }

        TextFlowLayer _textLayer;
        SelectionRangeSnapShot _selectionRangeSnapshot;
        MarkerLocation _startLocation;
        MarkerLocation _stopLocation;


        public VisualMarkerSelectionRange(SelectionRangeSnapShot selectionRangeSnapshot)
        {
            _selectionRangeSnapshot = selectionRangeSnapshot;
            BackgroundColor = Color.FromArgb(80, Color.Yellow);//test, default
        }
        //
        public Color BackgroundColor { get; set; }
        //
        public bool IsOnTheSameLine => _selectionRangeSnapshot.startLineNum == _selectionRangeSnapshot.endLineNum;


        internal void BindToTextLayer(TextFlowLayer textLayer)
        {
            _textLayer = textLayer;
            //check is on the sameline,
            //or multiple lines
            //
            if (IsOnTheSameLine)
            {
                TextLineBox line = textLayer.GetTextLine(_selectionRangeSnapshot.startLineNum);
                //at this line
                //find start and end point
                int startColNum = _selectionRangeSnapshot.startColumnNum;
                int endColNum = _selectionRangeSnapshot.endColumnNum;
                int lineHeight = line.ActualLineHeight;

                _startLocation = new MarkerLocation(line.LineNumber, line.GetXOffsetAtCharIndex(startColNum), line);
                _stopLocation = new MarkerLocation(line.LineNumber, line.GetXOffsetAtCharIndex(endColNum), line);
            }
            else
            {
                TextLineBox startLine = textLayer.GetTextLine(_selectionRangeSnapshot.startLineNum);
                _startLocation = new MarkerLocation(startLine.LineNumber, startLine.GetXOffsetAtCharIndex(_selectionRangeSnapshot.startColumnNum), startLine);

                //
                TextLineBox endLine = textLayer.GetTextLine(_selectionRangeSnapshot.endLineNum);
                _stopLocation = new MarkerLocation(endLine.LineNumber, endLine.GetXOffsetAtCharIndex(_selectionRangeSnapshot.endColumnNum), endLine);
                //
            }
        }
        public void Draw(DrawBoard destPage, UpdateArea updateArea)
        {
            if (IsOnTheSameLine)
            {
                TextLineBox line = _startLocation.line;
                if (!line.HasOwnerFlowLayer)
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
                TextLineBox startLine = _startLocation.line;
                TextLineBox endLine = _stopLocation.line;

                if (!startLine.HasOwnerFlowLayer || !endLine.HasOwnerFlowLayer)
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
                    TextLineBox adjacentStartLine = startLine.Next;
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

