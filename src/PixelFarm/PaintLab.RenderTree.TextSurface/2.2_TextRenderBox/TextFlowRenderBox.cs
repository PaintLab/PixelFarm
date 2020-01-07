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
        internal TextMarkerLayer _markerLayer;
        internal TextFlowLayer _textLayer; //this is a special layer that render text
        internal TextFlowEditSession _editSession;

        internal int _verticalExpectedCharIndex;
        internal readonly bool _isMultiLine = false;


        internal bool _isInVerticalPhase = false;
        internal bool _isFocus = false;

        internal bool _isDragBegin;

        public TextFlowRenderBox(RootGraphic rootgfx, int width, int height, bool isMultiLine)
            : base(rootgfx, width, height)
        {
            var defaultRunStyle = new RunStyle();
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
        public static TextFlowEditSession GetCurrentEditSession(TextFlowRenderBox textEditRenderBox)
        {
            return textEditRenderBox._editSession;
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
            VisualSelectionRange selectionRange = _editSession.SelectionRange;
            if (selectionRange != null && selectionRange.IsValid)
            {
                return _editSession.SelectionRange.GetSelectionUpdateArea();
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

        public virtual void Focus()
        {
            _isFocus = true;
        }
        public virtual void Blur()
        {
            _isFocus = false;
        }

        public void CancelSelection()
        {
            _editSession.CancelSelect();
        }
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
                            IUIEventListener listener = extRenderElement.GetController() as LayoutFarm.UI.IUIEventListener;
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


        public virtual void HandleDoubleClick(UIMouseEventArgs e)
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
        public virtual void HandleDrag(UIMouseEventArgs e)
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
        public virtual void HandleDragEnd(UIMouseEventArgs e)
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

        public virtual void HandleKeyPress(UIKeyEventArgs e)
        {

            //------------------------
            if (e.IsControlCharacter)
            {
                HandleKeyDown(e);
                return;
            }
            char c = e.KeyChar;
            e.CancelBubbling = true;
            if (_editSession.SelectionRange != null
                && _editSession.SelectionRange.IsValid)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }
            _editSession.UpdateSelectionRange();
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
                _editSession.DoHome();
                _editSession.CancelSelect();
            }
            else
            {
                _editSession.StartSelectIfNoSelection(); //start select before move to home
                _editSession.DoHome(); //move cursor to default home 
                _editSession.EndSelect(); //end selection
            }

            EnsureCaretVisible();
        }
        public virtual void DoEnd(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                _editSession.DoEnd();
                _editSession.CancelSelect();
            }
            else
            {
                _editSession.StartSelectIfNoSelection();
                _editSession.DoEnd();
                _editSession.EndSelect();
            }

            EnsureCaretVisible();
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

                ScrollOffset(textManCaretPosX - this.Width, 0);
            }
            else if (textManCaretPosX < 0)
            {
                ScrollOffset(textManCaretPosX - this.X, 0);
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
            _textLayer.AcceptVisitor(visitor);
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
                canvas.FillRectangle(BackgroundColor, 0, 0, innerBgSize.Width, innerBgSize.Height);
                //canvas.FillRectangle(ColorEx.dbugGetRandomColor(), 0, 0, innerBgSize.Width, innerBgSize.Height);
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

        public int LineCount => _editSession.LineCount;

        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            _editSession.ReplaceLocalContent(nBackspace, t);
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<Run> textRuns)
        {
            _editSession.ReplaceCurrentLineTextRun(textRuns);
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            _editSession.CopyCurrentLine(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            _editSession.CopyLine(lineNum, output);
        }
        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            _editSession.CopyAllToPlainText(stBuilder);
        }
        public void SplitCurrentLineToNewLine()
        {
            _editSession.SplitCurrentLineIntoNewLine();
        }
        public void AddTextRun(Run textspan)
        {
            _editSession.AddTextRunToCurrentLine(textspan);
        }
        public void AddTextRun(char[] buffer)
        {
            _editSession.AddTextRunToCurrentLine(buffer);
        }
        public void AddTextLine(PlainTextLine textLine)
        {
            _editSession.AddTextLine(textLine);
        }
        protected virtual void EnsureCaretVisible()
        {
            Point textManCaretPos = _editSession.CaretPos;
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
                            _editSession.CancelSelect();
                        }
                        else
                        {
                            _editSession.StartSelectIfNoSelection();
                        }

                        Point currentCaretPos = Point.Empty;
                        if (!_isMultiLine)
                        {
                            if (!_editSession.IsOnStartOfLine)
                            {
#if DEBUG
                                Point prvCaretPos = _editSession.CaretPos;
#endif
                                _editSession.TryMoveCaretBackward();
                                currentCaretPos = _editSession.CaretPos;
                            }
                        }
                        else
                        {
                            if (_editSession.IsOnStartOfLine)
                            {
                                _editSession.TryMoveCaretBackward();
                                currentCaretPos = _editSession.CaretPos;
                            }
                            else
                            {
                                if (!_editSession.IsOnStartOfLine)
                                {
#if DEBUG
                                    Point prvCaretPos = _editSession.CaretPos;
#endif
                                    _editSession.TryMoveCaretBackward();
                                    currentCaretPos = _editSession.CaretPos;
                                }
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _editSession.EndSelectIfNoSelection();
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
                            _editSession.CancelSelect();
                        }
                        else
                        {
                            _editSession.StartSelectIfNoSelection();
                        }


                        Point currentCaretPos = Point.Empty;
                        if (!_isMultiLine)
                        {
#if DEBUG
                            Point prvCaretPos = _editSession.CaretPos;
#endif
                            _editSession.TryMoveCaretForward();
                            currentCaretPos = _editSession.CaretPos;
                        }
                        else
                        {
                            if (_editSession.IsOnEndOfLine)
                            {
                                _editSession.TryMoveCaretForward();
                                currentCaretPos = _editSession.CaretPos;
                            }
                            else
                            {
#if DEBUG
                                Point prvCaretPos = _editSession.CaretPos;
#endif
                                _editSession.TryMoveCaretForward();
                                currentCaretPos = _editSession.CaretPos;
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _editSession.EndSelectIfNoSelection();
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
                                _verticalExpectedCharIndex = _editSession.CharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _editSession.CancelSelect();
                            }
                            else
                            {
                                _editSession.StartSelectIfNoSelection();
                            }
                            //----------------------------
                            //approximate line per viewport
                            int line_per_viewport = Height / _editSession.CurrentLineArea.Height;
                            if (line_per_viewport > 1)
                            {
                                if (_editSession.CurrentLineNumber - line_per_viewport < 0)
                                {
                                    //move to first line
                                    _editSession.CurrentLineNumber = 0;
                                }
                                else
                                {
                                    _editSession.CurrentLineNumber -= line_per_viewport;
                                }
                            }



                            if (_verticalExpectedCharIndex > _editSession.CurrentLineCharCount - 1)
                            {
                                _editSession.TryMoveCaretTo(_editSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _editSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }

                            //----------------------------
                            if (e.Shift)
                            {
                                _editSession.EndSelectIfNoSelection();
                            }

                            Rectangle lineArea = _editSession.CurrentLineArea;
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
                                _verticalExpectedCharIndex = _editSession.CharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _editSession.CancelSelect();
                            }
                            else
                            {
                                _editSession.StartSelectIfNoSelection();
                            }
                            //---------------------------- 

                            int line_per_viewport = Height / _editSession.CurrentLineArea.Height;

                            if (_editSession.CurrentLineNumber + line_per_viewport < _editSession.LineCount)
                            {

                                _editSession.CurrentLineNumber += line_per_viewport;
                            }
                            else
                            {
                                //move to last line
                                _editSession.CurrentLineNumber = _editSession.LineCount - 1;
                            }

                            if (_verticalExpectedCharIndex > _editSession.CurrentLineCharCount - 1)
                            {
                                _editSession.TryMoveCaretTo(_editSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _editSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }
                            //----------------------------

                            if (e.Shift)
                            {
                                _editSession.EndSelectIfNoSelection();
                            }
                            //----------------------------
                            Rectangle lineArea = _editSession.CurrentLineArea;
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
                                _verticalExpectedCharIndex = _editSession.CharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _editSession.CancelSelect();
                            }
                            else
                            {
                                _editSession.StartSelectIfNoSelection();
                            }
                            //---------------------------- 

                            _editSession.CurrentLineNumber++;
                            if (_verticalExpectedCharIndex > _editSession.CurrentLineCharCount - 1)
                            {
                                _editSession.TryMoveCaretTo(_editSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _editSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }
                            //----------------------------

                            if (e.Shift)
                            {
                                _editSession.EndSelectIfNoSelection();
                            }
                            //----------------------------
                            Rectangle lineArea = _editSession.CurrentLineArea;
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
                                _verticalExpectedCharIndex = _editSession.CharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _editSession.CancelSelect();
                            }
                            else
                            {
                                _editSession.StartSelectIfNoSelection();
                            }
                            //----------------------------

                            _editSession.CurrentLineNumber--;
                            if (_verticalExpectedCharIndex > _editSession.CurrentLineCharCount - 1)
                            {
                                _editSession.TryMoveCaretTo(_editSession.CurrentLineCharCount);
                            }
                            else
                            {
                                _editSession.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }

                            //----------------------------
                            if (e.Shift)
                            {
                                _editSession.EndSelectIfNoSelection();
                            }

                            Rectangle lineArea = _editSession.CurrentLineArea;
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
        public Point CurrentCaretPos => _editSession.CaretPos;
    }
}