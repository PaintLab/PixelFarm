//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextFlow
{
    partial class TextLineBox
    {
        internal void RefreshInlineArrange()
        {
            Run r = this.FirstRun;
            int lastestX = 0;
            while (r != null)
            {
                Run.DirectSetLocation(
                        r,
                        lastestX,
                        r.Top);
                //no inter-space between each run
                lastestX += r.Width;

                r = r.NextRun;
            }
        }
       
        internal void LocalSuspendLineReArrange()
        {
            _lineFlags |= LOCAL_SUSPEND_LINE_REARRANGE;
        }
        internal void LocalResumeLineReArrange()
        {
            _lineFlags &= ~LOCAL_SUSPEND_LINE_REARRANGE;
            LinkedListNode<Run> curNode = this.First;
            int cx = 0;
            while (curNode != null)
            {
                Run r = curNode.Value;
                Run.DirectSetLocation(r, cx, 0);
                //no inter-space between each run
                cx += r.Width;
                curNode = curNode.Next;
            }
        }
    }
}