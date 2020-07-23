//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    partial class TextLineBox
    {
        static LinkedListNode<Run> GetLineLinkNode(Run ve) => ve.LinkNode;

        void AddNormalRunToLast(Run v) => v.SetLinkNode(_runs.AddLast(v), this);

        void AddNormalRunToFirst(Run v) => v.SetLinkNode(_runs.AddFirst(v), this);

        void AddNormalRunBefore(Run beforeVisRun, Run v) => v.SetLinkNode(_runs.AddBefore(GetLineLinkNode(beforeVisRun), v), this);

        void AddNormalRunAfter(Run afterVisRun, Run v) => v.SetLinkNode(_runs.AddAfter(GetLineLinkNode(afterVisRun), v), this);

        public void Clear()
        {
            LinkedListNode<Run> curNode = this.First;
            while (curNode != null)
            {
                Run.RemoveParentLink(curNode.Value);
                curNode = curNode.Next;
            }
            _runs.Clear();
            _cacheCharCount = 0;
            _validCharCount = true;
        }

        public void Remove(Run v)
        {
            //#if DEBUG
            //            if (v.IsLineBreak)
            //            {
            //                throw new NotSupportedException("not support line break");
            //            }
            //#endif


            _runs.Remove(GetLineLinkNode(v));
            Run.RemoveParentLink(v);
            if ((_lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }

            if (!this.EndWithLineBreak && this.RunCount == 0 && _currentLineNumber > 0)
            {
                if (!_textFlowLayer.GetTextLine(_currentLineNumber - 1).EndWithLineBreak)
                {
                    _textFlowLayer.Remove(_currentLineNumber);
                }
            }
            else
            {
                //var ownerVe = editableFlowLayer.OwnerRenderElement;
                //if (ownerVe != null)
                //{
                //    RenderElement.InnerInvalidateLayoutAndStartBubbleUp(ownerVe);
                //}
                //else
                //{
                //    throw new NotSupportedException();
                //}
            }
        }
    }
}