//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    abstract class CanvasViewport
    {
        int _viewportX;
        int _viewportY;
        int _viewportWidth;
        int _viewportHeight;
        protected IRenderElement _topWindowBox;
        protected RootGraphic _rootGraphics;
        int _h_smallChange = 0;
        int _h_largeChange = 0;
        int _v_smallChange = 0;
        int _v_largeChange = 0;
        EventHandler<EventArgs> _canvasSizeChangedHandler;

        bool _isClosed;//is this viewport closed
        public CanvasViewport(RootGraphic rootgfx, Size viewportSize)
        {
            _rootGraphics = rootgfx;
            _topWindowBox = rootgfx.TopWindowRenderBox;
            _viewportWidth = viewportSize.Width;
            _viewportHeight = viewportSize.Height;
            _canvasSizeChangedHandler = Canvas_SizeChanged;
            _viewportX = 0;
            _viewportY = 0;

            FullMode = true;
        }

        public bool IsClosed => _isClosed;
        public int ViewportX => _viewportX;
        public int ViewportY => _viewportY;
        public int ViewportWidth => _viewportWidth;
        public int ViewportHeight => _viewportHeight;
        //
#if DEBUG
        public IdbugOutputWindow dbugOutputWindow
        {
            get;
            set;
        }
#endif
        public void UpdateCanvasViewportSize(int viewportWidth, int viewportHeight)
        {
            if (this._viewportWidth != viewportWidth || this._viewportHeight != viewportHeight)
            {
                this._viewportWidth = viewportWidth;
                this._viewportHeight = viewportHeight;
                ResetViewSize(viewportWidth, viewportHeight);
                CalculateCanvasPages();
            }
        }
        protected virtual void ResetViewSize(int viewportWidth, int viewportHeight)
        {
        }

        protected virtual void Canvas_SizeChanged(object sender, EventArgs e)
        {
            //EvaluateScrollBar();
        }
        public abstract void CanvasInvalidateArea(Rectangle r);

#if DEBUG
        internal int debug_render_to_output_count = -1;
#endif

        //
        internal bool FullMode { get; set; }
        //
        public Point LogicalViewportLocation => new Point(_viewportX, _viewportY);
        //
        protected virtual void CalculateCanvasPages()
        {
        }

        public void ScrollByNotRaiseEvent(int dx, int dy, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;
            if (dy < 0)
            {
                int old_y = _viewportY;
                if (_viewportY + dy < 0)
                {
                    dy = -_viewportY;
                    _viewportY = 0;
                }
                else
                {
                    _viewportY += dy;
                }
                vScrollEventArgs = new UIScrollEventArgs(
                    UIScrollEventType.ThumbPosition,
                    old_y,
                    _viewportY, UIScrollOrientation.VerticalScroll);
            }
            else if (dy > 0)
            {
                int old_y = _viewportY;
                int viewportButtom = _viewportY + _viewportHeight;
                //
                if (viewportButtom + dy > _rootGraphics.Height)
                {
                    if (viewportButtom < _rootGraphics.Height)
                    {
                        _viewportY = _rootGraphics.Height - _viewportHeight;
                    }
                }
                else
                {
                    _viewportY += dy;
                }
                vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, _viewportY, UIScrollOrientation.VerticalScroll);
            }
            hScrollEventArgs = null;
            if (dx == 0)
            {
            }
            else if (dx > 0)
            {
                int old_x = _viewportX;
                int viewportRight = _viewportX + _viewportWidth;
                if (viewportRight + dx > _rootGraphics.Width)
                {
                    if (viewportRight < _rootGraphics.Width)
                    {
                        _viewportX = _rootGraphics.Width - _viewportWidth;
                    }
                }
                else
                {
                    _viewportX += dx;
                }
                hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, _viewportX, UIScrollOrientation.HorizontalScroll);
            }
            else
            {
                int old_x = _viewportX;
                if (old_x + dx < 0)
                {
                    dx = -_viewportX;
                    _viewportX = 0;
                }
                else
                {
                    _viewportX += dx;
                }
                hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, _viewportX, UIScrollOrientation.HorizontalScroll);
            }
            CalculateCanvasPages();
        }

        public void ScrollToNotRaiseScrollChangedEvent(int x, int y, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            hScrollEventArgs = null;
            vScrollEventArgs = null;
            if (x > _rootGraphics.Width - _viewportWidth)
            {
                x = _rootGraphics.Width - _viewportWidth;
            }
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y > 0)
            {
                if (y > _rootGraphics.Height - _viewportHeight)
                {
                    y = _rootGraphics.Height - _viewportHeight;
                    if (y < 0)
                    {
                        y = 0;
                    }
                }
            }
            int old_y = _viewportY; _viewportX = x;
            _viewportY = y;
            vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, _viewportY, UIScrollOrientation.VerticalScroll);
            CalculateCanvasPages();
        }

        public void EvaluateScrollBar(out ScrollSurfaceRequestEventArgs hScrollSupportEventArgs,
             out ScrollSurfaceRequestEventArgs vScrollSupportEventArgs)
        {
            hScrollSupportEventArgs = null;
            vScrollSupportEventArgs = null;
            _v_largeChange = _viewportHeight;
            _v_smallChange = _v_largeChange / 4;
            _h_largeChange = _viewportWidth;
            _h_smallChange = _h_largeChange / 4;
            if (_rootGraphics.Height <= _viewportHeight)
            {
                vScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(false);
            }
            else
            {
                vScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(true);
            }

            if (_rootGraphics.Width <= _viewportWidth)
            {
                hScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(false);
            }
            else
            {
                hScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(true);
            }
        }
        public void Close()
        {
            OnClosing();
            _isClosed = true;
            _rootGraphics.CloseWinRoot();
        }

        protected virtual void OnClosing()
        {
        }
    }
}
