//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    partial class TextEditRenderBox
    {
        RenderSurfaceScrollRelation _scrollRelation;
        CustomRenderSurface _vscrollableSurface;


        public Color BackgroundColor { get; set; }
        public CustomRenderSurface ScrollableSurface
        {
            get { return this._vscrollableSurface; }
            set { this._vscrollableSurface = value; }
        }
        public RenderSurfaceScrollRelation ScrollRelation
        {
            get { return this._scrollRelation; }
            set { this._scrollRelation = value; }
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            RequestFont enterFont = canvas.CurrentFont;

            canvas.CurrentFont = this.CurrentTextSpanStyle.ReqFont;
            if (_vscrollableSurface != null)
            {
                _vscrollableSurface.DrawToThisPage(canvas, updateArea);
            }
            //1. bg 
            if (BackgroundColor.A > 0)
            {

                canvas.FillRectangle(BackgroundColor, 0, 0, this.Width, this.Height);
            }


            //2.1 markers 
            if (_internalTextLayerController.VisualMarkerCount > 0)
            {
                foreach (VisualMarkerSelectionRange marker in _internalTextLayerController.VisualMarkers)
                {
                    marker.Draw(canvas, updateArea);
                }
            }


            //2.2 selection
            if (_internalTextLayerController.SelectionRange != null)
            {
                _internalTextLayerController.SelectionRange.Draw(canvas, updateArea);
            }

            //3. each layer
            if (_vscrollableSurface != null)
            {
                _vscrollableSurface.DrawToThisPage(canvas, updateArea);
            }
            else
            {
                //draw text layer  
                this._textLayer.DrawChildContent(canvas, updateArea);
                if (this.HasDefaultLayer)
                {
                    this.DrawDefaultLayer(canvas, ref updateArea);
                }
            }

#if DEBUG
            //for debug
            //canvas.FillRectangle(Color.Red, 0, 0, 5, 5);

#endif
            //4. caret 
            if (this._stateShowCaret)
            {
                Point textManCaretPos = _internalTextLayerController.CaretPos;
                this._myCaret.DrawCaret(canvas, textManCaretPos.X, textManCaretPos.Y);
            }
            else
            {
            }
            canvas.CurrentFont = enterFont;
        }

        internal void BoxEvaluateScrollBar()
        {
            if (_vscrollableSurface != null)
            {
                _vscrollableSurface.ConfirmSizeChanged();
            }
        }
        public void ScrollToNotRaiseEvent(int x, int y)
        {
            if (!this.MayHasViewport)
            {
                return;
            }
            MyScrollToNotRaiseEvent(x, y);
        }

        public void MyScrollToNotRaiseEvent(int x, int y)
        {
            if (y == this.ViewportY && x == this.ViewportX)
            {
                return;
            }

            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);
        }


        SolidTextRun _latestHitSolidTextRun;
        internal void NotifyHitOnSolidTextRun(SolidTextRun solidTextRun)
        {
            _latestHitSolidTextRun = solidTextRun;
        }
        public SolidTextRun LastestHitSolidTextRun
        {
            get { return _latestHitSolidTextRun; }
        }

        void MyScrollToNotRaiseEvent(int x, int y,
            out UIScrollEventArgs hScrollEventArgs,
            out UIScrollEventArgs vScrollEventArgs)
        {
            hScrollEventArgs = null;
            vScrollEventArgs = null;
            Size innerContentSize = this.InnerContentSize;
            if (x < 0)
            {
                x = 0;
            }
            else if (x > 0)
            {
                if (x > innerContentSize.Width - Width)
                {
                    x = innerContentSize.Width - Width;
                    if (x < 0)
                    {
                        x = 0;
                    }
                }
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y > 0)
            {
                if (y > innerContentSize.Height - Height)
                {
                    y = innerContentSize.Height - Height;
                    if (y < 0)
                    {
                        y = 0;
                    }
                }
            }

            if (_vscrollableSurface == null)
            {
                this.InvalidateGraphics();
                this.SetViewport(x, y);
                this.InvalidateGraphics();
            }
            else
            {
                if (ViewportX != x && _scrollRelation.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, this.ViewportX, x, UIScrollOrientation.HorizontalScroll);
                }
                if (ViewportY != y && _scrollRelation.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, this.ViewportY, y, UIScrollOrientation.VerticalScroll);
                }

                this.SetViewport(x, y);
                _vscrollableSurface.QuadPagesCalculateCanvas();
                _vscrollableSurface.FullModeUpdate = true;
                this.InvalidateGraphics();
                _vscrollableSurface.FullModeUpdate = false;
            }
        }
        void MyScrollByNotRaiseEvent(int dx, int dy, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;
            Size innerContentSize = this.InnerContentSize;
            if (dy < 0)
            {
                int old_y = this.ViewportY;
                if (ViewportY + dy < 0)
                {
                    dy = -ViewportY;
                    this.SetViewport(this.ViewportX, 0);
                }
                else
                {
                    this.SetViewport(this.ViewportX, this.ViewportY + dy);
                }

                if (this._vscrollableSurface != null && _scrollRelation.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(
                        UIScrollEventType.ThumbPosition,
                        old_y, ViewportY,
                        UIScrollOrientation.VerticalScroll);
                }
            }
            else if (dy > 0)
            {
                int old_y = ViewportY;
                int viewportButtom = ViewportY + Height;
                if (viewportButtom + dy > innerContentSize.Height)
                {
                    if (viewportButtom < innerContentSize.Height)
                    {
                        this.SetViewport(this.ViewportX, innerContentSize.Height - Height);
                    }
                }
                else
                {
                    this.SetViewport(this.ViewportX, innerContentSize.Height + dy);
                }
                if (_vscrollableSurface != null && _scrollRelation.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, this.ViewportY, UIScrollOrientation.VerticalScroll);
                }
            }
            hScrollEventArgs = null;
            if (dx == 0)
            {
            }
            else if (dx > 0)
            {
                int old_x = this.ViewportX;
                int viewportRight = ViewportX + Width;
                if (viewportRight + dx > innerContentSize.Width)
                {
                    if (viewportRight < innerContentSize.Width)
                    {
                        this.SetViewport(innerContentSize.Width - Width, this.ViewportY);
                    }
                }
                else
                {
                    this.SetViewport(this.ViewportX + dx, this.ViewportY);
                }
                if (_vscrollableSurface != null && _scrollRelation.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, ViewportX, UIScrollOrientation.HorizontalScroll);
                }
            }
            else
            {
                int old_x = this.ViewportX;
                if (old_x + dx < 0)
                {
                    dx = -ViewportX;
                    SetViewport(0, this.ViewportY);
                }
                else
                {
                    SetViewport(this.ViewportX + dx, this.ViewportY);
                }
                if (_vscrollableSurface != null && _scrollRelation.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition,
                        old_x, this.ViewportX, UIScrollOrientation.HorizontalScroll);
                }
            }


            if (_vscrollableSurface != null)
            {
                _vscrollableSurface.QuadPagesCalculateCanvas();
                _vscrollableSurface.FullModeUpdate = true;
            }
        }
        void MyScrollBy(int dx, int dy)
        {
            if (dy == 0 && dx == 0)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollByNotRaiseEvent(dx, dy, out hScrollEventArgs, out vScrollEventArgs);
            if (_vscrollableSurface != null)
            {
                _scrollRelation.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
                this.InvalidateGraphics();
                _vscrollableSurface.FullModeUpdate = false;
            }
            else
            {
                this.InvalidateGraphics();
            }
        }
        void MyScrollTo(int x, int y)
        {
            if (y == this.ViewportY && x == this.ViewportX)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);
            if (_vscrollableSurface != null)
            {
                _scrollRelation.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
            }
        }
        public int HorizontalLargeChange
        {
            get
            {
                if (_vscrollableSurface != null)
                {
                    return _scrollRelation.HorizontalLargeChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int HorizontalSmallChange
        {
            get
            {
                if (_vscrollableSurface != null)
                {
                    return _scrollRelation.HorizontalSmallChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int VerticalLargeChange
        {
            get
            {
                if (_vscrollableSurface != null)
                {
                    return _scrollRelation.VerticalLargeChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int VerticalSmallChange
        {
            get
            {
                if (_vscrollableSurface != null)
                {
                    return _scrollRelation.VerticalSmallChange;
                }
                else
                {
                    return 0;
                }
            }
        }


        public void AddVScrollHandler(EventHandler<UIScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (_vscrollableSurface != null)
            {
                _scrollRelation.VScrollChanged += vscrollChanged;
                _scrollRelation.VScrollRequest += vscrollSupport;
            }
        }
        public void RemoveVScrollHandler(EventHandler<UIScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (_vscrollableSurface != null)
            {
                _scrollRelation.VScrollChanged -= vscrollChanged;
                _scrollRelation.VScrollRequest -= vscrollSupport;
            }
        }
        public void AddHScrollHandler(EventHandler<UIScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (_vscrollableSurface != null)
            {
                _scrollRelation.HScrollChanged += hscrollChanged;
                _scrollRelation.HScrollRequest += hscrollSupport;
            }
        }
        public void RemoveHScrollHandler(EventHandler<UIScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (_vscrollableSurface != null)
            {
                _scrollRelation.HScrollChanged -= hscrollChanged;
                _scrollRelation.HScrollRequest -= hscrollSupport;
            }
        }
        public void ScrollTo(int x, int y)
        {
            if (!this.MayHasViewport)
            {
                return;
            }

            MyScrollTo(x, y);
        }
        public void ScrollBy(int dx, int dy)
        {
            if (!this.MayHasViewport)
            {
                return;
            }
            MyScrollBy(dx, dy);
        }
        public CustomRenderSurface VisualScrollableSurface
        {
            get
            {
                return _vscrollableSurface;
            }
        }
    }
}