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

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using CO = PixelFarm.CpuBlit.PixelProcessing.CO;

namespace PixelFarm.PathReconstruction
{

    /// <summary>
    /// solid color bucket tool
    /// </summary>
    public class ColorBucket : FloodFillBase
    {
        byte _tolerance;
        Color _fillColor;
        int _fillColorInt32;
        public ColorBucket(Color fillColor)
          : this(fillColor, 0)
        {
        }
        public ColorBucket(Color fillColor, byte tolerance)
        {
            Update(fillColor, tolerance);
        }
        public Color FillColor => _fillColor;
        public byte Tolerance => _tolerance;
        public void Update(Color fillColor, byte tolerance)
        {
            _tolerance = tolerance;
            _fillColor = fillColor;

            _fillColorInt32 =
                (_fillColor.red << CO.R_SHIFT) |
                (_fillColor.green << CO.G_SHIFT) |
                (_fillColor.blue << CO.B_SHIFT) |
                (_fillColor.alpha << CO.A_SHIFT);

            if (tolerance > 0)
            {
                _pixelEvalutor = new ToleranceMatch(tolerance);

            }
            else
            {
                _pixelEvalutor = new ExactMatch();
            }
        }
        public bool SkipActualFill
        {
            get => _skipActualFill;
            set => _skipActualFill = value;
        }

        /// <summary>
        /// fill target bmp, start at (x,y), 
        /// </summary>
        /// <param name="bmpTarget"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="output"></param>
        public void Fill(IBitmapSrc bmpTarget, int x, int y, ReconstructedRegionData output = null)
        {
            //output is optional 
            HSpan[] hspans = InternalFill(bmpTarget, x, y, output != null && output.WithHSpansTable);
            if (output != null)
            {
                output.HSpans = hspans;
            }
            if (output.WithCheckedPixelTable)
            {
                output.CheckedPixelTable = CopyPixelCheckTable(out int checkTableW, out int checkTableH);
                output.CheckedPixelTableWidth = checkTableW;
                output.CheckedPixelTableHeight = checkTableH;
            }

        }
        protected override unsafe void FillPixel(int* targetPixAddr)
        {
            *targetPixAddr = _fillColorInt32;
        }
    }

    public class MagicWand : FloodFillBase
    {
        byte _tolerance;
        public MagicWand(byte tolerance)
        {
            //no actual fill 
            _skipActualFill = true;
            Tolerance = tolerance;
        }
        public byte Tolerance
        {
            get => _tolerance;
            set
            {
                _tolerance = value;
                if (value > 0)
                {
                    _pixelEvalutor = new ToleranceMatch(value);

                }
                else
                {
                    _pixelEvalutor = new ExactMatch();
                }
            }
        }

        /// <summary>
        /// collect hspans into output region data
        /// </summary>
        /// <param name="bmpTarget"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="output"></param>
        public void CollectRegion(IBitmapSrc bmpTarget, int x, int y, ReconstructedRegionData output)
        {
            output.HSpans = InternalFill(bmpTarget, x, y, output.WithHSpansTable);
            if (output.WithCheckedPixelTable)
            {
                output.CheckedPixelTable = CopyPixelCheckTable(out int checkTableW, out int checkTableH);
                output.CheckedPixelTableWidth = checkTableW;
                output.CheckedPixelTableHeight = checkTableH;
            }
        }

    }

    /// <summary>
    /// flood fill tool
    /// </summary>
    public abstract class FloodFillBase
    {
        int _imageWidth;
        int _imageHeight;

        int _pixelCheckTableW;
        int _pixelCheckTableH;


        protected bool _skipActualFill;
        bool[] _pixelsChecked;
        protected PixelEvaluator _pixelEvalutor;

        SimpleQueue<HSpan> _hspanQueue = new SimpleQueue<HSpan>(9);

        IBitmapSrc _destImgRW;
        List<HSpan> _upperSpans = new List<HSpan>();
        List<HSpan> _lowerSpans = new List<HSpan>();

        int _yCutAt;

        void AddHSpan(HSpan hspan)
        {
            if (hspan.y >= _yCutAt)
            {
                _lowerSpans.Add(hspan);
            }
            else
            {
                _upperSpans.Add(hspan);
            }
        }

        protected bool[] CopyPixelCheckTable(out int checkTableW, out int checkTableH)
        {
            checkTableW = _pixelCheckTableW;
            checkTableH = _pixelCheckTableH;
            bool[] copyCheckTable = new bool[_pixelCheckTableW * _pixelCheckTableH];

            Array.Copy(_pixelsChecked, copyCheckTable, copyCheckTable.Length);
            return copyCheckTable;
        }
        protected HSpan[] InternalFill(IBitmapSrc bmpTarget, int x, int y, bool collectHSpans)
        {
            //set cut-point 
            if (collectHSpans)
            {
                _yCutAt = y;
                _upperSpans.Clear();
                _lowerSpans.Clear();
            }
            // 
            _pixelCheckTableW = _pixelCheckTableH = 0;//reset

            y -= _imageHeight;
            unchecked // this way we can overflow the uint on negative and get a big number
            {
                if ((uint)x >= bmpTarget.Width || (uint)y >= bmpTarget.Height)
                {
                    return null;
                }
            }
            _destImgRW = bmpTarget;

            unsafe
            {
                //review TempMemPtr
                using (PixelFarm.CpuBlit.Imaging.TempMemPtr destBufferPtr = bmpTarget.GetBufferPtr())
                {

                    _pixelCheckTableW = _imageWidth = bmpTarget.Width;
                    _pixelCheckTableH = _imageHeight = bmpTarget.Height;
                    //reset new buffer, clear mem?
                    _pixelsChecked = new bool[_imageWidth * _imageHeight];

                    int* destBuffer = (int*)destBufferPtr.Ptr;
                    int startColorBufferOffset = bmpTarget.GetBufferOffsetXY32(x, y);

                    int start_color = *(destBuffer + startColorBufferOffset);

                    _pixelEvalutor.SetStartColor(Drawing.Color.FromArgb(
                        (start_color >> CO.R_SHIFT) & 0xff,
                        (start_color >> CO.G_SHIFT) & 0xff,
                        (start_color >> CO.B_SHIFT) & 0xff));


                    LinearFill(destBuffer, x, y);


                    while (_hspanQueue.Count > 0)
                    {
                        HSpan hspan = _hspanQueue.Dequeue();

                        if (collectHSpans)
                        {
                            AddHSpan(hspan);
                        }

                        int downY = hspan.y - 1;
                        int upY = hspan.y + 1;
                        int downPixelOffset = (_imageWidth * (hspan.y - 1)) + hspan.startX;
                        int upPixelOffset = (_imageWidth * (hspan.y + 1)) + hspan.startX;
                        for (int rangeX = hspan.startX; rangeX < hspan.endX; rangeX++)
                        {
                            if (hspan.y > 0)
                            {
                                if (!_pixelsChecked[downPixelOffset])
                                {
                                    int bufferOffset = bmpTarget.GetBufferOffsetXY32(rangeX, downY);

                                    if (_pixelEvalutor.CheckPixel(*(destBuffer + bufferOffset)))
                                    {
                                        LinearFill(destBuffer, rangeX, downY);
                                    }
                                }
                            }

                            if (hspan.y < (_imageHeight - 1))
                            {
                                if (!_pixelsChecked[upPixelOffset])
                                {
                                    int bufferOffset = bmpTarget.GetBufferOffsetXY32(rangeX, upY);
                                    if (_pixelEvalutor.CheckPixel(*(destBuffer + bufferOffset)))
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
            _imageHeight = 0;//***
            _hspanQueue.Clear();
            _destImgRW = null;

            //
            return collectHSpans ? SortAndCollectHSpans() : null;
        }
        HSpan[] SortAndCollectHSpans()
        {
            int spanSort(HSpan sp1, HSpan sp2)
            {
                //NESTED METHOD
                //sort  asc,
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

            //clear
            _upperSpans.Clear();
            _lowerSpans.Clear();

            return hspans;
        }

        protected virtual unsafe void FillPixel(int* destBuffer)
        {

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

            bool doActualFill = !_skipActualFill;

            for (; ; )
            {

                if (doActualFill)
                {
                    //replace target pixel value with new fillColor                   
                    FillPixel(destBuffer + bufferOffset);
                }
                _pixelsChecked[pixelOffset] = true;
                leftFillX--;
                pixelOffset--;
                bufferOffset--;
                if (leftFillX <= 0 || (_pixelsChecked[pixelOffset]) || !_pixelEvalutor.CheckPixel(*(destBuffer + bufferOffset)))
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
                if (doActualFill)
                {
                    //replace target pixel value with new fillColor
                    FillPixel(destBuffer + bufferOffset);
                    //*(destBuffer + bufferOffset) = fillColorInt32;
                }
                _pixelsChecked[pixelOffset] = true;
                rightFillX++;
                pixelOffset++;
                bufferOffset++;
                if (rightFillX >= _imageWidth || _pixelsChecked[pixelOffset] || !_pixelEvalutor.CheckPixel(*(destBuffer + bufferOffset)))
                {
                    break;
                }
            }

            _hspanQueue.Enqueue(new HSpan(leftFillX, rightFillX, y));//** 
            rightFillX--;

        }

        class SimpleQueue<T>
        {
            T[] _itemArray;
            int _size;
            int _head;
            int _shiftFactor;
            int _mask;
            //

            public SimpleQueue(int shiftFactor)
            {
                _shiftFactor = shiftFactor;
                _mask = (1 << shiftFactor) - 1;
                _itemArray = new T[1 << shiftFactor];
                _head = 0;
                _size = 0;
            }
            public int Count => _size;
            public T First => _itemArray[_head & _mask];

            public void Clear() => _head = 0;

            public void Enqueue(T itemToQueue)
            {
                if (_size == _itemArray.Length)
                {
                    int headIndex = _head & _mask;
                    _shiftFactor += 1;
                    _mask = (1 << _shiftFactor) - 1;
                    T[] newArray = new T[1 << _shiftFactor];
                    // copy the from head to the end
                    Array.Copy(_itemArray, headIndex, newArray, 0, _size - headIndex);
                    // copy form 0 to the size
                    Array.Copy(_itemArray, 0, newArray, _size - headIndex, headIndex);
                    _itemArray = newArray;
                    _head = 0;
                }
                _itemArray[(_head + (_size++)) & _mask] = itemToQueue;
            }

            public T Dequeue()
            {
                int headIndex = _head & _mask;
                T firstItem = _itemArray[headIndex];
                if (_size > 0)
                {
                    _head++;
                    _size--;
                }
                return firstItem;
            }
        }
    }


    /// <summary>
    /// horizontal (scanline) span
    /// </summary>
    public struct HSpan
    {
        public readonly int startX;

        /// <summary>
        /// BEFORE touch endX, not include!
        /// </summary>
        public readonly int endX;

        public readonly int y;

        public HSpan(int startX, int endX, int y)
        {
            this.startX = startX;
            this.endX = endX;
            this.y = y;

            //spanLen= endX-startX
        }

        internal bool HorizontalTouchWith(int otherStartX, int otherEndX)
        {
            return HorizontalTouchWith(this.startX, this.endX, otherStartX, otherEndX);
        }
        internal static bool HorizontalTouchWith(int x0, int x1, int x2, int x3)
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


}
