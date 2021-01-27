//Apache2, 2014-present, WinterDev

using System.Collections.Generic; 
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
        
        void IParentLink.AdjustLocation(ref int px, ref int py)
        {
            //nothing
        }
       
#if DEBUG
        string IParentLink.dbugGetLinkInfo()
        {
            return "";
        }
#endif
    }
}