//BSD, 2014-present, WinterDev  
using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.GLES2
{



    public partial class MyGLDrawBoard : DrawBoard, IDisposable
    {

        GLPainter _painter1;
        bool _isDisposed;
        Stack<Rectangle> _clipRectStack = new Stack<Rectangle>();
        Rectangle _currentClipRect;

        public MyGLDrawBoard(
           GLPainter painter, //*** we wrap around GLPainter *** 
           int width,
           int height)
        {
            //----------------
            //set painter first
            this._painter1 = painter;
            //----------------
            this._left = 0; //default start at 0,0
            this._top = 0;
            this._width = width;
            this._height = height;

            _currentClipRect = new Rectangle(0, 0, width, height);

            this.CurrentFont = new RequestFont("tahoma", 10);
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
        }

        public override Painter GetPainter()
        {
            //TODO: check if we must set canvas origin to painter or not
            return _painter1;
        }
        public override void Dispose()
        {

            //TODO: review here
        }
#if DEBUG
        public override string ToString()
        {
            return "visible_clip?";
        }
#endif
        public override void CloseCanvas()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            this.CloseCanvas();
        }

        void ClearPreviousStoredValues()
        {
            _painter1.SetOrigin(0, 0);

            this._canvasOriginX = 0;
            this._canvasOriginY = 0;
            this._clipRectStack.Clear();
        }

        void ReleaseUnManagedResource()
        {
            _clipRectStack.Clear();
            _currentClipRect = new Rectangle(0, 0, this.Width, this.Height);
#if DEBUG

            debug_releaseCount++;
#endif
        }

#if DEBUG
        static class dbugCounter
        {
            public static int dbugDrawStringCount;
        }
        public override void dbug_DrawRuler(int x)
        {
            int canvas_top = this._top;
            int canvas_bottom = this.Bottom;
            for (int y = canvas_top; y < canvas_bottom; y += 10)
            {
                this.DrawText(y.ToString().ToCharArray(), x, y);
            }
        }
        public override void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            var prevColor = this.StrokeColor;
            this.StrokeColor = color;
            DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom);
            DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top);
            this.StrokeColor = prevColor;
        }

#endif

    }
}