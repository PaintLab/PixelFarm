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
    sealed partial class TextLineBox
    {
        //this class dose not have Painting function
        //we paint each run at text-layer object
        //current line runs
        readonly LinkedList<Run> _runs = new LinkedList<Run>();
        /// <summary>
        /// owner layer
        /// </summary>
        TextFlowLayer _textFlowLayer;
        int _currentLineNumber;
        int _actualLineHeight;
        int _actualLineWidth;
        int _lineTop;
        int _lineFlags;

        bool _validCharCount = false;

        // 
        const int LINE_CONTENT_ARRANGED = 1 << (1 - 1);
        const int LINE_SIZE_VALID = 1 << (2 - 1);
        const int LOCAL_SUSPEND_LINE_REARRANGE = 1 << (3 - 1);
        const int END_WITH_LINE_BREAK = 1 << (4 - 1);

#if DEBUG
        static int dbugLineTotalCount = 0;
        internal int dbugLineId;
#endif
        internal TextLineBox(TextFlowLayer textFlowLayer)
        {
            _textFlowLayer = textFlowLayer;
            _actualLineHeight = textFlowLayer.DefaultLineHeight; //we start with default line height
#if DEBUG
            this.dbugLineId = dbugLineTotalCount;
            dbugLineTotalCount++;

#endif

            OverlappedTop = 3; //test only
            OverlappedBottom = 3; //test only
        }

        public ITextService TextService => _textFlowLayer.TextServices; //TODO: review this again***

        internal void ClientRunInvalidateGraphics(Run clientRun)
        {
            //bubble-up invalidated area from client
            Rectangle bounds = clientRun.Bounds;
            bounds.OffsetY(Top - OverlappedTop); //offset line top
            bounds.Height += (OverlappedTop + OverlappedBottom);

            OwnerFlowLayer.ClientLineBubbleupInvalidateArea(bounds);
        }
        internal byte OverlappedTop { get; set; }
        internal byte OverlappedBottom { get; set; }

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
        internal Run LastRun => _runs.Last?.Value;

        public float GetXOffsetAtCharIndex(int charIndex)
        {
            float xoffset = 0;
            int acc_charCount = 0;

            Run r = this.FirstRun;
            while (r != null)
            {
                if (r.CharacterCount + acc_charCount >= charIndex)
                {
                    //found at this run
                    return xoffset + r.GetRunWidth(charIndex - acc_charCount);
                }
                xoffset += r.Width;
                acc_charCount += r.CharacterCount;
                r = r.NextRun;
            }
            //foreach (Run r in _runs)
            //{
            //    if (r.CharacterCount + acc_charCount >= charIndex)
            //    {
            //        //found at this run
            //        return xoffset + r.GetRunWidth(charIndex - acc_charCount);
            //    }
            //    xoffset += r.Width;
            //    acc_charCount += r.CharacterCount;
            //}
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
            _actualLineHeight = maxHeight; //??

            if (this.RunCount == 0)
            {
                //no span
                _actualLineHeight = OwnerFlowLayer.DefaultLineHeight;
            }
        }
        internal bool HitTestCore(HitChain hitChain)
        {
            hitChain.GetTestPoint(out int testX, out int testY);
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
            get => (_lineFlags & END_WITH_LINE_BREAK) != 0;

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

        public bool IntersectsWith(int y) => y >= _lineTop && y < (_lineTop + _actualLineHeight);
        //
        public int Top => _lineTop;
        public int LineTop => _lineTop;
        //
        public int ActualLineWidth => _actualLineWidth;
        //
        public int ActualLineHeight => _actualLineHeight;
        //
        public Rectangle ActualLineArea => new Rectangle(0, _lineTop - OverlappedTop, _actualLineWidth, _actualLineHeight + OverlappedTop + OverlappedBottom);

        internal IEnumerable<Run> GetRunIterForward(Run startVisualElement)
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
        internal IEnumerable<Run> GetRunIterForward(Run startVisualElement, Run stopVisualElement)
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

        internal void InvalidateCharCount() => _validCharCount = false;

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
        public IEnumerable<Run> GetRunIter()
        {
            foreach (Run r in _runs)
            {
                yield return r;
            }
        }

        //
        public int LineBottom => _lineTop + _actualLineHeight;
        //
        internal int LineWidth
        {
            get
            {
                Run lastRun = this.LastRun;
                return (lastRun != null) ? lastRun.Right : 0;
            }
        }
        public void SetTop(int linetop) => _lineTop = linetop;
        //
        public int LineNumber => _currentLineNumber;
        //
        internal void SetLineNumber(int value) => _currentLineNumber = value;
        //
        bool IsFirstLine => _currentLineNumber == 0;
        //
        bool IsLastLine => _currentLineNumber == _textFlowLayer.LineCount - 1;
        //
        bool IsSingleLine => IsFirstLine && IsLastLine;
        //
        public bool IsBlankLine => RunCount == 0;
        //
        public TextLineBox Next => (_currentLineNumber < _textFlowLayer.LineCount - 1) ? _textFlowLayer.GetTextLine(_currentLineNumber + 1) : null;

        public TextLineBox Prev => (_currentLineNumber > 0) ? _textFlowLayer.GetTextLine(_currentLineNumber - 1) : null;

        public Run FirstRun => _runs.First?.Value;

        public bool NeedArrange => (_lineFlags & LINE_CONTENT_ARRANGED) == 0;

        internal void ValidateContentArrangement() => _lineFlags |= LINE_CONTENT_ARRANGED;

        public static void InnerCopyLineContent(TextLineBox line, StringBuilder stBuilder)
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
            //not include line-end char?
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
        //internal bool IsLocalSuspendLineRearrange => (_lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0;
        //internal void InvalidateLineLayout()
        //{
        //    _lineFlags &= ~LINE_SIZE_VALID;
        //    _lineFlags &= ~LINE_CONTENT_ARRANGED;
        //}
    }
}