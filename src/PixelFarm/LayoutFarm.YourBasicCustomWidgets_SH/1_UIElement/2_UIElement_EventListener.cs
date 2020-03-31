//Apache2, 2014-present, WinterDev
using System;

namespace LayoutFarm.UI
{

    partial class UIElement
    {
        IEventListener _externalEventListener;
        public bool AttachExternalEventListener(IEventListener externalEventListener)
        {
            if (externalEventListener == this)
                throw new System.Exception("recursive!");

            if (externalEventListener == null)
            {
                _externalEventListener = null;
                return false;
            }
            //--------------------------------------------------------
            if (_externalEventListener == null)
            {
                _externalEventListener = externalEventListener;
                return true;
            }
            else
            {
#if DEBUG
                //please clear the old listener before set a new one.
                //in this version a single UIElement has 1 external event listener
                System.Diagnostics.Debugger.Break();
#endif

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
            //TODO: review this, no external event or beh for this?
            return OnProcessDialogKey(e);
        }
        void IEventListener.ListenMouseDown(UIMouseDownEventArgs e)
        {
            OnMouseDown(e);
            _externalEventListener?.ListenMouseDown(e);
        }

        void IEventListener.ListenMouseMove(UIMouseMoveEventArgs e)
        {
            OnMouseMove(e);
            _externalEventListener?.ListenMouseMove(e);

        }
        void IEventListener.ListenMouseUp(UIMouseUpEventArgs e)
        {
            OnMouseUp(e);
            _externalEventListener?.ListenMouseUp(e);

        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IEventListener.ListenMousePress(UIMousePressEventArgs e)
        {
            OnMousePress(e);
            _externalEventListener?.ListenMousePress(e);

        }
        void IEventListener.ListenLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            OnLostMouseFocus(e);
            _externalEventListener?.ListenLostMouseFocus(e);

        }

        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            OnDoubleClick(e);
            _externalEventListener?.ListenMouseDoubleClick(e);

        }
        void IEventListener.ListenMouseWheel(UIMouseWheelEventArgs e)
        {
            OnMouseWheel(e);
            _externalEventListener?.ListenMouseWheel(e);
        }
        void IEventListener.ListenMouseEnter(UIMouseMoveEventArgs e)
        {
            OnMouseEnter(e);
            _externalEventListener?.ListenMouseEnter(e); 
        }
        void IEventListener.ListenMouseHover(UIMouseHoverEventArgs e)
        {
            OnMouseHover(e);
            _externalEventListener?.ListenMouseHover(e);
           
        }
        void IEventListener.ListenMouseLeave(UIMouseLeaveEventArgs e)
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

        bool IUIEventListener.BypassAllMouseEvents => this.TransparentForMouseEvents;


        bool IUIEventListener.AutoStopMouseEventPropagation => this.AutoStopMouseEventPropagation;


        void IEventListener.ListenGuestMsg(UIGuestMsgEventArgs e)
        {
            this.OnGuestMsg(e);
        }
        void IUIEventListener.GetGlobalLocation(out int x, out int y)
        {
            var globalLoca = this.CurrentPrimaryRenderElement.GetGlobalLocation();
            x = globalLoca.X;
            y = globalLoca.Y;
        }
    }



}