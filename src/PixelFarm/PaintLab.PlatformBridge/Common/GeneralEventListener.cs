//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class GeneralEventListener : IEventListener
    {
        public event UIEventHandler<UIMouseEventArgs> MouseDown;
        public event UIEventHandler<UIMouseEventArgs> MouseUp;
        public event UIEventHandler<UIMouseEventArgs> MouseMove;
        public event UIEventHandler<UIMouseEventArgs> MouseDoubleClick;

        public event UIEventHandler<UIMouseEventArgs> MouseEnter;
        public event UIEventHandler<UIMouseEventArgs> MouseLeave;
        public event UIEventHandler<UIMouseHoverEventArgs> MouseHover;

        public event UIEventHandler<UIKeyEventArgs> KeyDown;
        public event UIEventHandler<UIKeyEventArgs> KeyPress;
        public event UIEventHandler<UIKeyEventArgs> KeyUp;
        public event UIEventHandler<UIMousePressEventArgs> ContinuousMousePress;

        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {

        }

        void IEventListener.ListenGuestMsg(UIGuestMsgEventArgs e)
        {

        }

        void IEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            KeyDown?.Invoke(e);
        }

        void IEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            KeyPress?.Invoke(e);
        }

        void IEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            KeyUp?.Invoke(e);
        }

        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {

        }

        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {

        }


        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(e);
        }

        void IEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            MouseDown?.Invoke(e);
        }
        void IEventListener.ListenMouseEnter(UIMouseEventArgs e)
        {
            MouseEnter?.Invoke(e);
        }
        void IEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {
            MouseLeave?.Invoke(e);
        }
        void IEventListener.ListenMouseHover(UIMouseHoverEventArgs e)
        {
            MouseHover?.Invoke(e);
        }
        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            MouseMove?.Invoke(e);
        }

        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            MouseUp?.Invoke(e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {

        }
        void IEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {

        }
        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs args)
        {
            return false;
        }
        void IEventListener.ListenMousePress(UIMousePressEventArgs e)
        {
            ContinuousMousePress?.Invoke(e);
        }
    }

}