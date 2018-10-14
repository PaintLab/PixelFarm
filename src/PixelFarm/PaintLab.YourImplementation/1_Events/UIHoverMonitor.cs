//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    class UIHoverMonitorTask : UITimerTask
    {
        int mouseMoveCounter = -1;
        TimerTick targetEventHandler;
        public UIHoverMonitorTask(TimerTick targetEventHandler)
            : base(targetEventHandler)
        {
            this.targetEventHandler = targetEventHandler;
        }
        public override void Reset()
        {
            mouseMoveCounter = -1;
        }
        protected override void InvokeAction()
        {
            mouseMoveCounter++;
            if (mouseMoveCounter > 1)
            {
                base.InvokeAction();
            }
        }
    }
}