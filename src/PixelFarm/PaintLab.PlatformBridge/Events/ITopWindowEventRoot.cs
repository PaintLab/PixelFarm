//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{

    public interface ITopWindowEventRoot
    {
        //---------------
        //bridge from platform-specific window event
        //to our abstract windows
        //---------------            
        void RootMouseDown(PrimaryMouseEventArgs mouseEventArgs);
        void RootMouseUp(PrimaryMouseEventArgs mouseEventArgs);
        void RootMouseWheel(PrimaryMouseEventArgs mouseEventArgs);
        void RootMouseMove(PrimaryMouseEventArgs mouseEventArgs);

        //---------------
        void RootGotFocus(UIFocusEventArgs e);
        void RootLostFocus(UIFocusEventArgs e);
        //
        void RootKeyPress(UIKeyEventArgs key);
        void RootKeyDown(UIKeyEventArgs key);
        void RootKeyUp(UIKeyEventArgs key);
        bool RootProcessDialogKey(UIKeyEventArgs key);

        //TODO: touch...

        Cursor RequestCursor { get; }
        MouseCursorStyle RequestCursorStyle { get; }
    }
    public interface ITopWindowEventRootProvider
    {
        ITopWindowEventRoot EventRoot { get; }
    }




    public enum UIMouseButtons
    {
        Left,
        Right,
        Middle,
        None
    }

}