//Apache2, 2014-present, WinterDev


namespace LayoutFarm.Text
{
    partial class EditableTextLine
    {
        public void AddLast(EditableRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunToLast(v);
            }
            else
            {
                AddLineBreakAfter(this.LastRun);
            }
        }
        public void AddFirst(EditableRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunToFirst(v);
            }
            else
            {
                AddLineBreakBefore(this.FirstRun);
            }
        }
        public void AddBefore(EditableRun beforeVisualElement, EditableRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunBefore(beforeVisualElement, v);
            }
            else
            {
                AddLineBreakBefore(beforeVisualElement);
            }
        }
        public void AddAfter(EditableRun afterVisualElement, EditableRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunAfter(afterVisualElement, v);
            }
            else
            {
                AddLineBreakAfter(afterVisualElement);
            }
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