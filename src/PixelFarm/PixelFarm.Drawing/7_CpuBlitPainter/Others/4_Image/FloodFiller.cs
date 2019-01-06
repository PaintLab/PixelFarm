//MIT, 2014-present, WinterDev

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

        public class RawPath
        {
            List<RawContour> _contours = new List<RawContour>();
            RawContour _currentContour;
            public RawPath() { }
            public void BeginContour()
            {
                _currentContour = new RawContour();
                _contours.Add(_currentContour);
            }
            public void EndContour()
            {

            }
            public void AppendPoint(int x, int y) => _currentContour.AddPoint(x, y);
            public RawContour CurrentContour => _currentContour;

            public void MakeVxs(VertexStore vxs)
            {
                int j = _contours.Count;
                for (int i = 0; i < j; ++i)
                {
                    List<int> xyCoords = _contours[i]._xyCoords;
                    int count = xyCoords.Count;
                    if (count > 2)
                    {

                        vxs.AddMoveTo(xyCoords[0], xyCoords[1]);

                        for (int n = 2; n < count;)
                        {
                            vxs.AddLineTo(xyCoords[n], xyCoords[n + 1]);
                            n += 2;
                        }

                        vxs.AddCloseFigure();
                    }
                }

            }
        }

        public class RawContour
        {
            internal List<int> _xyCoords = new List<int>();
            public virtual void AddPoint(int x, int y)
            {
                _xyCoords.Add(x);
                _xyCoords.Add(y);
            }
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


            public void ReconstructPath(RawPath rawPath)
            {
                OutlineTracer outlineTracer = new OutlineTracer();
                outlineTracer.LoadHSpans(_upperSpans, _lowerSpans);
                outlineTracer.TraceOutline(rawPath);
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
                OwnerVerticalGroup = owner;
            }
            public VerticalGroup OwnerVerticalGroup { get; }
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


            public void ReadLeftSide(RawPath pathW, bool topDown)
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
                        pathW.AppendPoint(span.startX, span.y);
                    }
                }
                else
                {
                    for (int i = _spanList.Count - 1; i >= 0; --i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.startX, span.y);
                    }
                }
            }
            public void ReadRightSide(RawPath pathW, bool topDown)
            {
                if (_rightSideChecked) throw new System.NotSupportedException();

                _rightSideChecked = true;

                if (topDown)
                {
                    int count = _spanList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.endX, span.y);
                    }
                }
                else
                {
                    for (int i = _spanList.Count - 1; i >= 0; --i)
                    {
                        HSpan span = _spanList[i];
                        pathW.AppendPoint(span.endX, span.y);
                    }
                }
            }

            public bool HasRightColumn => this.ColNumber < this.OwnerVerticalGroup.ColumnCount - 1;
            public bool HasLeftColumn => this.ColNumber > 0;

            public bool LeftSideIsRead => _leftSideChecked;
            public bool RightSideIsRead => _rightSideChecked;


            public HSpanColumn FindLeftColumn() => HasLeftColumn ? OwnerVerticalGroup.GetColumn(ColNumber - 1) : null;
            public HSpanColumn FindRightColumn() => HasRightColumn ? OwnerVerticalGroup.GetColumn(ColNumber + 1) : null;

            public HSpanColumn FindLeftLowerColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsLastGroup ? null :
                        ownerGroup.GetLowerGroup().TopSideFindFirstTouchColumnFromLeft(this.XLeftBottom, this.XRightBottom);
            }
            public HSpanColumn FindRightLowerColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsLastGroup ? null :
                       ownerGroup.GetLowerGroup().TopSideFindFirstTouchColumnFromRight(this.XLeftBottom, this.XRightBottom);
            }
            public HSpanColumn FindRightUpperColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsFirstGroup ? null :
                       ownerGroup.GetUpperGroup().BottomSideFindFirstTouchColumnFromRight(this.XLeftTop, this.XRightTop);
            }
            public HSpanColumn FindLeftUpperColumn()
            {
                VerticalGroup ownerGroup = OwnerVerticalGroup;
                return ownerGroup.IsFirstGroup ? null :
                       ownerGroup.GetUpperGroup().BottomSideFindFirstTouchColumnFromLeft(this.XLeftTop, this.XRightTop);
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
            public bool BottomSideTouchWith(int lowerGroupTopLeft, int lowerGroupTopRight)
            {
                //[     THIS group    ]
                //---------------------                
                //[ other (lower)group]

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
                return false;
            }
            /// <summary>
            /// check if the top side of this group touch with specific range)
            /// </summary>
            /// <param name="upperBottomLeft"></param>
            /// <param name="upperBottomRight"></param>
            /// <returns></returns>
            public bool TopSideTouchWith(int upperBottomLeft, int upperBottomRight)
            {

                //[ other (lower)group]
                //---------------------  
                //[     THIS group    ] 

                //find the first column that its top side touch with 
                //another uppper group   
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
                return false;
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
            public VerticalGroup(VerticalGroupList ownerVertGroupList, int groupNo, int colCount, int startY)
            {
                _ownerVertGroupList = ownerVertGroupList;
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

            public HSpanColumn BottomSideFindFirstTouchColumnFromLeft(int lowerGroupTopLeft, int lowerGroupTopRight)
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
                    if (col.BottomSideTouchWith(lowerGroupTopLeft, lowerGroupTopRight))
                    {
                        return col;
                    }
                }
                return null;
            }
            public HSpanColumn BottomSideFindFirstTouchColumnFromRight(int lowerGroupTopLeft, int lowerGroupTopRight)
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
                    if (col.BottomSideTouchWith(lowerGroupTopLeft, lowerGroupTopRight))
                    {
                        return col;
                    }
                }
                return null;
            }
            public HSpanColumn TopSideFindFirstTouchColumnFromLeft(int upperBottomLeft, int upperBottomRight)
            {

                //[ other (lower)group]
                //---------------------  
                //[     THIS group    ]


                //find the first column that its top side touch with 
                //another uppper group  
                for (int i = 0; i < _hSpanColumns.Length; ++i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.TopSideTouchWith(upperBottomLeft, upperBottomRight))
                    {
                        return col;
                    }
                }
                return null;
            }
            public HSpanColumn TopSideFindFirstTouchColumnFromRight(int upperBottomLeft, int upperBottomRight)
            {
                //[ other (lower)group]
                //---------------------  
                //[     THIS group    ]


                //find the first column that its top side touch with 
                //another uppper group 

                for (int i = _hSpanColumns.Length - 1; i >= 0; --i)
                {
                    HSpanColumn col = _hSpanColumns[i];
                    if (col.TopSideTouchWith(upperBottomLeft, upperBottomRight))
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
                    _currentVertGroup = new VerticalGroup(_verticalGroupList, _verticalGroupList.Count, colCount, hspans[start].y);
                    _verticalGroupList.Append(_currentVertGroup);
                }

                _currentVertGroup.AddHSpans(hspans, start, colCount);
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
                        }
                        return leftLowerCol.LeftSideIsRead ?
                                new Remaining() : //complete
                                new Remaining(leftLowerCol, ReadSide.Left);
                    }
                    else
                    {    //no lower column => this is Bottom-End
                        return _currentCol.RightSideIsRead ?
                                new Remaining() : //complete
                                new Remaining(_currentCol, ReadSide.Right);
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
                        }
                        return rightUpperCol.RightSideIsRead ?
                                new Remaining() :
                                new Remaining(rightUpperCol, ReadSide.Right);
                    }
                    else
                    {
                        //no upper column => this is Top-End
                        return _currentCol.LeftSideIsRead ?
                                 new Remaining() :
                                 new Remaining(_currentCol, ReadSide.Left);
                    }
                }
            }
        }



        class OutlineTracer
        {
            VerticalGroupList _verticalGroupList = new VerticalGroupList();
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
                var sep = new VerticalGroupSeparator(_verticalGroupList);
                sep.LoadHSpans(upperParts);
                sep.LoadHSpans(lowerParts);
            }

            void TraceOutlineCcw(Remaining toReadNext, RawPath pathW)
            {
                pathW.BeginContour();
                //if we starts on left-side of the column                
                ColumnWalkerCcw ccw = new ColumnWalkerCcw(_verticalGroupList);
                ccw.Bind(pathW);

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
                            pathW.EndContour();
                            return;
                    }
                    toReadNext = ccw.FindReadNextColumn();
                }

            }
            /// <summary>
            /// trace outline counter-clockwise
            /// </summary>
            /// <param name="pathW"></param>
            public void TraceOutline(RawPath pathW)
            {

                int vertGroupCount = _verticalGroupList.Count;
                if (vertGroupCount == 0) return;

                _verticalGroupList.EvaluateCorners();


                List<Remaining> incompleteReadList = new List<Remaining>();
                TraceOutlineCcw(new Remaining(_verticalGroupList.GetGroup(0).GetColumn(0), ReadSide.Left), pathW);

                TRACE_AGAIN://**

                //check if the shape have hole(s) 
                _verticalGroupList.CollectIncompleteColumns(incompleteReadList);
                if (incompleteReadList.Count > 0)
                {
                    //this should be a hole
                    Remaining incompleteRead = incompleteReadList[0];
                    switch (incompleteRead.unreadSide)
                    {
                        case ReadSide.LeftAndRight:
                            throw new System.Exception();//?should not occur

                        //
                        case ReadSide.Left:
                        case ReadSide.Right:
                            {
                                TraceOutlineCcw(incompleteRead, pathW);

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
}
