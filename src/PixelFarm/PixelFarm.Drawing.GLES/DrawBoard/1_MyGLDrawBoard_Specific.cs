//BSD, 2014-present, WinterDev  
using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;


namespace PixelFarm.Drawing.GLES2
{


    public sealed class MyGLBackbuffer : DrawboardBuffer
    {
        GLRenderSurface _glRenderSurface;
        readonly int _w;
        readonly int _h;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public MyGLBackbuffer(int w, int h, bool useRGB = false)
        {
            _w = w;
            _h = h;
            _glRenderSurface = new GLRenderSurface(w, h, useRGB ? BitmapBufferFormat.RGBO : BitmapBufferFormat.RGBA);
        }

        public override int Width => _w;
        public override int Height => _h;
        public GLRenderSurface RenderSurface => _glRenderSurface;
        public override Image GetImage() => _glRenderSurface.GetGLBitmap();
        public override void Dispose()
        {
            if (_glRenderSurface != null)
            {
                _glRenderSurface.Dispose();
                _glRenderSurface = null;
            }
        }
        public override Image CopyToNewMemBitmap()
        {
            unsafe
            {
                //test only!
                //copy from gl to MemBitmap
                var outputBuffer = new PixelFarm.CpuBlit.MemBitmap(_w, _h);
                _glRenderSurface.CopySurface(0, 0, _w, _h, outputBuffer);
                return outputBuffer;
            }
        }
        public Image CopyToNewMemBitmap(int left, int top, int width, int height)
        {
            unsafe
            {
                //test only!
                //copy from gl to MemBitmap
                var outputBuffer = new PixelFarm.CpuBlit.MemBitmap(width, height);
                _glRenderSurface.CopySurface(left, top, width, height, outputBuffer);
                return outputBuffer;
            }
        }


    }



    public partial class MyGLDrawBoard : DrawBoard, IDisposable
    {
        public delegate DrawBoard GetCpuBlitDrawBoardDelegate();

        GLBitmap _tmpGLBmp;
        GLPainter _gpuPainter;

        bool _isDisposed;
        Stack<Rectangle> _clipRectStack = new Stack<Rectangle>();
        /// <summary>
        /// current clip rect, relative to canvas'origin
        /// </summaryor
        Rectangle _currentClipRect;

        GetCpuBlitDrawBoardDelegate _getCpuBlitDrawBoardDel;
        DrawBoard _cpuBlitDrawBoard;
        bool _evalCpuBlitCreator;
        Stack<SaveContext> _saveContexts = new Stack<SaveContext>();
        TextDrawingTech _textDrawingTechnique;

        public MyGLDrawBoard(GLPainter painter)
        {
            //----------------
            //set painter first
            _gpuPainter = painter;
            //----------------
            _left = 0; //default start at 0,0
            _top = 0;
            _width = painter.Width;
            _height = painter.Height;

            _currentClipRect = new Rectangle(0, 0, _width, _height);
            this.CurrentFont = painter.CurrentFont;
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
        }


        public override Color TextBackgroundColorHint
        {
            //temp fix
            get => _gpuPainter.TextBgColorHint;
            set
            {
#if DEBUG
                if (value.A < 255)
                {

                }
#endif
                _gpuPainter.TextBgColorHint = value;
            }
        }

        public override TextDrawingTech TextDrawingTech
        {
            get => _textDrawingTechnique;
            set
            {
                //temp fix
                _textDrawingTechnique = value;
                GlyphTexturePrinterDrawingTechnique tech = GlyphTexturePrinterDrawingTechnique.Copy;
                switch (value)
                {
                    case TextDrawingTech.LcdSubPix:
                        tech = GlyphTexturePrinterDrawingTechnique.LcdSubPixelRendering;
                        break;
                    case TextDrawingTech.Stencil:
                        tech = GlyphTexturePrinterDrawingTechnique.Stencil;
                        break;
                    case TextDrawingTech.Copy:
                        tech = GlyphTexturePrinterDrawingTechnique.Copy;
                        break;
                }
                _gpuPainter.TextPrinter.TextDrawingTechnique = tech;
            }
        }


        public override DrawboardBuffer CreateBackbuffer(int w, int h)
        {
            return new MyGLBackbuffer(w, h, true);//temp
        }

#if DEBUG
        public int dbugSwitchCount;
#endif
        class SaveContext
        {
            public int prevCanvasOrgX;
            public int prevCanvasOrgY;
            public Rectangle prevClipRect;
            public GLRenderSurface prevGLRenderSurface;
#if DEBUG
            public SaveContext() { }
            public bool dbugIsInPool;
#endif
        }

        Stack<SaveContext> _saveContextPool = new Stack<SaveContext>();
        SaveContext GetFreeSaveContext()
        {
            if (_saveContextPool.Count == 0) return new SaveContext();

#if DEBUG
            SaveContext s = _saveContextPool.Pop();
            if (!s.dbugIsInPool)
            {
                throw new NotSupportedException();
            }

            s.dbugIsInPool = false;
            return s;
#else
            
return _saveContextPool.Pop();
#endif


        }
        void ReleaseSaveContext(SaveContext saveContext)
        {
#if DEBUG
            saveContext.dbugIsInPool = true;
#endif
            _saveContextPool.Push(saveContext);
        }
        public override void EnterNewDrawboardBuffer(DrawboardBuffer backbuffer)
        {
#if DEBUG
            if (dbugSwitchCount > 0)
            {

            }
            dbugSwitchCount++;
#endif

            //save prev context
            SaveContext prevContext = GetFreeSaveContext();
            prevContext.prevClipRect = _currentClipRect;
            prevContext.prevCanvasOrgX = _canvasOriginX;
            prevContext.prevCanvasOrgY = _canvasOriginY;
            prevContext.prevGLRenderSurface = _gpuPainter.Core.CurrentRenderSurface;
            _saveContexts.Push(prevContext);

            _currentClipRect = new Rectangle(0, 0, backbuffer.Width, backbuffer.Height);
            MyGLBackbuffer glBackBuffer = (MyGLBackbuffer)backbuffer;

            _gpuPainter.Core.AttachToRenderSurface(glBackBuffer.RenderSurface);
            _gpuPainter.Core.OriginKind = RenderSurfaceOriginKind.LeftTop;
            _gpuPainter.UpdateCore();

            _left = 0;
            _top = 0;
            _width = _gpuPainter.Width;
            _height = _gpuPainter.Height;

            _canvasOriginX = 0;
            _canvasOriginY = 0;
            _gpuPainter.SetOrigin(0, 0);

            SetClipRect(_currentClipRect);
        }
        public override void ExitCurrentDrawboardBuffer()
        {
#if DEBUG
            if (dbugSwitchCount == 0)
            {

            }
            dbugSwitchCount--;
#endif

            SaveContext saveContext = _saveContexts.Pop();
            _canvasOriginX = saveContext.prevCanvasOrgX;
            _canvasOriginY = saveContext.prevCanvasOrgY;
            _currentClipRect = saveContext.prevClipRect;

            _gpuPainter.Core.AttachToRenderSurface(saveContext.prevGLRenderSurface);

            _gpuPainter.Core.OriginKind = RenderSurfaceOriginKind.LeftTop;
            _gpuPainter.UpdateCore();

            _left = 0;
            _top = 0;
            _width = _gpuPainter.Width;
            _height = _gpuPainter.Height;

            _gpuPainter.SetOrigin(_canvasOriginX, _canvasOriginY);
            SetClipRect(_currentClipRect);
            ReleaseSaveContext(saveContext);
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
        public GLPainter GetGLPainter() => _gpuPainter;

        public override ImageBinder GetInternalBitmapProvider()
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
                ImageBinder bmpProvider = src.GetInternalBitmapProvider();

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