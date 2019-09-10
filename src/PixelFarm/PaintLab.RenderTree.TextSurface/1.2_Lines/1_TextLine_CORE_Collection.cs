//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    partial class TextLineBox
    {
        void AddNormalRunToLast(Run v)
        {
            v.SetLinkNode(_runs.AddLast(v), this);
        }
        void AddNormalRunToFirst(Run v)
        {
            v.SetLinkNode(_runs.AddFirst(v), this);
        }

        static LinkedListNode<Run> GetLineLinkNode(Run ve)
        {
            return ve.LinkNode;
        }
        void AddNormalRunBefore(Run beforeVisualElement, Run v)
        {
            v.SetLinkNode(_runs.AddBefore(GetLineLinkNode(beforeVisualElement), v), this);
        }
        void AddNormalRunAfter(Run afterVisualElement, Run v)
        {
            v.SetLinkNode(_runs.AddAfter(GetLineLinkNode(afterVisualElement), v), this);
        }
        public void Clear()
        {
            LinkedListNode<Run> curNode = this.First;
            while (curNode != null)
            {
                Run.RemoveParentLink(curNode.Value);
                curNode = curNode.Next;
            }

            _runs.Clear();
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