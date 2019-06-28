//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using System.Text;

using LayoutFarm.RenderBoxes;
using LayoutFarm.UI;

using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{

    public sealed partial class TextEditRenderBox : RenderBoxBase, ITextFlowLayerOwner
    {
        EditorCaret _myCaret; //just for render, BUT this render element is not added to parent tree***
        TextFlowLayer _textLayer; //this is a special layer that render text
        TextMarkerLayer _markerLayer;
        SimpleTextSelectableLayer _textLayer2;
        TextFlowEditSession _editSession;


        int _verticalExpectedCharIndex;
        bool _isMultiLine = false;
        bool _isEditable;

        bool _isInVerticalPhase = false;
        bool _isFocus = false;
        bool _stateShowCaret = false;
        bool _isDragBegin;


        public TextEditRenderBox(
            RootGraphic rootgfx,
            int width, int height,
            bool isMultiLine,
            bool isEditable = true)
            : base(rootgfx, width, height)
        {
            _isEditable = isEditable;

            if (isEditable)
            {
                GlobalCaretController.RegisterCaretBlink(rootgfx);
                //
                _myCaret = new EditorCaret(2, 17);
            }

            RenderBackground = RenderCaret = RenderSelectionRange = RenderMarkers = true;
            //
            MayHasViewport = true;
            BackgroundColor = Color.White;// Color.Transparent;

            //-----------
            _textLayer2 = new SimpleTextSelectableLayer(rootgfx);
            _textLayer2.SetOwner(this);
            _textLayer2.SetText("hello\r\nthis is a selectable text layer");
            //-----------

            var defaultRunStyle = new RunStyle(rootgfx.TextServices);
            defaultRunStyle.FontColor = Color.Black;//set default
            defaultRunStyle.ReqFont = rootgfx.DefaultTextEditFontInfo;
            //
            _textLayer = new TextFlowLayer(this, this.Root.TextServices, defaultRunStyle); //presentation
            _textLayer.ContentSizeChanged += (s, e) => OnTextContentSizeChanged();

            //
            _editSession = new TextFlowEditSession(_textLayer);//controller

            _isMultiLine = isMultiLine;

            if (isMultiLine)
            {
                _textLayer.SetUseDoubleCanvas(false, true);
            }
            else
            {
                _textLayer.SetUseDoubleCanvas(true, false);
            }


            IsBlockElement = false;
            NumOfWhitespaceForSingleTab = 4;//default?, configurable?
        }
        public static TextFlowEditSession GetCurrentEditSession(TextEditRenderBox textEditRenderBox)
        {
            return textEditRenderBox._editSession;
        }
        public TextEditing.Commands.DocumentCommandListener DocCommandListener
        {
            get => _editSession.DocCmdListener;
            set => _editSession.DocCmdListener = value;
        }

        void ITextFlowLayerOwner.ClientLayerBubbleUpInvalidateArea(Rectangle clientInvalidatedArea)
        {
            ////client line send up 
            clientInvalidatedArea.Offset(this.X, this.Y);
            InvalidateParentGraphics(clientInvalidatedArea);

            ////TODO: review invalidate bubble
            //_owner.InvalidateParentGraphics(bubbleUpInvArea);
        }
        protected override PlainLayer CreateDefaultLayer() => new PlainLayer(this);
        //
        //public InternalTextLayerController TextLayerController => _internalTextLayerController;
        //
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
        //TODO: review here
        //in our editor we replace user tab with space
        //TODO: we may use a single 'solid-run' for a Tab
        public byte NumOfWhitespaceForSingleTab { get; set; }
        //
        public bool HasSomeText => (_textLayer.LineCount > 0) && _textLayer.GetTextLine(0).RunCount > 0;


        public void DoHome(bool pressShitKey)
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
        public void DoEnd(bool pressShitKey)
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


#if DEBUG
        public Rectangle dbugGetRectAreaOf(int beginlineNum, int beginColumnNum, int endLineNum, int endColumnNum)
        {
            TextFlowLayer flowLayer = _textLayer;
            TextLine beginLine = flowLayer.GetTextLineAtPos(beginlineNum);
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
                TextLine endLine = flowLayer.GetTextLineAtPos(endLineNum);
                VisualPointInfo endPoint = endLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }
        }
#endif

        public void HandleKeyPress(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
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
            bool preventDefault = false;
            if (_textSurfaceEventListener != null &&
                !(preventDefault = TextSurfaceEventListener.NotifyPreviewKeyPress(_textSurfaceEventListener, e)))
            {
                _editSession.UpdateSelectionRange();
            }
            if (preventDefault)
            {
                return;
            }

            if (_isEditable)
            {
                int insertAt = _editSession.CurrentLineCharIndex;

                _editSession.AddCharToCurrentLine(c);

                if (_textSurfaceEventListener != null)
                {
                    //TODO: review this again ***
                    if (_editSession.SelectionRange != null)
                    {
                        TextSurfaceEventListener.NotifyCharacterReplaced(_textSurfaceEventListener, e.KeyChar);
                    }
                    else
                    {
                        TextSurfaceEventListener.NotifyCharacterAdded(_textSurfaceEventListener, insertAt, e.KeyChar);
                    }
                }
            }


            EnsureCaretVisible();

            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(_textSurfaceEventListener, e); ;
            }
        }

        void InvalidateGraphicOfCurrentLineArea()
        {
#if DEBUG
            //Rectangle c_lineArea = _internalTextLayerController.CurrentParentLineArea;
#endif
            Rectangle lineArea = _editSession.CurrentLineArea;
            lineArea.Width = this.Width; //change original line area' width to this render element width

            InvalidateGraphicLocalArea(this, lineArea);
        }
        void InvalidateGraphicOfCurrentSelectionArea()
        {
#if DEBUG
#endif
            if (_editSession.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }
        }

        internal void SwapCaretState()
        {
            //TODO: review here *** 
            if (_isEditable)
            {
                _stateShowCaret = !_stateShowCaret;
                //this.InvalidateGraphics();
                //_internalTextLayerController.CurrentLineArea;
                this.InvalidateGraphicOfCurrentLineArea();
            }

            //int swapcount = dbugCaretSwapCount++;
            //if (stateShowCaret)
            //{
            //    Console.WriteLine(">>on " + swapcount);
            //    this.InvalidateGraphics();
            //    Console.WriteLine("<<on " + swapcount);
            //}
            //else
            //{
            //    Console.WriteLine(">>off " + swapcount);
            //    this.InvalidateGraphics();
            //    Console.WriteLine("<<off " + swapcount);
            //} 
        }
        internal void SetCaretState(bool visible)
        {
            if (_isEditable)
            {
                _stateShowCaret = visible;
                this.InvalidateGraphicOfCurrentLineArea();
                //this.InvalidateGraphics();
            }
        }
        public void Focus()
        {
            if (_isEditable)
            {
                GlobalCaretController.CurrentTextEditBox = this;
                this.SetCaretState(true);
                _isFocus = true;
            }
        }
        public void Blur()
        {
            if (_isEditable)
            {
                GlobalCaretController.CurrentTextEditBox = null;
                this.SetCaretState(false);
                _isFocus = false;
            }
        }
        //
        public bool IsFocused => _isFocus;
        //
        public void HandleMouseDown(UIMouseEventArgs e)
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
        public void HandleMouseWheel(UIMouseEventArgs e)
        {
            if (_textSurfaceEventListener != null &&
                TextSurfaceEventListener.NotifyPreviewMouseWheel(_textSurfaceEventListener, e))
            {
                //if the event is handled by the listener
                return;
            }

            //
            if (e.Delta < 0)
            {
                //scroll down
                //this.StepSmallToMax();
                ScrollOffset(0, 24);
            }
            else
            {
                //up
                //this.StepSmallToMin();
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
                if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
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
                if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
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
            if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
            {
                _editSession.StartSelectIfNoSelection();
                _editSession.SetCaretPos(e.X, e.Y);
                _editSession.EndSelect();
                //this.InvalidateGraphics();
                InvalidateGraphicOfCurrentSelectionArea();
            }
        }

        Rectangle GetSelectionUpdateArea()
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
        public void HandleMouseUp(UIMouseEventArgs e)
        {
            //empty?
        }
        public void HandleKeyUp(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
        }
        public void HandleKeyDown(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
            if (!e.HasKeyData)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case UIKeys.Home:
                    {
                        this.DoHome(e.Shift);
                    }
                    break;
                case UIKeys.End:
                    {
                        this.DoEnd(e.Shift);
                    }
                    break;
                case UIKeys.Back:
                    {
                        if (_isEditable)
                        {
                            if (_editSession.SelectionRange != null)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                            if (_textSurfaceEventListener == null)
                            {
                                _editSession.DoBackspace();
                            }
                            else
                            {
                                if (!TextSurfaceEventListener.NotifyPreviewBackSpace(_textSurfaceEventListener, e) &&
                                    _editSession.DoBackspace())
                                {
                                    TextSurfaceEventListener.NotifyCharactersRemoved(_textSurfaceEventListener,
                                        new TextDomEventArgs(_editSession._updateJustCurrentLine));
                                }
                            }
                            EnsureCaretVisible();
                        }
                    }
                    break;
                case UIKeys.Delete:
                    {
                        if (_isEditable)
                        {
                            if (_editSession.SelectionRange != null)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                            if (_textSurfaceEventListener == null)
                            {
                                _editSession.DoDelete();
                            }
                            else
                            {
                                VisualSelectionRangeSnapShot delpart = _editSession.DoDelete();
                                TextSurfaceEventListener.NotifyCharactersRemoved(_textSurfaceEventListener,
                                    new TextDomEventArgs(_editSession._updateJustCurrentLine, delpart));
                            }

                            EnsureCaretVisible();
                        }
                    }
                    break;
                default:
                    {
                        if (_textSurfaceEventListener != null)
                        {
                            UIKeys keycode = e.KeyCode;
                            if (keycode >= UIKeys.F1 && keycode <= UIKeys.F12)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                TextSurfaceEventListener.NotifyFunctionKeyDown(_textSurfaceEventListener, keycode);
                                EnsureCaretVisible();
                            }
                        }

                    }
                    break;
            }

            if (e.HasKeyData && e.Ctrl)
            {
                switch (e.KeyCode)
                {
                    case UIKeys.A:
                        {
                            //select all
                            //....
                            this.CurrentLineNumber = 0;
                            //start select to end
                            DoHome(false);//1st simulate 
                            DoHome(true); //2nd
                            this.CurrentLineNumber = this.LineCount - 1;
                            DoEnd(true); //
                        }
                        break;
                    case UIKeys.C:
                        {
                            StringBuilder stBuilder = GetFreeStringBuilder();
                            _editSession.CopySelectedTextToPlainText(stBuilder);
                            if (stBuilder != null)
                            {
                                if (stBuilder.Length == 0)
                                {
                                    Clipboard.Clear();
                                }
                                else
                                {
                                    Clipboard.SetText(stBuilder.ToString());
                                }
                            }
                            ReleaseStringBuilder(stBuilder);
                        }
                        break;
                    case UIKeys.V:
                        {
                            if (_isEditable && Clipboard.ContainUnicodeText())
                            {
                                //1. we need to parse multi-line to single line
                                //this may need text-break services 

                                _editSession.AddTextToCurrentLine(PlainTextDocumentHelper.CreatePlainTextDocument(Clipboard.GetUnicodeText()));

                                EnsureCaretVisible();
                            }
                        }
                        break;
                    case UIKeys.X:
                        {
                            if (_isEditable && _editSession.SelectionRange != null)
                            {
                                if (_editSession.SelectionRange != null)
                                {
                                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                }
                                StringBuilder stBuilder = GetFreeStringBuilder();
                                _editSession.CopySelectedTextToPlainText(stBuilder);
                                if (stBuilder != null)
                                {
                                    Clipboard.SetText(stBuilder.ToString());
                                }

                                _editSession.DoDelete();
                                EnsureCaretVisible();
                                ReleaseStringBuilder(stBuilder);
                            }
                        }
                        break;
                    case UIKeys.Z:
                        {
                            if (_isEditable)
                            {
                                _editSession.UndoLastAction();
                                EnsureCaretVisible();
                            }
                        }
                        break;
                    case UIKeys.Y:
                        {
                            if (_isEditable)
                            {
                                _editSession.ReverseLastUndoAction();
                                EnsureCaretVisible();
                            }
                        }
                        break;
                }
            }

            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(_textSurfaceEventListener, e);
            }

        }
        //
        public Point CurrentCaretPos => _editSession.CaretPos;
        //
        public bool HandleProcessDialogKey(UIKeyEventArgs e)
        {
            UIKeys keyData = (UIKeys)e.KeyData;
            SetCaretState(true);
            if (_isInVerticalPhase && (keyData != UIKeys.Up || keyData != UIKeys.Down))
            {
                _isInVerticalPhase = false;
            }

            switch (e.KeyCode)
            {

                case UIKeys.Escape:
                case UIKeys.End:
                case UIKeys.Home:
                    {
                        if (_textSurfaceEventListener != null)
                        {
                            return TextSurfaceEventListener.NotifyPreviewDialogKeyDown(_textSurfaceEventListener, e);
                        }
                        return false;
                    }
                case UIKeys.Tab:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewDialogKeyDown(_textSurfaceEventListener, e))
                        {
                            return true;
                        }
                        //
                        DoTab(); //default do tab
                        return true;
                    }
                case UIKeys.Return:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewEnter(_textSurfaceEventListener, e))
                        {
                            return true;
                        }

                        if (_isEditable)
                        {
                            if (_isMultiLine)
                            {
                                if (_editSession.SelectionRange != null)
                                {
                                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                }

                                if (_editSession.SelectionRange != null)
                                {
                                    //this selection range will be remove first
                                }

                                int lineBeforeSplit = _editSession.CurrentLineNumber;
                                int lineCharBeforeSplit = _editSession.CurrentLineCharIndex;

                                _editSession.SplitCurrentLineIntoNewLine();

                                if (_textSurfaceEventListener != null)
                                {
                                    var splitEventE = new SplitToNewLineEventArgs();
                                    splitEventE.LineNumberBeforeSplit = lineBeforeSplit;
                                    splitEventE.LineCharIndexBeforeSplit = lineCharBeforeSplit;

                                    TextSurfaceEventListener.NofitySplitNewLine(_textSurfaceEventListener, splitEventE);
                                }

                                Rectangle lineArea = _editSession.CurrentLineArea;
                                if (lineArea.Bottom > this.ViewportBottom)
                                {
                                    ScrollOffset(0, lineArea.Bottom - this.ViewportBottom);
                                }
                                else
                                {
                                    InvalidateGraphicOfCurrentLineArea();
                                }
                                EnsureCaretVisible();
                            }
                            else
                            {
                                if (_textSurfaceEventListener != null)
                                {
                                    TextSurfaceEventListener.NotifyKeyDownOnSingleLineText(_textSurfaceEventListener, e);
                                }
                            }

                        }

                        return true;
                    }

                case UIKeys.Left:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, e))
                        {
                            return true;
                        }

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
                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, e.KeyCode);
                        }

                        return true;
                    }
                case UIKeys.Right:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, e))
                        {
                            return true;
                        }

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
                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                        }

                        return true;
                    }
                case UIKeys.PageUp:
                    {
                        //similar to arrow  up
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewDialogKeyDown(_textSurfaceEventListener, e))
                        {
                            return true;
                        }

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
                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                        }
                        return true;
                    }

                case UIKeys.PageDown:
                    {

                        //similar to arrow  down
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewDialogKeyDown(_textSurfaceEventListener, e))
                        {
                            return true;
                        }

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
                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                        }
                        return true;
                    }
                case UIKeys.Down:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, e))
                        {
                            return true;
                        }
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
                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                            if (!_isMultiLine)
                            {
                                TextSurfaceEventListener.NotifyKeyDownOnSingleLineText(_textSurfaceEventListener, e);
                            }
                        }
                        return true;
                    }
                case UIKeys.Up:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, e))
                        {
                            return true;
                        }

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


                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                            if (!_isMultiLine)
                            {
                                TextSurfaceEventListener.NotifyKeyDownOnSingleLineText(_textSurfaceEventListener, e);
                            }
                        }
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
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
        void EnsureCaretVisible()
        {
            Point textManCaretPos = _editSession.CaretPos;
            if (_isEditable)
            {
                _myCaret.SetHeight(_editSession.CurrentCaretHeight);
            }

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
        void RefreshSnapshotCanvas()
        {
        }
        //
        public bool OnlyCurrentlineUpdated => _editSession._updateJustCurrentLine;
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

        public void DoTab()
        {
            if (!_isEditable) return;
            //
            if (_editSession.SelectionRange != null)
            {
                VisualSelectionRange visualSelectionRange = _editSession.SelectionRange;
                visualSelectionRange.SwapIfUnOrder();
                if (visualSelectionRange.IsValid && !visualSelectionRange.IsOnTheSameLine)
                {
                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                    //
                    _editSession.DoTabOverSelectedRange();
                    return; //finish here
                }
            }


            //------------
            //do tab as usuall
            int insertAt = _editSession.CurrentLineCharIndex;

            for (int i = NumOfWhitespaceForSingleTab; i >= 0; --i)
            {
                _editSession.AddCharToCurrentLine(' ');
            }

            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyStringAdded(_textSurfaceEventListener,
                    insertAt, new string(' ', NumOfWhitespaceForSingleTab));
            }

            InvalidateGraphicOfCurrentLineArea();
        }
    }


    public class InternalDoubleBufferCustomRenderBox : RenderBoxBase
    {
        DrawboardBuffer _builtInBackBuffer;
        bool _hasAccumRect;
        Rectangle _invalidateRect;
        bool _enableDoubleBuffer;
        public InternalDoubleBufferCustomRenderBox(RootGraphic rootgfx, int width, int height)
          : base(rootgfx, width, height)
        {
            NeedInvalidateRectEvent = true;
        }

        public RenderBoxBase ContentBox { get; set; }

        public bool EnableDoubleBuffer
        {
            get => _enableDoubleBuffer;
            set
            {
                _enableDoubleBuffer = value;
            }
        }
        protected override PlainLayer CreateDefaultLayer()
        {
            return new PlainLayer(this);
        }

        protected override void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds)
        {
            if (_builtInBackBuffer != null)
            {
                //TODO: review here,
                //in this case, we copy to another rect
                //since we don't want the offset to effect the total bounds 
#if DEBUG
                if (totalBounds.Width == 150)
                {

                }
                System.Diagnostics.Debug.WriteLine("noti, fromMe=" + fromMe + ",bounds" + totalBounds);
#endif
                if (!fromMe)
                {
                    totalBounds.Offset(-this.X, -this.Y);
                }

                _builtInBackBuffer.IsValid = false;

                if (!_hasAccumRect)
                {
                    _invalidateRect = totalBounds;
                    _hasAccumRect = true;
                }
                else
                {
                    _invalidateRect = Rectangle.Union(_invalidateRect, totalBounds);
                }
            }
            else
            {
                totalBounds.Offset(this.X, this.Y);
            }
            //base.OnInvalidateGraphicsNoti(totalBounds);//skip
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            if (ContentBox == null) return;
            //
            if (_enableDoubleBuffer)
            {
                MicroPainter painter = new MicroPainter(canvas);
                if (_builtInBackBuffer == null)
                {
                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
                }

                if (!_builtInBackBuffer.IsValid)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
#endif
                    float backupViewportW = painter.ViewportWidth; //backup
                    float backupViewportH = painter.ViewportHeight; //backup
                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
                    painter.SetViewportSize(this.Width, this.Height);
                    if (!_hasAccumRect)
                    {
                        _invalidateRect = new Rectangle(0, 0, Width, Height);
                    }

#if DEBUG
                    if (_invalidateRect.Width == 15)
                    {

                    }
#endif

                    //                    if (painter.PushLocalClipArea(
                    //                        _invalidateRect.Left, _invalidateRect.Top,
                    //                        _invalidateRect.Width, _invalidateRect.Height))
                    //                    {
                    //#if DEBUG
                    //                        //for debug , test clear with random color
                    //                        //another useful technique to see latest clear area frame-by-frame => use random color
                    //                        //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

                    //                        canvas.Clear(Color.White);
                    //#else
                    //                        canvas.Clear(Color.White);
                    //#endif

                    //                        base.DrawBoxContent(canvas, updateArea);
                    //                    }

                    //if (painter.PushLocalClipArea(
                    //    _invalidateRect.Left, _invalidateRect.Top,
                    //    _invalidateRect.Width, _invalidateRect.Height))
                    //{
#if DEBUG
                    //for debug , test clear with random color
                    //another useful technique to see latest clear area frame-by-frame => use random color
                    //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

                    canvas.Clear(Color.White);
#else
                    canvas.Clear(Color.White);
#endif


                    Rectangle updateArea2 = new Rectangle(0, 0, _builtInBackBuffer.Width, _builtInBackBuffer.Height);
                    //ContentBox.DrawBoxContent(canvas, updateArea2);
                    ContentBox.DrawToThisCanvas(canvas, updateArea2);
                    //}
                    //painter.PopLocalClipArea();
                    //
                    _builtInBackBuffer.IsValid = true;
                    _hasAccumRect = false;

                    painter.AttachToNormalBuffer();//*** switch back
                    painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
                }

#if DEBUG
                else
                {
                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + dbug_obj_id + " use cache");
                }
#endif
                painter.DrawImage(_builtInBackBuffer.GetImage(), 0, 0, this.Width, this.Height);
            }
            else
            {
                ContentBox.DrawToThisCanvas(canvas, updateArea);
            }
        }

        //        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        //        {
        //            if (_enableDoubleBuffer)
        //            {
        //                MicroPainter painter = new MicroPainter(canvas);
        //                if (_builtInBackBuffer == null)
        //                {
        //                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
        //                }

        //                //                if (!_builtInBackBuffer.IsValid)
        //                //                {
        //                //#if DEBUG
        //                //                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
        //                //#endif
        //                //                    float backupViewportW = painter.ViewportWidth; //backup
        //                //                    float backupViewportH = painter.ViewportHeight; //backup
        //                //                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
        //                //                    painter.SetViewportSize(this.Width, this.Height);
        //                //                    if (!_hasAccumRect)
        //                //                    {
        //                //                        _invalidateRect = new Rectangle(0, 0, Width, Height);
        //                //                    }

        //                //#if DEBUG
        //                //                    if (_invalidateRect.Width == 15)
        //                //                    {

        //                //                    }
        //                //#endif

        //                //                    //                    if (painter.PushLocalClipArea(
        //                //                    //                        _invalidateRect.Left, _invalidateRect.Top,
        //                //                    //                        _invalidateRect.Width, _invalidateRect.Height))
        //                //                    //                    {
        //                //                    //#if DEBUG
        //                //                    //                        //for debug , test clear with random color
        //                //                    //                        //another useful technique to see latest clear area frame-by-frame => use random color
        //                //                    //                        //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

        //                //                    //                        canvas.Clear(Color.White);
        //                //                    //#else
        //                //                    //                        canvas.Clear(Color.White);
        //                //                    //#endif

        //                //                    //                        base.DrawBoxContent(canvas, updateArea);
        //                //                    //                    }

        //                //                    //if (painter.PushLocalClipArea(
        //                //                    //    _invalidateRect.Left, _invalidateRect.Top,
        //                //                    //    _invalidateRect.Width, _invalidateRect.Height))
        //                //                    //{
        //                //#if DEBUG
        //                //                    //for debug , test clear with random color
        //                //                    //another useful technique to see latest clear area frame-by-frame => use random color
        //                //                    //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

        //                //                    canvas.Clear(Color.White);
        //                //#else
        //                //                        canvas.Clear(Color.White);
        //                //#endif


        //                //                    Rectangle updateArea2 = new Rectangle(0, 0, _builtInBackBuffer.Width, _builtInBackBuffer.Height);
        //                //                    base.DrawBoxContent(canvas, updateArea2);

        //                //                    //}
        //                //                    //painter.PopLocalClipArea();
        //                //                    //
        //                //                    _builtInBackBuffer.IsValid = true;
        //                //                    _hasAccumRect = false;

        //                //                    painter.AttachToNormalBuffer();//*** switch back
        //                //                    painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
        //                //                }
        //                if (!_builtInBackBuffer.IsValid)
        //                {
        //#if DEBUG
        //                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
        //#endif
        //                    float backupViewportW = painter.ViewportWidth; //backup
        //                    float backupViewportH = painter.ViewportHeight; //backup
        //                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
        //                                                          // painter.SetViewportSize(this.Width, this.Height);
        //                    if (!_hasAccumRect)
        //                    {
        //                        _invalidateRect = new Rectangle(0, 0, Width, Height);
        //                    }
        //                    else
        //                    {

        //                    }

        //                    if (painter.PushLocalClipArea(
        //                        _invalidateRect.Left, _invalidateRect.Top,
        //                        _invalidateRect.Width, _invalidateRect.Height))
        //                    {
        //#if DEBUG
        //                        //for debug , test clear with random color
        //                        //another useful technique to see latest clear area frame-by-frame => use random color
        //                        //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

        //                        canvas.Clear(Color.White);
        //#else
        //                        painter.Clear(Color.White);
        //#endif

        //                        base.DrawBoxContent(canvas, updateArea);
        //                    }

        //                    painter.PopLocalClipArea();
        //                    //
        //                    _builtInBackBuffer.IsValid = true;
        //                    _hasAccumRect = false;

        //                    painter.AttachToNormalBuffer();//*** switch back
        //                    //painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
        //                }
        //#if DEBUG
        //                else
        //                {
        //                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + dbug_obj_id + " use cache");
        //                }
        //#endif
        //                if (painter.PushLocalClipArea(
        //                        _invalidateRect.Left, _invalidateRect.Top,
        //                        _invalidateRect.Width, _invalidateRect.Height))
        //                {
        //                    painter.DrawImage(_builtInBackBuffer.GetImage(), 0, 0, this.Width, this.Height);
        //                }
        //                painter.PopLocalClipArea();
        //            }
        //            else
        //            {
        //                base.DrawBoxContent(canvas, updateArea);
        //            }
        //        }
    }

}
