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
        void IUIEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            OnKeyPress(e);
            _externalEventListener?.ListenKeyPress(e);
        }
        void IUIEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            OnKeyDown(e);
            _externalEventListener?.ListenKeyDown(e);
        }
        void IUIEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            OnKeyUp(e);
            _externalEventListener?.ListenKeyUp(e);
        }
        bool IUIEventListener.ListenProcessDialogKey(UIKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IUIEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            OnMouseDown(e);
            _externalEventListener?.ListenMouseDown(e);
        }
        void IUIEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
            _externalEventListener?.ListenMouseMove(e);
        }
        void IUIEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            OnMouseUp(e);
            _externalEventListener?.ListenMouseUp(e);
        }
        void IUIEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {
            OnLostMouseFocus(e);
            _externalEventListener?.ListenLostMouseFocus(e);
        }
        void IUIEventListener.ListenMouseClick(UIMouseEventArgs e)
        {

        }
        void IUIEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            OnDoubleClick(e);
            _externalEventListener?.ListenMouseDoubleClick(e);
        }
        void IUIEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {
            OnMouseWheel(e);
            _externalEventListener?.ListenMouseWheel(e);
        }
        void IUIEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {
            OnMouseLeave(e);
            _externalEventListener?.ListenMouseLeave(e);
        }
        void IUIEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {
            OnGotKeyboardFocus(e);
            _externalEventListener?.ListenGotKeyboardFocus(e);
        }
        void IUIEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostKeyboardFocus(e);
            _externalEventListener?.ListenLostKeyboardFocus(e);
        }
        void IUIEventListener.HandleContentLayout()
        {
            OnContentLayout();
        }
        void IUIEventListener.HandleContentUpdate()
        {
            OnContentUpdate();
        }
        void IUIEventListener.HandleElementUpdate()
        {
            OnElementChanged();
        }
        bool IUIEventListener.BypassAllMouseEvents
        {
            get
            {
                return this.TransparentAllMouseEvents;
            }
        }
        bool IUIEventListener.AutoStopMouseEventPropagation
        {
            get
            {
                return this.AutoStopMouseEventPropagation;
            }
        }
        void IUIEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
        {
            this.OnInterComponentMsg(sender, msgcode, msg);
        }

        void IUIEventListener.ListenGuestTalk(UIGuestTalkEventArgs e)
        {
            this.OnGuestTalk(e);
        }
        void IUIEventListener.GetGlobalLocation(out int x, out int y)
        {
            var globalLoca = this.CurrentPrimaryRenderElement.GetGlobalLocation();
            x = globalLoca.X;
            y = globalLoca.Y;
        }
    }
}