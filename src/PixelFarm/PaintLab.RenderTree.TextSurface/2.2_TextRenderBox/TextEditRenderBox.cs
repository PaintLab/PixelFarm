//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.TextEditing
{
    partial class TextEditRenderBox
    {
        SolidTextRun _latestHitSolidTextRun;
        public Color BackgroundColor { get; set; }
        public event EventHandler ViewportChanged;
        public event EventHandler ContentSizeChanged;

        public bool RenderBackground { get; set; }
        public bool RenderCaret { get; set; }
        public bool RenderMarkers { get; set; }
        public bool RenderSelectionRange { get; set; }

        public Size InnerBackgroundSize
        {
            get
            {
                Size innerSize = this.InnerContentSize;
                return new Size(
                    (innerSize.Width < this.Width) ? this.Width : innerSize.Width,
                    (innerSize.Height < this.Height) ? this.Height : innerSize.Height);
            }
        }
        public void RunVisitor(EditableRunVisitor visitor)
        {
            //1. bg, no nothing
            visitor.CurrentCaretPos = _internalTextLayerController.CaretPos;
            //2. markers 
            if (!visitor.SkipMarkerLayer && _internalTextLayerController.VisualMarkerCount > 0)
            {
                visitor.OnBeginMarkerLayer();
                foreach (VisualMarkerSelectionRange marker in _internalTextLayerController.VisualMarkers)
                {
                    visitor.VisitMarker(marker);
                }
                visitor.OnEndMarkerLayer();
            }

            //3.
            if (!visitor.SkipSelectionLayer && _internalTextLayerController.SelectionRange != null)
            {
                visitor.VisitSelectionRange(_internalTextLayerController.SelectionRange);
            }

            //4. text layer
            visitor.OnBeginTextLayer();
            _textLayer.RunVisitor(visitor);
            visitor.OnEndTextLayer(); 
            //5. others? 
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            RequestFont enterFont = canvas.CurrentFont;

            canvas.CurrentFont = this.CurrentTextSpanStyle.ReqFont;

            //1. bg 
            if (RenderBackground && BackgroundColor.A > 0)
            {
                Size innerBgSize = InnerBackgroundSize;
                canvas.FillRectangle(BackgroundColor, 0, 0, innerBgSize.Width, innerBgSize.Height);
            }


            //2.1 markers 
            if (RenderMarkers && _internalTextLayerController.VisualMarkerCount > 0)
            {
                foreach (VisualMarkerSelectionRange marker in _internalTextLayerController.VisualMarkers)
                {
                    marker.Draw(canvas, updateArea);
                }
            }


            //2.2 selection
            if (RenderSelectionRange && _internalTextLayerController.SelectionRange != null)
            {
                _internalTextLayerController.SelectionRange.Draw(canvas, updateArea);
            }


            //draw text layer  
            _textLayer.DrawChildContent(canvas, updateArea);
            if (this.HasDefaultLayer)
            {
                this.DrawDefaultLayer(canvas, ref updateArea);
            }

#if DEBUG
            //for debug
            //canvas.FillRectangle(Color.Red, 0, 0, 5, 5);

#endif
            //4. caret 
            if (RenderCaret && _stateShowCaret && _isEditable)
            {
                Point textManCaretPos = _internalTextLayerController.CaretPos;
                _myCaret.DrawCaret(canvas, textManCaretPos.X, textManCaretPos.Y);
            }

            canvas.CurrentFont = enterFont;
        }

        internal void OnTextContentSizeChanged()
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public SolidTextRun LastestHitSolidTextRun => _latestHitSolidTextRun;
        internal void NotifyHitOnSolidTextRun(SolidTextRun solidTextRun)
        {
            _latestHitSolidTextRun = solidTextRun;
        }


        //-----------------------------------------------
        //Scrolling...
        //-----------------------------------------------

        public void ScrollToLocation(int x, int y)
        {
            if (!this.MayHasViewport ||
                y == this.ViewportTop && x == this.ViewportLeft)
            {
                //no change!
                return;
            }

            ScrollToLocation_NotRaiseEvent(x, y, out var hScrollEventArgs, out var vScrollEventArgs);

            ViewportChanged?.Invoke(this, EventArgs.Empty);
        }

        void ScrollToLocation_NotRaiseEvent(int x, int y,
            out UIScrollEventArgs hScrollEventArgs,
            out UIScrollEventArgs vScrollEventArgs)
        {

            hScrollEventArgs = null;
            vScrollEventArgs = null;
            Size innerContentSize = this.InnerContentSize;
            if (x > innerContentSize.Width - Width)
            {
                x = innerContentSize.Width - Width;
                //inner content_size.Width may shorter than this.Width
                //so we check if (x<0) later
            }
            if (x < 0)
            {
                x = 0;
            }
            //
            int textLayoutBottom = _textLayer.Bottom;
            if (y > textLayoutBottom - Height)
            {
                y = textLayoutBottom - Height;
                //inner content_size.Height may shorter than this.Height
                //so we check if (y<0) later
            }
            if (y < 0)
            {
                y = 0;
            }
            this.InvalidateGraphics();
            this.SetViewport(x, y);
            this.InvalidateGraphics();
        }
        public void ScrollOffset(int dx, int dy)
        {
            if (!this.MayHasViewport ||
                (dy == 0 && dx == 0))
            {  //no change!
                return;
            }

            this.InvalidateGraphics();

            ScrollOffset_NotRaiseEvent(dx, dy, out var hScrollEventArgs, out var vScrollEventArgs);
            ViewportChanged?.Invoke(this, EventArgs.Empty);

            this.InvalidateGraphics();
        }

        void ScrollOffset_NotRaiseEvent(int dx, int dy,
            out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;

            Size contentSize = this.InnerContentSize;
            Size innerContentSize = new Size(this.Width, _textLayer.Bottom);

            if (dy < 0)
            {
                int old_y = this.ViewportTop;
                if (ViewportTop + dy < 0)
                {
                    //? limit                     
                    this.SetViewport(this.ViewportLeft, 0);
                }
                else
                {
                    this.SetViewport(this.ViewportLeft, this.ViewportTop + dy);
                }
            }
            else if (dy > 0)
            {
                int old_y = ViewportTop;
                int viewportButtom = ViewportTop + Height;
                if (viewportButtom + dy > innerContentSize.Height)
                {
                    int vwY = innerContentSize.Height - Height;
                    //limit                     
                    this.SetViewport(this.ViewportLeft, vwY > 0 ? vwY : 0);
                }
                else
                {
                    this.SetViewport(this.ViewportLeft, old_y + dy);
                }
            }
            //

            hScrollEventArgs = null;
            if (dx < 0)
            {
                int old_x = this.ViewportLeft;
                if (old_x + dx < 0)
                {
                    dx = -ViewportLeft;
                    SetViewport(0, this.ViewportTop);
                }
                else
                {
                    SetViewport(this.ViewportLeft + dx, this.ViewportTop);
                }
            }
            else if (dx > 0)
            {
                int old_x = this.ViewportLeft;
                int viewportRight = ViewportLeft + Width;
                if (viewportRight + dx > innerContentSize.Width)
                {
                    this.SetViewport(this.ViewportLeft + dx, this.ViewportTop);
                    //if (viewportRight < innerContentSize.Width)
                    //{
                    //    this.SetViewport(innerContentSize.Width - Width, this.ViewportTop);
                    //}
                }
                else
                {
                    //this.SetViewport(this.ViewportLeft + dx, this.ViewportTop);
                }
            }
        }
    }
}