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

    //public interface IUIElement
    //{
    //    bool AttachEventListener(IEventListener eventListener);
    //}
    //public interface IUIRootElement
    //{
    //    IEventListener CreateEventListener();
    //    IUIElement CreateElement(string elemName);
    //    void AddContent(IUIElement uiElement);
    //}

    /// <summary>
    /// can listen to some event
    /// </summary>
    public interface IEventListener
    {
        event UIEventHandler<UIMouseEventArgs> MouseDown;
        event UIEventHandler<UIMouseEventArgs> MouseUp;
        event UIEventHandler<UIMouseEventArgs> MouseMove;
        //
        event UIEventHandler<UIKeyEventArgs> KeyDown;
        event UIEventHandler<UIKeyEventArgs> KeyPress;
        event UIEventHandler<UIKeyEventArgs> KeyUp;


        //--------------------------------------------------------------------------
        void ListenKeyPress(UIKeyEventArgs e);
        void ListenKeyDown(UIKeyEventArgs e);
        void ListenKeyUp(UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs e);
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

    }

     
    public delegate void UIEventHandler<T>(T e) where T : System.EventArgs;
}