//Apache2, 2014-present, WinterDev

using System.Text;

using LayoutFarm.UI;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{

    public sealed class TextEditRenderBox : TextFlowRenderBox
    {
        EditorCaret _myCaret; //just for render, BUT this render element is not added to parent tree***
        bool _isEditable;
        bool _stateShowCaret = false;

        public TextEditRenderBox(
            int width, int height,
            bool isMultiLine,
            bool isEditable = true)
            : base(width, height, isMultiLine)
        {
            _isEditable = isEditable;

            if (isEditable)
            {
                GlobalCaretController.RegisterCaretBlink(GlobalRootGraphic.CurrentRootGfx);
                //
                _myCaret = new EditorCaret(2, 17);
                RenderCaret = true;
            }

            NumOfWhitespaceForSingleTab = 4;//default?, configurable?
        }

        public bool RenderCaret { get; set; }


        public override void HandleDrag(UIMouseMoveEventArgs e)
        {
            SetCaretVisible(true);
            GetRoot().CaretStopBlink();
            base.HandleDrag(e);
        }
        public override void HandleDragEnd(UIMouseUpEventArgs e)
        {
            SetCaretVisible(true);
            GetRoot().CaretStopBlink();
            base.HandleDragEnd(e);
        }
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {

            base.RenderClientContent(d, updateArea);
            //4. caret            

            if (RenderCaret && _stateShowCaret && _isEditable)
            {
                d.SetClipRect(new Rectangle(0, 0, this.Width, this.Height));
                Point pos = _editSession.CaretPos;
                _myCaret.DrawCaret(d, pos.X, pos.Y);
            }
        }

        public override void DoHome(bool pressShitKey)
        {
            base.DoHome(pressShitKey);
            EnsureCaretVisible();
        }
        public override void DoEnd(bool pressShitKey)
        {
            base.DoEnd(pressShitKey);
            EnsureCaretVisible();
        }

        public override void Focus()
        {
            if (_isEditable)
            {
                GlobalCaretController.CurrentTextEditBox = this;
                this.SetCaretVisible(true);
                _isFocus = true;
            }
        }

        bool _blurring;
        public override void Blur()
        {
            if (_isEditable && !_blurring)
            {
                _blurring = true;
                GlobalCaretController.CurrentTextEditBox = null;
                this.SetCaretVisible(false);
                _isFocus = false;
                _blurring = false;
            }
        }


        public override void HandleKeyPress(UIKeyEventArgs e)
        {
            this.SetCaretVisible(true);
            //------------------------
            if (e.IsControlCharacter)
            {
                HandleKeyDown(e);
                return;
            }



            e.CancelBubbling = true;

            InvalidateGraphicOfCurrentSelectionArea();

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

                _editSession.AddCharToCurrentLine(e.KeyChar);

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

        }
        internal void SetCaretVisible(bool visible)
        {
            if (_isEditable)
            {
                _stateShowCaret = visible;
                this.InvalidateGraphicOfCurrentLineArea();
            }
        }

        public override void HandleKeyUp(UIKeyEventArgs e)
        {
            this.SetCaretVisible(true);
            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(_textSurfaceEventListener, e); ;
            }
        }
        public override void HandleKeyDown(UIKeyEventArgs e)
        {
            this.SetCaretVisible(true);
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
                                InvalidateGraphicOfCurrentSelectionArea();
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
                                InvalidateGraphicOfCurrentSelectionArea();
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
                                InvalidateGraphicOfCurrentSelectionArea();
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
                            using (new StringBuilderPoolContext<TextEditRenderBox>(out StringBuilder stBuilder))
                            {
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
                            }

                        }
                        break;
                    case UIKeys.V:
                        {
                            if (_isEditable && Clipboard.ContainsUnicodeText())
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
                                InvalidateGraphicOfCurrentSelectionArea();

                                using (new StringBuilderPoolContext<TextEditRenderBox>(out StringBuilder stBuilder))
                                {
                                    _editSession.CopySelectedTextToPlainText(stBuilder);
                                    if (stBuilder != null)
                                    {
                                        Clipboard.SetText(stBuilder.ToString());
                                    }
                                    _editSession.DoDelete();
                                    EnsureCaretVisible();
                                }
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


        public override bool HandleProcessDialogKey(UIKeyEventArgs e)
        {
            UIKeys keyData = (UIKeys)e.KeyData;
            SetCaretVisible(true);
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
                                    InvalidateGraphicOfCurrentSelectionArea();

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

        public override void HandleMouseWheel(UIMouseWheelEventArgs e)
        {
            if (_textSurfaceEventListener != null &&
               TextSurfaceEventListener.NotifyPreviewMouseWheel(_textSurfaceEventListener, e))
            {
                //if the event is handled by the listener
                return;
            }

            base.HandleMouseWheel(e);
        }

        protected override void EnsureCaretVisible()
        {
            base.EnsureCaretVisible();

            if (_isEditable)
            {
                _myCaret.SetHeight(_editSession.CurrentCaretHeight);
            }
        }


        public void DoTab()
        {
            if (!_isEditable) return;
            //
            if (_editSession.SelectionRange != null)
            {
                VisualSelectionRange visualSelectionRange = _editSession.SelectionRange;
                visualSelectionRange.Normalize();
                if (visualSelectionRange.IsValid && !visualSelectionRange.IsOnTheSameLine)
                {
                    InvalidateGraphicOfCurrentSelectionArea();
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


        TextSurfaceEventListener _textSurfaceEventListener;
        public TextSurfaceEventListener TextSurfaceListener
        {
            get => _textSurfaceEventListener;
            set
            {
                _textSurfaceEventListener = value;
                if (value != null)
                {
                    _textSurfaceEventListener.SetMonitoringTextSurface(this);
                }
            }
        }


    }



}
