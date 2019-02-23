//BSD, 2014-present, WinterDev  
using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.GLES2
{

    class MyGLBackbuffer : Backbuffer
    {
        GLRenderSurface _glRenderSurface;

        public MyGLBackbuffer(int w, int h)
        {
            Width = w;
            Height = h;
            //
            _glRenderSurface = new GLRenderSurface(w, h);
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public GLRenderSurface RenderSurface => _glRenderSurface;
        public override Image GetImage() => _glRenderSurface.GetGLBitmap();
    }


    public partial class MyGLDrawBoard : DrawBoard, IDisposable
    {
        public delegate DrawBoard GetCpuBlitDrawBoardDelegate();

        GLBitmap _tmpGLBmp;
        GLPainter _gpuPainter;
        GLPainterContext _pcx;
        bool _isDisposed;
        Stack<Rectangle> _clipRectStack = new Stack<Rectangle>();
        Rectangle _currentClipRect;

        GetCpuBlitDrawBoardDelegate _getCpuBlitDrawBoardDel;
        DrawBoard _cpuBlitDrawBoard;
        bool _evalCpuBlitCreator;

        GLRenderSurface _backupRenderSurface;

        public MyGLDrawBoard(GLPainter painter)
        {

            //----------------
            //set painter first
            _gpuPainter = painter;
            _pcx = painter.PainterContext;
            //----------------
            _left = 0; //default start at 0,0
            _top = 0;
            _width = _pcx.CanvasWidth;
            _height = _pcx.CanvasHeight;

            _currentClipRect = new Rectangle(0, 0, _width, _height);

            this.CurrentFont = new RequestFont("tahoma", 10);
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
        }

        public override void SwitchBackToDefaultBuffer(Backbuffer backbuffer)
        {
            _gpuPainter.PainterContext.AttachToRenderSurface(null);
            _gpuPainter.PainterContext.OriginKind = RenderSurfaceOrientation.LeftTop;
        }
        public override Backbuffer CreateBackbuffer(int w, int h)
        {
            return new MyGLBackbuffer(w, h);
        }
        public override void AttachToBackBuffer(Backbuffer backbuffer)
        {

            _backupRenderSurface = _gpuPainter.PainterContext.CurrentRenderSurface;//***

            MyGLBackbuffer glBackBuffer = (MyGLBackbuffer)backbuffer;
            _gpuPainter.PainterContext.AttachToRenderSurface(glBackBuffer.RenderSurface);
            _gpuPainter.PainterContext.OriginKind = RenderSurfaceOrientation.LeftTop;
        }
        public override void Dispose()
        {
            //TODO: review here
            if (_tmpGLBmp != null)
            {
                _tmpGLBmp.Dispose();
                _tmpGLBmp = null;
            }
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
        public override BitmapBufferProvider GetInternalBitmapProvider()
        {
            //TODO: implement this
            //copy bitmap data to target 
            //(server-to-server)
            //(server-to-client)

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
        public override void BlitFrom(DrawBoard src, float srcX, float srcY, float srcW, float srcH, float dstX, float dstY)
        {
            if (!src.IsGpuDrawBoard)
            {
                //cpu draw board
                BitmapBufferProvider bmpProvider = src.GetInternalBitmapProvider();

                if (_tmpGLBmp == null)
                {
                    _tmpGLBmp = new GLBitmap(bmpProvider);
                }
                else
                {
                    _tmpGLBmp.UpdateTexture(new Rectangle((int)srcX, (int)srcY, (int)srcW, (int)srcH));
                }

                //---------
                this.DrawImage(_tmpGLBmp,
                    new RectangleF((int)dstX, (int)dstY, (int)srcW, (int)srcH), //dst
                    new RectangleF((int)srcX, (int)srcY, (int)srcW, (int)srcH)); //src
            }
            else
            {
                //TODO: implement this....
            }
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

            _canvasOriginX = 0;
            _canvasOriginY = 0;
            _clipRectStack.Clear();
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
            int canvas_top = _top;
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