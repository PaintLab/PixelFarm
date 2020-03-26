//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public abstract class UIVisitor
    {
        public bool ReportEnterAndExit { get; set; }
        public bool StopWalking { get; set; }

        protected virtual void OnEnter(object obj) { }
        protected virtual void OnExit(object obj) { }

        internal static void InvokeOnEnter(UIVisitor vis, object obj) => vis.OnEnter(obj);
        internal static void InvokeOnExit(UIVisitor vis, object obj) => vis.OnExit(obj); 
    }
}
