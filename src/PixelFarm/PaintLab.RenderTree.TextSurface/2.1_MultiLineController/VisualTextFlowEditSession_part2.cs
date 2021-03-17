//MIT, 2019-present, WinterDev

using System;
using PixelFarm.Drawing;

namespace LayoutFarm.TextFlow
{
    partial class VisualTextFlowEditSession : ITextFlowSelectSession
    {

#if DEBUG
        //internal debugActivityRecorder _dbugActivityRecorder;
        //internal bool dbugEnableTextManRecorder = false;
#endif

        protected VisualSelectionRange _selectionRange;//primary visual selection
        public event EventHandler SelectionCanceled;
        public Run CurrentTextRun => _lineWalker.GetCurrentTextRun();

        public void SelectAll()
        {
            //model
            CurrentLineNumber = 0;
            SetCaretPos(0, 0);
            StartSelect();
            CurrentLineNumber = LineCount - 1;//move to last line
            _lineWalker.SetCurrentCharIndexToEnd();//move to end 
            EndSelect();
        }

        public void StartSelect()
        {
            //active selection
            EditableVisualPointInfo currentPoint = GetCurrentPointInfo();
            if (_selectionRange == null)
            {
                _selectionRange = new VisualSelectionRange(_textLayer, currentPoint, currentPoint);
            }
            else
            {
                _selectionRange.StartPoint = _selectionRange.EndPoint = currentPoint;
            }

        }
        public void EndSelect()
        {
            if (_selectionRange != null)
            {
                Rectangle before = _selectionRange.GetSelectionUpdateArea();
                _selectionRange.EndPoint = GetCurrentPointInfo();
                Rectangle after = _selectionRange.GetSelectionUpdateArea();
                _textLayer.ClientLineBubbleupInvalidateArea(Rectangle.Union(before, after));
            }
        }

        public void CancelSelect()
        {

            if (_selectionRange != null)
            {
                //invalidate graphic area?
                _textLayer.ClientLineBubbleupInvalidateArea(_selectionRange.GetSelectionUpdateArea());
            }

            _selectionRange = null;
            _pte.CancelSelect();
            _hx.CancelSelection();
            SelectionCanceled?.Invoke(this, EventArgs.Empty);
        }
        public void StartSelectIfNoSelection()
        {
            if (_selectionRange == null)
            {
                this.StartSelect();
            }
        }
        public void EndSelectIfNoSelection()
        {
            if (_selectionRange == null)
            {
                this.StartSelect();
            }
            this.EndSelect();
        }

        void MoveToNextLine() => MoveToLine(_lineWalker.LineNumber + 1);

        void MoveToPrevLine() => MoveToLine(_lineWalker.LineNumber - 1);

        //
        public int CurrentLineCharCount => _lineWalker.CharCount;
        //
        public int LineCount => _textLayer.LineCount;

        /// <summary>
        /// index of new char on the current line
        /// </summary>
        public int CurrentLineNewCharIndex => _lineWalker.NewCharIndex;
        // 
        public int CurrentLineNumber
        {
            get => _lineWalker.LineNumber;
            set
            {
                int diff = value - _lineWalker.LineNumber;
                switch (diff)
                {
                    case 0:
                        {
                            //same line
                            return;
                        }
                    case 1:
                        {
                            if (_lineWalker.HasNextLine)
                            {
                                MoveToNextLine();
                                DoHome();
                            }
                        }
                        break;
                    case -1:
                        {
                            if (_lineWalker.HasPrevLine)
                            {
                                MoveToPrevLine();
                                DoEnd();
                            }
                        }
                        break;
                    default:
                        {
                            //TODO review here
                            if (diff > 1)
                            {
                                MoveToLine(value);
                            }
                            else
                            {
                                if (value < -1)
                                {
                                    MoveToLine(value);
                                    //_walker.MoveToLine(value);
                                }
                                else
                                {
                                    MoveToLine(value);
                                    //_walker.MoveToLine(value);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public VisualSelectionRange SelectionRange => _selectionRange;

        public void UpdateSelectionRange()
        {
            if (_selectionRange != null)
            {
                _selectionRange.UpdateSelectionRange();
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo() => _lineWalker.GetCurrentPointInfo();

        /// <summary>
        /// find underlying word at current caret pos
        /// </summary>
        public void FindUnderlyingWord(out int startAt, out int len)
        {
            _lineWalker.FindCurrentHitWord(out startAt, out len);
        }
        void MoveToLine(int lineNumber)
        {
            //move to next visual line
            TextLineBox linebox = _textLayer.GetTextLine(lineNumber);
            if (linebox == null)
            {
                //error=> line not found
                return;
            }
            _lineWalker.LoadLine(linebox);
            _pte.CurrentLineNumber = lineNumber;
        }
        public void TryMoveCaretTo(int charIndex, bool backward = false)
        {
            if (charIndex < 0)
            {
                throw new NotSupportedException();
            }

            if (CurrentLineNewCharIndex == charIndex) { return; }
            //----

            //on tech model
            if (_pte.NewCharIndex == 0 && backward)
            {
                //  move to prev line?
                if (backward && _lineWalker.LineNumber > 0)
                {
                    //move to prev line
                    MoveToPrevLine();
                    DoEnd();
                }
            }
            else
            {
                //move to next line
                if (charIndex < _pte.CurrentLineCharCount)
                {
                    //on the same line
                    _pte.TryMoveCaretTo(charIndex, backward);
                    //update presentation
                    _lineWalker.SetCurrentCharIndex2(_pte.NewCharIndex);
                }
                else
                {
                    if (_pte.IsOnTheEnd() && !backward)
                    {
                        if (_lineWalker.HasNextLine)
                        {
                            MoveToNextLine();
                        }
                    }
                    else
                    {
                        //on the same line
                        _pte.TryMoveCaretTo(charIndex, backward);
                        //update presentation
                        _lineWalker.SetCurrentCharIndex2(_pte.NewCharIndex);
                    }
                }
            }
        }
        public void DoEnd()
        {
            _pte.DoEnd();
            _lineWalker.SetCurrentCharIndexToEnd();
        }

        public void ReplaceCurrentLine(string newlineContent)
        {
            //temp fix
            throw new NotSupportedException();
        }
        public void DoHome()
        {
#if DEBUG
            //if (dbugEnableTextManRecorder)
            //{
            //    _dbugActivityRecorder.WriteInfo("TxLMan::DoHome");
            //    _dbugActivityRecorder.BeginContext();
            //}
#endif

            _pte.DoHome();//
            _lineWalker.SetCurrentCharIndexToBegin();
#if DEBUG
            //if (dbugEnableTextManRecorder)
            //{
            //    _dbugActivityRecorder.EndContext();
            //}
#endif
        }

        public void TryMoveCaretForward()
        {
            //move caret forward 1 key stroke
            TryMoveCaretTo(_lineWalker.NewCharIndex + 1);
        }
        public void TryMoveCaretBackward()
        {
            if (_lineWalker.NewCharIndex == 0)
            {
                //begin of the line
                if (CurrentLineNumber > 0)
                {
                    CurrentLineNumber--;
                    DoEnd();
                }
            }
            else
            {
                TryMoveCaretTo(_lineWalker.NewCharIndex - 1, true);
            }
        }
        //

        //
        public bool IsOnEndOfLine => _lineWalker.IsOnEndOfLine;
        public bool IsOnStartOfLine => _lineWalker.IsOnStartOfLine;
        public int CurrentCaretHeight
        {
            get
            {
                Run currentRun = this.CurrentTextRun;
                return (currentRun != null) ? currentRun.Height : 17; //TODO: review this...
            }
        }
        public Point CaretPos
        {
            get => _lineWalker.CaretPos;
            set
            {
                if (_textLayer.LineCount > 0)
                {
                    TextLineBox line = _textLayer.GetTextLineAtPos(value.Y);
                    int calculatedLineId = 0;
                    int lineTop = 0;
                    if (line != null)
                    {
                        calculatedLineId = line.LineNumber;
                        lineTop = line.Top;
                    }
                    this.CurrentLineNumber = calculatedLineId;
                    _lineWalker.TrySetCaretPos(value.X);
                    //check selected char index
                }
            }
        }
        //
        public int GetNextCharacterWidth() => _lineWalker.NextCharWidth;
        //
        public void SetCaretPos(int x, int y)
        {
            if (_textLayer.LineCount > 0)
            {
                TextLineBox line = _textLayer.GetTextLineAtPos(y);
                int lineNo = 0;
                int lineTop = 0;
                if (line != null)
                {
                    lineNo = line.LineNumber;
                    lineTop = line.Top;
                }

                this.CurrentLineNumber = lineNo;

                _lineWalker.TrySetCaretPos(x);
            }
        }
        public Rectangle CurrentLineArea => _lineWalker.LineArea;


    }




}