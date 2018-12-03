//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
    partial class RenderElement : IParentLink
    {

#if DEBUG
        public bool dbugBreak;
#endif
        internal LinkedListNode<RenderElement> _internalLinkedNode;
        //
        protected virtual bool _MayHasOverlapChild() => true;
        //
        public IParentLink MyParentLink => _parentLink;
        // 
        RenderElement IParentLink.ParentRenderElement => this;
        //yes, because when this renderElement act as parentlink
        //it return itself as parent
        //

        void IParentLink.AdjustLocation(ref Point p)
        {
            //nothing
        }
        RenderElement IParentLink.FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            //called from child node
            if (this._MayHasOverlapChild())
            {
                var child_internalLinkedNode = afterThisChild._internalLinkedNode;
                if (child_internalLinkedNode == null)
                {
                    return null;
                }
                var curnode = child_internalLinkedNode.Previous;
                while (curnode != null)
                {
                    var element = curnode.Value;
                    if (element.Contains(point))
                    {
                        return element;
                    }
                    curnode = curnode.Previous;
                }
            }
            return null;
        }

#if DEBUG
        string IParentLink.dbugGetLinkInfo()
        {
            return "";
        }
#endif
    }
}