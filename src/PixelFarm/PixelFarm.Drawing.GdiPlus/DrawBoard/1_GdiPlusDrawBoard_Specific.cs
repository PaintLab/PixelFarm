//BSD, 2014-2018, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
        int pageNumFlags;
        int pageFlags;
        bool isDisposed;

        GdiPlusRenderSurface _gdigsx;
        public GdiPlusDrawBoard(int left, int top, int width, int height)
            : this(0, 0, left, top, width, height)
        {
        }
        internal GdiPlusDrawBoard(
            int horizontalPageNum,
            int verticalPageNum,
            int left, int top,
            int width,
            int height)

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

        public void Reuse(int hPageNum, int vPageNum)
        {
            this.pageNumFlags = (hPageNum << 8) | vPageNum;
            int w = this.Width;
            int h = this.Height;
            this.ClearPreviousStoredValues();

            //TODO: review here
            _gdigsx.Reuse(hPageNum, vPageNum);

            left = hPageNum * w;
            top = vPageNum * h;
            right = left + w;
            bottom = top + h;
        }
        public void Reset(int hPageNum, int vPageNum, int newWidth, int newHeight)
        {
            this.pageNumFlags = (hPageNum << 8) | vPageNum;
            this.ReleaseUnManagedResource();
            this.ClearPreviousStoredValues();

            _gdigsx.Reset(hPageNum, vPageNum, newWidth, newHeight);


            left = hPageNum * newWidth;
            top = vPageNum * newHeight;
            right = left + newWidth;
            bottom = top + newHeight;
#if DEBUG
            debug_resetCount++;
#endif
        }
        public bool IsPageNumber(int hPageNum, int vPageNum)
        {
            return pageNumFlags == ((hPageNum << 8) | vPageNum);
        }
        public bool IsUnused
        {
            get
            {
                return (pageFlags & CANVAS_UNUSED) != 0;
            }
            set
            {
                if (value)
                {
                    pageFlags |= CANVAS_UNUSED;
                }
                else
                {
                    pageFlags &= ~CANVAS_UNUSED;
                }
            }
        }

        public bool DimensionInvalid
        {
            get
            {
                return (pageFlags & CANVAS_DIMEN_CHANGED) != 0;
            }
            set
            {
                if (value)
                {
                    pageFlags |= CANVAS_DIMEN_CHANGED;
                }
                else
                {
                    pageFlags &= ~CANVAS_DIMEN_CHANGED;
                }
            }
        }

        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);


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