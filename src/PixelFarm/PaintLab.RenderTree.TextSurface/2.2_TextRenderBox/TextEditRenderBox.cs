//Apache2, 2014-present, WinterDev

using System.Text;

using LayoutFarm.UI;
using PixelFarm.Drawing;
namespace LayoutFarm.TextFlow
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
                Point pos = _visualEditSession.CaretPos;
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
                _visualEditSession.UpdateSelectionRange();
            }
            if (preventDefault)
            {
                return;
            }

            if (_isEditable)
            {
                int insertAt = _visualEditSession.CurrentLineNewCharIndex;

                _visualEditSession.AddChar(e.KeyChar);

                if (_textSurfaceEventListener != null)
                {
                    //TODO: review this again ***
                    if (_visualEditSession.SelectionRange != null)
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
                            if (_visualEditSession.SelectionRange != null)
                            {
                                InvalidateGraphicOfCurrentSelectionArea();
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                            if (_textSurfaceEventListener == null)
                            {
                                _visualEditSession.DoBackspace();
                            }
                            else
                            {
                                if (!TextSurfaceEventListener.NotifyPreviewBackSpace(_textSurfaceEventListener, e) &&
                                    _visualEditSession.DoBackspace())
                                {
                                    TextSurfaceEventListener.NotifyCharactersRemoved(_textSurfaceEventListener,
                                        new TextDomEventArgs(_visualEditSession._updateJustCurrentLine));
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
                            if (_visualEditSession.SelectionRange != null)
                            {
                                InvalidateGraphicOfCurrentSelectionArea();
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                            if (_textSurfaceEventListener == null)
                            {
                                _visualEditSession.DoDelete();
                            }
                            else
                            {

                                _visualEditSession.DoDelete();

                                SelectionRangeSnapShot delpart = new SelectionRangeSnapShot();
                                TextSurfaceEventListener.NotifyCharactersRemoved(_textSurfaceEventListener,
                                           new TextDomEventArgs(_visualEditSession._updateJustCurrentLine, delpart));
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
                    case UIKeys.V:
                        {
                            if (_isEditable && Clipboard.ContainsUnicodeText())
                            {
                                //1. we need to parse multi-line to single line
                                //this may need text-break services                                  
                                _visualEditSession.AddText(Clipboard.GetUnicodeText());
                                EnsureCaretVisible();
                            }
                        }
                        break;
                    case UIKeys.C:
                        {
                            using (new StringBuilderPoolContext<TextEditRenderBox>(out StringBuilder sb))
                            {
                                _visualEditSession.CopySelectedTextToPlainText(sb);
                                //copy to plain text
                                if (sb.Length == 0)
                                {
                                    Clipboard.Clear();
                                }
                                else
                                {
                                    Clipboard.SetText(sb.ToString());
                                }
                            }
                        }
                        break;
                    case UIKeys.X:
                        {
                            if (_isEditable && _visualEditSession.SelectionRange != null)
                            {
                                InvalidateGraphicOfCurrentSelectionArea();

                                using (new StringBuilderPoolContext<TextEditRenderBox>(out StringBuilder sb))
                                {
                                    _visualEditSession.CopySelectedTextToPlainText(sb);

                                    if (sb.Length == 0)
                                    {
                                        Clipboard.Clear();
                                    }
                                    else
                                    {
                                        Clipboard.SetText(sb.ToString());
                                    }

                                    _visualEditSession.DoDelete();
                                    EnsureCaretVisible();
                                }
                            }
                        }
                        break;
                    case UIKeys.Z:
                        {
                            if (_isEditable)
                            {
                                _visualEditSession.UndoLastAction();
                                EnsureCaretVisible();
                            }
                        }
                        break;
                    case UIKeys.Y:
                        {
                            if (_isEditable)
                            {
                                _visualEditSession.ReverseLastUndoAction();
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
                default: return false;
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
                                if (_visualEditSession.SelectionRange != null)
                                {
                                    InvalidateGraphicOfCurrentSelectionArea();

                                }

                                if (_visualEditSession.SelectionRange != null)
                                {
                                    //this selection range will be remove first
                                }

                                int lineBeforeSplit = _visualEditSession.CurrentLineNumber;
                                int lineCharBeforeSplit = _visualEditSession.CurrentLineNewCharIndex;

                                _visualEditSession.SplitIntoNewLine();

                                if (_textSurfaceEventListener != null)
                                {
                                    var splitEventE = new SplitToNewLineEventArgs();
                                    splitEventE.LineNumberBeforeSplit = lineBeforeSplit;
                                    splitEventE.LineCharIndexBeforeSplit = lineCharBeforeSplit;

                                    TextSurfaceEventListener.NofitySplitNewLine(_textSurfaceEventListener, splitEventE);
                                }

                                Rectangle lineArea = _visualEditSession.CurrentLineArea;
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
                            _visualEditSession.CancelSelect();
                        }
                        else
                        {
                            _visualEditSession.StartSelectIfNoSelection();
                        }


                        if (!_isMultiLine)
                        {
                            if (!_visualEditSession.IsOnStartOfLine)
                            {
#if DEBUG
                                Point prvCaretPos = _visualEditSession.CaretPos;
#endif
                                _visualEditSession.TryMoveCaretBackward();

                            }
                        }
                        else
                        {
                            _visualEditSession.TryMoveCaretBackward();

                        }
                        //-------------------
                        if (e.Shift)
                        {
                            _visualEditSession.EndSelectIfNoSelection();
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
                _myCaret.SetHeight(_visualEditSession.CurrentCaretHeight);
            }
        }


        public void DoTab()
        {
            if (!_isEditable) return;
            //
            if (_visualEditSession.SelectionRange != null)
            {
                VisualSelectionRange visualSelectionRange = _visualEditSession.SelectionRange;
                visualSelectionRange.Normalize();
                if (visualSelectionRange.IsValid && !visualSelectionRange.IsOnTheSameLine)
                {
                    InvalidateGraphicOfCurrentSelectionArea();
                    //
                    _visualEditSession.DoTabOverSelectedRange();
                    return; //finish here
                }
            }
            //------------
            //do tab as usuall
            int insertAt = _visualEditSession.CurrentLineNewCharIndex;

            for (int i = NumOfWhitespaceForSingleTab; i >= 0; --i)
            {
                _visualEditSession.AddChar(' ');
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
