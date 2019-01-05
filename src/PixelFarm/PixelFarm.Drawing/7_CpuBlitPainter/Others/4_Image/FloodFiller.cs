//BSD, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.Imaging
{
    public partial class FloodFill
    {
        int _imageWidth;
        int _imageHeight;

        byte _tolerance0To255;
        Color _fillColor;
        bool[] _pixelsChecked;
        FillingRule _fillRule;
        SimpleQueue<HSpan> _ranges = new SimpleQueue<HSpan>(9);

        IBitmapSrc _destImgRW;

        /// <summary>
        /// if user want to collect output range 
        /// </summary>
        HSpanCollection _rangeCollectionOutput;




        public FloodFill(Color fillColor)
            : this(fillColor, 0)
        {
        }
        public FloodFill(Color fillColor, byte tolerance)
        {
            Update(fillColor, tolerance);
        }

        public void SetRangeCollectionOutput(HSpanCollection output)
        {
            _rangeCollectionOutput = output;
        }

        public Color FillColor => _fillColor;
        public byte Tolerance => _tolerance0To255;
        public void Update(Color fillColor, byte tolerance)
        {
            _tolerance0To255 = tolerance;
            _fillColor = fillColor;

            if (tolerance > 0)
            {
                _fillRule = new ToleranceMatch(fillColor, tolerance);
            }
            else
            {
                _fillRule = new ExactMatch(fillColor);
            }
        }



        public void Fill(MemBitmap memBmp, int x, int y)
        {
            Fill((IBitmapSrc)memBmp, x, y);
        }
        public void Fill(IBitmapSrc bufferToFillOn, int x, int y)
        {
            y -= _imageHeight;
            unchecked // this way we can overflow the uint on negative and get a big number
            {
                if ((uint)x >= bufferToFillOn.Width || (uint)y >= bufferToFillOn.Height)
                {
                    return;
                }
            }
            _destImgRW = bufferToFillOn;

            unsafe
            {
                using (TempMemPtr destBufferPtr = bufferToFillOn.GetBufferPtr())
                {

                    _imageWidth = bufferToFillOn.Width;
                    _imageHeight = bufferToFillOn.Height;
                    //reset new buffer, clear mem?
                    _pixelsChecked = new bool[_imageWidth * _imageHeight];

                    int* destBuffer = (int*)destBufferPtr.Ptr;
                    int startColorBufferOffset = bufferToFillOn.GetBufferOffsetXY32(x, y);

                    int start_color = *(destBuffer + startColorBufferOffset);

                    _fillRule.SetStartColor(Drawing.Color.FromArgb(
                        (start_color >> 16) & 0xff,
                        (start_color >> 8) & 0xff,
                        (start_color) & 0xff));


                    LinearFill(destBuffer, x, y);

                    bool addToOutputRanges = _rangeCollectionOutput != null;
                    if (addToOutputRanges)
                    {
                        _rangeCollectionOutput.Clear();
                        _rangeCollectionOutput.SetYCut(y);
                    }


                    while (_ranges.Count > 0)
                    {


                        HSpan range = _ranges.Dequeue();

                        if (addToOutputRanges)
                        {
                            _rangeCollectionOutput.AddHSpan(range);
                        }


                        int downY = range.y - 1;
                        int upY = range.y + 1;
                        int downPixelOffset = (_imageWidth * (range.y - 1)) + range.startX;
                        int upPixelOffset = (_imageWidth * (range.y + 1)) + range.startX;
                        for (int rangeX = range.startX; rangeX <= range.endX; rangeX++)
                        {
                            if (range.y > 0)
                            {
                                if (!_pixelsChecked[downPixelOffset])
                                {
                                    int bufferOffset = bufferToFillOn.GetBufferOffsetXY32(rangeX, downY);

                                    if (_fillRule.CheckPixel(*(destBuffer + bufferOffset)))
                                    {
                                        LinearFill(destBuffer, rangeX, downY);
                                    }
                                }
                            }

                            if (range.y < (_imageHeight - 1))
                            {
                                if (!_pixelsChecked[upPixelOffset])
                                {
                                    int bufferOffset = bufferToFillOn.GetBufferOffsetXY32(rangeX, upY);
                                    if (_fillRule.CheckPixel(*(destBuffer + bufferOffset)))
                                    {
                                        LinearFill(destBuffer, rangeX, upY);
                                    }
                                }
                            }
                            upPixelOffset++;
                            downPixelOffset++;
                        }
                    }
                }
            }

            //reset
            _imageHeight = 0;
            _ranges.Clear();
            _destImgRW = null;
        }

        /// <summary>
        /// fill to left side and right side of the line
        /// </summary>
        /// <param name="destBuffer"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        unsafe void LinearFill(int* destBuffer, int x, int y)
        {

            int leftFillX = x;
            int bufferOffset = _destImgRW.GetBufferOffsetXY32(x, y);
            int pixelOffset = (_imageWidth * y) + x;
            for (; ; )
            {
                _fillRule.SetPixel(destBuffer + bufferOffset);
                _pixelsChecked[pixelOffset] = true;
                leftFillX--;
                pixelOffset--;
                bufferOffset--;
                if (leftFillX <= 0 || (_pixelsChecked[pixelOffset]) || !_fillRule.CheckPixel(*(destBuffer + bufferOffset)))
                {
                    break;
                }
            }
            leftFillX++;
            //
            int rightFillX = x;
            bufferOffset = _destImgRW.GetBufferOffsetXY32(x, y);
            pixelOffset = (_imageWidth * y) + x;
            for (; ; )
            {
                _fillRule.SetPixel(destBuffer + bufferOffset);
                _pixelsChecked[pixelOffset] = true;
                rightFillX++;
                pixelOffset++;
                bufferOffset++;
                if (rightFillX >= _imageWidth || _pixelsChecked[pixelOffset] || !_fillRule.CheckPixel(*(destBuffer + bufferOffset)))
                {
                    break;
                }
            }
            rightFillX--;
            _ranges.Enqueue(new HSpan(leftFillX, rightFillX, y));
        }
    }



    //------------------------------------------------------------------------------
    partial class FloodFill
    {
        abstract class FillingRule
        {

            readonly Color _fillColor;
            readonly int _fillColorInt32;

            protected FillingRule(Color fillColor)
            {
                _fillColor = fillColor;


                _fillColorInt32 =
                    (_fillColor.red << PixelFarm.CpuBlit.PixelProcessing.CO.R_SHIFT) |
                    (_fillColor.green << PixelFarm.CpuBlit.PixelProcessing.CO.G_SHIFT) |
                    (_fillColor.blue << PixelFarm.CpuBlit.PixelProcessing.CO.B_SHIFT) |
                    (_fillColor.alpha << PixelFarm.CpuBlit.PixelProcessing.CO.A_SHIFT);
            }
            public abstract void SetStartColor(Color startColor);
            public unsafe void SetPixel(int* dest)
            {
                *dest = _fillColorInt32;
            }
            public abstract bool CheckPixel(int pixelValue32);
        }

        sealed class ExactMatch : FillingRule
        {
            int _startColorInt32;

            public ExactMatch(Color fillColor)
                : base(fillColor)
            {
            }
            public override void SetStartColor(Color startColor)
            {
                _startColorInt32 =
                    (startColor.red << PixelFarm.CpuBlit.PixelProcessing.CO.R_SHIFT) |
                    (startColor.green << PixelFarm.CpuBlit.PixelProcessing.CO.G_SHIFT) |
                    (startColor.blue << PixelFarm.CpuBlit.PixelProcessing.CO.B_SHIFT);
            }
            public override bool CheckPixel(int pixelValue32)
            {
                //ARGB
                return _startColorInt32 == pixelValue32;
                //int r = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.R_SHIFT) & 0xff;//16
                //int g = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.G_SHIFT) & 0xff;//8
                //int b = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.B_SHIFT) & 0xff;//0
                //return r == _startColor.red &&
                //       g == _startColor.green &&
                //       b == _startColor.blue;
                //return (destBuffer[bufferOffset] == startColor.red) &&
                //    (destBuffer[bufferOffset + 1] == startColor.green) &&
                //    (destBuffer[bufferOffset + 2] == startColor.blue);
            }
        }

        sealed class ToleranceMatch : FillingRule
        {
            int _tolerance0To255;

            //** only RGB?
            byte _red_min, _red_max;
            byte _green_min, _green_max;
            byte _blue_min, _blue_max;

            public ToleranceMatch(Color fillColor, int tolerance0To255)
                : base(fillColor)
            {
                _tolerance0To255 = tolerance0To255;
            }

            static byte Clamp0_255(int value)
            {
                if (value < 0) return 0;
                if (value > 255) return 255;
                return (byte)value;
            }

            public override void SetStartColor(Color startColor)
            {
                _red_min = Clamp0_255(startColor.R - _tolerance0To255);
                _red_max = Clamp0_255(startColor.R + _tolerance0To255);
                //
                _green_min = Clamp0_255(startColor.G - _tolerance0To255);
                _green_max = Clamp0_255(startColor.G + _tolerance0To255);
                //
                _blue_min = Clamp0_255(startColor.B - _tolerance0To255);
                _blue_max = Clamp0_255(startColor.B + _tolerance0To255);
            }
            public override bool CheckPixel(int pixelValue32)
            {

                int r = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.R_SHIFT) & 0xff;
                int g = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.G_SHIFT) & 0xff;
                int b = (pixelValue32 >> PixelFarm.CpuBlit.PixelProcessing.CO.B_SHIFT) & 0xff;

                //range test
                return (r >= _red_min) && (r <= _red_max) &&
                       (g >= _green_min) && (g <= _green_max) &&
                       (b >= _blue_min) && (b <= _blue_max);


                //return (destBuffer[bufferOffset] >= (startColor.red - tolerance0To255)) && destBuffer[bufferOffset] <= (startColor.red + tolerance0To255) &&
                //    (destBuffer[bufferOffset + 1] >= (startColor.green - tolerance0To255)) && destBuffer[bufferOffset + 1] <= (startColor.green + tolerance0To255) &&
                //    (destBuffer[bufferOffset + 2] >= (startColor.blue - tolerance0To255)) && destBuffer[bufferOffset + 2] <= (startColor.blue + tolerance0To255);
            }
        }
    }

    //------------------------------------------------------------------------------


    partial class FloodFill
    {
        /// <summary>
        /// horizontal (scanline) span
        /// </summary>
        public struct HSpan
        {
            public readonly int startX;
            public readonly int endX;
            public readonly int y;
            public HSpan(int startX, int endX, int y)
            {
                this.startX = startX;
                this.endX = endX;
                this.y = y;
            }
#if DEBUG
            public override string ToString()
            {
                return "line:" + y + ", x_start=" + startX + ",len=" + (endX - startX);
            }
#endif
        }


        public class HSpanCollection
        {
            //user can use only 1 list

            //but I test with 2 list (upper and lower) (esp, for debug

            List<HSpan> _upperSpans = new List<HSpan>();
            List<HSpan> _lowerSpans = new List<HSpan>();

            int _yCutAt;
            internal void SetYCut(int ycut)
            {
                _yCutAt = ycut;
            }
            public void Clear()
            {
                _lowerSpans.Clear();
                _upperSpans.Clear();
            }

            internal void AddHSpan(HSpan range)
            {
                if (range.y >= _yCutAt)
                {
                    _lowerSpans.Add(range);
                }
                else
                {
                    _upperSpans.Add(range);
                }
            }



            /// <summary>
            /// reconstruct vxs from collected HSpan
            /// </summary>
            /// <param name="outputVxs"></param>
            public void ReconstructVxs(VertexStore outputVxs)
            {
                MultiColumnSpans multiColumnHSpans = new MultiColumnSpans();
                multiColumnHSpans.LoadHSpans(_upperSpans, _lowerSpans);

                using (VectorToolBox.Borrow(outputVxs, out PathWriter pathW))
                {
                    multiColumnHSpans.TraceOutline(pathW);
                    pathW.CloseFigure();
                }

            }
        }



        class HSpanColumn
        {
            List<HSpan> _spanList = new List<HSpan>();
            bool _leftSideChecked;
            bool _rightSideChecked;


#if DEBUG
            bool dbugEvalCorners;
            public bool dbugEvalEnd;
#endif
            public HSpanColumn(VerticalGroup owner, int colNumber)
            {
                ColNumber = colNumber;
                Owner = owner;
            }
            public VerticalGroup Owner { get; }
            public int ColNumber { get; }
            public void AddHSpan(HSpan span)
            {
                _spanList.Add(span);
            }
            public void EvaluateCorners()
            {
#if DEBUG
                dbugEvalCorners = true;
#endif
                //the column may not be rectangle shape ***
                if (_spanList.Count == 0)
                {
                    //??
                    throw new System.NotSupportedException();
                }
                //..

                HSpan hspan = _spanList[0];
                XLeftTop = hspan.startX;
                XRightTop = hspan.endX;
                YTop = hspan.y;

                hspan = _spanList[_spanList.Count - 1];
                XLeftBottom = hspan.startX;
                XRightBottom = hspan.endX;
                YBottom = hspan.y + 1;//***
            }

            public int YTop { get; set; }
            public int YBottom { get; set; }

            public int XLeftTop { get; private set; }
            public int XRightTop { get; private set; }
            public int XLeftBottom { get; private set; }
            public int XRightBottom { get; private set; }

            public void ResetRead()
            {
                _leftSideChecked = _rightSideChecked = false;
            }
            public void ReadLeftSide(PathWriter pathW, bool topDown)
            {
                //read once
                if (_leftSideChecked) throw new System.NotSupportedException();

                _leftSideChecked = true;

                if (topDown)
                {
                    int count = _spanList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        HSpan span = _spanList[i];
                        pathW.LineTo(span.startX, span.y);
                    }
                }
                else
                {
                    for (int i = _spanList.Count - 1; i >= 0; --i)
                    {
                        HSpan span = _spanList[i];
                        pathW.LineTo(span.startX, span.y);
                    }
                }
            }
            public void ReadRightSide(PathWriter pathW, bool topDown)
            {
                if (_rightSideChecked) throw new System.NotSupportedException();

                _rightSideChecked = true;

                if (topDown)
                {
                    int count = _spanList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        HSpan span = _spanList[i];
                        pathW.LineTo(span.endX, span.y);
                    }
                }
                else
                {
                    for (int i = _spanList.Count - 1; i >= 0; --i)
                    {
                        HSpan span = _spanList[i];
                        pathW.LineTo(span.endX, span.y);
                    }
                }
            }

            public bool HasRightColumn => this.ColNumber < this.Owner.ColumnCount - 1;
            public bool HasLeftColumn => this.ColNumber > 0;

            public bool LeftSideIsRead => _leftSideChecked;
            public bool RightSideIsRead => _rightSideChecked;


            public bool UpperPartTouchWith(int startX, int endX)
            {
#if DEBUG
                if (!dbugEvalCorners) throw new System.NotSupportedException();
#endif

                return true;
            }
#if DEBUG
            public override string ToString()
            {
                if (dbugEvalCorners)
                {

                    return _spanList.Count.ToString() +
                        ",Y:" + YTop + "," + YBottom +
                        ",X_top:" + XLeftTop + "," + XRightTop +
                        ",X_bottom:" + XLeftBottom + "," + XRightBottom;
                }
                else
                {
                    return _spanList.Count.ToString();
                }

            }
#endif
        }
        class VerticalGroup
        {
            HSpanColumn[] _hSpanColumns;

            public VerticalGroup(int groupNo, int colCount, int startY)
            {
                _hSpanColumns = new HSpanColumn[colCount];
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    _hSpanColumns[i] = new HSpanColumn(this, i);
                }

                StartY = startY;
                GroupNo = groupNo;
            }
            public int GroupNo { get; }
            public int ColumnCount => _hSpanColumns.Length;
            public int StartY { get; }
            public HSpanColumn GetColumn(int index) => _hSpanColumns[index];

            public int CurrentReadColumnIndex { get; set; }
            public HSpanColumn CurrentColumn => _hSpanColumns[CurrentReadColumnIndex];


            public void AddHSpans(List<HSpan> hspans, int startIndex, int colCount)
            {
                for (int i = 0; i < colCount; ++i)
                {
                    _hSpanColumns[i].AddHSpan(hspans[startIndex]);
                    startIndex++;
                }
            }



            public void EvaluateColumnCorners()
            {
                //can do this more than 1 times
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    _hSpanColumns[i].EvaluateCorners();
                }
            }

            public HSpanColumn BottomSideFindFirstTouchColumnFromLeft(int otherTopLeft, int otherTopRight)
            {

                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.XLeftBottom == otherTopLeft)
                    {
                        return col;
                    }
                    else if (col.XLeftBottom > otherTopLeft)
                    {
                        //
                        if (col.XRightBottom <= otherTopRight)
                        {
                            return col;
                        }
                    }
                    else
                    {
                        if (col.XRightBottom >= otherTopRight)
                        {
                            return col;
                        }
                    }
                }
                return null;
            }
            public HSpanColumn BottomSideFindFirstTouchColumnFromRight(int otherTopLeft, int otherTopRight)
            {

                for (int i = _hSpanColumns.Length - 1; i >= 0; --i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.XLeftBottom == otherTopLeft)
                    {
                        return col;
                    }
                    else if (col.XLeftBottom > otherTopLeft)
                    {
                        //
                        if (col.XRightBottom <= otherTopRight)
                        {
                            return col;
                        }
                    }
                    else
                    {
                        if (col.XRightBottom >= otherTopRight)
                        {
                            return col;
                        }
                    }
                }
                return null;
            }


            public HSpanColumn TopSideFindFirstTouchColumnFromLeft(int otherBottomLeft, int otherBottomRight)
            {
                //may be more than 1, but when first found => just return true

                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.XLeftTop == otherBottomLeft)
                    {
                        return col;
                    }
                    else if (col.XLeftTop > otherBottomLeft)
                    {
                        //
                        if (col.XRightTop <= otherBottomRight)
                        {
                            return col;
                        }
                    }
                    else
                    {
                        if (col.XRightTop >= otherBottomRight)
                        {
                            return col;
                        }
                    }
                }
                return null;
            }
            public HSpanColumn TopSideFindFirstTouchColumnFromRight(int otherBottomLeft, int otherBottomRight)
            {
                //may be more than 1, but when first found => just return true

                for (int i = _hSpanColumns.Length - 1; i >= 0; --i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.XLeftTop == otherBottomLeft)
                    {
                        return col;
                    }
                    else if (col.XLeftTop > otherBottomLeft)
                    {
                        //
                        if (col.XRightTop <= otherBottomRight)
                        {
                            return col;
                        }
                    }
                    else
                    {
                        if (col.XRightTop >= otherBottomRight)
                        {
                            return col;
                        }
                    }
                }
                return null;
            }
#if DEBUG
            public override string ToString()
            {
                return StartY + " ,col=" + ColumnCount;
            }
#endif
        }

        enum ColumnWalkDirection
        {
            Left,
            Right,
            Down,
            Up,
        }


        class MultiColumnSpans
        {
            List<VerticalGroup> _verticalGroupList = new List<VerticalGroup>();

            int _lastestLine = -1;
            VerticalGroup _currentVertGroup;
            public void LoadHSpans(List<HSpan> upperParts, List<HSpan> lowerParts)
            {
                int spanSort(HSpan sp1, HSpan sp2)
                {
                    //NESTED METHOD
                    //sort  asc
                    if (sp1.y > sp2.y)
                    {
                        return 1;
                    }
                    else if (sp1.y < sp2.y)
                    {
                        return -1;
                    }
                    else
                    {
                        return sp1.startX.CompareTo(sp2.startX);
                    }
                }

                //1.
                upperParts.Sort(spanSort);
                lowerParts.Sort(spanSort);

                //2. separate into columns

                LoadHSpans(upperParts);
                LoadHSpans(lowerParts);
            }
            public void LoadHSpans(List<HSpan> hspans)
            {
                int count = hspans.Count;
                if (count == 0) return;

                int startCollectIndex = 0;
                int colCount = 0;
                _lastestLine = hspans[0].y;

                for (int i = 0; i < count; ++i)
                {
                    HSpan sp = hspans[i];
                    int lineDiff = sp.y - _lastestLine;
                    switch (lineDiff)
                    {
                        case 1:
                            {
                                //go next lower line
                                //flush current collected columns

                                FlushCollectedColumns(hspans, startCollectIndex, colCount);
                                //
                                startCollectIndex = i;
                                colCount = 1;
                                _lastestLine = sp.y;
                            }
                            break;
                        case 0:
                            {
                                //sameline
                                colCount++;
                            }
                            break;
                        default:
                            throw new System.NotSupportedException();
                    }
                }

                if (startCollectIndex < count - 1)
                {
                    //flush remaining 
                    FlushCollectedColumns(hspans, startCollectIndex, colCount);
                }
            }
            void FlushCollectedColumns(List<HSpan> hspans, int start, int colCount)
            {
                if (_currentVertGroup == null ||
                    _currentVertGroup.ColumnCount != colCount)
                {
                    //start new group
                    _currentVertGroup = new VerticalGroup(_verticalGroupList.Count, colCount, hspans[start].y);
                    _verticalGroupList.Add(_currentVertGroup);
                }

                _currentVertGroup.AddHSpans(hspans, start, colCount);
            }

            public void TraceOutline(PathWriter pathW)
            {
                List<VerticalGroup> waitingRightSide = new List<VerticalGroup>();
                int vertGroupCount = _verticalGroupList.Count;
                if (vertGroupCount == 0)
                {

                }

                VerticalGroup group = _verticalGroupList[0];
                HSpanColumn currentColumn = group.GetColumn(0);

                bool topDownPhase = true;
                for (; ; )
                {
                    if (topDownPhase)
                    {
                        currentColumn.ReadLeftSide(pathW, true);

                        if (group.GroupNo < vertGroupCount - 1)
                        {
                            //this group is not the last group
                            //then find connection from group to the lower
                            VerticalGroup lower = _verticalGroupList[group.GroupNo + 1];
                            HSpanColumn foundNextColumn = lower.TopSideFindFirstTouchColumnFromLeft(currentColumn.XLeftBottom, currentColumn.XRightBottom);
                            if (foundNextColumn != null)
                            {
                                group = lower;
                                currentColumn = foundNextColumn;
                            }
                            else
                            {
                                //turn right
                            }
                        }
                        else
                        {
                            //this is the last group
                            //move to right-side
                            if (currentColumn.HasRightColumn)
                            {
                                currentColumn.ReadRightSide(pathW, false);
                                currentColumn = group.GetColumn(currentColumn.ColNumber + 1);

                            }
                            else
                            {  //so turn up                           

                                if (group.GroupNo > 1)
                                {
                                    currentColumn.ReadRightSide(pathW, false);

                                    VerticalGroup upper = _verticalGroupList[group.GroupNo - 1];
                                    HSpanColumn nextUpperCol = upper.BottomSideFindFirstTouchColumnFromRight(currentColumn.XLeftTop, currentColumn.XRightTop);
                                    group = upper;
                                    currentColumn = nextUpperCol;
                                    topDownPhase = false;
                                }

                            }

                        }
                    }
                    else
                    {
                        currentColumn.ReadRightSide(pathW, false);
                        if (group.GroupNo > 0)
                        {
                            //move up
                            VerticalGroup upper = _verticalGroupList[group.GroupNo - 1];
                            HSpanColumn nextUpperCol = upper.BottomSideFindFirstTouchColumnFromRight(currentColumn.XLeftTop, currentColumn.XRightTop);

                            group = upper;
                            currentColumn = nextUpperCol;
                            topDownPhase = false;
                        }
                        else
                        {
                            //we are on the top of column
                            //check if left side of the column is read?
                            if (!currentColumn.LeftSideIsRead)
                            {
                                currentColumn.ReadLeftSide(pathW, true);
                                if (currentColumn.HasLeftColumn)
                                {
                                    //move to left-column
                                    currentColumn = group.GetColumn(currentColumn.ColNumber - 1);
                                    if (!currentColumn.RightSideIsRead)
                                    {

                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                //complete
                                //?
                                return;
                            }
                        }
                    }

                }

            }
        }

    }
}
