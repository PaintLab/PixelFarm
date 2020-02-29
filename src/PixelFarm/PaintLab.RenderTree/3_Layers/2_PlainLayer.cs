//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public class PlainLayer : RenderElementLayer
    {
        LinkedList<RenderElement> _myElements = new LinkedList<RenderElement>();
        public PlainLayer(RenderElement owner)
            : base(owner)
        {
        }
        public BoxContentLayoutKind LayoutHint { get; set; }
        public override IEnumerable<RenderElement> GetRenderElementReverseIter()
        {
            LinkedListNode<RenderElement> cur = _myElements.Last;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Previous;
            }
        }
        public override IEnumerable<RenderElement> GetRenderElementIter()
        {
            LinkedListNode<RenderElement> cur = _myElements.First;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Next;
            }
        }
        public void InsertChildBefore(RenderElement before, RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddBefore(before._internalLinkedNode, re);
            RenderElement.SetParentLink(re, _owner);
            re.InvalidateGraphics();
        }
        public void InsertChildAfter(RenderElement after, RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddAfter(after._internalLinkedNode, re);
            RenderElement.SetParentLink(re, _owner);
            re.InvalidateGraphics();
        }
        public void AddFirst(RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddFirst(re);
            RenderElement.SetParentLink(re, _owner);
            re.InvalidateGraphics();
        }
        public void AddChild(RenderElement re)
        {
            re._internalLinkedNode = _myElements.AddLast(re);
            RenderElement.SetParentLink(re, _owner);
            re.InvalidateGraphics();
        }
        public void RemoveChild(RenderElement re)
        {
            if (re._internalLinkedNode != null)
            {
                _myElements.Remove(re._internalLinkedNode);
                re._internalLinkedNode = null;
            }
            Rectangle bounds = re.RectBounds;
            RenderElement.SetParentLink(re, null);

            RenderElement.InvalidateGraphicLocalArea(this.OwnerRenderElement, bounds);
        }
        public override void Clear()
        {

            LinkedListNode<RenderElement> curNode = _myElements.First;
            while (curNode != null)
            {
                curNode.Value._internalLinkedNode = null;
                curNode = curNode.Next;
            }

            _myElements.Clear();
            this.OwnerRenderElement.InvalidateGraphics();
        }

#if DEBUG
        public int dbugChildCount => _myElements.Count;
#endif

        IEnumerable<RenderElement> GetDrawingIter()
        {
            LinkedListNode<RenderElement> curNode = _myElements.First;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Next;
            }
        }
        IEnumerable<RenderElement> GetHitTestIter()
        {
            LinkedListNode<RenderElement> curNode = _myElements.Last;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Previous;
            }
        }


        public override void DrawChildContent(DrawBoard d, UpdateArea updateArea)
        {
            if ((_layerFlags & IS_LAYER_HIDDEN) != 0)
            {
                return;
            }
            this.BeginDrawingChildContent();

            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;


            switch (LayoutHint)
            {
                case BoxContentLayoutKind.Absolute:
                    {
                        foreach (RenderElement child in this.GetDrawingIter())
                        {
                            if (child.IntersectsWith(updateArea) ||
                               !child.NeedClipArea)
                            {
                                //if the child not need clip
                                //its children (if exist) may intersect 
                                int x = child.X;
                                int y = child.Y;

                                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                                updateArea.Offset(-x, -y);
                                RenderElement.Render(child, d, updateArea);
                                updateArea.Offset(x, y);
                            }
                        }

                        //restore
                        d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);
                    }
                    break;
                case BoxContentLayoutKind.HorizontalStack:
                    {
                        bool found = false;
                        foreach (RenderElement child in this.GetDrawingIter())
                        {
                            if (child.IntersectsWith(updateArea))
                            {
                                found = true;
                                //if the child not need clip
                                //its children (if exist) may intersect 
                                int x = child.X;
                                int y = child.Y;

                                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                                updateArea.Offset(-x, -y);
                                RenderElement.Render(child, d, updateArea);
                                updateArea.Offset(x, y);
                            }
                            else if (found)
                            {
                                break;
                            }
                        }

                        //restore
                        d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);
                    }
                    break;
                case BoxContentLayoutKind.VerticalStack:
                    {
                        bool found = false;
                        foreach (RenderElement child in this.GetDrawingIter())
                        {
                            if (child.IntersectsWith(updateArea))
                            {
                                found = true;
                                //if the child not need clip
                                //its children (if exist) may intersect 
                                int x = child.X;
                                int y = child.Y;

                                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                                updateArea.Offset(-x, -y);
                                RenderElement.Render(child, d, updateArea);
                                updateArea.Offset(x, y);
                            }
                            else if (found)
                            {
                                break;
                            }
                        }
                        d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);
                    }
                    break;
            }

            this.FinishDrawingChildContent();
        }
#if DEBUG
        public override void dbug_DumpElementProps(dbugLayoutMsgWriter writer)
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
#endif
        public override bool HitTestCore(HitChain hitChain)
        {
            if ((_layerFlags & IS_LAYER_HIDDEN) == 0)
            {
                foreach (RenderElement renderE in this.GetHitTestIter())
                {
                    if (renderE.HitTestCore(hitChain))
                    {
                        return true;
                    }
                }
            }
            return false;
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


        public override void TopDownReArrangeContent()
        {
            //vinv_IsInTopDownReArrangePhase = true;
#if DEBUG
            vinv_dbug_EnterLayerReArrangeContent(this);
#endif
            //this.BeginLayerLayoutUpdate();
            //if (CustomRearrangeContent != null)
            //{
            //    CustomRearrangeContent(this, EventArgs.Empty);
            //}

            //this.EndLayerLayoutUpdate();
#if DEBUG
            vinv_dbug_ExitLayerReArrangeContent();
#endif
        }
        public override void TopDownReCalculateContentSize()
        {
#if DEBUG

            vinv_dbug_EnterLayerReCalculateContent(this);
#endif

            SetPostCalculateLayerContentSize(ReCalculateContentSizeNoLayout(_myElements));
#if DEBUG
            vinv_dbug_ExitLayerReCalculateContent();
#endif
        }
#if DEBUG
        public override string ToString()
        {
            return "plain layer " + "(L" + dbug_layer_id + this.dbugLayerState + ") postcal:" +
                this.PostCalculateContentSize.ToString() + " of " + this.OwnerRenderElement.dbug_FullElementDescription();
        }
#endif
    }
}