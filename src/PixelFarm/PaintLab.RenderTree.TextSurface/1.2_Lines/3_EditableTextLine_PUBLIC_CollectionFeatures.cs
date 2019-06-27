//Apache2, 2014-present, WinterDev

namespace LayoutFarm.TextEditing
{
    partial class EditableTextLine
    {
        public void AddLast(EditableRun v)
        {
            AddNormalRunToLast(v);
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
        }
        public RunStyle DefaultRunStyle => _editableFlowLayer.DefaultRunStyle;
        public EditableRun AddBefore(EditableRun beforeVisualElement, CopyRun v)
        {
            var newRun = new EditableTextRun(DefaultRunStyle, v.RawContent);
            AddBefore(beforeVisualElement, newRun);
            return newRun;
        }
        public void AddBefore(EditableRun beforeVisualElement, EditableRun v)
        {
            AddNormalRunBefore(beforeVisualElement, v);
        }
        public EditableTextRun AddAfter(EditableRun afterVisualElement, CopyRun v)
        {
            var newRun = new EditableTextRun(DefaultRunStyle, v.RawContent);
            AddAfter(afterVisualElement, newRun);
            return newRun;
        }
        public void AddAfter(EditableRun afterVisualElement, EditableRun v)
        {
            AddNormalRunAfter(afterVisualElement, v);
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
    }
}