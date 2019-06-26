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
    sealed partial class EditableTextLine
    {
        //current line runs
        LinkedList<EditableRun> _runs = new LinkedList<EditableRun>();

        EditableTextFlowLayer _editableFlowLayer;

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
        internal EditableTextLine(EditableTextFlowLayer ownerFlowLayer)
        {

            _editableFlowLayer = ownerFlowLayer;
            _actualLineHeight = ownerFlowLayer.DefaultLineHeight; //we start with default line height
#if DEBUG
            this.dbugLineId = dbugLineTotalCount;
            dbugLineTotalCount++;
#endif
        }

        public EditableTextFlowLayer EditableTextFlowLayer => _editableFlowLayer;
        public RootGraphic Root => _editableFlowLayer.RootGraphic;
        internal void RemoveOwnerFlowLayer() { _editableFlowLayer = null; }
        //
        public int RunCount => _runs.Count;
        //
        /// <summary>
        /// first run node
        /// </summary>
        public LinkedListNode<EditableRun> First => _runs.First;
        //
        /// <summary>
        /// last run node
        /// </summary>
        public LinkedListNode<EditableRun> Last => _runs.Last;
        //

        //
        public IEnumerable<EditableRun> GetTextRunIter()
        {
            foreach (EditableRun r in _runs)
            {
                yield return r;
            }
        }
        internal EditableRun LastRun
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
            foreach (EditableRun r in _runs)
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
            EditableRun r = this.FirstRun;
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
                LinkedListNode<EditableRun> cnode = this.First;
                int curLineTop = _lineTop;
                hitChain.OffsetTestPoint(0, -curLineTop);
                while (cnode != null)
                {
                    if (cnode.Value.HitTestCore(hitChain))
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

        public EditableTextFlowLayer OwnerFlowLayer => _editableFlowLayer;
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

        internal IEnumerable<EditableRun> GetVisualElementForward(EditableRun startVisualElement)
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
        internal IEnumerable<EditableRun> GetVisualElementForward(EditableRun startVisualElement, EditableRun stopVisualElement)
        {
            if (startVisualElement != null)
            {
                LinkedListNode<EditableRun> lexnode = GetLineLinkedNode(startVisualElement);
                while (lexnode != null)
                {
                    yield return lexnode.Value;
                    if (lexnode.Value == stopVisualElement)
                    {
                        break;
                    }
                    lexnode = lexnode.Next;
                }
            }
        }
        public int CharCount
        {
            get
            {
                //TODO: reimplement this again
                int charCount = 0;
                foreach (EditableRun r in _runs)
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
        bool IsLastLine => _currentLineNumber == _editableFlowLayer.LineCount - 1;
        //
        bool IsSingleLine => IsFirstLine && IsLastLine;
        //
        public bool IsBlankLine => RunCount == 0;
        //
        public EditableTextLine Next
        {
            get
            {
                if (_currentLineNumber < _editableFlowLayer.LineCount - 1)
                {
                    return _editableFlowLayer.GetTextLine(_currentLineNumber + 1);
                }
                else
                {
                    return null;
                }
            }
        }
        public EditableTextLine Prev
        {
            get
            {
                if (_currentLineNumber > 0)
                {
                    return _editableFlowLayer.GetTextLine(_currentLineNumber - 1);
                }
                else
                {
                    return null;
                }
            }
        }

        public EditableRun FirstRun
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
        public static void InnerCopyLineContent(EditableTextLine line, StringBuilder stBuilder)
        {
            line.CopyLineContent(stBuilder);
        }
        public void CopyLineContent(StringBuilder stBuilder)
        {
            LinkedListNode<EditableRun> curNode = this.First;
            while (curNode != null)
            {
                EditableRun v = curNode.Value;
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