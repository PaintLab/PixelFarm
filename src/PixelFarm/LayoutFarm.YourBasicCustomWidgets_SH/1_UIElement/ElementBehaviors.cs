//MIT, 2020, WinterDev
using System;
namespace LayoutFarm.UI
{

    /// <summary>
    /// behavior group for specific object S and State T
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class UIMouseBehaviour<S, T>
        where S : IAcceptBehviour
    {
        public class SenderInfo
        {
            /// <summary>
            /// event source
            /// </summary>
            public S Source { get; internal set; }
            /// <summary>
            /// state, may be nothing
            /// </summary>
            public T State { get; internal set; }
        }

        public event Action<SenderInfo, UIMouseDownEventArgs> MouseDown;
        public event Action<SenderInfo, UIMouseUpEventArgs> MouseUp;
        public event Action<SenderInfo, UIMouseMoveEventArgs> MouseMove;
        //----
        public event Action<SenderInfo, UIMouseMoveEventArgs> MouseEnter;
        public event Action<SenderInfo, UIMouseLeaveEventArgs> MouseLeave;
        public event Action<SenderInfo, UIMouseHoverEventArgs> MouseHover;
        public event Action<SenderInfo, UIMouseLostFocusEventArgs> MouseLostFocus;

        public event Action<SenderInfo, UIMousePressEventArgs> MousePress;
        public event Action<SenderInfo, UIMouseEventArgs> MouseClick;
        public event Action<SenderInfo, UIMouseEventArgs> MouseDoubleClick;
        public event Action<SenderInfo, UIGuestMsgEventArgs> GuestMsg;
        //----

        SenderInfo _senderInfo = new SenderInfo();
        internal void InvokeMouseDown(S sender, T state, UIMouseDownEventArgs e)
        {
            if (MouseDown != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseDown.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMouseUp(S sender, T state, UIMouseUpEventArgs e)
        {
            if (MouseUp != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseUp?.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMouseMove(S sender, T state, UIMouseMoveEventArgs e)
        {
            if (MouseMove != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseMove.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMouseEnter(S sender, T state, UIMouseMoveEventArgs e)
        {
            if (MouseEnter != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseEnter.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMouseLeave(S sender, T state, UIMouseLeaveEventArgs e)
        {
            if (MouseLeave != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseLeave.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMousePress(S sender, T state, UIMousePressEventArgs e)
        {
            if (MousePress != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MousePress.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMouseClick(S sender, T state, UIMouseEventArgs e)
        {
            if (MouseClick != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseClick.Invoke(_senderInfo, e);
            }
        }

        internal void InvokeMouseDoubleClick(S sender, T state, UIMouseEventArgs e)
        {
            if (MouseDoubleClick != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseDoubleClick.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeGuestMsg(S sender, T state, UIGuestMsgEventArgs e)
        {
            if (GuestMsg != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                GuestMsg.Invoke(_senderInfo, e);
            }
        }
        internal void InvokeMouseLostFocus(S sender, T state, UIMouseLostFocusEventArgs e)
        {
            if (MouseLostFocus != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseLostFocus.Invoke(_senderInfo, e);
            }

        }
        internal void InvokeMouseHover(S sender, T state, UIMouseHoverEventArgs e)
        {
            if (MouseHover != null)
            {
                _senderInfo.Source = sender;
                _senderInfo.State = state;
                MouseHover.Invoke(_senderInfo, e);
            }

        }


        MouseBehaviourInstance<S, T> _sharedInstance;
        public UIMouseBehaviour(T defaultSharedState = default)
        {
            _sharedInstance = new MouseBehaviourInstance<S, T>(this, defaultSharedState);
        }

        /// <summary>
        /// attach shared behavior instance to specific object
        /// </summary>
        /// <param name="acc"></param>
        public void AttachSharedBehaviorTo(S acc)
        {
            acc.AttachBehviour(_sharedInstance);
        }
        /// <summary>
        /// attach new behaviour instance+ its state to specific object
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="state"></param>
        public void AttachUniqueBehaviorTo(S acc, T state)
        {
            acc.AttachBehviour(new MouseBehaviourInstance<S, T>(this, state));
        }
    }

    public abstract class MouseBehaviorInstanceBase
    {
        internal abstract void ListenGuestMsg(object sender, UIGuestMsgEventArgs e);
        internal abstract void ListenLostMouseFocus(object sender, UIMouseLostFocusEventArgs e);
        internal abstract void ListenMouseDoubleClick(object sender, UIMouseEventArgs e);
        internal abstract void ListenMouseClick(object sender, UIMouseEventArgs e);
        internal abstract void ListenMouseDown(object sender, UIMouseDownEventArgs e);
        internal abstract void ListenMouseEnter(object sender, UIMouseMoveEventArgs e);
        internal abstract void ListenMouseLeave(object sender, UIMouseLeaveEventArgs e);
        internal abstract void ListenMouseMove(object sender, UIMouseMoveEventArgs e);
        internal abstract void ListenMouseUp(object sender, UIMouseUpEventArgs e);
        internal abstract void ListenMousePress(object sender, UIMousePressEventArgs e);
        internal abstract void ListenMouseHover(object sender, UIMouseHoverEventArgs e);
    }

    public interface IAcceptBehviour
    {
        bool AttachBehviour(MouseBehaviorInstanceBase inst);
    }


    public class MouseBehaviourInstance<S, T> : MouseBehaviorInstanceBase
        where S : IAcceptBehviour
    {
        readonly UIMouseBehaviour<S, T> _ownerBeh;
        T _state;
        public MouseBehaviourInstance(UIMouseBehaviour<S, T> ownerBeh, T state)
        {
#if DEBUG            
#endif
            _ownerBeh = ownerBeh;
            _state = state;
        }

        internal override void ListenGuestMsg(object sender, UIGuestMsgEventArgs e)
        {
            _ownerBeh.InvokeGuestMsg((S)sender, _state, e);
        }
        internal override void ListenLostMouseFocus(object sender, UIMouseLostFocusEventArgs e)
        {
            _ownerBeh.InvokeMouseLostFocus((S)sender, _state, e);
        }
        internal override void ListenMouseDoubleClick(object sender, UIMouseEventArgs e)
        {
            _ownerBeh.InvokeMouseDoubleClick((S)sender, _state, e);
        }
        internal override void ListenMouseClick(object sender, UIMouseEventArgs e)
        {
            _ownerBeh.InvokeMouseClick((S)sender, _state, e);
        }
        internal override void ListenMouseDown(object sender, UIMouseDownEventArgs e)
        {
            _ownerBeh.InvokeMouseDown((S)sender, _state, e);
        }
        internal override void ListenMouseEnter(object sender, UIMouseMoveEventArgs e)
        {
            _ownerBeh.InvokeMouseEnter((S)sender, _state, e);
        }
        internal override void ListenMouseLeave(object sender, UIMouseLeaveEventArgs e)
        {
            _ownerBeh.InvokeMouseLeave((S)sender, _state, e);
        }
        internal override void ListenMouseMove(object sender, UIMouseMoveEventArgs e)
        {
            _ownerBeh.InvokeMouseMove((S)sender, _state, e);
        }
        internal override void ListenMouseUp(object sender, UIMouseUpEventArgs e)
        {
            _ownerBeh.InvokeMouseUp((S)sender, _state, e);
        }
        internal override void ListenMousePress(object sender, UIMousePressEventArgs e)
        {
            _ownerBeh.InvokeMousePress((S)sender, _state, e);
        }
        internal override void ListenMouseHover(object sender, UIMouseHoverEventArgs e)
        {
            _ownerBeh.InvokeMouseHover((S)sender, _state, e);
        }
    }
}
