//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class GeneralEventListener : IEventListener
    {
        public event EventHandler<UIMouseDownEventArgs> MouseDown;
        public event EventHandler<UIMouseUpEventArgs> MouseUp;
        public event EventHandler<UIMouseMoveEventArgs> MouseMove;
        public event EventHandler<UIMouseMoveEventArgs> MouseDrag;
        public event EventHandler<UIMouseEventArgs> MouseDoubleClick;

        public event EventHandler<UIMouseMoveEventArgs> MouseEnter;
        public event EventHandler<UIMouseLeaveEventArgs> MouseLeave;
        public event EventHandler<UIMouseHoverEventArgs> MouseHover;

        public event EventHandler<UIKeyEventArgs> KeyDown;
        public event EventHandler<UIKeyEventArgs> KeyPress;
        public event EventHandler<UIKeyEventArgs> KeyUp;
        public event EventHandler<UIMousePressEventArgs> MousePress;
        public event EventHandler<UIMouseWheelEventArgs> MouseWheel;

        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {
        }
        void IEventListener.ListenGuestMsg(UIGuestMsgEventArgs e)
        {

        }

        void IEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        void IEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        void IEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {

        }

        void IEventListener.ListenLostMouseFocus(UIMouseLostFocusEventArgs e)
        {

        }


        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        void IEventListener.ListenMouseDown(UIMouseDownEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }
        void IEventListener.ListenMouseEnter(UIMouseMoveEventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }
        void IEventListener.ListenMouseLeave(UIMouseLeaveEventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }
        void IEventListener.ListenMouseHover(UIMouseHoverEventArgs e)
        {
            MouseHover?.Invoke(this, e);
        }
        void IEventListener.ListenMouseMove(UIMouseMoveEventArgs e)
        {
            if (e.IsDragging)
            {
                MouseDrag?.Invoke(this, e);
            }
            else
            {
                MouseMove?.Invoke(this, e);
            }
        }

        void IEventListener.ListenMouseUp(UIMouseUpEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IEventListener.ListenMouseWheel(UIMouseWheelEventArgs e) => MouseWheel?.Invoke(this, e);

        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs args)
        {
            return false;
        }
        void IEventListener.ListenMousePress(UIMousePressEventArgs e)
        {
            MousePress?.Invoke(this, e);
        }
    }

}