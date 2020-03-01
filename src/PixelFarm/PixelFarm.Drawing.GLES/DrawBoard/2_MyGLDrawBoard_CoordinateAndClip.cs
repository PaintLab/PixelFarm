//BSD, 2014-present, WinterDev

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
            //current clip rect is relative to current canvas origin
            //so if move canvas origin we must move current clip rect
            //in opposite direction 
            //clip rect move to another direction***
            _currentClipRect.Offset(-(x - _canvasOriginX), -(y - _canvasOriginY));
            _canvasOriginX = x;
            _canvasOriginY = y;
#if DEBUG
            if (x == 8 || y == 8)
            {

            }
            if (_clipRectStack.Count > 0)
            {

            }
#endif
        }

        public override int OriginX => _canvasOriginX;

        public override int OriginY => _canvasOriginY;

        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            //TODO: reivew clip combine mode

            _gpuPainter.SetClipBox(rect.Left, rect.Top, rect.Right, rect.Bottom);
            _currentClipRect = rect;
        }

        public override bool PushClipAreaRect(int width, int height, UpdateArea updateArea)
        {
            //TODO: review here  

            Rectangle intersectRect = Rectangle.Intersect(updateArea.CurrentRect, new Rectangle(0, 0, width, height));

            if (intersectRect.Width <= 0 || intersectRect.Height <= 0)
            {
                return false;
            }
            else
            {
                _clipRectStack.Push(_currentClipRect);
                _currentClipRect = intersectRect;
                                 
                updateArea.MakeBackup();
                updateArea.CurrentRect = intersectRect; //*** update area is changed
                _gpuPainter.SetClipBox(intersectRect.Left, intersectRect.Top, intersectRect.Right, intersectRect.Bottom); 

                return true;
            }
        }
        public override bool PushClipAreaRect(int left, int top, int width, int height, UpdateArea updateArea)
        {
            //TODO: review here
            Rectangle intersectRect = Rectangle.Intersect(updateArea.CurrentRect, new Rectangle(left, top, width, height));

            if (intersectRect.Width <= 0 || intersectRect.Height <= 0)
            {

                return false;
            }
            else
            {
                //true
                _clipRectStack.Push(_currentClipRect);
                _currentClipRect = intersectRect;

                updateArea.MakeBackup();
                updateArea.CurrentRect = intersectRect; //*** update area is changed

                _gpuPainter.SetClipBox(intersectRect.Left, intersectRect.Top, intersectRect.Right, intersectRect.Bottom);
                return true;
            }
        }
        public override void PopClipAreaRect()
        {
            if (_clipRectStack.Count > 0)
            {
                //move to prev clip area
                _currentClipRect = _clipRectStack.Pop();
                _gpuPainter.SetClipBox(_currentClipRect.Left, _currentClipRect.Top, _currentClipRect.Right, _currentClipRect.Bottom);
            }
        }
        /// <summary>
        /// currrent clip rect relative to current canvas origin
        /// </summary>
        public override Rectangle CurrentClipRect => _currentClipRect;

        public override int Top => _top;

        public override int Left => _left;

        public override int Width => _width;

        public override int Height => _height;

        public override int Bottom => _top + _height;

        public override int Right => _left + _width;

        public override Rectangle Rect => new Rectangle(_left, _top, _width, _height);

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