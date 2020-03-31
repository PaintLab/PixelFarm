//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.UI
{

    public class UIMouseBehaviour<T>
    {
        public event Action<T, UIMouseDownEventArgs> MouseDown;
        public event Action<T, UIMouseUpEventArgs> MouseUp;
        public event Action<T, UIMouseMoveEventArgs> MouseMove;
        //----
        public event Action<T, UIMouseMoveEventArgs> MouseEnter;
        public event Action<T, UIMouseLeaveEventArgs> MouseLeave;
        public event Action<T, UIMouseHoverEventArgs> MouseHover;
        public event Action<T, UIMouseLostFocusEventArgs> MouseLostFocus;

        public event Action<T, UIMousePressEventArgs> MousePress;
        public event Action<T, UIMouseEventArgs> MouseClick;
        public event Action<T, UIMouseEventArgs> MouseDoubleClick;
        public event Action<T, UIGuestMsgEventArgs> GuestMsg;
        //----

        internal void InvokeMouseDown(T state, UIMouseDownEventArgs e)
        {
            MouseDown?.Invoke(state, e);
        }
        internal void InvokeMouseUp(T state, UIMouseUpEventArgs e)
        {
            MouseUp?.Invoke(state, e);
        }
        internal void InvokeMouseMove(T state, UIMouseMoveEventArgs e)
        {
            MouseMove?.Invoke(state, e);
        }
        internal void InvokeMouseEnter(T state, UIMouseMoveEventArgs e)
        {
            MouseEnter?.Invoke(state, e);
        }
        internal void InvokeMouseLeave(T state, UIMouseLeaveEventArgs e)
        {
            MouseLeave?.Invoke(state, e);
        }
        internal void InvokeMousePress(T state, UIMousePressEventArgs e)
        {
            MousePress?.Invoke(state, e);
        }
        internal void InvokeMouseClick(T state, UIMouseEventArgs e)
        {
            MouseClick?.Invoke(state, e);
        }
        internal void InvokeMouseDoubleClick(T state, UIMouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(state, e);
        }
        internal void InvokeGuestMsg(T state, UIGuestMsgEventArgs e)
        {
            GuestMsg?.Invoke(state, e);
        }
        internal void InvokeMouseLostFocus(T state, UIMouseLostFocusEventArgs e)
        {
            MouseLostFocus?.Invoke(state, e);
        }
        internal void InvokeMouseHover(T state, UIMouseHoverEventArgs e)
        {
            MouseHover?.Invoke(state, e);
        }


        MouseBehaviourInstance<T> _sharedInstance;
        public UIMouseBehaviour()
        {
            _sharedInstance = new MouseBehaviourInstance<T>(this);
        }
        public MouseBehaviourInstance<T> CreateBehaviourInstance(T state)
        {
            return new MouseBehaviourInstance<T>(this, state);
        }

        public MouseBehaviourInstance<T> GetSharedInstance() => _sharedInstance;
    }


    public abstract class MouseBehaviourInstanceBase
    {
        internal abstract void ListenGuestMsg(UIGuestMsgEventArgs e);
        internal abstract void ListenLostMouseFocus(UIMouseLostFocusEventArgs e);
        internal abstract void ListenMouseDoubleClick(UIMouseEventArgs e);
        internal abstract void ListenMouseClick(UIMouseEventArgs e);
        internal abstract void ListenMouseDown(UIMouseDownEventArgs e);
        internal abstract void ListenMouseEnter(UIMouseMoveEventArgs e);
        internal abstract void ListenMouseLeave(UIMouseLeaveEventArgs e);
        internal abstract void ListenMouseMove(UIMouseMoveEventArgs e);
        internal abstract void ListenMouseUp(UIMouseUpEventArgs e);
        internal abstract void ListenMousePress(UIMousePressEventArgs e);
        internal abstract void ListenMouseHover(UIMouseHoverEventArgs e);
    }


    public class MouseBehaviourInstance<T> : MouseBehaviourInstanceBase
    {
        readonly UIMouseBehaviour<T> _ownerBeh;
        T _state;
        public MouseBehaviourInstance(UIMouseBehaviour<T> ownerBeh, T state)
        {
#if DEBUG            
#endif
            _ownerBeh = ownerBeh;
            _state = state;
        }
        public MouseBehaviourInstance(UIMouseBehaviour<T> ownerBeh)
        {
            _ownerBeh = ownerBeh;
        }
        internal override void ListenGuestMsg(UIGuestMsgEventArgs e)
        {
            _ownerBeh.InvokeGuestMsg(_state, e);
        }
        internal override void ListenLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            _ownerBeh.InvokeMouseLostFocus(_state, e);
        }
        internal override void ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            _ownerBeh.InvokeMouseDoubleClick(_state, e);
        }
        internal override void ListenMouseClick(UIMouseEventArgs e)
        {
            _ownerBeh.InvokeMouseClick(_state, e);
        }
        internal override void ListenMouseDown(UIMouseDownEventArgs e)
        {
            _ownerBeh.InvokeMouseDown(_state, e);
        }
        internal override void ListenMouseEnter(UIMouseMoveEventArgs e)
        {
            _ownerBeh.InvokeMouseEnter(_state, e);
        }
        internal override void ListenMouseLeave(UIMouseLeaveEventArgs e)
        {
            _ownerBeh.InvokeMouseLeave(_state, e);
        }
        internal override void ListenMouseMove(UIMouseMoveEventArgs e)
        {
            _ownerBeh.InvokeMouseMove(_state, e);
        }
        internal override void ListenMouseUp(UIMouseUpEventArgs e)
        {
            _ownerBeh.InvokeMouseUp(_state, e);
        }
        internal override void ListenMousePress(UIMousePressEventArgs e)
        {
            _ownerBeh.InvokeMousePress(_state, e);
        }
        internal override void ListenMouseHover(UIMouseHoverEventArgs e)
        {
            _ownerBeh.InvokeMouseHover(_state, e);
        }
    }
}
