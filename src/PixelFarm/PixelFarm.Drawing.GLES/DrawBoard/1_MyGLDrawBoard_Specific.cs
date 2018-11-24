//BSD, 2014-present, WinterDev  
using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.GLES2
{


    public partial class MyGLDrawBoard : DrawBoard, IDisposable
    {
        public delegate DrawBoard GetCpuBlitDrawBoardDelegate();


        GLPainter _gpuPainter;
        GLRenderSurface _glsx;
        bool _isDisposed;
        Stack<Rectangle> _clipRectStack = new Stack<Rectangle>();
        Rectangle _currentClipRect;

        GetCpuBlitDrawBoardDelegate _getCpuBlitDrawBoardDel;
        DrawBoard _cpuBlitDrawBoard;
        bool _evalCpuBlitCreator;

        public MyGLDrawBoard(
           GLPainter painter, //*** we wrap around GLPainter *** 
           GLRenderSurface glsx)
        {

            //----------------
            //set painter first
            _gpuPainter = painter;
            _glsx = glsx;
            //----------------
            this._left = 0; //default start at 0,0
            this._top = 0;
            this._width = glsx.CanvasWidth;
            this._height = glsx.CanvasHeight;

            _currentClipRect = new Rectangle(0, 0, this._width, this._height);

            this.CurrentFont = new RequestFont("tahoma", 10);
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
        }

        public void SetCpuBlitDrawBoardCreator(GetCpuBlitDrawBoardDelegate getCpuBlitDelegate)
        {
            _getCpuBlitDrawBoardDel = getCpuBlitDelegate;
        }

        public override bool IsGpuDrawBoard => true;

        public override Painter GetPainter()
        {
            //TODO: check if we must set canvas origin to painter or not
            return _gpuPainter;
        }
        public override LazyBitmapBufferProvider GetInternalLazyBitmapProvider()
        {
            throw new NotImplementedException();
        }
        public override DrawBoard GetCpuBlitDrawBoard()
        {
            if (!_evalCpuBlitCreator)
            {
                if (_getCpuBlitDrawBoardDel != null)
                {
                    _cpuBlitDrawBoard = _getCpuBlitDrawBoardDel();
                }
                _evalCpuBlitCreator = true;
            }
            return _cpuBlitDrawBoard;
        }
        public override void BlitFrom(DrawBoard src, float x, float y, float w, float h)
        {
            if (!src.IsGpuDrawBoard)
            {
                //cpu draw board
                
            }
            else
            {
                //TODO: implement this....
            }
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
            _gpuPainter.SetOrigin(0, 0);

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