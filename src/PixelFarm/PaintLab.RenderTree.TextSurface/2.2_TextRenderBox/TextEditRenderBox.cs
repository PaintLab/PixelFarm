//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
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
            RootGraphic rootgfx,
            int width, int height,
            bool isMultiLine,
            bool isEditable = true)
            : base(rootgfx, width, height, isMultiLine)
        {
            _isEditable = isEditable;

            if (isEditable)
            {
                GlobalCaretController.RegisterCaretBlink(rootgfx);
                //
                _myCaret = new EditorCaret(2, 17);
                RenderCaret = true;
            }

            ////----------- 
            ///
            //
            //if (isMultiLine)
            //{
            //    _textLayer.SetUseDoubleCanvas(false, true);
            //}
            //else
            //{
            //    _textLayer.SetUseDoubleCanvas(true, false);
            //} 

            NumOfWhitespaceForSingleTab = 4;//default?, configurable?
        }

        public bool RenderCaret { get; set; }

        protected override void RenderClientContent(DrawBoard d, Rectangle updateArea)
        {
            RequestFont enterFont = d.CurrentFont;
            d.CurrentFont = this.CurrentTextSpanStyle.ReqFont;
            //1. bg 
            if (RenderBackground && BackgroundColor.A > 0)
            {
                Size innerBgSize = InnerBackgroundSize;

#if DEBUG
                d.FillRectangle(BackgroundColor, 0, 0, innerBgSize.Width, innerBgSize.Height);
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
                    marker.Draw(d, updateArea);
                }
            }


            //----------------------------------------------
            //2.2 selection
            if (RenderSelectionRange && _editSession.SelectionRange != null)
            {
                _editSession.SelectionRange.Draw(d, updateArea);
            }
            //3 actual editable layer

            GlobalRootGraphic.CurrentRenderElement = this; //temp fix
            _textLayer.DrawChildContent(d, updateArea);
            GlobalRootGraphic.CurrentRenderElement = null; //temp fix

            if (this.HasDefaultLayer)
            {
                this.DrawDefaultLayer(d, ref updateArea);
            } 

#if DEBUG
            //for debug
            //canvas.FillRectangle(Color.Red, 0, 0, 5, 5);

#endif
            //4. caret 
            if (RenderCaret && _stateShowCaret && _isEditable)
            {
                Point textManCaretPos = _editSession.CaretPos;
                _myCaret.DrawCaret(d, textManCaretPos.X, textManCaretPos.Y);
            }

            d.CurrentFont = enterFont;
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
                this.SetCaretState(true);
                _isFocus = true;
            }
        }
        public override void Blur()
        {
            if (_isEditable)
            {
                GlobalCaretController.CurrentTextEditBox = null;
                this.SetCaretState(false);
                _isFocus = false;
            }
        }


        public override void HandleKeyPress(UIKeyEventArgs e)
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
            }
        }

        public override void HandleKeyUp(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
            if (_textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(_textSurfaceEventListener, e); ;
            }
        }
        public override void HandleKeyDown(UIKeyEventArgs e)
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
                            using (StringBuilderPool<TempTextLineCopyContext>.GetFreeStringBuilder(out StringBuilder stBuilder))
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

                                using (StringBuilderPool<TempTextLineCopyContext>.GetFreeStringBuilder(out StringBuilder stBuilder))
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

        public override void HandleMouseWheel(UIMouseEventArgs e)
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
