//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    partial class EditableTextLine
    {
        public void RefreshInlineArrange()
        {
            EditableRun r = this.FirstRun;
            int lastestX = 0;
            while (r != null)
            {
                EditableRun.DirectSetLocation(
                        r,
                        lastestX,
                        r.Y);
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
            LinkedListNode<EditableRun> curNode = this.First;
            int cx = 0;
            while (curNode != null)
            {
                EditableRun ve = curNode.Value;
                EditableRun.DirectSetLocation(ve, cx, 0);
                cx += ve.Width;
                curNode = curNode.Next;
            }
        }
    }
}