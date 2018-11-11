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


namespace PixelFarm.Drawing.WinGdi
{

    public partial class GdiPlusDrawBoard : DrawBoard, IDisposable
    {
        bool isDisposed;
        GdiPlusRenderSurface _gdigsx;
        static GdiPlusDrawBoard()
        {
            DrawBoardCreator.RegisterCreator(1, (w, h) => new GdiPlusDrawBoard(0, 0, w, h));
        }
        public GdiPlusDrawBoard(int left, int top, int width, int height)
        {
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;

            _gdigsx = new GdiPlusRenderSurface(left, top, width, height);
        }

#if DEBUG
        public override string ToString()
        {
            return _gdigsx.ToString();
        }
#endif

        public override Painter GetPainter()
        {
            //create agg painter
            return _gdigsx.GetAggPainter();

        }
        public override void RenderTo(Image destImg, int srcX, int srcYy, int srcW, int srcH)
        {

            //render back buffer to target image

            unsafe
            {
                CpuBlit.ActualBitmap img = destImg as CpuBlit.ActualBitmap;
                if (img != null)
                {
                    CpuBlit.Imaging.TempMemPtr tmpPtr = CpuBlit.ActualBitmap.GetBufferPtr(img);
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
            if (isDisposed)
            {
                return;
            }

            _gdigsx.CloseCanvas();

            isDisposed = true;
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (isDisposed)
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