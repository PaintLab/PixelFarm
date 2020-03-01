//BSD, 2014-present, WinterDev
//ArthurHub, Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

namespace PixelFarm.Drawing.WinGdi
{
    partial class GdiPlusDrawBoard
    {
        int _left;
        int _top;
        int _right;
        int _bottom;
        int _canvasOriginX = 0;
        int _canvasOriginY = 0;
        Rectangle _invalidateArea;

        bool _isEmptyInvalidateArea;
        //--------------------------------------------------------------------
        public override void SetCanvasOrigin(int x, int y)
        {
            _gdigsx.SetCanvasOrigin(x, y);
            //----------- 
            int total_dx = x - _canvasOriginX;
            int total_dy = y - _canvasOriginY;

            _canvasOriginX = x;
            _canvasOriginY = y;
        }

        public override int OriginX => _canvasOriginX;
        public override int OriginY => _canvasOriginY;


        /// <summary>
        /// Sets the clipping region of this <see cref="T:System.Drawing.Graphics"/> to the result of the specified operation combining the current clip region and the rectangle specified by a <see cref="T:System.Drawing.RectangleF"/> structure.
        /// </summary>
        /// <param name="rect"><see cref="T:System.Drawing.RectangleF"/> structure to combine. </param>
        /// <param name="combineMode">Member of the <see cref="T:System.Drawing.Drawing2D.CombineMode"/> enumeration that specifies the combining operation to use. </param>
        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            _gdigsx.SetClipRect(rect, combineMode);
        }
        public bool IntersectsWith(Rectangle clientRect)
        {
            return clientRect.IntersectsWith(_left, _top, _right, _bottom);
        }

        public override bool PushClipAreaRect(int width, int height, UpdateArea updateArea)
        {
            return _gdigsx.PushClipAreaRect(width, height, updateArea);
        }
        public override bool PushClipAreaRect(int left, int top, int width, int height, UpdateArea updateArea)
        {
            return _gdigsx.PushClipAreaRect(left, top, width, height, updateArea);
        }
        public override void PopClipAreaRect()
        {
#if DEBUG
            //return;
#endif
            _gdigsx.PopClipAreaRect();
        }
        public override Rectangle CurrentClipRect => _gdigsx.CurrentClipRect;

        //
        public override int Top => _top;
        public override int Left => _left;
        //
        public override int Width => _right - _left;
        public override int Height => _bottom - _top;
        public override int Bottom => _bottom;

        public override int Right => _right;
        public override Rectangle Rect => Rectangle.FromLTRB(_left, _top, _right, _bottom);
        public override Rectangle InvalidateArea => _invalidateArea;

        public override void ResetInvalidateArea()
        {
            _invalidateArea = Rectangle.Empty;
            _isEmptyInvalidateArea = true;//set
        }
        public override void Invalidate(Rectangle rect)
        {
            if (_isEmptyInvalidateArea)
            {
                _invalidateArea = rect;
                _isEmptyInvalidateArea = false;
            }
            else
            {
                _invalidateArea = Rectangle.Union(rect, _invalidateArea);
            }

            //need to draw again
            this.IsContentReady = false;
        }
        public bool IsContentReady { get; set; }
    }
}