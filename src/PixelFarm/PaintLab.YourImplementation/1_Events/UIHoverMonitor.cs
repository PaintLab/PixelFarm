//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    class UIHoverMonitorTask : UITimerTask
    {
        int _mouseMoveCounter = -1;
        TimerTick _targetEventHandler;
        public UIHoverMonitorTask(TimerTick targetEventHandler)
            : base(targetEventHandler)
        {
            _targetEventHandler = targetEventHandler;
        }
        public override void Reset()
        {
            _mouseMoveCounter = -1;
        }
        public override void InvokeAction()
        {
            _mouseMoveCounter++;
            if (_mouseMoveCounter > 1)
            {
                base.InvokeAction();
            }
        }
    }
}