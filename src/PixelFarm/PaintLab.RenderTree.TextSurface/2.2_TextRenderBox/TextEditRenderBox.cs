//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    partial class TextEditRenderBox
    {

        public Color BackgroundColor { get; set; }
        public event EventHandler ViewportChanged;
        public event EventHandler ContentSizeChanged;

        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            RequestFont enterFont = canvas.CurrentFont;

            canvas.CurrentFont = this.CurrentTextSpanStyle.ReqFont;

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


            //draw text layer  
            this._textLayer.DrawChildContent(canvas, updateArea);
            if (this.HasDefaultLayer)
            {
                this.DrawDefaultLayer(canvas, ref updateArea);
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

        internal void OnTextContentSizeChanged()
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
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
                if (y > _textLayer.Bottom - Height)
                {
                    y = _textLayer.Bottom - Height;
                    if (y < 0)
                    {
                        y = 0;
                    }
                }
            }


            this.InvalidateGraphics();
            this.SetViewport(x, y);
            this.InvalidateGraphics();

        }
        void MyScrollByNotRaiseEvent(int dx, int dy, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;

            var contentSize = this.InnerContentSize;

            Size innerContentSize = new Size(this.Width, this._textLayer.Bottom);

            if (dy < 0)
            {
                int old_y = this.ViewportY;
                if (ViewportY + dy < 0)
                {
                    //? limit                     
                    this.SetViewport(this.ViewportX, 0);
                }
                else
                {
                    this.SetViewport(this.ViewportX, this.ViewportY + dy);
                }
            }
            else if (dy > 0)
            {
                int old_y = ViewportY;
                int viewportButtom = ViewportY + Height;
                if (viewportButtom + dy > innerContentSize.Height)
                {
                    int vwY = innerContentSize.Height - Height;
                    //limit                     
                    this.SetViewport(this.ViewportX, vwY > 0 ? vwY : 0);
                }
                else
                {
                    this.SetViewport(this.ViewportX, old_y + dy);
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
            ViewportChanged?.Invoke(this, EventArgs.Empty);
            this.InvalidateGraphics();
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
            ViewportChanged?.Invoke(this, EventArgs.Empty);
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
    }
}