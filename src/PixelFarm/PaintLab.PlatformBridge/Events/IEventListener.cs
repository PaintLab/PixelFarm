//Apache2, 2014-present, WinterDev

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
        void ListenMouseClick(UIMouseEventArgs e);
        void ListenMouseLeave(UIMouseEventArgs e);
        void ListenMouseWheel(UIMouseEventArgs e);
        void ListenLostMouseFocus(UIMouseEventArgs e);
        //-------------------------------------------------------------------------- 
      
        void ListenMouseDoubleClick(UIMouseEventArgs e);
        //--------------------------------------------------------------------------
        void ListenGotKeyboardFocus(UIFocusEventArgs e);
        void ListenLostKeyboardFocus(UIFocusEventArgs e);
        //--------------------------------------------------------------------------  

        void ListenGuestMsg(UIGuestMsgEventArgs e);
        //-------------------------------------------------------------------------- 
        void ListenContinuousMousePress();
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
        void GetGlobalLocation(out int left, out int top);
        void GetViewport(out int left, out int top);
        bool AcceptKeyboardFocus { get; }
        bool DisableAutoMouseCapture { get; }
        //--------------------------------------------------------------------------  
    }

    public delegate void UIEventHandler<T>(T e);
    
}