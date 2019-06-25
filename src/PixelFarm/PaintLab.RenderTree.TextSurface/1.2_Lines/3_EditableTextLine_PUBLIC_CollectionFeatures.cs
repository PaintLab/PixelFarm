//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.TextEditing
{
    partial class EditableTextLine : LayoutFarm.RenderBoxes.IParentLink
    {
        public void AddLast(EditableRun v)
        {
            AddNormalRunToLast(v);
            //if (!v.IsLineBreak)
            //{
            //    AddNormalRunToLast(v);
            //}
            //else
            //{
            //    AddLineBreakAfter(this.LastRun);
            //}
        }
        public void AddLineBreakAfterLastRun()
        {
            AddLineBreakAfter(this.LastRun);
        }
        public void AddLineBreakBeforeFirstRun()
        {
            AddLineBreakBefore(this.FirstRun);
        }
        public void AddFirst(EditableRun v)
        {
            AddNormalRunToFirst(v);
            //if (!v.IsLineBreak)
            //{
            //    AddNormalRunToFirst(v);
            //}
            //else
            //{
            //    AddLineBreakBefore(this.FirstRun);
            //}
        }

        public EditableRun AddBefore(EditableRun beforeVisualElement, CopyRun v)
        {
            var newRun = new EditableTextRun(this.Root, v.RawContent, this.CurrentTextSpanStyle);
            AddBefore(beforeVisualElement, newRun);
            return newRun;
        }
        public void AddBefore(EditableRun beforeVisualElement, EditableRun v)
        {
            //if (!v.IsLineBreak)
            //{
            AddNormalRunBefore(beforeVisualElement, v);
            //}
            //else
            //{
            //    AddLineBreakBefore(beforeVisualElement);
            //}
        }
        public EditableTextRun AddAfter(EditableRun afterVisualElement, CopyRun v)
        {
            var newRun = new EditableTextRun(this.Root, v.RawContent, this.CurrentTextSpanStyle);
            AddAfter(afterVisualElement, newRun);
            return newRun;
        }
        public void AddAfter(EditableRun afterVisualElement, EditableRun v)
        {
            AddNormalRunAfter(afterVisualElement, v);
            //if (!v.IsLineBreak)
            //{
            //    AddNormalRunAfter(afterVisualElement, v);
            //}
            //else
            //{
            //    AddLineBreakAfter(afterVisualElement);
            //}
        }

        internal void UnsafeAddLast(EditableRun run)
        {
            run.SetInternalLinkedNode(_runs.AddLast(run), this);
        }
        internal void UnsafeAddFirst(EditableRun run)
        {
            run.SetInternalLinkedNode(_runs.AddFirst(run), this);
        }
        internal void UnsafeAddAfter(EditableRun after, EditableRun run)
        {
            run.SetInternalLinkedNode(_runs.AddAfter(GetLineLinkedNode(after), run), this);
        }
        internal void UnsafeRemoveVisualElement(EditableRun v)
        {
            _runs.Remove(GetLineLinkedNode(v));
        }

        RenderElement RenderBoxes.IParentLink.ParentRenderElement => this.OwnerFlowLayer.OwnerRenderElement;

        void RenderBoxes.IParentLink.AdjustLocation(ref Point p)
        {
            p.Y += this.LineTop;
        }

        RenderElement RenderBoxes.IParentLink.FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
        }
#if DEBUG
        string RenderBoxes.IParentLink.dbugGetLinkInfo()
        {
            return "editable-link";
        }
#endif
    }
}