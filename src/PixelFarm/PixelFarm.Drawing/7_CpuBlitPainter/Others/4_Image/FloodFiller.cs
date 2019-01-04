//BSD, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.Imaging
{
    public class FloodFill
    {
        int _imageWidth;
        int _imageHeight;

        byte _tolerance0To255;
        Color _fillColor;
        bool[] _pixelsChecked;
        FillingRule _fillRule;
        Queue<Range> _ranges = new Queue<Range>(9);
        IBitmapSrc _destImgRW;

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
                       (g >= _green_min) && (r <= _green_max) &&
                       (b >= _blue_min) && (r <= _blue_max);


                //return (destBuffer[bufferOffset] >= (startColor.red - tolerance0To255)) && destBuffer[bufferOffset] <= (startColor.red + tolerance0To255) &&
                //    (destBuffer[bufferOffset + 1] >= (startColor.green - tolerance0To255)) && destBuffer[bufferOffset + 1] <= (startColor.green + tolerance0To255) &&
                //    (destBuffer[bufferOffset + 2] >= (startColor.blue - tolerance0To255)) && destBuffer[bufferOffset + 2] <= (startColor.blue + tolerance0To255);
            }
        }

        struct Range
        {
            public readonly int startX;
            public readonly int endX;
            public readonly int y;
            public Range(int startX, int endX, int y)
            {
                this.startX = startX;
                this.endX = endX;
                this.y = y;
            }
        }


        public FloodFill(Color fillColor)
            : this(fillColor, 0)
        {
        }
        public FloodFill(Color fillColor, byte tolerance)
        {
            //

            Update(fillColor, tolerance);
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

                    while (_ranges.Count > 0)
                    {


                        Range range = _ranges.Dequeue();
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

            _imageHeight = 0;//reset
            _ranges.Clear();
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
            _ranges.Enqueue(new Range(leftFillX, rightFillX, y));
        }
    }
}
