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
using PixelFarm.CpuBlit;

namespace PixelFarm.Drawing.WinGdi
{

    class MyGdiBackbuffer : DrawboardBuffer
    {
        readonly int _w;
        readonly int _h;
        PixelFarm.CpuBlit.MemBitmap _memBitmap;
        public MyGdiBackbuffer(int w, int h)
        {
            _w = w;
            _h = h;

            _memBitmap = new CpuBlit.MemBitmap(w, h);
        }

        public override void Dispose()
        {
            if (_memBitmap != null)
            {
                _memBitmap.Dispose();
                _memBitmap = null;
            }
        }
        public override int Width => _w;
        public override int Height => _h;

        public override Image GetImage() => _memBitmap;
        public override Image CopyToNewMemBitmap() => MemBitmap.CreateFromCopy(_memBitmap);
    }


    public partial class GdiPlusDrawBoard : DrawBoard, IDisposable
    {

        bool _disposed;
        GdiPlusRenderSurface _gdigsx;
        Painter _painter;
        MemBitmapBinder _memBmpBinder;
        TextDrawingTech _textDrawingTechnique;
        Color _textBackgroundColorHint;

        public GdiPlusDrawBoard(GdiPlusRenderSurface renderSurface)
        {
            _left = 0;
            _top = 0;
            _right = renderSurface.Width;
            _bottom = renderSurface.Height;

            _gdigsx = renderSurface;
            _painter = _gdigsx.GetAggPainter();

            _memBmpBinder = new MemBitmapBinder(renderSurface.GetMemBitmap(), false);
            _memBmpBinder.BitmapFormat = BitmapBufferFormat.BGR;

            this.CurrentFont = renderSurface.CurrentFont;
        }

        
        public override TextDrawingTech TextDrawingTech
        {
            get => _textDrawingTechnique;
            set => _textDrawingTechnique = value;
        }
        public override Color TextBackgroundColorHint
        {
            get => _textBackgroundColorHint;
            set => _textBackgroundColorHint = value;//not used in this mode
        }

        public override void EnterNewDrawboardBuffer(DrawboardBuffer backbuffer)
        {

        }
        public override void ExitCurrentDrawboardBuffer()
        {

        }
        public override DrawboardBuffer CreateBackbuffer(int w, int h)
        {
            return new MyGdiBackbuffer(w, h);
        }
        public GdiPlusRenderSurface RenderSurface => _gdigsx;
        public override bool IsGpuDrawBoard => false;
        public override DrawBoard GetCpuBlitDrawBoard() => this;
        //
        public override ImageBinder GetInternalBitmapProvider() => _memBmpBinder;

#if DEBUG
        public override string ToString()
        {
            return _gdigsx.ToString();
        }
#endif

        public override Painter GetPainter()
        {
            //since painter origin and canvas origin is separated 
            //so must check here
            //TODO: revisit the painter and the surface => shared resource **

            _painter.SetOrigin(_canvasOriginX, _canvasOriginY);
            return _painter;
        }
        public override void RenderTo(Image destImg, int srcX, int srcYy, int srcW, int srcH)
        {

            //render back buffer to target image

            unsafe
            {
                CpuBlit.MemBitmap memBmp = destImg as CpuBlit.MemBitmap;
                if (memBmp != null)
                {
                    PixelFarm.CpuBlit.TempMemPtr tmpPtr = CpuBlit.MemBitmap.GetBufferPtr(memBmp);
                    byte* head = (byte*)tmpPtr.Ptr;
                    _gdigsx.RenderTo(head);
                    tmpPtr.Dispose();
                }
            }
        }
        public override void Dispose()
        {
            if (_gdigsx != null)
            {
                _gdigsx.CloseCanvas();
                _gdigsx = null;
            }
        }
        public override void CloseCanvas()
        {
            if (_disposed)
            {
                return;
            }

            _gdigsx.CloseCanvas();

            _disposed = true;
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }
            this.CloseCanvas();
        }
        void ClearPreviousStoredValues()
        {
            _gdigsx.ClearPreviousStoredValues();
        }

        void ReleaseUnManagedResource()
        {
            _gdigsx.ReleaseUnManagedResource();
#if DEBUG

            debug_releaseCount++;
#endif
        }

#if DEBUG
        public override void dbug_DrawRuler(int x)
        {
            _gdigsx.dbug_DrawRuler(x);
        }
        public override void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            _gdigsx.dbug_DrawCrossRect(color, rect);
        }
#endif
    }
}