//Apache2, 2014-present, WinterDev

namespace LayoutFarm.TextEditing
{
    partial class TextLineBox
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

        RunStyle DefaultRunStyle => _textFlowLayer.DefaultRunStyle;
        public Run AddBefore(Run beforeVisRun, CopyRun v)
        {
            var newRun = new TextRun(DefaultRunStyle, v.RawContent);
            AddBefore(beforeVisRun, newRun);
            return newRun;
        }
        public void AddBefore(Run beforeVisRun, Run v)
        {
            AddNormalRunBefore(beforeVisRun, v);
        }
        public TextRun AddAfter(Run afterVisRun, CopyRun v)
        {
            var newRun = new TextRun(DefaultRunStyle, v.RawContent);
            AddAfter(afterVisRun, newRun);
            return newRun;
        }
        public void AddAfter(Run afterVisRun, Run v)
        {
            AddNormalRunAfter(afterVisRun, v);
        }
        
    }
}