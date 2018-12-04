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

        public override int OriginX
        {
            get { return _canvasOriginX; }
        }
        public override int OriginY
        {
            get { return _canvasOriginY; }
        }


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

        public override bool PushClipAreaRect(int width, int height, ref Rectangle updateArea)
        {
            return _gdigsx.PushClipAreaRect(width, height, ref updateArea);
        }
        public override void PopClipAreaRect()
        {
            _gdigsx.PopClipAreaRect();
        }
        public override Rectangle CurrentClipRect
        {
            get
            {
                return _gdigsx.CurrentClipRect;
            }
        }



        public override int Top
        {
            get
            {
                return _top;
            }
        }
        public override int Left
        {
            get
            {
                return _left;
            }
        }

        public override int Width
        {
            get
            {

                return _right - _left;
            }
        }
        public override int Height
        {
            get
            {
                return _bottom - _top;
            }
        }
        public override int Bottom
        {
            get
            {
                return _bottom;
            }
        }
        public override int Right
        {
            get
            {
                return _right;
            }
        }
        public override Rectangle Rect
        {
            get
            {
                return Rectangle.FromLTRB(_left, _top, _right, _bottom);
            }
        }
        public override Rectangle InvalidateArea
        {
            get
            {
                return _invalidateArea;
            }
        }

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