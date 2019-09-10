//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    partial class TextLineBox
    {
        public void RefreshInlineArrange()
        {
            Run r = this.FirstRun;
            int lastestX = 0;
            while (r != null)
            {
                Run.DirectSetLocation(
                        r,
                        lastestX,
                        r.Top);
                lastestX += r.Width;
                r = r.NextRun;
            }
        }
        internal void SetPostArrangeLineSize(int lineWidth, int lineHeight)
        {
            _actualLineWidth = lineWidth;
            _actualLineHeight = lineHeight;

        }
        public void LocalSuspendLineReArrange()
        {
            _lineFlags |= LOCAL_SUSPEND_LINE_REARRANGE;
        }
        public void LocalResumeLineReArrange()
        {
            _lineFlags &= ~LOCAL_SUSPEND_LINE_REARRANGE;
            LinkedListNode<Run> curNode = this.First;
            int cx = 0;
            while (curNode != null)
            {
                Run ve = curNode.Value;
                Run.DirectSetLocation(ve, cx, 0);
                cx += ve.Width;
                curNode = curNode.Next;
            }
        }
    }
}