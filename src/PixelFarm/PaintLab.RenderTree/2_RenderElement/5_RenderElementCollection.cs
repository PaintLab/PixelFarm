//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
#if DEBUG
    public struct dbugLayoutInfo
    {
        public readonly int dbugLayerId;
        public dbugLayoutInfo(int dbugLayerId) => this.dbugLayerId = dbugLayerId;
    }
#endif
    public interface IParentLink
    {
        RenderElement ParentRenderElement { get; }
        void AdjustLocation(ref int px, ref int py);
        RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point);
#if DEBUG
        string dbugGetLinkInfo();
#endif

    }

    public enum LayoutHint : byte
    {
        Custom,
        HorizontalRowNonOverlap,
        VerticalColumnNonOverlap,
    }
    sealed class RenderElementCollection
    {
        readonly LinkedList<RenderElement> _myElements = new LinkedList<RenderElement>();
        int _contentW;
        int _contentH;

#if DEBUG
        static int dbug_TotalId;
        public readonly int dbug_id;
#endif
        public RenderElementCollection()
        {
#if DEBUG
            dbug_id = dbug_TotalId++;
#endif

        }


        public void InsertChildBefore(RenderElement parent, RenderElement before, RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddBefore(before._internalLinkedNode, re);
            RenderElement.SetParentLink(re, parent);
            re.InvalidateGraphics();
        }
        public void InsertChildAfter(RenderElement parent, RenderElement after, RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddAfter(after._internalLinkedNode, re);
            RenderElement.SetParentLink(re, parent);
            re.InvalidateGraphics();
        }
        public void AddFirst(RenderElement parent, RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddFirst(re);
            RenderElement.SetParentLink(re, parent);
            re.InvalidateGraphics();
        }
        public void AddChild(RenderElement parent, RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddLast(re);
            RenderElement.SetParentLink(re, parent);
            re.InvalidateGraphics();
        }
        public void RemoveChild(RenderElement parent, RenderElement re)
        {
            if (re._internalLinkedNode != null)
            {
                _myElements.Remove(re._internalLinkedNode);
                re._internalLinkedNode = null;
            }
            Rectangle bounds = re.RectBounds;
            RenderElement.SetParentLink(re, null);
            RenderElement.InvalidateGraphicLocalArea(parent, bounds);
        }
        public void Clear()
        {

            LinkedListNode<RenderElement> curNode = _myElements.First;
            while (curNode != null)
            {
                curNode.Value._internalLinkedNode = null;
                curNode = curNode.Next;
            }

            _myElements.Clear();
        }

        public IEnumerable<RenderElement> GetDrawingIter()
        {
            LinkedListNode<RenderElement> curNode = _myElements.First;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Next;
            }
        }
        public IEnumerable<RenderElement> GetHitTestIter()
        {
            LinkedListNode<RenderElement> curNode = _myElements.Last;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Previous;
            }
        }
        public IEnumerable<RenderElement> GetRenderElementReverseIter()
        {
            LinkedListNode<RenderElement> cur = _myElements.Last;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Previous;
            }
        }
        public IEnumerable<RenderElement> GetRenderElementIter()
        {
            LinkedListNode<RenderElement> cur = _myElements.First;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Next;
            }
        }

        static Size ReCalculateContentSizeNoLayout(LinkedList<RenderElement> velist)
        {
            int local_lineWidth = 0;
            int local_lineHeight = 17;
            LinkedListNode<RenderElement> curNode = velist.First;
            while (curNode != null)
            {
                RenderElement visualElement = curNode.Value;
                if (!visualElement.HasCalculatedSize)
                {
                    visualElement.TopDownReCalculateContentSize();
                }
                int e_desiredRight = visualElement.Right;
                if (local_lineWidth < e_desiredRight)
                {
                    local_lineWidth = e_desiredRight;
                }
                int e_desiredBottom = visualElement.Bottom;
                if (local_lineHeight < e_desiredBottom)
                {
                    local_lineHeight = e_desiredBottom;
                }
                curNode = curNode.Next;
            }

            return new Size(local_lineWidth, local_lineHeight);
        }

        public void TopDownReCalculateContentSize()
        {
            //#if DEBUG

            //            vinv_dbug_EnterLayerReCalculateContent(this);
            //#endif
            Size s = ReCalculateContentSizeNoLayout(_myElements);
            _contentW = s.Width;
            _contentH = s.Height;

            //#if DEBUG
            //            vinv_dbug_ExitLayerReCalculateContent();
            //#endif
        }
        public Size CalculatedContentSize => new Size(_contentW, _contentH);
#if DEBUG
        public dbugLayoutInfo dbugGetLayerInfo() => new dbugLayoutInfo(this.dbug_id);
        public void dbug_DumpElementProps(dbugLayoutMsgWriter writer)
        {
            writer.Add(new dbugLayoutMsg(
                this, this.ToString()));
            writer.EnterNewLevel();
            foreach (RenderElement child in this.GetDrawingIter())
            {
                child.dbug_DumpVisualProps(writer);
            }
            writer.LeaveCurrentLevel();
        }
        public int dbugChildCount => _myElements.Count;
        public override string ToString()
        {
            return "elems " + "(L" + dbug_id + ") postcal:" +
                new Size(_contentW, _contentH);
        }
#endif
    }
}