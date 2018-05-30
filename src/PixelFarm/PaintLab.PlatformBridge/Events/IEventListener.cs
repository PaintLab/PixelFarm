//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.UI
{
    public interface IUIEventListener
    {
        //--------------------------------------------------------------------------
        void ListenKeyPress(UIKeyEventArgs args);
        void ListenKeyDown(UIKeyEventArgs e);
        void ListenKeyUp(UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs args);
        //--------------------------------------------------------------------------
        void ListenMouseDown(UIMouseEventArgs e);
        void ListenMouseMove(UIMouseEventArgs e);
        void ListenMouseUp(UIMouseEventArgs e);
        void ListenMouseLeave(UIMouseEventArgs e);
        void ListenMouseWheel(UIMouseEventArgs e);
        void ListenLostMouseFocus(UIMouseEventArgs e);
        //-------------------------------------------------------------------------- 
        void ListenMouseClick(UIMouseEventArgs e);
        void ListenMouseDoubleClick(UIMouseEventArgs e);
        //--------------------------------------------------------------------------
        void ListenGotKeyboardFocus(UIFocusEventArgs e);
        void ListenLostKeyboardFocus(UIFocusEventArgs e);
        //--------------------------------------------------------------------------  
        void ListenInterComponentMsg(object sender, int msgcode, string msg);
        void ListenGuestTalk(UIGuestTalkEventArgs e);
        //-------------------------------------------------------------------------- 
        void HandleContentLayout();
        void HandleContentUpdate();
        void HandleElementUpdate();
        //--------------------------------------------------------------------------
        bool BypassAllMouseEvents { get; }
        bool AutoStopMouseEventPropagation { get; }
        void GetGlobalLocation(out int x, out int y);
        //--------------------------------------------------------------------------  
    }

    public interface IUIElement
    {
        bool AttachEventListener(IEventListener eventListener);
    }
    public interface IUIRootElement
    {
        IEventListener CreateEventListener();
        IUIElement CreateElement(string elemName);
        void AddContent(IUIElement uiElement);
    }
    /// <summary>
    /// can listen to some event
    /// </summary>
    public interface IEventListener
    {
        event UIEventHandler<IEventArgs> MouseDown;
        event UIEventHandler<IEventArgs> MouseUp;
        event UIEventHandler<IEventArgs> MouseMove;
        //
        event UIEventHandler<IEventArgs> KeyDown;
        event UIEventHandler<IEventArgs> KeyPress;
        event UIEventHandler<IEventArgs> KeyUp;


        //--------------------------------------------------------------------------
        void ListenKeyPress(UIKeyEventArgs args);
        void ListenKeyDown(UIKeyEventArgs e);
        void ListenKeyUp(UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs args);
        //--------------------------------------------------------------------------
        void ListenMouseDown(UIMouseEventArgs e);
        void ListenMouseMove(UIMouseEventArgs e);
        void ListenMouseUp(UIMouseEventArgs e);
        void ListenMouseLeave(UIMouseEventArgs e);
        void ListenMouseWheel(UIMouseEventArgs e);
        void ListenLostMouseFocus(UIMouseEventArgs e);
        //-------------------------------------------------------------------------- 
        void ListenMouseClick(UIMouseEventArgs e);
        void ListenMouseDoubleClick(UIMouseEventArgs e);
        //--------------------------------------------------------------------------
        void ListenGotKeyboardFocus(UIFocusEventArgs e);
        void ListenLostKeyboardFocus(UIFocusEventArgs e);
        //--------------------------------------------------------------------------  
        void ListenInterComponentMsg(object sender, int msgcode, string msg);
        void ListenGuestTalk(UIGuestTalkEventArgs e);
        //-------------------------------------------------------------------------- 


        void HandleContentLayout();
        void HandleContentUpdate();
        void HandleElementUpdate();
    }



    public interface IEventArgs
    {
        IEventName EventName { get; }
        int X { get; }
        int Y { get; }
    }
    public enum IEventName
    {
        Custom,

        MouseDown,
        MouseMove,
        MouseUp,
        //Focus
        //
        KeyDown,
        KeyPress,
        KeyUp

    }

    public delegate void UIEventHandler<T>(T e)
        where T : IEventArgs;
}