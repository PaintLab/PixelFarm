//MIT, 2020, WinterDev
using System;
namespace LayoutFarm.UI
{
    public class UIMouseBehaviour<S, T> : UIMouseBehaviourBase<S, T>
          where S : IAcceptBehviour
    {

        readonly MouseBehaviourInstance<S, T> _sharedInstance;
        public UIMouseBehaviour(T defaultSharedState = default)
        {
            //TODO:  _sharedInstance => should be replacable?  
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
    public class UIMouseBehaviour<S> : UIMouseBehaviourBase<S, object>
         where S : IAcceptBehviour
    {
        readonly MouseBehaviourInstance<S, object> _sharedInstance;
        public UIMouseBehaviour()
        {
            //TODO:  _sharedInstance => should be replacable?  
            _sharedInstance = new MouseBehaviourInstance<S, object>(this, null);
        }
        /// <summary>
        /// attach shared behavior instance to specific object
        /// </summary>
        /// <param name="acc"></param>
        public void AttachSharedBehaviorTo(S acc)
        {
            acc.AttachBehviour(_sharedInstance);
        }
    }

    /// <summary>
    /// behavior group for specific object S and State T
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class UIMouseBehaviourBase<S, T>
    where S : IAcceptBehviour
    {
        public struct SenderInfo
        {
            /// <summary>
            /// event source
            /// </summary>
            public S Source { get; internal set; }
            /// <summary>
            /// state, may be nothing
            /// </summary>
            public T State { get; internal set; }

            public SenderInfo(S source, T state)
            {
                Source = source;
                State = state;
            }
        }

        public event Action<SenderInfo, UIMouseDownEventArgs> MouseDown;
        public event Action<SenderInfo, UIMouseUpEventArgs> MouseUp;
        public event Action<SenderInfo, UIMouseMoveEventArgs> MouseMove;
        public event Action<SenderInfo, UIMouseMoveEventArgs> MouseDrag;
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

        /// <summary>
        /// has mouse-down
        /// </summary>
        internal bool HasMouseDown => MouseDown != null;
        internal void InvokeMouseDown(S sender, T state, UIMouseDownEventArgs e)
        {
            MouseDown.Invoke(new SenderInfo(sender, state), e);
        }
        /// <summary>
        /// has mouse-up
        /// </summary>
        internal bool HasMouseUp => MouseUp != null;
        internal void InvokeMouseUp(S sender, T state, UIMouseUpEventArgs e)
        {
            MouseUp?.Invoke(new SenderInfo(sender, state), e);
        }
        /// <summary>
        /// has mouse move subscription
        /// </summary>
        internal bool HasMouseMove => MouseMove != null;
        internal void InvokeMouseMove(S sender, T state, UIMouseMoveEventArgs e) => MouseMove.Invoke(new SenderInfo(sender, state), e);
        /// <summary>
        /// has mouse move subscription
        /// </summary>
        internal bool HasMouseDrag => MouseDrag != null;
        internal void InvokeMouseDrag(S sender, T state, UIMouseMoveEventArgs e) => MouseDrag.Invoke(new SenderInfo(sender, state), e);

        /// <summary>
        /// has mouse enter
        /// </summary>
        internal bool HasMouseEnter => MouseEnter != null;
        internal void InvokeMouseEnter(S sender, T state, UIMouseMoveEventArgs e) => MouseEnter.Invoke(new SenderInfo(sender, state), e);

        /// <summary>
        /// has mouse leave
        /// </summary>
        internal bool HasMouseLeave => MouseLeave != null;
        internal void InvokeMouseLeave(S sender, T state, UIMouseLeaveEventArgs e) => MouseLeave.Invoke(new SenderInfo(sender, state), e);

        internal bool HasMousePress => MousePress != null;
        internal void InvokeMousePress(S sender, T state, UIMousePressEventArgs e) => MousePress.Invoke(new SenderInfo(sender, state), e);

        internal bool HasMouseClick => MouseClick != null;
        internal void InvokeMouseClick(S sender, T state, UIMouseEventArgs e) => MouseClick.Invoke(new SenderInfo(sender, state), e);

        internal bool HasMouseDoubleClick => MouseDoubleClick != null;
        internal void InvokeMouseDoubleClick(S sender, T state, UIMouseEventArgs e) => MouseDoubleClick.Invoke(new SenderInfo(sender, state), e);

        internal bool HasGuestMsg => GuestMsg != null;
        internal void InvokeGuestMsg(S sender, T state, UIGuestMsgEventArgs e) => GuestMsg.Invoke(new SenderInfo(sender, state), e);

        internal bool HasMouseLostFocus => MouseLostFocus != null;
        internal void InvokeMouseLostFocus(S sender, T state, UIMouseLostFocusEventArgs e) => MouseLostFocus.Invoke(new SenderInfo(sender, state), e);

        internal bool HasMouseHover => MouseHover != null;
        internal void InvokeMouseHover(S sender, T state, UIMouseHoverEventArgs e) => MouseHover.Invoke(new SenderInfo(sender, state), e);



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
        readonly UIMouseBehaviourBase<S, T> _ownerBeh;
        T _state;
        public MouseBehaviourInstance(UIMouseBehaviourBase<S, T> ownerBeh, T state)
        {
#if DEBUG            
#endif
            _ownerBeh = ownerBeh;
            _state = state;
        }

        internal override void ListenGuestMsg(object sender, UIGuestMsgEventArgs e)
        {
            //since we don't want a 'blank round trip'
            //(invoke a blank action), so , => check before invocation
            if (_ownerBeh.HasGuestMsg) _ownerBeh.InvokeGuestMsg((S)sender, _state, e);
        }
        internal override void ListenLostMouseFocus(object sender, UIMouseLostFocusEventArgs e)
        {

            if (_ownerBeh.HasMouseLostFocus) _ownerBeh.InvokeMouseLostFocus((S)sender, _state, e);
        }
        internal override void ListenMouseDoubleClick(object sender, UIMouseEventArgs e)
        {

            if (_ownerBeh.HasMouseDoubleClick) _ownerBeh.InvokeMouseDoubleClick((S)sender, _state, e);
        }
        internal override void ListenMouseClick(object sender, UIMouseEventArgs e)
        {
            if (_ownerBeh.HasMouseClick) _ownerBeh.InvokeMouseClick((S)sender, _state, e);
        }
        internal override void ListenMouseDown(object sender, UIMouseDownEventArgs e)
        {
            if (_ownerBeh.HasMouseDown) _ownerBeh.InvokeMouseDown((S)sender, _state, e);
        }
        internal override void ListenMouseEnter(object sender, UIMouseMoveEventArgs e)
        {
            if (_ownerBeh.HasMouseEnter) _ownerBeh.InvokeMouseEnter((S)sender, _state, e);
        }
        internal override void ListenMouseLeave(object sender, UIMouseLeaveEventArgs e)
        {
            if (_ownerBeh.HasMouseLeave) _ownerBeh.InvokeMouseLeave((S)sender, _state, e);
        }
        internal override void ListenMouseMove(object sender, UIMouseMoveEventArgs e)
        {
            if (e.IsDragging)
            {
                if (_ownerBeh.HasMouseDrag)
                {
                    _ownerBeh.InvokeMouseDrag((S)sender, _state, e);
                }
            }
            else
            {
                if (_ownerBeh.HasMouseMove)
                {
                    _ownerBeh.InvokeMouseMove((S)sender, _state, e);
                }
            }

        }
        internal override void ListenMouseUp(object sender, UIMouseUpEventArgs e)
        {
            if (_ownerBeh.HasMouseUp) _ownerBeh.InvokeMouseUp((S)sender, _state, e);
        }
        internal override void ListenMousePress(object sender, UIMousePressEventArgs e)
        {
            if (_ownerBeh.HasMousePress) _ownerBeh.InvokeMousePress((S)sender, _state, e);
        }
        internal override void ListenMouseHover(object sender, UIMouseHoverEventArgs e)
        {
            if (_ownerBeh.HasMouseHover) _ownerBeh.InvokeMouseHover((S)sender, _state, e);
        }
    }




}
