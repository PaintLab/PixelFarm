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

using System;
namespace PixelFarm.Drawing.GLES2
{
    partial class MyGLDrawBoard
    {
        int _left;
        int _top;
        int _width;
        int _height;

        int _canvasOriginX = 0;
        int _canvasOriginY = 0;
        Rectangle _invalidateArea;

        bool _isEmptyInvalidateArea;
        //--------------------------------------------------------------------
        public override void SetCanvasOrigin(int x, int y)
        {

            _gpuPainter.SetOrigin(x, y);
            //----------- 
            int total_dx = x - _canvasOriginX;
            int total_dy = y - _canvasOriginY;
            //this.gx.TranslateTransform(total_dx, total_dy);
            //clip rect move to another direction***
            _currentClipRect.Offset(-total_dx, -total_dy);
            _canvasOriginX = x;
            _canvasOriginY = y;
        }

        public override int OriginX
        {
            get { return this._canvasOriginX; }
        }
        public override int OriginY
        {
            get { return this._canvasOriginY; }
        }
        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            //TODO: reivew clip combine mode
            //_gpuPainter.SetClipBox(rect.Left, rect.Bottom, rect.Right, rect.Top);
            _gpuPainter.SetClipBox(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
        public override bool PushClipAreaRect(int width, int height, ref Rectangle updateArea)
        {
            //TODO: review here
            //return true;
            // throw new NotSupportedException();
            _clipRectStack.Push(_currentClipRect);

            Rectangle intersectRect = Rectangle.Intersect(updateArea, new Rectangle(0, 0, width, height));
            _currentClipRect = intersectRect;

            if (intersectRect.Width <= 0 || intersectRect.Height <= 0)
            {
                //not intersec?
                return false;
            }
            else
            {
                updateArea = intersectRect;
                _gpuPainter.SetClipBox(intersectRect.Left, intersectRect.Top, intersectRect.Right, intersectRect.Bottom);
                return true;
            }
        }
        public override void PopClipAreaRect()
        {
            if (_clipRectStack.Count > 0)
            {
                _currentClipRect = _clipRectStack.Pop();
                _gpuPainter.SetClipBox(_currentClipRect.Left, _currentClipRect.Top, _currentClipRect.Right, _currentClipRect.Bottom);
            }
        }
        public override Rectangle CurrentClipRect
        {
            get
            {
                return _currentClipRect;
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
                return _width;
            }
        }
        public override int Height
        {
            get
            {
                return _height;
            }
        }
        public override int Bottom
        {
            get
            {
                return _top + _height;
            }
        }
        public override int Right
        {
            get
            {
                return _left + _width;
            }
        }
        public override Rectangle Rect
        {
            get
            {
                return new Rectangle(_left, _top, _width, _height);
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
            this._invalidateArea = Rectangle.Empty;
            this._isEmptyInvalidateArea = true;//set
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