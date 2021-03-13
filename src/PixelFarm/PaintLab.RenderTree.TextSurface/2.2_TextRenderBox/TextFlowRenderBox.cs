//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.UI;
using PixelFarm.Drawing;
using Typography.Text;

namespace LayoutFarm.TextFlow
{
    public class TextFlowRenderBox : AbstractRectRenderElement, ITextFlowLayerOwner
    {
        internal TextMarkerLayer _markerLayer;
        internal TextFlowLayer _textLayer; //this is a special layer that render text
        internal VisualTextFlowEditSession _visualEditSession;

        internal int _verticalExpectedCharIndex;
        internal readonly bool _isMultiLine = false;


        internal bool _isInVerticalPhase = false;
        internal bool _isFocus = false;

        bool _isDragBegin;

        public TextFlowRenderBox(int width, int height, bool isMultiLine)
            : base(width, height)
        {
            var defaultRunStyle = new RunStyle();
            defaultRunStyle.FontColor = Color.Black;//set default

            defaultRunStyle.ReqFont = GlobalRootGraphic.CurrentRootGfx.DefaultTextEditFontInfo;//TODO: review here

            _textLayer = new TextFlowLayer(this, defaultRunStyle);
            _textLayer.AppendNewLine(new TextLineBox(_textLayer));//
            _textLayer.ContentSizeChanged += (s, e) => OnTextContentSizeChanged();

            //create 
            _visualEditSession = new VisualTextFlowEditSession(_textLayer);//controller
            _isMultiLine = isMultiLine;

            RenderBackground = RenderSelectionRange = RenderMarkers = true;
            //
            MayHasViewport = true;
            BackgroundColor = Color.White;// Color.Transparent; 
        }

        public static VisualTextFlowEditSession GetEditSession(TextFlowRenderBox textFlowRenderBox) => textFlowRenderBox._visualEditSession;

        public void Reload(PlainTextDocument plainTextDoc)
        {
            //clear
            _visualEditSession.Clear();
            int j = plainTextDoc.LineCount;
            using (var temp = new StringBuilderPoolContext<TextFlowRenderBox>(out StringBuilder sb))
            {
                for (int i = 0; i < j; ++i)
                {
                    sb.Length = 0;//clear
                    if (i > 0)
                    {
                        _visualEditSession.SplitIntoNewLine();
                    }
                    plainTextDoc.WriteLineTo(i, sb);
                    _visualEditSession.AddText(sb.ToString());
                }
            }

        }
        public void SelectAll()
        {
            _visualEditSession.SelectAll();
        }
        public TextDrawingTech TextDrawingTech { get; set; } = TextDrawingTech.LcdSubPix;
        internal TextFlowLayer TextFlowLayer => _textLayer;
        public Color SelectionTextColor { get; set; } = Color.Black;
        public Color SelectionBackgroundColor { get; set; } = Color.Yellow;
        void ITextFlowLayerOwner.ClientLayerBubbleUpInvalidateArea(Rectangle clientInvalidatedArea)
        {
            //client line send up 
            clientInvalidatedArea.Offset(this.X, this.Y);
            InvalidateParentGraphics(clientInvalidatedArea);

            ////TODO: review invalidate bubble
            //_owner.InvalidateParentGraphics(bubbleUpInvArea);
        }
        //TODO: review here
        //in our editor we replace user tab with space
        //TODO: we may use a single 'solid-run' for a Tab
        public byte NumOfWhitespaceForSingleTab { get; set; }


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
                _textLayer.SetDefaultRunStyle(new RunStyle()
                {
                    FontColor = value.FontColor,
                    ReqFont = value.ReqFont,
                    ContentHAlign = value.ContentHAlign
                });
            }
        }

        protected Rectangle GetSelectionUpdateArea()
        {
            VisualSelectionRange selectionRange = _visualEditSession.SelectionRange;
            if (selectionRange != null && selectionRange.IsValid)
            {
                return _visualEditSession.SelectionRange.GetSelectionUpdateArea();
            }
            else
            {
                return Rectangle.Empty;
            }
        }
#if DEBUG
        public Rectangle dbugGetRectAreaOf(int beginlineNum, int beginColumnNum, int endLineNum, int endColumnNum)
        {
            TextFlowLayer flowLayer = _textLayer;
            TextLineBox beginLine = flowLayer.GetTextLineAtPos(beginlineNum);
            if (beginLine == null)
            {
                return Rectangle.Empty;
            }
            if (beginlineNum == endLineNum)
            {
                VisualPointInfo beginPoint = beginLine.GetTextPointInfoFromCharIndex(beginColumnNum);
                VisualPointInfo endPoint = beginLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }
            else
            {
                VisualPointInfo beginPoint = beginLine.GetTextPointInfoFromCharIndex(beginColumnNum);
                TextLineBox endLine = flowLayer.GetTextLineAtPos(endLineNum);
                VisualPointInfo endPoint = endLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }
        }
#endif
        protected void InvalidateGraphicOfCurrentLineArea()
        {
#if DEBUG
            //Rectangle c_lineArea = _internalTextLayerController.CurrentParentLineArea;
#endif
            Rectangle lineArea = _visualEditSession.CurrentLineArea;
            lineArea.Width = this.Width; //change original line area' width to this render element width
#if DEBUG
            //if (lineArea.Height == 31)
            //{

            //}
#endif
            InvalidateGraphics(lineArea);
        }
        protected void InvalidateGraphicOfCurrentSelectionArea()
        {

            VisualSelectionRange selectionRange;
            if ((selectionRange = _visualEditSession.SelectionRange) != null && selectionRange.IsValid)
            {
                InvalidateGraphics(selectionRange.GetSelectionUpdateArea());
            }
        }

        public bool HasSomeText => (_textLayer.LineCount > 0) && _textLayer.GetTextLine(0).RunCount > 0;

        public virtual void Focus() => _isFocus = true;

        public virtual void Blur() => _isFocus = false;

        public void CancelSelection() => _visualEditSession.CancelSelect();

        public bool IsFocused => _isFocus;

        public int InnerContentWidth => IsMultiLine ? this.Width : _visualEditSession.CurrentLineArea.Width;
        public int InnerContentHeight => IsMultiLine ? _textLayer.Bottom : _visualEditSession.CurrentLineArea.Height;

        public virtual void HandleMouseDown(UIMouseDownEventArgs e)
        {
            if (e.Buttons == UIMouseButtons.Left)
            {
                InvalidateGraphicOfCurrentLineArea();

                if (!e.Shift)
                {
                    _visualEditSession.SetCaretPos(e.X, e.Y);
                    if (_visualEditSession.SelectionRange != null)
                    {
                        Rectangle r = GetSelectionUpdateArea();
                        _visualEditSession.CancelSelect();
                        InvalidateGraphics(r);
                    }
                    else
                    {
                        InvalidateGraphicOfCurrentLineArea();
                    }

                    if (_visualEditSession.LatestHitRun is SolidRun latestHitSolidTextRun &&
                        latestHitSolidTextRun.ExternalRenderElement?.GetController() is LayoutFarm.UI.IUIEventListener listener)
                    {
                        //we mousedown on the solid text run
                        listener.ListenMouseDown(e);
                    }
                }
                else
                {
                    _visualEditSession.StartSelectIfNoSelection();
                    _visualEditSession.SetCaretPos(e.X, e.Y);
                    _visualEditSession.EndSelect();
                    InvalidateGraphicOfCurrentLineArea();
                }
            }
        }
        public virtual void HandleMouseWheel(UIMouseWheelEventArgs e)
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


        public virtual void HandleDoubleClick(UIMouseEventArgs e)
        {
            _visualEditSession.CancelSelect();
            Run textRun = this.CurrentTextRun;
            if (textRun != null)
            {
#if DEBUG
                VisualPointInfo pointInfo = _visualEditSession.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                //int local_sel_Index = pointInfo.RunLocalSelectedIndex;
#endif

                //default behaviour is select only a hit word under the caret
                //so ask the text layer to find a hit word
                _visualEditSession.FindUnderlyingWord(out int startAt, out int len);
                if (len > 0)
                {
                    InvalidateGraphicOfCurrentLineArea();
                    _visualEditSession.TryMoveCaretTo(startAt, true);
                    _visualEditSession.StartSelect();
                    _visualEditSession.TryMoveCaretTo(startAt + len);
                    _visualEditSession.EndSelect();


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
#if DEBUG
                VisualPointInfo pointInfo = _visualEditSession.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                //int local_sel_Index = pointInfo.RunLocalSelectedIndex;
#endif
                //default behaviour is select only a hit word under the caret
                //so ask the text layer to find a hit word                 
                _visualEditSession.FindUnderlyingWord(out startAt, out len);
            }
            else
            {
                startAt = len = 0;
            }
        }


        public virtual void HandleDrag(UIMouseMoveEventArgs e)
        {
            if (!_isDragBegin)
            {
                //dbugMouseDragBegin++;
                //first time
                _isDragBegin = true;
                _visualEditSession.SetCaretPos(e.X, e.Y);
                _visualEditSession.StartSelect();
                _visualEditSession.EndSelect();
            }
            else
            {
                _visualEditSession.StartSelectIfNoSelection();
                _visualEditSession.SetCaretPos(e.X, e.Y);
                _visualEditSession.EndSelect();
            }

            InvalidateGraphicOfCurrentSelectionArea();
        }

        public virtual void HandleDragEnd(UIMouseUpEventArgs e)
        {
            _isDragBegin = false;

            _visualEditSession.StartSelectIfNoSelection();
            _visualEditSession.SetCaretPos(e.X, e.Y);
            _visualEditSession.EndSelect();
            //this.InvalidateGraphics();
            InvalidateGraphicOfCurrentSelectionArea();
        }

        public virtual void HandleKeyPress(UIKeyEventArgs e)
        {

            //------------------------
            if (e.IsControlCharacter)
            {
                HandleKeyDown(e);
                return;
            }

#if DEBUG
            char c = e.KeyChar;
#endif
            e.CancelBubbling = true;
            InvalidateGraphicOfCurrentSelectionArea();

            _visualEditSession.UpdateSelectionRange();
            EnsureCaretVisible();
        }

        public virtual void HandleKeyUp(UIKeyEventArgs e) { }
        public virtual void HandleKeyDown(UIKeyEventArgs e)
        {

        }

        public virtual void DoHome(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                _visualEditSession.DoHome();
                _visualEditSession.CancelSelect();
            }
            else
            {
                _visualEditSession.StartSelectIfNoSelection(); //start select before move to home
                _visualEditSession.DoHome(); //move cursor to default home 
                _visualEditSession.EndSelect(); //end selection
            }

            EnsureCaretVisible();
        }
        public virtual void DoEnd(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                _visualEditSession.DoEnd();
                _visualEditSession.CancelSelect();
            }
            else
            {
                _visualEditSession.StartSelectIfNoSelection();
                _visualEditSession.DoEnd();
                _visualEditSession.EndSelect();
            }

            EnsureCaretVisible();
        }
        public bool IsMultiLine => _isMultiLine;

        //
        public int CurrentLineHeight => _visualEditSession.CurrentLineArea.Height;
        //
        public int CurrentLineCharIndex => _visualEditSession.CurrentLineNewCharIndex;
        //
         
        //
        public int CurrentLineNumber
        {
            get => _visualEditSession.CurrentLineNumber;
            set => _visualEditSession.CurrentLineNumber = value;
        }
        //
        public void ScrollToCurrentLine()
        {
            this.ScrollToLocation(0, _visualEditSession.CaretPos.Y);
        }

        protected void RefreshSnapshotCanvas()
        {
            //TODO: review this method
        }
        public void HandleMouseUp(UIMouseUpEventArgs e)
        {
            //empty?
        }
        //
        public Run CurrentTextRun => _visualEditSession.CurrentTextRun;
        //
        //public void GetSelectedText(StringBuilder output)
        //{
        //    throw new NotSupportedException();
        //    //_editSession.CopySelectedTextToPlainText(output);
        //}

        protected void EnsureLocationVisible(int textManCaretPosX, int textManCaretPosY)
        {
            textManCaretPosX -= ViewportLeft;
            textManCaretPosY -= ViewportTop;


            //----------------------  
            //horizontal
            if (textManCaretPosX >= this.Width)
            {
                if (!_isMultiLine)
                {
                    var r = _visualEditSession.CurrentLineArea;

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

                ScrollOffset(textManCaretPosX - this.Width, 0);
            }
            else if (textManCaretPosX < 0)
            {
                ScrollOffset(textManCaretPosX - this.X, 0);
            }


            int innerContentWidth = this.InnerContentWidth;
            if (ViewportLeft > 0 && innerContentWidth - ViewportLeft < this.Width)
            {
                ScrollToLocation(innerContentWidth - ViewportLeft, 0);
            }
            //----------------------  
            //vertical ??
            //----------------------  
            if (_visualEditSession._updateJustCurrentLine)
            {
                InvalidateGraphicOfCurrentLineArea();
            }
            else
            {
                InvalidateGraphics();
            }
        }


        Color _bgColor;
        public Color BackgroundColor
        {
            get => _bgColor;
            set
            {
                _bgColor = value;
                BgIsNotOpaque = value.A < 255;
                if (this.HasParentLink)
                {
                    this.InvalidateGraphics();
                }
            }
        }
        public event EventHandler ViewportChanged;
        public event EventHandler ContentSizeChanged;

        public bool RenderBackground { get; set; }
        public bool RenderMarkers { get; set; }
        public bool RenderSelectionRange { get; set; }

        public void RunVisitor(RunVisitor visitor)
        {
            //1. bg, no nothing
            visitor.CurrentCaretPos = _visualEditSession.CaretPos;
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
            if (!visitor.SkipSelectionLayer && _visualEditSession.SelectionRange != null)
            {
                visitor.VisitSelectionRange(_visualEditSession.SelectionRange);
            }

            //4. text layer
            visitor.OnBeginTextLayer();
            _textLayer.AcceptVisitor(visitor);
            visitor.OnEndTextLayer();
            //5. others? 
        }

        public void ClearAllChildren() => _visualEditSession.Clear();

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            RequestFont enterFont = d.CurrentFont;
            TextDrawingTech prev_text_drawing_tech = d.TextDrawingTech;//backup

            d.TextDrawingTech = this.TextDrawingTech;

            d.CurrentFont = this.CurrentTextSpanStyle.ReqFont;
            //1. bg 
            if (RenderBackground && BackgroundColor.A > 0)
            {
#if DEBUG
                d.FillRectangle(BackgroundColor, 0, 0, Width, Height);
                //canvas.FillRectangle(ColorEx.dbugGetRandomColor(), 0, 0, innerBgSize.Width, innerBgSize.Height);
#else
                d.FillRectangle(BackgroundColor, 0, 0, Width, Height);
#endif
                d.TextBackgroundColorHint = BackgroundColor;
            }

            //2.1 markers 
            if (RenderMarkers && _markerLayer != null && _markerLayer.VisualMarkerCount > 0)
            {
                for (int i = 0; i < _markerLayer.VisualMarkerCount; ++i)
                {
                    _markerLayer.VisualMarkers[i].Draw(d, updateArea);
                }

            }

            Color prev_hintColor = d.TextBackgroundColorHint;
            if (RenderSelectionRange && _visualEditSession.SelectionRange != null)
            {
                //with selection
                _visualEditSession.SelectionRange.FontColor = SelectionTextColor;
                _visualEditSession.SelectionRange.BackgroundColor = SelectionBackgroundColor;

                if (d.TextDrawingTech == TextDrawingTech.LcdSubPix)
                {
                    TextRun.s_currentRenderE = this;
                    _textLayer.DrawChildContentLcdEffectText(d, updateArea, _visualEditSession.SelectionRange);
                    TextRun.s_currentRenderE = null; //temp fix

                }
                else
                {
                    _visualEditSession.SelectionRange.Draw(d, updateArea);

                    TextRun.s_currentRenderE = this;//temp fix
                    _textLayer.DrawChildContent(d, updateArea);
                    TextRun.s_currentRenderE = null;//temp fix 
                }

            }
            else
            {
                //no selection 
                TextRun.s_currentRenderE = this;//temp fix
                _textLayer.DrawChildContent(d, updateArea);
                TextRun.s_currentRenderE = null;//temp fix 
            }
#if DEBUG
            //for debug
            //canvas.FillRectangle(Color.Red, 0, 0, 5, 5);
#endif

            d.CurrentFont = enterFont;
            d.TextBackgroundColorHint = prev_hintColor;
            d.TextDrawingTech = prev_text_drawing_tech;
        }

        internal void OnTextContentSizeChanged()
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        //public Run LastestHitSolidTextRun => _textLayer.LatestHitRun as SolidRun;
        //public Run LastestHitRun => _textLayer.LatestHitRun;


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
            int innerContentW = this.InnerContentWidth;
            if (x > innerContentW - Width)
            {
                x = innerContentW - Width;
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
            out UIScrollEventArgs hScrollEventArgs,
            out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;
            int innerContentH = this.InnerContentHeight;
            if (dy < 0)
            {
#if DEBUG
                int old_y = this.ViewportTop;
#endif
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
                if (viewportButtom + dy > innerContentH)
                {
                    int vwY = innerContentH - Height;
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
                if (viewportRight + dx > InnerContentWidth)
                {
                    this.SetViewport(this.ViewportLeft + dx, this.ViewportTop);
                }
                else
                {

                }
            }
        }

        public int LineCount => _visualEditSession.LineCount;

        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            _visualEditSession.ReplaceLocalContent(nBackspace, t);
        }
        public void CopyCurrentLine(TextCopyBuffer output)
        {
            _visualEditSession.CopyCurrentLine(output);
        }
        public void CopyLine(int lineNum, TextCopyBuffer output)
        {
            _visualEditSession.CopyLine(lineNum, output);
        }
        public void CopyContentToStringBuilder(TextCopyBuffer output)
        {
            _visualEditSession.CopyAllToPlainText(output);
        }

        public void SplitCurrentLineToNewLine()
        {
            _visualEditSession.SplitIntoNewLine();
        }

        protected virtual void EnsureCaretVisible()
        {
            Point textManCaretPos = _visualEditSession.CaretPos;
            EnsureLocationVisible(textManCaretPos.X, textManCaretPos.Y);
        }

        public virtual bool HandleProcessDialogKey(UIKeyEventArgs e)
        {
            UIKeys keyData = (UIKeys)e.KeyData;

            if (_isInVerticalPhase && (keyData != UIKeys.Up || keyData != UIKeys.Down))
            {
                _isInVerticalPhase = false;
            }

            switch (e.KeyCode)
            {
                case UIKeys.Left:
                    {
                        InvalidateGraphicOfCurrentLineArea();
                        if (!e.Shift)
                        {
                            _visualEditSession.CancelSelect();
                        }
                        else
                        {
                            _visualEditSession.StartSelectIfNoSelection();
                        }

                        Point currentCaretPos = Point.Empty;
                        if (!_isMultiLine)
                        {
                            if (!_visualEditSession.IsOnStartOfLine)
                            {
#if DEBUG
                                Point prvCaretPos = _visualEditSession.CaretPos;
#endif
                                _visualEditSession.TryMoveCaretBackward();
                                currentCaretPos = _visualEditSession.CaretPos;
                            }
                        }
                        else
                        {
                            if (_visualEditSession.IsOnStartOfLine)
                            {
                                _visualEditSession.TryMoveCaretBackward();
                                currentCaretPos = _visualEditSession.CaretPos;
                            }
                            else
                            {
                                if (!_visualEditSession.IsOnStartOfLine)
                                {
#if DEBUG
                                    Point prvCaretPos = _visualEditSession.CaretPos;
#endif
                                    _visualEditSession.TryMoveCaretBackward();
                                    currentCaretPos = _visualEditSession.CaretPos;
                                }
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _visualEditSession.EndSelectIfNoSelection();
                        }
                        //-------------------

                        EnsureCaretVisible();

                        return true;
                    }
                case UIKeys.Right:
                    {

                        InvalidateGraphicOfCurrentLineArea();
                        if (!e.Shift)
                        {
                            _visualEditSession.CancelSelect();
                        }
                        else
                        {
                            _visualEditSession.StartSelectIfNoSelection();
                        }


                        Point currentCaretPos = Point.Empty;
                        if (!_isMultiLine)
                        {
#if DEBUG
                            Point prvCaretPos = _visualEditSession.CaretPos;
#endif
                            _visualEditSession.TryMoveCaretForward();
                            currentCaretPos = _visualEditSession.CaretPos;
                        }
                        else
                        {
                            if (_visualEditSession.IsOnEndOfLine)
                            {
                                _visualEditSession.TryMoveCaretForward();
                                currentCaretPos = _visualEditSession.CaretPos;
                            }
                            else
                            {
#if DEBUG
                                Point prvCaretPos = _visualEditSession.CaretPos;
#endif
                                _visualEditSession.TryMoveCaretForward();
                                currentCaretPos = _visualEditSession.CaretPos;
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _visualEditSession.EndSelectIfNoSelection();
                        }
                        //-------------------

                        EnsureCaretVisible();


                        return true;
                    }
                case UIKeys.PageUp:
                    {

                        if (_isMultiLine)
                        {
                            if (!_isInVerticalPhase)
                            {
                                _isInVerticalPhase = true;
                                _verticalExpectedCharIndex = _visualEditSession.CurrentLineNewCharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _visualEditSession.CancelSelect();
                            }
                            else
                            {
                                _visualEditSession.StartSelectIfNoSelection();
                            }
                            //----------------------------
                            //approximate line per viewport
                            int line_per_viewport = Height / _visualEditSession.CurrentLineArea.Height;
                            if (line_per_viewport > 1)
                            {
                                if (_visualEditSession.CurrentLineNumber - line_per_viewport < 0)
                                {
                                    //move to first line
                                    _visualEditSession.CurrentLineNumber = 0;
                                }
                                else
                                {
                                    _visualEditSession.CurrentLineNumber -= line_per_viewport;
                                }
                            }



                            if (_verticalExpectedCharIndex > _visualEditSession.CurrentLineCharCount - 1)
                            {
                                _visualEditSession.TryMoveCaretTo(_visualEditSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _visualEditSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }

                            //----------------------------
                            if (e.Shift)
                            {
                                _visualEditSession.EndSelectIfNoSelection();
                            }

                            Rectangle lineArea = _visualEditSession.CurrentLineArea;
                            if (lineArea.Top < ViewportTop)
                            {
                                ScrollOffset(0, lineArea.Top - ViewportTop);
                            }
                            else
                            {
                                EnsureCaretVisible();
                                InvalidateGraphicOfCurrentLineArea();
                            }
                        }
                        else
                        {
                        }

                        return true;
                    }

                case UIKeys.PageDown:
                    {


                        if (_isMultiLine)
                        {
                            if (!_isInVerticalPhase)
                            {
                                _isInVerticalPhase = true;
                                _verticalExpectedCharIndex = _visualEditSession.CurrentLineNewCharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _visualEditSession.CancelSelect();
                            }
                            else
                            {
                                _visualEditSession.StartSelectIfNoSelection();
                            }
                            //---------------------------- 

                            int line_per_viewport = Height / _visualEditSession.CurrentLineArea.Height;

                            if (_visualEditSession.CurrentLineNumber + line_per_viewport < _visualEditSession.LineCount)
                            {

                                _visualEditSession.CurrentLineNumber += line_per_viewport;
                            }
                            else
                            {
                                //move to last line
                                _visualEditSession.CurrentLineNumber = _visualEditSession.LineCount - 1;
                            }

                            if (_verticalExpectedCharIndex > _visualEditSession.CurrentLineCharCount - 1)
                            {
                                _visualEditSession.TryMoveCaretTo(_visualEditSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _visualEditSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }
                            //----------------------------

                            if (e.Shift)
                            {
                                _visualEditSession.EndSelectIfNoSelection();
                            }
                            //----------------------------
                            Rectangle lineArea = _visualEditSession.CurrentLineArea;
                            if (lineArea.Bottom > this.ViewportBottom)
                            {
                                ScrollOffset(0, lineArea.Bottom - this.ViewportBottom);
                            }
                            else
                            {

                                InvalidateGraphicOfCurrentLineArea();
                            }

                        }
                        EnsureCaretVisible();

                        return true;
                    }
                case UIKeys.Down:
                    {

                        if (_isMultiLine)
                        {
                            if (!_isInVerticalPhase)
                            {
                                _isInVerticalPhase = true;
                                _verticalExpectedCharIndex = _visualEditSession.CurrentLineNewCharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _visualEditSession.CancelSelect();
                            }
                            else
                            {
                                _visualEditSession.StartSelectIfNoSelection();
                            }
                            //---------------------------- 

                            _visualEditSession.CurrentLineNumber++;
                            if (_verticalExpectedCharIndex > _visualEditSession.CurrentLineCharCount - 1)
                            {
                                _visualEditSession.TryMoveCaretTo(_visualEditSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _visualEditSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }
                            //----------------------------

                            if (e.Shift)
                            {
                                _visualEditSession.EndSelectIfNoSelection();
                            }
                            //----------------------------
                            Rectangle lineArea = _visualEditSession.CurrentLineArea;
                            if (lineArea.Bottom > this.ViewportBottom)
                            {
                                ScrollOffset(0, lineArea.Bottom - this.ViewportBottom);
                            }
                            else
                            {

                                InvalidateGraphicOfCurrentLineArea();
                            }
                        }
                        EnsureCaretVisible();

                        return true;
                    }
                case UIKeys.Up:
                    {

                        if (_isMultiLine)
                        {
                            if (!_isInVerticalPhase)
                            {
                                _isInVerticalPhase = true;
                                _verticalExpectedCharIndex = _visualEditSession.CurrentLineNewCharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _visualEditSession.CancelSelect();
                            }
                            else
                            {
                                _visualEditSession.StartSelectIfNoSelection();
                            }
                            //----------------------------

                            _visualEditSession.CurrentLineNumber--;
                            if (_verticalExpectedCharIndex > _visualEditSession.CurrentLineCharCount - 1)
                            {
                                _visualEditSession.TryMoveCaretTo(_visualEditSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _visualEditSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }

                            //----------------------------
                            if (e.Shift)
                            {
                                _visualEditSession.EndSelectIfNoSelection();
                            }

                            Rectangle lineArea = _visualEditSession.CurrentLineArea;
                            if (lineArea.Top < ViewportTop)
                            {
                                ScrollOffset(0, lineArea.Top - ViewportTop);
                            }
                            else
                            {
                                EnsureCaretVisible();
                                InvalidateGraphicOfCurrentLineArea();
                            }
                        }
                        else
                        {
                        }
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
        public Point CurrentCaretPos => _visualEditSession.CaretPos;
    }
}