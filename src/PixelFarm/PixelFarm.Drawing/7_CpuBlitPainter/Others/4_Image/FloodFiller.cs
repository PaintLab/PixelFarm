//MIT, 2014-present, WinterDev
//MatterHackers 
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------


using System.Collections.Generic;
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.Imaging
{
    /// <summary>
    /// flood fill tool
    /// </summary>
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
        ConnectedHSpans _connectedHSpans;

        public FloodFill(Color fillColor)
            : this(fillColor, 0)
        {
        }
        public FloodFill(Color fillColor, byte tolerance)
        {
            Update(fillColor, tolerance);
        }

        public void SetOutput(ConnectedHSpans output)
        {
            _connectedHSpans = output;
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

                    bool addToOutputRanges = _connectedHSpans != null;
                    if (addToOutputRanges)
                    {
                        _connectedHSpans.Clear();
                        _connectedHSpans.SetYCut(y);
                    }


                    while (_ranges.Count > 0)
                    {


                        HSpan range = _ranges.Dequeue();

                        if (addToOutputRanges)
                        {
                            _connectedHSpans.AddHSpan(range);
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

    partial class FloodFill
    {

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
                OwnerVerticalGroup = owner;
            }
            public VerticalGroup OwnerVerticalGroup { get; }
            public int ColNumber { get; }

            public void AddHSpan(HSpan span)
            {
                _spanList.Add(span);
                XLeftBottom = span.startX;
                XRightBottom = span.endX;
                YBottom = span.y + 1;
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


            public void ReadLeftSide(RawPath pathW, bool topDown)
            {
                //read once
                if (_leftSideChecked) throw new System.NotSupportedException();

                _leftSideChecked = true;

                if (topDown)
                {
                    int count = _spanList.Count;

                    RawPath.BeginLoadSegmentPoints(pathW);
                    for (int i = 0; i < count; ++i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.startX, span.y);
                    }
                    RawPath.EndLoadSegmentPoints(pathW);
                }
                else
                {
                    RawPath.BeginLoadSegmentPoints(pathW);
                    for (int i = _spanList.Count - 1; i >= 0; --i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.startX, span.y);
                    }
                    RawPath.EndLoadSegmentPoints(pathW);
                }
            }
            public void ReadRightSide(RawPath pathW, bool topDown)
            {
                if (_rightSideChecked) throw new System.NotSupportedException();

                _rightSideChecked = true;

                if (topDown)
                {
                    RawPath.BeginLoadSegmentPoints(pathW);
                    int count = _spanList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.endX, span.y);
                    }
                    RawPath.EndLoadSegmentPoints(pathW);
                }
                else
                {
                    RawPath.BeginLoadSegmentPoints(pathW);
                    for (int i = _spanList.Count - 1; i >= 0; --i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.endX, span.y);
                    }
                    RawPath.EndLoadSegmentPoints(pathW);
                }
            }

            public bool HasRightColumn => this.ColNumber < this.OwnerVerticalGroup.ColumnCount - 1; //not the last one
            public bool HasLeftColumn => this.ColNumber > 0;

            public bool LeftSideIsRead => _leftSideChecked;
            public bool RightSideIsRead => _rightSideChecked;


            public HSpanColumn FindLeftColumn() => HasLeftColumn ? OwnerVerticalGroup.GetColumn(ColNumber - 1) : null;
            public HSpanColumn FindRightColumn() => HasRightColumn ? OwnerVerticalGroup.GetColumn(ColNumber + 1) : null;

            public HSpanColumn FindLeftLowerColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsLastGroup ? null :
                        ownerGroup.GetLowerGroup().TopSideFindFirstTouchColumnFromLeft(this.YBottom, this.XLeftBottom, this.XRightBottom);
            }
            public HSpanColumn FindRightLowerColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsLastGroup ? null :
                       ownerGroup.GetLowerGroup().TopSideFindFirstTouchColumnFromRight(this.YBottom, this.XLeftBottom, this.XRightBottom);
            }
            public HSpanColumn FindRightUpperColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsFirstGroup ? null :
                       ownerGroup.GetUpperGroup().BottomSideFindFirstTouchColumnFromRight(this.YTop, this.XLeftTop, this.XRightTop);
            }
            public HSpanColumn FindLeftUpperColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsFirstGroup ? null :
                       ownerGroup.GetUpperGroup().BottomSideFindFirstTouchColumnFromLeft(this.YTop, this.XLeftTop, this.XRightTop);
            }
            public ReadSide FindUnreadSide()
            {
                ReadSide incompleteSide = ReadSide.None;
                if (!_leftSideChecked)
                {
                    incompleteSide |= ReadSide.Left;
                }
                if (!_rightSideChecked)
                {
                    incompleteSide |= ReadSide.Right;
                }

                return incompleteSide;
            }

            /// <summary>
            /// check if the bottom side of this group touch with specific range 
            /// </summary>
            /// <param name="lowerGroupTopLeft"></param>
            /// <param name="lowerGroupTopRight"></param>
            /// <returns></returns>
            public bool BottomSideTouchWith(int lowerGroupTop, int lowerGroupTopLeft, int lowerGroupTopRight)
            {
                //[     THIS group    ]
                //---------------------                
                //[ other (lower)group]

                if (lowerGroupTop != this.YBottom)
                {
                    return false;
                }
                if (this.XLeftBottom == lowerGroupTopLeft)
                {
                    return true;
                }
                else if (this.XLeftBottom > lowerGroupTopLeft)
                {
                    return this.XLeftBottom <= lowerGroupTopRight;
                }
                else
                {
                    return this.XRightBottom >= lowerGroupTopLeft;
                }

            }
            /// <summary>
            /// check if the top side of this group touch with specific range)
            /// </summary>
            /// <param name="upperBottomLeft"></param>
            /// <param name="upperBottomRight"></param>
            /// <returns></returns>
            public bool TopSideTouchWith(int upperBottom, int upperBottomLeft, int upperBottomRight)
            {

                //[ other (lower)group]
                //---------------------  
                //[     THIS group    ] 

                //find the first column that its top side touch with 
                //another uppper group   

                if (upperBottom != this.YTop)
                {
                    return false;
                }

                if (this.XLeftTop == upperBottomLeft)
                {
                    return true;
                }
                else if (this.XLeftTop > upperBottomLeft)
                {
                    //
                    return this.XLeftTop <= upperBottomRight;
                }
                else
                {
                    return this.XRightTop >= upperBottomLeft;
                }
            }
#if DEBUG
            public override string ToString()
            {
                if (dbugEvalCorners)
                {

                    return "grp:" + OwnerVerticalGroup.GroupNo + ",col:" + ColNumber +
                        ",Y:" + YTop + "," + YBottom +
                        ",X_top:" + XLeftTop + "," + XRightTop +
                        ",X_bottom:" + XLeftBottom + "," + XRightBottom;
                }
                else
                {
                    return "grp:" + OwnerVerticalGroup.GroupNo + ",col:" + ColNumber;
                }

            }
#endif
        }

        [System.Flags]
        enum ReadSide : byte
        {
            None = 0,
            Left = 1,
            Right = 1 << 1,//2 
            LeftAndRight = Left | Right //3
        }
        struct Remaining
        {
            public readonly HSpanColumn column;
            public readonly ReadSide unreadSide;
            public Remaining(HSpanColumn column, ReadSide unreadSide)
            {
                this.column = column;
                this.unreadSide = unreadSide;
            }
        }

        class VerticalGroup
        {
            HSpanColumn[] _hSpanColumns;
            bool _completeAll;
            VerticalGroupList _ownerVertGroupList;
            public VerticalGroup(VerticalGroupList ownerVertGroupList, int groupNo,
                HSpan[] hspans,
                int startIndex,
                int colCount)
            {
                _ownerVertGroupList = ownerVertGroupList;
                _hSpanColumns = new HSpanColumn[colCount];
                GroupNo = groupNo;
                int index = startIndex;
                for (int i = 0; i < colCount; ++i)
                {
                    var col = new HSpanColumn(this, i);
                    col.AddHSpan(hspans[index]);
                    _hSpanColumns[i] = col;
                    index++;
                }
                StartY = hspans[startIndex].y;
            }
            public int ColumnCount => _hSpanColumns.Length;
            public int GroupNo { get; }

            public int StartY { get; }
            public HSpanColumn GetColumn(int index) => _hSpanColumns[index];

            public int CurrentReadColumnIndex { get; set; }
            public HSpanColumn CurrentColumn => _hSpanColumns[CurrentReadColumnIndex];


            HSpanColumn FindTouchColumn(HSpan newspan, ref int colIndex)
            {

                for (int i = colIndex; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.BottomSideTouchWith(newspan.y, newspan.startX, newspan.endX))
                    {
                        //found
                        colIndex = i;
                        return col;
                    }
                }
                //----

                if (colIndex > 0)
                {
                    //we didn't start from the first
                    for (int i = 0; i < colIndex; ++i)
                    {
                        HSpanColumn col = _hSpanColumns[i];
                        if (col.BottomSideTouchWith(newspan.y, newspan.startX, newspan.endX))
                        {
                            //found
                            colIndex = i;
                            return col;
                        }
                    }
                }

                //not found
                return null;
            }

            public bool AddHSpans(HSpan[] hspans, int startIndex, int count)
            {

                int index = startIndex;
                //we must touch one by one
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    HSpan hspan = hspans[index];
                    if (!col.BottomSideTouchWith(hspan.y, hspan.startX, hspan.endX))
                    {
                        //found some 'untouch column'
                        //break all 
                        //need another vertical group
                        return false;
                    }
                    index++;
                }
                //---
                //pass all
                index = startIndex; //reset
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    col.AddHSpan(hspans[index]);
                    index++;
                }
                return true;
            }


            public bool IsLastGroup => GroupNo == _ownerVertGroupList.Count - 1;
            public bool IsFirstGroup => GroupNo == 0;
            public VerticalGroup GetUpperGroup() => IsFirstGroup ? null : _ownerVertGroupList.GetGroup(GroupNo - 1);
            public VerticalGroup GetLowerGroup() => IsLastGroup ? null : _ownerVertGroupList.GetGroup(GroupNo + 1);

            public void EvaluateColumnCorners()
            {
                //can do this more than 1 times
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    _hSpanColumns[i].EvaluateCorners();
                }
            }

            public HSpanColumn BottomSideFindFirstTouchColumnFromLeft(int lowerGroupTop, int lowerGroupTopLeft, int lowerGroupTopRight)
            {
                //[     THIS group    ]
                //---------------------                
                //[ other (lower)group]
                //
                //find the first column that its bottom side touch with 
                //another lower group               

                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.BottomSideTouchWith(lowerGroupTop, lowerGroupTopLeft, lowerGroupTopRight))
                    {
                        return col;
                    }
                }
                return null;
            }
            public HSpanColumn BottomSideFindFirstTouchColumnFromRight(int lowerGroupTop, int lowerGroupTopLeft, int lowerGroupTopRight)
            {
                //[     THIS group    ]
                //---------------------                
                //[ other (lower)group]
                //
                //find the first column that its bottom side touch with 
                //another lower group  
                for (int i = _hSpanColumns.Length - 1; i >= 0; --i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.BottomSideTouchWith(lowerGroupTop, lowerGroupTopLeft, lowerGroupTopRight))
                    {
                        return col;
                    }
                }
                return null;
            }
            public HSpanColumn TopSideFindFirstTouchColumnFromLeft(int upperBottom, int upperBottomLeft, int upperBottomRight)
            {

                //[ other (lower)group]
                //---------------------  
                //[     THIS group    ]


                //find the first column that its top side touch with 
                //another uppper group  
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.TopSideTouchWith(upperBottom, upperBottomLeft, upperBottomRight))
                    {
                        return col;
                    }
                }
                return null;
            }
            public HSpanColumn TopSideFindFirstTouchColumnFromRight(int upperBottom, int upperBottomLeft, int upperBottomRight)
            {
                //[ other (lower)group]
                //---------------------  
                //[     THIS group    ]


                //find the first column that its top side touch with 
                //another uppper group 

                for (int i = _hSpanColumns.Length - 1; i >= 0; --i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.TopSideTouchWith(upperBottom, upperBottomLeft, upperBottomRight))
                    {
                        return col;
                    }
                }
                return null;
            }


            public void CollectIncompleteRead(List<Remaining> incompleteColumns)
            {
                if (_completeAll) return;
                //
                bool hasSomeIncompleteColumn = false;
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn hspanCol = _hSpanColumns[i];
                    ReadSide incompleteSide = hspanCol.FindUnreadSide();
                    if (incompleteSide != ReadSide.None)
                    {
                        hasSomeIncompleteColumn = true;
                        incompleteColumns.Add(new Remaining(hspanCol, incompleteSide));
                    }
                }
                _completeAll = !hasSomeIncompleteColumn;
            }
#if DEBUG
            public override string ToString()
            {
                return StartY + " ,col=" + ColumnCount;
            }
#endif
        }


        struct VerticalGroupSeparator
        {
            int _lastestLine;
            VerticalGroup _currentVertGroup;
            VerticalGroupList _verticalGroupList;
            public VerticalGroupSeparator(VerticalGroupList verticalGroupList)
            {
                _lastestLine = -1;
                _currentVertGroup = null;
                _verticalGroupList = verticalGroupList;
            }

            public void Separate(HSpan[] hspans)
            {
                int count = hspans.Length;
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
            void FlushCollectedColumns(HSpan[] hspans, int start, int colCount)
            {
                if (_currentVertGroup == null ||
                    _currentVertGroup.ColumnCount != colCount)
                {
                    //start new group
                    //create new                     
                    _verticalGroupList.Append(
                        _currentVertGroup = new VerticalGroup(_verticalGroupList, _verticalGroupList.Count, hspans, start, colCount));
                    return;
                }

                if (_currentVertGroup.AddHSpans(hspans, start, colCount))
                {
                    //pass
                    return;
                }

                //create and add to a new vertical group
                _verticalGroupList.Append(
                        _currentVertGroup = new VerticalGroup(_verticalGroupList, _verticalGroupList.Count, hspans, start, colCount));
            }
        }

        class VerticalGroupList
        {
            List<VerticalGroup> _verticalGroupList = new List<VerticalGroup>();

            public int Count => _verticalGroupList.Count;

            public VerticalGroup GetGroup(int index) => _verticalGroupList[index];

            public void Append(VerticalGroup vertGtoup)
            {
                _verticalGroupList.Add(vertGtoup);
            }
            public void EvaluateCorners()
            {
                int j = _verticalGroupList.Count;
                for (int i = 0; i < j; ++i)
                {
                    _verticalGroupList[i].EvaluateColumnCorners();
                }
            }
            public void CollectIncompleteColumns(List<Remaining> incompleteReadList)
            {
                int j = _verticalGroupList.Count;
                for (int i = 0; i < j; ++i)
                {
                    _verticalGroupList[i].CollectIncompleteRead(incompleteReadList);
                }
            }
        }

        struct ColumnWalkerCcw
        {

            int _vertGroupCount;
            RawPath _pathWriter;
            HSpanColumn _currentCol;
            bool _latestReadOnRightSide;
            VerticalGroupList _vertGroupList;
            public ColumnWalkerCcw(VerticalGroupList verticalGroupList)
            {
                _pathWriter = null;
                _latestReadOnRightSide = false;
                _vertGroupList = verticalGroupList;
                _vertGroupCount = verticalGroupList.Count;
                _currentCol = null;
            }
            public void Bind(RawPath pathW)
            {
                _pathWriter = pathW;
            }
            public void Bind(HSpanColumn hspanCol)
            {
                _currentCol = hspanCol;
            }

            public void ReadLeftSide()
            {
                _latestReadOnRightSide = false;
                _currentCol.ReadLeftSide(_pathWriter, true);
            }
            public void ReadRightSide()
            {
                _latestReadOnRightSide = true;
                _currentCol.ReadRightSide(_pathWriter, false);
            }
            public Remaining FindReadNextColumn()
            {
                if (!_latestReadOnRightSide)
                {
                    //latest state is on LEFT side of the column
                    HSpanColumn leftLowerCol = _currentCol.FindLeftLowerColumn();
                    HSpanColumn leftCol = _currentCol.FindLeftColumn();
                    if (leftLowerCol != null)
                    {
                        if (leftCol != null)
                        {
                            HSpanColumn rightLowerCol = leftCol.FindRightLowerColumn();
                            if (leftLowerCol == rightLowerCol)
                            {
                                //if they share the same 
                                return leftCol.RightSideIsRead ?
                                            new Remaining() : //complete
                                            new Remaining(leftCol, ReadSide.Right);
                            }
                            else
                            {

                                if (leftLowerCol.LeftSideIsRead && !leftCol.RightSideIsRead)
                                {

                                    return new Remaining(leftCol, ReadSide.Right);
                                }
                            }
                        }

                        return leftLowerCol.LeftSideIsRead ?
                                new Remaining() : //complete
                                new Remaining(leftLowerCol, ReadSide.Left);
                    }
                    else
                    {    //no lower column => this is Bottom-End

                        if (!_currentCol.RightSideIsRead)
                        {
                            return new Remaining(_currentCol, ReadSide.Right);
                        }
                        else
                        {

                        }

                        return new Remaining(); //complete
                    }
                }
                else
                {
                    //latest state is on RIGHT side of the column  
                    HSpanColumn rightUpperCol = _currentCol.FindRightUpperColumn();
                    HSpanColumn rightCol = _currentCol.FindRightColumn();

                    if (rightUpperCol != null)
                    {
                        if (rightCol != null)
                        {
                            HSpanColumn leftUpperCol = rightCol.FindLeftUpperColumn();
                            if (rightUpperCol == leftUpperCol)
                            {
                                return rightCol.LeftSideIsRead ?
                                            new Remaining() : //complete
                                            new Remaining(rightCol, ReadSide.Left);
                            }
                            else
                            {
                                if (rightUpperCol.RightSideIsRead && !rightCol.LeftSideIsRead)
                                {
                                    //???
                                    return new Remaining(rightCol, ReadSide.Left);
                                }
                            }
                        }
                        return rightUpperCol.RightSideIsRead ?
                                new Remaining() :
                                new Remaining(rightUpperCol, ReadSide.Right);
                    }
                    else
                    {
                        if (rightCol != null && !rightCol.LeftSideIsRead)
                        {

                        }

                        //no upper column => this is Top-End
                        return _currentCol.LeftSideIsRead ?
                                 new Remaining() :
                                 new Remaining(_currentCol, ReadSide.Left);
                    }
                }
            }
        }

        internal class OutlineTracer
        {
            VerticalGroupList _verticalGroupList = new VerticalGroupList();

            void TraceOutlineCcw(Remaining toReadNext, RawPath rawPath, bool outside)
            {
                rawPath.BeginContour(outside);
                //if we starts on left-side of the column                
                ColumnWalkerCcw ccw = new ColumnWalkerCcw(_verticalGroupList);
                ccw.Bind(rawPath);
                //-------------------------------
                for (; ; )
                {
                    switch (toReadNext.unreadSide)
                    {
                        default:
                            throw new System.NotSupportedException();
                        case ReadSide.Left:
                            ccw.Bind(toReadNext.column);
                            ccw.ReadLeftSide();
                            break;
                        case ReadSide.Right:
                            ccw.Bind(toReadNext.column);
                            ccw.ReadRightSide();
                            break;
                        case ReadSide.None:
                            //complete
                            rawPath.EndContour();
                            return;
                    }
                    toReadNext = ccw.FindReadNextColumn();
                }

            }
            /// <summary>
            /// trace outline counter-clockwise
            /// </summary>
            /// <param name="pathW"></param>
            public void TraceOutline(HSpan[] sortedHSpans, RawPath pathW)
            {
                var sep = new VerticalGroupSeparator(_verticalGroupList);
                sep.Separate(sortedHSpans);

                int vertGroupCount = _verticalGroupList.Count;
                if (vertGroupCount == 0) return;

                _verticalGroupList.EvaluateCorners();


                List<Remaining> incompleteReadList = new List<Remaining>();
                TraceOutlineCcw(new Remaining(_verticalGroupList.GetGroup(0).GetColumn(0), ReadSide.Left), pathW, true);

                TRACE_AGAIN://**

                //check if the shape have hole(s) 
                _verticalGroupList.CollectIncompleteColumns(incompleteReadList);

                if (incompleteReadList.Count > 0)
                {
                    //this should be a hole
                    Remaining incompleteRead = incompleteReadList[0];
                    switch (incompleteRead.unreadSide)
                    {
                        //?should not occur
                        case ReadSide.LeftAndRight:
                            {
                                TraceOutlineCcw(new Remaining(incompleteRead.column, ReadSide.Left), pathW, false);
                                incompleteReadList.Clear();

                                goto TRACE_AGAIN;
                            }
                        case ReadSide.Left:
                        case ReadSide.Right:
                            {
                                TraceOutlineCcw(incompleteRead, pathW, false);
                                incompleteReadList.Clear();
                                goto TRACE_AGAIN;

                            }
                    }
                }
                else
                {
                    //complete all
                }
            }
        }

    }


    //------------------------------------------------------------------------------
    public static class RawPathExtensions
    {
        public static void Simplify(this RawPath rawPath, float tolerance = 0.5f, bool heighQualityEnable = false)
        {

            int j = rawPath._contours.Count;
            for (int i = 0; i < j; ++i)
            {
                RawContour contour = rawPath._contours[i];
                var simplifiedPoints = PixelFarm.CpuBlit.VertexProcessing.SimplificationHelpers.Simplify(
                     contour._xyCoords,
                     (p1, p2) => p1 == p2,
                     p => p.x,
                     p => p.y,
                     tolerance,
                     heighQualityEnable);
                //replace current raw contour with the new one
#if DEBUG
                System.Diagnostics.Debug.WriteLine("simplification before:" + contour._xyCoords.Count + ",after" + simplifiedPoints.Count);
#endif

                //create a new raw contour, 
                //but you can replace internal data of the old contour too,
                RawContour newContour = new RawContour();
                newContour.IsOutside = contour.IsOutside;
                foreach (var point in simplifiedPoints)
                {
                    newContour.AddPoint(point);
                }
                rawPath._contours[i] = newContour;
            }
        }
    }
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
        internal bool HasHorizontalTouch(int otherStartX, int otherEndX)
        {
            return HasHorizontalTouch(this.startX, this.endX, otherStartX, otherEndX);
        }
        internal static bool HasHorizontalTouch(int x0, int x1, int x2, int x3)
        {
            if (x0 == x2)
            {
                return true;
            }
            else if (x0 > x2)
            {
                //
                return x0 <= x3;
            }
            else
            {
                return x1 >= x2;
            }
        }

#if DEBUG
        public override string ToString()
        {
            return "line:" + y + ", x_start=" + startX + ",end_x=" + endX + ",len=" + (endX - startX);
        }
#endif
    }


    public class ConnectedHSpans
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


        public void ReconstructPath(RawPath rawPath)
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
            _upperSpans.Sort(spanSort);
            _lowerSpans.Sort(spanSort);

            HSpan[] hspans = new HSpan[_upperSpans.Count + _lowerSpans.Count];
            _upperSpans.CopyTo(hspans);
            _lowerSpans.CopyTo(hspans, _upperSpans.Count);


            var outlineTracer = new FloodFill.OutlineTracer();
            outlineTracer.TraceOutline(hspans, rawPath);
        }

    }


    public class RawPath
    {
        internal List<RawContour> _contours = new List<RawContour>();
        RawContour _currentContour;
        public RawPath() { }
        public void BeginContour(bool outside)
        {
            _currentContour = new RawContour();
            _currentContour.IsOutside = outside;
            _contours.Add(_currentContour);
        }
        public void EndContour()
        {

        }

        public void AppendPoint(int x, int y) => _currentContour.AddPoint(x, y);


        public int ContourCount => _contours.Count;

        public RawContour GetContour(int index) => _contours[index];

        internal static void BeginLoadSegmentPoints(RawPath rawPath) => rawPath.OnBeginLoadSegmentPoints();

        internal static void EndLoadSegmentPoints(RawPath rawPath) => rawPath.OnEndLoadSegmentPoints();


        protected virtual void OnBeginLoadSegmentPoints()
        {
            //for hinting
            //that the following AppendPoints come from the same vertical column side
        }
        protected virtual void OnEndLoadSegmentPoints()
        {
            //for hinting
            //that the following AppendPoints come from the same vertical column side
        }
        public void MakeVxs(VertexStore vxs)
        {
            int contourCount = _contours.Count;

            for (int i = 0; i < contourCount; ++i)
            {
                //each contour
                RawContour contour = _contours[i];
                List<Point> xyCoords = contour._xyCoords;
                int count = xyCoords.Count;

                if (count > 1)
                {
                    if (contour.IsOutside)
                    {
                        Point p = xyCoords[0];
                        vxs.AddMoveTo(p.x, p.y);
                        for (int n = 1; n < count; ++n)
                        {
                            p = xyCoords[n];
                            vxs.AddLineTo(p.x, p.y);
                        }
                        vxs.AddCloseFigure();
                    }
                    else
                    {
                        Point p = xyCoords[count - 1];
                        vxs.AddMoveTo(p.x, p.y);
                        for (int n = count - 1; n >= 0; --n)
                        {
                            p = xyCoords[n];
                            vxs.AddLineTo(p.x, p.y);
                        }
                        vxs.AddCloseFigure();
                    }

                }
            }

        }
    }

    public class RawContour
    {
        internal List<Point> _xyCoords = new List<Point>();

        public RawContour() { }
        public bool IsOutside { get; set; }
        public virtual void AddPoint(int x, int y)
        {
            _xyCoords.Add(new Point(x, y));
        }
        public virtual void AddPoint(Point p)
        {
            _xyCoords.Add(p);
        }
    }

}
