//Apache2, 2014-present, WinterDev


namespace LayoutFarm.UI
{
    public class GeneralEventListener : IEventListener
    {
        public event UIEventHandler<UIMouseEventArgs> MouseDown;
        public event UIEventHandler<UIMouseEventArgs> MouseUp;
        public event UIEventHandler<UIMouseEventArgs> MouseMove;
        public event UIEventHandler<UIKeyEventArgs> KeyDown;
        public event UIEventHandler<UIKeyEventArgs> KeyPress;
        public event UIEventHandler<UIKeyEventArgs> KeyUp;

        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {

        }

        void IEventListener.ListenGuestTalk(UIGuestTalkEventArgs e)
        {

        }

        void IEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
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

        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            MouseDown?.Invoke(e);
        }

        void IEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {

        }

        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            MouseMove?.Invoke(e);
        }

        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            MouseUp?.Invoke(e);
        }
        void IEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {

        }
        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs args)
        {
            return false;
        }
    }

}