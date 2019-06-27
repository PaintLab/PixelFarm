//Apache2, 2014-present, WinterDev

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.TextEditing
{

#if DEBUG
    [DebuggerDisplay("ELN {dbugShortLineInfo}")]
#endif
    sealed partial class TextLine
    {
        //current line runs
        LinkedList<Run> _runs = new LinkedList<Run>();
        /// <summary>
        /// owner layer
        /// </summary>
        TextFlowLayer _textFlowLayer;
        int _currentLineNumber;
        int _actualLineHeight;
        int _actualLineWidth;
        int _lineTop;
        int _lineFlags;
        //
        //
        const int LINE_CONTENT_ARRANGED = 1 << (1 - 1);
        const int LINE_SIZE_VALID = 1 << (2 - 1);
        const int LOCAL_SUSPEND_LINE_REARRANGE = 1 << (3 - 1);
        const int END_WITH_LINE_BREAK = 1 << (4 - 1);

#if DEBUG
        static int dbugLineTotalCount = 0;
        internal int dbugLineId;
#endif
        internal TextLine(TextFlowLayer textFlowLayer)
        {

            _textFlowLayer = textFlowLayer;
            _actualLineHeight = textFlowLayer.DefaultLineHeight; //we start with default line height
#if DEBUG
            this.dbugLineId = dbugLineTotalCount;
            dbugLineTotalCount++;
#endif
        }

        public ITextService TextService => _textFlowLayer.TextServices;

        internal void ClientRunInvalidateGraphics(Rectangle bubbleUpInvalidatedArea)
        {
            bubbleUpInvalidatedArea.OffsetY(Top); //offset line top
            OwnerFlowLayer.ClientLineBubbleupInvalidateArea(bubbleUpInvalidatedArea);
        }

        internal void RemoveOwnerFlowLayer() => _textFlowLayer = null;

        public int RunCount => _runs.Count;

        /// <summary>
        /// first run node
        /// </summary>
        public LinkedListNode<Run> First => _runs.First;
        //
        /// <summary>
        /// last run node
        /// </summary>
        public LinkedListNode<Run> Last => _runs.Last;
        // 

        public IEnumerable<Run> GetTextRunIter()
        {
            foreach (Run r in _runs)
            {
                yield return r;
            }
        }
        internal Run LastRun
        {
            get
            {
                if (this.RunCount > 0)
                {
                    return this.Last.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public float GetXOffsetAtCharIndex(int charIndex)
        {
            float xoffset = 0;
            int acc_charCount = 0;
            foreach (Run r in _runs)
            {
                if (r.CharacterCount + acc_charCount >= charIndex)
                {
                    //found at this run
                    return xoffset + r.GetRunWidth(charIndex - acc_charCount);
                }
                xoffset += r.Width;
                acc_charCount += r.CharacterCount;
            }
            return 0;//?
        }
        public void TextLineReCalculateActualLineSize()
        {
            Run r = this.FirstRun;
            int maxHeight = 2;
            int accumWidth = 0;
            while (r != null)
            {
                if (r.Height > maxHeight)
                {
                    maxHeight = r.Height;
                }
                accumWidth += r.Width;
                r = r.NextRun;
            }
            _actualLineWidth = accumWidth;
            _actualLineHeight = maxHeight;

            if (this.RunCount == 0)
            {
                //no span
                _actualLineHeight = OwnerFlowLayer.DefaultLineHeight;
            }
        }
        internal bool HitTestCore(HitChain hitChain)
        {
            int testX;
            int testY;
            hitChain.GetTestPoint(out testX, out testY);
            if (this.RunCount == 0)
            {
                return false;
            }
            else
            {
                LinkedListNode<Run> cnode = this.First;
                int curLineTop = _lineTop;
                hitChain.OffsetTestPoint(0, -curLineTop);
                while (cnode != null)
                {
                    if (cnode.Value.HitTest(hitChain.TestPointX, hitChain.TestPointY))
                    {
                        hitChain.OffsetTestPoint(0, curLineTop);
                        return true;
                    }
                    cnode = cnode.Next;
                }
                hitChain.OffsetTestPoint(0, curLineTop);
                return false;
            }
        }

        public TextFlowLayer OwnerFlowLayer => _textFlowLayer;
        //
        public bool EndWithLineBreak
        {
            get
            {
                return (_lineFlags & END_WITH_LINE_BREAK) != 0;
            }
            set
            {
                if (value)
                {
                    _lineFlags |= END_WITH_LINE_BREAK;
                }
                else
                {
                    _lineFlags &= ~END_WITH_LINE_BREAK;
                }
            }
        }

        public bool IntersectsWith(int y)
        {
            return y >= _lineTop && y < (_lineTop + _actualLineHeight);
        }
        //
        public int Top => _lineTop;
        public int LineTop => _lineTop;
        //
        public int ActualLineWidth => _actualLineWidth;
        //
        public int ActualLineHeight => _actualLineHeight;
        //
        public Rectangle ActualLineArea => new Rectangle(0, _lineTop, _actualLineWidth, _actualLineHeight);

        internal IEnumerable<Run> GetVisualElementForward(Run startVisualElement)
        {
            if (startVisualElement != null)
            {
                yield return startVisualElement;
                var curRun = startVisualElement.NextRun;
                while (curRun != null)
                {
                    yield return curRun;
                    curRun = curRun.NextRun;
                }
            }
        }
        internal IEnumerable<Run> GetVisualElementForward(Run startVisualElement, Run stopVisualElement)
        {
            if (startVisualElement != null)
            {
                LinkedListNode<Run> node = GetLineLinkNode(startVisualElement);
                while (node != null)
                {
                    yield return node.Value;
                    if (node.Value == stopVisualElement)
                    {
                        break;
                    }
                    node = node.Next;
                }
            }
        }
        public int CharCount
        {
            get
            {
                //TODO: reimplement this again
                int charCount = 0;
                foreach (Run r in _runs)
                {
                    charCount += r.CharacterCount;
                }
                return charCount;
            }
        }

        //
        public int LineBottom => _lineTop + _actualLineHeight;
        //
        internal int LineWidth
        {
            get
            {
                var lastRun = this.LastRun;
                if (lastRun == null)
                {
                    return 0;
                }
                else
                {
                    return lastRun.Right;
                }
            }
        }

        public void SetTop(int linetop)
        {
            _lineTop = linetop;
        }
#if DEBUG
        public override string ToString()
        {
            return this.dbugShortLineInfo;
        }
        public string dbugShortLineInfo
        {
            get
            {
                return "LINE[" + dbugLineId + "]:" + _currentLineNumber + "{T:" + _lineTop.ToString() + ",W:" +
                   _actualLineWidth + ",H:" + _actualLineHeight + "}";
            }
        }
#endif
        //
        public int LineNumber => _currentLineNumber;
        //
        internal void SetLineNumber(int value)
        {
            _currentLineNumber = value;
        }
        //
        bool IsFirstLine => _currentLineNumber == 0;
        //
        bool IsLastLine => _currentLineNumber == _textFlowLayer.LineCount - 1;
        //
        bool IsSingleLine => IsFirstLine && IsLastLine;
        //
        public bool IsBlankLine => RunCount == 0;
        //
        public TextLine Next
        {
            get
            {
                if (_currentLineNumber < _textFlowLayer.LineCount - 1)
                {
                    return _textFlowLayer.GetTextLine(_currentLineNumber + 1);
                }
                else
                {
                    return null;
                }
            }
        }
        public TextLine Prev
        {
            get
            {
                if (_currentLineNumber > 0)
                {
                    return _textFlowLayer.GetTextLine(_currentLineNumber - 1);
                }
                else
                {
                    return null;
                }
            }
        }

        public Run FirstRun
        {
            get
            {
                if (this.RunCount > 0)
                {
                    return this.First.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        //
        public bool NeedArrange => (_lineFlags & LINE_CONTENT_ARRANGED) == 0;

        internal void ValidateContentArrangement()
        {
            _lineFlags |= LINE_CONTENT_ARRANGED;
        }
        public static void InnerCopyLineContent(TextLine line, StringBuilder stBuilder)
        {
            line.CopyLineContent(stBuilder);
        }
        public void CopyLineContent(StringBuilder stBuilder)
        {
            LinkedListNode<Run> curNode = this.First;
            while (curNode != null)
            {
                Run v = curNode.Value;
                v.CopyContentToStringBuilder(stBuilder);
                curNode = curNode.Next;
            }
        }

        internal bool IsLocalSuspendLineRearrange => (_lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0;

        internal void InvalidateLineLayout()
        {
            _lineFlags &= ~LINE_SIZE_VALID;
            _lineFlags &= ~LINE_CONTENT_ARRANGED;
        }
    }
}