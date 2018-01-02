//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.UI
{
    partial class UIElement
    {
        IEventListener _externalEventListener;
        public bool AttachExternalEventListener(IEventListener externalEventListener)
        {
            if (_externalEventListener == null)
            {
                this._externalEventListener = externalEventListener;
                return true;
            }
            else
            {
                return false;
            }
        }
        void IEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            OnKeyPress(e);
            _externalEventListener?.ListenKeyPress(e);
        }
        void IEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            OnKeyDown(e);
            _externalEventListener?.ListenKeyDown(e);
        }
        void IEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            OnKeyUp(e);
            _externalEventListener?.ListenKeyUp(e);
        }
        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            OnMouseDown(e);
            _externalEventListener?.ListenMouseDown(e);
        }
        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
            _externalEventListener?.ListenMouseMove(e);
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            OnMouseUp(e);
            _externalEventListener?.ListenMouseUp(e);
        }
        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {
            OnLostMouseFocus(e);
            _externalEventListener?.ListenLostMouseFocus(e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {

        }
        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            OnDoubleClick(e);
            _externalEventListener?.ListenMouseDoubleClick(e);
        }
        void IEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {
            OnMouseWheel(e);
            _externalEventListener?.ListenMouseWheel(e);
        }
        void IEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {
            OnMouseLeave(e);
            _externalEventListener?.ListenMouseLeave(e);
        }
        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {
            OnGotKeyboardFocus(e);
            _externalEventListener?.ListenGotKeyboardFocus(e);
        }
        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostKeyboardFocus(e);
            _externalEventListener?.ListenLostKeyboardFocus(e);
        }
        void IEventListener.HandleContentLayout()
        {
            OnContentLayout();
        }
        void IEventListener.HandleContentUpdate()
        {
            OnContentUpdate();
        }
        void IEventListener.HandleElementUpdate()
        {
            OnElementChanged();
        }
        bool IEventListener.BypassAllMouseEvents
        {
            get
            {
                return this.TransparentAllMouseEvents;
            }
        }
        bool IEventListener.AutoStopMouseEventPropagation
        {
            get
            {
                return this.AutoStopMouseEventPropagation;
            }
        }
        void IEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
        {
            this.OnInterComponentMsg(sender, msgcode, msg);
        }

        void IEventListener.ListenGuestTalk(UIGuestTalkEventArgs e)
        {
            this.OnGuestTalk(e);
        }
        void IEventListener.GetGlobalLocation(out int x, out int y)
        {
            var globalLoca = this.CurrentPrimaryRenderElement.GetGlobalLocation();
            x = globalLoca.X;
            y = globalLoca.Y;
        }
    }
}