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
        void IEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            OnMouseDown(e);
            _externalEventListener?.ListenMouseDown(e);
            _uiElemBeh?.ListenMouseDown(e);
        }
        void IEventListener.ListenContinuousMousePress()
        {
            OnContinuousMousePress();
            _externalEventListener?.ListenContinuousMousePress();
            _uiElemBeh?.ListenContinuousMousePress();
        }
        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
            _externalEventListener?.ListenMouseMove(e);
            _uiElemBeh?.ListenMouseMove(e);
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            OnMouseUp(e);
            _externalEventListener?.ListenMouseUp(e);
            _uiElemBeh?.ListenMouseUp(e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {

        }
        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
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
        void IEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {
            OnMouseWheel(e);
            _externalEventListener?.ListenMouseWheel(e);
            _uiElemBeh?.ListenMouseWheel(e);
        }
        void IEventListener.ListenMouseLeave(UIMouseEventArgs e)
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
        public event UIBehEventHandler<UIMouseEventArgs> MouseDown;
        public event UIBehEventHandler<UIMouseEventArgs> MouseUp;
        public event UIBehEventHandler<UIMouseEventArgs> MouseMove;
        public event UIBehEventHandler<UIMouseEventArgs> MouseLeave;

        public event UIBehEventHandler<UIKeyEventArgs> KeyDown;
        public event UIBehEventHandler<UIKeyEventArgs> KeyPress;
        public event UIBehEventHandler<UIKeyEventArgs> KeyUp;

        public event UIBehEventHandler<System.EventArgs> ContinuosMousePress;

        void IEventListener.ListenContinuousMousePress() => OnContinuosMousePress();
        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e) => OnGotKeyboardFocus(e);
        void IEventListener.ListenGuestMsg(UIGuestMsgEventArgs e) => OnGuestMsg(e);
        void IEventListener.ListenKeyDown(UIKeyEventArgs e) => OnKeyDown(e);
        void IEventListener.ListenKeyPress(UIKeyEventArgs e) => OnKeyPress(e);
        void IEventListener.ListenKeyUp(UIKeyEventArgs e) => OnKeyUp(e);

        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e) => OnLostKeyboardFocus(e);

        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseDown(UIMouseEventArgs e) => OnMouseDown(e);
        void IEventListener.ListenMouseLeave(UIMouseEventArgs e) => OnMouseLeave(e);
        void IEventListener.ListenMouseMove(UIMouseEventArgs e) => OnMouseMove(e);
        void IEventListener.ListenMouseUp(UIMouseEventArgs e) => OnMouseUp(e);
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }

        void IEventListener.ListenMouseWheel(UIMouseEventArgs e) => OnMouseWheel(e);
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
        protected virtual void OnLostMouseFocus(UIMouseEventArgs e) { }
        protected virtual void OnMouseWheel(UIMouseEventArgs e) { }
        protected virtual void OnMouseMove(UIMouseEventArgs e) => MouseMove?.Invoke(this, e);
        protected virtual void OnMouseUp(UIMouseEventArgs e) => MouseUp?.Invoke(this, e);
        protected virtual void OnMouseDown(UIMouseEventArgs e) => MouseDown?.Invoke(this, e);
        protected virtual void OnMouseLeave(UIMouseEventArgs e) => MouseLeave?.Invoke(this, e);

        protected virtual void OnContinuosMousePress() => ContinuosMousePress?.Invoke(this, System.EventArgs.Empty);
    }

}