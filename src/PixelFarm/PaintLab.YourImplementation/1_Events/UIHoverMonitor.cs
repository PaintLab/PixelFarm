//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    class UIHoverMonitorTask : UITimerTask
    {
        int _mouseMoveCounter = -1;
        UIMouseHoverEventArgs _mouseHoverEventArgs;
        IUIEventListener _elem;
        public UIHoverMonitorTask()
            : base(null)
        {
            _mouseHoverEventArgs = new UIMouseHoverEventArgs();
        }
        public override void Reset()
        {
            _mouseMoveCounter = -1;
            _elem = null;
        }
        public void SetMonitorElement(IUIEventListener elem)
        {
            if (_elem != elem)
            {
                //reset count
                _mouseMoveCounter = -1;//reset
                _elem = elem;
            }
        }
        public override void InvokeAction()
        {
            if (_elem != null)
            {
                _mouseMoveCounter++;
                if (_mouseMoveCounter > 1)
                {
                    _mouseHoverEventArgs.CurrentContextElement = _elem;
                    _elem.ListenMouseHover(_mouseHoverEventArgs);
                }
            }
        }
    }
}