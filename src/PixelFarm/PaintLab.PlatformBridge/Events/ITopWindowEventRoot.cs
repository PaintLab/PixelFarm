//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    namespace InputBridge
    {
        public interface ITopWindowEventRoot
        {
            //---------------
            //bridge from platform-specific window event
            //to our abstract windows
            //---------------            
            void RootMouseDown(UIMouseEventArgs mouseEventArgs);
            void RootMouseUp(UIMouseEventArgs mouseEventArgs);
            void RootMouseWheel(UIMouseEventArgs mouseEventArgs);
            void RootMouseMove(UIMouseEventArgs mouseEventArgs);
            //
            void RootGotFocus(UIFocusEventArgs e);
            void RootLostFocus(UIFocusEventArgs e);
            //
            void RootKeyPress(UIKeyEventArgs key);
            void RootKeyDown(UIKeyEventArgs key);
            void RootKeyUp(UIKeyEventArgs key);
            bool RootProcessDialogKey(UIKeyEventArgs key);

            //TODO: touch...
        }
        public interface ITopWindowEventRootProvider
        {
            ITopWindowEventRoot EventRoot { get; }
        }
    }



    public enum UIMouseButtons
    {
        Left,
        Right,
        Middle,
        None
    }

}