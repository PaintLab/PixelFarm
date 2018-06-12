//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.UI
{

    /// <summary>
    /// can listen to some event
    /// </summary>
    public interface IEventListener
    {


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
    public interface IUIEventListener : IEventListener
    {
        //-------------------------------------------------------------------------- 
        void HandleContentLayout();
        void HandleContentUpdate();
        void HandleElementUpdate();
        //--------------------------------------------------------------------------
        bool BypassAllMouseEvents { get; }
        bool AutoStopMouseEventPropagation { get; }
        void GetGlobalLocation(out int x, out int y);
        void GetViewport(out int x, out int y);
        //--------------------------------------------------------------------------  
    }

    public delegate void UIEventHandler<T>(T e);
}