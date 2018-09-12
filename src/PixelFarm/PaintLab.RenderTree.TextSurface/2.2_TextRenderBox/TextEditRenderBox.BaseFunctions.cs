//Apache2, 2014-present, WinterDev

using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    public sealed partial class TextEditRenderBox : RenderBoxBase
    {
        CaretRenderElement _myCaret;

        EditableTextFlowLayer _textLayer; //this is a special layer that render text

        InternalTextLayerController _internalTextLayerController;

        int _verticalExpectedCharIndex;
        bool _isMultiLine = false;
        bool _isInVerticalPhase = false;
        bool _isFocus = false;
        bool _stateShowCaret = false;
        bool _isDragBegin;
        TextSpanStyle _currentSpanStyle;

        public TextEditRenderBox(RootGraphic rootgfx,
            int width, int height,
            bool isMultiLine)
            : base(rootgfx, width, height)
        {
            //1. TextEditRenderBox is created
            //2.  
            GlobalCaretController.RegisterCaretBlink(rootgfx);
            //
            _myCaret = new CaretRenderElement(rootgfx, 2, 17);
            _myCaret.TransparentForAllEvents = true;

            //
            this.MayHasViewport = true;
            this.BackgroundColor = Color.White;// Color.Transparent;
            this._currentSpanStyle = new TextSpanStyle();
            this._currentSpanStyle.ReqFont = rootgfx.DefaultTextEditFontInfo;

            //
            _textLayer = new EditableTextFlowLayer(this); //presentation
            _internalTextLayerController = new InternalTextLayerController(_textLayer);//controller

            this._isMultiLine = isMultiLine;
            if (isMultiLine)
            {
                _textLayer.SetUseDoubleCanvas(false, true);
            }
            else
            {
                _textLayer.SetUseDoubleCanvas(true, false);
            }
            this.NeedClipArea = true;
            this.IsBlockElement = false;
        }

        public InternalTextLayerController TextLayerController { get { return _internalTextLayerController; } }

        public TextSpanStyle CurrentTextSpanStyle
        {
            get { return this._currentSpanStyle; }
            set
            {
                this._currentSpanStyle = value;
            }
        }

        public bool HasSomeText
        {
            get
            {
                return (_textLayer.LineCount > 0) && _textLayer.GetTextLine(0).RunCount > 0;
            }
        }
        internal static void NotifyTextContentSizeChanged(TextEditRenderBox ts)
        {
            ts.BoxEvaluateScrollBar();
        }

        public void DoHome(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                _internalTextLayerController.DoHome();
                _internalTextLayerController.CancelSelect();
            }
            else
            {
                _internalTextLayerController.StartSelectIfNoSelection();
                _internalTextLayerController.DoHome();
                _internalTextLayerController.EndSelect();
            }

            EnsureCaretVisible();
        }
        public void DoEnd(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                _internalTextLayerController.DoEnd();
                _internalTextLayerController.CancelSelect();
            }
            else
            {
                _internalTextLayerController.StartSelectIfNoSelection();
                _internalTextLayerController.DoEnd();
                _internalTextLayerController.EndSelect();
            }

            EnsureCaretVisible();
        }


        public Rectangle GetRectAreaOf(int beginlineNum, int beginColumnNum, int endLineNum, int endColumnNum)
        {
            EditableTextFlowLayer flowLayer = this._textLayer;
            EditableTextLine beginLine = flowLayer.GetTextLineAtPos(beginlineNum);
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
                EditableTextLine endLine = flowLayer.GetTextLineAtPos(endLineNum);
                VisualPointInfo endPoint = endLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }
        }
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
            if (_internalTextLayerController.SelectionRange != null
                && _internalTextLayerController.SelectionRange.IsValid)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }
            bool preventDefault = false;
            if (_textSurfaceEventListener != null &&
                !(preventDefault = TextSurfaceEventListener.NotifyPreviewKeydown(_textSurfaceEventListener, c)))
            {
                _internalTextLayerController.UpdateSelectionRange();
            }
            if (preventDefault)
            {
                return;
            }
            if (_internalTextLayerController.SelectionRange != null)
            {
                _internalTextLayerController.AddCharToCurrentLine(c);
                if (_textSurfaceEventListener != null)
                {
                    TextSurfaceEventListener.NotifyCharactersReplaced(_textSurfaceEventListener, e.KeyChar);
                }
            }
            else
            {
                _internalTextLayerController.AddCharToCurrentLine(c);
                if (_textSurfaceEventListener != null)
                {
                    TextSurfaceEventListener.NotifyCharacterAdded(_textSurfaceEventListener, e.KeyChar);
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
            Rectangle c_lineArea = this._internalTextLayerController.CurrentParentLineArea;
#endif
            InvalidateGraphicLocalArea(this, this._internalTextLayerController.CurrentParentLineArea);
        }


        internal void SwapCaretState()
        {
            //TODO: review here ***

            this._stateShowCaret = !_stateShowCaret;
            this.InvalidateGraphics();
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
            this._stateShowCaret = visible;
            this.InvalidateGraphics();
        }
        public void Focus()
        {
            GlobalCaretController.CurrentTextEditBox = this;
            this.SetCaretState(true);
            this._isFocus = true;
        }
        public void Blur()
        {
            GlobalCaretController.CurrentTextEditBox = null;
            this.SetCaretState(false);
            this._isFocus = false;
        }
        public bool IsFocused
        {
            get
            {
                return this._isFocus;
            }
        }

        public void HandleMouseDown(UIMouseEventArgs e)
        {
            if (e.Button == UIMouseButtons.Left)
            {
                InvalidateGraphicOfCurrentLineArea();

                if (!e.Shift)
                {
                    _internalTextLayerController.SetCaretPos(e.X, e.Y);
                    if (_internalTextLayerController.SelectionRange != null)
                    {
                        Rectangle r = GetSelectionUpdateArea();
                        _internalTextLayerController.CancelSelect();
                        InvalidateGraphicLocalArea(this, r);
                    }
                    else
                    {
                        InvalidateGraphicOfCurrentLineArea();
                    }

                    if (_latestHitSolidTextRun != null)
                    {
                        //we mousedown on the solid text run
                        RenderElement extRenderElement = _latestHitSolidTextRun.ExternRenderElement;
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
                    _internalTextLayerController.StartSelectIfNoSelection();
                    _internalTextLayerController.SetCaretPos(e.X, e.Y);
                    _internalTextLayerController.EndSelect();
                    InvalidateGraphicOfCurrentLineArea();
                }
            }
        }
        public void HandleDoubleClick(UIMouseEventArgs e)
        {
            _internalTextLayerController.CancelSelect();
            EditableRun textRun = this.CurrentTextRun;
            if (textRun != null)
            {

                VisualPointInfo pointInfo = _internalTextLayerController.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                int local_sel_Index = pointInfo.RunLocalSelectedIndex;
                //default behaviour is select only a hit word under the caret
                //so ask the text layer to find a hit word
                int startAt, len;
                _internalTextLayerController.FindUnderlyingWord(out startAt, out len);
                if (len > 0)
                {
                    InvalidateGraphicOfCurrentLineArea();
                    _internalTextLayerController.TryMoveCaretTo(startAt, true);
                    _internalTextLayerController.StartSelect();
                    _internalTextLayerController.TryMoveCaretTo(startAt + len);
                    _internalTextLayerController.EndSelect();


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
            EditableRun textRun = this.CurrentTextRun;
            if (textRun != null)
            {

                VisualPointInfo pointInfo = _internalTextLayerController.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                int local_sel_Index = pointInfo.RunLocalSelectedIndex;
                //default behaviour is select only a hit word under the caret
                //so ask the text layer to find a hit word                 
                _internalTextLayerController.FindUnderlyingWord(out startAt, out len);
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
                    _internalTextLayerController.SetCaretPos(e.X, e.Y);
                    _internalTextLayerController.StartSelect();
                    _internalTextLayerController.EndSelect();
                    this.InvalidateGraphics();
                }
            }
            else
            {
                //dbugMouseDragging++;
                if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
                {
                    _internalTextLayerController.StartSelectIfNoSelection();
                    _internalTextLayerController.SetCaretPos(e.X, e.Y);
                    _internalTextLayerController.EndSelect();
                    this.InvalidateGraphics();
                }
            }
        }
        public void HandleDragEnd(UIMouseEventArgs e)
        {
            _isDragBegin = false;
            if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
            {
                _internalTextLayerController.StartSelectIfNoSelection();
                _internalTextLayerController.SetCaretPos(e.X, e.Y);
                _internalTextLayerController.EndSelect();
                this.InvalidateGraphics();
            }
        }

        Rectangle GetSelectionUpdateArea()
        {
            VisualSelectionRange selectionRange = _internalTextLayerController.SelectionRange;
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
                        if (_internalTextLayerController.SelectionRange != null)
                        {
                            InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                        }
                        else
                        {
                            InvalidateGraphicOfCurrentLineArea();
                        }
                        if (_textSurfaceEventListener == null)
                        {
                            _internalTextLayerController.DoBackspace();
                        }
                        else
                        {
                            if (!TextSurfaceEventListener.NotifyPreviewBackSpace(_textSurfaceEventListener) &&
                                _internalTextLayerController.DoBackspace())
                            {
                                TextSurfaceEventListener.NotifyCharactersRemoved(_textSurfaceEventListener,
                                    new TextDomEventArgs(_internalTextLayerController._updateJustCurrentLine));
                            }
                        }

                        EnsureCaretVisible();
                    }
                    break;
                case UIKeys.Delete:
                    {
                        if (_internalTextLayerController.SelectionRange != null)
                        {
                            InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                        }
                        else
                        {
                            InvalidateGraphicOfCurrentLineArea();
                        }
                        if (_textSurfaceEventListener == null)
                        {
                            _internalTextLayerController.DoDelete();
                        }
                        else
                        {
                            VisualSelectionRangeSnapShot delpart = _internalTextLayerController.DoDelete();
                            TextSurfaceEventListener.NotifyCharactersRemoved(_textSurfaceEventListener,
                                new TextDomEventArgs(_internalTextLayerController._updateJustCurrentLine, delpart));
                        }

                        EnsureCaretVisible();
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
                            _internalTextLayerController.CopySelectedTextToPlainText(stBuilder);
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
                            if (Clipboard.ContainUnicodeText())
                            {
                                //1. we need to parse multi-line to single line
                                //this may need text-break services

                                _internalTextLayerController.AddUnformattedStringToCurrentLine(
                                    this.Root,
                                    Clipboard.GetUnicodeText(),
                                    this._currentSpanStyle);

                                EnsureCaretVisible();
                            }
                        }
                        break;
                    case UIKeys.X:
                        {
                            if (_internalTextLayerController.SelectionRange != null)
                            {
                                if (_internalTextLayerController.SelectionRange != null)
                                {
                                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                }
                                StringBuilder stBuilder = GetFreeStringBuilder();
                                _internalTextLayerController.CopySelectedTextToPlainText(stBuilder);
                                if (stBuilder != null)
                                {
                                    Clipboard.SetText(stBuilder.ToString());
                                }

                                _internalTextLayerController.DoDelete();
                                EnsureCaretVisible();
                                ReleaseStringBuilder(stBuilder);
                            }
                        }
                        break;
                    case UIKeys.Z:
                        {
                            _internalTextLayerController.UndoLastAction();
                            EnsureCaretVisible();
                        }
                        break;
                    case UIKeys.Y:
                        {
                            _internalTextLayerController.ReverseLastUndoAction();
                            EnsureCaretVisible();
                        }
                        break;
                }
            }

            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(_textSurfaceEventListener, e);
            }
        }
        public Point CurrentCaretPos
        {
            get { return this._internalTextLayerController.CaretPos; }
        }

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
                    if (_textSurfaceEventListener != null)
                    {
                        return TextSurfaceEventListener.NotifyPreviewEsc(_textSurfaceEventListener);
                    }
                    return false;
                case UIKeys.Home:
                    {
                        HandleKeyDown(e);
                        return true;
                    }
                case UIKeys.Return:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewEnter(_textSurfaceEventListener))
                        {
                            return true;
                        }
                        if (_isMultiLine)
                        {
                            if (_internalTextLayerController.SelectionRange != null)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                            }
                            _internalTextLayerController.SplitCurrentLineIntoNewLine();
                            if (_textSurfaceEventListener != null)
                            {
                                TextSurfaceEventListener.NofitySplitNewLine(_textSurfaceEventListener, e);
                            }

                            Rectangle lineArea = _internalTextLayerController.CurrentLineArea;
                            if (lineArea.Bottom > this.ViewportBottom)
                            {
                                ScrollBy(0, lineArea.Bottom - this.ViewportBottom);
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                            EnsureCaretVisible();
                            return true;
                        }
                        return true;
                    }

                case UIKeys.Left:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, keyData))
                        {
                            return true;
                        }

                        InvalidateGraphicOfCurrentLineArea();
                        if (!e.Shift)
                        {
                            _internalTextLayerController.CancelSelect();
                        }
                        else
                        {
                            _internalTextLayerController.StartSelectIfNoSelection();
                        }

                        Point currentCaretPos = Point.Empty;
                        if (!_isMultiLine)
                        {
                            if (!_internalTextLayerController.IsOnStartOfLine)
                            {
#if DEBUG
                                Point prvCaretPos = _internalTextLayerController.CaretPos;
#endif
                                _internalTextLayerController.TryMoveCaretBackward();
                                currentCaretPos = _internalTextLayerController.CaretPos;
                            }
                        }
                        else
                        {
                            if (_internalTextLayerController.IsOnStartOfLine)
                            {
                                _internalTextLayerController.TryMoveCaretBackward();
                                currentCaretPos = _internalTextLayerController.CaretPos;
                            }
                            else
                            {
                                if (!_internalTextLayerController.IsOnStartOfLine)
                                {
#if DEBUG
                                    Point prvCaretPos = _internalTextLayerController.CaretPos;
#endif
                                    _internalTextLayerController.TryMoveCaretBackward();
                                    currentCaretPos = _internalTextLayerController.CaretPos;
                                }
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _internalTextLayerController.EndSelectIfNoSelection();
                        }
                        //-------------------

                        EnsureCaretVisible();
                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                        }

                        return true;
                    }
                case UIKeys.Right:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, keyData))
                        {
                            return true;
                        }

                        InvalidateGraphicOfCurrentLineArea();
                        if (!e.Shift)
                        {
                            _internalTextLayerController.CancelSelect();
                        }
                        else
                        {
                            _internalTextLayerController.StartSelectIfNoSelection();
                        }


                        Point currentCaretPos = Point.Empty;
                        if (!_isMultiLine)
                        {
#if DEBUG
                            Point prvCaretPos = _internalTextLayerController.CaretPos;
#endif
                            _internalTextLayerController.TryMoveCaretForward();
                            currentCaretPos = _internalTextLayerController.CaretPos;
                        }
                        else
                        {
                            if (_internalTextLayerController.IsOnEndOfLine)
                            {
                                _internalTextLayerController.TryMoveCaretForward();
                                currentCaretPos = _internalTextLayerController.CaretPos;
                            }
                            else
                            {
#if DEBUG
                                Point prvCaretPos = _internalTextLayerController.CaretPos;
#endif
                                _internalTextLayerController.TryMoveCaretForward();
                                currentCaretPos = _internalTextLayerController.CaretPos;
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _internalTextLayerController.EndSelectIfNoSelection();
                        }
                        //-------------------

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
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, keyData))
                        {
                            return true;
                        }
                        if (_isMultiLine)
                        {
                            if (!_isInVerticalPhase)
                            {
                                _isInVerticalPhase = true;
                                _verticalExpectedCharIndex = _internalTextLayerController.CharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _internalTextLayerController.CancelSelect();
                            }
                            else
                            {
                                _internalTextLayerController.StartSelectIfNoSelection();
                            }
                            //---------------------------- 

                            _internalTextLayerController.CurrentLineNumber++;
                            if (_verticalExpectedCharIndex > _internalTextLayerController.CurrentLineCharCount - 1)
                            {
                                _internalTextLayerController.TryMoveCaretTo(_internalTextLayerController.CurrentLineCharCount - 1);
                            }
                            else
                            {
                                _internalTextLayerController.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }
                            //----------------------------

                            if (e.Shift)
                            {
                                _internalTextLayerController.EndSelectIfNoSelection();
                            }
                            //----------------------------
                            Rectangle lineArea = _internalTextLayerController.CurrentLineArea;
                            if (lineArea.Bottom > this.ViewportBottom)
                            {
                                ScrollBy(0, lineArea.Bottom - this.ViewportBottom);
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                        }

                        if (_textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(_textSurfaceEventListener, keyData);
                        }
                        return true;
                    }
                case UIKeys.Up:
                    {
                        if (_textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(_textSurfaceEventListener, keyData))
                        {
                            return true;
                        }

                        if (_isMultiLine)
                        {
                            if (!_isInVerticalPhase)
                            {
                                _isInVerticalPhase = true;
                                _verticalExpectedCharIndex = _internalTextLayerController.CharIndex;
                            }

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                _internalTextLayerController.CancelSelect();
                            }
                            else
                            {
                                _internalTextLayerController.StartSelectIfNoSelection();
                            }
                            //----------------------------

                            _internalTextLayerController.CurrentLineNumber--;
                            if (_verticalExpectedCharIndex > _internalTextLayerController.CurrentLineCharCount - 1)
                            {
                                _internalTextLayerController.TryMoveCaretTo(_internalTextLayerController.CurrentLineCharCount);
                            }
                            else
                            {
                                _internalTextLayerController.TryMoveCaretTo(_verticalExpectedCharIndex);
                            }

                            //----------------------------
                            if (e.Shift)
                            {
                                _internalTextLayerController.EndSelectIfNoSelection();
                            }

                            Rectangle lineArea = _internalTextLayerController.CurrentLineArea;
                            if (lineArea.Top < ViewportY)
                            {
                                ScrollBy(0, lineArea.Top - ViewportY);
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
                case UIKeys.Tab:
                    {
                        DoTab();
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
                return _internalTextLayerController.CurrentLineArea.Size;
            }
        }
        void EnsureCaretVisible()
        {
            //----------------------
            Point textManCaretPos = _internalTextLayerController.CaretPos;
            _myCaret.SetHeight(_internalTextLayerController.CurrentCaretHeight);
            textManCaretPos.Offset(-ViewportX, -ViewportY);
            //----------------------  
            if (textManCaretPos.X >= this.Width)
            {
                if (!_isMultiLine)
                {
                    var r = _internalTextLayerController.CurrentLineArea;
                    //Rectangle r = internalTextLayerController.CurrentParentLineArea;
                    if (r.Width >= this.Width)
                    {
#if DEBUG
                        dbug_SetInitObject(this);
                        dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.ArtVisualTextSurafce_EnsureCaretVisible);
#endif
                        //SetCalculatedSize(this, r.Width, r.Height);
                        //InnerDoTopDownReCalculateContentSize(this);
                        this.BoxEvaluateScrollBar();
                        RefreshSnapshotCanvas();
#if DEBUG
                        dbug_EndLayoutTrace();
#endif
                    }
                }
                else
                {
                }

                ScrollBy(textManCaretPos.X - this.Width, 0);
            }
            else if (textManCaretPos.X < 0)
            {
                ScrollBy(textManCaretPos.X - this.X, 0);
            }

            Size innerContentSize = this.InnerContentSize;
            if (ViewportX > 0 && innerContentSize.Width - ViewportX < this.Width)
            {
                ScrollTo(this.InnerContentSize.Width - ViewportX, 0);
            }


            if (_internalTextLayerController._updateJustCurrentLine)
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
        public bool OnlyCurrentlineUpdated
        {
            get
            {
                return _internalTextLayerController._updateJustCurrentLine;
            }
        }
        public int CurrentLineHeight
        {
            get
            {
                return _internalTextLayerController.CurrentLineArea.Height;
            }
        }
        public int CurrentLineCharIndex
        {
            get
            {
                return _internalTextLayerController.CurrentLineCharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return _internalTextLayerController.CurrentTextRunCharIndex;
            }
        }
        public int CurrentLineNumber
        {
            get
            {
                return _internalTextLayerController.CurrentLineNumber;
            }
            set
            {
                _internalTextLayerController.CurrentLineNumber = value;
            }
        }
        public void ScrollToCurrentLine()
        {
            this.ScrollTo(0, _internalTextLayerController.CaretPos.Y);
        }

        public void DoTab()
        {
            if (_internalTextLayerController.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }

            _internalTextLayerController.AddCharToCurrentLine(' ');
            _internalTextLayerController.AddCharToCurrentLine(' ');
            _internalTextLayerController.AddCharToCurrentLine(' ');
            _internalTextLayerController.AddCharToCurrentLine(' ');
            _internalTextLayerController.AddCharToCurrentLine(' ');
            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyCharacterAdded(_textSurfaceEventListener, '\t');
            }

            InvalidateGraphicOfCurrentLineArea();
        }

        public void DoTyping(string text)
        {
            if (_internalTextLayerController.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }

            char[] charBuff = text.ToCharArray();
            int j = charBuff.Length;
            for (int i = 0; i < j; ++i)
            {
                _internalTextLayerController.AddCharToCurrentLine(charBuff[i]);
            }
            InvalidateGraphicOfCurrentLineArea();
        }
    }
}
