//Apache2, 2014-present, WinterDev

namespace LayoutFarm.TextEditing
{
    partial class TextLine
    {
        public void AddLast(Run v)
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
        public void AddFirst(Run v)
        {
            AddNormalRunToFirst(v);
        }
        public RunStyle DefaultRunStyle => _textFlowLayer.DefaultRunStyle;
        public Run AddBefore(Run beforeVisualElement, CopyRun v)
        {
            var newRun = new TextRun(this, DefaultRunStyle, v.RawContent);
            AddBefore(beforeVisualElement, newRun);
            return newRun;
        }
        public void AddBefore(Run beforeVisualElement, Run v)
        {
            AddNormalRunBefore(beforeVisualElement, v);
        }
        public TextRun AddAfter(Run afterVisualElement, CopyRun v)
        {
            var newRun = new TextRun(this, DefaultRunStyle, v.RawContent);
            AddAfter(afterVisualElement, newRun);
            return newRun;
        }
        public void AddAfter(Run afterVisualElement, Run v)
        {
            AddNormalRunAfter(afterVisualElement, v);
        }
        internal void UnsafeAddLast(Run run)
        {
            run.SetLinkNode(_runs.AddLast(run));
        }
        internal void UnsafeAddFirst(Run run)
        {
            run.SetLinkNode(_runs.AddFirst(run));
        }
        internal void UnsafeAddAfter(Run after, Run run)
        {
            run.SetLinkNode(_runs.AddAfter(GetLineLinkNode(after), run));
        }
        internal void UnsafeRemoveVisualElement(Run v)
        {
            _runs.Remove(GetLineLinkNode(v));
        }
    }
}