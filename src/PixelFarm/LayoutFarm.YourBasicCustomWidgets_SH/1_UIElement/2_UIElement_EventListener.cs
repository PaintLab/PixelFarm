//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{

    partial class UIElement
    {
        //special listener for appearance of of the UIElement
        IUIElementBehaviour _uiElemBeh;
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
        public bool AttachUIBehaviour(IUIElementBehaviour behListener)
        {
            if (behListener == this)
                throw new System.Exception("recursive!");

            if (behListener == null)
            {
                _uiElemBeh = null;
                return false;
            }
            //--------------------------------------------------------
            if (_uiElemBeh == null)
            {
                _uiElemBeh = behListener;
                return true;
            }
            else
            {
#if DEBUG
                //please clear the old listener before set a new one.
                //in this version a single AttachUIBehaviour has 1 external event listener
                System.Diagnostics.Debugger.Break();
#endif

                return false;
            }
        }


        void IEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            OnKeyPress(e);
            _externalEventListener?.ListenKeyPress(e);
            _uiElemBeh?.ListenKeyPress(e);
        }
        void IEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            OnKeyDown(e);
            _externalEventListener?.ListenKeyDown(e);
            _uiElemBeh?.ListenKeyDown(e);
        }
        void IEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            OnKeyUp(e);
            _externalEventListener?.ListenKeyUp(e);
            _uiElemBeh?.ListenKeyUp(e);
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
            _uiElemBeh?.ListenMouseDown(e);
        }

        void IEventListener.ListenMouseMove(UIMouseMoveEventArgs e)
        {
            OnMouseMove(e);
            _externalEventListener?.ListenMouseMove(e);
            _uiElemBeh?.ListenMouseMove(e);
        }
        void IEventListener.ListenMouseUp(UIMouseUpEventArgs e)
        {
            OnMouseUp(e);
            _externalEventListener?.ListenMouseUp(e);
            _uiElemBeh?.ListenMouseUp(e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IEventListener.ListenMousePress(UIMousePressEventArgs e)
        {
            OnMousePress(e);
            _externalEventListener?.ListenMousePress(e);
            _uiElemBeh?.ListenMousePress(e);
        }
        void IEventListener.ListenLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            OnLostMouseFocus(e);
            _externalEventListener?.ListenLostMouseFocus(e);
            _uiElemBeh?.ListenLostMouseFocus(e);
        }

        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            OnDoubleClick(e);
            _externalEventListener?.ListenMouseDoubleClick(e);
            _uiElemBeh?.ListenMouseDoubleClick(e);
        }
        void IEventListener.ListenMouseWheel(UIMouseWheelEventArgs e)
        {
            OnMouseWheel(e);
            _externalEventListener?.ListenMouseWheel(e);
            _uiElemBeh?.ListenMouseWheel(e);
        }
        void IEventListener.ListenMouseEnter(UIMouseMoveEventArgs e)
        {
            OnMouseEnter(e);
            _externalEventListener?.ListenMouseEnter(e);
            _uiElemBeh?.ListenMouseEnter(e);
        }
        void IEventListener.ListenMouseHover(UIMouseHoverEventArgs e)
        {
            OnMouseHover(e);
            _externalEventListener?.ListenMouseHover(e);
            _uiElemBeh?.ListenMouseHover(e);
        }
        void IEventListener.ListenMouseLeave(UIMouseLeaveEventArgs e)
        {
            OnMouseLeave(e);
            _externalEventListener?.ListenMouseLeave(e);
            _uiElemBeh?.ListenMouseLeave(e);
        }
        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {
            OnGotKeyboardFocus(e);
            _externalEventListener?.ListenGotKeyboardFocus(e);
            _uiElemBeh?.ListenGotKeyboardFocus(e);
        }
        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostKeyboardFocus(e);
            _externalEventListener?.ListenLostKeyboardFocus(e);
            _uiElemBeh?.ListenLostKeyboardFocus(e);
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


    public interface IUIElementBehaviour : IEventListener
    {
    }

    public delegate void UIBehEventHandler<T>(object b, T e);


    public class GeneralUIElementBehaviour : IUIElementBehaviour
    {
        public event UIBehEventHandler<UIMouseDownEventArgs> MouseDown;
        public event UIBehEventHandler<UIMouseUpEventArgs> MouseUp;
        public event UIBehEventHandler<UIMouseMoveEventArgs> MouseMove;

        //----
        public event UIBehEventHandler<UIMouseMoveEventArgs> MouseEnter;
        public event UIBehEventHandler<UIMouseLeaveEventArgs> MouseLeave;

        public event UIBehEventHandler<UIMousePressEventArgs> MousePress;
        public event UIBehEventHandler<UIMouseHoverEventArgs> MouseHover;
        //----

        public event UIBehEventHandler<UIKeyEventArgs> KeyDown;
        public event UIBehEventHandler<UIKeyEventArgs> KeyPress;
        public event UIBehEventHandler<UIKeyEventArgs> KeyUp;


        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e) => OnGotKeyboardFocus(e);
        void IEventListener.ListenGuestMsg(UIGuestMsgEventArgs e) => OnGuestMsg(e);
        void IEventListener.ListenKeyDown(UIKeyEventArgs e) => OnKeyDown(e);
        void IEventListener.ListenKeyPress(UIKeyEventArgs e) => OnKeyPress(e);
        void IEventListener.ListenKeyUp(UIKeyEventArgs e) => OnKeyUp(e);

        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e) => OnLostKeyboardFocus(e);

        void IEventListener.ListenLostMouseFocus(UIMouseLostFocusEventArgs e)
        {

        }
        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {

        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IEventListener.ListenMouseDown(UIMouseDownEventArgs e) => OnMouseDown(e);

        void IEventListener.ListenMouseEnter(UIMouseMoveEventArgs e) => OnMouseEnter(e);
        void IEventListener.ListenMouseLeave(UIMouseLeaveEventArgs e) => OnMouseLeave(e);

        void IEventListener.ListenMouseHover(UIMouseHoverEventArgs e) => OnMouseHover(e);


        void IEventListener.ListenMouseMove(UIMouseMoveEventArgs e) => OnMouseMove(e);
        void IEventListener.ListenMouseUp(UIMouseUpEventArgs e) => OnMouseUp(e);

        void IEventListener.ListenMousePress(UIMousePressEventArgs e) => OnMousePress(e);
        void IEventListener.ListenMouseWheel(UIMouseWheelEventArgs e) => OnMouseWheel(e);

        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs args)
        {
            return false;
        }
        //--------------------------------------------------
        protected virtual void OnGotKeyboardFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnGuestMsg(UIGuestMsgEventArgs e)
        {

        }
        protected virtual void OnKeyDown(UIKeyEventArgs e) => KeyDown?.Invoke(this, e);

        protected virtual void OnKeyPress(UIKeyEventArgs e) => KeyPress?.Invoke(this, e);
        protected virtual void OnKeyUp(UIKeyEventArgs e) => KeyUp?.Invoke(this, e);

        protected virtual void OnLostKeyboardFocus(UIFocusEventArgs e) { }
        protected virtual void OnLostMouseFocus(UIMouseLostFocusEventArgs e) { }

        protected virtual void OnMouseWheel(UIMouseWheelEventArgs e) { }

        protected virtual void OnMouseMove(UIMouseMoveEventArgs e) => MouseMove?.Invoke(this, e);
        protected virtual void OnMouseUp(UIMouseUpEventArgs e) => MouseUp?.Invoke(this, e);
        protected virtual void OnMouseDown(UIMouseDownEventArgs e) => MouseDown?.Invoke(this, e);

        protected virtual void OnMouseEnter(UIMouseMoveEventArgs e) => MouseEnter?.Invoke(this, e);
        protected virtual void OnMouseLeave(UIMouseLeaveEventArgs e) => MouseLeave?.Invoke(this, e);

        protected virtual void OnMouseHover(UIMouseHoverEventArgs e) => MouseHover?.Invoke(this, e);

        protected virtual void OnMousePress(UIMousePressEventArgs e) => MousePress?.Invoke(this, e);
    }

}