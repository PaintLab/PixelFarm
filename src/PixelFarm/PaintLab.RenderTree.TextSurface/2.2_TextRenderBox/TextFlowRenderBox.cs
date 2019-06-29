//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.RenderBoxes;
using LayoutFarm.UI;

using PixelFarm.Drawing;


namespace LayoutFarm.TextEditing
{
    public class TextFlowRenderBox : RenderBoxBase, ITextFlowLayerOwner
    {
        internal TextFlowLayer _textLayer; //this is a special layer that render text
        internal TextMarkerLayer _markerLayer;
        internal TextFlowEditSession _editSession;

        internal int _verticalExpectedCharIndex;
        internal bool _isMultiLine = false;


        internal bool _isInVerticalPhase = false;
        internal bool _isFocus = false;

        internal bool _isDragBegin;

        public TextFlowRenderBox(RootGraphic rootgfx, int width, int height, bool isMultiLine)
            : base(rootgfx, width, height)
        {
            var defaultRunStyle = new RunStyle(rootgfx.TextServices);
            defaultRunStyle.FontColor = Color.Black;//set default
            defaultRunStyle.ReqFont = rootgfx.DefaultTextEditFontInfo;

            _textLayer = new TextFlowLayer(this, this.Root.TextServices, defaultRunStyle); //presentation
            _textLayer.ContentSizeChanged += (s, e) => OnTextContentSizeChanged();

            //
            _editSession = new TextFlowEditSession(_textLayer);//controller
            _isMultiLine = isMultiLine;

            IsBlockElement = false;


            RenderBackground = RenderSelectionRange = RenderMarkers = true;
            //
            MayHasViewport = true;
            BackgroundColor = Color.White;// Color.Transparent;
        }
        void ITextFlowLayerOwner.ClientLayerBubbleUpInvalidateArea(Rectangle clientInvalidatedArea)
        {
            ////client line send up 
            clientInvalidatedArea.Offset(this.X, this.Y);
            InvalidateParentGraphics(clientInvalidatedArea);

            ////TODO: review invalidate bubble
            //_owner.InvalidateParentGraphics(bubbleUpInvArea);
        }
        //TODO: review here
        //in our editor we replace user tab with space
        //TODO: we may use a single 'solid-run' for a Tab
        public byte NumOfWhitespaceForSingleTab { get; set; }
        public static TextFlowEditSession GetCurrentEditSession(TextEditRenderBox textEditRenderBox)
        {
            return textEditRenderBox._editSession;
        }
        public TextEditing.Commands.DocumentCommandListener DocCommandListener
        {
            get => _editSession.DocCmdListener;
            set => _editSession.DocCmdListener = value;
        }
        protected override PlainLayer CreateDefaultLayer() => new PlainLayer(this);

        public TextSpanStyle CurrentTextSpanStyle
        {
            get
            {

                RunStyle defaultRunStyle = _textLayer.DefaultRunStyle;
                return new TextSpanStyle()
                {
                    FontColor = defaultRunStyle.FontColor,
                    ReqFont = defaultRunStyle.ReqFont,
                    ContentHAlign = defaultRunStyle.ContentHAlign
                };
            }
            set
            {
                _textLayer.SetDefaultRunStyle(new RunStyle(Root.TextServices)
                {
                    FontColor = value.FontColor,
                    ReqFont = value.ReqFont,
                    ContentHAlign = value.ContentHAlign
                });
            }
        }

        protected Rectangle GetSelectionUpdateArea()
        {
            VisualSelectionRange selectionRange = _editSession.SelectionRange;
            if (selectionRange != null && selectionRange.IsValid)
            {
                return Rectangle.FromLTRB(0,
                    selectionRange.TopEnd.LineTop,
                    Width,
                    selectionRange.BottomEnd.Line.LineBottom);
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        protected void InvalidateGraphicOfCurrentLineArea()
        {
#if DEBUG
            //Rectangle c_lineArea = _internalTextLayerController.CurrentParentLineArea;
#endif
            Rectangle lineArea = _editSession.CurrentLineArea;
            lineArea.Width = this.Width; //change original line area' width to this render element width

            InvalidateGraphicLocalArea(this, lineArea);
        }
        protected void InvalidateGraphicOfCurrentSelectionArea()
        {
            if (_editSession.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }
        }

        public bool HasSomeText => (_textLayer.LineCount > 0) && _textLayer.GetTextLine(0).RunCount > 0;
        public virtual void Focus() { }
        public virtual void Blur() { }
        //
        public bool IsFocused => _isFocus;
        //
        public override Size InnerContentSize
        {
            get
            {
                if (IsMultiLine)
                {
                    return new Size(
                          this.Width,//TODO: fix this
                          _textLayer.Bottom);
                }
                return _editSession.CurrentLineArea.Size;
            }
        }

        public virtual void HandleMouseDown(UIMouseEventArgs e)
        {
            if (e.Button == UIMouseButtons.Left)
            {
                InvalidateGraphicOfCurrentLineArea();

                if (!e.Shift)
                {
                    _editSession.SetCaretPos(e.X, e.Y);
                    if (_editSession.SelectionRange != null)
                    {
                        Rectangle r = GetSelectionUpdateArea();
                        _editSession.CancelSelect();
                        InvalidateGraphicLocalArea(this, r);
                    }
                    else
                    {
                        InvalidateGraphicOfCurrentLineArea();
                    }

                    SolidRun latestHitSolidTextRun = _editSession.LatestHitRun as SolidRun;
                    if (latestHitSolidTextRun != null)
                    {
                        //we mousedown on the solid text run
                        RenderElement extRenderElement = latestHitSolidTextRun.ExternalRenderElement;
                        if (extRenderElement != null)
                        {
                            LayoutFarm.UI.IUIEventListener listener = extRenderElement.GetController() as LayoutFarm.UI.IUIEventListener;
                            if (listener != null)
                            {
                                listener.ListenMouseDown(e);
                            }
                        }
                    }
                }
                else
                {
                    _editSession.StartSelectIfNoSelection();
                    _editSession.SetCaretPos(e.X, e.Y);
                    _editSession.EndSelect();
                    InvalidateGraphicOfCurrentLineArea();
                }
            }
        }
        public virtual void HandleMouseWheel(UIMouseEventArgs e)
        {

            //
            if (e.Delta < 0)
            {
                //scroll down
                //this.StepSmallToMax();
                //TODO: review Y diff
                ScrollOffset(0, 24);
            }
            else
            {
                //up
                //this.StepSmallToMin();
                //TODO: review Y diff
                ScrollOffset(0, -24);
            }
        }
        public void HandleDoubleClick(UIMouseEventArgs e)
        {
            _editSession.CancelSelect();
            Run textRun = this.CurrentTextRun;
            if (textRun != null)
            {

                VisualPointInfo pointInfo = _editSession.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                int local_sel_Index = pointInfo.RunLocalSelectedIndex;
                //default behaviour is select only a hit word under the caret
                //so ask the text layer to find a hit word
                int startAt, len;
                _editSession.FindUnderlyingWord(out startAt, out len);
                if (len > 0)
                {
                    InvalidateGraphicOfCurrentLineArea();
                    _editSession.TryMoveCaretTo(startAt, true);
                    _editSession.StartSelect();
                    _editSession.TryMoveCaretTo(startAt + len);
                    _editSession.EndSelect();


                    //internalTextLayerController.TryMoveCaretTo(lineCharacterIndex - local_sel_Index, true);
                    //internalTextLayerController.StartSelect();
                    //internalTextLayerController.TryMoveCaretTo(internalTextLayerController.CharIndex + textRun.CharacterCount);
                    //internalTextLayerController.EndSelect();

                    InvalidateGraphicOfCurrentLineArea();
                }
            }
        }
        public void FindCurrentUnderlyingWord(out int startAt, out int len)
        {
            Run textRun = this.CurrentTextRun;
            if (textRun != null)
            {

                VisualPointInfo pointInfo = _editSession.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                int local_sel_Index = pointInfo.RunLocalSelectedIndex;
                //default behaviour is select only a hit word under the caret
                //so ask the text layer to find a hit word                 
                _editSession.FindUnderlyingWord(out startAt, out len);
            }
            else
            {
                startAt = len = 0;
            }
        }
        public void HandleDrag(UIMouseEventArgs e)
        {
            if (!_isDragBegin)
            {
                //dbugMouseDragBegin++;
                //first time
                _isDragBegin = true;
                if (e.Button == UIMouseButtons.Left)
                {
                    _editSession.SetCaretPos(e.X, e.Y);
                    _editSession.StartSelect();
                    _editSession.EndSelect();

                    InvalidateGraphicOfCurrentSelectionArea();
                }
            }
            else
            {
                //dbugMouseDragging++;
                if (e.Button == UIMouseButtons.Left)
                {
                    _editSession.StartSelectIfNoSelection();
                    _editSession.SetCaretPos(e.X, e.Y);
                    _editSession.EndSelect();

                    InvalidateGraphicOfCurrentSelectionArea();
                }
            }
        }
        public void HandleDragEnd(UIMouseEventArgs e)
        {
            _isDragBegin = false;
            if (e.Button == UIMouseButtons.Left)
            {
                _editSession.StartSelectIfNoSelection();
                _editSession.SetCaretPos(e.X, e.Y);
                _editSession.EndSelect();
                //this.InvalidateGraphics();
                InvalidateGraphicOfCurrentSelectionArea();
            }
        }
        public bool IsMultiLine => _isMultiLine;

        //
        public int CurrentLineHeight => _editSession.CurrentLineArea.Height;
        //
        public int CurrentLineCharIndex => _editSession.CurrentLineCharIndex;
        //
        public int CurrentTextRunCharIndex => _editSession.CurrentTextRunCharIndex;
        //
        public int CurrentLineNumber
        {
            get => _editSession.CurrentLineNumber;
            set => _editSession.CurrentLineNumber = value;
        }
        //
        public void ScrollToCurrentLine()
        {
            this.ScrollToLocation(0, _editSession.CaretPos.Y);
        }

        protected void RefreshSnapshotCanvas()
        {
            //TODO: review this method
        }
        public void HandleMouseUp(UIMouseEventArgs e)
        {
            //empty?
        }
        //
        public Run CurrentTextRun => _editSession.CurrentTextRun;
        //
        public void GetSelectedText(StringBuilder output)
        {
            _editSession.CopySelectedTextToPlainText(output);
        }

        protected void EnsureLocationVisible(Point textManCaretPos)
        {

            textManCaretPos.Offset(-ViewportLeft, -ViewportTop);
            //----------------------  
            //horizontal
            if (textManCaretPos.X >= this.Width)
            {
                if (!_isMultiLine)
                {
                    var r = _editSession.CurrentLineArea;

                    //Rectangle r = internalTextLayerController.CurrentParentLineArea;
                    if (r.Width >= this.Width)
                    {
#if DEBUG
                        dbug_SetInitObject(this);
                        dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.ArtVisualTextSurafce_EnsureCaretVisible);
#endif
                        //SetCalculatedSize(this, r.Width, r.Height);
                        //InnerDoTopDownReCalculateContentSize(this);
                        this.OnTextContentSizeChanged();
                        RefreshSnapshotCanvas();
#if DEBUG
                        dbug_EndLayoutTrace();
#endif
                    }
                }
                else
                {
                }

                ScrollOffset(textManCaretPos.X - this.Width, 0);
            }
            else if (textManCaretPos.X < 0)
            {
                ScrollOffset(textManCaretPos.X - this.X, 0);
            }

            Size innerContentSize = this.InnerContentSize;
            if (ViewportLeft > 0 && innerContentSize.Width - ViewportLeft < this.Width)
            {
                ScrollToLocation(this.InnerContentSize.Width - ViewportLeft, 0);
            }
            //----------------------  
            //vertical ??
            //----------------------  
            if (_editSession._updateJustCurrentLine)
            {
                InvalidateGraphicOfCurrentLineArea();
            }
            else
            {
                InvalidateGraphics();
            }
        }


        public Color BackgroundColor { get; set; }
        public event EventHandler ViewportChanged;
        public event EventHandler ContentSizeChanged;

        public bool RenderBackground { get; set; }

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
        public void RunVisitor(RunVisitor visitor)
        {
            //1. bg, no nothing
            visitor.CurrentCaretPos = _editSession.CaretPos;
            //2. markers 
            if (!visitor.SkipMarkerLayer && _markerLayer != null &&
                _markerLayer.VisualMarkerCount > 0)
            {
                visitor.OnBeginMarkerLayer();
                foreach (VisualMarkerSelectionRange marker in _markerLayer.VisualMarkers)
                {
                    visitor.VisitMarker(marker);
                }
                visitor.OnEndMarkerLayer();
            }

            //3.
            if (!visitor.SkipSelectionLayer && _editSession.SelectionRange != null)
            {
                visitor.VisitSelectionRange(_editSession.SelectionRange);
            }

            //4. text layer
            visitor.OnBeginTextLayer();
            _textLayer.RunVisitor(visitor);
            visitor.OnEndTextLayer();
            //5. others? 
        }

        //
        public override void ClearAllChildren()
        {
            _editSession.Clear();
            base.ClearAllChildren();
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            RequestFont enterFont = canvas.CurrentFont;

            canvas.CurrentFont = this.CurrentTextSpanStyle.ReqFont;


            //1. bg 
            if (RenderBackground && BackgroundColor.A > 0)
            {
                Size innerBgSize = InnerBackgroundSize;

#if DEBUG
                //canvas.FillRectangle(BackgroundColor, 0, 0, innerBgSize.Width, innerBgSize.Height);
                canvas.FillRectangle(ColorEx.dbugGetRandomColor(), 0, 0, innerBgSize.Width, innerBgSize.Height);
#else
                canvas.FillRectangle(BackgroundColor, 0, 0, innerBgSize.Width, innerBgSize.Height);
#endif


            }


            //2.1 markers 
            if (RenderMarkers && _markerLayer != null &&
                _markerLayer.VisualMarkerCount > 0)
            {
                foreach (VisualMarkerSelectionRange marker in _markerLayer.VisualMarkers)
                {
                    marker.Draw(canvas, updateArea);
                }
            }


            //2.2 selection
            if (RenderSelectionRange && _editSession.SelectionRange != null)
            {
                _editSession.SelectionRange.Draw(canvas, updateArea);
            }



            ////3.1 background selectable layer
            //_textLayer2.Draw(canvas, updateArea);

            //3.2 actual editable layer
            _textLayer.DrawChildContent(canvas, updateArea);
            if (this.HasDefaultLayer)
            {
                this.DrawDefaultLayer(canvas, ref updateArea);
            }
            //----------------------------------------------

#if DEBUG
            //for debug
            //canvas.FillRectangle(Color.Red, 0, 0, 5, 5);

#endif
            ////4. caret 
            //if (RenderCaret && _stateShowCaret && _isEditable)
            //{
            //    Point textManCaretPos = _editSession.CaretPos;
            //    _myCaret.DrawCaret(canvas, textManCaretPos.X, textManCaretPos.Y);
            //}

            canvas.CurrentFont = enterFont;
        }

        internal void OnTextContentSizeChanged()
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public Run LastestHitSolidTextRun => _textLayer.LatestHitRun as SolidRun;
        public Run LastestHitRun => _textLayer.LatestHitRun;


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