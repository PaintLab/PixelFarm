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

            MouseCursorStyle MouseCursorStyle { get; }
            void RootMouseDown(UIMouseEventArgs mouseEventArgs);
            void RootMouseUp(UIMouseEventArgs mouseEventArgs);
            void RootMouseWheel(UIMouseEventArgs mouseEventArgs);
            void RootMouseMove(UIMouseEventArgs mouseEventArgs);
            //
            void RootGotFocus();
            void RootLostFocus();
            //
            void RootKeyPress(char c);
            void RootKeyDown(int keydata);
            void RootKeyUp(int keydata);
            bool RootProcessDialogKey(int keydata);
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