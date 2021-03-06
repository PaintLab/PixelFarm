﻿//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{

    /// <summary>
    /// can listen to some event
    /// </summary>
    public interface IEventListener
    {
        void ListenKeyPress(UIKeyEventArgs e);
        void ListenKeyDown(UIKeyEventArgs e);
        void ListenKeyUp(UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs e);

        void ListenMouseDown(UIMouseDownEventArgs e);
        void ListenMouseMove(UIMouseMoveEventArgs e);
        void ListenMouseUp(UIMouseUpEventArgs e);
        void ListenMouseWheel(UIMouseWheelEventArgs e);

        void ListenMouseEnter(UIMouseMoveEventArgs e);
        void ListenMouseLeave(UIMouseLeaveEventArgs e);

        void ListenMouseHover(UIMouseHoverEventArgs e);

        void ListenLostMouseFocus(UIMouseLostFocusEventArgs e);
        void ListenMousePress(UIMousePressEventArgs e);

        void ListenMouseClick(UIMouseEventArgs e);
        void ListenMouseDoubleClick(UIMouseEventArgs e);

        void ListenGotKeyboardFocus(UIFocusEventArgs e);
        void ListenLostKeyboardFocus(UIFocusEventArgs e);

        void ListenGuestMsg(UIGuestMsgEventArgs e);

    }
    public interface IUIEventListener : IEventListener
    {
        void HandleElementUpdate();
        bool Enabled { get; set; }
        bool BypassAllMouseEvents { get; }
        bool AutoStopMouseEventPropagation { get; }
        void GetGlobalLocation(out int left, out int top);
        void GetViewport(out int left, out int top);
        bool AcceptKeyboardFocus { get; }
        bool DisableAutoMouseCapture { get; }

#if DEBUG
        void dbugDevWriteInfo();
#endif

    }

}